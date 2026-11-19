using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Every layer a style declares, listed in style order, each one switchable. Turning
/// a layer off is a renderer decision, not a restyle: no tile is rebuilt.
/// </summary>
public class StyleLayerToggles : SamplePage
{
    private MapStyle _style;

    protected override async void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;

        _style = new MapStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });
        map.Basemap = new GpuBasemap(_style);
        map.CurrentExtent = new RectangleShape(-8_240_000, 4_975_000, -8_228_000, 4_965_000); // Manhattan
        await map.RefreshAsync();

        // StyleLayers can only be read once the style is COMPILED - the sprite sheet
        // fetched, the sources resolved, every paint expression turned into a delegate.
        // A map that draws the style does that, but the GPU overlay opens
        // asynchronously, so RefreshAsync returns before it has. OpenAsync is the
        // explicit ask; without it the list comes back empty and the panel says
        // "0 layers" over a map that is plainly drawing those very layers.
        await _style.OpenAsync();
        var layers = _style.StyleLayers.ToList();
        AddToPanel(new Label
        {
            Text = FormattableString.Invariant($"{layers.Count} layers"),
            FontAttributes = FontAttributes.Bold,
            FontSize = 13,
        });

        foreach (var layer in layers)
        {
            var row = new HorizontalStackLayout { Spacing = 8 };
            var toggle = new Switch { IsToggled = true, Scale = 0.75 };
            var id = layer.Id;
            toggle.Toggled += (_, e) =>
            {
                _style.SetLayerVisibility(id, e.Value);
                _ = map.RefreshAsync();
            };
            row.Add(toggle);
            row.Add(new Label { Text = id, FontSize = 12, VerticalOptions = LayoutOptions.Center });
            AddToPanel(row);
        }
    }
}
