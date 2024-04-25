using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateLineStyleSample : ContentPage
    {
        public CreateLineStyleSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, load Frisco Streets shapefile data and add it to the map
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudRasterMapsOverlay(
                "AOf22-EmFgIEeK4qkdx5HhwbkBjiRCmIDbIYuP8jWbc~",
                "xK0pbuywjaZx4sqauaga8DMlzZprz0qQSjLTow90EhBx5D8gFd2krw~~", ThinkGeoCloudRasterMapsMapType.Aerial_V2_X2);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the map extent
            mapView.CurrentExtent =
                new RectangleShape(-10779675.1746605, 3914631.77546835, -10779173.5566652, 3914204.80300804);

            // Create a layer with line data
            var friscoRailroad = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Railroad/Railroad.shp"));
            var layerOverlay = new LayerOverlay();

            // Project the layer's data to match the projection of the map
            friscoRailroad.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Create a line style
            var lineStyle = new LineStyle(new GeoPen(GeoBrushes.DimGray, 6), new GeoPen(GeoBrushes.WhiteSmoke, 4));
            // Add the line style to the collection of custom styles for ZoomLevel 1.
            friscoRailroad.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = lineStyle;
            // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the line style on every zoom level on the map. 
            friscoRailroad.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add the layer to a layer overlay
            layerOverlay.Layers.Add("Railroad", friscoRailroad);

            // Add the overlay to the map
            mapView.Overlays.Add("overlay", layerOverlay);
        }

        private async void rbLineStyle_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (mapView.Overlays.Count <= 0) return;

            var radioButton = sender as Xamarin.Forms.RadioButton;
            if (!radioButton.IsChecked)
                return;

            var layerOverlay = (LayerOverlay) mapView.Overlays["overlay"];
            var friscoRailroad = (ShapeFileFeatureLayer) layerOverlay.Layers["Railroad"];

            // Create a line style
            var lineStyle = new LineStyle(new GeoPen(GeoBrushes.DimGray, 6), new GeoPen(GeoBrushes.WhiteSmoke, 4));

            // Add the line style to the collection of custom styles for ZoomLevel 1.
            friscoRailroad.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = lineStyle;
            // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the line style on every zoom level on the map. 
            friscoRailroad.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Refresh the layerOverlay to show the new style
            await layerOverlay.RefreshAsync();
        }

        private async void rbDashedLineStyle_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (mapView.Overlays.Count <= 0) return;

            var radioButton = sender as Xamarin.Forms.RadioButton;
            if (!radioButton.IsChecked)
                return;

            var layerOverlay = (LayerOverlay) mapView.Overlays["overlay"];
            var friscoRailroad = (ShapeFileFeatureLayer) layerOverlay.Layers["Railroad"];

            var lineStyle = new LineStyle(
                new GeoPen(GeoColors.Black, 6),
                new GeoPen(GeoColors.White, 4)
                {
                    DashStyle = LineDashStyle.Custom,
                    DashPattern = {3f, 3f},
                    StartCap = DrawingLineCap.Flat,
                    EndCap = DrawingLineCap.Flat
                }
            );

            // Add the line style to the collection of custom styles for ZoomLevel 1.
            friscoRailroad.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = lineStyle;
            // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the line style on every zoom level on the map. 
            friscoRailroad.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Refresh the layerOverlay to show the new style
            await layerOverlay.RefreshAsync();
        }
    }
}