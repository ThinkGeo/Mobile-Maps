# ThinkGeoCloudVectorMapsOverlay


## Inheritance Hierarchy

+ `Object`
  + [`Overlay`](ThinkGeo.UI.iOS.Overlay.md)
    + [`TileOverlay`](ThinkGeo.UI.iOS.TileOverlay.md)
      + **`ThinkGeoCloudVectorMapsOverlay`**

## Members Summary

### Public Constructors Summary


|Name|
|---|
|[`ThinkGeoCloudVectorMapsOverlay()`](#thinkgeocloudvectormapsoverlay)|
|[`ThinkGeoCloudVectorMapsOverlay(String,String)`](#thinkgeocloudvectormapsoverlaystringstring)|
|[`ThinkGeoCloudVectorMapsOverlay(String,String,ThinkGeoCloudVectorMapsMapType)`](#thinkgeocloudvectormapsoverlaystringstringthinkgeocloudvectormapsmaptype)|
|[`ThinkGeoCloudVectorMapsOverlay(String,String,Uri)`](#thinkgeocloudvectormapsoverlaystringstringuri)|

### Protected Constructors Summary


|Name|
|---|
|N/A|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`Attribution`](#attribution)|`String`|N/A|
|[`AutoRefreshInterval`](#autorefreshinterval)|`TimeSpan`|N/A|
|[`CanRefreshRegion`](#canrefreshregion)|`Boolean`|N/A|
|[`ClientId`](#clientid)|`String`|Gets or sets the client identifier.|
|[`ClientSecret`](#clientsecret)|`String`|Gets or sets the client secret.|
|[`DrawingCanvas`](#drawingcanvas)|[`MapDrawingCanvas`](ThinkGeo.UI.iOS.MapDrawingCanvas.md)|Gets or sets the drawing canvas.|
|[`DrawingExceptionMode`](#drawingexceptionmode)|[`DrawingExceptionMode`](../ThinkGeo.Core/ThinkGeo.Core.DrawingExceptionMode.md)|N/A|
|[`DrawingQuality`](#drawingquality)|[`DrawingQuality`](../ThinkGeo.Core/ThinkGeo.Core.DrawingQuality.md)|N/A|
|[`IsCacheOnly`](#iscacheonly)|`Boolean`|N/A|
|[`IsEmpty`](#isempty)|`Boolean`|N/A|
|[`IsVisible`](#isvisible)|`Boolean`|N/A|
|[`MapArguments`](#maparguments)|[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)|N/A|
|[`MapType`](#maptype)|[`ThinkGeoCloudVectorMapsMapType`](../ThinkGeo.Core/ThinkGeo.Core.ThinkGeoCloudVectorMapsMapType.md)|Gets or sets the type of the map.|
|[`MaxExtent`](#maxextent)|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|
|[`Name`](#name)|`String`|N/A|
|[`Opacity`](#opacity)|`Double`|N/A|
|[`OverlayView`](#overlayview)|`UIView`|N/A|
|[`StyleJsonUri`](#stylejsonuri)|`Uri`|Gets or sets the style json URI.|
|[`TileCache`](#tilecache)|[`RasterTileCache`](../ThinkGeo.Core/ThinkGeo.Core.RasterTileCache.md)|Gets or sets the tile cache.|
|[`TileHeight`](#tileheight)|`Int32`|N/A|
|[`TileSnappingMode`](#tilesnappingmode)|[`TileSnappingMode`](ThinkGeo.UI.iOS.TileSnappingMode.md)|N/A|
|[`TileType`](#tiletype)|[`TileType`](ThinkGeo.UI.iOS.TileType.md)|N/A|
|[`TileWidth`](#tilewidth)|`Int32`|N/A|
|[`TimeoutInSeconds`](#timeoutinseconds)|`Int32`|Gets or sets the timeout in seconds.|
|[`TransitionEffect`](#transitioneffect)|[`TransitionEffect`](ThinkGeo.UI.iOS.TransitionEffect.md)|N/A|
|[`VectorTileCache`](#vectortilecache)|[`FileVectorTileCache`](../ThinkGeo.Core/ThinkGeo.Core.FileVectorTileCache.md)|Gets or sets the vector tile cache.|
|[`WebProxy`](#webproxy)|`WebProxy`|Gets or sets the web proxy.|

### Protected Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`CacheMark`](#cachemark)|`String`|N/A|
|[`DrawingView`](#drawingview)|`UIView`|N/A|
|[`SingleTile`](#singletile)|[`TileView`](ThinkGeo.UI.iOS.TileView.md)|N/A|
|[`StretchView`](#stretchview)|`UIView`|N/A|

### Public Methods Summary


|Name|
|---|
|[`Close()`](#close)|
|[`Dispose()`](#dispose)|
|[`Draw(MapArguments,OverlayRefreshType)`](#drawmapargumentsoverlayrefreshtype)|
|[`Equals(Object)`](#equalsobject)|
|[`GetBoundingBox()`](#getboundingbox)|
|[`GetHashCode()`](#gethashcode)|
|[`GetOneTile()`](#getonetile)|
|[`GetType()`](#gettype)|
|[`Initialize(MapArguments)`](#initializemaparguments)|
|[`PostTransform(TransformArguments,MapArguments)`](#posttransformtransformargumentsmaparguments)|
|[`Refresh()`](#refresh)|
|[`Refresh(RectangleShape)`](#refreshrectangleshape)|
|[`Refresh(IEnumerable<RectangleShape>)`](#refreshienumerable<rectangleshape>)|
|[`Refresh(TimeSpan)`](#refreshtimespan)|
|[`Refresh(TimeSpan,RequestDrawingBufferTimeType)`](#refreshtimespanrequestdrawingbuffertimetype)|
|[`Refresh(RectangleShape,TimeSpan)`](#refreshrectangleshapetimespan)|
|[`Refresh(RectangleShape,TimeSpan,RequestDrawingBufferTimeType)`](#refreshrectangleshapetimespanrequestdrawingbuffertimetype)|
|[`Refresh(IEnumerable<RectangleShape>,TimeSpan)`](#refreshienumerable<rectangleshape>timespan)|
|[`Refresh(IEnumerable<RectangleShape>,TimeSpan,RequestDrawingBufferTimeType)`](#refreshienumerable<rectangleshape>timespanrequestdrawingbuffertimetype)|
|[`RemoveAllAnimations()`](#removeallanimations)|
|[`ToString()`](#tostring)|

### Protected Methods Summary


|Name|
|---|
|[`CloseCore()`](#closecore)|
|[`Dispose(Boolean)`](#disposeboolean)|
|[`DrawCore(MapArguments,OverlayRefreshType)`](#drawcoremapargumentsoverlayrefreshtype)|
|[`DrawException(GeoCanvas,Exception)`](#drawexceptiongeocanvasexception)|
|[`DrawExceptionCore(GeoCanvas,Exception)`](#drawexceptioncoregeocanvasexception)|
|[`DrawTile(TileView,MapArguments)`](#drawtiletileviewmaparguments)|
|[`DrawTileCore(GeoCanvas,TileView)`](#drawtilecoregeocanvastileview)|
|[`Finalize()`](#finalize)|
|[`GetBoundingBoxCore()`](#getboundingboxcore)|
|[`GetTile()`](#gettile)|
|[`GetTileCore()`](#gettilecore)|
|[`GetTileMatrix(Double,Int32,Int32,GeographyUnit)`](#gettilematrixdoubleint32int32geographyunit)|
|[`GetTileMatrixCells(GeographyUnit,RectangleShape,Double)`](#gettilematrixcellsgeographyunitrectangleshapedouble)|
|[`InitializeCore(MapArguments)`](#initializecoremaparguments)|
|[`MemberwiseClone()`](#memberwiseclone)|
|[`OnDrawingException(DrawingExceptionOverlayEventArgs)`](#ondrawingexceptiondrawingexceptionoverlayeventargs)|
|[`OnDrawnException(DrawnExceptionOverlayEventArgs)`](#ondrawnexceptiondrawnexceptionoverlayeventargs)|
|[`OnSendingWebRequest(SendingWebRequestEventArgs)`](#onsendingwebrequestsendingwebrequesteventargs)|
|[`OnSentWebRequest(SentWebRequestEventArgs)`](#onsentwebrequestsentwebrequesteventargs)|
|[`PostTransformCore(TransformArguments,MapArguments)`](#posttransformcoretransformargumentsmaparguments)|
|[`PrepareInertialPan(RectangleShape,RectangleShape,MapArguments)`](#prepareinertialpanrectangleshaperectangleshapemaparguments)|
|[`PrepareInertialPanInternal(RectangleShape,RectangleShape,MapArguments)`](#prepareinertialpaninternalrectangleshaperectangleshapemaparguments)|
|[`RefreshCore()`](#refreshcore)|
|[`RefreshCore(RectangleShape)`](#refreshcorerectangleshape)|
|[`RemoveAllAnimationCore()`](#removeallanimationcore)|

### Public Events Summary


|Name|Event Arguments|Description|
|---|---|---|
|[`SendingWebRequest`](#sendingwebrequest)|[`SendingWebRequestEventArgs`](../ThinkGeo.Core/ThinkGeo.Core.SendingWebRequestEventArgs.md)|N/A|
|[`SentWebRequest`](#sentwebrequest)|[`SentWebRequestEventArgs`](../ThinkGeo.Core/ThinkGeo.Core.SentWebRequestEventArgs.md)|N/A|
|[`DrawingException`](#drawingexception)|[`DrawingExceptionOverlayEventArgs`](ThinkGeo.UI.iOS.DrawingExceptionOverlayEventArgs.md)|N/A|
|[`DrawnException`](#drawnexception)|[`DrawnExceptionOverlayEventArgs`](ThinkGeo.UI.iOS.DrawnExceptionOverlayEventArgs.md)|N/A|

## Members Detail

### Public Constructors


|Name|
|---|
|[`ThinkGeoCloudVectorMapsOverlay()`](#thinkgeocloudvectormapsoverlay)|
|[`ThinkGeoCloudVectorMapsOverlay(String,String)`](#thinkgeocloudvectormapsoverlaystringstring)|
|[`ThinkGeoCloudVectorMapsOverlay(String,String,ThinkGeoCloudVectorMapsMapType)`](#thinkgeocloudvectormapsoverlaystringstringthinkgeocloudvectormapsmaptype)|
|[`ThinkGeoCloudVectorMapsOverlay(String,String,Uri)`](#thinkgeocloudvectormapsoverlaystringstringuri)|

### Protected Constructors


### Public Properties

#### `Attribution`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `AutoRefreshInterval`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`TimeSpan`

---
#### `CanRefreshRegion`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `ClientId`

**Summary**

   *Gets or sets the client identifier.*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `ClientSecret`

**Summary**

   *Gets or sets the client secret.*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `DrawingCanvas`

**Summary**

   *Gets or sets the drawing canvas.*

**Remarks**

   *N/A*

**Return Value**

[`MapDrawingCanvas`](ThinkGeo.UI.iOS.MapDrawingCanvas.md)

---
#### `DrawingExceptionMode`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`DrawingExceptionMode`](../ThinkGeo.Core/ThinkGeo.Core.DrawingExceptionMode.md)

---
#### `DrawingQuality`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`DrawingQuality`](../ThinkGeo.Core/ThinkGeo.Core.DrawingQuality.md)

---
#### `IsCacheOnly`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `IsEmpty`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `IsVisible`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `MapArguments`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)

---
#### `MapType`

**Summary**

   *Gets or sets the type of the map.*

**Remarks**

   *N/A*

**Return Value**

[`ThinkGeoCloudVectorMapsMapType`](../ThinkGeo.Core/ThinkGeo.Core.ThinkGeoCloudVectorMapsMapType.md)

---
#### `MaxExtent`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)

---
#### `Name`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `Opacity`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Double`

---
#### `OverlayView`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIView`

---
#### `StyleJsonUri`

**Summary**

   *Gets or sets the style json URI.*

**Remarks**

   *N/A*

**Return Value**

`Uri`

---
#### `TileCache`

**Summary**

   *Gets or sets the tile cache.*

**Remarks**

   *N/A*

**Return Value**

[`RasterTileCache`](../ThinkGeo.Core/ThinkGeo.Core.RasterTileCache.md)

---
#### `TileHeight`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Int32`

---
#### `TileSnappingMode`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`TileSnappingMode`](ThinkGeo.UI.iOS.TileSnappingMode.md)

---
#### `TileType`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`TileType`](ThinkGeo.UI.iOS.TileType.md)

---
#### `TileWidth`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Int32`

---
#### `TimeoutInSeconds`

**Summary**

   *Gets or sets the timeout in seconds.*

**Remarks**

   *N/A*

**Return Value**

`Int32`

---
#### `TransitionEffect`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`TransitionEffect`](ThinkGeo.UI.iOS.TransitionEffect.md)

---
#### `VectorTileCache`

**Summary**

   *Gets or sets the vector tile cache.*

**Remarks**

   *N/A*

**Return Value**

[`FileVectorTileCache`](../ThinkGeo.Core/ThinkGeo.Core.FileVectorTileCache.md)

---
#### `WebProxy`

**Summary**

   *Gets or sets the web proxy.*

**Remarks**

   *N/A*

**Return Value**

`WebProxy`

---

### Protected Properties

#### `CacheMark`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `DrawingView`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIView`

---
#### `SingleTile`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`TileView`](ThinkGeo.UI.iOS.TileView.md)

---
#### `StretchView`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIView`

---

### Public Methods

#### `Close()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `Dispose()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `Draw(MapArguments,OverlayRefreshType)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|mapArguments|[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)|N/A|
|refreshType|[`OverlayRefreshType`](ThinkGeo.UI.iOS.OverlayRefreshType.md)|N/A|

---
#### `Equals(Object)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Boolean`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|obj|`Object`|N/A|

---
#### `GetBoundingBox()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `GetHashCode()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Int32`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `GetOneTile()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`TileView`](ThinkGeo.UI.iOS.TileView.md)|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `GetType()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Type`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `Initialize(MapArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|mapArguments|[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)|N/A|

---
#### `PostTransform(TransformArguments,MapArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|transformInfo|[`TransformArguments`](ThinkGeo.UI.iOS.TransformArguments.md)|N/A|
|mapArguments|[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)|N/A|

---
#### `Refresh()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `Refresh(RectangleShape)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|extent|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|

---
#### `Refresh(IEnumerable<RectangleShape>)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|extentsToRefresh|IEnumerable<[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)>|N/A|

---
#### `Refresh(TimeSpan)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|bufferTime|`TimeSpan`|N/A|

---
#### `Refresh(TimeSpan,RequestDrawingBufferTimeType)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|bufferTime|`TimeSpan`|N/A|
|bufferTimeType|[`RequestDrawingBufferTimeType`](../ThinkGeo.Core/ThinkGeo.Core.RequestDrawingBufferTimeType.md)|N/A|

---
#### `Refresh(RectangleShape,TimeSpan)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|extentToRefresh|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|
|bufferTime|`TimeSpan`|N/A|

---
#### `Refresh(RectangleShape,TimeSpan,RequestDrawingBufferTimeType)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|extentToRefresh|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|
|bufferTime|`TimeSpan`|N/A|
|bufferTimeType|[`RequestDrawingBufferTimeType`](../ThinkGeo.Core/ThinkGeo.Core.RequestDrawingBufferTimeType.md)|N/A|

---
#### `Refresh(IEnumerable<RectangleShape>,TimeSpan)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|extentsToRefresh|IEnumerable<[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)>|N/A|
|bufferTime|`TimeSpan`|N/A|

---
#### `Refresh(IEnumerable<RectangleShape>,TimeSpan,RequestDrawingBufferTimeType)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|extentsToRefresh|IEnumerable<[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)>|N/A|
|bufferTime|`TimeSpan`|N/A|
|bufferTimeType|[`RequestDrawingBufferTimeType`](../ThinkGeo.Core/ThinkGeo.Core.RequestDrawingBufferTimeType.md)|N/A|

---
#### `RemoveAllAnimations()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `ToString()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`String`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---

### Protected Methods

#### `CloseCore()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `Dispose(Boolean)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|disposing|`Boolean`|N/A|

---
#### `DrawCore(MapArguments,OverlayRefreshType)`

**Summary**

   *This method draws the overlay by the specified arguments.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|mapArguments|[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)|This parameter indicates an mapArguments to draw the overlay.|
|refreshType|[`OverlayRefreshType`](ThinkGeo.UI.iOS.OverlayRefreshType.md)|This parameter indicates whether this overlay needs to be refresh or not.|

---
#### `DrawException(GeoCanvas,Exception)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|geoCanvas|[`GeoCanvas`](../ThinkGeo.Core/ThinkGeo.Core.GeoCanvas.md)|N/A|
|exception|`Exception`|N/A|

---
#### `DrawExceptionCore(GeoCanvas,Exception)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|geoCanvas|[`GeoCanvas`](../ThinkGeo.Core/ThinkGeo.Core.GeoCanvas.md)|N/A|
|ex|`Exception`|N/A|

---
#### `DrawTile(TileView,MapArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|tile|[`TileView`](ThinkGeo.UI.iOS.TileView.md)|N/A|
|mapArguments|[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)|N/A|

---
#### `DrawTileCore(GeoCanvas,TileView)`

**Summary**

   *Draws the tile core.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|geoCanvas|[`GeoCanvas`](../ThinkGeo.Core/ThinkGeo.Core.GeoCanvas.md)|The geo canvas.|
|tile|[`TileView`](ThinkGeo.UI.iOS.TileView.md)|The tile.|

---
#### `Finalize()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `GetBoundingBoxCore()`

**Summary**

   *Gets the bounding box core.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|RectangleShape.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `GetTile()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`TileView`](ThinkGeo.UI.iOS.TileView.md)|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `GetTileCore()`

**Summary**

   *Gets the tile core.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`TileView`](ThinkGeo.UI.iOS.TileView.md)||

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `GetTileMatrix(Double,Int32,Int32,GeographyUnit)`

**Summary**

   *Gets the tile matrix.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`TileMatrix`](../ThinkGeo.Core/ThinkGeo.Core.TileMatrix.md)||

**Parameters**

|Name|Type|Description|
|---|---|---|
|scale|`Double`|The scale.|
|tileWidth|`Int32`|Width of the tile.|
|tileHeight|`Int32`|Height of the tile.|
|boundingBoxUnit|[`GeographyUnit`](../ThinkGeo.Core/ThinkGeo.Core.GeographyUnit.md)|The bounding box unit.|

---
#### `GetTileMatrixCells(GeographyUnit,RectangleShape,Double)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|Collection<[`MatrixCell`](../ThinkGeo.Core/ThinkGeo.Core.MatrixCell.md)>|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|mapUnit|[`GeographyUnit`](../ThinkGeo.Core/ThinkGeo.Core.GeographyUnit.md)|N/A|
|targetExtent|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|
|targetScale|`Double`|N/A|

---
#### `InitializeCore(MapArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|mapArgument|[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)|N/A|

---
#### `MemberwiseClone()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Object`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `OnDrawingException(DrawingExceptionOverlayEventArgs)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|args|[`DrawingExceptionOverlayEventArgs`](ThinkGeo.UI.iOS.DrawingExceptionOverlayEventArgs.md)|N/A|

---
#### `OnDrawnException(DrawnExceptionOverlayEventArgs)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|args|[`DrawnExceptionOverlayEventArgs`](ThinkGeo.UI.iOS.DrawnExceptionOverlayEventArgs.md)|N/A|

---
#### `OnSendingWebRequest(SendingWebRequestEventArgs)`

**Summary**

   *Raises the  event.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`SendingWebRequestEventArgs`](../ThinkGeo.Core/ThinkGeo.Core.SendingWebRequestEventArgs.md)|The  instance containing the event data.|

---
#### `OnSentWebRequest(SentWebRequestEventArgs)`

**Summary**

   *Raises the  event.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`SentWebRequestEventArgs`](../ThinkGeo.Core/ThinkGeo.Core.SentWebRequestEventArgs.md)|The  instance containing the event data.|

---
#### `PostTransformCore(TransformArguments,MapArguments)`

**Summary**

   *Posts the transform for the overlay.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|transformInfo|[`TransformArguments`](ThinkGeo.UI.iOS.TransformArguments.md)|The transform information.|
|mapArguments|[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)|The map arguments.|

---
#### `PrepareInertialPan(RectangleShape,RectangleShape,MapArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|currentExtent|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|
|velocityExtent|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|
|mapArguments|[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)|N/A|

---
#### `PrepareInertialPanInternal(RectangleShape,RectangleShape,MapArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|currentExtent|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|
|velocityExtent|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|
|mapArguments|[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)|N/A|

---
#### `RefreshCore()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `RefreshCore(RectangleShape)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|extent|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|N/A|

---
#### `RemoveAllAnimationCore()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---

### Public Events

#### SendingWebRequest

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`SendingWebRequestEventArgs`](../ThinkGeo.Core/ThinkGeo.Core.SendingWebRequestEventArgs.md)

#### SentWebRequest

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`SentWebRequestEventArgs`](../ThinkGeo.Core/ThinkGeo.Core.SentWebRequestEventArgs.md)

#### DrawingException

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`DrawingExceptionOverlayEventArgs`](ThinkGeo.UI.iOS.DrawingExceptionOverlayEventArgs.md)

#### DrawnException

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`DrawnExceptionOverlayEventArgs`](ThinkGeo.UI.iOS.DrawnExceptionOverlayEventArgs.md)


