# ExtentInteractiveOverlay


## Inheritance Hierarchy

+ `Object`
  + [`Overlay`](ThinkGeo.UI.iOS.Overlay.md)
    + [`InteractiveOverlay`](ThinkGeo.UI.iOS.InteractiveOverlay.md)
      + **`ExtentInteractiveOverlay`**

## Members Summary

### Public Constructors Summary


|Name|
|---|
|[`ExtentInteractiveOverlay()`](#extentinteractiveoverlay)|

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
|[`DoubleTapMode`](#doubletapmode)|[`MapDoubleTapMode`](ThinkGeo.UI.iOS.MapDoubleTapMode.md)|Gets or sets the double tap mode.|
|[`DrawingExceptionMode`](#drawingexceptionmode)|[`DrawingExceptionMode`](../ThinkGeo.Core/ThinkGeo.Core.DrawingExceptionMode.md)|N/A|
|[`DrawingQuality`](#drawingquality)|[`DrawingQuality`](../ThinkGeo.Core/ThinkGeo.Core.DrawingQuality.md)|N/A|
|[`InteractiveView`](#interactiveview)|[`TileView`](ThinkGeo.UI.iOS.TileView.md)|N/A|
|[`IsBusy`](#isbusy)|`Boolean`|Gets a value indicating whether this instance is busy.|
|[`IsEmpty`](#isempty)|`Boolean`|N/A|
|[`IsInertiallyPanEnabled`](#isinertiallypanenabled)|`Boolean`|Gets or sets a value indicating whether this instance is inertially pan enabled.|
|[`IsVisible`](#isvisible)|`Boolean`|N/A|
|[`MapArguments`](#maparguments)|[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)|N/A|
|[`Name`](#name)|`String`|N/A|
|[`Opacity`](#opacity)|`Double`|N/A|
|[`OverlayView`](#overlayview)|`UIView`|N/A|

### Protected Properties Summary

|Name|Return Type|Description|
|---|---|---|
|N/A|N/A|N/A|

### Public Methods Summary


|Name|
|---|
|[`Close()`](#close)|
|[`Dispose()`](#dispose)|
|[`DoubleTap(InteractionArguments)`](#doubletapinteractionarguments)|
|[`Draw(MapArguments,OverlayRefreshType)`](#drawmapargumentsoverlayrefreshtype)|
|[`Equals(Object)`](#equalsobject)|
|[`GetBoundingBox()`](#getboundingbox)|
|[`GetHashCode()`](#gethashcode)|
|[`GetType()`](#gettype)|
|[`Initialize(MapArguments)`](#initializemaparguments)|
|[`LongPress(InteractionArguments)`](#longpressinteractionarguments)|
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
|[`SingleTap(InteractionArguments)`](#singletapinteractionarguments)|
|[`ToString()`](#tostring)|
|[`TouchDown(InteractionArguments)`](#touchdowninteractionarguments)|
|[`TouchMove(InteractionArguments)`](#touchmoveinteractionarguments)|
|[`TouchPointerDown(InteractionArguments)`](#touchpointerdowninteractionarguments)|
|[`TouchUp(InteractionArguments)`](#touchupinteractionarguments)|

### Protected Methods Summary


|Name|
|---|
|[`CloseCore()`](#closecore)|
|[`Dispose(Boolean)`](#disposeboolean)|
|[`DoubleTapCore(InteractionArguments)`](#doubletapcoreinteractionarguments)|
|[`DrawCore(MapArguments,OverlayRefreshType)`](#drawcoremapargumentsoverlayrefreshtype)|
|[`DrawException(GeoCanvas,Exception)`](#drawexceptiongeocanvasexception)|
|[`DrawExceptionCore(GeoCanvas,Exception)`](#drawexceptioncoregeocanvasexception)|
|[`Finalize()`](#finalize)|
|[`GetBoundingBoxCore()`](#getboundingboxcore)|
|[`InitializeCore(MapArguments)`](#initializecoremaparguments)|
|[`LongPressCore(InteractionArguments)`](#longpresscoreinteractionarguments)|
|[`MemberwiseClone()`](#memberwiseclone)|
|[`OnDrawingException(DrawingExceptionOverlayEventArgs)`](#ondrawingexceptiondrawingexceptionoverlayeventargs)|
|[`OnDrawnException(DrawnExceptionOverlayEventArgs)`](#ondrawnexceptiondrawnexceptionoverlayeventargs)|
|[`PostTransformCore(TransformArguments,MapArguments)`](#posttransformcoretransformargumentsmaparguments)|
|[`PrepareInertialPan(RectangleShape,RectangleShape,MapArguments)`](#prepareinertialpanrectangleshaperectangleshapemaparguments)|
|[`PrepareInertialPanInternal(RectangleShape,RectangleShape,MapArguments)`](#prepareinertialpaninternalrectangleshaperectangleshapemaparguments)|
|[`RefreshCore()`](#refreshcore)|
|[`RefreshCore(RectangleShape)`](#refreshcorerectangleshape)|
|[`RemoveAllAnimationCore()`](#removeallanimationcore)|
|[`SingleTapCore(InteractionArguments)`](#singletapcoreinteractionarguments)|
|[`TouchDownCore(InteractionArguments)`](#touchdowncoreinteractionarguments)|
|[`TouchMoveCore(InteractionArguments)`](#touchmovecoreinteractionarguments)|
|[`TouchPointerDownCore(InteractionArguments)`](#touchpointerdowncoreinteractionarguments)|
|[`TouchUpCore(InteractionArguments)`](#touchupcoreinteractionarguments)|

### Public Events Summary


|Name|Event Arguments|Description|
|---|---|---|
|[`DrawingException`](#drawingexception)|[`DrawingExceptionOverlayEventArgs`](ThinkGeo.UI.iOS.DrawingExceptionOverlayEventArgs.md)|N/A|
|[`DrawnException`](#drawnexception)|[`DrawnExceptionOverlayEventArgs`](ThinkGeo.UI.iOS.DrawnExceptionOverlayEventArgs.md)|N/A|

## Members Detail

### Public Constructors


|Name|
|---|
|[`ExtentInteractiveOverlay()`](#extentinteractiveoverlay)|

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
#### `DoubleTapMode`

**Summary**

   *Gets or sets the double tap mode.*

**Remarks**

   *N/A*

**Return Value**

[`MapDoubleTapMode`](ThinkGeo.UI.iOS.MapDoubleTapMode.md)

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
#### `InteractiveView`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`TileView`](ThinkGeo.UI.iOS.TileView.md)

---
#### `IsBusy`

**Summary**

   *Gets a value indicating whether this instance is busy.*

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
#### `IsInertiallyPanEnabled`

**Summary**

   *Gets or sets a value indicating whether this instance is inertially pan enabled.*

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

### Protected Properties


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
#### `DoubleTap(InteractionArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|N/A|

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
#### `LongPress(InteractionArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|N/A|

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
#### `SingleTap(InteractionArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|N/A|

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
#### `TouchDown(InteractionArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|N/A|

---
#### `TouchMove(InteractionArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|N/A|

---
#### `TouchPointerDown(InteractionArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|N/A|

---
#### `TouchUp(InteractionArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|N/A|

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
#### `DoubleTapCore(InteractionArguments)`

**Summary**

   *This overrides the DoubleTap logic in its base class InterativeOverlay.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|Interaction results of this method.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|This parameter is the interaction arguments for the method.|

---
#### `DrawCore(MapArguments,OverlayRefreshType)`

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
#### `LongPressCore(InteractionArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|N/A|

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
#### `PostTransformCore(TransformArguments,MapArguments)`

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
#### `SingleTapCore(InteractionArguments)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|N/A|

---
#### `TouchDownCore(InteractionArguments)`

**Summary**

   *This overrides the TouchDown logic in its base class InterativeOverlay.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|Interaction results of this method.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|This parameter is the interaction arguments for the method.|

---
#### `TouchMoveCore(InteractionArguments)`

**Summary**

   *This overrides the TouchMove logic in its base class InterativeOverlay.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|Interaction results of this method.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|This parameter is the interaction arguments for the method.|

---
#### `TouchPointerDownCore(InteractionArguments)`

**Summary**

   *This overrides the TouchPointerDown logic in its base class InterativeOverlay.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|Interaction results of this method.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|This parameter is the interaction arguments for the method.|

---
#### `TouchUpCore(InteractionArguments)`

**Summary**

   *This overrides the TouchUp logic in its base class InterativeOverlay.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|Interaction results of this method.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|This parameter is the interaction arguments for the method.|

---

### Public Events

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

