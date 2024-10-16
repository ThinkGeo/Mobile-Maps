using CoreGraphics;
using System;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class BufferAFeature : BaseViewController
    {
        public BufferAFeature()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.Meter;

            // ThinkGeoCloudRasterMapsOverlay worldMapKitOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);
            // MapView.Overlays.Add("WorldMapKitOverlay", worldMapKitOverlay);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            MapView.Overlays.Add("WorldOverlay", layerOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            worldLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            worldLayer.Open();
            Feature feature = worldLayer.QueryTools.GetFeatureById("137", ReturningColumnsType.NoColumns);
            MapView.CurrentExtent = worldLayer.GetBoundingBox();
            worldLayer.Close();

            InMemoryFeatureLayer inMemoryLayer = new InMemoryFeatureLayer();
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.DeepOcean;
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(100, GeoColors.Green));
            inMemoryLayer.InternalFeatures.Add("POLYGON", feature);

            InMemoryFeatureLayer bufferLayer = new InMemoryFeatureLayer();
            bufferLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            bufferLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(100, GeoColors.Green));

            LayerOverlay inmemoryOverlay = new LayerOverlay();
            inmemoryOverlay.Layers.Add("InMemoryFeatureLayer", inMemoryLayer);
            MapView.Overlays.Add("InmemoryOverlay", inmemoryOverlay);

            LayerOverlay bufferOverlay = new LayerOverlay();
            bufferOverlay.Layers.Add("BufferLayer", bufferLayer);
            MapView.Overlays.Add("BufferOverlay", bufferOverlay);

            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.Refresh();
        }

        private void bufferButton_TouchUpInside(object sender, EventArgs e)
        {
            LayerOverlay inMemoryOverlay = (LayerOverlay)MapView.Overlays["InmemoryOverlay"];
            LayerOverlay bufferOverLay = (LayerOverlay)MapView.Overlays["BufferOverlay"];

            InMemoryFeatureLayer inMemoryLayer = (InMemoryFeatureLayer)inMemoryOverlay.Layers["InMemoryFeatureLayer"];
            InMemoryFeatureLayer bufferLayer = (InMemoryFeatureLayer)bufferOverLay.Layers["BufferLayer"];

            AreaBaseShape baseShape = (AreaBaseShape)inMemoryLayer.InternalFeatures["POLYGON"].GetShape();
            MultipolygonShape bufferedShape = baseShape.Buffer(100, 8, BufferCapType.Butt, GeographyUnit.Meter, DistanceUnit.Kilometer);
            Feature bufferFeature = new Feature(bufferedShape.GetWellKnownBinary(), "Buffer1");

            bufferLayer.InternalFeatures.Clear();
            bufferLayer.InternalFeatures.Add("BufferFeature", bufferFeature);

            MapView.Overlays["BufferOverlay"].Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 130, (contentView) =>
            {
                UIButton bufferButton = UIButton.FromType(UIButtonType.RoundedRect);
                bufferButton.Frame = new CGRect(0, 0, 80, 30);
                bufferButton.BackgroundColor = UIColor.FromRGB(241, 241, 241);
                bufferButton.TouchUpInside += bufferButton_TouchUpInside;
                bufferButton.SetTitle("Buffer", UIControlState.Normal);
                contentView.AddSubview(bufferButton);
            });
        }
    }
}