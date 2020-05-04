
# Supported Data Formats

All editions of Map Suite come with support for the most popular GIS file formats baked right in; these are the **natively-supported formats**.  In general, a natively-supported format will enjoy better performance and API friendliness than those that are supposed through extensions.

## Vector Formats

Vector data allows you to style and display your data exactly how you want it.  If the data format you are looking for is not listed below you can easily [integrate your own custom data](extensibility-guide.md) format.

|Format|Support Type|Map Suite Version|
|---|---|---|
| [ShapeFile](http://en.wikipedia.org/wiki/Shapefile)((All versions of Map Suite support ShapeFile DBFs in the standard dBASE III format.  Additionally, since Map Suite 8.0, DBFs in Visual FoxPro format are also supported.))   | Read/write   | 9.0   |
| [MapInfo TAB](http://en.wikipedia.org/wiki/MapInfo_TAB_format)   | Read/write   | 9.0   |
| ThinkGeo [TinyGeo](http://thinkgeo.com/forums/MapSuite/tabid/143/aft/9903/Default.aspx)   | Read/write   | 9.0   |
| [GPX](http://en.wikipedia.org/wiki/GPS_Exchange_Format) (GPs eXchange format)   | Read   | 9.0   |
| ESRI [Grid](http://en.wikipedia.org/wiki/Esri_grid) (*.grd*)   | Read   | 9.0   |

## Raster Formats

Raster data is typically aerial imagery, satellite imagery and topo maps.  However you can use almost any image as a raster layer and add it to the map.  For example you may have a jpeg that represents a floor plan of a building that you would like to display on a map.

|Format|Support Type|Map Suite Version|
|---|---|---|
| GeoTIFF   | Read   | 9.0   |
| BMP   | Read/Write   | 9.0   |
| PNG   | Read/Write   | 9.0   |
| GIF   | Read/Write   | 9.0   |
| JPG   | Read/Write   | 9.0   |
| TIFF   | Read/Write   | 9.0   |

## Web Based Datasets

Web based map data comes in a variety of forms.  Several of the providers below are tile based web services that allow you to easily integrate rich basemaps into your application.  Other technologies such as WMS and WFS provide a common way for web applications to consume maps and feature information over the web.  Finally the NOAA web data layers allow you to integrate near real time weather information into your application using Map Suite.

|Format/Provider|Map Suite Version|Description|
|---|---|---|
| [World Map Kit Online](http://thinkgeo.com/servers/world-map-kit-online/)   | 9.0   | ThinkGeo's basemap based on Open Street Map Data that is displayed in most of the sample applications.  You can check out what the maps look like at <http://maps.thinkgeo.com>   |
| [WMS (Web Map Service)](http://www.opengeospatial.org/standards/wms)   | 9.0   | WMS is a open standard set by the OGC to consume maps over the web.   |
| [WFS (Web Feature Service)](http://www.opengeospatial.org/standards/wfs)   | 9.0   | WFS is a open standard set by the OGC.   |
| Google Maps ((*Use of Google Maps within a Map Suite developer SDK requires a Google Maps API For Business license and is subject to Google's licensing terms. For more information, please visit: <https://developers.google.com/maps/licensing>   | 9.0   | Google Maps   |
| Bing Maps ((*Use of Bing Maps within a Map Suite developer SDK requires a Bing Maps license and is subject to Microsoft's licensing terms. For more information, please visit: <http://www.microsoft.com/maps/Licensing/licensing.aspx>   | 9.0   | Bing Maps   |
| NOAA Radar   | 9.0   | NEXRAD Radar updated every 15 minutes covering most of the United States.   |
| NOAA Weather Watches & Warnings   | 9.0   | Current weather watches and warnings covering the United States.   |
| NOAA Weather Stations   | 9.0   | Current temperatures and other weather information from NOAA weather stations around the United States.   |
