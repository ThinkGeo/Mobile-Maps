using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{ 
    public class FindWorldCoordinatesWhereTouched : SampleFragment
    {
        private TextView longitudeLabelView;
        private TextView latitudeLabelView;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 154, 205, 50));
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            LayerOverlay highlightOverlay = new LayerOverlay();
            highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);

            
            mapView.MapUnit = GeographyUnit.DecimalDegree;
            mapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            mapView.Overlays.Add("WorldOverlay", layerOverlay);
            mapView.Overlays.Add("HighlightOverlay", highlightOverlay);
            mapView.SingleTap += mapView_SingleTap;

            longitudeLabelView = new TextView(this.Context);
            latitudeLabelView = new TextView(this.Context);
            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo, new Collection<View>() { longitudeLabelView, latitudeLabelView });
        }

        private void mapView_SingleTap(object sender, SingleTapMapViewEventArgs e)
        {
            longitudeLabelView.Text = string.Format("Longitude : {0:N4}", e.WorldX);
            latitudeLabelView.Text = string.Format("Latitude : {0:N4}", e.WorldY);

            LayerOverlay worldOverlay = (LayerOverlay)mapView.Overlays["WorldOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];
            LayerOverlay highlightOverlay = (LayerOverlay)mapView.Overlays["HighlightOverlay"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesContaining(e.WorldPoint, new string[1] { "CNTRY_NAME" });
            worldLayer.Close();

            highlightLayer.Open();
            highlightLayer.InternalFeatures.Clear();
            if (selectedFeatures.Count > 0)
            {
                highlightLayer.InternalFeatures.Add(selectedFeatures[0]);
            }

            highlightLayer.Close();
            highlightOverlay.Refresh();
        }
    }
}