using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Drawing and editing by hand: pick a mode and draw points, lines or polygons, or
/// switch to editing and drag, resize, rotate and reshape what is there, with a
/// measurement following whatever is being moved.
/// <para>
/// The rest of the editing story, which the desktop gallery tells on pages of its own,
/// is here as the two things a rule can do to an edit: move it somewhere else, which
/// is what the snapping switch does, and correct it afterwards, which is what the blue
/// rectangle does to itself. Both hang off the edit overlay's own events, so they
/// belong with the editing rather than beside it.
/// </para>
/// </summary>
public class EditFeatures : SamplePage
{
    /// <summary>How near the finger must come, in pixels, before a vertex snaps.</summary>
    private const float SnapTolerance = 25;

    private LayerOverlay _layerOverlay;
    private LayerOverlay _schoolsOverlay;
    private ShapeFileFeatureLayer _schoolsLayer;
    private MapView _map;
    private Label _measure;
    private Switch _snap;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Light,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            { VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder) }));

        map.CenterPoint = new PointShape(-10778000, 3912000);
        map.MapScale = 77000;

        var demoPolygon = new Feature("POLYGON((-10778500 3915600,-10778500 3910000,-10774040 3910000,-10774040 3915600,-10778500 3915600))");
        var demoPoint = new Feature("POINT(-10773220 3913230)");
        var demoLine = new Feature("LINESTRING(-10780700 3916500, -10780700 3910040)");

        // A shape can carry what it is, and a rule can read it back: this one is a
        // rectangle and is corrected into one after every drag. The other two shapes
        // say nothing about themselves and are free.
        demoPolygon.ColumnValues["constraint"] = "rectangle";

        map.EditOverlay.EditShapesLayer.InternalFeatures.Add(demoPolygon);
        map.EditOverlay.EditShapesLayer.InternalFeatures.Add(demoPoint);
        map.EditOverlay.EditShapesLayer.InternalFeatures.Add(demoLine);
        map.EditOverlay.CalculateAllControlPoints();

        // The layer that keeps the shapes once they are no longer being drawn or edited.
        var featureLayer = new InMemoryFeatureLayer();
        featureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Blue, 8, GeoColors.Black);
        featureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Blue, 4, true);
        // Translucent, because the shapes sit here whenever they are not being edited
        // and a solid fill covers the map it was drawn over.
        featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(
            GeoColor.FromArgb(70, GeoColors.Blue), GeoColors.Blue, 2);
        featureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        _layerOverlay = new LayerOverlay();
        _layerOverlay.Layers.Add("featureLayer", featureLayer);
        map.Overlays.Add("layerOverlay", _layerOverlay);

        // WPF watches MouseMoved on the track overlay to keep the measurement live;
        // a finger has TouchMoved instead.
        map.TrackOverlay.TouchMoved += (_, e) => ShowMeasure(e.AffectedFeature.GetShape());
        map.EditOverlay.VertexMoved += (_, e) => ShowMeasure(e.AffectedFeature.GetShape());

        // The two rules. Snapping does nothing while its switch is off; the rectangle
        // reads the shape rather than a switch, so it needs none.
        map.EditOverlay.VertexMoving += EditOverlay_VertexMoving;
        map.EditOverlay.VertexMoved += EditOverlay_VertexMoved;

        AddChoiceStrip(new[] { "Navigate", "Point", "Line", "Polygon", "Edit" }, 0, ChooseMode);

        AddToPanel(BuildSnapRow());
        _measure = AddStatusLabel();

        // The mode the strip opens on has to be the mode the map is in. The three
        // shapes above are put into the edit overlay to exist at all, which is where
        // the editing mode wants them and the only place they can be handed to it
        // from - and left there, the sample opened under a full set of drag and vertex
        // handles while the strip said Navigate, and a finger meaning to pan the map
        // grabbed a control point instead.
        ChooseMode(0);

        _ = map.RefreshAsync();
    }

    /// <summary>
    /// The one switch, named for what it does to an edit rather than for the kind of
    /// rule it is - "Snap" is what the API calls it, and left at that nobody can tell
    /// what the switch is about to do to their shape.
    /// </summary>
    private View BuildSnapRow()
    {
        _snap = new Switch();
        _snap.Toggled += (_, e) =>
        {
            ShowSchools(e.Value);
            _measure.Text = e.Value
                ? "Red dots are schools; a vertex within 25 px lands on one"
                : "Vertices land where the finger leaves them";
        };

        var row = new HorizontalStackLayout { Spacing = 8 };
        row.Add(new Label { Text = "Snap to schools", FontSize = 12, TextColor = PanelInk, VerticalOptions = LayoutOptions.Center });
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

        var corrected = new PolygonShape(ring);
        ReplaceEditShape(feature, corrected);

        // Otherwise the corner springs back to a right angle and nothing says why.
        // The measurement is the corrected shape's, not the quadrilateral's, which is
        // what the handler that ran before this one had already written.
        _measure.Text = GetMeasureResult(corrected) + " - kept rectangular";
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
            // Drawing is not editing: the shapes go back to the layer, because a
            // control point under the finger takes the touch that was meant to put
            // down the next vertex.
            case 1:
                MoveEverythingTo(_layerOverlay);
                _map.TrackOverlay.TrackMode = TrackMode.Point;
                break;
            case 2:
                MoveEverythingTo(_layerOverlay);
                _map.TrackOverlay.TrackMode = TrackMode.Line;
                break;
            case 3:
                MoveEverythingTo(_layerOverlay);
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

}
