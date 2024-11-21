using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class LoadSqliteFeatureLayer : BaseViewController
    {
        public LoadSqliteFeatureLayer()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            string connectionString = @"Data Source=\\192.168.0.3\Internal Test Data\Sqlite\Mapping.sqlite";
            SqliteFeatureLayer sqliteFeatureLayer = new SqliteFeatureLayer(connectionString, "Segments", "geomID", "geom");
            sqliteFeatureLayer.Name = sqliteFeatureLayer.TableName;
            sqliteFeatureLayer.WhereClause = "WHERE ReplicationState = 1";
            sqliteFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(LineStyle.CreateSimpleLineStyle(GeoColors.Black, 2, true));
            sqliteFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(sqliteFeatureLayer);

            MapView.Overlays.Add(layerOverlay);
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = new RectangleShape(-8336043.617221244, 5212990.090742311, -8328829.872716907, 5207266.868281254);
            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}