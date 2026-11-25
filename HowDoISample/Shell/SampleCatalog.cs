namespace HowDoISample;

using HowDoISample.Samples;

/// <summary>
/// The one catalog. The menu and the search index are both projections of this
/// list, so a sample is described once.
/// <para>
/// The groups are in the order someone meets the SDK in: get a map on screen, point
/// it at a service, put your own data in, make it look right. Data is grouped by
/// who holds it - a service somebody else operates is From a Service, a store you
/// own is Your Data.
/// </para>
/// <para>
/// The source follows the menu: a row's page is Samples/&lt;group&gt;/&lt;title&gt;.cs, and
/// its class has the file's name - "Map Navigation", "Buildings in 3D" is the class
/// BuildingsIn3D in Samples/MapNavigation/BuildingsIn3D.cs. Renaming a row or moving
/// it to another group means renaming or moving its file with it.
/// </para>
/// </summary>
public static class SampleCatalog
{
    public static readonly IReadOnlyList<SampleEntry> All = new SampleEntry[]
    {
        // ---- Map Navigation ----
        new("Map Navigation", "Buildings in 3D",
            "A city with its real height: one extrusion layer over a published style stands the Upper East Side up beside Central Park, and a switch moves the whole map between the light and dark themes",
            "GpuBasemap, fill-extrusion", () => new BuildingsIn3D(),
            aliases: "buildings, 3D buildings, extrusion, fill-extrusion, city, skyline, height, tilt, "
                   + "style.json, SetStyleAsync, dark theme, light theme, first map, New York, Manhattan"),
        new("Map Navigation", "Terrain and Hillshade",
            "The ground with its real shape: satellite imagery draped over a terrain source with roads drawn on the photograph, and a switch to a hill-shaded vector map, at Mount Rainier",
            "MapStyle.SetTerrain, AddStyle", () => new TerrainAndHillshade(),
            aliases: "terrain in 3D, terrain, DEM, hillshade, terrarium, elevation, drape, Mount Rainier, mountain, 3D, "
                   + "hybrid, roads over imagery, shaded relief, buildings, onlyLayerTypes"),
        new("Map Navigation", "GPS Navigation",
            "A drive along a recorded GPS track - the camera flies to each point, the vehicle rides the flight's progress, and the road behind turns green",
            "DynamicSource", () => new GpsNavigation(),
            aliases: "navigation, GPS, GPX, track, vehicle, follow camera, route, theme, tilt, turn by turn, live location"),
        new("Map Navigation", "Restrict Map Extent",
            "Keeping the map inside a box and between two scales - tilt it, turn it and zoom it as you like, it still cannot leave the red box",
            "CurrentExtentChanging", () => new RestrictMapExtent(),
            aliases: "restrict, bounds, clamp, MinScale, MaxScale, limit, pan, zoom, tilt, rotate"),
        new("Map Navigation", "Globe and Projections",
            "The world as a sphere and as three projections, morphing between them, from satellite imagery or vector streets - the projection is the camera's, so nothing is reprojected",
            "DisplayProjection", () => new GlobeAndProjections(),
            aliases: "globe, sphere, projection, Robinson, Mollweide, Web Mercator, "
                   + "proj4, proj string, AnimateDisplayProjectionAsync, morph, raster, vector"),

        // ---- From a Service ----
        new("From a Service", "Vector Map Providers",
            "The same map from several vector tile providers - a new provider is a new style over the same map, and the camera holds",
            "ThinkGeoVectorTileSource", () => new VectorMapProviders(),
            aliases: "OpenFreeMap, Liberty, Positron, style.json, SetStyleAsync, restyle"),
        new("From a Service", "Raster Map Providers",
            "Rasters nobody can send as vectors - aerial imagery, an open tile server, a WMS and a WMTS - each swapped in place with the camera holding",
            "RasterTileSource, WmsRasterTileSource, WmtsRasterTileSource", () => new RasterMapProviders(),
            aliases: "OpenStreetMap, XYZ, aerial, satellite, imagery, WMS, WMTS, NASA, Swisstopo, ThinkGeoRasterTileSource"),
        new("From a Service", "Live Flights",
            "Every aircraft airborne right now, from OpenSky, each dead-reckoned forward from its last report at fifty frames a second",
            "DynamicSource", () => new LiveFlights(),
            aliases: "OpenSky, flights, aircraft, live, dead reckoning, markers, animation, heading"),
        new("From a Service", "Weather Radar",
            "The last half hour of American weather radar, looping - six images fetched once and switched by visibility, so nothing is re-fetched between frames",
            "GeoImageRasterTileSource", () => new WeatherRadar(),
            aliases: "radar, NEXRAD, nexrad, weather, reflectivity, Mesonet, animation, TransparentColor, SetLayerVisibility"),

        // ---- Your Data ----
        new("Your Data", "Vector File Formats",
            "The same map from six vector file formats - shapefile, GeoJSON, GPX, MapInfo TAB, TinyGeo and spatial SQLite - with the style never learning which it was",
            "FeatureSource", () => new VectorFileFormats(),
            aliases: "shapefile, GeoJSON, KML, GPX, TAB, MapInfo, TinyGeo, SQLite, spatial, formats, read, connection string, offline, on the device, no network"),
        new("Your Data", "Offline Tiles",
            "A map with no network at all: tiles read from an MBTiles or PMTiles archive on the device, vector or raster",
            "MbTilesVectorTileSource", () => new OfflineTiles(),
            aliases: "offline, mbtiles, pmtiles, archive, no network, PmTilesRasterTileSource, Stamen"),

        // ---- Styling ----
        new("Styling", "Render Points, Lines and Polygons",
            "The three shapes of vector data, each with its own style - parks as fills, streets as lines, hotels as circles, and a label layer over each",
            "MapStyle", () => new RenderPointsLinesAndPolygons(),
            aliases: "fill, line, circle, symbol, style.json, shapefile, labels, source-layer"),
        new("Styling", "Heatmap",
            "Coyote sightings as a heatmap, with the ramp, radius and intensity changed at run time - a paint change, not a restyle",
            "MapStyle.SetHeatmapPaint", () => new Heatmap(),
            aliases: "heatmap, density, ColorStop, ramp, Viridis, heatmap-radius, shapefile"),
        new("Styling", "Isolines",
            "Contours drawn from scattered measurements - mosquito counts turned into isolines, with the level count and search radius live",
            "MapStyle.AddIsolines", () => new Isolines(),
            aliases: "isoline, contour, interpolation, IDW, PointValueSource, IsolineStyle, levels"),
        new("Styling", "Weather Grids",
            "Two forecast grids over the same country - forty-four hours of sky cover played as an animation, and the surface wind as particles carried along it",
            "MapStyle.AddGridFill, MapStyle.AddParticleFlow", () => new WeatherGrids(),
            aliases: "weather, NDFD, forecast, sky, cloud, grid fill, animation, wind, particle, flow, vector field, advection, GRIB"),

        // ---- Markers and Popups ----
        new("Markers and Popups", "Markers",
            "Markers are views on the map, not drawings in it - a pin whose tip points at its ground point, an animated one, and tap to add or remove",
            "SimpleMarkerOverlay", () => new Markers(),
            aliases: "marker, pin, icon, animated, tap, add, remove, ground point, tilt"),
        new("Markers and Popups", "Popups",
            "A popup for every hotel, each one holding its ground point as the map is panned, zoomed and tilted",
            "PopupOverlay", () => new Popups(),
            aliases: "popup, callout, label, hotel, overlay, tilt, ground point"),
        new("Markers and Popups", "Cluster Markers",
            "Coyote sightings clustered into tappable discs, rebuilt for the view each time it settles, with a tap zooming to what is inside",
            "SimpleMarkerOverlay", () => new ClusterMarkers(),
            aliases: "cluster, marker, bucket, grid, zoom, sightings, aggregate"),

        // ---- Map Tools and Adornments ----
        new("Map Tools and Adornments", "Scale Line and Scale Bar",
            "The two scale adornments drawn over the GPU basemap, in imperial, metric or both",
            "ScaleLineAdornmentLayer", () => new ScaleLineAndScaleBar(),
            aliases: "scale bar, scale line, ScaleBarAdornmentLayer, UnitSystem, miles, kilometres"),
        new("Map Tools and Adornments", "Graticule Lines and Legend",
            "The two adornments that frame a map - a degree grid labelled down the edges and a legend whose swatches are the styles painting the states",
            "GraticuleAdornmentLayer, LegendAdornmentLayer", () => new GraticuleLinesAndLegend(),
            aliases: "graticule, grid, lat lon, degrees, legend, swatch, class break, key, population, AdornmentOverlay, adornment"),
        new("Map Tools and Adornments", "Export the Map to PDF",
            "The map on screen written out as a true vector PDF - geometry as paths, labels as selectable text - and handed to the system to open, file or print",
            "PdfGeoCanvas, PrinterLayoutDocument, MvtTilesAsyncLayer", () => new ExportTheMapToPdf(),
            aliases: "print, printing, PDF, export, vector, page, A4, letter, layout, share, AirPrint, PrinterLayoutDocument"),

        // ---- Shapes and Editing ----
        new("Shapes and Editing", "Measure Distance and Area",
            "Measuring what a finger draws - a line's length or a polygon's area, taken on the ground; double-tap or long-press to finish the shape",
            "TrackInteractiveOverlay, GetLength, GetArea", () => new MeasureDistanceAndArea(),
            aliases: "measure, distance, area, ruler, track, draw, polygon, line, projection, double tap, long press"),
        new("Shapes and Editing", "Edit Features",
            "Drawing and editing by hand - draw points, lines and polygons or drag, resize, rotate and reshape what is there, with every overlay event logged",
            "TrackOverlay, EditOverlay", () => new EditFeatures(),
            aliases: "draw, edit, track, vertex, drag, resize, rotate, polygon, line, point, measure"),
        new("Shapes and Editing", "Shape Operations",
            "What a shape can be asked to become - buffered, simplified, rotated, scaled, translated, hulled, enveloped, intersected, differenced, unioned",
            "AreaBaseShape", () => new ShapeOperations(),
            aliases: "buffer, simplify, rotate, scale, translate, convex hull, envelope, intersection, difference, union"),
        new("Shapes and Editing", "Find Features by Spatial Relation",
            "Tap the zoning map and either the area you are standing in or everything within five hundred metres lights up",
            "QueryTools.GetFeaturesContaining", () => new FindFeaturesBySpatialRelation(),
            aliases: "spatial query, contains, within distance, zoning, QueryTools, tap"),
    };

    /// <summary>The rows matching a search box's text, in catalog order.</summary>
    public static IReadOnlyList<SampleEntry> Search(string text)
        => All.Where(entry => entry.Matches(text)).ToList();
}

/// <summary>
/// One menu group and the rows under it. It is the list of rows rather than holding
/// one: a grouped CollectionView binds to a collection of collections.
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
