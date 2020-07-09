using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{ 
    public class ChangeEditSettings : SampleFragment
    {
        private CheckBox canReShape;
        private CheckBox canResize;
        private CheckBox canRotate;
        private CheckBox canDrag;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(worldLayer);

            Feature feature = new Feature(new RectangleShape(-55.5723249724898, 15.7443857300058, -10.5026750275102, -7.6443857300058));

            mapView.MapUnit = GeographyUnit.DecimalDegree;
            mapView.CurrentExtent = new RectangleShape(-79.303125, 76.471875, 0.853125000000006, -38.840625);
            mapView.Overlays.Add(layerOverlay);
            mapView.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature.Id, feature);

            canReShape = new CheckBox(this.Context);
            canReShape.Text = "ReShape";
            canReShape.CheckedChange += Setting_CheckedChange;

            canResize = new CheckBox(this.Context);
            canResize.Text = "Resize";
            canResize.CheckedChange += Setting_CheckedChange;

            canRotate = new CheckBox(this.Context);
            canRotate.Text = "Rotate";
            canRotate.CheckedChange += Setting_CheckedChange;

            canDrag = new CheckBox(this.Context);
            canDrag.Text = "Drag";
            canDrag.CheckedChange += Setting_CheckedChange;

            LinearLayout linearLayout = new LinearLayout(this.Context);
            linearLayout.Orientation = Orientation.Horizontal;
            linearLayout.AddView(canReShape);
            linearLayout.AddView(canResize);
            linearLayout.AddView(canRotate);
            linearLayout.AddView(canDrag);

            SampleViewHelper.InitializeInstruction(this.Context, currentView. FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo, new Collection<View>() { linearLayout });
        }

        private void Setting_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            mapView.EditOverlay.CanReshape = canReShape.Checked;
            mapView.EditOverlay.CanResize = canResize.Checked;
            mapView.EditOverlay.CanRotate = canRotate.Checked;
            mapView.EditOverlay.CanDrag = canDrag.Checked;

            mapView.EditOverlay.CalculateAllControlPoints();
            mapView.Refresh();
        }
    }
}