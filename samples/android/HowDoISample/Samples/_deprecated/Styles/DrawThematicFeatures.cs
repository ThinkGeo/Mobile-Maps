using Android.App;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class DrawThematicFeatures : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ClassBreakStyle classBreakStyle = new ClassBreakStyle("POP1990");
            classBreakStyle.ClassBreaks.Add(new ClassBreak(453588, AreaStyle.CreateSimpleAreaStyle(GeoColors.Green)));
            classBreakStyle.ClassBreaks.Add(new ClassBreak(6314875, AreaStyle.CreateSimpleAreaStyle(GeoColors.LightYellow)));
            classBreakStyle.ClassBreaks.Add(new ClassBreak(12176161, AreaStyle.CreateSimpleAreaStyle(GeoColors.Yellow)));
            classBreakStyle.ClassBreaks.Add(new ClassBreak(18037448, AreaStyle.CreateSimpleAreaStyle(GeoColors.Crimson)));
            classBreakStyle.ClassBreaks.Add(new ClassBreak(23898734, AreaStyle.CreateSimpleAreaStyle(GeoColors.Red)));

            ShapeFileFeatureLayer statesLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/states.shp"));
            statesLayer.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            statesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69)));
            statesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(classBreakStyle);
            statesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            statesLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            statesLayer.Open();
            var currentExtent = statesLayer.GetBoundingBox();
            statesLayer.Close();

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileSizeMode = TileSizeMode.DefaultX2;
            layerOverlay.Layers.Add(statesLayer);

            ThinkGeoCloudRasterMapsOverlay ThinkGeoCloudRasterMapsOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);

            
            mapView.MapUnit = GeographyUnit.Meter;
            mapView.CurrentExtent = currentExtent;
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            mapView.Overlays.Add(ThinkGeoCloudRasterMapsOverlay);
            mapView.Overlays.Add(layerOverlay);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }
    }
}