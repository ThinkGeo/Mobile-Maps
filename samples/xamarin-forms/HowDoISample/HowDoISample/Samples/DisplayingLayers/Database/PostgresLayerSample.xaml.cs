using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ThinkGeo.UI.XamarinForms;
using System.Collections.ObjectModel;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PostgresLayerSample : ContentPage
    {
        public PostgresLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;            

            // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service and add it to the map.
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Create a new overlay that will hold our new layer and add it to the map.
            LayerOverlay coyoteSightingsOverlay = new LayerOverlay();
            mapView.Overlays.Add(coyoteSightingsOverlay);

            // Create the new layer and set the projection as the data is in srid 2276 as our background is srid 3857 (spherical mercator).
            PostgreSqlFeatureLayer coyoteSightingsLayer = new PostgreSqlFeatureLayer("User ID=thinkgeo_user;Password=cs%^%#trsdFG;Host=sampledatabases.thinkgeo.com;Port=5432;Database=thinkgeo_samples;Pooling=true;", "frisco_coyote_sightings", "id", 2276);
            coyoteSightingsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add the layer to the overlay we created earlier.
            coyoteSightingsOverlay.Layers.Add("Coyote Sightings", coyoteSightingsLayer);

            // Set a point style to zoom level 1 and then apply it to all zoom levels up to 20.
            coyoteSightingsLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Red, new GeoPen(GeoColors.White, 2));
            coyoteSightingsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Set the map view current extent to a bounding box that shows just a few sightings.
            mapView.CurrentExtent = new RectangleShape(-10784283.099060204, 3918532.598821122, -10781699.527518518, 3916820.409397046);

            // Refresh the map.
            mapView.Refresh();

            #region Create Sample Data Code

            // ========================================================
            // Code for creating the sample data in PostgreSql
            // ========================================================

            //    Collection<FeatureSourceColumn> columns = new Collection<FeatureSourceColumn>();
            //    columns.Add(new FeatureSourceColumn("comment", "varchar", 255));

            //    PostgreSqlFeatureSource target = new PostgreSqlFeatureSource("User ID={username};Password={password};Host=10.10.10.179;Port=5432;Database=thinkgeo_samples;Pooling=true;", "frisco_coyote_sightings", "ID", 2276);
            //    target.Open();

            //    ShapeFileFeatureSource source = new ShapeFileFeatureSource(@"../../../data/Frisco_Coyote_Sightings.shp");
            //    source.Open();

            //    var sourceFeatures = source.GetAllFeatures(ReturningColumnsType.AllColumns);

            //    target.BeginTransaction();

            //    foreach (var feature in sourceFeatures)
            //    {
            //        var dict = new Dictionary<string, string>();
            //        dict.Add("comment", feature.ColumnValues["Comments"].ToString().Replace('"', ' ').Replace("'", ""));
            //        dict.Add("id", feature.ColumnValues["OBJECTID"]);
            //        var newFeature = new Feature(feature.GetWellKnownBinary(), feature.ColumnValues["OBJECTID"], dict);

            //        target.AddFeature(newFeature);
            //    }

            //    var results = target.CommitTransaction();
            //    target.Close();

            //    target.Open();
            //    var features = target.GetAllFeatures(ReturningColumnsType.AllColumns);
            //    target.Close();
            #endregion
        }

        /// <summary>
        /// ...
        /// </summary>
    }
}
