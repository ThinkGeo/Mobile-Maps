using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOnlineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DisplayOpenStreetMap
{
    private bool _initialized;
    public DisplayOpenStreetMap()
    {
        InitializeComponent();
    }

    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (_initialized)
            return;
        _initialized = true;

        // It is important to set the map unit first to either feet, meters or decimal degrees.
        MapView.MapUnit = GeographyUnit.Meter;

        var layerOverlay = new OpenStreetMapsOverlay("ThinkGeo Samples");
        MapView.Overlays.Add(layerOverlay);

        // Set the map scale and center point
        MapView.MapScale = 35_000;
        MapView.CenterPoint = new PointShape(-10777932, 3912260);
        await MapView.RefreshAsync();
    }
}