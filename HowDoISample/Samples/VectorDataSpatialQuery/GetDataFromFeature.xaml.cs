using System.Text;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataSpatialQuery;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class GetDataFromFeature
{
    private bool _initialized;
    private ShapeFileFeatureLayer _parksLayer;
    public GetDataFromFeature()
    {
        InitializeComponent();

        MapView.SingleTap += MapView_SingleTap;
    }

    private async void GetDataFromFeature_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service. 
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // Set the Map Unit to meters (used in Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        // Create a feature layer to hold the Frisco parks data
        _parksLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Parks.shp"));

        // Convert the Frisco shapefile from its native projection to Spherical Mercator, to match the map
        var projectionConverter = new ProjectionConverter(2276, 3857);
        _parksLayer.FeatureSource.ProjectionConverter = projectionConverter;

        // Add a style to use to draw the Frisco parks polygons
        _parksLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.MediumPurple), GeoColors.MediumPurple, 2);
        _parksLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add the feature layer to an overlay, and add the overlay to the map
        var parksOverlay = new LayerOverlay();
        parksOverlay.Layers.Add("Frisco Parks", _parksLayer);
        MapView.Overlays.Add(parksOverlay);

        // Add a PopupOverlay to the map, to display feature information
        var popupOverlay = new PopupOverlay();
        MapView.Overlays.Add("Info Popup Overlay", popupOverlay);

        // Set the map extent to the bounding box of the parks
        _parksLayer.Open();
        MapView.MapScale = 70_000;
        MapView.CenterPoint = new PointShape(-10778098, 3915623);
        _parksLayer.Close();

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Get a feature based on a location
    /// </summary>
    private Feature GetFeatureFromLocation(BaseShape location)
    {
        // Find the feature that was tapped on by querying the layer for features containing the tapped coordinates
        _parksLayer.Open();
        var selectedFeature = _parksLayer.QueryTools.GetFeaturesContaining(location, ReturningColumnsType.AllColumns)
            .FirstOrDefault();
        _parksLayer.Close();

        return selectedFeature;
    }

    /// <summary>
    ///     Display a popup containing a feature's info
    /// </summary>
    private void DisplayFeatureInfo(Feature feature)
    {
        var parkInfoString = new StringBuilder();

        // Each column in a feature is a data attribute
        // Add attribute pairs to the info string
        parkInfoString.AppendLine($"NAME: {feature.ColumnValues["NAME"]}");
        parkInfoString.AppendLine($"ADDR: {feature.ColumnValues["ADDRESS"]}");
        parkInfoString.AppendLine($"Type: {feature.ColumnValues["P_TYPE"]}");
        parkInfoString.Append($"ACRE: {feature.ColumnValues["ACRES"]}");

        //Create a new popup with the park info string
        var popupOverlay = (PopupOverlay)MapView.Overlays["Info Popup Overlay"];
        var popup = new Popup
        {
            Position = feature.GetShape().GetCenterPoint(),
            Text = parkInfoString.ToString()
        };

        //Clear the popup overlay and add the new popup to it
        popupOverlay.Children.Clear();
        popupOverlay.Children.Add(popup);
    }

    /// <summary>
    ///     Pull data from the selected feature and display it when tapped
    /// </summary>
    private void MapView_SingleTap(object _, SingleTapMapViewEventArgs e)
    {
        // Get the selected feature based on the map tap location
        var pointInWorldCoordinate = MapView.ToWorldCoordinate(e.X, e.Y);
        var selectedFeature = GetFeatureFromLocation(pointInWorldCoordinate);

        // If a feature was selected, get the data from it and display it
        if (selectedFeature != null)
            DisplayFeatureInfo(selectedFeature);
    }
}