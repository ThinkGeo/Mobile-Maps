# Draw Edit Features Sample for iOS

### Description

This samples shows you how to implement drawing and editing shapes into your web application.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_mobile_for_ios) for the details.

![Screenshot](https://github.com/ThinkGeo/DrawEditFeaturesSample-ForiOS/blob/master/ScreenShot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code

```CSharp
switch (button.Title(UIControlState.Application))
{
    case "Cursor":
        //....
        break;
    case "Clear":
        mapView.EditOverlay.ClearAllControlPoints();
        mapView.EditOverlay.EditShapesLayer.Open();
        mapView.EditOverlay.EditShapesLayer.Clear();
        mapView.TrackOverlay.TrackShapeLayer.Open();
        mapView.TrackOverlay.TrackShapeLayer.Clear();
        mapView.Refresh();
        break;
    case "Point":
        mapView.TrackOverlay.TrackMode = TrackMode.Point;
        break;
    case "Line":
        mapView.TrackOverlay.TrackMode = TrackMode.Line;
        break;
    case "Rectangle":
        mapView.TrackOverlay.TrackMode = TrackMode.Rectangle;
        break;
    case "Polygon":
        mapView.TrackOverlay.TrackMode = TrackMode.Polygon;
        break;
    case "Circle":
        mapView.TrackOverlay.TrackMode = TrackMode.Circle;
        break;
    case "Ellipse":
        mapView.TrackOverlay.TrackMode = TrackMode.Ellipse;
        break;
    case "Edit":
        mapView.TrackOverlay.TrackMode = TrackMode.None;
        foreach (Feature feature in mapView.TrackOverlay.TrackShapeLayer.InternalFeatures)
        {
            mapView.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature);
        }
        mapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
        mapView.EditOverlay.CalculateAllControlPoints();
        mapView.Refresh();
        break;
}
```

### Getting Help

[Map Suite mobile for iOS Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_mobile_for_ios)

[Map Suite mobile for iOS Product Description](https://thinkgeo.com/ui-controls#mobile-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.GeographyUnit](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.geographyunit)
- [ThinkGeo.MapSuite.iOS.MapView](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.ios.mapview)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
