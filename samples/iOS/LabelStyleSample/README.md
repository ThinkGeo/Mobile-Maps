# Labeling Style Sample for iOS

### Description

This samples shows you how to examine different labeling techniques to make your maps informative.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_mobile_for_ios) for the details.

![Screenshot](ScreenShot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code

```CSharp
WkbFileFeatureLayer subdivisionsLayer = new WkbFileFeatureLayer("AppData/WkbFiles/Subdivisions.wkb");
subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.StandardColors.White, GeoColor.FromHtml("#9C9C9C"), 1);
subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = new TextStyle("NAME_COMMO", new GeoFont("Arail", 9, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.SimpleColors.Black));
subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.HaloPen = new GeoPen(GeoColor.StandardColors.White, 1);
subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.Mask = new AreaStyle();
subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.BestPlacement = true;
subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.GridSize = 3000;
subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.NoDuplicateLabels;
subdivisionsLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.SuppressPartialLabels = true;
subdivisionsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

LayerOverlay labelingPolygonsOverlay = new LayerOverlay();
labelingPolygonsOverlay.Layers.Add("subdivision", subdivisionsLayer);
MapView.Overlays.Add("LabelingPolygons", labelingPolygonsOverlay);
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
- [ThinkGeo.MapSuite.Core.WkbFileFeatureLayer](http://wiki.thinkgeo.com/wiki/thinkgeo.mapsuite.core.wkbfilefeaturelayer)
### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
