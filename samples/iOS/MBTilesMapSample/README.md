# ThinkGeo MBTiles Maps Sample for iOS

### Description

This sample demonstrates how you can draw the map with Vector Tiles saved in *.MBTiles in your Map Suite GIS applications, with any style you want from [StyleJSON (Mapping Defination Grammar)](https://wiki.thinkgeo.com/wiki/thinkgeo_stylejson). It will show you how to use the XyzFileBitmapTileCache to improve the performance of map rendering.


If you want the *.mbtile file of any area in the world, or you have any requirement of building *.mbtile file based on your own data, such as shape file, Oracle, MsSql and more, please contact support@thinkgeo.com.


*.MBTile format can be supported in all of the Map Suite controls such as Wpf, Web, MVC, WebApi, Android and iOS.

Please refer to [Wiki](https://wiki.thinkgeo.com/wiki/map_suite_mobile_for_ios) for the details.

![Screenshot](https://github.com/ThinkGeo/ThinkGeoMBTilesMapsSample-ForiOS/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.5.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
this.mapView.MapUnit = GeographyUnit.Meter;
this.mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

// Create background map for Frisco with MB tile requested from mbtiles Database.  
ThinkGeoMBTilesFeatureLayer thinkGeoMBTilesFeatureLayer = new ThinkGeoMBTilesFeatureLayer("AppData/tiles_Frisco.mbtiles", new Uri("AppData/thinkgeo-world-streets-light.json", UriKind.Relative));

LayerOverlay layerOverlay = new LayerOverlay();
layerOverlay.TileSizeMode = TileSizeMode.DefaultX2;
layerOverlay.MaxExtent = new RectangleShape(-20037508.2314698, 20037508.2314698, 20037508.2314698, -20037508.2314698);
layerOverlay.Layers.Add(thinkGeoMBTilesFeatureLayer);

this.mapView.Overlays.Add(layerOverlay);
this.mapView.CurrentExtent = new RectangleShape(-10780508.5162109, 3916643.16078401, -10775922.2945393, 3914213.89649231);
this.mapView.Refresh();
```

**[Warning]**

If you would like to draw the *.MbTiles with ThinkGeo pre-built StyleJson file, which is created based on a customized icon set made for map designer, the following steps are required, according to the "[Xamarin iOS Development Guide](https://blog.xamarin.com/custom-fonts-in-ios/)".

1. Download "vectorMap-icons" icons font family from [ThinkGeo CDN](https://cdn.thinkgeo.com/vectormap-icons/1.0.0/vectormap-icons.ttf)
2. Add the download "vectorMap-icons" icons font file to your project. Once you've added the font, you should then right click it and select the ‘Properties’ menu option to do following changes:
   * Change ‘Build Action’ to ‘BundleResource’.
   * Change ‘Copy to output directory’ to ‘Always copy’.

3. To tell iOS you are using custom fonts, you should open the Info.plist file and add following parts to "<dict></dict>".
	
```
  <array>
	<string>vectormap-icons.ttf</string>
  </array>
```


### Getting Help

[Map Suite Mobile for Android Wiki Resources](https://wiki.thinkgeo.com/wiki/map_suite_mobile_for_ios)

[Map Suite Mobile for Android Product Description](https://thinkgeo.com/gis-ui-mobile#platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

Working...


### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
