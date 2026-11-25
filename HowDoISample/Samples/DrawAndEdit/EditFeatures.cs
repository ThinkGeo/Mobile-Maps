using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// Drawing and editing by hand: pick a mode and draw points, lines or polygons, or
/// switch to editing and drag, resize, rotate and reshape what is there, with a
/// measurement following whatever is being moved.
/// <para>
/// A rule that moves an edit somewhere else - snapping - is here too, on the edit
/// overlay's own event, because it belongs with the editing rather than beside it.
/// A rule that CORRECTS an edit afterwards is not, and cannot be written against
/// <c>VertexMoved</c> as it stands: the overlay writes its own moved feature into
/// EditShapesLayer after the event has been raised, so whatever a handler puts there
/// is overwritten before the next frame. Every platform does this, so it is the
/// event's behaviour rather than a phone's.
/// </para>
/// </summary>
public class EditFeatures : SamplePage
{
    /// <summary>How near the finger must come, in pixels, before a vertex snaps.</summary>
    private const float SnapTolerance = 25;

    /// <summary>The panel's ink for a control that is switched off for now.</summary>
    private static readonly Color DimInk = Color.FromArgb("#9AA0A6");

    private LayerOverlay _layerOverlay;
    private LayerOverlay _schoolsOverlay;
    private ShapeFileFeatureLayer _schoolsLayer;
    private MapView _map;
    private Label _measure;
    private Switch _snap;
    private Label _snapLabel;

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

        map.EditOverlay.VertexMoving += EditOverlay_VertexMoving;

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

        _snapLabel = new Label
        {
            Text = "Snap to schools",
            FontSize = 12,
            TextColor = PanelInk,
            VerticalOptions = LayoutOptions.Center,
        };

        var row = new HorizontalStackLayout { Spacing = 8 };
        row.Add(_snapLabel);
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

    private void ChooseMode(int index)
    {
        // Snapping is a rule about moving a vertex, so it has nothing to say in any
        // mode but editing. Left live, the switch read as something the map was
        // ignoring; switched off for now, it says which mode it belongs to. The
        // schools go with it - they are what there is to snap to, and in a mode that
        // cannot snap they are red dots over the map for no reason.
        var editing = index == 4;
        if (_snap != null)
        {
            _snap.IsEnabled = editing;
            _snapLabel.TextColor = editing ? PanelInk : DimInk;
            ShowSchools(editing && _snap.IsToggled);
        }

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
