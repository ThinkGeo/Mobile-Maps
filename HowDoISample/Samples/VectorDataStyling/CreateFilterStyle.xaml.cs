using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataStyling;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CreateFilterStyle
{
    private bool _initialized;
    public CreateFilterStyle()
    {
        InitializeComponent();
    }

    private async void CreateFilterStyle_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        // Add Cloud Maps as a background overlay
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        var friscoCrimeLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Frisco_Crime.shp"))
        {
            FeatureSource =
                {
                    // Project the layer's data to match the projection of the map
                    ProjectionConverter = new ProjectionConverter(2276, 3857)
                }
        };

        // Add friscoCrimeLayer to a LayerOverlay
        var layerOverlay = new LayerOverlay();
        layerOverlay.Layers.Add(friscoCrimeLayer);

        await AddFilterStyle(friscoCrimeLayer);

        // Add layerOverlay to the mapView
        MapView.Overlays.Add(layerOverlay);

        // Set the map scale and center point
        MapView.MapScale = 70000;
        MapView.CenterPoint = new PointShape(-10778209, 3914820);
        await MapView.RefreshAsync();
    }

    private static async Task AddFilterStyle(FeatureLayer layer)
    {
        // Create a filter style based on the "Drugs" Offense Group
        var drugFilterStyle = new FilterStyle
        {
            Conditions = { new FilterCondition("OffenseGro", "Drugs") },
            Styles =
                {
                    new PointStyle(PointSymbolType.Circle, 28, GeoBrushes.White, GeoPens.Red),
                    new PointStyle(new GeoImage(await FileSystem.OpenAppPackageFileAsync("coyote_paw.png"))){ImageScale = .5}
                }
        };

        // Create a filter style based on the "Weapons" Offense Group
        var weaponFilterStyle = new FilterStyle
        {
            Conditions = { new FilterCondition("OffenseGro", "Weapons") },
            Styles =
                {
                    new PointStyle(PointSymbolType.Circle, 28, GeoBrushes.White, GeoPens.Red),
                    new PointStyle(new GeoImage(await FileSystem.OpenAppPackageFileAsync("weapon_icon.png"))){ImageScale = .5}
                }
        };

        // Create a filter style based on the "Vandalism" Offense Group
        var vandalismFilterStyle = new FilterStyle
        {
            Conditions = { new FilterCondition("OffenseGro", "Vandalism") },
            Styles =
                {
                    new PointStyle(PointSymbolType.Circle, 28, GeoBrushes.White, GeoPens.Red),
                    new PointStyle(new GeoImage(await FileSystem.OpenAppPackageFileAsync("vandalism_icon.png"))) {ImageScale = .5}
                }
        };

        // Add the filter styles to the CustomStyles collection
        layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(drugFilterStyle);
        layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(weaponFilterStyle);
        layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(vandalismFilterStyle);
        layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }
}