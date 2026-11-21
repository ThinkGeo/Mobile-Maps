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
    private static readonly (string Name, ScaleLineUnitSystem System)[] UnitSystems =
    {
        // Top unit first, bottom unit second - which is all that tells the four
        // of them apart.
        ("Imperial / metric", ScaleLineUnitSystem.ImperialAndMetric),
        ("Metric / imperial", ScaleLineUnitSystem.Default),
        ("Metric / nautical", ScaleLineUnitSystem.MetricAndNauticalMile),
        ("Nautical / imperial", ScaleLineUnitSystem.NauticalMileAndImperial),
    };

    private ScaleLineAdornmentLayer _scaleLine;
    private MapView _map;

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
            UnitSystem = ScaleLineUnitSystem.ImperialAndMetric,

            // A dark translucent plate instead of the light blue box with a red
            // border it had, which is a debugging colour scheme and the reason it
            // looked like a sticker pasted on the map. Mobile maps either draw this
            // with no plate at all and a halo under the glyphs, or with a quiet dark
            // one; a quiet dark one is what survives both a light and a dark basemap.
            // A quiet dark plate, not the light blue box with a red border it had -
            // that is a debugging colour scheme, and the reason it read as a sticker
            // pasted on the map.
            BackgroundMask = AreaStyle.CreateSimpleAreaStyle(
                GeoColor.FromArgb(150, 18, 20, 24), GeoColor.FromArgb(70, 255, 255, 255)),
            Pen = new GeoPen(GeoColors.White, 2),
            HaloPen = new GeoPen(GeoColor.FromArgb(190, 0, 0, 0), 5),

            // In points, like every other number here, so it means the same share of
            // a phone screen as of a desktop window. The default is a desktop number
            // and would run a scale line most of the way across a phone.
            MaxBarLength = 110,
        };

        // The label styles are left alone, and now that is a choice rather than a
        // workaround. They used to come out tiny, and raising the font size made the
        // label vanish altogether - which looked like a bug in the label path and was
        // not: the MAUI canvas records at the screen's density but was handing the
        // font and the pen through at face value, so the text was measured for one
        // size and drawn at a third of it. The canvas scales them now, the way the
        // Skia canvas always has, and the default twelve points reads as twelve.

        var scaleBar = new ScaleBarAdornmentLayer
        {
            Location = AdornmentLocation.UpperRight,
            XOffsetInPixel = -12,
            YOffsetInPixel = 60,
            Projection = projection,
            BackgroundMask = AreaStyle.CreateSimpleAreaStyle(
                GeoColor.FromArgb(150, 18, 20, 24), GeoColor.FromArgb(70, 255, 255, 255)),

            // Half the default. The scale bar labels every tick, so what it costs in
            // width it spends on numbers; at the desktop default it reached from one
            // side of the phone to the other and met the scale line coming the other
            // way.
            MaxWidth = 140,
        };

        map.AdornmentOverlay.Layers.Add(_scaleLine);
        map.AdornmentOverlay.Layers.Add(scaleBar);

        AddChoiceStrip(
            UnitSystems.Select(u => u.Name).ToList(), 0,
            index =>
            {
                _scaleLine.UnitSystem = UnitSystems[index].System;
                _ = _map.AdornmentOverlay.RefreshAsync();
            });

        map.CenterPoint = new PointShape(-10778000, 3912000);
        map.MapScale = 77000;
        _ = map.RefreshAsync();
    }

}
