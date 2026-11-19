using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Dragging a vertex that snaps: drag one of the line's control points near a school
/// and it jumps onto it, as long as the school is within the tolerance circle drawn
/// around it.
/// </summary>
public class EditWithSnapping : SamplePage
{
    private const float Tolerance = 25;

    private ShapeFileFeatureLayer _schoolsLayer;
    private MapView _map;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Light,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            { VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder) }));

        _schoolsLayer = new ShapeFileFeatureLayer(SampleShared.Shapefile("Schools.shp"));
        _schoolsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new TolerancePointStyle(Tolerance));
        _schoolsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        _schoolsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        var schoolsOverlay = new LayerOverlay();
        schoolsOverlay.Layers.Add(_schoolsLayer);
        map.Overlays.Add(schoolsOverlay);

        map.EditOverlay.VertexMoving += EditOverlay_VertexMoving;

        var lineShape = new LineShape();
        lineShape.Vertices.Add(new Vertex(-10783003, 3918370));
        lineShape.Vertices.Add(new Vertex(-10783070, 3917335));
        lineShape.Vertices.Add(new Vertex(-10781292, 3916438));
        map.EditOverlay.EditShapesLayer.InternalFeatures.Add(new Feature(lineShape));
        map.EditOverlay.CalculateAllControlPoints();

        map.CenterPoint = new PointShape(-10782870, 3917470);
        map.MapScale = 30000;

        AddStatusLabel().Text = "Drag a control point near a school to snap it";
        PutPanelAtTop();
        _ = map.RefreshAsync();
    }

    private void EditOverlay_VertexMoving(object sender, VertexMovingEditInteractiveOverlayEventArgs e)
    {
        var nearest = _schoolsLayer.QueryTools.GetFeaturesNearestTo(
            e.TargetVertex, GeographyUnit.Meter, 1, ReturningColumnsType.NoColumns);
        if (nearest.Count == 0 || nearest[0].GetShape() is not PointShape school) return;

        var screenDistance = MapUtil.GetScreenDistanceBetweenTwoWorldPoints(
            _map.CurrentExtent, school, e.TargetVertex,
            (float)_map.CanvasWidth, (float)_map.CanvasHeight);

        if (screenDistance >= Tolerance) return;

        e.TargetVertex.X = school.X;
        e.TargetVertex.Y = school.Y;
    }
}

internal class TolerancePointStyle : PointStyle
{
    public TolerancePointStyle(float tolerance)
    {
        Tolerance = tolerance;
    }

    public float Tolerance { get; set; }

    protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas,
        Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
    {
        foreach (var feature in features)
        {
            // Draws the vertex.
            var pointShape = (PointShape)feature.GetShape();
            canvas.DrawEllipse(pointShape, 5, 5, new GeoSolidBrush(GeoColors.Black), DrawingLevel.LevelOne);

            // Draws the tolerance circle.
            var screenPointF = MapUtil.ToScreenCoordinate(canvas.CurrentWorldExtent, pointShape, canvas.Width, canvas.Height);
            canvas.DrawEllipse(screenPointF, Tolerance * 2, Tolerance * 2, new GeoPen(GeoColors.Black),
                new GeoSolidBrush(), DrawingLevel.LevelFour, 0, 0, PenBrushDrawingOrder.PenFirst);
        }
    }
}
