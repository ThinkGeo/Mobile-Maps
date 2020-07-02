using Android.App;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{ 
    public class ShortestLineBetweenFeatures : SampleFragment
    {
        private MapView androidMap;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            
            androidMap.MapUnit = GeographyUnit.DecimalDegree;
            androidMap.CurrentExtent = new RectangleShape(0, 100, 100, 0);

            LayerOverlay inMemoryOverlay = new LayerOverlay();
            androidMap.Overlays.Add(inMemoryOverlay);

            LayerOverlay shortestLineOverlay = new LayerOverlay();
            shortestLineOverlay.TileType = TileType.SingleTile;
            androidMap.Overlays.Add(shortestLineOverlay);

            BaseShape areaShape1 = BaseShape.CreateShapeFromWellKnownData("POLYGON((10 20,30 60,40 10,10 20))");
            BaseShape areaShape2 = new EllipseShape(new PointShape(70, 70), 10, 20);

            InMemoryFeatureLayer inMemoryLayer = new InMemoryFeatureLayer();
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(125, GeoColors.Gray));
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Black;
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            inMemoryLayer.InternalFeatures.Add(new Feature(areaShape1));
            inMemoryLayer.InternalFeatures.Add(new Feature(areaShape2));
            inMemoryOverlay.Layers.Add(inMemoryLayer);

            InMemoryFeatureLayer shortestLineLayer = new InMemoryFeatureLayer();
            shortestLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen.Color = GeoColors.Red;
            shortestLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            shortestLineOverlay.Layers.Add(shortestLineLayer);

            MultilineShape shortestLine = areaShape1.GetShortestLineTo(areaShape2, GeographyUnit.Meter);
            shortestLineLayer.InternalFeatures.Add(new Feature(shortestLine));

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }
    }
}