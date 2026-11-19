using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Editing under rules: no vertex may land closer than 300 m to its neighbours, the
/// rectangle stays a rectangle however its corners are dragged, and a corner can be
/// put exactly where the survey says by typing its coordinates.
/// </summary>
public class EditWithConstraints : SamplePage
{
    // A vertex may not land closer than this to its neighbours, drawn or edited,
    // measured on the ground: in Frisco one Web Mercator unit is only about 0.84 m.
    private const double MinSpacingMeters = 300;

    private MapView _map;
    private Entry _vertexX;
    private Entry _vertexY;
    private Label _status;
    private Vertex _lastMovedVertex;
    private string _lastMovedFeatureId;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Light,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            { VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder) }));

        map.CenterPoint = new PointShape(-10776500, 3912800);
        map.MapScale = 77000;

        // The rectangle carries its rule as data, so the VertexMoved handler can tell
        // it apart from the free-form polygon next to it.
        var rectangle = new Feature("POLYGON((-10778500 3915600,-10774040 3915600,-10774040 3912400,-10778500 3912400,-10778500 3915600))");
        rectangle.ColumnValues["constraint"] = "rectangle";
        var polygon = new Feature("POLYGON((-10778300 3911600,-10775300 3911800,-10774100 3909900,-10776900 3909200,-10778300 3911600))");

        WhenFirstDrawn(map, () =>
        {
            map.EditOverlay.EditShapesLayer.InternalFeatures.Add(rectangle.Id, rectangle);
            map.EditOverlay.EditShapesLayer.InternalFeatures.Add(polygon.Id, polygon);
            map.EditOverlay.CalculateAllControlPoints();
            _ = map.RefreshAsync(new Overlay[] { map.EditOverlay });
        });

        map.EditOverlay.VertexAdding += EditOverlay_VertexAdding;
        map.EditOverlay.VertexMoved += EditOverlay_VertexMoved;
        map.TrackOverlay.VertexAdding += TrackOverlay_VertexAdding;

        AddChoicePicker(new[] { "Edit what is there", "Draw a polygon" }, 0, index =>
        {
            if (index == 1)
            {
                _map.TrackOverlay.TrackMode = TrackMode.Polygon;
                return;
            }

            // Whatever was drawn joins the editable set.
            foreach (var feature in _map.TrackOverlay.TrackShapeLayer.InternalFeatures)
            {
                _map.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature.Id, feature);
            }

            _map.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
            _map.TrackOverlay.TrackMode = TrackMode.None;
            _map.EditOverlay.CalculateAllControlPoints();
            _ = _map.RefreshAsync(new Overlay[] { _map.TrackOverlay, _map.EditOverlay });
        });

        var coordinates = new HorizontalStackLayout { Spacing = 6 };
        _vertexX = new Entry { Placeholder = "x", FontSize = 12, WidthRequest = 110, Keyboard = Keyboard.Numeric };
        _vertexY = new Entry { Placeholder = "y", FontSize = 12, WidthRequest = 110, Keyboard = Keyboard.Numeric };
        var apply = new Button { Text = "Place it", FontSize = 12, Padding = new Thickness(10, 4) };
        apply.Clicked += (_, _) => ApplyCoordinates();
        coordinates.Add(_vertexX);
        coordinates.Add(_vertexY);
        coordinates.Add(apply);
        AddToPanel(coordinates);

        _status = AddStatusLabel();
        _status.Text = FormattableString.Invariant($"Vertices must stay {MinSpacingMeters:0} m apart");

        _ = map.RefreshAsync();
    }

    /// <summary>
    /// Minimum spacing while EDITING: a vertex added on a segment is rejected when it
    /// lands too close to any vertex the shape already has.
    /// </summary>
    private void EditOverlay_VertexAdding(object sender, VertexAddingEditInteractiveOverlayEventArgs e)
    {
        foreach (var vertex in VerticesOf(e.AffectedFeature.GetShape()))
        {
            if (new LineShape(new[] { vertex, e.AddingVertex }).GetLength(3857, DistanceUnit.Meter) < MinSpacingMeters)
            {
                e.Cancel = true;
                _status.Text = "Too close to a vertex it already has";
                return;
            }
        }
    }

    /// <summary>
    /// Minimum spacing while DRAWING: the shape being tracked already contains a
    /// rubber-band vertex at the tap position, so the check walks back to the last
    /// vertex that is NOT the tap and measures against that one.
    /// </summary>
    private void TrackOverlay_VertexAdding(object sender, VertexAddingTrackInteractiveOverlayEventArgs e)
    {
        Vertex? previous = null;
        foreach (var vertex in VerticesOf(e.AffectedFeature.GetShape()))
        {
            if (Distance(vertex, e.AddingVertex) > 1e-9) previous = vertex;
        }

        if (previous.HasValue
            && new LineShape(new[] { previous.Value, e.AddingVertex }).GetLength(3857, DistanceUnit.Meter) < MinSpacingMeters)
        {
            e.Cancel = true;
            _status.Text = "Too close to the last corner";
        }
    }

    private void EditOverlay_VertexMoved(object sender, VertexMovedEditInteractiveOverlayEventArgs e)
    {
        _lastMovedVertex = e.MovedVertex;
        _lastMovedFeatureId = e.AffectedFeature.Id;
        _vertexX.Text = e.MovedVertex.X.ToString("0.##", CultureInfo.InvariantCulture);
        _vertexY.Text = e.MovedVertex.Y.ToString("0.##", CultureInfo.InvariantCulture);

        if (e.AffectedFeature.ColumnValues.TryGetValue("constraint", out var rule) && rule == "rectangle")
        {
            RestoreRectangle(e.AffectedFeature, e.MovedVertex);
        }
    }

    /// <summary>
    /// Re-places the last dragged vertex at the typed coordinates - dragging gets the
    /// vertex near, the numbers put it exactly where the survey says.
    /// </summary>
    private void ApplyCoordinates()
    {
        if (_lastMovedFeatureId == null
            || !double.TryParse(_vertexX.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
            || !double.TryParse(_vertexY.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var y)
            || !_map.EditOverlay.EditShapesLayer.InternalFeatures.Contains(_lastMovedFeatureId))
        {
            return;
        }

        var feature = _map.EditOverlay.EditShapesLayer.InternalFeatures[_lastMovedFeatureId];
        var shape = feature.GetShape();
        MoveNearestVertex(shape, _lastMovedVertex, new Vertex(x, y));
        _lastMovedVertex = new Vertex(x, y);
        ReplaceEditShape(feature, shape);

        if (feature.ColumnValues.TryGetValue("constraint", out var rule) && rule == "rectangle")
        {
            RestoreRectangle(_map.EditOverlay.EditShapesLayer.InternalFeatures[_lastMovedFeatureId], _lastMovedVertex);
        }
    }

    /// <summary>
    /// The rectangle rule: the dragged corner and the corner opposite it define the
    /// shape; the two corners between them are recomputed. The opposite corner is
    /// found by elimination - of the three corners that did not move, the two ADJACENT
    /// ones share no coordinate with each other.
    /// </summary>
    private void RestoreRectangle(Feature feature, Vertex movedTo)
    {
        if (feature.GetShape() is not PolygonShape polygon) return;

        var corners = new List<Vertex>(VerticesOf(polygon));
        if (corners.Count != 4) return;

        var movedIndex = 0;
        for (var i = 1; i < 4; i++)
        {
            if (Distance(corners[i], movedTo) < Distance(corners[movedIndex], movedTo)) movedIndex = i;
        }

        var others = new List<Vertex>();
        for (var i = 0; i < 4; i++)
        {
            if (i != movedIndex) others.Add(corners[i]);
        }

        var opposite = others[0];
        if (!SharesAxis(others[0], others[1])) opposite = others[2];
        else if (!SharesAxis(others[0], others[2])) opposite = others[1];

        var moved = corners[movedIndex];
        if (Math.Abs(moved.X - opposite.X) < 1e-6 || Math.Abs(moved.Y - opposite.Y) < 1e-6) return;

        var ring = new RingShape();
        ring.Vertices.Add(moved);
        ring.Vertices.Add(new Vertex(moved.X, opposite.Y));
        ring.Vertices.Add(opposite);
        ring.Vertices.Add(new Vertex(opposite.X, moved.Y));
        ring.Vertices.Add(moved);

        ReplaceEditShape(feature, new PolygonShape(ring));
    }

    private void ReplaceEditShape(Feature original, BaseShape shape)
    {
        shape.Id = original.Id;
        var replacement = new Feature(shape);
        foreach (var column in original.ColumnValues)
        {
            replacement.ColumnValues[column.Key] = column.Value;
        }

        _map.EditOverlay.EditShapesLayer.InternalFeatures.Remove(original.Id);
        _map.EditOverlay.EditShapesLayer.InternalFeatures.Add(replacement.Id, replacement);
        _map.EditOverlay.CalculateAllControlPoints();
        // MAUI's single-overlay RefreshAsync takes a CancellationToken, not an
        // overlay; the collection overload is the one that redraws just this overlay.
        _ = _map.RefreshAsync(new Overlay[] { _map.EditOverlay });
    }

    private static void MoveNearestVertex(BaseShape shape, Vertex from, Vertex to)
    {
        switch (shape)
        {
            case PolygonShape polygon:
                MoveNearest(polygon.OuterRing.Vertices, from, to, closedRing: true);
                break;
            case LineShape line:
                MoveNearest(line.Vertices, from, to, closedRing: false);
                break;
        }
    }

    private static void MoveNearest(IList<Vertex> vertices, Vertex from, Vertex to, bool closedRing)
    {
        var nearest = 0;
        for (var i = 1; i < vertices.Count; i++)
        {
            if (Distance(vertices[i], from) < Distance(vertices[nearest], from)) nearest = i;
        }

        vertices[nearest] = to;

        // A ring's first and last vertex are the same point; moving one end moves both.
        if (closedRing && nearest == 0) vertices[^1] = to;
        else if (closedRing && nearest == vertices.Count - 1) vertices[0] = to;
    }

    /// <summary>The shape's vertices, without the ring-closing duplicate.</summary>
    private static IEnumerable<Vertex> VerticesOf(BaseShape shape)
    {
        switch (shape)
        {
            case PolygonShape polygon:
                var ring = polygon.OuterRing.Vertices;
                var count = ring.Count > 1 && Distance(ring[0], ring[^1]) < 1e-9 ? ring.Count - 1 : ring.Count;
                for (var i = 0; i < count; i++) yield return ring[i];
                break;

            case LineShape line:
                foreach (var vertex in line.Vertices) yield return vertex;
                break;
        }
    }

    private static bool SharesAxis(Vertex a, Vertex b)
        => Math.Abs(a.X - b.X) < 1e-6 || Math.Abs(a.Y - b.Y) < 1e-6;

    private static double Distance(Vertex a, Vertex b)
        => Math.Sqrt(((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y)));
}
