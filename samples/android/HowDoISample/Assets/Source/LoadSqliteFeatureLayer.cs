using Android.App;
using Android.OS;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class LoadSqliteFeatureLayer : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            string connectionString = @"Data Source=\\192.168.0.3\Internal Test Data\Sqlite\Mapping.sqlite";
            SqliteFeatureLayer sqliteFeatureLayer = new SqliteFeatureLayer(connectionString, "Segments", "geomID", "geom");
            sqliteFeatureLayer.Name = sqliteFeatureLayer.TableName;
            sqliteFeatureLayer.WhereClause = "WHERE ReplicationState = 1";
            sqliteFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(LineStyle.CreateSimpleLineStyle(GeoColors.Black, 2, true));
            sqliteFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(sqliteFeatureLayer);

            androidMap.Overlays.Add(layerOverlay);
            androidMap.MapUnit = GeographyUnit.Meter;
            androidMap.CurrentExtent = new RectangleShape(-8336043.617221244, 5212990.090742311, -8328829.872716907, 5207266.868281254);
            androidMap.Refresh();
        }
    }
}