using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using System.Globalization;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class AreaOfAFeature : SampleFragment
    {
        private TextView messageTextView;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            LayerOverlay highlightOverlay = new LayerOverlay();
            highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 154, 205, 50));
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            androidMap.MapUnit = GeographyUnit.DecimalDegree;
            androidMap.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            androidMap.Overlays.Add("WorldOverlay", layerOverlay);
            androidMap.Overlays.Add("HighlightOverlay", highlightOverlay);
            androidMap.SingleTap += AndroidMap_SingleTap;

            messageTextView = new TextView(this.Context);
            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout) ,base.SampleInfo, new Collection<View>() { messageTextView });
        }

        private void AndroidMap_SingleTap(object sender, SingleTapMapViewEventArgs e)
        {
            LayerOverlay worldOverlay = (LayerOverlay)androidMap.Overlays["WorldOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];

            LayerOverlay highlightOverlay = (LayerOverlay)androidMap.Overlays["HighlightOverlay"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesContaining(e.WorldPoint, new string[1] { "CNTRY_NAME" });
            worldLayer.Close();

            highlightLayer.Open();
            highlightLayer.InternalFeatures.Clear();
            if (selectedFeatures.Count > 0)
            {
                AreaBaseShape areaShape = (AreaBaseShape)selectedFeatures[0].GetShape();
                double area = areaShape.GetArea(GeographyUnit.DecimalDegree, AreaUnit.SquareKilometers);
                messageTextView.Text = string.Format(CultureInfo.InvariantCulture, "{0} has an area of {1:N0} square kilometers.", selectedFeatures[0].ColumnValues["CNTRY_NAME"].Trim(), area);

                highlightLayer.InternalFeatures.Add(selectedFeatures[0]);
            }
            highlightLayer.Close();
            highlightOverlay.Refresh();

        }
    }
}