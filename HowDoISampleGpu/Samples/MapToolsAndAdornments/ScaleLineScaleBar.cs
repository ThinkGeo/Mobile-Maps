using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The two scale adornments - a scale line and a scale bar - drawn over the GPU
/// basemap by the classic adornment pipeline.
/// </summary>
public class ScaleLineScaleBar : SamplePage
{
    /// <summary>
    /// The three systems a map is read in, each setting both adornments at once.
    /// <para>
    /// The strip used to offer the scale line's four pairings - imperial over
    /// metric, metric over imperial, metric over nautical, nautical over imperial -
    /// and nothing about tapping along it said what it was for. It changed one of
    /// the two adornments, it changed it by swapping which of two bars was on top,
    /// and two of the four cells differed only in the smaller bar. What a reader
    /// wants from this strip is the question it actually answers: what units is this
    /// map measured in? So each cell now names one system and sets it on both - the
    /// scale bar's whole set of numbers changes with it, which is a difference
    /// nobody has to hunt for.
    /// </para>
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

        // The map's own projection is what makes the number true rather than
        // plausible. Without it the adornment measures the distance across the screen
        // in web-mercator metres, which at this latitude is 19% longer than the ground
        // it covers; with it, the two ends of the bar are converted to lat/lon and the
        // distance between them taken on the ellipsoid. A scale bar that is wrong by a
        // fifth is worse than none.
        var projection = new Projection(3857);

        // One in each top corner. They were stacked in the same corner, 58 pixels
        // apart, which is close enough that they read as one broken control.
        _scaleLine = new ScaleLineAdornmentLayer
        {
            Location = AdornmentLocation.UpperLeft,

            // In from the edge: the scale line draws its boxes from the adornment's
            // origin, so at zero the "500 m" box starts off the screen and reads
            // "00 m".
            XOffsetInPixel = 12,
            YOffsetInPixel = 60,
            Projection = projection,

            // Ink on paper, and the paper nearly opaque. The plate here was a dark
            // translucent one, which on a dark basemap is the basemap: the plate
            // disappeared into it, and inside the plate both adornments draw in
            // black by default - the scale line's labels, the scale bar's bars and
            // every one of its numbers - so the parts that were not the one white
            // line disappeared as well. A light plate is the one choice that reads
            // against either theme AND agrees with the ink the layers already use,
            // which is why every mobile map that draws a plate at all draws a light
            // one.
            BackgroundMask = Plate,

            // Black on the plate, with a white halo under it so the line survives
            // the moment the plate is taken away.
            Pen = new GeoPen(GeoColor.FromArgb(235, 16, 18, 22), 2),
            HaloPen = new GeoPen(GeoColor.FromArgb(210, 255, 255, 255), 5),

            // In points, like every other number here, so it means the same share of
            // a phone screen as of a desktop window. The default is a desktop number
            // and would run a scale line most of the way across a phone.
            MaxBarLength = 110,
        };

        // The label styles are left alone - black, which is what reads on the plate
        // - and now that is a choice rather than a
        // workaround. They used to come out tiny, and raising the font size made the
        // label vanish altogether - which looked like a bug in the label path and was
        // not: the MAUI canvas records at the screen's density but was handing the
        // font and the pen through at face value, so the text was measured for one
        // size and drawn at a third of it. The canvas scales them now, the way the
        // Skia canvas always has, and the default twelve points reads as twelve.

        _scaleBar = new ScaleBarAdornmentLayer
        {
            Location = AdornmentLocation.UpperRight,
            XOffsetInPixel = -12,
            YOffsetInPixel = 60,
            Projection = projection,
            BackgroundMask = Plate,

            // Half the default. The scale bar labels every tick, so what it costs in
            // width it spends on numbers; at the desktop default it reached from one
            // side of the phone to the other and met the scale line coming the other
            // way. 120 rather than 140 because the unit - KM, MI - is written after
            // the last tick, and at 140 it was written on the edge of the plate.
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
