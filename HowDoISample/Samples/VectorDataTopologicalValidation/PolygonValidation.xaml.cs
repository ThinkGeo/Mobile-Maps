using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataTopologicalValidation;

public partial class PolygonValidation
{
    private bool _initialized;
    public PolygonValidation()
	{
		InitializeComponent();
	}

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to Decimal Degree
        MapView.MapUnit = GeographyUnit.DecimalDegree;

        // Create an InMemoryFeatureLayer to hold the shapes to be validated
        // Add styles to display points, lines, and polygons on this layer in green
        var validatedFeaturesLayer = new InMemoryFeatureLayer();
        validatedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
            PointStyle.CreateSimpleCircleStyle(GeoColors.Green, 12, GeoColors.Green);
        validatedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.Green), GeoColors.Green);
        validatedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
            LineStyle.CreateSimpleLineStyle(GeoColors.Green, 3, false);
        validatedFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Create an InMemoryFeatureLayer to hold the shapes to perform the validation against
        // Add styles to display points, lines, and polygons on this layer in blue
        var filterFeaturesLayer = new InMemoryFeatureLayer();
        filterFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
            PointStyle.CreateSimpleCircleStyle(GeoColors.Blue, 12, GeoColors.Blue);
        filterFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.Blue), GeoColors.Blue);
        filterFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
            LineStyle.CreateSimpleLineStyle(GeoColors.Blue, 3, false);
        filterFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Create an InMemoryFeatureLayer to hold the result features from the validation API
        // Add styles to display points, lines, and polygons on this layer in red
        var resultFeaturesLayer = new InMemoryFeatureLayer();
        resultFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
            PointStyle.CreateSimpleCircleStyle(GeoColors.Red, 12, GeoColors.Red);
        resultFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(50, GeoColors.Red), GeoColors.Red);
        resultFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
            LineStyle.CreateSimpleLineStyle(GeoColors.Red, 3, false);
        resultFeaturesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Add the layers to an overlay, and add the overlay to the map
        var featuresOverlay = new LayerOverlay();
        featuresOverlay.Layers.Add("Filter Features", filterFeaturesLayer);
        featuresOverlay.Layers.Add("Validated Features", validatedFeaturesLayer);
        featuresOverlay.Layers.Add("Result Features", resultFeaturesLayer);
        MapView.Overlays.Add("Features Overlay", featuresOverlay);

        RdoCheckIfPolygonBoundariesOverlapPolygonBoundaries.IsChecked = true;

        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Validate polygons based on whether their boundaries overlap with the boundaries of a second set of polygons, and
    ///     display the results on the map
    /// </summary>
    private async void CheckIfPolygonBoundariesOverlapPolygonBoundaries(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        // Create a sample set of polygon features to use for the validation
        var coveringPolygonFeature = new Feature("POLYGON((0 0,100 0,100 100,0 100,0 0))");
        var coveredPolygonFeature = new Feature("POLYGON((0 0,50 0,50 50,0 50,0 0))");

        // Use the TopologyValidator API to validate the sample data
        var coveringPolygons = new Collection<Feature> { coveringPolygonFeature };
        var coveredPolygons = new Collection<Feature> { coveredPolygonFeature };
        var result =
            TopologyValidator.PolygonBoundariesMustOverlapPolygonBoundaries(coveringPolygons, coveredPolygons);

        // Get the invalid features returned from the API
        var invalidResultFeatures = result.InvalidFeatures;

        // Clear the MapView and add the new valid/invalid features to the map
        await ClearMapAndAddFeatures(
            [coveredPolygonFeature], 
            invalidResultFeatures,
            [coveringPolygonFeature]
            );

        // Update the help text
        TxtValidationInfo.Text =
            "Features being validated against are shown in blue. \nPolygons being validated are shown in green. \nNon-overlapping polygon boundaries are shown in red.";
    }

    /// <summary>
    ///     Validate polygons based on whether their boundaries overlap with a separate set of lines, and display the results
    ///     on the map
    /// </summary>
    private async void CheckIfPolygonBoundariesOverlapLines(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        // Create a sample set of polygon and line features to use for the validation
        var polygonFeature = new Feature("POLYGON((0 0,100 0,100 100,0 100,0 0))");
        var lineFeature = new Feature("LINESTRING(-50 0,100 0,100 150)");

        // Use the TopologyValidator API to validate the sample data
        var polygons = new Collection<Feature> { polygonFeature };
        var lines = new Collection<Feature> { lineFeature };
        var result = TopologyValidator.PolygonBoundariesMustOverlapLines(polygons, lines);

        // Get the invalid features returned from the API
        var invalidResultFeatures = result.InvalidFeatures;

        // Clear the MapView and add the new valid/invalid features to the map
        await ClearMapAndAddFeatures(
            [polygonFeature], 
            invalidResultFeatures,
            [lineFeature]
            );

        // Update the help text
        TxtValidationInfo.Text =
            "Features being validated against are shown in blue. \nPolygons being validated are shown in green. \nNon-overlapping polygon boundaries are shown in red.";
    }

    /// <summary>
    ///     Validate polygons based on whether they overlap a second set of polygons, and display the results on the map
    /// </summary>
    private async void CheckIfPolygonsOverlapPolygons(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        // Create a sample set of polygon features to use for the validation
        var polygonFeature1 = new Feature("POLYGON((25 25,50 25,50 50,25 50,25 25))");
        var polygonFeature2 = new Feature("POLYGON((75 25,125 25,125 75,75 75,75 25))");
        var polygonFeature3 = new Feature("POLYGON((150 25,200 25,200 75,150 75,150 25))");
        var coveringPolygonFeature = new Feature("POLYGON((0 0,100 0,100 100,0 100,0 0))");

        // Use the TopologyValidator API to validate the sample data
        var coveringPolygons = new Collection<Feature> { coveringPolygonFeature };
        var coveredPolygons = new Collection<Feature> { polygonFeature1, polygonFeature2, polygonFeature3 };
        var result = TopologyValidator.PolygonsMustOverlapPolygons(coveringPolygons, coveredPolygons);

        // Get the invalid features returned from the API
        var invalidResultFeatures = result.InvalidFeatures;

        // Clear the MapView and add the new valid/invalid features to the map
        await ClearMapAndAddFeatures(
            [polygonFeature1, polygonFeature2, polygonFeature3],
            invalidResultFeatures, 
            [coveringPolygonFeature]
            );

        // Update the help text
        TxtValidationInfo.Text =
            "Features being validated against are shown in blue. \nOverlapping regions are shown in green. \nNon-overlapping regions are shown in red.";
    }

    /// <summary>
    ///     Validate polygons based on whether they lie within other polygons, and display the results on the map
    /// </summary>
    private async void CheckIfPolygonsAreWithinPolygons(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        // Create a sample set of polygon features to use for the validation
        var polygonFeature1 = new Feature("POLYGON((25 25,50 25,50 50,25 50,25 25))");
        var polygonFeature2 = new Feature("POLYGON((75 25,125 25,125 75,75 75,75 25))");
        var polygonFeature3 = new Feature("POLYGON((150 25,200 25,200 75,150 75,150 25))");
        var coveringPolygonFeature = new Feature("POLYGON((0 0,100 0,100 100,0 100,0 0))");

        // Use the TopologyValidator API to validate the sample data
        var coveringPolygons = new Collection<Feature> { coveringPolygonFeature };
        var coveredPolygons = new Collection<Feature> { polygonFeature1, polygonFeature2, polygonFeature3 };
        var result = TopologyValidator.PolygonsMustBeWithinPolygons(coveringPolygons, coveredPolygons);

        // Get the invalid features returned from the API
        var invalidResultFeatures = result.InvalidFeatures;

        // Clear the MapView and add the new valid/invalid features to the map
        await ClearMapAndAddFeatures(
            [polygonFeature1, polygonFeature2, polygonFeature3],
            invalidResultFeatures, 
            [coveringPolygonFeature]
            );

        // Update the help text
        TxtValidationInfo.Text =
            "Features being validated against are shown in blue. \nPolygons fully within polygons are shown in green. \nPolygons not within polygons are shown in red.";
    }

    /// <summary>
    ///     Validate polygons based on whether they contain points, and display the results on the map
    /// </summary>
    private async void CheckIfPolygonsContainPoints(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        // Create a sample set of points and polygon features to use for the validation
        var pointFeature = new Feature("POINT(40 40)");        
        var polygonWithPointFeature = new Feature("POLYGON((0 0,80 0,80 80,0 80,0 0))");
        var polygonFeature = new Feature("POLYGON((0 100,80 100,80 180,0 180,0 100))");

        // Use the TopologyValidator API to validate the sample data
        var polygons = new Collection<Feature> { polygonFeature, polygonWithPointFeature };
        var points = new Collection<Feature> { pointFeature };
        var result = TopologyValidator.PolygonsMustContainPoint(polygons, points);

        // Get the invalid features returned from the API
        var invalidResultFeatures = result.InvalidFeatures;

        // Clear the MapView and add the new valid/invalid features to the map
        await ClearMapAndAddFeatures(
            [polygonFeature, polygonWithPointFeature],
            invalidResultFeatures, 
            [pointFeature]
            );

        // Update the help text
        TxtValidationInfo.Text =
            "Features being validated against are shown in blue. \nPolygons containing points are shown in green. \nPolygons not containing points are shown in red.";
    }

    /// <summary>
    ///     Validate polygons based on whether they overlap each other, and display the results on the map. Unlike other
    ///     validations, this function validates and returns invalid polygons from both input sets
    /// </summary>
    private async void CheckIfPolygonsCoverEachOther(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        // Create a sample set of polygon features to use for the validation
        var polygonFeature1 = new Feature("POLYGON((0 0,100 0,100 100,0 100,0 0))");
        var polygonFeature2 = new Feature("POLYGON((-50 -50,50 -50,50 50,-50 50,-50 -50))");

        // Use the TopologyValidator API to validate the sample data
        var firstPolygonsSet = new Collection<Feature> { polygonFeature1 };
        var secondPolygonsSet = new Collection<Feature> { polygonFeature2 };
        var result = TopologyValidator.PolygonsMustOverlapEachOther(firstPolygonsSet, secondPolygonsSet);

        // Get the invalid features returned from the API
        var invalidResultFeatures = result.InvalidFeatures;

        // Clear the MapView and add the new valid/invalid features to the map
        await ClearMapAndAddFeatures(
            [polygonFeature1, polygonFeature2], 
            invalidResultFeatures
            );

        // Update the help text
        TxtValidationInfo.Text =
            "All non-overlapping regions from two different sets of polygons are shown in red. \nOverlapping regions are shown in green";
    }

    /// <summary>
    ///     Validate polygons based on whether the union of the polygons has any interior gaps, and display the results on the
    ///     map
    /// </summary>
    private async void CheckIfPolygonsHaveGaps(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        // Create a sample set of polygon features to use for the validation
        var polygonFeature1 = new Feature("POLYGON((0 0,40 0,40 40,0 40,0 0))");
        var polygonFeature2 = new Feature("POLYGON((30 30,70 30,70 70,30 70,30 30))");
        var polygonFeature3 = new Feature("POLYGON((60 0,100 0,100 40,60 40,60 0))");
        var polygonFeature4 = new Feature("POLYGON((30 10,70 10,70 -30,30 -30,30 10))");

        // Use the TopologyValidator API to validate the sample data
        var polygons = new Collection<Feature> { polygonFeature1, polygonFeature2, polygonFeature3, polygonFeature4 };
        var result = TopologyValidator.PolygonsMustNotHaveGaps(polygons);

        // Get the invalid features returned from the API
        var invalidResultFeatures = result.InvalidFeatures;

        // Clear the MapView and add the new valid/invalid features to the map
        await ClearMapAndAddFeatures(
            [polygonFeature1, polygonFeature2, polygonFeature3, polygonFeature4],
            invalidResultFeatures
            );

        // Update the help text
        TxtValidationInfo.Text =
            "Features being validated are shown in green. \nGaps (Inner rings) within the union of the polygons are shown in red.";
    }

    /// <summary>
    ///     Validate polygons based on whether polygons within the same set overlap, and display the results on the map
    /// </summary>
    private async void CheckPolygonsMustNotOverlap(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        // Create a sample set of polygon features to use for the validation
        var polygonFeature1 = new Feature("POLYGON((25 25,50 25,50 50,25 50,25 25))");
        var polygonFeature2 = new Feature("POLYGON((75 25,125 25,125 75,75 75,75 25))");
        var polygonFeature3 = new Feature("POLYGON((150 25,200 25,200 75,150 75,150 25))");
        var polygonFeature4 = new Feature("POLYGON((0 0,100 0,100 100,0 100,0 0))");

        // Use the TopologyValidator API to validate the sample data
        var polygons = new Collection<Feature> { polygonFeature1, polygonFeature2, polygonFeature3, polygonFeature4 };
        var result = TopologyValidator.PolygonsMustNotOverlap(polygons);

        // Get the invalid features returned from the API
        var invalidResultFeatures = result.InvalidFeatures;

        // Clear the MapView and add the new valid/invalid features to the map
        await ClearMapAndAddFeatures(
            [polygonFeature1, polygonFeature2, polygonFeature3, polygonFeature4],
            invalidResultFeatures
            );

        // Update the help text
        TxtValidationInfo.Text =
            "Features being validated are shown in green. \nOverlapping polygon regions are shown in red.";
    }

    /// <summary>
    ///     Validate polygons based on whether they overlap polygons from a separate set, and display the results on the map
    /// </summary>
    private async void CheckPolygonsMustNotOverlapPolygons(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        // Create a sample set of polygon features to use for the validation
        var polygonFeature1 = new Feature("POLYGON((25 25,50 25,50 50,25 50,25 25))");
        var polygonFeature2 = new Feature("POLYGON((75 25,125 25,125 75,75 75,75 25))");
        var polygonFeature3 = new Feature("POLYGON((150 25,200 25,200 75,150 75,150 25))");
        var coveringPolygonFeature = new Feature("POLYGON((0 0,100 0,100 100,0 100,0 0))");

        // Use the TopologyValidator API to validate the sample data
        var coveringPolygons = new Collection<Feature> { coveringPolygonFeature };
        var coveredPolygons = new Collection<Feature> { polygonFeature1, polygonFeature2, polygonFeature3 };
        var result = TopologyValidator.PolygonsMustNotOverlapPolygons(coveringPolygons, coveredPolygons);

        // Get the invalid features returned from the API
        var invalidResultFeatures = result.InvalidFeatures;

        // Clear the MapView and add the new valid/invalid features to the map
        await ClearMapAndAddFeatures(
            [polygonFeature1, polygonFeature2, polygonFeature3],
            invalidResultFeatures, 
            [coveringPolygonFeature]
            );

        // Update the help text
        TxtValidationInfo.Text =
            "Features being validated against are shown in blue. \nNon-overlapping polygon regions are shown in green. \nOverlapping polygon regions are shown in red.";
    }

    /// <summary>
    ///     Clear the previously displayed features from the map, and add new features
    /// </summary>
    private async Task ClearMapAndAddFeatures(Collection<Feature> validatedFeatures, Collection<Feature> resultFeatures,
        Collection<Feature> filterFeatures = null)
    {
        // Get the InMemoryFeatureLayers from the MapView
        var validatedFeaturesOverLay = (LayerOverlay)MapView.Overlays["Features Overlay"];
        var validatedFeaturesLayer = (InMemoryFeatureLayer)validatedFeaturesOverLay.Layers["Validated Features"];
        var filterFeaturesLayer = (InMemoryFeatureLayer)validatedFeaturesOverLay.Layers["Filter Features"];
        var resultFeaturesLayer = (InMemoryFeatureLayer)validatedFeaturesOverLay.Layers["Result Features"];

        validatedFeaturesLayer.Open();
        filterFeaturesLayer.Open();
        resultFeaturesLayer.Open();

        // Clear the existing features from each layer
        validatedFeaturesLayer.Clear();
        filterFeaturesLayer.Clear();
        resultFeaturesLayer.Clear();

        // Add (blue) filter features to the map, if there are any
        if (filterFeatures != null)
            foreach (var filterFeature in filterFeatures)
                filterFeaturesLayer.InternalFeatures.Add(filterFeature);

        // Add (green) validated features to the map
        foreach (var validatedFeature in validatedFeatures)
            validatedFeaturesLayer.InternalFeatures.Add(validatedFeature);

        // Add (red) invalid features to the map
        foreach (var resultFeature in resultFeatures) resultFeaturesLayer.InternalFeatures.Add(resultFeature);

        // Refresh/redraw the layers and reset the map extent
        var featureOverlay = (LayerOverlay)MapView.Overlays["Features Overlay"];
        var centerPoint = featureOverlay.GetBoundingBox().GetCenterPoint();
        centerPoint.Y -= 50;
        MapView.CenterPoint = centerPoint;
        MapView.MapScale = 200000000;        
        await MapView.RefreshAsync();

        validatedFeaturesLayer.Close();
        filterFeaturesLayer.Close();
        resultFeaturesLayer.Close();
    }
}