namespace HowDoISampleGpu;

using HowDoISampleGpu.Samples;

/// <summary>
/// The one catalog. The menu, the search index and the home page are all
/// projections of this list, so a sample is described once.
/// <para>
/// Ordering and grouping follow samples/wpf/HowDoISample/Shell/SampleCatalog.cs,
/// deliberately: the two galleries are the same gallery on two platforms, and a
/// customer comparing them should find the same sample under the same heading.
/// Grouping is by two properties a dataset actually HAS, never by the role an
/// application gives it - who HOLDS the data (a service somebody else operates =
/// Maps from a Service; a store you own = Your Data) and what the data IS. The
/// renderer is not a group: it is a property of a row.
/// </para>
/// <para>
/// Samples the desktop gallery has and this one does not are listed in
/// <see cref="NotOnMobile"/> with the reason, so the gap is a fact in the code
/// rather than something to rediscover.
/// </para>
/// </summary>
public static class SampleCatalog
{
    public static readonly IReadOnlyList<SampleEntry> All = new SampleEntry[]
    {
        // ---- Start Here ----
        new("Start Here", "First Light",
            "The smallest GPU map there is: a style.json URI on a MapView, and nothing else",
            "GpuStyleOverlay", SampleDraws.Gpu, () => new FirstLight(),
            aliases: "hello, smoke test, style.json, getting started"),

        // ---- Maps from a Service - Raster ----
        new("Maps from a Service - Vector", "Vector Map Providers",
            "The same map from several vector tile providers - a new provider is a new style over the same map, and the camera holds",
            "ThinkGeoVectorTileSource", SampleDraws.Gpu, () => new VectorMapProviders(),
            aliases: "OpenFreeMap, Liberty, Positron, style.json, SetStyleAsync, restyle"),

        new("Maps from a Service - Vector", "WFS",
            "A WFS V2 service merged into the basemap's style and drawn by the renderer - the Dutch national cadastral parcels, from PDOK",
            "WfsV2AsyncFeatureSource", SampleDraws.Gpu, () => new WFS(),
            aliases: "PDOK, kadaster, parcels, OGC, MinDataZoom, Crs"),

        new("Maps from a Service - Raster", "Raster Map Providers",
            "The same map from several raster tile providers - the raster registration is swapped in place and the camera holds",
            "RasterTileSource", SampleDraws.Gpu, () => new RasterMapProviders(),
            aliases: "OpenStreetMap, XYZ, aerial, hybrid, ThinkGeoRasterTileSource"),

        new("Maps from a Service - Raster", "WMS",
            "NASA's near-real-time satellite imagery from a WMS server, laid into the basemap's own layers rather than over them",
            "WmsRasterTileSource", SampleDraws.Gpu, () => new WMS(),
            aliases: "GIBS, VIIRS, earthdata, StyleLayerSlot, AboveFills, imagery"),

        new("Maps from a Service - Raster", "WMTS",
            "The Swiss national map from swisstopo's WMTS server as the basemap, read through the server's web-mercator tile matrix set",
            "WmtsRasterTileSource", SampleDraws.Gpu, () => new WMTS(),
            aliases: "swisstopo, TileMatrixSet, raster service, OGC"),
    };

    /// <summary>
    /// The groups in menu order, each with its rows. A projection of
    /// <see cref="All"/>, not a second list to keep in step with it.
    /// </summary>
    public static IReadOnlyList<SampleGroup> Groups =>
        All.GroupBy(entry => entry.Group)
           .Select(group => new SampleGroup(group.Key, group.ToList()))
           .ToList();

    /// <summary>The rows matching a search box's text, in catalog order.</summary>
    public static IReadOnlyList<SampleEntry> Search(string text)
        => All.Where(entry => entry.Matches(text)).ToList();

    /// <summary>
    /// What samples/wpf/HowDoISample has that this gallery does not, and why. Kept
    /// here rather than in a document because the reason is a fact about the
    /// platform and belongs next to the catalog it explains.
    /// </summary>
    public static readonly IReadOnlyList<(string Title, string Reason)> NotOnMobile = new[]
    {
        ("Print the Map", "The WPF printing stack has no mobile counterpart."),
        ("Markdown Document Viewer", "Built on AvalonEdit and WebView2, both Windows-only."),
        ("Databases - Oracle", "No OCI client ships for iOS or Android; the other databases are included."),
    };
}

/// <summary>
/// One menu group and the rows under it. It IS the list of rows rather than
/// holding one: a grouped CollectionView binds to a collection of collections, and
/// a group that merely has an Entries property renders its header and no rows at
/// all - silently, which is a long way to look for a missing colon.
/// </summary>
public sealed class SampleGroup : List<SampleEntry>
{
    public SampleGroup(string title, IEnumerable<SampleEntry> entries)
        : base(entries)
        => Title = title;

    public string Title { get; }
}
