using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// A layer slipped into the middle of someone else's style: aerial imagery at the
/// bottom, a white wash over it, and the street style's lines and labels on top -
/// the wash inserted under the labels rather than over everything.
/// </summary>
public class InsertALayerUnderTheLabels : SamplePage
{
    private ThinkGeoRasterTileSource _aerial;
    private ThinkGeoVectorTileSource _vector;
    private FeatureSourceVectorTileSource _washQuad;
    private string _streetStyleJson;
    private MapView _map;
    private Label _status;
    private double _wash = 0.45;

    protected override async void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        // Downtown Dallas.
        map.CenterPoint = new PointShape(-10775615, 3866073);
        map.MapScale = 75000;

        _status = AddStatusLabel();
        AddToPanel(new Label { Text = "Wash opacity", FontSize = 11, TextColor = Colors.Gray });
        var slider = new Slider { Minimum = 0, Maximum = 1, Value = _wash };
        slider.ValueChanged += (_, e) => _wash = e.NewValue;
        slider.DragCompleted += (_, _) => _ = ApplyAsync();
        AddToPanel(slider);

        _aerial = new ThinkGeoRasterTileSource(SampleShared.CloudApiKey, ThinkGeoCloudRasterMapsMapType.Aerial_V2_X1);
        _vector = new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        };

        // The street style as one document, resolved once: every wash change composes
        // a fresh style from it.
        var street = new MapStyle();
        street.AddStyle(ThinkGeoVectorStyles.Light, _vector);
        await street.ResolveAsync();
        _streetStyleJson = street.ToJson();

        // The wash is one world-covering polygon, cut into tiles like any other vector
        // data - which is what keeps it whole at every zoom.
        var world = new InMemoryFeatureSource(Array.Empty<FeatureSourceColumn>(), new Collection<Feature>
        {
            new(new RectangleShape(-20037000, 20037000, 20037000, -20037000).GetWellKnownBinary(), "world"),
        });
        _washQuad = new FeatureSourceVectorTileSource();
        _washQuad.FeatureSources.Add("world", world);

        map.Basemap = new GpuBasemap(ComposeStyle(_wash));
        _status.Text = FormattableString.Invariant($"wash {_wash:0.00}");
        await map.RefreshAsync();
    }

    private MapStyle ComposeStyle(double washOpacity)
    {
        var style = new MapStyle().AddRaster(_aerial, sourceId: "aerial");

        // Only the street style's lines and symbols: its fills would paint an opaque
        // land base over the imagery.
        style.AddStyle(_streetStyleJson, _vector, StyleLayerType.Line, StyleLayerType.Symbol);

        style.AddVectorTiles(_washQuad, "washsrc", StyleLayerSlot.UnderLabels,
            StyleLayer.CreateFill("wash", "world",
                paint: (_, _) => new FillPaint(GeoColors.White, (float)washOpacity), antialias: false));

        return style;
    }

    private async Task ApplyAsync()
    {
        try
        {
            await _map.Basemap.SetStyleAsync(ComposeStyle(_wash));
            _status.Text = FormattableString.Invariant($"wash {_wash:0.00}");
        }
        catch (Exception exception)
        {
            _status.Text = exception.Message.Split('\n')[0];
        }

        await _map.RefreshAsync();
    }
}
