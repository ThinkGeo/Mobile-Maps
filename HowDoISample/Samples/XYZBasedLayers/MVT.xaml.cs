using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.XYZBasedLayers;

public partial class MVT
{
    private bool _initialized;
    private MvtTilesAsyncLayer _mvtLayer;

    public ObservableCollection<string> StyleUris { get; } = new ObservableCollection<string>
    {
        "https://demotiles.maplibre.org/style.json",
        "https://demotiles.maplibre.org/styles/osm-bright-gl-style/style.json",
        "https://demotiles.maplibre.org/styles/osm-bright-gl-terrain/style.json",
        "https://tiles.preludemaps.com/styles/WorldStreets_Light/style.json"
    };

    private int _selectedStyleIndex;
    public int SelectedStyleIndex
    {
        get => _selectedStyleIndex;
        set
        {
            if (_selectedStyleIndex != value)
            {
                _selectedStyleIndex = value;
                OnPropertyChanged(nameof(SelectedStyleIndex));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public MVT()
    {
        InitializeComponent();
        BindingContext = this;
        SelectedStyleIndex = 0;
    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized) return;
        _initialized = true;

        MapView.MapUnit = GeographyUnit.Meter;

        var layerOverlay = new LayerOverlay
        {
            TileType = TileType.MultiTile
        };

        _mvtLayer = new MvtTilesAsyncLayer("https://demotiles.maplibre.org/style.json");
        string cachePath = Path.Combine(FileSystem.AppDataDirectory, "rasterMbTilesLayerCache");

        if (!Directory.Exists(cachePath))
        {
            Directory.CreateDirectory(cachePath);
        }

        _mvtLayer.VectorTileCache = new FileTileCache(cachePath);

        layerOverlay.Layers.Add(_mvtLayer);
        MapView.Overlays.Add(layerOverlay);

        await _mvtLayer.OpenAsync();

        var boundingBox = _mvtLayer.GetBoundingBox();
        await MapView.ZoomToAsync(boundingBox);

        await MapView.RefreshAsync();
    }

    private async void StylePicker_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        if (!_initialized || _mvtLayer == null) return;

        var picker = sender as Picker;
        var selectedUri = picker?.SelectedItem as string;

        if (string.IsNullOrEmpty(selectedUri)) return;

        _mvtLayer.StyleJsonUri = selectedUri;

        await _mvtLayer.CloseAsync();
        await _mvtLayer.OpenAsync();
        await MapView.RefreshAsync();
    }

    public void Dispose()
    {
        MapView?.Dispose();
        GC.SuppressFinalize(this);
    }
}