using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{ 
    public class FindTheDifferenceOfTwoFeatures : SampleFragment
    {
        private MapView androidMap;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer inMemoryLayer = new InMemoryFeatureLayer();
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(50, 100, 100, 200)));
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.RoyalBlue;
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            inMemoryLayer.InternalFeatures.Add("AreaShape1", new Feature(new RectangleShape(10, 50, 50, 10).GetWellKnownBinary(), "AreaShape1"));
            inMemoryLayer.InternalFeatures.Add("AreaShape2", new Feature(new RectangleShape(30, 80, 80, 30).GetWellKnownBinary(), "AreaShape2"));

            LayerOverlay inmemoryOverlay = new LayerOverlay();
            inmemoryOverlay.TileType = TileType.SingleTile;
            inmemoryOverlay.Layers.Add("InMemoryFeatureLayer", inMemoryLayer);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            
            androidMap.MapUnit = GeographyUnit.DecimalDegree;
            androidMap.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            androidMap.Overlays.Add("WorldOverlay", layerOverlay);
            androidMap.Overlays.Add("InMemoryOverlay", inmemoryOverlay);

            Button differenceButton = new Button(this.Context);
            differenceButton.Text = "Difference";
            differenceButton.Click += DifferenceButtonClick;
            differenceButton.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo, new Collection<View>() { differenceButton });
        }

        private void DifferenceButtonClick(object sender, EventArgs e)
        {
            LayerOverlay inMemoryOverlay = (LayerOverlay)androidMap.Overlays["InMemoryOverlay"];
            InMemoryFeatureLayer inMemoryLayer = (InMemoryFeatureLayer)inMemoryOverlay.Layers["InMemoryFeatureLayer"];

            if (inMemoryLayer.InternalFeatures.Count > 1)
            {
                AreaBaseShape sourceShape = (AreaBaseShape)inMemoryLayer.InternalFeatures["AreaShape2"].GetShape();
                AreaBaseShape targetShape = (AreaBaseShape)inMemoryLayer.InternalFeatures["AreaShape1"].GetShape();
                AreaBaseShape resultShape = sourceShape.GetDifference(targetShape);

                inMemoryLayer.InternalFeatures.Clear();
                inMemoryLayer.InternalFeatures.Add("ResultFeature", new Feature(resultShape.GetWellKnownBinary(), "ResultFeature"));
                inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(100, GeoColors.Blue));

                androidMap.Overlays["InMemoryOverlay"].Refresh();
            }
        }
    }
}