﻿using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayMapScaleLineSample : ContentPage
    {
        public DisplayMapScaleLineSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     TODO: Update sample once API has been ported
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsOverlay.VectorTileCache = new FileVectorTileCache(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"),
                "CloudMapsVector");
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

            mapView.Refresh();
        }


        /// <summary>
        ///     Enable the ScaleLine and add it to the MapView (default: bottom left)
        /// </summary>
        private void DisplayScaleLine_Checked(object sender, EventArgs e)
        {
            //mapView.MapTools.ScaleLine.IsEnabled = true;
        }

        /// <summary>
        ///     Disable the ScaleLine and remove it from the MapView
        /// </summary>
        private void DisplayScaleLine_Unchecked(object sender, EventArgs e)
        {
            //mapView.MapTools.ScaleLine.IsEnabled = false;
        }
    }
}