using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataEditing;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class EditWithSnapping
{
    private bool _initialized;

    public EditWithSnapping()
    {
        InitializeComponent();
    }

    private const float Tolerance = 25;
    private ShapeFileFeatureLayer _parksLayer;

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        MapView.MapUnit = GeographyUnit.Meter;

        // Add Cloud Maps as a background overlay
        var backgroundOverlay = new ThinkGeoVectorOverlay(SampleKeys.ClientId, SampleKeys.ClientSecret, ThinkGeoCloudVectorMapsMapType.Light);
        backgroundOverlay.TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache");
        MapView.Overlays.Add("Background Maps", backgroundOverlay);

        MapView.CenterPoint = new RectangleShape(-10783147, 3917677, -10782596, 3917271).GetCenterPoint();
        MapView.MapScale = 30000;

        // Load the Frisco data to a layer
        var parksFile = Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Schools.shp");
        _parksLayer = new ShapeFileFeatureLayer(parksFile);
        _parksLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new TolerancePointStyle(Tolerance));
        _parksLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Project the layer's data from its native projection (2276) to spherical mercator(3857)
        _parksLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        var inMemoryOverlay = new LayerOverlay();
        inMemoryOverlay.Layers.Add(_parksLayer);
        MapView.Overlays.Add(inMemoryOverlay);

        MapView.EditOverlay.VertexMoving += SnapToLayerEditInteractiveOverlay_VertexMoving;

        var lineShape = new LineShape();
        lineShape.Vertices.Add(new Vertex(-10783003, 3918370));
        lineShape.Vertices.Add(new Vertex(-10783070, 3917335));
        lineShape.Vertices.Add(new Vertex(-10781292, 3916438));
        MapView.EditOverlay.EditShapesLayer.InternalFeatures.Add(new Feature(lineShape));

        MapView.EditOverlay.CalculateAllControlPoints();

        await MapView.RefreshAsync();
    }

    private void SnapToLayerEditInteractiveOverlay_VertexMoving(object sender, VertexMovingEditInteractiveOverlayEventArgs e)
    {
        var toSnapInMemoryFeatures = _parksLayer.QueryTools.GetFeaturesNearestTo(e.TargetVertex, GeographyUnit.Meter, 1, ReturningColumnsType.AllColumns);

        if (toSnapInMemoryFeatures.Count == 0)
            return;

        var toSnapPointShape = toSnapInMemoryFeatures[0].GetShape() as PointShape;
        var screenDistance = MapUtil.GetScreenDistanceBetweenTwoWorldPoints(MapView.CurrentExtent, toSnapPointShape, e.TargetVertex, (float)MapView.Width, (float)MapView.Height);

        if (screenDistance >= Tolerance) return;
        if (toSnapPointShape == null) return;
        e.TargetVertex.X = toSnapPointShape.X;
        e.TargetVertex.Y = toSnapPointShape.Y;
    }
}

internal class TolerancePointStyle(float tolerance) : PointStyle
{
    public float Tolerance { get; set; } = tolerance;

    protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas, Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
    {
        foreach (var feature in features)
        {
            //Draws the vertex.
            var pointShape = (PointShape)feature.GetShape();
            canvas.DrawEllipse(pointShape, 5, 5, new GeoSolidBrush(GeoColors.Black), DrawingLevel.LevelOne);

            //Draws the tolerance circle.
            var screenPointF = MapUtil.ToScreenCoordinate(canvas.CurrentWorldExtent, pointShape, canvas.Width, canvas.Height);
            canvas.DrawEllipse(screenPointF, Tolerance * 2, Tolerance * 2, new GeoPen(GeoColors.Black), new GeoSolidBrush(), DrawingLevel.LevelFour, 0, 0, PenBrushDrawingOrder.PenFirst);
        }
    }
}