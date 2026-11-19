using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            // Registers the GPU map surface's handler. MAUI resolves a view to its
            // native peer through the handler registry, and a view whose handler was
            // never registered fails at layout time with nothing on screen and
            // nothing in the log - so this line is not optional.
            .UseThinkGeoGpu()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder.Build();
    }
}
