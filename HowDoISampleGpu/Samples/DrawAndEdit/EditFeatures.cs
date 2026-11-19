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
/// </summary>
public class EditFeatures : SamplePage
{
    private readonly ObservableCollection<string> _log = new();

    private LayerOverlay _layerOverlay;
    private MapView _map;
    private Label _measure;
    private Label _logLabel;
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
