using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Miscellaneous;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class WorldProjection
{
    private bool _initialized;
    private ShapeFileFeatureLayer _worldLayer;

    public WorldProjection()
    {
        InitializeComponent();
    }

    private async void WorldProjection_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        MapView.IsRotationEnabled = true;
        MapView.MapUnit = GeographyUnit.Meter;

        // Create a new overlay that will hold our new layer and add it to the map.
        var layerOverlay = new LayerOverlay();
        MapView.Overlays.Add(layerOverlay);

        // Create the world layer, it will be decimal degrees at first, but we will be able to change it
        _worldLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Countries02.shp"));
        _worldLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
        // Set up the styles for the countries for zoom level 1 and then apply it until zoom level 20
        _worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.DarkSlateGray;
        _worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush =
            new GeoSolidBrush(GeoColor.FromHtml("#C9E1BE"));
        _worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add the layer to the overlay we created earlier.
        layerOverlay.Layers.Add(_worldLayer);

        // Set the map scale and center point
        MapView.MapScale = 80_000_000;
        MapView.CenterPoint = new PointShape(0, 0);
        await MapView.RefreshAsync();

        CompassButton.Clicked += async (_, _) => await MapView.ZoomToExtentAsync(MapView.CenterPoint, MapView.MapScale, 0);
    }

    private async void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value) return;
        if (_worldLayer == null) return;

        var radioButton = (RadioButton)sender;
        switch (radioButton.StyleId)
        {
            case "PolarStereographic":
                // Set the new projection converter and open it.  Next set the map to the correct map unit and new extent
                _worldLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, SampleKeys.ProjString1);
                MapView.MapUnit = GeographyUnit.Meter;
                MapView.MapRotation = 0;
                MapView.MapScale = 13_0000_000;
                MapView.CenterPoint = new PointShape(176160, -818529);
                break;
            case "DecimalDegrees":
                // Set the new projection to null as the original data is in decimal degrees. 
                _worldLayer.FeatureSource.ProjectionConverter = null;
                MapView.MapUnit = GeographyUnit.DecimalDegree;
                MapView.MapRotation = 0;
                MapView.MapScale = 80_000_000;
                MapView.CenterPoint = new PointShape(20, 14);
                break;
            case "SphericalMercator":
                // Set the new projection to null as the original data is in decimal degrees. 
                _worldLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
                MapView.MapUnit = GeographyUnit.Meter;
                MapView.MapRotation = 0;
                MapView.MapScale = 80_000_000;
                MapView.CenterPoint = new PointShape(0, 0);
                break;
            case "MgaZone55":
                // Set the new projection converter and open it.  Next set the map to the correct map unit and new extent
                _worldLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, SampleKeys.ProjString2);
                MapView.MapUnit = GeographyUnit.Meter;
                MapView.MapRotation = 0;
                MapView.MapScale = 16_000_000;
                MapView.CenterPoint = new PointShape(-1202852, 6853983);
                break;
            case "AlbersEqualAreaConic":
                _worldLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 
                    "+proj=aea +lat_1=20 +lat_2=60 +lat_0=40 +lon_0=-96 +x_0=0 +y_0=0 +ellps=GRS80 +datum=NAD83 +units=m no_defs");
                MapView.MapUnit = GeographyUnit.Meter;
                MapView.MapRotation = 0;
                MapView.MapScale = 70_000_000;
                MapView.CenterPoint = new PointShape(-107573, -246560);
                break;
            case "EqualAreaCylindrical":
                // Set the new projection converter and open it.  Next set the map to the correct map unit and new extent
                _worldLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 
                    "+proj=cea +lon_0=0 +x_0=0 +y_0=0 +lat_ts=45 +ellps=WGS84 +datum=WGS84 +units=m +no_defs");
                MapView.MapUnit = GeographyUnit.Meter;
                MapView.MapRotation = 0;
                MapView.MapScale = 80_000_000;
                MapView.CenterPoint = new PointShape(1152761, 496982);
                break;
        }
        _worldLayer.FeatureSource.ProjectionConverter?.Open();
        await MapView.RefreshAsync();
    }
}