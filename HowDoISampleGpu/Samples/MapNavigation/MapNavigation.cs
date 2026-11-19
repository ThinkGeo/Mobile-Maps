using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The map's own events, as they happen: the extent changing, the scale changing,
/// taps. Pan, pinch, turn and tilt the map and watch the log.
/// </summary>
public class MapNavigation : SamplePage
{
    private readonly ObservableCollection<string> _log = new();
    private Label _centre;
    private Label _entries;
    private int _index;

    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Dark,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            {
                VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
            }));

        _centre = AddToPanel(new Label { FontSize = 12, FontFamily = "Courier" });
        _entries = AddToPanel(new Label { FontSize = 11, TextColor = Colors.DimGray, MaxLines = 6 });

        var empireStateBuilding = ProjectionConverter.Convert(4326, 3857, new PointShape(-73.9856654, 40.74843661));
        map.MapRotation = -30;   // the desktop map calls this RotationAngle
        map.MapScale = 100000;
        map.CenterPoint = empireStateBuilding;

        map.CurrentExtentChanged += (_, e) =>
        {
            // The MAUI event carries the centre points rather than the extents the
            // desktop one does; the centre is what this readout wants anyway.
            var centre = e.NewCenterPoint ?? map.CenterPoint;
            if (centre == null) return;
            ShowCentre(centre);
            var degrees = ProjectionConverter.Convert(3857, 4326, centre);
            Append($"CurrentExtentChanged: {degrees.Y:N4}, {degrees.X:N4}");
        };

        map.CurrentScaleChanged += (_, e) => Append($"CurrentScaleChanged: 1:{e.OldScale:N0} to 1:{e.NewScale:N0}");
        map.SingleTap += (_, e) =>
        {
            var world = map.ToWorldCoordinate(e.X, e.Y);
            Append($"SingleTap: ({world.X:N0}, {world.Y:N0})");
        };
        map.DoubleTap += (_, e) =>
        {
            var world = map.ToWorldCoordinate(e.X, e.Y);
            Append($"DoubleTap: ({world.X:N0}, {world.Y:N0})");
        };

        // The readout starts filled rather than waiting for the first gesture: a panel
        // that is blank on open reads as broken, not as idle.
        ShowCentre(map.CenterPoint);
        Append("opened");

        _ = map.RefreshAsync();
    }

    private void ShowCentre(PointShape centre)
    {
        var degrees = ProjectionConverter.Convert(3857, 4326, centre);
        _centre.Text = $"Lat {degrees.Y:N4}, Lon {degrees.X:N4}";
    }

    private void Append(string message)
    {
        _log.Insert(0, $"{++_index}. {message}");
        while (_log.Count > 6) _log.RemoveAt(_log.Count - 1);
        _entries.Text = string.Join("\n", _log);
    }
}
