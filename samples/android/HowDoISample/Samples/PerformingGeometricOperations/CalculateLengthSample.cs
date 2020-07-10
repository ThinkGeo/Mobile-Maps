using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// Learn how to calculate the length of a line
    /// </summary>
    public class CalculateLengthSample : SampleFragment
    {
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

            RadioButton button = new RadioButton(this.Context);
            button.Text = "Button";
            button.Click += Button_Click;
            button.Selected = true;

            LinearLayout linearLayout = new LinearLayout(this.Context);
            linearLayout.Orientation = Orientation.Horizontal;

            linearLayout.AddView(button);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), this.SampleInfo, new Collection<View>() { linearLayout });
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

            ShapeFileFeatureLayer friscoTrails = new ShapeFileFeatureLayer(@"mnt/sdcard/MapSuiteSampleData/HowDoISamples/AppData/SampleData/Shapefile/Hike_Bike.shp");
            InMemoryFeatureLayer selectedLineLayer = new InMemoryFeatureLayer();
            LayerOverlay layerOverlay = new LayerOverlay();

            // Project friscoTrails layer to Spherical Mercator to match the map projection
            friscoTrails.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style friscoTrails layer
            friscoTrails.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Orange, 2, false);
            friscoTrails.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style selectedLineLayer
            selectedLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Green, 2, false);
            selectedLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add friscoTrails layer to a LayerOverlay
            layerOverlay.Layers.Add("friscoTrails", friscoTrails);

            // Add selectedLineLayer to the layerOverlay
            layerOverlay.Layers.Add("selectedLineLayer", selectedLineLayer);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10782307.6877106, 3918904.87378907, -10774377.3460701, 3912073.31442403);

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);
        }

        /// <summary>
        /// Calculates the length of a line selected on the map and displays it in the lengthResult TextBox
        /// </summary>
        private void MapView_OnMapClick(object sender, SingleTapMapViewEventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

            ShapeFileFeatureLayer friscoTrails = (ShapeFileFeatureLayer)layerOverlay.Layers["friscoTrails"];
            InMemoryFeatureLayer selectedLineLayer = (InMemoryFeatureLayer)layerOverlay.Layers["selectedLineLayer"];

            // Query the friscoTrails layer to get the first feature closest to the map click event
            var feature = friscoTrails.QueryTools.GetFeaturesNearestTo(e.WorldPoint, GeographyUnit.Meter, 1,
                ReturningColumnsType.NoColumns).First();

            // Show the selected feature on the map
            selectedLineLayer.InternalFeatures.Clear();
            selectedLineLayer.InternalFeatures.Add(feature);
            layerOverlay.Refresh();

            // Get the length of the first feature
            var length = ((LineBaseShape)feature.GetShape()).GetLength(GeographyUnit.Meter, DistanceUnit.Kilometer);

            // Display the selectedLine's length in the lengthResult TextBox
            lengthResult.Text = $"{length:f3} km";
        }
    }
}