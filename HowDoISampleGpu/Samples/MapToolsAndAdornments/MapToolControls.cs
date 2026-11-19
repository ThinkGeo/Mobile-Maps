using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The controls the map itself offers: a coordinate readout, zoom buttons, and a logo
/// in the corner.
/// </summary>
/// <remarks>
/// WPF reaches them as named properties - MapTools.MouseCoordinate, MapTools.PanZoomBar,
/// MapTools.Logo. MAUI's MapTools is a collection of IMapTool, so the same three are
/// added rather than switched on, and the first two have mobile counterparts:
/// CoordinateMapTool follows the finger rather than a mouse, and ZoomMapTool is two
/// buttons rather than a pan-and-zoom bar, because panning is what a finger is for.
/// There is no logo tool, so the logo is Core's LogoAdornmentLayer, which draws in
/// every platform.
/// </remarks>
public class MapToolControls : SamplePage
{
    private static readonly string[] Formats = { "Longitude and latitude", "Web mercator metres" };

    private CoordinateMapTool _coordinates;

    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Light,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            { VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder) }));

        map.CenterPoint = new PointShape(-10778000, 3912000);
        map.MapScale = 77000;

        // The readout: the map is in web mercator, so a converter is what turns the
        // reading into degrees.
        _coordinates = new CoordinateMapTool
        {
            FontSize = 16,
            TextColor = Colors.Black,
            BackgroundColor = Color.FromRgba(255, 255, 255, 200),
            Padding = new Thickness(8, 4),
            ProjectionConverter = new ProjectionConverter(3857, 4326),
            DisplayTextFormat = "{0:N5}, {1:N5}",
        };
        map.MapTools.Add(_coordinates);
        var zoom = new ZoomMapTool();
        map.MapTools.Add(zoom);

        // A tool is placed from its own DefaultLocation when it is added. The zoom
        // tool's is the top-left, which is where this gallery puts its control card,
        // so this one is moved to the right-hand edge after the fact - the same
        // AbsoluteLayout the collection used.
        AbsoluteLayout.SetLayoutBounds(zoom, new Rect(0.98, 0.45, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
        AbsoluteLayout.SetLayoutFlags(zoom, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.PositionProportional);

        map.AdornmentOverlay.Layers.Add(new LogoAdornmentLayer(
            new GeoImage(Path.Combine(SampleShared.DataFolder, "Images", "generic-logo.png")))
        {
            Location = AdornmentLocation.LowerRight,
        });

        AddChoicePicker(Formats, 0, index =>
        {
            if (index == 0)
            {
                _coordinates.ProjectionConverter = new ProjectionConverter(3857, 4326);
                _coordinates.DisplayTextFormat = "{0:N5}, {1:N5}";
            }
            else
            {
                _coordinates.ProjectionConverter = null;
                _coordinates.DisplayTextFormat = "X: {0:N2}, Y: {1:N2}";
            }
        });

        AddStatusLabel().Text = "Touch the map to move the readout";
        _ = map.RefreshAsync();
    }
}
