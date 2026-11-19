using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The three shapes of vector data, each with its own style: parks as fills,
/// streets as lines, hotels as circles - and a label layer over each.
/// </summary>
public class RenderPointsLinesAndPolygons : SamplePage
{
    private const string StyleJson = """
    {
      "layers": [

        { "type": "background",
          "paint": { "background-color": "#1E2329" } },

        { "type": "fill",
          "source-layer": "Parks",
          "paint": {
              "fill-color": "#2E4A3B",
              "fill-outline-color": "#4F7A5F" }
        },

        { "type": "line",
          "source-layer": "Streets",
          "paint": {
              "line-color": "#13171B",
              "line-width": 6 }
        },

        { "type": "line",
          "source-layer": "Streets",
          "paint": {
              "line-color": "#505A66",
              "line-width": 4 }
        },

        { "type": "circle",
          "source-layer": "Hotels",
          "paint": {
              "circle-radius": 4.5,
              "circle-color": "#F2A65A",
              "circle-stroke-color": "#1E2329",
              "circle-stroke-width": 1 }
        },

        { "type": "symbol",
          "source-layer": "Streets",
          "layout": {
              "symbol-placement": "line",
              "text-field": ["get", "FULL_NAME"],
              "text-font": ["Noto Sans Bold"],
              "text-size": 11 },
          "paint": {
              "text-color": "#C9D1D9",
              "text-halo-color": "#1E2329",
              "text-halo-width": 1.5 }
        },

        { "type": "symbol",
          "source-layer": "Parks",
          "layout": {
              "text-field": ["get", "NAME"],
              "text-font": ["Noto Sans Bold"],
              "text-size": 11 },
          "paint": {
              "text-color": "#9AD3A8",
              "text-halo-color": "#1E2329",
              "text-halo-width": 1.5 }
        },

        { "type": "symbol",
          "source-layer": "Hotels",
          "layout": {
              "text-field": ["get", "NAME"],
              "text-font": ["Noto Sans Bold"],
              "text-size": 11,
              "text-offset": [0, 0.5],
              "text-anchor": "top" },
          "paint": {
              "text-color": "#F5C58E",
              "text-halo-color": "#1E2329",
              "text-halo-width": 1.5 }
        }
      ]
    }
    """;

    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;

        // The shapefiles are in Texas North Central feet (EPSG:2276); reproject to
        // the map's EPSG:3857. Every restyle passes this same source: a document on
        // its own has no data to draw.
        var tiles = new FeatureSourceVectorTileSource();
        foreach (var file in new[] { "Parks.shp", "Streets.shp", "Hotels.shp" })
        {
            tiles.FeatureSources.Add(new ShapeFileFeatureSource(SampleShared.Shapefile(file))
            {
                ProjectionConverter = new ProjectionConverter(2276, 3857),
            });
        }

        map.Basemap = new GpuBasemap(new MapStyle(StyleJson, tiles));
        map.CenterPoint = new PointShape(-10777290, 3908740);
        map.MapScale = 9000;
        _ = map.RefreshAsync();
    }
}
