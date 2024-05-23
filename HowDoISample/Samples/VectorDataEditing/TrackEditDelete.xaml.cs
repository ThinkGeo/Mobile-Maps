using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataEditing;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class TrackEditDelete
{
    private bool _initialized;
    public TrackEditDelete()
    {
        InitializeComponent();
        MapView.SingleTap += MapView_SingleTap;
        CompassButton.Clicked += async (_, _) =>
            await MapView.ZoomToExtentAsync(MapView.CenterPoint, MapView.MapScale, 0);
    }

    public static readonly BindableProperty InstructionProperty = BindableProperty.Create(
        propertyName: nameof(Instruction),
        returnType: typeof(string),
        declaringType: typeof(TrackEditDelete),
        defaultValue: null);

    public string Instruction
    {
        get => (string)GetValue(InstructionProperty);
        set => SetValue(InstructionProperty, value);
    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        // Add Cloud Maps as a background overlay
        var backgroundOverlay = new ThinkGeoVectorOverlay(SampleKeys.ClientId, SampleKeys.ClientSecret,
            ThinkGeoCloudVectorMapsMapType.Dark)
        {
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorDark_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // Set the map extent
        MapView.CenterPoint = new RectangleShape(-10786436, 3918518, -10769429, 3906002).GetCenterPoint();
        MapView.MapScale = 200000;

        // Create the layer that will store the drawn shapes
        var featureLayer = new InMemoryFeatureLayer();

        // Add styles for the layer
        featureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
            PointStyle.CreateSimpleCircleStyle(GeoColors.Blue, 8, GeoColors.Black);
        featureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
            LineStyle.CreateSimpleLineStyle(GeoColors.Blue, 4, true);
        featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(GeoColors.Blue, GeoColors.Black);
        featureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add the layer to a LayerOverlay
        var layerOverlay = new LayerOverlay();
        layerOverlay.Layers.Add("featureLayer", featureLayer);

        // Add the LayerOverlay to the map
        MapView.Overlays.Add("layerOverlay", layerOverlay);

        // Update instructions
        Instruction = "Draw Point Mode - Tap the map to add a point.";
        MapView.TrackOverlay.TrackMode = TrackMode.Point;

        MapView.IsRotationEnabled = true;
        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Update the layer whenever the user switches modes
    /// </summary>
    private async Task UpdateLayerFeaturesAsync()
    {
        var layerOverlay = (LayerOverlay)MapView.Overlays["layerOverlay"];
        var featureLayer = (InMemoryFeatureLayer)layerOverlay.Layers["featureLayer"];

        // If the user switched away from a Drawing Mode, add all the newly drawn shapes in the TrackOverlay into the featureLayer
        foreach (var feature in MapView.TrackOverlay.TrackShapeLayer.InternalFeatures)
            featureLayer.InternalFeatures.Add(feature.Id, feature);

        // Clear out all the TrackOverlay's features
        MapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();

        // If the user switched away from Edit Mode, add all the shapes that were in the EditOverlay back into the featureLayer
        foreach (var feature in MapView.EditOverlay.EditShapesLayer.InternalFeatures)
            featureLayer.InternalFeatures.Add(feature.Id, feature);

        // Clear out all the EditOverlay's features
        MapView.EditOverlay.EditShapesLayer.InternalFeatures.Clear();
        MapView.EditOverlay.ClearAllControlPoints();

        // Refresh the overlays to show latest results
        await MapView.TrackOverlay.RefreshAsync();
        await MapView.EditOverlay.RefreshAsync();
        await layerOverlay.RefreshAsync();
    }

    private async void NavMode_Click(object sender, EventArgs e)
    {
        if (sender is not RadioButton radioButton)
            return;
        if (!radioButton.IsChecked)
            return;
        if (MapView == null)
            return;
        if (!MapView.Overlays.Contains("layerOverlay"))
            return;

        // Update the layer's features from any previous mode
        await UpdateLayerFeaturesAsync();

        // Set TrackMode to None, so that the user will no longer draw shapes and will be able to navigate the map normally
        MapView.TrackOverlay.TrackMode = TrackMode.None;

        // Update instructions
        Instruction = "Navigation Mode - The default map state. Allows you to pan and zoom the map.";
    }

    /// <summary>
    ///     Set the mode to draw points on the map
    /// </summary>
    private async void DrawPoint_Click(object sender, EventArgs e)
    {
        if (sender is not RadioButton radioButton)
            return;
        if (!radioButton.IsChecked)
            return;
        if (MapView == null)
            return;

        // Update the layer's features from any previous mode
        await UpdateLayerFeaturesAsync();

        // Set TrackMode to Point, which draws a new point on the map on mouse tap
        MapView.TrackOverlay.TrackMode = TrackMode.Point;
        // Update instructions
        Instruction = "Draw Point Mode - Tap the map to add a point.";
    }

    /// <summary>
    ///     Set the mode to draw lines on the map
    /// </summary>
    private async void DrawLine_Click(object sender, EventArgs e)
    {
        if (sender is not RadioButton radioButton)
            return;
        if (!radioButton.IsChecked)
            return;

        // Update the layer's features from any previous mode
        await UpdateLayerFeaturesAsync();

        // Set TrackMode to Line, which draws a new line on the map on mouse tap. Double taps to finish drawing the line.
        MapView.TrackOverlay.TrackMode = TrackMode.Line;

        // Update instructions
        Instruction =
            "Draw Line Mode - Tap the map to add a vertex, tap-move to relocate the vertex, long press to finish.";
    }

    /// <summary>
    ///     Set the mode to draw lines on the map
    /// </summary>
    private async void DrawEllipse_Click(object sender, EventArgs e)
    {
        if (sender is not RadioButton radioButton)
            return;
        if (!radioButton.IsChecked)
            return;

        // Update the layer's features from any previous mode
        await UpdateLayerFeaturesAsync();

        // Set TrackMode to Line, which draws a new line on the map on mouse tap. Double taps to finish drawing the line.
        MapView.TrackOverlay.TrackMode = TrackMode.Ellipse;

        // Update instructions
        Instruction = "Draw Ellipse Mode - Tap-move on the map to draw an ellipse.";
    }

    /// <summary>
    ///     Set the mode to draw polygons on the map
    /// </summary>
    private async void DrawPolygon_Click(object sender, EventArgs e)
    {
        if (sender is not RadioButton radioButton)
            return;
        if (!radioButton.IsChecked)
            return;

        // Update the layer's features from any previous mode
        await UpdateLayerFeaturesAsync();

        // Set TrackMode to Polygon, which draws a new polygon on the map on touch. Double taps to finish drawing the polygon.
        MapView.TrackOverlay.TrackMode = TrackMode.Polygon;

        // Update instructions
        Instruction =
            "Draw Polygon Mode - Tap the map to add a vertex, tap-move to relocate the vertex, long press to finish.";
    }

    /// <summary>
    ///     Set the mode to edit drawn shapes
    /// </summary>
    private async void EditShape_Click(object sender, EventArgs e)
    {
        if (sender is not RadioButton radioButton)
            return;
        if (!radioButton.IsChecked)
            return;

        // Update the layer's features from any previous mode

        var layerOverlay = (LayerOverlay)MapView.Overlays["layerOverlay"];
        var featureLayer = (InMemoryFeatureLayer)layerOverlay.Layers["featureLayer"];

        // If the user switched away from a Drawing Mode, add all the newly drawn shapes in the TrackOverlay into the featureLayer
        foreach (var feature in MapView.TrackOverlay.TrackShapeLayer.InternalFeatures)
            MapView.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature.Id, feature);
        MapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();

        // Put all features in the featureLayer into the EditOverlay
        foreach (var feature in featureLayer.InternalFeatures)
            MapView.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature.Id, feature);
        featureLayer.InternalFeatures.Clear();

        // This method draws all the handles and manipulation points on the map to edit. 
        MapView.EditOverlay.CalculateAllControlPoints();

        // Refresh the overlays to show latest results
        await MapView.TrackOverlay.RefreshAsync();
        await MapView.EditOverlay.RefreshAsync();
        await layerOverlay.RefreshAsync();

        // Set TrackMode to None, so that the user will no longer draw shapes
        MapView.TrackOverlay.TrackMode = TrackMode.None;

        // Update instructions
        Instruction =
            "Edit Mode: Use control points to translate, rotate, or scale shapes. Tap and drag a vertex to move it, tap a line segment to add a vertex, and double-tap a vertex to remove it.";
    }

    /// <summary>
    ///     Set the mode to delete features ont the map
    /// </summary>
    private async void DeleteShape_Click(object sender, EventArgs e)
    {
        if (sender is not RadioButton radioButton)
            return;
        if (!radioButton.IsChecked)
            return;

        // Update the layer's features from any previous mode
        await UpdateLayerFeaturesAsync();

        // Set TrackMode to None, so that the user will no longer draw shapes
        MapView.TrackOverlay.TrackMode = TrackMode.None;

        // Update instructions
        Instruction = "Delete Shape Mode - Tap a shape to delete it.";
    }

    /// <summary>
    ///     Event handler that finds the nearest feature and removes it from the layer
    /// </summary>
    private async void MapView_SingleTap(object sender, SingleTapMapViewEventArgs e)
    {
        if (!DeleteShapeRadioButton.IsChecked)
            return;

        var layerOverlay = (LayerOverlay)MapView.Overlays["layerOverlay"];
        var featureLayer = (InMemoryFeatureLayer)layerOverlay.Layers["featureLayer"];

        var pointInWorldCoordinate = MapView.ToWorldCoordinate(e.X, e.Y);

        // Query the layer for the closest feature within 100 meters
        var closestFeatures = featureLayer.QueryTools.GetFeaturesNearestTo(pointInWorldCoordinate, GeographyUnit.Meter, 1, new Collection<string>(), 100, DistanceUnit.Meter);

        // If a feature was found, remove it from the layer
        if (closestFeatures.Count <= 0) return;
        featureLayer.InternalFeatures.Remove(closestFeatures[0]);

        // Refresh the layerOverlay to show the results
        await layerOverlay.RefreshAsync();
    }
}