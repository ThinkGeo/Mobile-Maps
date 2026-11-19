using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The smallest thing that proves the chain: a MAUI MapView whose basemap is drawn
/// by the GPU pipeline. Everything specific to that is the one overlay added below -
/// the map view, the camera and the gestures are the ordinary MAUI ones.
/// </summary>
public class FirstLight : ContentPage
{
    private const string StyleUri = "https://tiles.preludemaps.com/styles/WorldStreets_Light/style.json";

    // Manhattan, in spherical mercator metres - the view the desktop parity work uses.
    private static readonly PointShape HomeCenter = new(-8237642d, 4976252d);

    public FirstLight()
    {
        Title = "First Light";
        Shell.SetNavBarIsVisible(this, false);

        var map = new MapView
        {
            MapUnit = GeographyUnit.Meter,
            CenterPoint = HomeCenter,
            MapScale = new ThinkGeoCloudMapsZoomLevelSet().GetScales()[13],
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };

        map.Overlays.Add("Basemap", new GpuStyleOverlay(StyleUri));

        // The map draws itself once it first has a size. Without this nothing is
        // ever asked to draw, and the page comes up empty with nothing in the log.
        map.Behaviors.Add(new AutoLoadMapViewBehavior());

        Content = map;
    }
}
