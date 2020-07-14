using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// Learn how to buffer a shape
    /// </summary>
    public class BufferShapeSample : SampleFragment
    {
        private TextView bufferLabel;
        private EditText bufferDistance;
        private Button bufferButton;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            SetupSample();

            SetupMap();
        }

        /// <summary>
        /// Sets up the sample's layout and controls
        /// </summary>
        private void SetupSample()
        {
            base.OnStart();

            bufferLabel = new TextView(this.Context)
            {
                Text = "Buffer Distance (m):"
            };

            bufferDistance = new EditText(this.Context)
            {
                Text = "1000",
                InputType = InputTypes.ClassNumber
            };

            bufferButton = new Button(this.Context)
            {
                Text = "Buffer"
            };
            bufferButton.Click += BufferButton_Click;

            var gridLayout = new GridLayout(this.Context)
            {
                RowCount = 2,
                ColumnCount = 2
            };
            gridLayout.AddView(bufferLabel, new GridLayout.LayoutParams(GridLayout.InvokeSpec(0), GridLayout.InvokeSpec(0, 1f)));
            gridLayout.AddView(bufferDistance, new GridLayout.LayoutParams(GridLayout.InvokeSpec(0), GridLayout.InvokeSpec(1, 1f)));
            gridLayout.AddView(bufferButton, new GridLayout.LayoutParams(GridLayout.InvokeSpec(1), GridLayout.InvokeSpec(0, 2, 1f)));

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), this.SampleInfo, new Collection<View>() { gridLayout });
        }

        /// <summary>
        /// Sets up the map layers and styles
        /// </summary>
        private void SetupMap()
        {
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Set the zoom levels to match cloud maps
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("USlbIyO5uIMja2y0qoM21RRM6NBXUad4hjK3NBD6pD0~", "f6OJsvCDDzmccnevX55nL7nXpPDXXKANe5cN6czVjCH0s8jhpCH-2A~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            ShapeFileFeatureLayer cityLimits = new ShapeFileFeatureLayer(@"mnt/sdcard/MapSuiteSampleData/HowDoISamples/AppData/SampleData/Shapefile/FriscoCityLimits.shp");
            InMemoryFeatureLayer bufferLayer = new InMemoryFeatureLayer();
            LayerOverlay layerOverlay = new LayerOverlay();

            // Project cityLimits layer to Spherical Mercator to match the map projection
            cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style cityLimits layer
            cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style the bufferLayer
            bufferLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
            bufferLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add cityLimits to a LayerOverlay
            layerOverlay.Layers.Add("cityLimits", cityLimits);

            // Add bufferLayer to the layerOverlay
            layerOverlay.Layers.Add("bufferLayer", bufferLayer);

            // Set the map extent to the cityLimits layer bounding box
            cityLimits.Open();
            mapView.CurrentExtent = cityLimits.GetBoundingBox();
            cityLimits.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);
        }

        /// <summary>
        /// Buffers the first feature in the cityLimits layer and adds them to the bufferLayer to display on the map
        /// </summary>
        private void BufferButton_Click(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

            ShapeFileFeatureLayer cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
            InMemoryFeatureLayer bufferLayer = (InMemoryFeatureLayer)layerOverlay.Layers["bufferLayer"];

            // Query the cityLimits layer to get all the features
            cityLimits.Open();
            var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
            cityLimits.Close();

            // Buffer the first feature by the amount of the bufferDistance TextBox
            var buffer = features[0].Buffer(Convert.ToInt32(bufferDistance.Text), GeographyUnit.Meter, DistanceUnit.Meter);

            // Add the buffer shape into an InMemoryFeatureLayer to display the result.
            // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the underlying data using BeginTransaction and CommitTransaction instead.
            bufferLayer.InternalFeatures.Clear();
            bufferLayer.InternalFeatures.Add(buffer);

            // Redraw the layerOverlay to see the buffered features on the map
            layerOverlay.Refresh();
        }
    }
}