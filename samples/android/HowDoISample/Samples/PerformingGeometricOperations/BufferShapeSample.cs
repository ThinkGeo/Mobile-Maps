using System;
using System.IO;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;
using Xamarin.Essentials;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class BufferShapeSample : Fragment
    {
        MapView mapView;
        EditText bufferAmount;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            var view = inflater.Inflate(Resource.Layout.PerformingGeometricOperations_BufferShapeSample, container, false);

            var bottomSheet = view.FindViewById<LinearLayout>(Resource.Id.bottom_sheet);
            var behavior = BottomSheetBehavior.From(bottomSheet);
            behavior.PeekHeight = bottomSheet.Height;
            behavior.Hideable = true;

            var fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += (sender, e) =>
            {
                switch (behavior.State)
                {
                    case BottomSheetBehavior.StateCollapsed:
                        behavior.State = BottomSheetBehavior.StateExpanded;
                        break;
                    case BottomSheetBehavior.StateExpanded:
                        behavior.State = BottomSheetBehavior.StateCollapsed;
                        break;
                    default:
                        break;
                }
            };

            mapView = view.FindViewById<MapView>(Resource.Id.mapView);
            bufferAmount = view.FindViewById<EditText>(Resource.Id.bufferAmount);

            var bufferButton = view.FindViewById<Button>(Resource.Id.bufferButton);
            bufferButton.Click += BufferButton_Click;

            SetupMap();

            return view;
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

            ShapeFileFeatureLayer cityLimits = new ShapeFileFeatureLayer(Path.Combine(FileSystem.AppDataDirectory, "AppData/SampleData/Shapefile/FriscoCityLimits.shp"));
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

            // Buffer the first feature by the amount of the bufferAmount TextBox
            var buffer = features[0].Buffer(Convert.ToInt32(bufferAmount.Text), GeographyUnit.Meter, DistanceUnit.Meter);

            // Add the buffer shape into an InMemoryFeatureLayer to display the result.
            // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the underlying data using BeginTransaction and CommitTransaction instead.
            bufferLayer.InternalFeatures.Clear();
            bufferLayer.InternalFeatures.Add(buffer);

            // Redraw the layerOverlay to see the buffered features on the map
            layerOverlay.Refresh();
        }

        public override void OnDestroy()
        {
            if (mapView != null)
            {
                mapView.Dispose();
            }

            base.OnDestroy();
        }
    }
}