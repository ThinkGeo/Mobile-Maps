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
        new("Start Here", "Migrate a Classic Layer",
            "The same five classic layers drawn twice - above by the GPU after their styles are translated into a style document, below by the pipeline that has always drawn them",
            "FeatureLayerTranslator", SampleDraws.SideBySide, () => new MigrateAClassicLayer(),
            aliases: "migrate, translate, classic, ValueStyle, TextStyle, hatch, side by side, warnings"),

        new("Start Here", "First Light",
            "The smallest GPU map there is: a style.json URI on a MapView, and nothing else",
            "GpuBasemap", SampleDraws.Gpu, () => new FirstLight(),
            aliases: "hello, smoke test, style.json, getting started"),

        // ---- Map Navigation ----
        new("Map Navigation", "Zoom to a Black Hole",
            "Ten real telescope pictures of M87, each inside the one before it, crossfaded by zoom - a powers-of-ten dive from the galaxy to its black hole",
            "ZoomToAsync", SampleDraws.Gpu, () => new ZoomToBlackHole(),
            aliases: "M87, black hole, powers of ten, zoom, crossfade, raster-opacity, TileMatrixSet, EHT"),

        new("Map Navigation", "Overview Map",
            "A small map in the corner showing where the big one is looking - a red frame over the ground the main view covers, and a tap to go there",
            "DynamicSource", SampleDraws.SideBySide, () => new OverviewMap(),
            aliases: "overview, inset, locator, mini map, frame, ToWorldCoordinate, tilt"),

        new("Map Navigation", "Restrict Map Extent",
            "Keeping the map inside a box and between two scales - the three restrictions, each switchable, with the box drawn in red",
            "CurrentExtentChanging", SampleDraws.Gpu, () => new RestrictMapExtent(),
            aliases: "restrict, bounds, clamp, MinScale, MaxScale, limit, pan, zoom"),

        // ---- Map Tools and Adornments ----
        new("Map Tools and Adornments", "Display Graticule Lines",
            "A latitude and longitude grid over the map, drawn by the classic adornment pipeline above the GPU basemap",
            "GraticuleAdornmentLayer", SampleDraws.GpuAndOverlays, () => new DisplayGraticuleLines(),
            aliases: "graticule, grid, lat lon, degrees, AdornmentOverlay, adornment"),

        new("Map Tools and Adornments", "Legend",
            "A legend that cannot lie - the swatch in each row and the style that paints the state are the same object",
            "LegendAdornmentLayer", SampleDraws.GpuAndOverlays, () => new Legend(),
            aliases: "legend, swatch, class break, adornment, key, population"),

        new("Map Tools and Adornments", "Measure Distance and Area",
            "Measuring what a finger draws - a line's length or a polygon's area, taken on the ground rather than on the web-mercator plane",
            "TrackInteractiveOverlay", SampleDraws.GpuAndOverlays, () => new MeasureDistanceAndArea(),
            aliases: "measure, distance, area, ruler, track, draw, polygon, line, projection"),

        new("Map Tools and Adornments", "Scale Line and Scale Bar",
            "The two scale adornments drawn over the GPU basemap, in imperial, metric or both",
            "ScaleLineAdornmentLayer", SampleDraws.GpuAndOverlays, () => new ScaleLineScaleBar(),
            aliases: "scale bar, scale line, ScaleBarAdornmentLayer, UnitSystem, miles, kilometres"),

        // ---- Your Data ----
        new("Your Data", "Vector File Formats",
            "The same map from five vector file formats - shapefile, GeoJSON, GPX, MapInfo TAB and TinyGeo - with the style never learning which it was",
            "FeatureSource", SampleDraws.Gpu, () => new VectorFileFormats(),
            aliases: "shapefile, GeoJSON, KML, GPX, TAB, MapInfo, TinyGeo, formats, read"),

        new("Your Data", "Databases",
            "The same map over three databases - a SQLite file carried with the app and two servers reached over the network - with the style never learning which it was",
            "FeatureSource", SampleDraws.Gpu, () => new Databases(),
            aliases: "database, SQLite, PostgreSQL, PostGIS, SQL Server, spatial, table, connection string"),

        new("Your Data", "Offline Tiles",
            "A map with no network at all: tiles read from an MBTiles or PMTiles archive on the device, vector or raster",
            "MbTilesVectorTileSource", SampleDraws.Gpu, () => new OfflineTiles(),
            aliases: "offline, mbtiles, pmtiles, archive, no network, PmTilesRasterTileSource, Stamen"),

        // ---- Dynamic Data ----
        new("Dynamic Data", "Live Flights",
            "Every aircraft airborne right now, from OpenSky, each dead-reckoned forward from its last report at fifty frames a second",
            "DynamicSource", SampleDraws.Gpu, () => new LiveFlights(),
            aliases: "OpenSky, flights, aircraft, live, dead reckoning, markers, animation, heading"),

        new("Dynamic Data", "Vehicle Navigation",
            "A drive along a recorded GPS track - the camera flies to each point, the vehicle rides the flight's progress, and the road behind turns green",
            "DynamicSource", SampleDraws.Gpu, () => new VehicleNavigation(),
            aliases: "navigation, GPS, track, vehicle, follow camera, route, theme, tilt"),

        new("Dynamic Data", "NDFD Sky Grid Animation",
            "Forty-four forecast hours of sky cover played as an animation over a grid fill, with five palettes and a slider that scrubs the hours",
            "MapStyle.AddGridFill", SampleDraws.Gpu, () => new NdfdSkyGridAnimation(),
            aliases: "sky, cloud, NDFD, GRIB, forecast, animation, grid fill, palette, colour stops"),

        new("Dynamic Data", "NDFD Wind Field",
            "The forecast surface wind drawn two ways from one field - sixty-five thousand particles advected along it, and the same field as magnitude-scaled arrows",
            "MapStyle.AddParticleFlow", SampleDraws.Gpu, () => new NdfdWindField(),
            aliases: "wind, NDFD, forecast, particle, flow, vector field, arrows, advection, weather"),

        new("Dynamic Data", "NEXRAD Radar",
            "The last half hour of American weather radar, looping - six images fetched once and switched by visibility, so nothing is re-fetched between frames",
            "GeoImageRasterTileSource", SampleDraws.Gpu, () => new NexradRadar(),
            aliases: "radar, NEXRAD, weather, Mesonet, animation, TransparentColor, SetLayerVisibility"),

        new("Dynamic Data", "Dynamic Rendering",
            "Two centuries of American state populations as circles that grow and change colour - geometry rewritten on every frame, with no tile cut",
            "DynamicSource", SampleDraws.Gpu, () => new DynamicRendering(),
            aliases: "DynamicSource, animation, population, census, circles, interpolate, no tiles"),

        // ---- Extending the SDK ----

        new("Markers and Popups", "Cluster Markers",
            "Coyote sightings clustered into tappable discs, rebuilt for the view each time it settles, with a tap zooming to what is inside",
            "SimpleMarkerOverlay", SampleDraws.GpuAndOverlays, () => new ClusterMarkers(),
            aliases: "cluster, marker, bucket, grid, zoom, sightings, aggregate"),

        new("Markers and Popups", "Markers",
            "Markers are views on the map, not drawings in it - a pin whose tip points at its ground point, an animated one, and tap to add or remove",
            "SimpleMarkerOverlay", SampleDraws.GpuAndOverlays, () => new Markers(),
            aliases: "marker, pin, icon, animated, tap, add, remove, ground point, tilt"),

        new("Markers and Popups", "Popups",
            "A popup for every hotel, each one holding its ground point as the map is panned, zoomed and tilted",
            "PopupOverlay", SampleDraws.GpuAndOverlays, () => new Popups(),
            aliases: "popup, callout, label, hotel, overlay, tilt, ground point"),

        new("Draw and Edit", "Edit Features",
            "Drawing and editing by hand - draw points, lines and polygons or drag, resize, rotate and reshape what is there, with every overlay event logged",
            "TrackOverlay, EditOverlay", SampleDraws.GpuAndOverlays, () => new EditFeatures(),
            aliases: "draw, edit, track, vertex, drag, resize, rotate, polygon, line, point, measure"),

        new("Extending the SDK", "Custom Adornment Layer",
            "An adornment layer of your own: range rings around the centre of the view, drawn over the GPU basemap",
            "AdornmentLayer.DrawCore", SampleDraws.GpuAndOverlays, () => new CustomAdornmentLayer(),
            aliases: "adornment, DrawCore, GeoCanvas, range rings, custom layer, extend"),

        new("Extending the SDK", "Custom Feature Sources",
            "A feature source of your own - here over a CSV file of points - drawn by the style document like any other",
            "FeatureSource.OpenCore", SampleDraws.Gpu, () => new CustomFeatureSources(),
            aliases: "FeatureSource, custom, CSV, OpenCore, GetAllFeaturesCore, extend"),

        new("Extending the SDK", "Handle Tile Failures",
            "What a map does when a tile cannot be loaded - the default picture, nothing at all, or nothing and ask again",
            "TileLoadFailed", SampleDraws.Gpu, () => new HandleTileFailures(),
            aliases: "401, error tile, DrawException, Retry, ShowOnMap, failure"),

        // ---- Shapes and Spatial Queries ----
        new("Shapes and Spatial Queries", "Shape Measurements",
            "What a shape can be asked about itself - its area, its length, its centre, and the shortest line from it to somewhere else",
            "AreaBaseShape.GetArea", SampleDraws.Gpu, () => new ShapeMeasurements(),
            aliases: "area, length, centre, centroid, shortest line, GetLength, GetShortestLineTo, measure"),

        new("Shapes and Spatial Queries", "Validate Topology",
            "The topology rules, each with data made to both pass and fail it - what the rule checks in green, what it checks against in blue, what breaks it in red",
            "TopologyValidator", SampleDraws.Gpu, () => new ValidateTopology(),
            aliases: "topology, validate, pseudonode, self-intersect, overlap, gaps, rules"),

        new("Shapes and Spatial Queries", "Shape Operations",
            "What a shape can be asked to become - buffered, simplified, rotated, scaled, translated, hulled, enveloped, intersected, differenced, unioned",
            "AreaBaseShape", SampleDraws.Gpu, () => new ShapeOperations(),
            aliases: "buffer, simplify, rotate, scale, translate, convex hull, envelope, intersection, difference, union"),

        new("Shapes and Spatial Queries", "Get Data from Features",
            "What a feature carries besides its shape: tap a park and its columns are listed",
            "Feature.ColumnValues", SampleDraws.Gpu, () => new GetDataFromFeatures(),
            aliases: "attributes, columns, ColumnValues, ReturningColumnsType, identify, tap"),

        new("Shapes and Spatial Queries", "Find Features by Spatial Relation",
            "The spatial relations asked of Frisco's zoning polygons - containing, intersecting, within a distance of, disjoint from a tapped point",
            "QueryTools", SampleDraws.Gpu, () => new FindFeaturesBySpatialRelation(),
            aliases: "spatial query, contains, intersects, disjoint, within distance, QueryTools, zoning"),

        // ---- Styling ----
        new("Styling", "Isolines",
            "Contours drawn from scattered measurements - mosquito counts turned into isolines, with the level count, line width, interpolation power and search radius all live",
            "MapStyle.AddIsolines", SampleDraws.Gpu, () => new Isolines(),
            aliases: "isoline, contour, interpolation, IDW, PointValueSource, IsolineStyle, levels"),

        new("Styling", "Local Fonts",
            "Labelling without a glyph server - city names in six scripts drawn from SDF glyph files shipped with the app",
            "MapStyle glyphs", SampleDraws.Gpu, () => new LocalFonts(),
            aliases: "font, glyph, pbf, SDF, CJK, Cyrillic, Greek, Inuktitut, offline labels, text-font"),

        new("Styling", "Render Based on Code",
            "Layers whose paint and filter are code rather than a document - a density ramp over the American states, and the world's capitals lit as day or night by a clock",
            "StyleLayer.CreateFill", SampleDraws.Gpu, () => new RenderBasedOnCode(),
            aliases: "code, paint delegate, filter, CirclePaint, FillPaint, LinePaint, day night, density"),

        new("Styling", "Render Based on Rules",
            "The style document's own decision-making - match, step, filter, case and zoom interpolation - colouring countries from their attributes rather than from code",
            "StyleLayer filter", SampleDraws.Gpu, () => new RenderBasedOnRules(),
            aliases: "match, step, filter, case, interpolate, expression, data-driven, attributes"),

        new("Styling", "Render Points, Lines and Polygons",
            "The three shapes of vector data, each with its own style - parks as fills, streets as lines, hotels as circles, and a label layer over each",
            "MapStyle", SampleDraws.Gpu, () => new RenderPointsLinesAndPolygons(),
            aliases: "fill, line, circle, symbol, style.json, shapefile, labels, source-layer"),

        new("Styling", "Insert a Layer Under the Labels",
            "A layer slipped into the middle of someone else's style - aerial imagery, a white wash over it, and the street style's lines and labels on top",
            "StyleLayerSlot.UnderLabels", SampleDraws.Gpu, () => new InsertALayerUnderTheLabels(),
            aliases: "slot, UnderLabels, AboveFills, wash, compose, insert, layer order"),

        new("Styling", "Advanced Labels",
            "Seven lessons in labelling, each a style document you can edit on the spot - curves, anchors, priority, wrapping, text-optional, CJK and several columns",
            "symbol layout", SampleDraws.Gpu, () => new AdvancedLabels(),
            aliases: "label, text, symbol, anchor, halo, wrap, text-optional, symbol-sort-key, curved text, CJK"),

        new("Styling", "Display Dot Density",
            "A dot map: one dot for every twenty-five thousand housing units, scattered inside the state that owns them",
            "InMemoryFeatureSource", SampleDraws.Gpu, () => new DisplayDotDensity(),
            aliases: "dot density, scatter, census, housing, circle, thematic"),

        new("Styling", "Hatch Styles",
            "A hatch brush as a fill pattern: the style names an image, and the image is a GeoHatchBrush the application supplies",
            "GeoHatchBrush", SampleDraws.Gpu, () => new HatchStyles(),
            aliases: "hatch, fill-pattern, GeoHatchStyle, pattern, brush, Images"),

        new("Styling", "Heatmap",
            "Coyote sightings as a heatmap, with the ramp, radius, intensity and opacity changed at run time - a paint change, not a restyle",
            "MapStyle.SetHeatmapPaint", SampleDraws.Gpu, () => new Heatmap(),
            aliases: "heatmap, density, ColorStop, ramp, Viridis, heatmap-radius, shapefile"),

        new("Styling", "Style Layer Toggles",
            "Every layer a style declares, listed in style order and each one switchable - turning a layer off is a renderer decision, not a restyle",
            "MapStyle.SetLayerVisibility", SampleDraws.Gpu, () => new StyleLayerToggles(),
            aliases: "StyleLayers, visibility, layer list, toggle, show, hide"),

        // ---- 3D and the Camera ----
        new("3D and the Camera", "Terrain in 3D",
            "The ground with its real shape: satellite imagery draped over a terrain source, hill-shaded and tilted, over the Grand Canyon",
            "MapStyle.SetTerrain", SampleDraws.Gpu, () => new Terrain3D(),
            aliases: "terrain, DEM, hillshade, terrarium, elevation, drape, Grand Canyon, 3D"),

        new("3D and the Camera", "Lane Level Navigation",
            "A drive through Wolfsburg at lane level - an OpenDRIVE HD map cut into vector tiles, a follow camera, and the city's buildings standing around it",
            "DynamicSource", SampleDraws.Gpu, () => new LaneLevelNavigation(),
            aliases: "navigation, lane, OpenDRIVE, HD map, follow camera, tilt, 3D buildings, route, turn by turn"),

        // ---- Projections ----
        new("Projections", "Any Projection",
            "The world drawn in any projection a proj string can describe - Robinson, Mollweide, Eckert, Albers, a UTM zone - morphing from one to the next",
            "DisplayProjection.FromProjString", SampleDraws.Gpu, () => new AnyProjection(),
            aliases: "proj4, proj string, Robinson, Mollweide, Eckert, sinusoidal, Bonne, Albers, UTM, equal area"),

        new("Projections", "Cross the Dateline",
            "A flight from Los Angeles to Shanghai across the 180th meridian - one line stored with longitudes past -180, folded back into the world at the tile cut",
            "DisplayProjection.Globe", SampleDraws.Gpu, () => new CrossTheDateline(),
            aliases: "dateline, antimeridian, 180, seam, globe, great circle, flight, corridor, wrap"),

        new("Projections", "Measure in the Right Projection",
            "Why a measurement needs the right projection - Greenland carried away from its own latitude swells in Web Mercator while the ground it covers never changes",
            "ProjectionConverter", SampleDraws.Gpu, () => new MeasureInTheRightProjection(),
            aliases: "area, measure, equal area, laea, Web Mercator, distortion, Greenland, latitude"),

        new("Projections", "Project Features",
            "Three ways a feature gets from one projection to another - the source converting as it reads, a converter run by hand, and the ThinkGeo Cloud",
            "ProjectionConverter", SampleDraws.Gpu, () => new ProjectFeatures(),
            aliases: "projection, convert, EPSG, 2276, 4326, 3857, ProjectionCloudClient, reproject"),

        new("Projections", "The Globe",
            "The world as a globe, as web mercator and as Robinson - the same map morphing between the shapes it can be drawn in",
            "DisplayProjection", SampleDraws.Gpu, () => new TheGlobe(),
            aliases: "globe, sphere, Robinson, proj string, AnimateDisplayProjectionAsync, morph"),

        // ---- Maps from a Service - Raster ----
        new("Maps from a Service - Vector", "Vector Map Providers",
            "The same map from several vector tile providers - a new provider is a new style over the same map, and the camera holds",
            "ThinkGeoVectorTileSource", SampleDraws.Gpu, () => new VectorMapProviders(),
            aliases: "OpenFreeMap, Liberty, Positron, style.json, SetStyleAsync, restyle"),

        new("Maps from a Service - Vector", "NOAA Weather",
            "Live American weather from NOAA - every reporting station's temperature, and the warnings in force, coloured by severity",
            "NoaaWeatherStationFeatureSource", SampleDraws.Gpu, () => new NOAAWeather(),
            aliases: "NOAA, weather, stations, temperature, warnings, severity, live, step, match"),

        new("Maps from a Service - Raster", "Raster Map Providers",
            "The same map from several raster tile providers - the raster registration is swapped in place and the camera holds",
            "RasterTileSource", SampleDraws.Gpu, () => new RasterMapProviders(),
            aliases: "OpenStreetMap, XYZ, aerial, hybrid, ThinkGeoRasterTileSource"),

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
        ("Databases - Oracle", "No OCI client ships for iOS or Android; the other databases are included."),
        ("Vector File Formats - KML, CAD, GeoPDF", "Read through GDAL, whose natives ship for Windows and Linux but not for iOS or Android. The other five formats are included."),
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
