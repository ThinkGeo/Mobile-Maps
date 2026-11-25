using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// The two adornments that frame a map rather than sit on it: a latitude and
/// longitude grid labelled down the edges, and a legend whose swatches are the very
/// styles that paint the states.
/// </summary>
/// <remarks>
/// Both are drawn over the finished GPU basemap by the classic adornment pipeline:
/// the two render paths compose, and the adornments redraw with the gesture.
/// </remarks>
public class GraticuleLinesAndLegend : SamplePage
{
    /// <summary>
    /// The lower forty-eight, in web mercator. Not the shapefile's own bounding box:
    /// that one carries Alaska and Hawaii, and would open on a third of a hemisphere.
    /// </summary>
    private static readonly RectangleShape LowerFortyEight =
        new(-13914936, 6343008, -7402746, 2802226);

    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;

        // A graticule is a grid of straight horizontal and vertical lines labelled with
        // degrees, which is only true of a map looked at straight down and facing north.
        // The grid cannot follow the camera, so here the camera does not rotate or tilt.
        map.IsRotationEnabled = false;
        map.IsTiltEnabled = false;

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Light,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            {
                VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
            }));

        var states = new ShapeFileFeatureLayer(SampleShared.Shapefile("USStates_3857.shp"));
        var classes = new ClassBreakStyle("POP1990");
        var breaks = new[] { 0d, 2_000_000d, 6_000_000d, 12_000_000d };
        var colors = new[]
        {
            GeoColors.PastelBlue, GeoColors.LightBlue, GeoColors.MediumBlue, GeoColors.DarkBlue,
        };

        var legend = new LegendAdornmentLayer
        {
            Title = new LegendItem
            {
                TextStyle = new TextStyle("Population 1990",
                    new GeoFont("Verdana", 10, DrawingFontStyles.Bold), GeoBrushes.Black),
            },

            // The bottom left corner: the bottom right one belongs to the basemap's
            // attribution, and the graticule labels its meridians along the top edge.
            Location = AdornmentLocation.LowerLeft,
            XOffsetInPixel = 12,
            YOffsetInPixel = -12,
        };

        for (var i = 0; i < breaks.Length; i++)
        {
            // The legend item and the map share one style object: a swatch that is
            // built separately is a swatch that can lie.
            var areaStyle = new AreaStyle(GeoPens.DimGray, new GeoSolidBrush(colors[i]));
            classes.ClassBreaks.Add(new ClassBreak(breaks[i], areaStyle));

            legend.LegendItems.Add(new LegendItem
            {
                ImageStyle = areaStyle,
                TextStyle = new TextStyle(
                    i == breaks.Length - 1
                        ? FormattableString.Invariant($"over {breaks[i] / 1_000_000:0}M")
                        : FormattableString.Invariant($"{breaks[i] / 1_000_000:0}M to {breaks[i + 1] / 1_000_000:0}M"),
                    new GeoFont("Verdana", 9), GeoBrushes.Black),
            });
        }

        states.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(classes);
        states.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var overlay = new LayerOverlay();
        overlay.Layers.Add(states);
        map.Overlays.Add(overlay);

        // The graticule is computed in EPSG:4326; the projection tells it the map is
        // in EPSG:3857.
        var graticuleLayer = new GraticuleAdornmentLayer
        {
            Projection = new Projection(3857),
        };

        // Pen widths and type sizes are in points. The layer's default half-point
        // hairline is too faint on a phone, so the line is given a width.
        graticuleLayer.GraticuleLineStyle.OuterPen.Color = GeoColor.FromArgb(150, GeoColors.Navy);
        graticuleLayer.GraticuleLineStyle.OuterPen.Width = 1f;
        graticuleLayer.GraticuleTextFont = new GeoFont("Noto Sans", 11f);

        map.AdornmentOverlay.Layers.Add("graticule", graticuleLayer);
        map.AdornmentOverlay.Layers.Add("legend", legend);

        OpenOn(map, LowerFortyEight);
        _ = map.RefreshAsync();
    }
}
