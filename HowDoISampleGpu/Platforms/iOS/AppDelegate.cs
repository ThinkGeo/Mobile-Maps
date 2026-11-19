using Foundation;
using ThinkGeo.Gpu;

namespace HowDoISampleGpu;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    // No backend selection here: ThinkGeo.Gpu.iOS defaults to ANGLE over Metal,
    // and the deprecated EAGL path is compiled out unless THINKGEO_GPU_EAGL is
    // defined. The choice is process-wide and must precede the first view, which
    // CreateMauiApp satisfies.
    protected override MauiApp CreateMauiApp()
    {
        // Without this the engine's Trace output goes nowhere a device or simulator
        // log can read, and a map that fails to come up says nothing at all.
        GpuIosDiagnostics.RouteTraceToConsole();
        return MauiProgram.CreateMauiApp();
    }
}
