using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Drawing and editing by hand: pick a mode and draw points, lines or polygons, or
/// switch to editing and drag, resize, rotate and reshape what is there. Every event
/// the two overlays raise is written to a log, and a measurement follows whatever is
/// being moved.
/// <para>
/// The two switches are the rest of the editing story, which the desktop gallery tells
/// on pages of its own: a rule that refuses an edit, and a rule that corrects one.
/// Both hang off the same overlay events, so they belong with the editing rather than
/// beside it.
/// </para>
/// </summary>
public class EditFeatures : SamplePage
{
    /// <summary>No vertex may be added within this of one the shape already has.</summary>
    private const double MinSpacingMeters = 300;

    /// <summary>How near the finger must come, in pixels, before a vertex snaps.</summary>
    private const float SnapTolerance = 25;

    private readonly ObservableCollection<string> _log = new();

    private LayerOverlay _layerOverlay;
    private LayerOverlay _schoolsOverlay;
    private ShapeFileFeatureLayer _schoolsLayer;
    private MapView _map;
    private Label _measure;
    private Label _logLabel;
    private Switch _constrain;
    private Switch _snap;
    private int _logIndex;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Light,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            { VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder) }));

        map.CenterPoint = new PointShape(-10778000, 3912000);
        map.MapScale = 77000;

        var demoPolygon = new Feature("POLYGON((-10778500 3915600,-10778500 3910000,-10774040 3910000,-10774040 3915600,-10778500 3915600))");
        var demoPoint = new Feature("POINT(-10773220 3913230)");
        var demoLine = new Feature("LINESTRING(-10780700 3916500, -10780700 3910040)");

        // The rule below acts on whatever carries this; the other shapes are free.
        demoPolygon.ColumnValues["constraint"] = "rectangle";

        map.EditOverlay.EditShapesLayer.InternalFeatures.Add(demoPolygon);
        map.EditOverlay.EditShapesLayer.InternalFeatures.Add(demoPoint);
        map.EditOverlay.EditShapesLayer.InternalFeatures.Add(demoLine);
        map.EditOverlay.CalculateAllControlPoints();

        // The layer that keeps the shapes once they are no longer being drawn or edited.
        var featureLayer = new InMemoryFeatureLayer();
        featureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Blue, 8, GeoColors.Black);
        featureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Blue, 4, true);
        featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.Blue, GeoColors.Black);
        featureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        _layerOverlay = new LayerOverlay();
        _layerOverlay.Layers.Add("featureLayer", featureLayer);
        map.Overlays.Add("layerOverlay", _layerOverlay);

        // WPF watches MouseMoved on the track overlay to keep the measurement live;
        // a finger has TouchMoved instead.
        map.TrackOverlay.TouchMoved += (_, e) => ShowMeasure(e.AffectedFeature.GetShape());
        map.EditOverlay.VertexMoved += (_, e) => ShowMeasure(e.AffectedFeature.GetShape());

        // The two rules. Both are always subscribed and both do nothing while their
        // switch is off, which keeps the wiring in one place.
        map.EditOverlay.VertexAdding += EditOverlay_VertexAdding;
        map.EditOverlay.VertexMoving += EditOverlay_VertexMoving;
        map.EditOverlay.VertexMoved += EditOverlay_VertexMoved;

        map.TrackOverlay.TrackStarted += (_, e) => AppendLog($"TrackOverlay.TrackStarted at ({e.StartedVertex.X:N0}, {e.StartedVertex.Y:N0})");
        map.TrackOverlay.VertexAdded += (_, e) => AppendLog($"TrackOverlay.VertexAdded at ({e.AddedVertex.X:N0}, {e.AddedVertex.Y:N0})");
        map.TrackOverlay.TrackEnded += (_, e) => AppendLog($"TrackOverlay.TrackEnded: {e.TrackShape.GetType().Name}");
        map.EditOverlay.FeatureDragged += (_, e) => AppendLog($"EditOverlay.FeatureDragged: {e.DraggedFeature.GetShape().GetType().Name}");
        map.EditOverlay.FeatureResized += (_, e) => AppendLog($"EditOverlay.FeatureResized: {e.ResizedFeature.GetShape().GetType().Name}");
        map.EditOverlay.FeatureRotated += (_, e) => AppendLog($"EditOverlay.FeatureRotated: {e.RotatedFeature.GetShape().GetType().Name}");
        map.EditOverlay.VertexAdded += (_, e) => AppendLog($"EditOverlay.VertexAdded at ({e.AddedVertex.X:N0}, {e.AddedVertex.Y:N0})");
        map.EditOverlay.VertexMoved += (_, e) => AppendLog($"EditOverlay.VertexMoved to ({e.MovedVertex.X:N0}, {e.MovedVertex.Y:N0})");
        map.EditOverlay.VertexRemoved += (_, e) => AppendLog($"EditOverlay.VertexRemoved at ({e.RemovedVertex.X:N0}, {e.RemovedVertex.Y:N0})");

        AddChoicePicker(new[] { "Navigate", "Draw a point", "Draw a line", "Draw a polygon", "Edit what is there" }, 0, ChooseMode);

        AddToPanel(BuildRuleRow());

        var clear = new Button { Text = "Clear the log", FontSize = 12, Padding = new Thickness(10, 4) };
        clear.Clicked += (_, _) =>
        {
            _log.Clear();
            _logLabel.Text = string.Empty;
        };
        AddToPanel(clear);

        _measure = AddStatusLabel();
        _logLabel = AddToPanel(new Label { FontSize = 10, TextColor = Colors.Gray, MaxLines = 5 });

        _ = map.RefreshAsync();
    }

    /// <summary>The two editing rules, as switches rather than as pages of their own.</summary>
    private View BuildRuleRow()
    {
        _constrain = new Switch();
        _snap = new Switch();
        _snap.Toggled += (_, _) => ShowSchools(_snap.IsToggled);

        var row = new HorizontalStackLayout { Spacing = 8 };
        row.Add(new Label { Text = "Constrain", FontSize = 12, VerticalOptions = LayoutOptions.Center });
        row.Add(_constrain);
        row.Add(new Label { Text = "Snap", FontSize = 12, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(8, 0, 0, 0) });
        row.Add(_snap);

        return row;
    }

    /// <summary>
    /// The schools are what there is to snap to, so they are only loaded - and only
    /// drawn - while snapping is on.
    /// </summary>
    private void ShowSchools(bool show)
    {
        if (show && _schoolsOverlay == null)
        {
            _schoolsLayer = new ShapeFileFeatureLayer(SampleShared.Shapefile("Schools.shp"));

            // The shapefile is in the Texas state plane; the map is in web mercator.
            _schoolsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
            _schoolsLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                PointStyle.CreateSimpleCircleStyle(GeoColors.DarkRed, 7, GeoColors.White);
            _schoolsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            _schoolsOverlay = new LayerOverlay();
            _schoolsOverlay.Layers.Add(_schoolsLayer);
            _map.Overlays.Add("schools", _schoolsOverlay);
        }

        if (_schoolsOverlay != null)
        {
            _schoolsOverlay.IsVisible = show;
        }

        _ = _map.RefreshAsync();
    }

    /// <summary>A rule that refuses an edit: no vertex closer than the spacing allows.</summary>
    private void EditOverlay_VertexAdding(object sender, VertexAddingEditInteractiveOverlayEventArgs e)
    {
        if (_constrain?.IsToggled != true)
        {
            return;
        }

        foreach (var vertex in VerticesOf(e.AffectedFeature.GetShape()))
        {
            if (new LineShape(new[] { vertex, e.AddingVertex }).GetLength(3857, DistanceUnit.Meter) < MinSpacingMeters)
            {
                e.Cancel = true;
                _measure.Text = "Too close to a vertex it already has";
                return;
            }
        }
    }

    /// <summary>A rule that moves an edit: a vertex near a school lands on it.</summary>
    private void EditOverlay_VertexMoving(object sender, VertexMovingEditInteractiveOverlayEventArgs e)
    {
        if (_snap?.IsToggled != true || _schoolsLayer == null)
        {
            return;
        }

        var nearest = _schoolsLayer.QueryTools.GetFeaturesNearestTo(
            e.TargetVertex, GeographyUnit.Meter, 1, ReturningColumnsType.NoColumns);
        if (nearest.Count == 0 || nearest[0].GetShape() is not PointShape school)
        {
            return;
        }

        // In pixels, not metres: what counts as "near" is what the finger can see.
        var screenDistance = MapUtil.GetScreenDistanceBetweenTwoWorldPoints(
            _map.CurrentExtent, school, e.TargetVertex,
            (float)_map.CanvasWidth, (float)_map.CanvasHeight);

        if (screenDistance >= SnapTolerance)
        {
            return;
        }

        e.TargetVertex.X = school.X;
        e.TargetVertex.Y = school.Y;
    }

    /// <summary>A rule that corrects an edit: a shape marked rectangular stays one.</summary>
    private void EditOverlay_VertexMoved(object sender, VertexMovedEditInteractiveOverlayEventArgs e)
    {
        if (_constrain?.IsToggled != true)
        {
            return;
        }

        if (e.AffectedFeature.ColumnValues.TryGetValue("constraint", out var rule) && rule == "rectangle")
        {
            RestoreRectangle(e.AffectedFeature, e.MovedVertex);
        }
    }

    /// <summary>
    /// Four corners, one of them dragged: the two beside it follow, so the shape is a
    /// rectangle again rather than the quadrilateral the drag made of it.
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

    /// <summary>The corners of a shape, without a polygon's repeated closing vertex.</summary>
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

    private void ChooseMode(int index)
    {
        switch (index)
        {
            case 0:
                MoveEverythingTo(_layerOverlay);
                _map.TrackOverlay.TrackMode = TrackMode.None;
                break;
            case 1:
                _map.TrackOverlay.TrackMode = TrackMode.Point;
                break;
            case 2:
                _map.TrackOverlay.TrackMode = TrackMode.Line;
                break;
            case 3:
                _map.TrackOverlay.TrackMode = TrackMode.Polygon;
                break;
            default:
                MoveEverythingTo(null);
                _map.TrackOverlay.TrackMode = TrackMode.None;

                // Draws all the handles and manipulation points, putting everything in
                // edit mode.
                _map.EditOverlay.CalculateAllControlPoints();
                break;
        }

        _ = _map.RefreshAsync(new Overlay[] { _map.TrackOverlay, _map.EditOverlay, _layerOverlay });
    }

    /// <summary>
    /// Shapes live in exactly one of the three places. Navigating keeps them in the
    /// layer; editing gathers them into the edit overlay. Either way the track overlay
    /// is emptied, because a tracked shape is one that has not been put anywhere yet.
    /// </summary>
    private void MoveEverythingTo(LayerOverlay destination)
    {
        var featureLayer = (InMemoryFeatureLayer)_layerOverlay.Layers["featureLayer"];

        if (destination == null)
        {
            foreach (var feature in _map.TrackOverlay.TrackShapeLayer.InternalFeatures)
            {
                _map.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature.Id, feature);
            }

            foreach (var feature in featureLayer.InternalFeatures)
            {
                _map.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature.Id, feature);
            }

            featureLayer.InternalFeatures.Clear();
        }
        else
        {
            foreach (var feature in _map.TrackOverlay.TrackShapeLayer.InternalFeatures)
            {
                featureLayer.InternalFeatures.Add(feature.Id, feature);
            }

            foreach (var feature in _map.EditOverlay.EditShapesLayer.InternalFeatures)
            {
                featureLayer.InternalFeatures.Add(feature.Id, feature);
            }

            _map.EditOverlay.EditShapesLayer.InternalFeatures.Clear();
        }

        _map.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
    }

    private void ShowMeasure(BaseShape shape) => _measure.Text = GetMeasureResult(shape);

    private static string GetMeasureResult(BaseShape shape)
    {
        // The background map is in Spherical Mercator; passing the projection in is
        // what makes the number a ground measurement rather than a plane one.
        var currentProjection = new Projection(Projection.GetSphericalMercatorProjString());

        return shape switch
        {
            AreaBaseShape polygon => polygon.GetArea(currentProjection, AreaUnit.SquareMiles).ToString("N2") + " square miles",
            LineBaseShape line => line.GetLength(currentProjection, DistanceUnit.Mile).ToString("N2") + " miles",
            _ => string.Empty,
        };
    }

    private void AppendLog(string message)
    {
        _log.Add($"{_logIndex++}: {message}");

        // The desktop gallery has a list box down the side; the card shows the last
        // few lines, which is what a phone has room for.
        _logLabel.Text = string.Join("\n", _log.Skip(Math.Max(0, _log.Count - 5)));
    }
}
