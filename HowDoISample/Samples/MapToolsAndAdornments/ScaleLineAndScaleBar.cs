using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// The two scale adornments - a scale line and a scale bar - drawn over the GPU
/// basemap by the classic adornment pipeline.
/// </summary>
public class ScaleLineAndScaleBar : SamplePage
{
    /// <summary>
    /// The three systems a map is read in. Each one is set on both adornments at once.
    /// </summary>
    private static readonly (string Name, ScaleLineUnitSystem Line, UnitSystem Bar, string Reads)[] UnitSystems =
    {
        ("Metric", ScaleLineUnitSystem.Default, UnitSystem.Metric,
            "kilometres and metres, with miles underneath"),
        ("Imperial", ScaleLineUnitSystem.ImperialAndMetric, UnitSystem.Imperial,
            "miles and feet, with kilometres underneath"),
        ("Nautical", ScaleLineUnitSystem.NauticalMileAndImperial, UnitSystem.NauticalMile,
            "nautical miles, with statute miles underneath"),
    };

    /// <summary>
    /// The plate both adornments stand on: all but opaque, so the map cannot come
    /// through it, with an edge dark enough to hold its shape over pale ground.
    /// </summary>
    private static AreaStyle Plate => AreaStyle.CreateSimpleAreaStyle(
        GeoColor.FromArgb(236, 252, 252, 250), GeoColor.FromArgb(130, 24, 26, 30));

    private ScaleLineAdornmentLayer _scaleLine;
    private ScaleBarAdornmentLayer _scaleBar;
    private MapView _map;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Dark,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            {
                VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
            }));

        // The map's own projection is what makes the number true. Without it the
        // adornment measures the distance across the screen in web-mercator metres,
        // which at this latitude is 19% longer than the ground it covers; with it,
        // the two ends of the bar are converted to lat/lon and the distance between
        // them is taken on the ellipsoid.
        var projection = new Projection(3857);

        // Both along the left, one under the other, so they read as two answers to
        // the same question.
        _scaleLine = new ScaleLineAdornmentLayer
        {
            Location = AdornmentLocation.UpperLeft,

            // In from the edge: the scale line draws its boxes from the adornment's
            // origin, so at zero the "500 m" box starts off the screen and reads
            // "00 m".
            XOffsetInPixel = 12,
            YOffsetInPixel = 60,
            Projection = projection,

            // A light plate: both adornments draw in black by default, and a light
            // plate reads over a light or a dark basemap.
            BackgroundMask = Plate,

            // Black on the plate, with a white halo under it.
            Pen = new GeoPen(GeoColor.FromArgb(235, 16, 18, 22), 2),
            HaloPen = new GeoPen(GeoColor.FromArgb(210, 255, 255, 255), 5),

            // In points. The default would run a scale line most of the way across a
            // phone.
            MaxBarLength = 110,
        };

        // The label styles are left at their default: black, which reads on the plate.

        _scaleBar = new ScaleBarAdornmentLayer
        {
            Location = AdornmentLocation.UpperLeft,
            XOffsetInPixel = 12,

            // Below the scale line's plate, with a gap the width of the plates' own
            // padding.
            YOffsetInPixel = 104,
            Projection = projection,
            BackgroundMask = Plate,

            // Half the default: the scale bar labels every tick, and at the default
            // width it would reach across the phone. The unit - KM, MI - is written
            // after the last tick and needs room inside the plate.
            MaxWidth = 120,
        };

        map.AdornmentOverlay.Layers.Add(_scaleLine);
        map.AdornmentOverlay.Layers.Add(_scaleBar);

        AddChoiceStrip(UnitSystems.Select(u => u.Name).ToList(), 0, Show);
        _status = AddStatusLabel();
        Show(0);

        map.CenterPoint = new PointShape(-10778000, 3912000);
        map.MapScale = 77000;
        _ = map.RefreshAsync();
    }

    /// <summary>One system, set on both adornments and said in words underneath.</summary>
    private void Show(int index)
    {
        var chosen = UnitSystems[index];
        _scaleLine.UnitSystem = chosen.Line;
        _scaleBar.UnitFamily = chosen.Bar;
        _status.Text = "Measured in " + chosen.Reads;
        _ = _map.AdornmentOverlay.RefreshAsync();
    }
}
