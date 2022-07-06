using System;
using System.IO;
using System.Linq;
using System.Text;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to get data from a feature in a ShapeFile
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GetDataFromFeatureSample : ContentPage
    {
        public GetDataFromFeatureSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Set up the map with the ThinkGeo Cloud Maps overlay and a feature layer containing Frisco parks data
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service. 
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsOverlay.VectorTileCache = new FileVectorTileCache(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"),
                "CloudMapsVector");

            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the Map Unit to meters (used in Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Create a feature layer to hold the Frisco parks data
            var parksLayer = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Parks.shp"));

            // Convert the Frisco shapefile from its native projection to Spherical Mercator, to match the map
            var projectionConverter = new ProjectionConverter(2276, 3857);
            parksLayer.FeatureSource.ProjectionConverter = projectionConverter;

            // Add a style to use to draw the Frisco parks polygons
            parksLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            parksLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple,
                    2);

            // Add the feature layer to an overlay, and add the overlay to the map
            var parksOverlay = new LayerOverlay();
            parksOverlay.Layers.Add("Frisco Parks", parksLayer);
            mapView.Overlays.Add(parksOverlay);

            // Add a PopupOverlay to the map, to display feature information
            var popupOverlay = new PopupOverlay();
            mapView.Overlays.Add("Info Popup Overlay", popupOverlay);

            // Set the map extent to the bounding box of the parks
            parksLayer.Open();
            mapView.CurrentExtent = parksLayer.GetBoundingBox();
            // mapView.ZoomIn();
            parksLayer.Close();

            // Refresh and redraw the map
            mapView.Refresh();
        }

        /// <summary>
        ///     Get a feature based on a location
        /// </summary>
        private Feature GetFeatureFromLocation(PointShape location)
        {
            // Get the parks layer from the MapView
            var parksLayer = mapView.FindFeatureLayer("Frisco Parks");

            // Find the feature that was tapped on by querying the layer for features containing the tapped coordinates
            parksLayer.Open();
            var selectedFeature = parksLayer.QueryTools.GetFeaturesContaining(location, ReturningColumnsType.AllColumns)
                .FirstOrDefault();
            parksLayer.Close();

            return selectedFeature;
        }

        /// <summary>
        ///     Display a popup containing a feature's info
        /// </summary>
        private void DisplayFeatureInfo(Feature feature)
        {
            var parkInfoString = new StringBuilder();

            // Each column in a feature is a data attribute
            // Add all attribute pairs to the info string
            foreach (var column in feature.ColumnValues) parkInfoString.AppendLine($"{column.Key}: {column.Value}");

            //Create a new popup with the park info string
            var popupOverlay = (PopupOverlay) mapView.Overlays["Info Popup Overlay"];
            var popup = new Popup {Position = feature.GetShape().GetCenterPoint()};
            popup.Content = parkInfoString.ToString();

            //Clear the popup overlay and add the new popup to it
            popupOverlay.Popups.Clear();
            popupOverlay.Popups.Add(popup);

            //Refresh the overlay to redraw the popups
            popupOverlay.Refresh();
        }

        /// <summary>
        ///     Pull data from the selected feature and display it when tapped
        /// </summary>
        private void MapView_OnMapSingleTap(object sender, TouchMapViewEventArgs e)
        {
            // Get the selected feature based on the map tap location
            var selectedFeature = GetFeatureFromLocation(e.PointInWorldCoordinate);

            // If a feature was selected, get the data from it and display it
            if (selectedFeature != null) DisplayFeatureInfo(selectedFeature);
        }
    }
}