using Android.App;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class DrawFeaturesBasedOnRegularExpression : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            RegexStyle regexStyle = new RegexStyle();
            regexStyle.ColumnName = "CNTRY_NAME";
            regexStyle.RegexItems.Add(new RegexItem(".*land", new AreaStyle(new GeoSolidBrush(GeoColors.LightGreen))));

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69)));
            worldLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(regexStyle);
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(worldLayer);

            
            mapView.MapUnit = GeographyUnit.DecimalDegree;
            mapView.CurrentExtent = new RectangleShape(-135.7, 83.6, 113.5, -53);
            mapView.Overlays.Add(layerOverlay);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }
    }
}