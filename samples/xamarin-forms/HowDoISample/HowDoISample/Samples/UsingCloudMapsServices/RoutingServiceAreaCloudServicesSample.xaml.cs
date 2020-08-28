﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.XamarinForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutingServiceAreaCloudServicesSample : ContentPage
    {
        private RoutingCloudClient routingCloudClient;
        private Collection<TimeSpan> serviceAreaIntervals;

        public RoutingServiceAreaCloudServicesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service.
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the map's unit of measurement to meters (Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;            

            // Create a new feature layer to display the service areas
            InMemoryFeatureLayer serviceAreasLayer = new InMemoryFeatureLayer();

            // Add a classbreak style to display the service areas
            // We will display a different color for 15, 30, 45, and 60 minute travel times
            Collection<ClassBreak> serviceAreasClassBreaks = new Collection<ClassBreak>();
            serviceAreasClassBreaks.Add(new ClassBreak(15, AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(60, GeoColors.Green), GeoColors.Green)));
            serviceAreasClassBreaks.Add(new ClassBreak(30, AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(60, GeoColors.Yellow), GeoColors.Yellow)));
            serviceAreasClassBreaks.Add(new ClassBreak(45, AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(60, GeoColors.Orange), GeoColors.Orange)));
            serviceAreasClassBreaks.Add(new ClassBreak(60, AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(60, GeoColors.Red), GeoColors.Red)));

            ClassBreakStyle serviceAreasClassBreakStyle = new ClassBreakStyle("TravelTimeFromCenterPoint", BreakValueInclusion.IncludeValue, serviceAreasClassBreaks);
            serviceAreasLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(serviceAreasClassBreakStyle);
            serviceAreasLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Set up the legend adornment
            SetUpLegendAdornment(serviceAreasClassBreaks);

            // Add the layer to an overlay, and add the overlay to the mapview
            LayerOverlay serviceAreaOverlay = new LayerOverlay();
            serviceAreaOverlay.Layers.Add("Service Area Layer", serviceAreasLayer);
            mapView.Overlays.Add("Service Area Overlay", serviceAreaOverlay);

            // Add a simple marker overlay to display the center point of the service area
            SimpleMarkerOverlay serviceAreaMarkerOverlay = new SimpleMarkerOverlay();
            mapView.Overlays.Add("Service Area Marker Overlay", serviceAreaMarkerOverlay);

            mapView.CurrentExtent = new RectangleShape(-10895153.061011, 4016319.51333112, -10653612.0529718, 3797709.61365001);

            // Create a new set of time spans for 15, 30, 45, 60 minutes. These will be used to create the classbreaks for the routing service area request
            serviceAreaIntervals = new Collection<TimeSpan>() {
                new TimeSpan(0, 15, 0),
                new TimeSpan(0, 30, 0),
                new TimeSpan(0, 45, 0),
                new TimeSpan(1, 0, 0)
            };

            // Initialize the RoutingCloudClient with our ThinkGeo Cloud Client credentials
            routingCloudClient = new RoutingCloudClient("FSDgWMuqGhZCmZnbnxh-Yl1HOaDQcQ6mMaZZ1VkQNYw~", "IoOZkBJie0K9pz10jTRmrUclX6UYssZBeed401oAfbxb9ufF1WVUvg~~");

            // Run a sample query
            PointShape samplePoint = new PointShape(-10776836.140633, 3912350.714164);
            GetAndDrawServiceArea(samplePoint);

            mapView.Refresh();
        }

        /// <summary>
        /// Get the service area from a given point on the map
        /// </summary>
        private async Task<CloudRoutingGetServiceAreaResult> GetServiceArea(PointShape centerpoint)
        {
            // Set options for the service area request
            // We can control options like Travel Direction and Contour Granularity
            CloudRoutingGetServiceAreaOptions options = new CloudRoutingGetServiceAreaOptions();
            options.DistanceUnit = DistanceUnit.Meter;

            // Set the srid for the query to 3857 (Spherical Mercator)
            int srid = 3857;

            // Run the service area query
            // Pass in the service area intervals. These will be used as the service areas for the query (15, 30, 45 60 minutes)
            CloudRoutingGetServiceAreaResult getServiceAreaResult = await routingCloudClient.GetServiceAreaAsync(centerpoint, srid, serviceAreaIntervals, options);
            return getServiceAreaResult;
        }

        /// <summary>
        /// Draw the ServiceArea polygons on the map
        /// </summary>
        private void DrawServiceArea(CloudRoutingGetServiceAreaResult result)
        {
            CloudRoutingServiceAreaResult serviceAreaResult = result.ServiceAreaResult;

            // Get the simple marker overlay from the map
            SimpleMarkerOverlay serviceAreaMarkerOverlay = (SimpleMarkerOverlay)mapView.Overlays["Service Area Marker Overlay"];

            // Clear the previous markers
            serviceAreaMarkerOverlay.Markers.Clear();

            // Add the service area center point marker to the map
            serviceAreaMarkerOverlay.Markers.Add(CreateNewMarker(new PointShape(serviceAreaResult.Waypoint.Coordinate)));

            // Get the service area polygons layer from the map
            InMemoryFeatureLayer serviceAreaLayer = (InMemoryFeatureLayer)mapView.FindFeatureLayer("Service Area Layer");

            // Clear the previous polygons
            serviceAreaLayer.InternalFeatures.Clear();

            // Add the new service area polygons to the map
            for (int i = 0; i < serviceAreaIntervals.Count; i++)
            {
                // Add a 'TravelTimeFromCenterPoint' attribute for the class break style
                Dictionary<string, string> columnValues = new Dictionary<string, string>();
                columnValues.Add("TravelTimeFromCenterPoint", serviceAreaIntervals[i].TotalMinutes.ToString());

                // Add each polygon to the feature layer
                BaseShape serviceAreaPolygon = serviceAreaResult.ServiceAreas[i];
                serviceAreaLayer.InternalFeatures.Add(new Feature(serviceAreaPolygon, columnValues));
            }

            // Zoom to the extent of the service area and refresh the map
            serviceAreaLayer.Open();
            mapView.CurrentExtent = serviceAreaLayer.GetBoundingBox();
            serviceAreaLayer.Close();

            mapView.Refresh();
        }

        /// <summary>
        /// Draw the new point and service area on the map
        /// </summary>
        private async void GetAndDrawServiceArea(PointShape point)
        {
            // Show a loading graphic to let users know the request is running
            //loadingImage.Visibility = Visibility.Visible;

            // Run the service area query
            CloudRoutingGetServiceAreaResult getServiceAreaResult = await GetServiceArea(point);

            // Hide the loading graphic
            //loadingImage.Visibility = Visibility.Hidden;

            // Handle an exception returned from the service
            if (getServiceAreaResult.Exception != null)
            {
                await DisplayAlert("Alert", getServiceAreaResult.Exception.Message, "Error");
                return;
            }

           // Draw the result on the map
            DrawServiceArea(getServiceAreaResult);
        }

        private void SetUpLegendAdornment(Collection<ClassBreak> classBreaks)
        {
            //Create a legend adornment based on the service area classbreaks
            LegendAdornmentLayer legend = new LegendAdornmentLayer();

            // Set up the legend adornment
            legend.Title = new LegendItem()
            {
                TextStyle = new TextStyle("Travel Times", new GeoFont("Verdana", 10, DrawingFontStyles.Bold), GeoBrushes.Black)
            };
            legend.Location = AdornmentLocation.LowerRight;
            mapView.AdornmentOverlay.Layers.Add(legend);

            // Add a LegendItems to the legend adornment for each ClassBreak
            foreach (var classBreak in classBreaks)
            {
                var legendItem = new LegendItem()
                {
                    ImageStyle = classBreak.DefaultAreaStyle,
                    TextStyle = new TextStyle($@"<{classBreak.Value} minutes", new GeoFont("Verdana", 10), GeoBrushes.Black)
                };
                legend.LegendItems.Add(legendItem);
            }
        }

        /// <summary>
        /// Perform the service area query when a new point is drawn
        /// </summary>
        private void MapView_OnMapClick(object sender, TouchMapViewEventArgs e)
        {
            GetAndDrawServiceArea(e.PointInWorldCoordinate);
        }

        /// <summary>
        /// Create a new map marker using preloaded image assets
        /// </summary>
        private Marker CreateNewMarker(PointShape point)
        {
            return new Marker()
            { 
                ImageSource = (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "/Resources/AQUA.png")),
                YOffset = -17
            };
        }
    }
}
