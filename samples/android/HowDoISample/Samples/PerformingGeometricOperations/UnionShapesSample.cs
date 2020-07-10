using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// Learn how to union shapes into a single shape
    /// </summary>
    public class UnionShapesSample : SampleFragment
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

            ShapeFileFeatureLayer dividedCityLimits = new ShapeFileFeatureLayer(@"mnt/sdcard/MapSuiteSampleData/HowDoISamples/AppData/SampleData/Shapefile/FriscoCityLimitsDivided.shp");
            InMemoryFeatureLayer unionLayer = new InMemoryFeatureLayer();
            LayerOverlay layerOverlay = new LayerOverlay();

            // Project dividedCityLimits layer to Spherical Mercator to match the map projection
            dividedCityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style dividedCityLimits layer
            dividedCityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(128, GeoColors.LightOrange), GeoColors.DimGray, 2);
            dividedCityLimits.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("NAME", "Segoe UI", 12, DrawingFontStyles.Bold, GeoColors.Black, GeoColors.White, 2);
            dividedCityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style unionLayer
            unionLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray, 2);
            unionLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add dividedCityLimits to a LayerOverlay
            layerOverlay.Layers.Add("dividedCityLimits", dividedCityLimits);

            // Add unionLayer to the layerOverlay
            layerOverlay.Layers.Add("unionLayer", unionLayer);

            // Set the map extent to the dividedCityLimits layer bounding box
            dividedCityLimits.Open();
            mapView.CurrentExtent = dividedCityLimits.GetBoundingBox();
            dividedCityLimits.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);
        }

        /// <summary>
        /// Unions all the features in the dividedCityLimits layer and displays the results on the map
        /// </summary>
        private void Button_Click(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

            ShapeFileFeatureLayer dividedCityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["dividedCityLimits"];
            InMemoryFeatureLayer unionLayer = (InMemoryFeatureLayer)layerOverlay.Layers["unionLayer"];

            // Query the dividedCityLimits layer to get all the features
            dividedCityLimits.Open();
            var features = dividedCityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
            dividedCityLimits.Close();

            // Union all the features into a single Multipolygon Shape
            var union = AreaBaseShape.Union(features);

            // Add the union shape into unionLayer to display the result.
            // If this were to be a permanent change to the dividedCityLimits FeatureSource, you would modify the underlying data using BeginTransaction and CommitTransaction instead.
            unionLayer.InternalFeatures.Clear();
            unionLayer.InternalFeatures.Add(new Feature(union));

            // Hide the dividedCityLimits layer
            dividedCityLimits.IsVisible = false;

            // Redraw the layerOverlay to see the unioned features on the map
            layerOverlay.Refresh();
        }
    }
}