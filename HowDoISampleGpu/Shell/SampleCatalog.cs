namespace HowDoISampleGpu;

using HowDoISampleGpu.Samples;

/// <summary>
/// The one catalog. The menu, the search index and the home page are all
/// projections of this list, so a sample is described once.
/// <para>
/// Grouping is by two properties a dataset actually HAS, never by the role an
/// application gives it - who HOLDS the data (a service somebody else operates =
/// From a Service; a store you own = Your Data) and what the data IS. The
/// renderer is not a group: it is a property of a row.
/// </para>
/// <para>
/// There was a "Dynamic Data" group once, and it was the rule above being broken
/// in plain sight: dynamic is a role an application gives data, not something the
/// data is. The same shapefile sat in Styling when it was drawn once and in
/// Dynamic Data when it was redrawn every frame. Worse, it hid the one distinction
/// this list is built on - the two genuinely live service feeds, OpenSky and
/// Mesonet, were in it, while the group named after services held nothing but
/// basemap providers. Its rows are now where the data puts them: the two feeds
/// under From a Service, the forecast grids and the per-frame geometry under
/// Styling beside the other ways of drawing a dataset, and the GPS track under
/// Map Navigation with the other cameras.
/// </para>
/// <para>
/// The order is the order someone meets the SDK in, which is where this parts
/// company with samples/wpf/HowDoISample: get a map on screen, point it at a
/// service, put your own data in, make it look right. Those first three cover
/// most of what most people came for. The desktop gallery is organised by API
/// area instead, which reads as a table of contents for the SDK rather than for
/// the job, and a phone has far less room to browse before someone gives up
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

        // ---- Map Navigation ----
        new("Map Navigation", "Your First Map",
            "The smallest GPU map there is: a style URI on a MapView and nothing else, opening over Union Square and already leaning, with Midtown standing up behind it and a switch between the light and dark themes",
            "GpuBasemap", SampleDraws.Gpu, () => new YourFirstMap(),
            aliases: "hello, first light, smoke test, style.json, getting started, dark theme, light theme, "
                   + "SetStyleAsync, fill-extrusion, 3D buildings, tilt"),
        new("Map Navigation", "Restrict Map Extent",
            "Keeping the map inside a box and between two scales - tilt it, turn it and zoom it as you like, it still cannot leave the red box",
            "CurrentExtentChanging", SampleDraws.Gpu, () => new RestrictMapExtent(),
            aliases: "restrict, bounds, clamp, MinScale, MaxScale, limit, pan, zoom, tilt, rotate"),
        new("Map Navigation", "Terrain and Hillshade",
            "The ground with its real shape: satellite imagery draped over a terrain source and hill-shaded, over the Grand Canyon",
            "MapStyle.SetTerrain", SampleDraws.Gpu, () => new TerrainAndHillshade(),
            aliases: "terrain in 3D, terrain, DEM, hillshade, terrarium, elevation, drape, Grand Canyon, 3D"),
        new("Map Navigation", "GPS Navigation",
            "A drive along a recorded GPS track - the camera flies to each point, the vehicle rides the flight's progress, and the road behind turns green",
            "DynamicSource", SampleDraws.Gpu, () => new GpsNavigation(),
            aliases: "navigation, GPS, GPX, track, vehicle, follow camera, route, theme, tilt, turn by turn, live location"),
        new("Map Navigation", "Lane Level Navigation",
            "A drive through Wolfsburg at lane level - an OpenDRIVE HD map cut into vector tiles, a follow camera, and the city's buildings standing around it",
            "DynamicSource", SampleDraws.Gpu, () => new LaneLevelNavigation(),
            aliases: "navigation, lane, OpenDRIVE, HD map, follow camera, tilt, 3D buildings, route, turn by turn"),
        new("Map Navigation", "The Globe",
            "The world as a sphere and as three projections, morphing between them, from satellite imagery or vector streets - the projection is the camera's, so nothing is reprojected",
            "DisplayProjection", SampleDraws.Gpu, () => new TheGlobe(),
            aliases: "globe, sphere, projection, Robinson, Mollweide, Web Mercator, proj4, proj string, "
                   + "AnimateDisplayProjectionAsync, morph, raster, vector"),

        // ---- From a Service ----
        new("From a Service", "Vector Map Providers",
            "The same map from several vector tile providers - a new provider is a new style over the same map, and the camera holds",
            "ThinkGeoVectorTileSource", SampleDraws.Gpu, () => new VectorMapProviders(),
            aliases: "OpenFreeMap, Liberty, Positron, style.json, SetStyleAsync, restyle"),
        new("From a Service", "Raster Map Providers",
            "The same map from several raster tile providers - the raster registration is swapped in place and the camera holds",
            "RasterTileSource", SampleDraws.Gpu, () => new RasterMapProviders(),
            aliases: "OpenStreetMap, XYZ, aerial, hybrid, ThinkGeoRasterTileSource"),
        new("From a Service", "Live Flights",
            "Every aircraft airborne right now, from OpenSky, each dead-reckoned forward from its last report at fifty frames a second",
            "DynamicSource", SampleDraws.Gpu, () => new LiveFlights(),
            aliases: "OpenSky, flights, aircraft, live, dead reckoning, markers, animation, heading"),
        new("From a Service", "NEXRAD Radar",
            "The last half hour of American weather radar, looping - six images fetched once and switched by visibility, so nothing is re-fetched between frames",
            "GeoImageRasterTileSource", SampleDraws.Gpu, () => new NexradRadar(),
            aliases: "radar, NEXRAD, weather, Mesonet, animation, TransparentColor, SetLayerVisibility"),

        // ---- Your Data ----
        new("Your Data", "Vector File Formats",
            "The same map from six vector file formats - shapefile, GeoJSON, GPX, MapInfo TAB, TinyGeo and spatial SQLite - with the style never learning which it was",
            "FeatureSource", SampleDraws.Gpu, () => new VectorFileFormats(),
            aliases: "shapefile, GeoJSON, KML, GPX, TAB, MapInfo, TinyGeo, SQLite, spatial, formats, read, connection string, offline, on the device, no network"),
        new("Your Data", "Offline Tiles",
            "A map with no network at all: tiles read from an MBTiles or PMTiles archive on the device, vector or raster",
            "MbTilesVectorTileSource", SampleDraws.Gpu, () => new OfflineTiles(),
            aliases: "offline, mbtiles, pmtiles, archive, no network, PmTilesRasterTileSource, Stamen"),

        // ---- Styling ----
        new("Styling", "Render Points, Lines and Polygons",
            "The three shapes of vector data, each with its own style - parks as fills, streets as lines, hotels as circles, and a label layer over each",
            "MapStyle", SampleDraws.Gpu, () => new RenderPointsLinesAndPolygons(),
            aliases: "fill, line, circle, symbol, style.json, shapefile, labels, source-layer"),
        new("Styling", "Heatmap",
            "Coyote sightings as a heatmap, with the ramp, radius and intensity changed at run time - a paint change, not a restyle",
            "MapStyle.SetHeatmapPaint", SampleDraws.Gpu, () => new Heatmap(),
            aliases: "heatmap, density, ColorStop, ramp, Viridis, heatmap-radius, shapefile"),
        new("Styling", "Isolines",
            "Contours drawn from scattered measurements - mosquito counts turned into isolines, with the level count and search radius live",
            "MapStyle.AddIsolines", SampleDraws.Gpu, () => new Isolines(),
            aliases: "isoline, contour, interpolation, IDW, PointValueSource, IsolineStyle, levels"),
        new("Styling", "NDFD Sky Grid Animation",
            "Forty-four forecast hours of sky cover played as an animation over a grid fill, with five palettes and a slider that scrubs the hours",
            "MapStyle.AddGridFill", SampleDraws.Gpu, () => new NdfdSkyGridAnimation(),
            aliases: "sky, cloud, NDFD, GRIB, forecast, animation, grid fill, palette, colour stops"),
        new("Styling", "NDFD Wind Field",
            "The forecast surface wind drawn two ways from one field - sixty-five thousand particles advected along it, and the same field as magnitude-scaled arrows",
            "MapStyle.AddParticleFlow", SampleDraws.Gpu, () => new NdfdWindField(),
            aliases: "wind, NDFD, forecast, particle, flow, vector field, arrows, advection, weather"),
        new("Styling", "Dynamic Rendering",
            "Two centuries of American state populations as circles that grow and change colour - geometry rewritten on every frame, with no tile cut",
            "DynamicSource", SampleDraws.Gpu, () => new DynamicRendering(),
            aliases: "DynamicSource, animation, population, census, circles, interpolate, no tiles"),

        // ---- Markers and Adornments ----
        new("Markers and Adornments", "Markers",
            "Markers are views on the map, not drawings in it - a pin whose tip points at its ground point, an animated one, and tap to add or remove",
            "SimpleMarkerOverlay", SampleDraws.GpuAndOverlays, () => new Markers(),
            aliases: "marker, pin, icon, animated, tap, add, remove, ground point, tilt"),
        new("Markers and Adornments", "Popups",
            "A popup for every hotel, each one holding its ground point as the map is panned, zoomed and tilted",
            "PopupOverlay", SampleDraws.GpuAndOverlays, () => new Popups(),
            aliases: "popup, callout, label, hotel, overlay, tilt, ground point"),
        new("Markers and Adornments", "Cluster Markers",
            "Coyote sightings clustered into tappable discs, rebuilt for the view each time it settles, with a tap zooming to what is inside",
            "SimpleMarkerOverlay", SampleDraws.GpuAndOverlays, () => new ClusterMarkers(),
            aliases: "cluster, marker, bucket, grid, zoom, sightings, aggregate"),
        new("Markers and Adornments", "Legend",
            "A legend that cannot lie - the swatch in each row and the style that paints the state are the same object",
            "LegendAdornmentLayer", SampleDraws.GpuAndOverlays, () => new Legend(),
            aliases: "legend, swatch, class break, adornment, key, population"),
        new("Markers and Adornments", "Scale Line and Scale Bar",
            "The two scale adornments drawn over the GPU basemap, in imperial, metric or both",
            "ScaleLineAdornmentLayer", SampleDraws.GpuAndOverlays, () => new ScaleLineScaleBar(),
            aliases: "scale bar, scale line, ScaleBarAdornmentLayer, UnitSystem, miles, kilometres"),
        new("Markers and Adornments", "Display Graticule Lines",
            "A latitude and longitude grid over the map, drawn by the classic adornment pipeline above the GPU basemap",
            "GraticuleAdornmentLayer", SampleDraws.GpuAndOverlays, () => new DisplayGraticuleLines(),
            aliases: "graticule, grid, lat lon, degrees, AdornmentOverlay, adornment"),
        new("Shapes and Editing", "Measure Distance and Area",
            "Measuring what a finger draws - a line's length or a polygon's area, taken on the ground rather than on the web-mercator plane",
            "TrackInteractiveOverlay", SampleDraws.GpuAndOverlays, () => new MeasureDistanceAndArea(),
            aliases: "measure, distance, area, ruler, track, draw, polygon, line, projection"),

        // ---- Shapes and Editing ----
        new("Shapes and Editing", "Edit Features",
            "Drawing and editing by hand - draw points, lines and polygons or drag, resize, rotate and reshape what is there, with every overlay event logged",
            "TrackOverlay, EditOverlay", SampleDraws.GpuAndOverlays, () => new EditFeatures(),
            aliases: "draw, edit, track, vertex, drag, resize, rotate, polygon, line, point, measure"),
        new("Shapes and Editing", "Shape Measurements",
            "What a shape can be asked about itself - its area, its length, its centre, and the shortest line from it to somewhere else",
            "AreaBaseShape.GetArea", SampleDraws.Gpu, () => new ShapeMeasurements(),
            aliases: "area, length, centre, centroid, shortest line, GetLength, GetShortestLineTo, measure"),
        new("Shapes and Editing", "Shape Operations",
            "What a shape can be asked to become - buffered, simplified, rotated, scaled, translated, hulled, enveloped, intersected, differenced, unioned",
            "AreaBaseShape", SampleDraws.Gpu, () => new ShapeOperations(),
            aliases: "buffer, simplify, rotate, scale, translate, convex hull, envelope, intersection, difference, union"),
        new("Shapes and Editing", "Get Data from Features",
            "What a feature carries besides its shape: tap a park and its columns are listed",
            "Feature.ColumnValues", SampleDraws.Gpu, () => new GetDataFromFeatures(),
            aliases: "attributes, columns, ColumnValues, ReturningColumnsType, identify, tap"),
        new("Shapes and Editing", "Find Features by Spatial Relation",
            "The spatial relations asked of Frisco's zoning polygons - containing, intersecting, within a distance of, disjoint from a tapped point",
            "QueryTools", SampleDraws.Gpu, () => new FindFeaturesBySpatialRelation(),
            aliases: "spatial query, contains, intersects, disjoint, within distance, QueryTools, zoning"),

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
        ("Map Tools and Adornments - Drag and Resize Adornments",
            "AdornmentLayer.DragMode and ResizeMode are Core properties and compile everywhere, but only the "
            + "desktop acts on them: ThinkGeo.UI.Desktop has an AdornmentResizeHandler that moves and resizes "
            + "adornments from mouse events, and MAUI's AdornmentOverlay only draws. The adornments the sample "
            + "shows - legend, scale line, scale bar, logo, magnetic declination - all draw on mobile; dragging "
            + "and resizing them is what does not exist."),
        ("Your Data - Nautical Charts",
            "S-57 charts are read through GDAL - ThinkGeo.NauticalCharts references ThinkGeo.Gdal - "
            + "and GDAL's natives are not published for iOS or Android."),
        ("Projections - Reproject a Raster",
            "Warping a raster into another projection is GdalProjectionConverter's job, and GDAL's natives "
            + "are not published for iOS or Android. The Proj-backed ProjectionConverter converts coordinates "
            + "but does not warp pixels: the same OpenStreetMap layer draws correctly with no converter and "
            + "draws nothing with one."),
        ("Markdown Document Viewer", "Built on AvalonEdit and WebView2, both Windows-only."),
        ("From a Service - NOAA Weather",
            "Works on a phone; cut from this gallery during the trimming pass that took it from sixty-one "
            + "samples to thirty-four. The service samples that remain show the same thing - somebody else's "
            + "server as a source - without a second live weather feed beside NEXRAD Radar and the two NDFD samples."),
        ("Extending the SDK - Custom Adornment Layer, Custom Feature Sources, Handle Tile Failures",
            "All three work on a phone; the group was cut from this gallery during the same trimming pass. "
            + "They extend the SDK rather than show it, which is reference material for somebody already "
            + "building, and the desktop gallery is where that reader is better served."),
        ("Databases",
            "Partly a platform fact and partly a decision. GeoPackage goes through GDAL, File GeoDatabase through "
            + "Esri's Windows-only native SDK and Oracle through OCI, none of which ship for iOS or Android. "
            + "PostgreSQL and SQL Server do work from a phone and are still left out: holding a connection to a "
            + "database server open across a mobile network is the wrong shape for the platform - it is the "
            + "client doing the server's job, over the least reliable link in the system. The mobile answers are "
            + "a file you carry and a service you call, which Vector File Formats, Offline Tiles and the service "
            + "samples cover. The one database a phone genuinely holds, a spatial SQLite file, is a row of "
            + "Vector File Formats."),
        ("Vector File Formats - KML, CAD, GeoPDF", "Read through GDAL, whose natives ship for Windows and Linux but not for iOS or Android. The other five formats are included."),
        ("Make the Labels Stand Out",
            "A wash slipped under someone else's labels to lift them off aerial imagery. The trick is real and the "
            + "result was not: on a phone the wash reads as a haze over the picture rather than as the labels "
            + "standing out, and the one control it has costs a restyle per move because a fill's opacity has no "
            + "runtime knob. Dropped on looks, not on capability."),
        ("Raster File Formats", "Every format it shows - GeoTIFF, JPEG 2000, world-filed JPEG, MrSID, ECW - is read through GDAL, so the whole sample is a Windows and Linux one. Raster data still reaches a mobile map through the tile sources, which Offline Tiles and the service samples cover."),
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

    /// <summary>How many samples the heading is standing over, for the band's right edge.</summary>
    public string Label => Count == 1 ? "1 sample" : $"{Count} samples";
}
