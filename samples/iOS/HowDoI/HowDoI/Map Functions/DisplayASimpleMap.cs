using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class DisplayASimpleMap : BaseViewController
    {
        public DisplayASimpleMap()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ThinkGeoCloudRasterMapsOverlay overlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);
            MapView.Overlays.Add(overlay);

            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}