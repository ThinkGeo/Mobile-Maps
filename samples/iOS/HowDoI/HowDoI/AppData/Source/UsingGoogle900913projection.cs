using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class UsingGoogle900913projection : BaseViewController
    {
        public UsingGoogle900913projection()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = new RectangleShape(-13939426.6371, 6701997.4056, -7812401.86, 2626987.386962);

            ProjectionConverter proj4Projection = new ProjectionConverter();
            proj4Projection.InternalProjection = new Projection(Projection.GetDecimalDegreesProjString());
            proj4Projection.ExternalProjection = new Projection(Projection.GetGoogleMapProjString());

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            worldLayer.FeatureSource.ProjectionConverter = proj4Projection;

            LayerOverlay staticOverlay = new LayerOverlay();
            staticOverlay.Layers.Add(worldLayer);
            MapView.Overlays.Add(staticOverlay);

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 80);
        }
    }
}