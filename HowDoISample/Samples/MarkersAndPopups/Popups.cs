using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// A popup for every hotel, each one holding its ground point as the map is panned,
/// zoomed and tilted.
/// </summary>
public class Popups : SamplePage
{
    private readonly PopupOverlay _popupOverlay = new();

    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Dark,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            { VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder) }));

        map.CenterPoint = new PointShape(-10777290, 3908740);
        map.MapScale = 9030;

        // A popup stays on its ground point under perspective, so tilting is the thing
        // to try here.
        map.TiltAngle = 40;

        var hotels = new ShapeFileFeatureSource(SampleShared.Shapefile("Hotels.shp"))
        {
            ProjectionConverter = new ProjectionConverter(2276, 3857),
        };

        hotels.Open();
        var count = 0;
        foreach (var feature in hotels.GetAllFeatures(ReturningColumnsType.AllColumns))
        {
            var point = feature.GetShape().GetCenterPoint();

            // A Popup is a ContentView: its Position and its Text are properties.
            _popupOverlay.Add(new Popup
            {
                Position = point,
                Text = feature.ColumnValues["NAME"],
            });
            count++;
        }

        hotels.Close();

        map.Overlays.Add(_popupOverlay);
        AddStatusLabel().Text = $"{count} hotels, each popup holding its ground point";
        _ = map.RefreshAsync();
    }
}
