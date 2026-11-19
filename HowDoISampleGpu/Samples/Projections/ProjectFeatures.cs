using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Three ways a feature gets from one projection to another: the source converting
/// as it reads, a converter run over the geometry by hand, and the ThinkGeo Cloud
/// doing it.
/// </summary>
public class ProjectFeatures : SamplePage
{
    private const string SubdivisionsStyle = """
    {
      "layers": [
        { "type": "fill",
          "source-layer": "subdivisions",
          "paint": {
              "fill-color": "rgba(147,112,219,0.2)",
              "fill-outline-color": "#9370DB" }
        }
      ]
    }
    """;

    /// <summary>Three places in longitude and latitude, for the two explicit modes.</summary>
    private static readonly string[] SampleWkt =
    {
        "POINT(-96.834530 33.150076)",
        "POINT(-96.820000 33.160000)",
        "POINT(-96.850000 33.140000)",
    };

    private static readonly string[] Modes =
    {
        "The source converts as it reads",
        "A converter run over the geometry",
        "The ThinkGeo Cloud converts",
    };

    private readonly ThinkGeoVectorTileSource _cloud = new(SampleShared.CloudApiKey)
    {
        VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
    };

    private readonly FeatureSourceVectorTileSource _tiles = new();
    private readonly DynamicSource _projected = new();

    private readonly ProjectionCloudClient _projectionCloud = new()
    {
        ClientId = SampleShared.ClientId,
        ClientSecret = SampleShared.ClientSecret,
    };

    private RectangleShape _subdivisionsBounds;
    private MapView _map;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        // The converter belongs to the source, so it applies wherever the source is
        // read - the tile cutter included.
        var subdivisions = new ShapeFileFeatureSource(SampleShared.Shapefile("Subdivisions.shp"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        };

        // Nothing is converted by hand: the extent the source reports, like every
        // feature it reads, is already in web mercator. It is read before the tile
        // cutter starts reading the same source on its own threads.
        subdivisions.Open();
        _subdivisionsBounds = subdivisions.GetBoundingBox();
        subdivisions.Close();
        _tiles.FeatureSources.Add("subdivisions", subdivisions);

        AddChoicePicker(Modes, 0, index => _ = ShowAsync(index));
        _status = AddStatusLabel();

        _ = ShowAsync(0);
    }

    private async Task ShowAsync(int index)
    {
        try
        {
            var fromSource = index == 0;
            var style = new MapStyle(ThinkGeoVectorStyles.Dark, _cloud);

            if (fromSource)
            {
                style.AddStyle(SubdivisionsStyle, _tiles);
            }
            else
            {
                // The star the points are drawn with, registered under the name they ask for.
                style.Images.Add("star", new PointStyle(PointSymbolType.Star, 24, GeoBrushes.MediumPurple, GeoPens.Purple));
                style.AddDynamic(_projected);
            }

            if (_map.Basemap == null) _map.Basemap = new GpuBasemap(style);
            else await _map.Basemap.SetStyleAsync(style);

            if (fromSource)
            {
                var bounds = (RectangleShape)_subdivisionsBounds.CloneDeep();
                bounds.ScaleUp(50);
                _map.CurrentExtent = bounds;
                _status.Text = "The shapefile is in EPSG:2276; its ProjectionConverter reports web mercator";
                await _map.RefreshAsync();
                return;
            }

            var inDegrees = new Collection<Feature>();
            foreach (var wkt in SampleWkt) inDegrees.Add(new Feature(wkt));

            _status.Text = index == 2 ? "asking the ThinkGeo Cloud…" : "converting…";
            var converted = index == 2
                ? await _projectionCloud.ProjectAsync(inDegrees, 4326, 3857)
                : ConvertTheGeometryExplicitly(inDegrees);

            _projected.Clear();
            RectangleShape framed = null;
            for (var i = 0; i < converted.Count; i++)
            {
                _projected.SetPoint("point-" + i, converted[i], sizeInPixel: 24f, iconName: "star");
                var box = converted[i].GetBoundingBox();
                if (framed == null) framed = box;
                else framed.ExpandToInclude(box);
            }

            if (framed != null)
            {
                _map.CenterPoint = framed.GetCenterPoint();
                _map.MapScale = 50000;
            }

            _status.Text = $"{converted.Count} points converted from EPSG:4326 to EPSG:3857";
        }
        catch (Exception exception)
        {
            _status.Text = exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }

    private static Collection<Feature> ConvertTheGeometryExplicitly(IEnumerable<Feature> decimalDegreeFeatures)
    {
        var projectionConverter = new ProjectionConverter(4326, 3857);
        projectionConverter.Open();
        var sphericalMercatorFeatures = projectionConverter.ConvertToExternalProjection(decimalDegreeFeatures);
        projectionConverter.Close();

        return sphericalMercatorFeatures;
    }
}
