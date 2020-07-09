using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{ 
    public class ScaleFeatureUpAndDown : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            worldLayer.Open();
            Feature feature = worldLayer.QueryTools.GetFeatureById("135", new string[0]);
            worldLayer.Close();

            InMemoryFeatureLayer inMemoryLayer = new InMemoryFeatureLayer();
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(100, GeoColors.Green));
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Green;
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            inMemoryLayer.InternalFeatures.Add("135", feature);

            LayerOverlay inMemoryOverlay = new LayerOverlay();
            inMemoryOverlay.TileType = TileType.SingleTile;
            inMemoryOverlay.Layers.Add("InMemoryFeatureLayer", inMemoryLayer);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            
            mapView.MapUnit = GeographyUnit.DecimalDegree;
            mapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            mapView.Overlays.Add("WorldOverlay", layerOverlay);
            mapView.Overlays.Add("InMemoryOverlay", inMemoryOverlay);

            Button scaleUpButton = new Button(this.Context);
            scaleUpButton.Text = "ScaleUp";
            scaleUpButton.Click += ScaleUpButtonClick;

            Button scaleDownButton = new Button(this.Context);
            scaleDownButton.Text = "ScaleDown";
            scaleDownButton.Click += ScaleDownButtonClick;

            LinearLayout linearLayout = new LinearLayout(this.Context);
            linearLayout.Orientation = Orientation.Horizontal;

            linearLayout.AddView(scaleUpButton);
            linearLayout.AddView(scaleDownButton);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo, new Collection<View>() { linearLayout });
        }

        private void ScaleDownButtonClick(object sender, System.EventArgs e)
        {
            UpdateFeatureByScale(20, false);
        }

        private void ScaleUpButtonClick(object sender, System.EventArgs e)
        {
            UpdateFeatureByScale(25, true);
        }

        private void UpdateFeatureByScale(double percentage, bool isScaleUp)
        {
            LayerOverlay inMemoryOverlay = (LayerOverlay)mapView.Overlays["InMemoryOverlay"];
            InMemoryFeatureLayer inMemoryLayer = (InMemoryFeatureLayer)inMemoryOverlay.Layers["InMemoryFeatureLayer"];

            inMemoryLayer.Open();
            inMemoryLayer.EditTools.BeginTransaction();
            if (isScaleUp)
            {
                inMemoryLayer.EditTools.ScaleUp("135", percentage);
            }
            else
            {
                inMemoryLayer.EditTools.ScaleDown("135", percentage);
            }
            inMemoryLayer.EditTools.CommitTransaction();
            inMemoryLayer.Close();

            mapView.Overlays["InMemoryOverlay"].Refresh();
        }
    }
}