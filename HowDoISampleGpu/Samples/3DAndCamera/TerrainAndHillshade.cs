using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The ground with its real shape: satellite imagery draped over a terrain source and
/// hill-shaded, over the Grand Canyon.
/// <para>
/// No controls at all. Terrain is what the sample is, so it is on; and the lean is
/// the only reason the terrain is visible as terrain rather than as shading, so the
/// map opens with it. Two fingers sliding together change it from there, which is
/// the gesture the platform already gives for it.
/// </para>
/// <para>
/// It was called "Terrain in 3D", which said the wrong thing twice: every GPU sample
/// here is drawn in 3D and most of them can be leaned over, so the words singled this
/// one out for the thing it shares with all of them, and said nothing about what it
/// alone has - a DEM under the imagery and a hillshade layer reading it.
/// </para>
/// </summary>
public class TerrainAndHillshade : SamplePage
{
    private const string HillshadeStyle = """
    {
      "layers": [
        { "type": "hillshade",
          "source": "terrain-dem",
          "paint": { "hillshade-exaggeration": 0.4 } }
      ]
    }
    """;

    /// <summary>The south rim, looking north across the gorge.</summary>
    private static readonly RectangleShape StartExtent =
        new(-12_534_000, 4_346_000, -12_484_000, 4_314_000);

    /// <summary>
    /// Leaning enough that the gorge has depth rather than shading. Flat, a DEM and a
    /// hillshade image look the same: the relief is in the shading either way, and
    /// nothing tells you the ground itself has height.
    /// </summary>
    private const double StartTilt = 63;

    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;

        // The AWS open terrarium tiles (no key), as a code-built RasterDemSource.
        // SetTerrain adds it under the id "terrain-dem", which the hillshade layer
        // above reads.
        var dem = new RasterDemSource(
            "https://s3.amazonaws.com/elevation-tiles-prod/terrarium/{z}/{x}/{y}.png",
            DemEncoding.Terrarium);

        var style = new MapStyle()
            .AddRaster(new ThinkGeoRasterTileSource(SampleShared.CloudApiKey, ThinkGeoCloudRasterMapsMapType.Aerial_V2_X1),
                sourceId: "thinkgeo")
            .SetTerrain(dem, 1.35)
            .AddStyle(HillshadeStyle);

        map.Basemap = new GpuBasemap(style);
        map.CurrentExtent = StartExtent;
        map.TiltAngle = StartTilt;
        _ = map.RefreshAsync();
    }
}
