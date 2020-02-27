# Labeling Style Sample for Android

### Description

This samples shows you how to examine different labeling techniques to make your maps informative.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_mobile_for_android) for the details.

![Screenshot](https://github.com/ThinkGeo/LabelingStyleSample-ForAndroid/blob/master/ScreenShot.png)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code

```csharp
textStyle.Mask.IsActive = useMask;
textStyle.GridSize = gridSizeValue;
textStyle.DuplicateRule = labelDuplicateRule;
textStyle.OverlappingRule = allowOverlapping ? LabelOverlappingRule.AllowOverlapping : LabelOverlappingRule.NoOverlapping;
featureLayer.DrawingMarginInPixel = (float)drawingMarginInPixel;
```

### Getting Help

- [Map Suite mobile for Android Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_mobile_for_android)
- [Map Suite mobile for Android Product Description](https://thinkgeo.com/ui-controls#mobile-platforms)
- [ThinkGeo Community Site](http://community.thinkgeo.com/)
- [ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Styles.TextStyle](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.styles.textstyle)
- [ThinkGeo.MapSuite.Layers.FeatureLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.featurelayer)
- [ThinkGeo.MapSuite.Layers.ZoomLevelSet](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.zoomlevelset)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
