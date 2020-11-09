# Building 3D Sample for iOS

### Description
This project shows to create a simulated 3D buildings with iOS map control and shapefile.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_mobile_for_ios) for the details.

![Screenshot](ScreenShot.png)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code

```csharp
OsmBuildingOverlay buildingOverlay = new OsmBuildingOverlay();
string buildingFilePath = @"osm_buildings_900913_min.shp";
ShapeFileFeatureSource shapeFileFeatureSource = new ShapeFileFeatureSource(buildingFilePath);
buildingOverlay.BuildingFeatureSource = shapeFileFeatureSource;
mapView.Overlays.Add(buildingOverlay);

shapeFileFeatureSource.Open();
mapView.CurrentExtent = shapeFileFeatureSource.GetBoundingBoxById("1");
mapView.Refresh();
```

### Getting Help

- [Map Suite mobile for iOS Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_mobile_for_ios)
- [Map Suite mobile for iOS Product Description](https://thinkgeo.com/ui-controls#mobile-platforms)
- [ThinkGeo Community Site](http://community.thinkgeo.com/)
- [ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.iOS.OsmBuildingOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.ios.OsmBuildingOverlay)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
