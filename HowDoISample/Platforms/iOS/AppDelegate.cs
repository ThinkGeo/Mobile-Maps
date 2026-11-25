using Foundation;
using ThinkGeo.Gpu;

namespace HowDoISample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp()
    {
        // Routes the engine's Trace output to the console, where a device or
        // simulator log can read it.
        GpuIosDiagnostics.RouteTraceToConsole();
        return MauiProgram.CreateMauiApp();
    }
}
