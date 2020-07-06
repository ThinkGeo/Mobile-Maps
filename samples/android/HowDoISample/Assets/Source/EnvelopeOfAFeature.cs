using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{ 
    public class EnvelopeOfAFeature : SampleFragment
    {
        private MapView mapView;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer boundingBoxLayer = new InMemoryFeatureLayer();
            boundingBoxLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(100, GeoColors.Green));
            boundingBoxLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Green;
            boundingBoxLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay boundingboxOverlay = new LayerOverlay();
            boundingboxOverlay.Layers.Add("BoundingBoxLayer", boundingBoxLayer);
            boundingboxOverlay.TileType = TileType.SingleTile;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            
            mapView.MapUnit = GeographyUnit.DecimalDegree;
            mapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            mapView.Overlays.Add("WorldOverlay", layerOverlay);
            mapView.Overlays.Add("BoundingBoxOverlay", boundingboxOverlay);

            Button envelopeButton = new Button(this.Context);
            envelopeButton.Text = "Envelope";
            envelopeButton.Click += EnvelopeButtonClick;
            envelopeButton.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo, new Collection<View>() { envelopeButton });
        }

        private void EnvelopeButtonClick(object sender, EventArgs e)
        {
            LayerOverlay worldOverlay = (LayerOverlay)mapView.Overlays["WorldOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];
            worldLayer.Open();
            RectangleShape usBoundingBox = worldLayer.QueryTools.GetFeatureById("137", new string[0]).GetBoundingBox();
            worldLayer.Close();

            InMemoryFeatureLayer boundingBoxLayer = ((LayerOverlay)mapView.Overlays["BoundingBoxOverlay"]).Layers["BoundingBoxLayer"] as InMemoryFeatureLayer;
            if (!boundingBoxLayer.InternalFeatures.Contains("BoundingBox"))
            {
                boundingBoxLayer.InternalFeatures.Add("BoundingBox", new Feature(usBoundingBox.GetWellKnownBinary(), "BoundingBox"));
            }

            mapView.Overlays["BoundingBoxOverlay"].Refresh();
        }
    }
}