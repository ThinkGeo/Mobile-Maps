using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class BufferAFeature : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.Open();
            Feature feature = worldLayer.QueryTools.GetFeatureById("137", new string[0]);
            worldLayer.Close();

            InMemoryFeatureLayer inMemoryLayer = new InMemoryFeatureLayer();
            inMemoryLayer.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(100, GeoColors.Green));
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.DeepOcean;
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            inMemoryLayer.InternalFeatures.Add("POLYGON", feature);
            inMemoryLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            inMemoryLayer.Open();
            var currentExtent = inMemoryLayer.GetBoundingBox();
            inMemoryLayer.Close();

            InMemoryFeatureLayer bufferLayer = new InMemoryFeatureLayer();
            bufferLayer.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            bufferLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(100, GeoColors.Green));
            bufferLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            bufferLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            LayerOverlay bufferOverlay = new LayerOverlay();
            bufferOverlay.TileSizeMode = TileSizeMode.DefaultX2;
            bufferOverlay.Layers.Add("BufferLayer", bufferLayer);
            bufferOverlay.Layers.Add("InMemoryFeatureLayer", inMemoryLayer);

            ThinkGeoCloudRasterMapsOverlay ThinkGeoCloudRasterMapsOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);

            androidMap.MapUnit = GeographyUnit.Meter;
            androidMap.CurrentExtent = currentExtent;
            androidMap.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            androidMap.Overlays.Add("ThinkGeoCloudRasterMapsOverlay", ThinkGeoCloudRasterMapsOverlay);
            androidMap.Overlays.Add("BufferOverlay", bufferOverlay);

            Button bufferButton = new Button(this.Context);
            bufferButton.Text = "Buffer";
            bufferButton.Click += BufferButtonClick;
            bufferButton.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo, new Collection<View>() { bufferButton });
        }

        private void BufferButtonClick(object sender, System.EventArgs e)
        {
            LayerOverlay bufferOverLay = (LayerOverlay)androidMap.Overlays["BufferOverlay"];

            InMemoryFeatureLayer inMemoryLayer = (InMemoryFeatureLayer)bufferOverLay.Layers["InMemoryFeatureLayer"];
            InMemoryFeatureLayer bufferLayer = (InMemoryFeatureLayer)bufferOverLay.Layers["BufferLayer"];

            AreaBaseShape baseShape = (AreaBaseShape)inMemoryLayer.InternalFeatures["POLYGON"].GetShape();
            MultipolygonShape bufferedShape = baseShape.Buffer(100, 8, BufferCapType.Butt, GeographyUnit.DecimalDegree, DistanceUnit.Kilometer);
            Feature bufferFeature = new Feature(bufferedShape.GetWellKnownBinary(), "Buffer1");

            bufferLayer.InternalFeatures.Clear();
            bufferLayer.InternalFeatures.Add("BufferFeature", bufferFeature);

            androidMap.Overlays["BufferOverlay"].Refresh();
        }
    }
}