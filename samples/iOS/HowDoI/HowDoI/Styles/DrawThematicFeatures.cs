using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class DrawThematicFeatures : BaseViewController
    {
        public DrawThematicFeatures()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.Meter;

            ThinkGeoCloudRasterMapsOverlay ThinkGeoCloudRasterMapsOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);
            MapView.Overlays.Add(ThinkGeoCloudRasterMapsOverlay);

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);

            ClassBreakStyle classBreakStyle = new ClassBreakStyle("POP1990");
            classBreakStyle.ClassBreaks.Add(new ClassBreak(453588, AreaStyle.CreateSimpleAreaStyle(GeoColors.Green)));
            classBreakStyle.ClassBreaks.Add(new ClassBreak(6314875, AreaStyle.CreateSimpleAreaStyle(GeoColors.LightYellow)));
            classBreakStyle.ClassBreaks.Add(new ClassBreak(12176161, AreaStyle.CreateSimpleAreaStyle(GeoColors.Yellow)));
            classBreakStyle.ClassBreaks.Add(new ClassBreak(18037448, AreaStyle.CreateSimpleAreaStyle(GeoColors.Crimson)));
            classBreakStyle.ClassBreaks.Add(new ClassBreak(23898734, AreaStyle.CreateSimpleAreaStyle(GeoColors.Red)));

            ShapeFileFeatureLayer statesLayer = new ShapeFileFeatureLayer("AppData/SampleData/states.shp");
            statesLayer.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            statesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69)));
            statesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(classBreakStyle);
            statesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            statesLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
            layerOverlay.Layers.Add(statesLayer);

            statesLayer.Open();
            MapView.CurrentExtent = statesLayer.GetBoundingBox();
            statesLayer.Close();

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 80);
        }
    }
}