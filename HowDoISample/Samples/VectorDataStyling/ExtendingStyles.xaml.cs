using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataStyling;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ExtendingStyles
{
    private bool _initialized;
    private ShapeFileFeatureLayer _worldCapitalsLayer;
    public ExtendingStyles()
    {
        InitializeComponent();
    }

    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        // Add Cloud Maps as a background overlay
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        _worldCapitalsLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "WorldCapitals.shp"));

        _worldCapitalsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        _worldCapitalsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

        var worldOverlay = new LayerOverlay();
        worldOverlay.Layers.Add("WorldCapitals", _worldCapitalsLayer);

        MapView.Overlays.Add("Overlay", worldOverlay);

        // Set the map scale and center point
        MapView.MapScale = 74000000;
        MapView.CenterPoint = new PointShape(450061, 1074668);

        await MapView.RefreshAsync();
    }

    private async void TimeBasedPointStyle_Click(object sender, EventArgs e)
    {
        var timeBasedPointStyle = new TimeBasedPointStyle
        {
            TimeZoneColumnName = "TimeZone",
            DaytimePointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Yellow, 12, GeoColors.Black),
            NighttimePointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Gray, 12, GeoColors.Black)
        };

        _worldCapitalsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
        _worldCapitalsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(timeBasedPointStyle);

        var worldOverlay = (LayerOverlay)MapView.Overlays["Overlay"];
        await worldOverlay.RefreshAsync();
    }

    private async void SizedBasedPointStyle_Click(object sender, EventArgs e)
    {
        var sizedPointStyle = new SizedPointStyle(PointStyle.CreateSimpleCircleStyle(GeoColors.Blue, 1),
            "population", 500000);

        _worldCapitalsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
        _worldCapitalsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(sizedPointStyle);

        var worldOverlay = (LayerOverlay)MapView.Overlays["Overlay"];
        await worldOverlay.RefreshAsync();
    }

    // This style draws points on the capitols with their color based on the current time and if
    // we think it's daylight there or not.
    internal class TimeBasedPointStyle(
        string timeZoneColumnName,
        PointStyle daytimePointStyle,
        PointStyle nighttimePointStyle)
        : ThinkGeo.Core.Style
    {
        public TimeBasedPointStyle()
            : this(string.Empty, new PointStyle(), new PointStyle())
        {
        }

        public PointStyle DaytimePointStyle { get; set; } = daytimePointStyle;

        public PointStyle NighttimePointStyle { get; set; } = nighttimePointStyle;

        public string TimeZoneColumnName { get; set; } = timeZoneColumnName;

        protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas,
            Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
        {
            foreach (var feature in features)
            {
                // Here we are going to do the calculation to see what
                // time it is for each feature and draw the appropriate style
                var offsetToGmt = Convert.ToSingle(feature.ColumnValues[TimeZoneColumnName]);
                var localTime = DateTime.UtcNow.AddHours(offsetToGmt);
                if (localTime.Hour is >= 7 and <= 19)
                    // Daytime
                    DaytimePointStyle.Draw(new Collection<Feature> { feature }, canvas, labelsInThisLayer,
                        labelsInAllLayers);
                else
                    //Nighttime
                    NighttimePointStyle.Draw(new Collection<Feature> { feature }, canvas, labelsInThisLayer,
                        labelsInAllLayers);
            }
        }

        protected override Collection<string> GetRequiredColumnNamesCore()
        {
            var columns = new Collection<string>();

            // Grab any columns that the daytime style may need.
            var daytimeColumns = DaytimePointStyle.GetRequiredColumnNames();
            foreach (var column in daytimeColumns)
                if (!columns.Contains(column))
                    columns.Add(column);

            // Grab any columns that the nighttime style may need.
            var nighttimeColumns = NighttimePointStyle.GetRequiredColumnNames();
            foreach (var column in nighttimeColumns)
                if (!columns.Contains(column))
                    columns.Add(column);

            // Make sure we add the timezone column
            if (!columns.Contains(TimeZoneColumnName)) columns.Add(TimeZoneColumnName);

            return columns;
        }
    }

    // This style draws a point sized with the population of the capitol.  It uses the DrawCore of the style
    // to draw directly on the canvas.  It can also leverage other styles to draw on the canvas as well.
    internal class SizedPointStyle(PointStyle pointStyle, string sizeColumnName, float ratio) : ThinkGeo.Core.Style
    {
        public SizedPointStyle()
            : this(new PointStyle(), string.Empty, 1)
        {
        }

        public PointStyle PointStyle { get; set; } = pointStyle;

        public float Ratio { get; set; } = ratio;

        public string SizeColumnName { get; set; } = sizeColumnName;

        protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas,
            Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
        {
            // Loop through each feature and determine how large the point should 
            // be then adjust it's size.
            foreach (var feature in features)
            {
                var sizeData = Convert.ToSingle(feature.ColumnValues[SizeColumnName]);
                var symbolSize = sizeData / Ratio;
                PointStyle.SymbolSize = symbolSize;
                PointStyle.Draw(new Collection<Feature> { feature }, canvas, labelsInThisLayer, labelsInAllLayers);
            }
        }

        protected override Collection<string> GetRequiredColumnNamesCore()
        {
            // Here we grab the columns from the pointStyle and then add
            // the sizeColumn name to make sure we pull back the column
            //  that we need to calculate the size
            var columns = PointStyle.GetRequiredColumnNames();
            if (!columns.Contains(SizeColumnName)) columns.Add(SizeColumnName);

            return columns;
        }
    }
}