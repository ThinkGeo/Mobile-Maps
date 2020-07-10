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
    /// Learn how to calculate the shortest line between two shapes
    /// </summary>
    public class CalculateShortestLineBetweenShapesSample : SampleFragment
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

            ShapeFileFeatureLayer friscoParks = new ShapeFileFeatureLayer(@"mnt/sdcard/MapSuiteSampleData/HowDoISamples/AppData/SampleData/Shapefile/Parks.shp");
            InMemoryFeatureLayer stadiumLayer = new InMemoryFeatureLayer();
            InMemoryFeatureLayer shortestLineLayer = new InMemoryFeatureLayer();
            LayerOverlay layerOverlay = new LayerOverlay();

            // Project friscoParks layer to Spherical Mercator to match the map projection
            friscoParks.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style friscoParks layer
            friscoParks.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(64, GeoColors.Green), GeoColors.DimGray);
            friscoParks.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style stadiumLayer
            stadiumLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Blue, 16, GeoColors.White, 4);
            stadiumLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style shortestLineLayer
            shortestLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Red, 2, false);
            shortestLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add friscoParks layer to a LayerOverlay
            layerOverlay.Layers.Add("friscoParks", friscoParks);

            // Add stadiumLayer layer to a LayerOverlay
            layerOverlay.Layers.Add("stadiumLayer", stadiumLayer);

            // Add shortestLineLayer to the layerOverlay
            layerOverlay.Layers.Add("shortestLineLayer", shortestLineLayer);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10782307.6877106, 3918904.87378907, -10774377.3460701, 3912073.31442403);

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            // Add Toyota Stadium feature to stadiumLayer
            var stadium = new Feature(new PointShape(-10779651.500992451, 3915933.0023557912));
            stadiumLayer.InternalFeatures.Add(stadium);
        }

        /// <summary>
        /// Calculates the shortest line from the selected park to the stadium and displays it's length and shows the line on the map
        /// </summary>
        private void MapView_OnMapClick(object sender, SingleTapMapViewEventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

            ShapeFileFeatureLayer friscoParks = (ShapeFileFeatureLayer)layerOverlay.Layers["friscoParks"];
            InMemoryFeatureLayer stadiumLayer = (InMemoryFeatureLayer)layerOverlay.Layers["stadiumLayer"];
            InMemoryFeatureLayer shortestLineLayer = (InMemoryFeatureLayer)layerOverlay.Layers["shortestLineLayer"];

            // Query the friscoParks layer to get the first feature closest to the map click event
            var park = friscoParks.QueryTools.GetFeaturesNearestTo(e.WorldPoint, GeographyUnit.Meter, 1,
                ReturningColumnsType.NoColumns).First();

            // Get the stadium feature from the stadiumLayer
            var stadium = stadiumLayer.InternalFeatures[0];

            // Get the shortest line from the selected park to the stadium
            var shortestLine = park.GetShape().GetShortestLineTo(stadium, GeographyUnit.Meter);

            // Show the shortestLine on the map
            shortestLineLayer.InternalFeatures.Clear();
            shortestLineLayer.InternalFeatures.Add(new Feature(shortestLine));
            layerOverlay.Refresh();

            // Get the area of the first feature
            var length = shortestLine.GetLength(GeographyUnit.Meter, DistanceUnit.Kilometer);

            // Display the shortestLine's length in the distanceResult TextBox
            distanceResult.Text = $"{length:f3} km";
        }
    }
}