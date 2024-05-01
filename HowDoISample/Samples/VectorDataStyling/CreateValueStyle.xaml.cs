﻿using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.VectorDataStyling;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CreateValueStyle
{
    private bool _initialized;
    public CreateValueStyle()
    {
        InitializeComponent();
    }

    private async void CreateValueStyle_OnSizeChanged(object sender, EventArgs e)
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

        var friscoCrime = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Frisco_Crime.shp"));

        var legend = new LegendAdornmentLayer();

        // Project the layer's data to match the projection of the map
        friscoCrime.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        // Add friscoCrimeLayer to a LayerOverlay
        var layerOverlay = new LayerOverlay();
        layerOverlay.Layers.Add(friscoCrime);

        // Set up the legend adornment
        legend.Title = new LegendItem
        {
            TextStyle = new TextStyle("Crime Categories", new GeoFont("Verdana", 10, DrawingFontStyles.Bold),
                GeoBrushes.Black)
        };
        legend.Height = 600;
        legend.Location = AdornmentLocation.LowerRight;
        MapView.AdornmentOverlay.Layers.Add(legend);

        AddValueStyle(friscoCrime, legend);

        // Add layerOverlay to the mapView
        MapView.Overlays.Add(layerOverlay);

        // Set the map scale and center point
        MapView.MapScale = 40000;
        MapView.CenterPoint = new PointShape(-10777932, 3912260);
        await MapView.RefreshAsync();
    }

    private static void AddValueStyle(FeatureLayer friscoCrime, LegendAdornmentLayer legend)
    {
        // Get all the distinct OffenseGroups in the friscoCrime data
        friscoCrime.Open();
        var offenseGroups = friscoCrime.FeatureSource.GetDistinctColumnValues("OffenseGro");
        friscoCrime.Close();

        // Create a set of colors to represent each OffenseGroup using a spectrum starting from red
        var colors = GeoColor.GetColorsInQualityFamily(GeoColors.Red, offenseGroups.Count);

        // Create a ValueItem styled with a PointStyle to represent each instance of an OffenseGroup
        var valueItems = new Collection<ValueItem>();
        foreach (var offenseGroup in offenseGroups)
        {
            // Create a PointStyle to represent the OffenseGroup by selecting a color using the index of the OffenseGroup
            var style = PointStyle.CreateSimpleCircleStyle(colors[offenseGroups.IndexOf(offenseGroup)], 10,
                GeoColors.Black, 2);

            // Create a ValueItem that will house the pointStyle for the OffenseGroup
            valueItems.Add(new ValueItem(offenseGroup.ColumnValue, style));

            // Add a LegendItem to the legend adornment
            var legendItem = new LegendItem
            {
                ImageStyle = style,
                TextStyle = new TextStyle(offenseGroup.ColumnValue, new GeoFont("Verdana", 10), GeoBrushes.Black)
            };
            legend.LegendItems.Add(legendItem);
        }

        // Create the ValueStyle that will use the previously created valueItems to style the data using the OffenseGroup column values
        var valueStyle = new ValueStyle("OffenseGro", valueItems);

        // Add the valueStyle to the friscoCrime layer's CustomStyles and apply the style to all ZoomLevels
        friscoCrime.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(valueStyle);
        friscoCrime.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    }
}