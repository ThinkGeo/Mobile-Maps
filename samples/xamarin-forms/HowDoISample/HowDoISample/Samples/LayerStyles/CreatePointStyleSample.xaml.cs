using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreatePointStyleSample : ContentPage
    {
        private PointStyle predefinedStyle;
        private PointStyle imageStyle;
        private PointStyle fontStyle;
        private LayerOverlay layerOverlay;

        public CreatePointStyleSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, load Frisco Hotels shapefile data and add it to the map
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            // Set the map extent
            mapView.CurrentExtent =
                new RectangleShape(-10778329.017082, 3909598.36751101, -10776250.8853871, 3907890.47766975);

            var hotelsLayer = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Hotels.shp"));

            layerOverlay = new LayerOverlay();
            // Project the layer's data to match the projection of the map
            hotelsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add the layer to a layer overlay
            layerOverlay.Layers.Add("hotels", hotelsLayer);

            // Add the overlay to the map
            mapView.Overlays.Add("hotels", layerOverlay);

            // Create a predefined point style
            predefinedStyle = new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Blue, new GeoPen(GeoBrushes.White, 2));

            // Create a image point style
            imageStyle = new PointStyle(new GeoImage(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Resources/hotel_icon.png")));
            imageStyle.SymbolSize = 40;
            //imageStyle.IsActive = false;

            // Create a font point style
            fontStyle = new PointStyle(new GeoFont("Verdana", 16, DrawingFontStyles.Bold), "@", GeoBrushes.Black);
            fontStyle.Mask = new AreaStyle(GeoBrushes.White);
            fontStyle.MaskType = MaskType.Circle;
            //fontStyle.IsActive = false;

            // Add the point style to the collection of custom styles for ZoomLevel 1.
            hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(predefinedStyle);
            hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(imageStyle);
            hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(fontStyle);
            hotelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Create a pointStyle using a PointSymbol and add it to the Hotels layer
        /// </summary>
        private async void PredefinedStyle_OnChecked(object sender, EventArgs e)
        {
            predefinedStyle.IsActive = true;
            imageStyle.IsActive = false;
            fontStyle.IsActive = false;

            await layerOverlay.RefreshAsync();
        }

        /// <summary>
        ///     Create a pointStyle using an icon image and add it to the Hotels layer
        /// </summary>
        private async void ImageStyle_OnChecked(object sender, EventArgs e)
        {
            predefinedStyle.IsActive = false;
            imageStyle.IsActive = true;
            fontStyle.IsActive = false;

            await layerOverlay.RefreshAsync();
        }

        /// <summary>
        ///     Create a pointStyle using a font symbol and add it to the Hotels layer
        /// </summary>
        private async void FontStyle_Checked(object sender, EventArgs e)
        {
            predefinedStyle.IsActive = false;
            imageStyle.IsActive = false;
            fontStyle.IsActive = true;

            await layerOverlay.RefreshAsync();
        }
    }
}