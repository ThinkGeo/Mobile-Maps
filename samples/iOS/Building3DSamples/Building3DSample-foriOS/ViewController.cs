using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using UIKit;

namespace Building3DSample_foriOS
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView mapView = new MapView(View.Frame);
            View.AddSubview(mapView);

            mapView.MapUnit = GeographyUnit.Meter;

            OsmBuildingOverlay buildingOverlay = new OsmBuildingOverlay();
            string buildingFilePath = @"osm_buildings_900913_min.shp";
            ShapeFileFeatureSource shapeFileFeatureSource = new ShapeFileFeatureSource(buildingFilePath);
            buildingOverlay.BuildingFeatureSource = shapeFileFeatureSource;
            mapView.Overlays.Add(buildingOverlay);

            shapeFileFeatureSource.Open();
            mapView.CurrentExtent = shapeFileFeatureSource.GetBoundingBoxById("1");
            mapView.Refresh();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}