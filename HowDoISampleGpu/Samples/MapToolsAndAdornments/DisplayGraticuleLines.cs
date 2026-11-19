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

        // The graticule is an adornment: it is drawn over the finished map in screen
        // space, as a grid of straight horizontal and vertical lines labelled with
        // degrees. That is only true of a map looked at straight down and facing
        // north. Turn the map and the lines no longer point along the meridians they
        // are labelled with; lean it and the spacing is wrong as well. The grid
        // cannot follow the camera, so on this sample the camera does not move.
        map.IsRotationEnabled = false;
        map.IsTiltEnabled = false;

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

        // Drawn above the GPU basemap by the classic pipeline, which is the point: the
        // two render paths compose.
        map.AdornmentOverlay.Layers.Add("graticule", graticuleLayer);

        map.CenterPoint = new PointShape(-10777200, 3911500);
        map.MapScale = 36200;


        _ = map.RefreshAsync();
    }


}
