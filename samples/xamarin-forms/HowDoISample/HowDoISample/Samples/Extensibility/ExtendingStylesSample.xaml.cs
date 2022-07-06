using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Style = ThinkGeo.Core.Style;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExtendingStylesSample : ContentPage
    {
        public ExtendingStylesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~",
                "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            var worldCapitalsLayer = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/WorldCapitals.shp"));
            worldCapitalsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            worldCapitalsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            var worldOverlay = new LayerOverlay();
            worldOverlay.Layers.Add("WorldCapitals", worldCapitalsLayer);

            mapView.Overlays.Add("Overlay", worldOverlay);

            mapView.CurrentExtent =
                new RectangleShape(-15360785.1188513, 14752615.1010077, 16260907.558937, -12603279.9259404);

            mapView.Refresh();
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            mapView.Dispose();
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        private void TimeBasedPointStyle_Click(object sender, EventArgs e)
        {
            var worldCapitalsLayer = mapView.FindFeatureLayer("WorldCapitals");

            var timeBasedPointStyle = new TimeBasedPointStyle();
            timeBasedPointStyle.TimeZoneColumnName = "TimeZone";
            timeBasedPointStyle.DaytimePointStyle =
                PointStyle.CreateSimpleCircleStyle(GeoColors.Yellow, 12, GeoColors.Black);
            timeBasedPointStyle.NighttimePointStyle =
                PointStyle.CreateSimpleCircleStyle(GeoColors.Gray, 12, GeoColors.Black);

            worldCapitalsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            worldCapitalsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(timeBasedPointStyle);

            mapView.Refresh();
        }

        private void SizedBasedPointStyle_Click(object sender, EventArgs e)
        {
            var worldCapitalsLayer = mapView.FindFeatureLayer("WorldCapitals");

            var sizedpointStyle = new SizedPointStyle(PointStyle.CreateSimpleCircleStyle(GeoColors.Blue, 1),
                "population", 500000);

            worldCapitalsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            worldCapitalsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(sizedpointStyle);

            mapView.Refresh();
        }
    }

    // This style draws points on the capitols with their color based on the current time and if
    // we think it's daylight there or not.
    internal class TimeBasedPointStyle : Style
    {
        public TimeBasedPointStyle()
            : this(string.Empty, new PointStyle(), new PointStyle())
        {
        }

        public TimeBasedPointStyle(string timeZoneColumnName, PointStyle daytimePointStyle,
            PointStyle nighttimePointStyle)
        {
            this.TimeZoneColumnName = timeZoneColumnName;
            this.DaytimePointStyle = daytimePointStyle;
            this.NighttimePointStyle = nighttimePointStyle;
        }

        public PointStyle DaytimePointStyle { get; set; }

        public PointStyle NighttimePointStyle { get; set; }

        public string TimeZoneColumnName { get; set; }


        protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas,
            Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
        {
            foreach (var feature in features)
            {
                // Here we are going to do the calculation to see what
                // time it is for each feature and draw the appropriate style
                var offsetToGmt = Convert.ToSingle(feature.ColumnValues[TimeZoneColumnName]);
                var localTime = DateTime.UtcNow.AddHours(offsetToGmt);
                if (localTime.Hour >= 7 && localTime.Hour <= 19)
                    // Daytime
                    DaytimePointStyle.Draw(new Collection<Feature> {feature}, canvas, labelsInThisLayer,
                        labelsInAllLayers);
                else
                    //Nighttime

                    NighttimePointStyle.Draw(new Collection<Feature> {feature}, canvas, labelsInThisLayer,
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
    internal class SizedPointStyle : Style
    {
        public SizedPointStyle()
            : this(new PointStyle(), string.Empty, 1)
        {
        }

        public SizedPointStyle(PointStyle pointStyle, string sizeColumnName, float ratio)
        {
            this.PointStyle = pointStyle;
            this.SizeColumnName = sizeColumnName;
            this.Ratio = ratio;
        }

        public PointStyle PointStyle { get; set; }

        public float Ratio { get; set; }

        public string SizeColumnName { get; set; }

        protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas,
            Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
        {
            // Loop through each feaure and determine how large the point should 
            // be then adjust it's size.
            foreach (var feature in features)
            {
                var sizeData = Convert.ToSingle(feature.ColumnValues[SizeColumnName]);
                var symbolSize = sizeData / Ratio;
                PointStyle.SymbolSize = symbolSize;
                PointStyle.Draw(new Collection<Feature> {feature}, canvas, labelsInThisLayer, labelsInAllLayers);
            }
        }

        protected override Collection<string> GetRequiredColumnNamesCore()
        {
            // Here we grab the columns from the pointStyle and then add
            // the sizeColumn name to make sure we pull back the column
            //  that we need to calculate the size
            var columns = new Collection<string>();
            columns = PointStyle.GetRequiredColumnNames();
            if (!columns.Contains(SizeColumnName)) columns.Add(SizeColumnName);

            return columns;
        }
    }
}