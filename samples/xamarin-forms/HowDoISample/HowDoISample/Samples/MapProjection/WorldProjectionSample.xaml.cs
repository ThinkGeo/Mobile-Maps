using System;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorldProjectionSample : ContentPage
    {
        public WorldProjectionSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     ...
        /// </summary>
        protected override void OnAppearing()
        {
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.DecimalDegree;

            // Create a new overlay that will hold our new layer and add it to the map.
            var layerOverlay = new LayerOverlay();
            mapView.Overlays.Add("world overlay", layerOverlay);

            // Create the world layer, it will be decimal degrees at first but we will be able to change it
            var worldLayer = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Countries02.shp"));

            // Setup the styles for the countries for zoom level 1 and then apply it until zoom level 20
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.DarkSlateGray;
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush =
                new GeoSolidBrush(GeoColor.FromHtml("#C9E1BE"));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add the layer to the overlay we created earlier.
            layerOverlay.Layers.Add("world layer", worldLayer);

            mapView.CurrentExtent =
                new RectangleShape(-139.971925820039, 140.267236484135, 151.824949179961, -111.099951015865);
        }

        private async void Radial_Checked(object sender, EventArgs e)
        {
            var radioButton = (RadioButton) sender;
            var layer = mapView.FindFeatureLayer("world layer");

            if (layer == null) return;
            switch (radioButton.StyleId)
            {
                case "DecimalDegrees":
                    // Set the new projection to null as the original data is in decimal degrees. 
                    layer.FeatureSource.ProjectionConverter = null;
                    mapView.MapUnit = GeographyUnit.DecimalDegree;
                    mapView.CurrentExtent = new RectangleShape(-139.971925820039, 140.267236484135, 151.824949179961, -111.099951015865);
                    break;
                case "MgaZone55":
                    // Set the new projection converter and open it.  Next set the map to the correct map unit and new extent
                    layer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326,
                        @"+proj=utm +zone=55 +south +ellps=GRS80 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs");
                    layer.FeatureSource.ProjectionConverter.Open();
                    mapView.MapUnit = GeographyUnit.Meter;
                    mapView.CurrentExtent = new RectangleShape(-5255863.3178402, 10345432.4700869, 2850158.42260337, 3362534.22380121);
                    break;
                case "AlbersEqualAreaConic":
                    // Set the new projection converter and open it.  Next set the map to the correct map unit and new extent
                    layer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326,
                        @"+proj=aea +lat_1=20 +lat_2=60 +lat_0=40 +lon_0=-96 +x_0=0 +y_0=0 +ellps=GRS80 +datum=NAD83 +units=m no_defs");
                    layer.FeatureSource.ProjectionConverter.Open();
                    mapView.MapUnit = GeographyUnit.Meter;
                    mapView.CurrentExtent = new RectangleShape(-4167908.28780145, 3251198.24423971, 3952761.55210086, -3744318.54555566);
                    break;
                case "PolarStereographic":
                    // Set the new projection converter and open it.  Next set the map to the correct map unit and new extent
                    layer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326,
                        @"+proj=stere +lat_0=90 +lat_ts=70 +lon_0=-45 +k=1 +x_0=0 +y_0=0 +a=6378273 +b=6356889.449 +units=m +no_defs s");
                    layer.FeatureSource.ProjectionConverter.Open();
                    mapView.MapUnit = GeographyUnit.Meter;
                    mapView.CurrentExtent = new RectangleShape(-7944508.96033433, 6176987.61570341, 8296830.7194703, -7814045.96388732);
                    break;
                case "EqualAreaCylindrical":
                    // Set the new projection converter and open it.  Next set the map to the correct map unit and new extent
                    layer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326,
                        @"+proj=cea +lon_0=0 +x_0=0 +y_0=0 +lat_ts=45 +ellps=WGS84 +datum=WGS84 +units=m +no_defs");
                    layer.FeatureSource.ProjectionConverter.Open();
                    mapView.MapUnit = GeographyUnit.Meter;
                    mapView.CurrentExtent = new RectangleShape(-17364804.1443055, 14762778.3057369, 15059282.8174688, -13168814.679406);
                    break;
            }

            await mapView.RefreshAsync();
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            mapView.Dispose();
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}