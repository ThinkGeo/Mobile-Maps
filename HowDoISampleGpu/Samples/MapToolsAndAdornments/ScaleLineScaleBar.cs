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
        ("Imperial and metric", ScaleLineUnitSystem.ImperialAndMetric),
        ("Metric on top, imperial below", ScaleLineUnitSystem.Default),
        ("Metric and nautical miles", ScaleLineUnitSystem.MetricAndNauticalMile),
        ("Nautical miles and imperial", ScaleLineUnitSystem.NauticalMileAndImperial),
    };

    private ScaleLineAdornmentLayer _scaleLine;
    private MapView _map;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        // Both adornments draw at the bottom-left, which is where the control card
        // sits by default - so the card moves rather than covering the thing the
        // sample is about.
        PutPanelAtTop();

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Dark,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            {
                VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
            }));

        // The map's own projection lets the adornments convert the bar's ends to
        // lat/lon and measure the ground distance between them.
        var projection = new Projection(3857);

        _scaleLine = new ScaleLineAdornmentLayer
        {
            Projection = projection,
            BackgroundMask = AreaStyle.CreateSimpleAreaStyle(GeoColors.LightBlue, GeoColors.Red),
            UnitSystem = ScaleLineUnitSystem.ImperialAndMetric,
        };

        var scaleBar = new ScaleBarAdornmentLayer
        {
            YOffsetInPixel = -50,
            Projection = projection,
            BackgroundMask = AreaStyle.CreateSimpleAreaStyle(GeoColors.LightBlue, GeoColors.Red),
        };

        map.AdornmentOverlay.Layers.Add(_scaleLine);
        map.AdornmentOverlay.Layers.Add(scaleBar);

        AddChoicePicker(
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
