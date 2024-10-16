using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class EsriGridLayer
{
    private bool _initialized;
    public EsriGridLayer()
    {
        InitializeComponent();
    }

    private async void ESRIGridLayer_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // It is important to set the map unit first to either feet, meters or decimal degrees.
        MapView.MapUnit = GeographyUnit.Meter;

        // Create background world map with vector tile requested from ThinkGeo Cloud Service. 
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // Create a new overlay that will hold our new layer and add it to the map.
        var staticOverlay = new LayerOverlay();
        MapView.Overlays.Add(staticOverlay);

        // Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
        var gridFeatureLayer = new GridFeatureLayer
        {
            PathFilename = Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "GridFile", "Mosquitoes.grd"),
            FeatureSource =
            {
                ProjectionConverter = new ProjectionConverter(2276, 3857)
            }
        };

        // Add the layer to the overlay we created earlier.
        staticOverlay.Layers.Add("GridFeatureLayer", gridFeatureLayer);

        // Create a class break style based on the cell values and set area styles based on the values
        var gridClassBreakStyle = new ClassBreakStyle("CellValue");
        gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(double.MinValue,
            new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, 0, 255, 0)))));
        gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(12,
            new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, 128, 255, 128)))));
        gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(24,
            new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, 224, 251, 132)))));
        gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(36,
            new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, 225, 255, 0)))));
        gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(48,
            new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, 245, 210, 10)))));
        gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(60,
            new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, 255, 128, 0)))));
        gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(72,
            new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, 255, 0, 0)))));
        gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(double.MaxValue,
            new AreaStyle(new GeoSolidBrush(GeoColors.Transparent))));

        // Take the class break style we created above and set it on zoom level 1 and then apply it to all zoom levels up to 20.
        gridFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(gridClassBreakStyle);
        gridFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Set the map scale and center point
        MapView.MapScale = 150_000;
        MapView.CenterPoint = new PointShape(-10778441, 3914624);
        await MapView.RefreshAsync();
    }
}