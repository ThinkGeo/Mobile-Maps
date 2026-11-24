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
        // The first row is the gallery's front door, so it is a map that is on screen
        // before anyone has decided to wait: a city standing up. The row after it is
        // its pair - the height of a city and the shape of the ground - then the camera
        // being driven and the camera being fenced in. The globe comes last, and the
        // comment on it says why.
        new("Map Navigation", "Buildings in 3D",
            "A city with its real height: one extrusion layer over a published style stands Midtown up behind Union Square, and a switch moves the whole map between the light and dark themes",
            "GpuBasemap, fill-extrusion", SampleDraws.Gpu, () => new BuildingsIn3D(),
            aliases: "buildings, 3D buildings, extrusion, fill-extrusion, city, skyline, height, tilt, "
                   + "style.json, SetStyleAsync, dark theme, light theme, first map, New York, Manhattan"),
        new("Map Navigation", "Terrain and Hillshade",
            "The ground with its real shape: satellite imagery draped over a terrain source with roads drawn on the photograph, and a switch to a hill-shaded vector map, at Mount Rainier",
            "MapStyle.SetTerrain, AddStyle", SampleDraws.Gpu, () => new TerrainAndHillshade(),
            aliases: "terrain in 3D, terrain, DEM, hillshade, terrarium, elevation, drape, Mount Rainier, mountain, 3D, "
                   + "hybrid, roads over imagery, shaded relief, buildings, onlyLayerTypes"),
        new("Map Navigation", "GPS Navigation",
            "A drive along a recorded GPS track - the camera flies to each point, the vehicle rides the flight's progress, and the road behind turns green",
            "DynamicSource", SampleDraws.Gpu, () => new GpsNavigation(),
            aliases: "navigation, GPS, GPX, track, vehicle, follow camera, route, theme, tilt, turn by turn, live location"),
        new("Map Navigation", "Restrict Map Extent",
            "Keeping the map inside a box and between two scales - tilt it, turn it and zoom it as you like, it still cannot leave the red box",
            "CurrentExtentChanging", SampleDraws.Gpu, () => new RestrictMapExtent(),
            aliases: "restrict, bounds, clamp, MinScale, MaxScale, limit, pan, zoom, tilt, rotate"),

        // ---- From a Service ----
        new("From a Service", "Vector Map Providers",
            "The same map from several vector tile providers - a new provider is a new style over the same map, and the camera holds",
            "ThinkGeoVectorTileSource", SampleDraws.Gpu, () => new VectorMapProviders(),
            aliases: "OpenFreeMap, Liberty, Positron, style.json, SetStyleAsync, restyle"),
        new("From a Service", "Raster Map Providers",
            "Rasters nobody can send as vectors - aerial imagery, an open tile server, a WMS and a WMTS - each swapped in place with the camera holding",
            "RasterTileSource, WmsRasterTileSource, WmtsRasterTileSource", SampleDraws.Gpu, () => new RasterMapProviders(),
            aliases: "OpenStreetMap, XYZ, aerial, satellite, imagery, WMS, WMTS, NASA, Swisstopo, ThinkGeoRasterTileSource"),
        new("From a Service", "Live Flights",
            "Every aircraft airborne right now, from OpenSky, each dead-reckoned forward from its last report at fifty frames a second",
            "DynamicSource", SampleDraws.Gpu, () => new LiveFlights(),
            aliases: "OpenSky, flights, aircraft, live, dead reckoning, markers, animation, heading"),
        // Named for the data, not for the programme that collects it: the sample is
        // how a radar mosaic reaches the map, and NEXRAD is whose mosaic it happens
        // to be. The name stays in the aliases, where someone looking for it by it
        // will still find this row.
        new("From a Service", "Weather Radar",
            "The last half hour of American weather radar, looping - six images fetched once and switched by visibility, so nothing is re-fetched between frames",
            "GeoImageRasterTileSource", SampleDraws.Gpu, () => new WeatherRadar(),
            aliases: "radar, NEXRAD, nexrad, weather, reflectivity, Mesonet, animation, TransparentColor, SetLayerVisibility"),

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
        new("Styling", "Weather Grids",
            "Two forecast grids over the same country - forty-four hours of sky cover played as an animation, and the surface wind as particles carried along it",
            "MapStyle.AddGridFill, MapStyle.AddParticleFlow", SampleDraws.Gpu, () => new WeatherGrids(),
            aliases: "weather, NDFD, forecast, sky, cloud, grid fill, animation, wind, particle, flow, vector field, advection, GRIB"),

        // ---- Markers and Popups ----
        new("Markers and Popups", "Markers",
            "Markers are views on the map, not drawings in it - a pin whose tip points at its ground point, an animated one, and tap to add or remove",
            "SimpleMarkerOverlay", SampleDraws.GpuAndOverlays, () => new Markers(),
            aliases: "marker, pin, icon, animated, tap, add, remove, ground point, tilt"),
        new("Markers and Popups", "Popups",
            "A popup for every hotel, each one holding its ground point as the map is panned, zoomed and tilted",
            "PopupOverlay", SampleDraws.GpuAndOverlays, () => new Popups(),
            aliases: "popup, callout, label, hotel, overlay, tilt, ground point"),
        new("Markers and Popups", "Cluster Markers",
            "Coyote sightings clustered into tappable discs, rebuilt for the view each time it settles, with a tap zooming to what is inside",
            "SimpleMarkerOverlay", SampleDraws.GpuAndOverlays, () => new ClusterMarkers(),
            aliases: "cluster, marker, bucket, grid, zoom, sightings, aggregate"),

        // ---- Map Tools and Adornments ----
        new("Map Tools and Adornments", "Scale Line and Scale Bar",
            "The two scale adornments drawn over the GPU basemap, in imperial, metric or both",
            "ScaleLineAdornmentLayer", SampleDraws.GpuAndOverlays, () => new ScaleLineScaleBar(),
            aliases: "scale bar, scale line, ScaleBarAdornmentLayer, UnitSystem, miles, kilometres"),
        new("Map Tools and Adornments", "Graticule Lines and Legend",
            "The two adornments that frame a map - a degree grid labelled down the edges and a legend whose swatches are the styles painting the states",
            "GraticuleAdornmentLayer, LegendAdornmentLayer", SampleDraws.GpuAndOverlays, () => new GraticuleLinesAndLegend(),
            aliases: "graticule, grid, lat lon, degrees, legend, swatch, class break, key, population, AdornmentOverlay, adornment"),
        new("Map Tools and Adornments", "Export the Map to PDF",
            "The map on screen written out as a true vector PDF - geometry as paths, labels as selectable text - and handed to the system to open, file or print",
            "PdfGeoCanvas, PrinterLayoutDocument, MvtTilesAsyncLayer", SampleDraws.GpuAndOverlays, () => new ExportToPdf(),
            aliases: "print, printing, PDF, export, vector, page, A4, letter, layout, share, AirPrint, PrinterLayoutDocument"),

        // ---- Shapes and Editing ----
        new("Shapes and Editing", "Measure Distance and Area",
            "Measuring what a finger draws - a line's length or a polygon's area and where its centre is, taken on the ground; double-tap or long-press to finish the shape",
            "TrackInteractiveOverlay, GetLength, GetArea, GetCenterPoint", SampleDraws.GpuAndOverlays, () => new MeasureDistanceAndArea(),
            aliases: "measure, distance, area, ruler, track, draw, polygon, line, centre, centroid, projection, double tap, long press"),
        new("Shapes and Editing", "Edit Features",
            "Drawing and editing by hand - draw points, lines and polygons or drag, resize, rotate and reshape what is there, with every overlay event logged",
            "TrackOverlay, EditOverlay", SampleDraws.GpuAndOverlays, () => new EditFeatures(),
            aliases: "draw, edit, track, vertex, drag, resize, rotate, polygon, line, point, measure"),
        new("Shapes and Editing", "Shape Operations",
            "What a shape can be asked to become - buffered, simplified, rotated, scaled, translated, hulled, enveloped, intersected, differenced, unioned",
            "AreaBaseShape", SampleDraws.Gpu, () => new ShapeOperations(),
            aliases: "buffer, simplify, rotate, scale, translate, convex hull, envelope, intersection, difference, union"),
        new("Shapes and Editing", "Find Features by Spatial Relation",
            "Tap the zoning map and either the area you are standing in or everything within five hundred metres lights up, named from the same query",
            "QueryTools.GetFeaturesContaining", SampleDraws.Gpu, () => new FindFeaturesBySpatialRelation(),
            aliases: "spatial query, contains, within distance, zoning, QueryTools, attributes, columns, tap"),

        // Last, and not where a reader would look for it. It used to open the gallery
        // under the name "Hello World", which promised the simplest thing here and
        // delivered the most expensive: a vector globe's first load is measured in
        // seconds, so the first map anyone saw was the slowest one in the app, and the
        // name gave them nothing to attribute the wait to. The name now says what the
        // sample is about, and the row sits where someone who wants a globe will go
        // looking rather than where everyone arrives.
        new("Map Navigation", "Globe and Projections",
            "The world as a sphere and as three projections, morphing between them, from satellite imagery or vector streets - the projection is the camera's, so nothing is reprojected",
            "DisplayProjection", SampleDraws.Gpu, () => new TheGlobe(),
            aliases: "hello, hello world, globe, sphere, the globe, projection, Robinson, Mollweide, Web Mercator, "
                   + "proj4, proj string, AnimateDisplayProjectionAsync, morph, raster, vector, "
                   + "first light, smoke test, getting started"),

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
        ("Styling - Dynamic Rendering",
            "Built for this gallery and then cut from it, like Lane Level Navigation above. Its API was "
            + "not its own: DynamicSource is used by nine samples here, and three of them tell this one's "
            + "story better - Live Flights rewrites thousands of aircraft every frame from a live service, "
            + "GPS Navigation carries the same source under a moving camera, and Shape Operations recomputes "
            + "a shape's geometry as a parameter sweeps. What was left was the data: two centuries of state "
            + "populations as circles that grow, which is a chart drawn over a map rather than a question "
            + "anyone asks a map to answer, and which cost a phone's panel a year slider and a play control "
            + "to show. The shapefile it shared stayed - Graticule Lines and Legend draws the same states."),
        ("Lane Level Navigation",
            "Built for this gallery and then cut from it. It told the same story as GPS Navigation - a follow "
            + "camera carried along a route - and the part that was its own, a lane-level OpenDRIVE HD map, is "
            + "the part nobody can copy: the data is three and a half megabytes of Wolfsburg prepared in advance "
            + "and carried in the package, and a reader with no OpenDRIVE of their own cannot do anything with "
            + "what the sample shows. A sample that cannot be followed is a demonstration, not an example."),
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
            + "server as a source - without a second live weather feed beside Weather Radar and Weather Grids."),
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
