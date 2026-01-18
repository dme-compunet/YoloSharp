namespace Compunet.YoloSharp.Services;

internal class SessionRunnerService(YoloSession yoloSession,
                                    YoloConfiguration configuration,
                                    IPixelsNormalizerService normalizer,
                                    IMemoryAllocatorService memoryAllocator) : ISessionRunnerService
{
    private readonly object _lock = new();
    private readonly RunOptions _options = new();

    public InferenceSession Session => yoloSession.Session;

    public SessionIoShapeInfo IoShapeInfo => yoloSession.ShapeInfo;

    public IYoloRawOutput PreprocessAndRun(Image<Rgb24> image, out PredictorTimer timer)
    {
        // Create timer
        timer = new PredictorTimer();

        // Start pre-process timer
        timer.StartPreprocess();

        // Allocate the input tensor
        using var input = memoryAllocator.AllocateTensor<float>(IoShapeInfo.Input0, true);

        // Preprocess image to tensor and bind to ort binding
        NormalizeInput(image, input.Tensor);

        // Start inference timer
        timer.StartInference();

        if (IoShapeInfo.IsDynamicOutput)
        {
            return RunWithDynamicOutput(input.Tensor);
        }
        else
        {
            return RunWithFixedOutput(input.Tensor);
        }
    }

    private YoloRawOutput RunWithFixedOutput(MemoryTensor<float> input)
    {
        // Create io binding
        using var binding = Session.CreateIoBinding();

        // Create ort input value
        using var ortInput = CreateOrtValue(input);

        // Bind the input
        binding.BindInput(Session.InputNames[0], ortInput);

        // Create and bind raw output
        using var _ = CreateRawOutput(binding, out var output);

        // Run the model
        if (configuration.SuppressParallelInference)
        {
            lock (_lock)
            {
                Session.RunWithBinding(_options, binding);
            }
        }
        else
        {
            Session.RunWithBinding(_options, binding);
        }

        // Return the yolo raw output
        return output;
    }

    private OrtYoloRawOutput RunWithDynamicOutput(MemoryTensor<float> input)
    {
        var inputs = new NamedOnnxValue[]
        {
            NamedOnnxValue.CreateFromTensor(Session.InputNames[0], new DenseTensor<float>(input.Buffer, input.Dimensions))
        };

        OrtYoloRawOutput Run()
        {
            var result = Session.Run(inputs);

            return new OrtYoloRawOutput(result);
        }

        if (configuration.SuppressParallelInference)
        {
            lock (_lock)
            {
                return Run();
            }
        }
        else
        {
            return Run();
        }
    }

    private CompositeDisposable CreateRawOutput(OrtIoBinding binding, out YoloRawOutput output)
    {
        var output0Info = IoShapeInfo.Output0;
        var output1Info = IoShapeInfo.Output1;

        // Allocate output0 tensor buffer
        var output0 = memoryAllocator.AllocateTensor<float>(output0Info);

        // Create ort output0 value
        var ortOutput0 = CreateOrtValue(output0.Tensor);

        // Bind tensor buffer to ort binding
        binding.BindOutput(Session.OutputNames[0], ortOutput0);

        if (output1Info != null)
        {
            // Allocate output1 tensor buffer
            var output1 = memoryAllocator.AllocateTensor<float>(output1Info.Value);

            // Create ort output0 value
            var ortOutput1 = CreateOrtValue(output1.Tensor);

            // Bind tensor buffer to ort binding
            binding.BindOutput(Session.OutputNames[1], ortOutput1);

            output = new YoloRawOutput(output0, output1);

            return new CompositeDisposable([ortOutput0, ortOutput1]);
        }
        else
        {
            output = new YoloRawOutput(output0, null);

            return new CompositeDisposable([ortOutput0]);
        }
    }

    #region Preprocess

    private void NormalizeInput(Image<Rgb24> image, MemoryTensor<float> target)
    {
        // Apply auto orient if required
        if (configuration.ApplyAutoOrient)
        {
            image.AutoOrient();
        }

        // Resize the input image
        using var resized = ResizeImage(image, out var padding);

        // Process the image to tensor
        normalizer.NormalizerPixelsToTensor(resized, target, padding);
    }

    private Image<Rgb24> ResizeImage(Image<Rgb24> image, out Vector<int> padding)
    {
        // Get the model image input size
        var inputSize = yoloSession.Metadata.ImageSize;

        // Create resize options
        var options = new ResizeOptions()
        {
            Size = inputSize,

            // Select resize mode according to 'KeepAspectRatio'
            Mode = configuration.KeepAspectRatio
                   ? ResizeMode.Max
                   : ResizeMode.Stretch,

            // Select faster resampling algorithm
            Sampler = KnownResamplers.NearestNeighbor
        };

        // Create resized image
        var resized = image.Clone(x => x.Resize(options));

        // Calculate padding
        padding =
        (
            (inputSize.Width - resized.Size.Width) / 2,
            (inputSize.Height - resized.Size.Height) / 2
        );

        // Return the resized image
        return resized;
    }

    #endregion

    private static OrtValue CreateOrtValue(MemoryTensor<float> tensor)
    {
        return CreateOrtValue(tensor.Buffer, tensor.Dimensions64);
    }

    private static OrtValue CreateOrtValue(Memory<float> buffer, long[] shape)
    {
        return OrtValue.CreateTensorValueFromMemory(OrtMemoryInfo.DefaultInstance, buffer, shape);
    }
}