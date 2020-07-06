using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class ExtentAndScaleChangeEvents : SampleFragment
    { 
        private TextView labelExtent;
        private TextView labelScale;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileType = TileType.SingleTile;
            layerOverlay.Layers.Add(worldLayer);

            mapView.MapUnit = GeographyUnit.DecimalDegree;
            mapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            mapView.CurrentExtentChanged += mapView_CurrentExtentChanged;
            mapView.Overlays.Add(layerOverlay);

            labelExtent = new TextView(this.Context);
            labelScale = new TextView(this.Context );

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), this.SampleInfo, new Collection<View>() { labelExtent, labelScale });
        }

        private void mapView_CurrentExtentChanged(object sender, CurrentExtentChangedMapViewEventArgs e)
        {
            PointShape upperLeftPoint = e.NewExtent.UpperLeftPoint;
            PointShape lowerRightPoint = e.NewExtent.LowerRightPoint;

            labelExtent.Text = string.Format("Map cureent extent: {0}, {1}, {2}, {3}", upperLeftPoint.X.ToString("n2"), upperLeftPoint.Y.ToString("n2"), lowerRightPoint.X.ToString("n2"), lowerRightPoint.Y.ToString("n2"));
            labelScale.Text = string.Format("Map cureent scale: {0}", MapUtil.GetScale(e.NewExtent, (float)mapView.Width, mapView.MapUnit).ToString("n4"));
        }
    }
}