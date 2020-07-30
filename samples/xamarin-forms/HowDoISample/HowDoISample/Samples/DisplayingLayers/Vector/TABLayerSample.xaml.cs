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
    public partial class TABLayerSample : ContentPage
    {
        public TABLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        private void MapView_Loaded(object sender, EventArgs e)
        {
            //// It is important to set the map unit first to either feet, meters or decimal degrees.
            //mapView.MapUnit = GeographyUnit.Meter;

            //// Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service and add it to the map.
            //var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            //mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            //// Create a new overlay that will hold our new layer and add it to the map.
            //LayerOverlay cityboundaryOverlay = new LayerOverlay();
            //mapView.Overlays.Add(cityboundaryOverlay);

            //// Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
            //TabFeatureLayer cityBoundaryLayer = new TabFeatureLayer(@"../../../Data/Tab/City_ETJ.tab");
            //cityBoundaryLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            //// Add the layer to the overlay we created earlier.
            //cityboundaryOverlay.Layers.Add("City Boundary", cityBoundaryLayer);

            //// Set this so we can use our own styles as opposed to the styles in the file.
            //cityBoundaryLayer.StylingType = TabStylingType.StandardStyling;

            //// Create an Area style on zoom level 1 and then apply it to all zoom levels up to 20.
            //cityBoundaryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(100, GeoColors.Green), GeoColors.Green);
            //cityBoundaryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Open the layer and set the map view current extent to the bounding box of the layer.
            //cityBoundaryLayer.Open();
            //mapView.CurrentExtent = cityBoundaryLayer.GetBoundingBox();

            //// Refresh the map.
            //mapView.Refresh();
        }

        /// <summary>
        /// ...
        /// </summary>
        private void Button_Clicked(object sender, EventArgs e)
        {
            // ...
        }
    }
}