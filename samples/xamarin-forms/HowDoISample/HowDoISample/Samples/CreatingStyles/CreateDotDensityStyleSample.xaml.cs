using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to display a CloudMapsVector Layer on the map
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateDotDensityStyleSample : ContentPage
    {
        public CreateDotDensityStyleSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();


            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~",
                "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            var housingUnitsLayer = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Frisco 2010 Census Housing Units.shp"));

            // Project the layer's data to match the projection of the map
            housingUnitsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            var housingUnitsStyle = new DotDensityStyle("H_UNITS", .1, 1, GeoColors.Blue);

            // Add and apply the ClassBreakStyle to the housingUnitsLayer
            housingUnitsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(housingUnitsStyle);
            housingUnitsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add housingUnitsLayer to a LayerOverlay
            var layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(housingUnitsLayer);

            // Add layerOverlay to the mapView
            mapView.Overlays.Add(layerOverlay);

            // Set the map extent
            housingUnitsLayer.Open();
            mapView.CurrentExtent = housingUnitsLayer.GetBoundingBox();
            housingUnitsLayer.Close();
        }
    }
}