using Android.App;
using Android.OS;
using Android.Widget;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{ 
    public class FindFeatureTouched : SampleFragment
    {
        private MapView mapView;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(100, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            worldLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 154, 205, 50));
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileSnappingMode = TileSnappingMode.Snapping;
            layerOverlay.TileSizeMode = TileSizeMode.DefaultX2;
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            LayerOverlay highlightOverlay = new LayerOverlay();
            highlightOverlay.TileSnappingMode = TileSnappingMode.Snapping;
            highlightOverlay.TileSizeMode = TileSizeMode.DefaultX2;
            highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);

            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudRasterMapsOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);
            thinkGeoCloudRasterMapsOverlay.TileResolution = TileResolution.Standard;
            thinkGeoCloudRasterMapsOverlay.TileCache = null;

            
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            mapView.MapUnit = GeographyUnit.Meter;
            mapView.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            mapView.SingleTap += mapView_SingleTap;
            mapView.Overlays.Add("ThinkGeoCloudRasterMapsOverlay", thinkGeoCloudRasterMapsOverlay);
            mapView.Overlays.Add("WorldOverlay", layerOverlay);
            mapView.Overlays.Add("HighlightOverlay", highlightOverlay);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }

        private void mapView_SingleTap(object sender, SingleTapMapViewEventArgs e)
        {
            LayerOverlay worldOverlay = (LayerOverlay)mapView.Overlays["WorldOverlay"];
            LayerOverlay highlightOverlay = (LayerOverlay)mapView.Overlays["HighlightOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];
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