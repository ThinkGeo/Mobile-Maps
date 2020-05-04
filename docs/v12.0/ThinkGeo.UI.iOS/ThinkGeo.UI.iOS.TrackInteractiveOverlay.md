# TrackInteractiveOverlay


## Inheritance Hierarchy

+ `Object`
  + [`Overlay`](ThinkGeo.UI.iOS.Overlay.md)
    + [`InteractiveOverlay`](ThinkGeo.UI.iOS.InteractiveOverlay.md)
      + **`TrackInteractiveOverlay`**

## Members Summary

### Public Constructors Summary


|Name|
|---|
|[`TrackInteractiveOverlay()`](#trackinteractiveoverlay)|

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
|[`DrawingExceptionMode`](#drawingexceptionmode)|[`DrawingExceptionMode`](../ThinkGeo.Core/ThinkGeo.Core.DrawingExceptionMode.md)|N/A|
|[`DrawingQuality`](#drawingquality)|[`DrawingQuality`](../ThinkGeo.Core/ThinkGeo.Core.DrawingQuality.md)|N/A|
|[`InteractiveView`](#interactiveview)|[`TileView`](ThinkGeo.UI.iOS.TileView.md)|N/A|
|[`IsEmpty`](#isempty)|`Boolean`|This property override the logic in its base class by watching the feature count in trackShapeLayer. If it is empty ,we can skip drawing it for better performance.|
|[`IsInTracking`](#isintracking)|`Boolean`|This property gets or sets to sign that if there is any shape being tracking.|
|[`IsVisible`](#isvisible)|`Boolean`|N/A|
|[`MapArguments`](#maparguments)|[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)|N/A|
|[`Name`](#name)|`String`|N/A|
|[`Opacity`](#opacity)|`Double`|N/A|
|[`OverlayView`](#overlayview)|`UIView`|N/A|
|[`TrackMode`](#trackmode)|[`TrackMode`](ThinkGeo.UI.iOS.TrackMode.md)|Gets a mode of TrackOverlay.|
|[`TrackShapeLayer`](#trackshapelayer)|[`InMemoryFeatureLayer`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureLayer.md)|This property gets the TrackShape layers which holds the track shapes.|

### Protected Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`Vertices`](#vertices)|Collection<[`Vertex`](../ThinkGeo.Core/ThinkGeo.Core.Vertex.md)>|This property gets the vertices to make up the track shape. This is a protected property which probablly need to be used in its sub classes.|

### Public Methods Summary


|Name|
|---|
|[`CancelTracking()`](#canceltracking)|
|[`Close()`](#close)|
|[`Dispose()`](#dispose)|
|[`DoubleTap(InteractionArguments)`](#doubletapinteractionarguments)|
|[`Draw(MapArguments,OverlayRefreshType)`](#drawmapargumentsoverlayrefreshtype)|
|[`Equals(Object)`](#equalsobject)|
|[`GetBoundingBox()`](#getboundingbox)|
|[`GetHashCode()`](#gethashcode)|
|[`GetTrackingShape()`](#gettrackingshape)|
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
|[`EndTracking()`](#endtracking)|
|[`Finalize()`](#finalize)|
|[`GetBoundingBoxCore()`](#getboundingboxcore)|
|[`GetTrackingShapeCore()`](#gettrackingshapecore)|
|[`InitializeCore(MapArguments)`](#initializecoremaparguments)|
|[`LongPressCore(InteractionArguments)`](#longpresscoreinteractionarguments)|
|[`MemberwiseClone()`](#memberwiseclone)|
|[`OnDrawingException(DrawingExceptionOverlayEventArgs)`](#ondrawingexceptiondrawingexceptionoverlayeventargs)|
|[`OnDrawnException(DrawnExceptionOverlayEventArgs)`](#ondrawnexceptiondrawnexceptionoverlayeventargs)|
|[`OnTouchMoved(TouchMovedTrackInteractiveOverlayEventArgs)`](#ontouchmovedtouchmovedtrackinteractiveoverlayeventargs)|
|[`OnTrackEnded(TrackEndedTrackInteractiveOverlayEventArgs)`](#ontrackendedtrackendedtrackinteractiveoverlayeventargs)|
|[`OnTrackEnding(TrackEndingTrackInteractiveOverlayEventArgs)`](#ontrackendingtrackendingtrackinteractiveoverlayeventargs)|
|[`OnTrackStarted(TrackStartedTrackInteractiveOverlayEventArgs)`](#ontrackstartedtrackstartedtrackinteractiveoverlayeventargs)|
|[`OnTrackStarting(TrackStartingTrackInteractiveOverlayEventArgs)`](#ontrackstartingtrackstartingtrackinteractiveoverlayeventargs)|
|[`OnVertexAdded(VertexAddedTrackInteractiveOverlayEventArgs)`](#onvertexaddedvertexaddedtrackinteractiveoverlayeventargs)|
|[`OnVertexAdding(VertexAddingTrackInteractiveOverlayEventArgs)`](#onvertexaddingvertexaddingtrackinteractiveoverlayeventargs)|
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
|[`TrackEnded`](#trackended)|[`TrackEndedTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TrackEndedTrackInteractiveOverlayEventArgs.md)|N/A|
|[`TrackEnding`](#trackending)|[`TrackEndingTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TrackEndingTrackInteractiveOverlayEventArgs.md)|N/A|
|[`TrackStarted`](#trackstarted)|[`TrackStartedTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TrackStartedTrackInteractiveOverlayEventArgs.md)|N/A|
|[`TrackStarting`](#trackstarting)|[`TrackStartingTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TrackStartingTrackInteractiveOverlayEventArgs.md)|N/A|
|[`VertexAdded`](#vertexadded)|[`VertexAddedTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexAddedTrackInteractiveOverlayEventArgs.md)|N/A|
|[`VertexAdding`](#vertexadding)|[`VertexAddingTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexAddingTrackInteractiveOverlayEventArgs.md)|N/A|
|[`TouchMoved`](#touchmoved)|[`TouchMovedTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TouchMovedTrackInteractiveOverlayEventArgs.md)|N/A|
|[`DrawingException`](#drawingexception)|[`DrawingExceptionOverlayEventArgs`](ThinkGeo.UI.iOS.DrawingExceptionOverlayEventArgs.md)|N/A|
|[`DrawnException`](#drawnexception)|[`DrawnExceptionOverlayEventArgs`](ThinkGeo.UI.iOS.DrawnExceptionOverlayEventArgs.md)|N/A|

## Members Detail

### Public Constructors


|Name|
|---|
|[`TrackInteractiveOverlay()`](#trackinteractiveoverlay)|

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
#### `IsEmpty`

**Summary**

   *This property override the logic in its base class by watching the feature count in trackShapeLayer. If it is empty ,we can skip drawing it for better performance.*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `IsInTracking`

**Summary**

   *This property gets or sets to sign that if there is any shape being tracking.*

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
#### `TrackMode`

**Summary**

   *Gets a mode of TrackOverlay.*

**Remarks**

   *The default mode is TrackMode.None which means you cannot draw or edit features at client. By setting the mode to TrackMode.Point, TrackMode.Line, TrackMode.Polygon etc., you could add point, line or polygon to the FeatureOverlay. Setting the mode to TrackMode.Edit, you could edit the shapes at the client side.*

**Return Value**

[`TrackMode`](ThinkGeo.UI.iOS.TrackMode.md)

---
#### `TrackShapeLayer`

**Summary**

   *This property gets the TrackShape layers which holds the track shapes.*

**Remarks**

   *N/A*

**Return Value**

[`InMemoryFeatureLayer`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureLayer.md)

---

### Protected Properties

#### `Vertices`

**Summary**

   *This property gets the vertices to make up the track shape. This is a protected property which probablly need to be used in its sub classes.*

**Remarks**

   *N/A*

**Return Value**

Collection<[`Vertex`](../ThinkGeo.Core/ThinkGeo.Core.Vertex.md)>

---

### Public Methods

#### `CancelTracking()`

**Summary**

   *Cancels the tracking.*

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
#### `GetTrackingShape()`

**Summary**

   *This method gets the current Tracking shape.*

**Remarks**

   *This method is the concrete wrapper for the abstract method GetTrackingShapeCore. This method draws the representation of the overlay based on the extent you provided.*

**Return Value**

|Type|Description|
|---|---|
|[`BaseShape`](../ThinkGeo.Core/ThinkGeo.Core.BaseShape.md)|Returns a shape represents the current status of tracking shape.|

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

   *Releases unmanaged and - optionally - managed resources.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|disposing|`Boolean`|true to release both managed and unmanaged resources; false to release only unmanaged resources.|

---
#### `DoubleTapCore(InteractionArguments)`

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
#### `DrawCore(MapArguments,OverlayRefreshType)`

**Summary**

   *This method draws the TrackInterativeOverlay.*

**Remarks**

   *This method draws the representation of the overlay based on the extent you provided. When implementing this abstract method, consider each feature and its column data values. You can use the full power of the GeoCanvas to do the drawing. If you need column data for a feature, be sure to override the GetRequiredColumnNamesCore and add the columns you need to the collection. In many of the styles, we add properties that allow the user to specify which field they need; then, in the GetRequiredColumnNamesCore, we read that property and add it to the collection.*

**Return Value**

|Type|Description|
|---|---|
|`Void`|None|

**Parameters**

|Name|Type|Description|
|---|---|---|
|mapArguments|[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)|This parameter is current mapArguments.|
|refreshType|[`OverlayRefreshType`](ThinkGeo.UI.iOS.OverlayRefreshType.md)|This parameter is overlay refresh type.|

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
#### `EndTracking()`

**Summary**

   *This method ends the tracking shape by initialize some variables.*

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
#### `GetTrackingShapeCore()`

**Summary**

   *This is the Core method of GetTrackingShape.You could overrides this method to have your own logic. This method gets the current Tracking shape.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`BaseShape`](../ThinkGeo.Core/ThinkGeo.Core.BaseShape.md)|Returns a shape represents the current status of tracking shape.|

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

   *This overrides the MouseDoubleClick logic in its base class InterativeOverlay.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|Interaction results of this method.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|interactionArguments|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|This parameter is the interaction auguments for the method.|

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
#### `OnTouchMoved(TouchMovedTrackInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired when mouse moved a vertex to the Tracking shape.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`TouchMovedTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TouchMovedTrackInteractiveOverlayEventArgs.md)|The MouseMovedTrackInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnTrackEnded(TrackEndedTrackInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired after the end of Tracking a shape.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`TrackEndedTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TrackEndedTrackInteractiveOverlayEventArgs.md)|The TrackEndedTrackInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnTrackEnding(TrackEndingTrackInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired before the end of Tracking a shape.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`TrackEndingTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TrackEndingTrackInteractiveOverlayEventArgs.md)|The TrackEndingTrackInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnTrackStarted(TrackStartedTrackInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired after the start of Tracking a shape.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`TrackStartedTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TrackStartedTrackInteractiveOverlayEventArgs.md)|The TrackStartedTrackInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnTrackStarting(TrackStartingTrackInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired before the start of Tracking a shape.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`TrackStartingTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TrackStartingTrackInteractiveOverlayEventArgs.md)|The TrackStartingTrackInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnVertexAdded(VertexAddedTrackInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired after adding a vertex to the Tracking shape.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`VertexAddedTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexAddedTrackInteractiveOverlayEventArgs.md)|The VertexAddedTrackInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnVertexAdding(VertexAddingTrackInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired before adding a vertex to the Tracking shape.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`VertexAddingTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexAddingTrackInteractiveOverlayEventArgs.md)|The VertexAddingTrackInteractiveOverlayEventArgs passed for the event raised.|

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

   *This overrides the MouseClick logic in its base class.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|Interaction results of this method.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|interactionArguments|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|This parameter is the interaction auguments for the method.|

---
#### `TouchDownCore(InteractionArguments)`

**Summary**

   *This overrides the MouseDown logic in its base class InterativeOverlay.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|Interaction results of this method.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|interactionArguments|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|This parameter is the interaction auguments for the method.|

---
#### `TouchMoveCore(InteractionArguments)`

**Summary**

   *This overrides the MouseMove logic in its base class InterativeOverlay.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|Interaction results of this method.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|interactionArguments|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|This parameter is the interaction auguments for the method.|

---
#### `TouchPointerDownCore(InteractionArguments)`

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
#### `TouchUpCore(InteractionArguments)`

**Summary**

   *This overrides the MouseUp logic in its base class InterativeOverlay.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`InteractiveResult`](ThinkGeo.UI.iOS.InteractiveResult.md)|Interaction results of this method.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|interactionArguments|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|This parameter is the interaction auguments for the method.|

---

### Public Events

#### TrackEnded

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`TrackEndedTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TrackEndedTrackInteractiveOverlayEventArgs.md)

#### TrackEnding

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`TrackEndingTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TrackEndingTrackInteractiveOverlayEventArgs.md)

#### TrackStarted

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`TrackStartedTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TrackStartedTrackInteractiveOverlayEventArgs.md)

#### TrackStarting

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`TrackStartingTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TrackStartingTrackInteractiveOverlayEventArgs.md)

#### VertexAdded

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`VertexAddedTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexAddedTrackInteractiveOverlayEventArgs.md)

#### VertexAdding

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`VertexAddingTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexAddingTrackInteractiveOverlayEventArgs.md)

#### TouchMoved

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`TouchMovedTrackInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.TouchMovedTrackInteractiveOverlayEventArgs.md)

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


