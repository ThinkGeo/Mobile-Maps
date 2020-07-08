using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// This sample shows how to display a Postgres layer.
    /// </summary>
    public class PostgresLayerSample : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            SetupSample();

            SetupMap();
        }

        /// <summary>
        /// Sets up the sample's layout and controls
        /// </summary>
        private void SetupSample()
        {
            base.OnStart();

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }

        /// <summary>
        /// Sets up the map layers and styles
        /// </summary>
        private void SetupMap()
        {
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("USlbIyO5uIMja2y0qoM21RRM6NBXUad4hjK3NBD6pD0~", "f6OJsvCDDzmccnevX55nL7nXpPDXXKANe5cN6czVjCH0s8jhpCH-2A~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the zoom levels to match cloud maps
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

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

            // ========================================================
            // Code for creating the sample data in PostgreSql
            // ========================================================

            //Collection<FeatureSourceColumn> columns = new Collection<FeatureSourceColumn>();
            //columns.Add(new FeatureSourceColumn("comment", "varchar", 255));

            //PostgreSqlFeatureSource target = new PostgreSqlFeatureSource("User ID={username};Password={password};Host=10.10.10.179;Port=5432;Database=thinkgeo_samples;Pooling=true;", "frisco_coyote_sightings", "ID", 2276);
            //target.Open();

            //ShapeFileFeatureSource source = new ShapeFileFeatureSource(@"mnt/sdcard/MapSuiteSampleData/HowDoISamples/AppData/SampleData/Frisco_Coyote_Sightings.shp");
            //source.Open();

            //var sourceFeatures = source.GetAllFeatures(ReturningColumnsType.AllColumns);

            //target.BeginTransaction();

            //foreach (var feature in sourceFeatures)
            //{
            //    var dict = new Dictionary<string, string>();
            //    dict.Add("comment", feature.ColumnValues["Comments"].ToString().Replace('"', ' ').Replace("'", ""));
            //    dict.Add("id", feature.ColumnValues["OBJECTID"]);
            //    var newFeature = new Feature(feature.GetWellKnownBinary(), feature.ColumnValues["OBJECTID"], dict);

            //    target.AddFeature(newFeature);
            //}

            //var results = target.CommitTransaction();
            //target.Close();

            //target.Open();
            //var features = target.GetAllFeatures(ReturningColumnsType.AllColumns);
            //target.Close();
        }
    }
}