﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.Xamarin.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayMapMouseCoordinatesSample : ContentPage
    {
        public DisplayMapMouseCoordinatesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            //// Set the map's unit of measurement to meters(Spherical Mercator)
            //mapView.MapUnit = GeographyUnit.Meter;

            //// Add Cloud Maps as a background overlay
            //var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            //mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            //// Set the map extent
            //mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

        }


        /// <summary>
        /// Sets the visibility of the MouseCoordinates to true
        /// </summary>
        private void DisplayMouseCoordinates_Checked(object sender, EventArgs e)
        {
           // mapView.MapTools.MouseCoordinate.IsEnabled = true;
        }

        /// <summary>
        /// Sets the visibility of the MouseCoordinates to false
        /// </summary>
        private void DisplayMouseCoordinates_Unchecked(object sender, EventArgs e)
        {
            //mapView.MapTools.MouseCoordinate.IsEnabled = false;
        }

        /// <summary>
        /// Changes the display format of the MouseCoordinates based on ComboBox selection
        /// </summary>
        private void CoordinateType_SelectionChanged(object sender, EventArgs e)
        {
            //switch (((ComboBoxItem)coordinateType.SelectedItem).Content)
            //{
            //    case "(lat), (lon)":
            //        // Set to Lat, Lon format
            //        mapView.MapTools.MouseCoordinate.MouseCoordinateType = MouseCoordinateType.LatitudeLongitude;
            //        break;
            //    case "(lon), (lat)":
            //        // Set to Lon, Lat format
            //        mapView.MapTools.MouseCoordinate.MouseCoordinateType = MouseCoordinateType.LongitudeLatitude;
            //        break;
            //    case "(degrees), (minutes), (seconds)":
            //        // Set to Degrees, Minutes, Seconds format
            //        mapView.MapTools.MouseCoordinate.MouseCoordinateType = MouseCoordinateType.DegreesMinutesSeconds;
            //        break;
            //    case "(custom)":
            //        // Set to a custom format
            //        mapView.MapTools.MouseCoordinate.MouseCoordinateType = MouseCoordinateType.Custom;
            //        // Add an EventHandler to handle what the formatted output should look like
            //        mapView.MapTools.MouseCoordinate.CustomFormatted += new System.EventHandler<CustomFormattedMouseCoordinateMapToolEventArgs>(MouseCoordinate_CustomMouseCoordinateFormat);
            //        break;
            //}
        }

        /// <summary>
        /// Event handler that formats the MouseCoordinates to use WorldCoordinates and changes the Foreground color to red.
        /// Other modifications to the display of the MouseCoordinates can be safely done here.
        /// </summary>
        //private void MouseCoordinate_CustomMouseCoordinateFormat(object sender, CustomFormattedMouseCoordinateMapToolEventArgs e)
        //{
        //    ((MouseCoordinateMapTool)sender).Foreground = new SolidColorBrush(Colors.Red);
        //    e.Result = $"X: {e.WorldCoordinate.X.ToString("N0")}, Y: {e.WorldCoordinate.Y.ToString("N0")}";
        //}
    }
}