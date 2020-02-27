# Draw Edit Features Sample for Android

### Description

This samples shows you how to implement drawing and editing shapes into your web application.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_mobile_for_android) for the details.

![Screenshot](https://github.com/ThinkGeo/DrawEditFeaturesSample-ForAndroid/blob/master/ScreenShot.png)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code

```csharp
androidMap.TrackOverlay.TrackMode = TrackMode.None;
foreach (Feature feature in androidMap.TrackOverlay.TrackShapeLayer.InternalFeatures)
{
  androidMap.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature);
}
androidMap.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
androidMap.EditOverlay.CalculateAllControlPoints();
androidMap.Refresh();
```

### Getting Help

- [Map Suite mobile for Android Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_mobile_for_android)
- [Map Suite mobile for Android Product Description](https://thinkgeo.com/ui-controls#mobile-platforms)
- [ThinkGeo Community Site](http://community.thinkgeo.com/)
- [ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Android.TrackInteractiveOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.android.trackinteractiveoverlay)
- [ThinkGeo.MapSuite.Android.EditInteractiveOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.android.editinteractiveoverlay)
- [ThinkGeo.MapSuite.Android.TrackMode](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.android.trackmode)
- [ThinkGeo.MapSuite.Android.MapView](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.android.mapview)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
