using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class AddVariousMapTools : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(worldLayer);

            mapView.MapUnit = GeographyUnit.DecimalDegree;
            mapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            mapView.Overlays.Add(layerOverlay);

            CheckBox zoomToolCheckBox = new CheckBox(this.Context);
            zoomToolCheckBox.Text = "Zoom Tools";
            zoomToolCheckBox.Checked = true;
            zoomToolCheckBox.CheckedChange += ZoomToolCheckBox_CheckedChange;

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), this.SampleInfo, new Collection<View>() { zoomToolCheckBox });
        }

        private void ZoomToolCheckBox_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            mapView.MapTools.ZoomMapTool.IsEnabled = e.IsChecked;
        }
    }
}