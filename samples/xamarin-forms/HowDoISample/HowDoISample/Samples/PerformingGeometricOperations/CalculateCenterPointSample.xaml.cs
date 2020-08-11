using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.Xamarin.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalculateCenterPointSample : ContentPage
    {
        public CalculateCenterPointSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the censusHousing and centerPointLayer layers
        /// into a grouped LayerOverlay and display it on the map.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            ShapeFileFeatureLayer censusHousing = new ShapeFileFeatureLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Frisco 2010 Census Housing Units.shp"));
            InMemoryFeatureLayer centerPointLayer = new InMemoryFeatureLayer();
            LayerOverlay layerOverlay = new LayerOverlay();

            // Project censusHousing layer to Spherical Mercator to match the map projection
            censusHousing.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style censusHousing layer
            censusHousing.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            censusHousing.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style centerPointLayer
            centerPointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Green, 12, GeoColors.White, 4);
            centerPointLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(64, GeoColors.Green), GeoColors.Black, 2);
            centerPointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add censusHousing layer to a LayerOverlay
            layerOverlay.Layers.Add("censusHousing", censusHousing);

            // Add centerPointLayer to the layerOverlay
            layerOverlay.Layers.Add("centerPointLayer", centerPointLayer);

            // Set the map extent to the censusHousing layer bounding box
            censusHousing.Open();
            mapView.CurrentExtent = censusHousing.GetBoundingBox();
            censusHousing.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            centroidCenter.IsChecked = true;
        }

        /// <summary>
        /// Calculates the center point of a feature
        /// </summary>
        /// <param name="feature"> The target feature to calculate it's center point</param>
        private void CalculateCenterPoint(Feature feature)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            InMemoryFeatureLayer centerPointLayer = (InMemoryFeatureLayer)layerOverlay.Layers["centerPointLayer"];

            PointShape centerPoint;

            // Get the CenterPoint of the selected feature
            if ((bool)centroidCenter.IsChecked)
            {
                // Centroid, or geometric center, method. Accurate, but can be relatively slower on extremely complex shapes
                centerPoint = feature.GetShape().GetCenterPoint();
            }
            else
            {
                // BoundingBox method. Less accurate, but much faster
                centerPoint = feature.GetBoundingBox().GetCenterPoint();
            }

            // Show the centerPoint on the map
            centerPointLayer.InternalFeatures.Clear();
            centerPointLayer.InternalFeatures.Add("selectedFeature", feature);
            centerPointLayer.InternalFeatures.Add("centerPoint", new Feature(centerPoint));

            // Refresh the overlay to show the results
            layerOverlay.Refresh();

        }

        /// <summary>
        /// Map event that fires whenever the user clicks on the map. Gets the closest feature from the click event and calculates the center point
        /// </summary>
        private void MapView_OnMapClick(object sender, TouchMapViewEventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            ShapeFileFeatureLayer censusHousing = (ShapeFileFeatureLayer)layerOverlay.Layers["censusHousing"];

            // Query the censusHousing layer to get the first feature closest to the map click event
            var feature = censusHousing.QueryTools.GetFeaturesNearestTo(e.PointInWorldCoordinate, GeographyUnit.Meter, 1,
                ReturningColumnsType.NoColumns).First();

            CalculateCenterPoint(feature);
        }

        /// <summary>
        /// RadioButton checked event that will recalculate the center point so long as a feature was already selected
        /// </summary>
        private void RadioButton_Checked(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            InMemoryFeatureLayer centerPointLayer = (InMemoryFeatureLayer)layerOverlay.Layers["centerPointLayer"];

            // Recalculate the center point if a feature has already been selected
            if (centerPointLayer.InternalFeatures.Contains("selectedFeature"))
            {
                CalculateCenterPoint(centerPointLayer.InternalFeatures["selectedFeature"]);
            }
        }
    }
}