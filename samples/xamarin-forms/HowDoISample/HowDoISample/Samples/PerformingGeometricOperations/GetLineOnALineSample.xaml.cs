using System;
using System.IO;
using System.Linq;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn how to get part of a line from an existing line
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GetLineOnALineSample : ContentPage
    {
        public GetLineOnALineSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, add the railway and subLineLayer layers into a
        ///     grouped LayerOverlay and display them on the map.
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

            var railway = new InMemoryFeatureLayer();
            var subLineLayer = new InMemoryFeatureLayer();
            var layerOverlay = new LayerOverlay();

            // Add the rail line feature to the railway layer
            railway.InternalFeatures.Add(new Feature(
                "LineString (-10776730.91861553490161896 3925750.69222266925498843, -10778989.31895966082811356 3915278.00731692276895046, -10781766.12723691388964653 3909228.15506267035380006, -10782065.98029803484678268 3907458.59967381786555052, -10781867.48601813986897469 3905465.21030976390466094)"));

            // Style railway layer
            railway.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
                LineStyle.CreateSimpleLineStyle(GeoColors.Red, 2, false);
            railway.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style the subLineLayer
            subLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
                LineStyle.CreateSimpleLineStyle(GeoColors.Green, 2, false);
            subLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add railway to the layerOverlay
            layerOverlay.Layers.Add("railway", railway);

            // Add subLineLayer to the layerOverlay
            layerOverlay.Layers.Add("subLineLayer", subLineLayer);

            // Set the map extent to the railway layer bounding box
            railway.Open();
            mapView.CurrentExtent = railway.GetBoundingBox();
            railway.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Get a subLine of a line and displays it on the map
        /// </summary>
        private async void GetSubLine_OnClick(object sender, EventArgs e)
        {
            var layerOverlay = (LayerOverlay) mapView.Overlays["layerOverlay"];

            var railway = (InMemoryFeatureLayer) layerOverlay.Layers["railway"];
            var subLineLayer = (InMemoryFeatureLayer) layerOverlay.Layers["subLineLayer"];

            // Query the railway layer to get all the features
            railway.Open();
            var feature = railway.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns).First();
            railway.Close();

            // Get the subLine from the railway line shape
            var subLine = ((LineShape) feature.GetShape()).GetLineOnALine(StartingPoint.FirstPoint,
                Convert.ToDouble(startingOffset.Text), Convert.ToDouble(distance.Text), GeographyUnit.Meter,
                DistanceUnit.Meter);

            // Add the subLine into an InMemoryFeatureLayer to display the result.
            subLineLayer.InternalFeatures.Clear();
            subLineLayer.InternalFeatures.Add(new Feature(subLine));

            // Redraw the layerOverlay to see the subLine on the map
            await layerOverlay.RefreshAsync();
        }
    }
}