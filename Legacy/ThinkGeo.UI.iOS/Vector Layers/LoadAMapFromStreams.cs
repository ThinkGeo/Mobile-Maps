using System.IO;
using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class LoadAMapFromStreams : BaseViewController
    {
        public LoadAMapFromStreams()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = (new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625));

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);

            ShapeFileFeatureLayer shapeFileLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            shapeFileLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.Transparent, GeoColor.FromArgb(100, GeoColors.Green));
            shapeFileLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            ShapeFileFeatureSource shapeFileSource = (ShapeFileFeatureSource)shapeFileLayer.FeatureSource;
            shapeFileSource.StreamLoading += LoadAMapFromStreams_StreamLoading;

            layerOverlay.Layers.Add(shapeFileLayer);
            MapView.Overlays.Add(layerOverlay);

            MapView.Refresh();
        }

        private static void LoadAMapFromStreams_StreamLoading(object sender, StreamLoadingEventArgs e)
        {
            string fileName = Path.GetFileName(e.AlternateStreamName);
            e.AlternateStream = new FileStream(@"AppData/SampleData/" + fileName, FileMode.Open, FileAccess.Read);
        }
    }
}