using System;
using System.Collections.ObjectModel;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateValueStyleSample : ContentPage
    {
        public CreateValueStyleSample()
        {
            InitializeComponent();
        }


        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay. Also, project and style the friscoCrime layer
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

            var friscoCrime = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Frisco_Crime.shp"));
            var legend = new LegendAdornmentLayer();

            // Project the layer's data to match the projection of the map
            friscoCrime.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add friscoCrimeLayer to a LayerOverlay
            var layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(friscoCrime);

            // Setup the legend adornment
            legend.Title = new LegendItem
            {
                TextStyle = new TextStyle("Crime Categories", new GeoFont("Verdana", 10, DrawingFontStyles.Bold),
                    GeoBrushes.Black)
            };
            legend.Height = 600;
            legend.Location = AdornmentLocation.LowerRight;
            mapView.AdornmentOverlay.Layers.Add(legend);

            AddValueStyle(friscoCrime, legend);

            // Add layerOverlay to the mapView
            mapView.Overlays.Add(layerOverlay);

            // Set the map extent
            mapView.CurrentExtent =
                new RectangleShape(-10780196.9469504, 3916119.49665258, -10776231.7761301, 3912703.71697007);

            await mapView.RefreshAsync();
        }

        /// <summary>
        ///     Adds a ValueStyle to the friscoCrime layer that represents each OffenseGroup as a different color
        /// </summary>
        private void AddValueStyle(ShapeFileFeatureLayer friscoCrime, LegendAdornmentLayer legend)
        {
            // Get all the distinct OffenseGroups in the friscoCrime data
            friscoCrime.Open();
            var offenseGroups = friscoCrime.FeatureSource.GetDistinctColumnValues("OffenseGro");
            friscoCrime.Close();

            // Create a set of colors to represent each OffenseGroup using a spectrum starting from red
            var colors = GeoColor.GetColorsInQualityFamily(GeoColors.Red, offenseGroups.Count);

            // Create a ValueItem styled with a PointStyle to represent each instance of an OffenseGroup
            var valueItems = new Collection<ValueItem>();
            foreach (var offenseGroup in offenseGroups)
            {
                // Create a PointStyle to represent the OffenseGroup by selecting a color using the index of the OffenseGroup
                var style = PointStyle.CreateSimpleCircleStyle(colors[offenseGroups.IndexOf(offenseGroup)], 10,
                    GeoColors.Black, 2);

                // Create a ValueItem that will house the pointStyle for the OffenseGroup
                valueItems.Add(new ValueItem(offenseGroup.ColumnValue, style));

                // Add a LegendItem to the legend adornment
                var legendItem = new LegendItem
                {
                    ImageStyle = style,
                    TextStyle = new TextStyle(offenseGroup.ColumnValue, new GeoFont("Verdana", 10), GeoBrushes.Black)
                };
                legend.LegendItems.Add(legendItem);
            }

            // Create the ValueStyle that will use the previously created valueItems to style the data using the OffenseGroup column values
            var valueStyle = new ValueStyle("OffenseGro", valueItems);

            // Add the valueStyle to the friscoCrime layer's CustomStyles and apply the style to all ZoomLevels
            friscoCrime.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(valueStyle);
            friscoCrime.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }
    }
}