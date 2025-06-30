namespace MyProject;

public sealed class App : Application
{
    private Window _window = null!;
    private GpuDevice _device = null!;

    protected override void OnStart()
    {
        // Called when the application is starting.

        // Create the window.
        using var windowOptions = new WindowOptions();
        windowOptions.Title = "MyProject";
        windowOptions.Width = 640;
        windowOptions.Height = 480;
        windowOptions.IsResizable = true;
        _window = CreateWindow(windowOptions);

        // Create the GPU device.
        using var gpuDeviceOptions = new GpuDeviceOptions();
        gpuDeviceOptions.IsLowPowerMode = false;
        gpuDeviceOptions.IsDebugMode = true;
        _device = CreateGpuDevice(gpuDeviceOptions);

        // Try to associate the window to the GPU device.
        if (!_device.TryClaimWindow(_window))
        {
            // If we failed, just exit.
            Exit();
        }
    }

    protected override void OnExit()
    {
        // Called when the application is exiting.

        // Clean up resources associated with the GPU device.
        _device.Dispose();
        _device = null!;

        // Clean up resources associated with the window.
        _window.Dispose();
        _window = null!;
    }

    protected override void OnEvent(in SDL.SDL_Event e)
    {
        // Called when the application is to process an event such as a keyboard event or mouse event.
    }

    protected override void OnUpdate(TimeSpan deltaTime)
    {
        // Called when the application is to do some processing which is not related to rendering.
    }

    protected override void OnDraw(TimeSpan deltaTime)
    {
        // Called when the application is to do some rendering.

        var commandBuffer = _device.GetCommandBuffer();
        if (!commandBuffer.TryGetSwapchainTexture(_window, out var swapchainTexture))
        {
            commandBuffer.Cancel();
            return;
        }

        var renderTargetInfoColor = default(GpuRenderTargetInfoColor);
        renderTargetInfoColor.Texture = swapchainTexture!;
        renderTargetInfoColor.LoadOp = GpuRenderTargetLoadOp.Clear;
        renderTargetInfoColor.StoreOp = GpuRenderTargetStoreOp.Store;
        renderTargetInfoColor.ClearColor = Rgba32F.CornflowerBlue;

        // Begin the on screen render pass.
        var renderPass = commandBuffer.BeginRenderPass(null, renderTargetInfoColor);

        // Add your screen rendering code here!

        // Ends the on screen render pass.
        renderPass.End();

        commandBuffer.Submit();
    }
}
