# ThinkGeo Cloud Maps Sample for iOS

### Description

This sample demonstrates how you can display ThinkGeo Cloud Maps in your Map Suite GIS applications. It will show you how to use the XYZFileBitmapTileCache to improve the performance of map rendering. ThinkGeoCloudMapsOverlay uses the ThinkGeo Cloud XYZ Tile Server as raster map tile server. It supports 5 different map styles:
- Light
- Dark
- Aerial
- Hybrid
- TransparentBackground

ThinkGeo Cloud Maps support would work in all of the Map Suite controls such as Wpf, WinForms, Web, MVC, WebApi and Android.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_mobile_for_ios) for the details.

![Screenshot](Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
MapView = new MapView(View.Frame);
MapView.MapUnit = GeographyUnit.Meter;
MapView.CurrentExtent = new RectangleShape(-20037508.2314698, 20037508.2314698, 20037508.2314698, -20037508.2314698);
MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
View.AddSubview(MapView);

/*===========================================
  Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
  a Client ID and Secret. These were sent to you via email when you signed up
  with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/
worldOverlay = new ThinkGeoCloudRasterMapsOverlay();
// Tiles will be cached in the MyDocuments folder (Such as %APPPATH%/Documents/) by default if the TileCache property is not set.
worldOverlay.TileCache = new XyzFileBitmapTileCache(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ThinkGeoCloudMapsTileCache"));
MapView.Overlays.Add(worldOverlay);
MapView.Refresh();
```
### Getting Help

[Map Suite Mobile for iOS Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_mobile_for_ios)

[Map Suite Mobile for iOS Product Description](https://thinkgeo.com/ui-controls#ios-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

Working...


### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
