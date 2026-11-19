using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// A latitude and longitude grid over the map, with its degree labels pinned to the
/// map's edges.
/// </summary>
public class DisplayGraticuleLines : SamplePage
{
    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Light,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            {
                VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
            }));

        // The graticule is computed in EPSG:4326; the projection tells it the map is
        // in EPSG:3857.
        var graticuleLayer = new GraticuleAdornmentLayer
        {
            Projection = new Projection(3857),
        };
        graticuleLayer.GraticuleLineStyle.OuterPen.Color = GeoColor.FromArgb(125, GeoColors.Navy);

        // On the adornment overlay the grid is redrawn on every pan tick and the degree
        // labels stay pinned to the map's edges - it is drawn above the GPU basemap by
        // the classic pipeline, which is the point: the two render paths compose.
        map.AdornmentOverlay.Layers.Add("graticule", graticuleLayer);

        map.CenterPoint = new PointShape(-10777200, 3911500);
        map.MapScale = 36200;
        _ = map.RefreshAsync();
    }
}
