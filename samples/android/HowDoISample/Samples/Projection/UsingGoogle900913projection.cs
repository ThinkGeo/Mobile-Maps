using Android.App;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class UsingGoogle900913projection : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ProjectionConverter projectionConverter = new ProjectionConverter();
            projectionConverter.InternalProjection = new Projection(Projection.GetDecimalDegreesProjString());
            projectionConverter.ExternalProjection = new Projection(Projection.GetGoogleMapProjString());

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            worldLayer.FeatureSource.ProjectionConverter = projectionConverter;

            LayerOverlay staticOverlay = new LayerOverlay();
            staticOverlay.Layers.Add(worldLayer);

            
            androidMap.MapUnit = GeographyUnit.Meter;
            androidMap.CurrentExtent = new RectangleShape(-13939426.6371, 6701997.4056, -7812401.86, 2626987.386962);
            androidMap.Overlays.Add(staticOverlay);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }
    }
}