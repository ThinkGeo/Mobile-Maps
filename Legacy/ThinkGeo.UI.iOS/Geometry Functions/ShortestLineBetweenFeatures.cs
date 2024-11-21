using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class ShortestLineBetweenFeatures : BaseViewController
    {
        public ShortestLineBetweenFeatures()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(0, 100, 100, 0);

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add("WorldOverlay", layerOverlay);

            InMemoryFeatureLayer inMemoryLayer = new InMemoryFeatureLayer();
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(125, GeoColors.Gray));
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Black;
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            inMemoryLayer.InternalFeatures.Add("AreaShape1", new Feature("POLYGON((10 20,30 60,40 10,10 20))", "AreaShape1"));
            BaseShape shape = new EllipseShape(new PointShape(70, 70), 10, 20);
            shape.Id = "AreaShape2";
            inMemoryLayer.InternalFeatures.Add("AreaShape2", new Feature(shape));

            InMemoryFeatureLayer shortestLineLayer = new InMemoryFeatureLayer();
            shortestLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen.Color = GeoColors.Red;
            shortestLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay inMemoryOverlay = new LayerOverlay();
            inMemoryOverlay.Layers.Add("InMemoryFeatureLayer", inMemoryLayer);
            MapView.Overlays.Add("InMemoryOverlay", inMemoryOverlay);

            LayerOverlay shortestLineOverlay = new LayerOverlay();
            shortestLineOverlay.TileType = TileType.SingleTile;
            shortestLineOverlay.Layers.Add("ShortestLineLayer", shortestLineLayer);
            MapView.Overlays.Add("ShortestLineOverlay", shortestLineOverlay);

            LayerOverlay inmemoryOverlay = new LayerOverlay();
            inmemoryOverlay.TileType = TileType.SingleTile;
            inmemoryOverlay.Layers.Add("InMemoryFeatureLayer", inMemoryLayer);
            MapView.Overlays.Add("InmemoryOverlay", inmemoryOverlay);

            MapView.Refresh();
            GetShortestLine();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => 80);
        }

        private void GetShortestLine()
        {
            LayerOverlay inMemoryOverlay = (LayerOverlay)MapView.Overlays["InMemoryOverlay"];
            LayerOverlay shortestLineOverlay = (LayerOverlay)MapView.Overlays["ShortestLineOverlay"];

            InMemoryFeatureLayer inMemoryLayer = (InMemoryFeatureLayer)inMemoryOverlay.Layers["InMemoryFeatureLayer"];
            InMemoryFeatureLayer shortestLineLayer = (InMemoryFeatureLayer)shortestLineOverlay.Layers["ShortestLineLayer"];

            BaseShape areaShape1 = inMemoryLayer.InternalFeatures["AreaShape1"].GetShape();
            BaseShape areaShape2 = inMemoryLayer.InternalFeatures["AreaShape2"].GetShape();
            MultilineShape multiLineShape = areaShape1.GetShortestLineTo(areaShape2, GeographyUnit.Meter);

            shortestLineLayer.InternalFeatures.Clear();
            shortestLineLayer.InternalFeatures.Add("ShortestLine", new Feature(multiLineShape.GetWellKnownBinary(), "ShortestLine"));
            MapView.Overlays["ShortestLineOverlay"].Refresh();
        }
    }
}