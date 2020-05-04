# EditInteractiveOverlay


## Inheritance Hierarchy

+ `Object`
  + [`Overlay`](ThinkGeo.UI.iOS.Overlay.md)
    + [`InteractiveOverlay`](ThinkGeo.UI.iOS.InteractiveOverlay.md)
      + **`EditInteractiveOverlay`**

## Members Summary

### Public Constructors Summary


|Name|
|---|
|[`EditInteractiveOverlay()`](#editinteractiveoverlay)|

### Protected Constructors Summary


|Name|
|---|
|N/A|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`Attribution`](#attribution)|`String`|N/A|
|[`AutoRefreshInterval`](#autorefreshinterval)|`TimeSpan`|N/A|
|[`CanAddVertex`](#canaddvertex)|`Boolean`|Gets a value which indicates whether the shape can Add new vertex.|
|[`CanDrag`](#candrag)|`Boolean`|Gets a value which indicates whether the shape can be dragged.|
|[`CanRefreshRegion`](#canrefreshregion)|`Boolean`|N/A|
|[`CanRemoveVertex`](#canremovevertex)|`Boolean`|Gets a value which indicates whether the shape can remove a existing vertex.|
|[`CanReshape`](#canreshape)|`Boolean`|Gets a value which indicates whether the shape can be reshaped.|
|[`CanResize`](#canresize)|`Boolean`|Gets a value which indicates whether the shape can be resized.|
|[`CanRotate`](#canrotate)|`Boolean`|Gets a value which indicates whether the shape can be rotated.|
|[`DragControlPointsLayer`](#dragcontrolpointslayer)|[`InMemoryFeatureLayer`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureLayer.md)|This property gets the InMemoryFeatureLayer which holds the control points for drag.|
|[`DrawingExceptionMode`](#drawingexceptionmode)|[`DrawingExceptionMode`](../ThinkGeo.Core/ThinkGeo.Core.DrawingExceptionMode.md)|N/A|
|[`DrawingQuality`](#drawingquality)|[`DrawingQuality`](../ThinkGeo.Core/ThinkGeo.Core.DrawingQuality.md)|N/A|
|[`EditShapesLayer`](#editshapeslayer)|[`InMemoryFeatureLayer`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureLayer.md)|This property gets the InMemoryFeatureLayer which holds the edit shapes.|
|[`ExistingControlPointsLayer`](#existingcontrolpointslayer)|[`InMemoryFeatureLayer`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureLayer.md)|This property gets the InMemoryFeatureLayer which holds the control points which represents the existing vertices of the edit shapes.|
|[`InteractiveView`](#interactiveview)|[`TileView`](ThinkGeo.UI.iOS.TileView.md)|N/A|
|[`IsEmpty`](#isempty)|`Boolean`|This property override its property in base class by watching the feature count in editShapesLayer. If it is empty ,we can skip drawing it for better performance.|
|[`IsVisible`](#isvisible)|`Boolean`|N/A|
|[`MapArguments`](#maparguments)|[`MapArguments`](ThinkGeo.UI.iOS.MapArguments.md)|N/A|
|[`Name`](#name)|`String`|N/A|
|[`Opacity`](#opacity)|`Double`|N/A|
|[`OverlayView`](#overlayview)|`UIView`|N/A|
|[`ResizeControlPointsLayer`](#resizecontrolpointslayer)|[`InMemoryFeatureLayer`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureLayer.md)|This property gets the InMemoryFeatureLayer which holds the control points for resize.|
|[`RotateControlPointsLayer`](#rotatecontrolpointslayer)|[`InMemoryFeatureLayer`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureLayer.md)|This property gets the InMemoryFeatureLayer which holds the control points for rotate.|
|[`SelectedControlPointLayer`](#selectedcontrolpointlayer)|[`InMemoryFeatureLayer`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureLayer.md)|N/A|
|[`TrackMode`](#trackmode)|[`TrackMode`](ThinkGeo.UI.iOS.TrackMode.md)|Gets or sets a mode of TrackOverlay.|

### Protected Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`ControlPointType`](#controlpointtype)|[`ControlPointType`](ThinkGeo.UI.iOS.ControlPointType.md)|This property gets or sets the ControlPointType for the control points of the edit shapes.|
|[`OriginalEditingFeature`](#originaleditingfeature)|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|This property gets the feature represents the original editing feature.|
|[`SelectControlPointFeature`](#selectcontrolpointfeature)|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|This property gets the feature represents the select control points of the edit shapes.|

### Public Methods Summary


|Name|
|---|
|[`CalculateAllControlPoints()`](#calculateallcontrolpoints)|
|[`ClearAllControlPoints()`](#clearallcontrolpoints)|
|[`Close()`](#close)|
|[`DeleteTrackShape()`](#deletetrackshape)|
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
|[`AddVertex(PointShape,Double)`](#addvertexpointshapedouble)|
|[`AddVertexCore(Feature,PointShape,Double)`](#addvertexcorefeaturepointshapedouble)|
|[`CalculateDragControlPoints()`](#calculatedragcontrolpoints)|
|[`CalculateDragControlPointsCore(Feature)`](#calculatedragcontrolpointscorefeature)|
|[`CalculateResizeControlPoints()`](#calculateresizecontrolpoints)|
|[`CalculateResizeControlPointsCore(Feature)`](#calculateresizecontrolpointscorefeature)|
|[`CalculateRotateControlPoints()`](#calculaterotatecontrolpoints)|
|[`CalculateRotateControlPointsCore(Feature)`](#calculaterotatecontrolpointscorefeature)|
|[`CalculateVertexControlPoints()`](#calculatevertexcontrolpoints)|
|[`CalculateVertexControlPointsCore(Feature)`](#calculatevertexcontrolpointscorefeature)|
|[`CloseCore()`](#closecore)|
|[`Dispose(Boolean)`](#disposeboolean)|
|[`DoubleTapCore(InteractionArguments)`](#doubletapcoreinteractionarguments)|
|[`DragFeature(Feature,PointShape,PointShape)`](#dragfeaturefeaturepointshapepointshape)|
|[`DragFeatureCore(Feature,PointShape,PointShape)`](#dragfeaturecorefeaturepointshapepointshape)|
|[`DrawCore(MapArguments,OverlayRefreshType)`](#drawcoremapargumentsoverlayrefreshtype)|
|[`DrawException(GeoCanvas,Exception)`](#drawexceptiongeocanvasexception)|
|[`DrawExceptionCore(GeoCanvas,Exception)`](#drawexceptioncoregeocanvasexception)|
|[`EndEditing(PointShape)`](#endeditingpointshape)|
|[`EndEditingCore(PointShape)`](#endeditingcorepointshape)|
|[`Finalize()`](#finalize)|
|[`GetAllVerticesFromFeature(Feature)`](#getallverticesfromfeaturefeature)|
|[`GetBoundingBoxCore()`](#getboundingboxcore)|
|[`InitializeCore(MapArguments)`](#initializecoremaparguments)|
|[`LongPressCore(InteractionArguments)`](#longpresscoreinteractionarguments)|
|[`MemberwiseClone()`](#memberwiseclone)|
|[`MoveVertex(Feature,PointShape,PointShape)`](#movevertexfeaturepointshapepointshape)|
|[`MoveVertexCore(Feature,PointShape,PointShape)`](#movevertexcorefeaturepointshapepointshape)|
|[`OnControlPointSelected(ControlPointSelectedEditInteractiveOverlayEventArgs)`](#oncontrolpointselectedcontrolpointselectededitinteractiveoverlayeventargs)|
|[`OnControlPointSelecting(ControlPointSelectingEditInteractiveOverlayEventArgs)`](#oncontrolpointselectingcontrolpointselectingeditinteractiveoverlayeventargs)|
|[`OnDrawingException(DrawingExceptionOverlayEventArgs)`](#ondrawingexceptiondrawingexceptionoverlayeventargs)|
|[`OnDrawnException(DrawnExceptionOverlayEventArgs)`](#ondrawnexceptiondrawnexceptionoverlayeventargs)|
|[`OnEditEnded(Feature)`](#oneditendedfeature)|
|[`OnFeatureDragged(FeatureDraggedEditInteractiveOverlayEventArgs)`](#onfeaturedraggedfeaturedraggededitinteractiveoverlayeventargs)|
|[`OnFeatureDragging(FeatureDraggingEditInteractiveOverlayEventArgs)`](#onfeaturedraggingfeaturedraggingeditinteractiveoverlayeventargs)|
|[`OnFeatureResized(FeatureResizedEditInteractiveOverlayEventArgs)`](#onfeatureresizedfeatureresizededitinteractiveoverlayeventargs)|
|[`OnFeatureResizing(FeatureResizingEditInteractiveOverlayEventArgs)`](#onfeatureresizingfeatureresizingeditinteractiveoverlayeventargs)|
|[`OnFeatureRotated(FeatureRotatedEditInteractiveOverlayEventArgs)`](#onfeaturerotatedfeaturerotatededitinteractiveoverlayeventargs)|
|[`OnFeatureRotating(FeatureRotatingEditInteractiveOverlayEventArgs)`](#onfeaturerotatingfeaturerotatingeditinteractiveoverlayeventargs)|
|[`OnVertexAdded(VertexAddedEditInteractiveOverlayEventArgs)`](#onvertexaddedvertexaddededitinteractiveoverlayeventargs)|
|[`OnVertexAdding(VertexAddingEditInteractiveOverlayEventArgs)`](#onvertexaddingvertexaddingeditinteractiveoverlayeventargs)|
|[`OnVertexMoved(VertexMovedEditInteractiveOverlayEventArgs)`](#onvertexmovedvertexmovededitinteractiveoverlayeventargs)|
|[`OnVertexMoving(VertexMovingEditInteractiveOverlayEventArgs)`](#onvertexmovingvertexmovingeditinteractiveoverlayeventargs)|
|[`OnVertexRemoved(VertexRemovedEditInteractiveOverlayEventArgs)`](#onvertexremovedvertexremovededitinteractiveoverlayeventargs)|
|[`OnVertexRemoving(VertexRemovingEditInteractiveOverlayEventArgs)`](#onvertexremovingvertexremovingeditinteractiveoverlayeventargs)|
|[`PostTransformCore(TransformArguments,MapArguments)`](#posttransformcoretransformargumentsmaparguments)|
|[`PrepareInertialPan(RectangleShape,RectangleShape,MapArguments)`](#prepareinertialpanrectangleshaperectangleshapemaparguments)|
|[`PrepareInertialPanInternal(RectangleShape,RectangleShape,MapArguments)`](#prepareinertialpaninternalrectangleshaperectangleshapemaparguments)|
|[`RefreshCore()`](#refreshcore)|
|[`RefreshCore(RectangleShape)`](#refreshcorerectangleshape)|
|[`RemoveAllAnimationCore()`](#removeallanimationcore)|
|[`RemoveVertex(PointShape,Double)`](#removevertexpointshapedouble)|
|[`RemoveVertexCore(Feature,Vertex,Double)`](#removevertexcorefeaturevertexdouble)|
|[`ResizeFeature(Feature,PointShape,PointShape)`](#resizefeaturefeaturepointshapepointshape)|
|[`ResizeFeatureCore(Feature,PointShape,PointShape)`](#resizefeaturecorefeaturepointshapepointshape)|
|[`RotateFeature(Feature,PointShape,PointShape)`](#rotatefeaturefeaturepointshapepointshape)|
|[`RotateFeatureCore(Feature,PointShape,PointShape)`](#rotatefeaturecorefeaturepointshapepointshape)|
|[`SetSelectedControlPoint(PointShape,Double)`](#setselectedcontrolpointpointshapedouble)|
|[`SetSelectedControlPointCore(PointShape,Double)`](#setselectedcontrolpointcorepointshapedouble)|
|[`SingleTapCore(InteractionArguments)`](#singletapcoreinteractionarguments)|
|[`TouchDownCore(InteractionArguments)`](#touchdowncoreinteractionarguments)|
|[`TouchMoveCore(InteractionArguments)`](#touchmovecoreinteractionarguments)|
|[`TouchPointerDownCore(InteractionArguments)`](#touchpointerdowncoreinteractionarguments)|
|[`TouchUpCore(InteractionArguments)`](#touchupcoreinteractionarguments)|

### Public Events Summary


|Name|Event Arguments|Description|
|---|---|---|
|[`ControlPointSelected`](#controlpointselected)|[`ControlPointSelectedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.ControlPointSelectedEditInteractiveOverlayEventArgs.md)|N/A|
|[`ControlPointSelecting`](#controlpointselecting)|[`ControlPointSelectingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.ControlPointSelectingEditInteractiveOverlayEventArgs.md)|N/A|
|[`EditEnded`](#editended)|[`EditEndedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.EditEndedEditInteractiveOverlayEventArgs.md)|N/A|
|[`FeatureDragged`](#featuredragged)|[`FeatureDraggedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureDraggedEditInteractiveOverlayEventArgs.md)|N/A|
|[`FeatureDragging`](#featuredragging)|[`FeatureDraggingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureDraggingEditInteractiveOverlayEventArgs.md)|N/A|
|[`FeatureResized`](#featureresized)|[`FeatureResizedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureResizedEditInteractiveOverlayEventArgs.md)|N/A|
|[`FeatureResizing`](#featureresizing)|[`FeatureResizingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureResizingEditInteractiveOverlayEventArgs.md)|N/A|
|[`FeatureRotated`](#featurerotated)|[`FeatureRotatedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureRotatedEditInteractiveOverlayEventArgs.md)|N/A|
|[`FeatureRotating`](#featurerotating)|[`FeatureRotatingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureRotatingEditInteractiveOverlayEventArgs.md)|N/A|
|[`VertexAdded`](#vertexadded)|[`VertexAddedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexAddedEditInteractiveOverlayEventArgs.md)|N/A|
|[`VertexAdding`](#vertexadding)|[`VertexAddingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexAddingEditInteractiveOverlayEventArgs.md)|N/A|
|[`VertexMoved`](#vertexmoved)|[`VertexMovedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexMovedEditInteractiveOverlayEventArgs.md)|N/A|
|[`VertexMoving`](#vertexmoving)|[`VertexMovingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexMovingEditInteractiveOverlayEventArgs.md)|N/A|
|[`VertexRemoved`](#vertexremoved)|[`VertexRemovedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexRemovedEditInteractiveOverlayEventArgs.md)|N/A|
|[`VertexRemoving`](#vertexremoving)|[`VertexRemovingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexRemovingEditInteractiveOverlayEventArgs.md)|N/A|
|[`DrawingException`](#drawingexception)|[`DrawingExceptionOverlayEventArgs`](ThinkGeo.UI.iOS.DrawingExceptionOverlayEventArgs.md)|N/A|
|[`DrawnException`](#drawnexception)|[`DrawnExceptionOverlayEventArgs`](ThinkGeo.UI.iOS.DrawnExceptionOverlayEventArgs.md)|N/A|

## Members Detail

### Public Constructors


|Name|
|---|
|[`EditInteractiveOverlay()`](#editinteractiveoverlay)|

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
#### `CanAddVertex`

**Summary**

   *Gets a value which indicates whether the shape can Add new vertex.*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `CanDrag`

**Summary**

   *Gets a value which indicates whether the shape can be dragged.*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `CanRefreshRegion`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `CanRemoveVertex`

**Summary**

   *Gets a value which indicates whether the shape can remove a existing vertex.*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `CanReshape`

**Summary**

   *Gets a value which indicates whether the shape can be reshaped.*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `CanResize`

**Summary**

   *Gets a value which indicates whether the shape can be resized.*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `CanRotate`

**Summary**

   *Gets a value which indicates whether the shape can be rotated.*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `DragControlPointsLayer`

**Summary**

   *This property gets the InMemoryFeatureLayer which holds the control points for drag.*

**Remarks**

   *Every control points for drag are not the existing vertex of the edit shapes.*

**Return Value**

[`InMemoryFeatureLayer`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureLayer.md)

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
#### `EditShapesLayer`

**Summary**

   *This property gets the InMemoryFeatureLayer which holds the edit shapes.*

**Remarks**

   *N/A*

**Return Value**

[`InMemoryFeatureLayer`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureLayer.md)

---
#### `ExistingControlPointsLayer`

**Summary**

   *This property gets the InMemoryFeatureLayer which holds the control points which represents the existing vertices of the edit shapes.*

**Remarks**

   *Every control points in this layer are the existing vertices of the edit shapes.*

**Return Value**

[`InMemoryFeatureLayer`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureLayer.md)

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

   *This property override its property in base class by watching the feature count in editShapesLayer. If it is empty ,we can skip drawing it for better performance.*

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
#### `ResizeControlPointsLayer`

**Summary**

   *This property gets the InMemoryFeatureLayer which holds the control points for resize.*

**Remarks**

   *Every control points for resize are not the existing vertex of the edit shapes.*

**Return Value**

[`InMemoryFeatureLayer`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureLayer.md)

---
#### `RotateControlPointsLayer`

**Summary**

   *This property gets the InMemoryFeatureLayer which holds the control points for rotate.*

**Remarks**

   *Every control points for rotate are not the existing vertex of the edit shapes.*

**Return Value**

[`InMemoryFeatureLayer`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureLayer.md)

---
#### `SelectedControlPointLayer`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

[`InMemoryFeatureLayer`](../ThinkGeo.Core/ThinkGeo.Core.InMemoryFeatureLayer.md)

---
#### `TrackMode`

**Summary**

   *Gets or sets a mode of TrackOverlay.*

**Remarks**

   *The default mode is TrackMode.None which means you cannot draw or edit features at client. By setting the mode to TrackMode.Point, TrackMode.Line, TrackMode.Polygon etc., you could add point, line or polygon to the FeatureOverlay. Setting the mode to TrackMode.Edit, you could edit the shapes at the client side.*

**Return Value**

[`TrackMode`](ThinkGeo.UI.iOS.TrackMode.md)

---

### Protected Properties

#### `ControlPointType`

**Summary**

   *This property gets or sets the ControlPointType for the control points of the edit shapes.*

**Remarks**

   *N/A*

**Return Value**

[`ControlPointType`](ThinkGeo.UI.iOS.ControlPointType.md)

---
#### `OriginalEditingFeature`

**Summary**

   *This property gets the feature represents the original editing feature.*

**Remarks**

   *N/A*

**Return Value**

[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)

---
#### `SelectControlPointFeature`

**Summary**

   *This property gets the feature represents the select control points of the edit shapes.*

**Remarks**

   *N/A*

**Return Value**

[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)

---

### Public Methods

#### `CalculateAllControlPoints()`

**Summary**

   *This method calculates all control points.*

**Remarks**

   *First, it will clear all control points. Then it will calculate each control points according to their settings.*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `ClearAllControlPoints()`

**Summary**

   *This method clears all control points in corresponding layer.*

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
#### `DeleteTrackShape()`

**Summary**

   *Delete the selected track shape.*

**Remarks**

   *Should set TrackMode as EditShape mode first, use mouse select one shape, and then call DeleteTrackShape, it will delete the selected shape.*

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

#### `AddVertex(PointShape,Double)`

**Summary**

   *This is the method to add vertex from a feature.*

**Remarks**

   *This method is the concrete wrapper for the abstract method AddVertexCore. As this is a concrete public method that wraps a Core method, we reserve the right to add events and other logic to pre- or post-process data returned by the Core version of the method. In this way, we leave our framework open on our end, but also allow you the developer to extend our logic to suit your needs. If you have questions about this, please contact our support team as we would be happy to work with you on extending our framework.*

**Return Value**

|Type|Description|
|---|---|
|`Boolean`|True if add vertex succeed , other wise return false.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|targetPointShape|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifies the point shape to search the vertex.|
|searchingTolerance|`Double`|This parameter specifes the searching tolerance to search the vetex.|

---
#### `AddVertexCore(Feature,PointShape,Double)`

**Summary**

   *This is the Core method of AddVertex which encapsulate the logic.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|Returns a vertex added feature.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|targetFeature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|This parameter specifies the target feature to be add vertex from.|
|targetPointShape|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifies the target vertex to be added.|
|searchingTolerance|`Double`|This parameter specifes the searching tolerance to search the vetex.|

---
#### `CalculateDragControlPoints()`

**Summary**

   *This method caculates the Drag control points for all the features in the EditShapesLayer. You can override its logic by rewrite its core method.*

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
#### `CalculateDragControlPointsCore(Feature)`

**Summary**

   *This is the core API for the CalculateDragControlPoints, you can override this method if you want to change its logic.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|IEnumerable<[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)>|A collection of features stands for the Drag control points.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|feature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|The target feature to caculate the control point.|

---
#### `CalculateResizeControlPoints()`

**Summary**

   *This method caculates the Resize control points for all the features in the EditShapesLayer. You can override its logic by rewrite its core method.*

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
#### `CalculateResizeControlPointsCore(Feature)`

**Summary**

   *This is the core API for the CalculateResizeControlPoints, you can override this method if you want to change its logic.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|IEnumerable<[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)>|A collection of features stands for the Resize control points.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|feature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|The target feature to caculate the control point.|

---
#### `CalculateRotateControlPoints()`

**Summary**

   *This method caculates the Rotate control points for all the features in the EditShapesLayer. You can override its logic by rewrite its core method.*

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
#### `CalculateRotateControlPointsCore(Feature)`

**Summary**

   *This is the core API for the CalculateRotateControlPoints, you can override this method if you want to change its logic.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|IEnumerable<[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)>|A collection of features stands for the Rotate control points.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|feature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|The target feature to caculate the control point.|

---
#### `CalculateVertexControlPoints()`

**Summary**

   *This method caculates the vertex control points for all the features in the EditShapesLayer. You can override its logic by rewrite its core method.*

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
#### `CalculateVertexControlPointsCore(Feature)`

**Summary**

   *This is the core API for the CalculateVertexControlPoints, you can override this method if you want to change its logic.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|IEnumerable<[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)>|A collection of features stands for the Vertex control points.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|feature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|The target feature to caculate the control point.|

---
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
|interactionArguments|[`InteractionArguments`](ThinkGeo.UI.iOS.InteractionArguments.md)|N/A|

---
#### `DragFeature(Feature,PointShape,PointShape)`

**Summary**

   *This is the method to Drag a feature.*

**Remarks**

   *This method is the concrete wrapper for the abstract method DragFeatureCore. As this is a concrete public method that wraps a Core method, we reserve the right to add events and other logic to pre- or post-process data returned by the Core version of the method. In this way, we leave our framework open on our end, but also allow you the developer to extend our logic to suit your needs. If you have questions about this, please contact our support team as we would be happy to work with you on extending our framework.*

**Return Value**

|Type|Description|
|---|---|
|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|Returns a dragged feature.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|sourceFeature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|This parameter specifies the source feature to be dragged.|
|sourceControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the source control point to drag the feature.|
|targetControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the target control point to drag the feature.|

---
#### `DragFeatureCore(Feature,PointShape,PointShape)`

**Summary**

   *This is the Core method of DragFeature which encapsulate the logic.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|Returns a dragged feature.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|sourceFeature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|This parameter specifies the source feature to be dragged.|
|sourceControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the source control point to drag the feature.|
|targetControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the target control point to drag the feature.|

---
#### `DrawCore(MapArguments,OverlayRefreshType)`

**Summary**

   *This method draws the EditInterativeOverlay.*

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
#### `EndEditing(PointShape)`

**Summary**

   *This method End the editing for the interative editing on the feature in the EditShapesLayer. You can override its logic by rewrite its core method.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|targetPointShape|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This is the targetPointShape possible be used when overriding.|

---
#### `EndEditingCore(PointShape)`

**Summary**

   *This is the core method of EndEditing method. This method End the editing for the interative editing on the feature in the EditShapesLayer.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|targetPointShape|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This is the targetPointShape possible be used when overriding.|

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
#### `GetAllVerticesFromFeature(Feature)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|Collection<[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)>|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|feature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|N/A|

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
#### `MoveVertex(Feature,PointShape,PointShape)`

**Summary**

   *This is the method to move vertex from a feature.*

**Remarks**

   *This method is the concrete wrapper for the abstract method MoveVertexCore. As this is a concrete public method that wraps a Core method, we reserve the right to add events and other logic to pre- or post-process data returned by the Core version of the method. In this way, we leave our framework open on our end, but also allow you the developer to extend our logic to suit your needs. If you have questions about this, please contact our support team as we would be happy to work with you on extending our framework.*

**Return Value**

|Type|Description|
|---|---|
|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|Returns a rotated feature.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|sourceFeature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|This parameter specifies the source feature to be move vertex from.|
|sourceControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the source control point to move vertex from the feature.|
|targetControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the target control point to move vertex from the feature.|

---
#### `MoveVertexCore(Feature,PointShape,PointShape)`

**Summary**

   *This is the Core method of MoveVertex which encapsulate the logic.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|Returns a vertex moved feature.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|sourceFeature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|This parameter specifies the source feature to be move vertex from.|
|sourceControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the source control point to move vertex from the feature.|
|targetControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the target control point to move vertex from the feature.|

---
#### `OnControlPointSelected(ControlPointSelectedEditInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired after control point selected.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`ControlPointSelectedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.ControlPointSelectedEditInteractiveOverlayEventArgs.md)|The ControlPointSelectedEditInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnControlPointSelecting(ControlPointSelectingEditInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired before control point selected.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`ControlPointSelectingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.ControlPointSelectingEditInteractiveOverlayEventArgs.md)|The ControlPointSelectingEditInteractiveOverlayEventArgs passed for the event raised.|

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
#### `OnEditEnded(Feature)`

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
|editedFeature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|N/A|

---
#### `OnFeatureDragged(FeatureDraggedEditInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired after dragging the feature.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`FeatureDraggedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureDraggedEditInteractiveOverlayEventArgs.md)|The FeatureDraggedEditInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnFeatureDragging(FeatureDraggingEditInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired before dragging the feature.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`FeatureDraggingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureDraggingEditInteractiveOverlayEventArgs.md)|The FeatureDraggingEditInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnFeatureResized(FeatureResizedEditInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired after resizing the feature.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`FeatureResizedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureResizedEditInteractiveOverlayEventArgs.md)|The FeatureResizedEditInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnFeatureResizing(FeatureResizingEditInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired before resizing the feature.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`FeatureResizingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureResizingEditInteractiveOverlayEventArgs.md)|The FeatureResizingEditInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnFeatureRotated(FeatureRotatedEditInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired after rotating the feature.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`FeatureRotatedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureRotatedEditInteractiveOverlayEventArgs.md)|The FeatureRotatedEditInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnFeatureRotating(FeatureRotatingEditInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired before rotating the feature.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`FeatureRotatingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureRotatingEditInteractiveOverlayEventArgs.md)|The FeatureRotatingEditInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnVertexAdded(VertexAddedEditInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired after vertex added to the edit feature.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`VertexAddedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexAddedEditInteractiveOverlayEventArgs.md)|The VertexAddedEditInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnVertexAdding(VertexAddingEditInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired before vertex added to the edit feature.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`VertexAddingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexAddingEditInteractiveOverlayEventArgs.md)|The VertexAddingEditInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnVertexMoved(VertexMovedEditInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired after moving the feature.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`VertexMovedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexMovedEditInteractiveOverlayEventArgs.md)|The VertexMovedEditInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnVertexMoving(VertexMovingEditInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired before moving the feature.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`VertexMovingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexMovingEditInteractiveOverlayEventArgs.md)|The VertexMovingEditInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnVertexRemoved(VertexRemovedEditInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired after vertex removed from the edit feature.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`VertexRemovedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexRemovedEditInteractiveOverlayEventArgs.md)|The VertexRemovedEditInteractiveOverlayEventArgs passed for the event raised.|

---
#### `OnVertexRemoving(VertexRemovingEditInteractiveOverlayEventArgs)`

**Summary**

   *This event will be fired before vertex removed from the edit feature.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|e|[`VertexRemovingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexRemovingEditInteractiveOverlayEventArgs.md)|The VertexRemovingEditInteractiveOverlayEventArgs passed for the event raised.|

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
#### `RemoveVertex(PointShape,Double)`

**Summary**

   *This is the method to remove vertex from a feature.*

**Remarks**

   *This method is the concrete wrapper for the abstract method RemoveVertexCore. As this is a concrete public method that wraps a Core method, we reserve the right to add events and other logic to pre- or post-process data returned by the Core version of the method. In this way, we leave our framework open on our end, but also allow you the developer to extend our logic to suit your needs. If you have questions about this, please contact our support team as we would be happy to work with you on extending our framework.*

**Return Value**

|Type|Description|
|---|---|
|`Boolean`|True if remove vertex succeed , other wise return false.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|targetPointShape|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifies the point shape to search the vertex.|
|searchingTolerance|`Double`|This parameter specifes the searching tolerance to search the vetex.|

---
#### `RemoveVertexCore(Feature,Vertex,Double)`

**Summary**

   *This is the Core method of RemoveVertex which encapsulate the logic.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|Returns a vertex removed feature.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|editShapeFeature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|This parameter specifies the target feature to be remove vertex from.|
|selectedVertex|[`Vertex`](../ThinkGeo.Core/ThinkGeo.Core.Vertex.md)|This parameter specifies the selected vertex to search the vertex.|
|searchingTolerance|`Double`|This parameter specifes the searching tolerance to search the vetex.|

---
#### `ResizeFeature(Feature,PointShape,PointShape)`

**Summary**

   *This is the method to Resize a feature.*

**Remarks**

   *This method is the concrete wrapper for the abstract method ResizeFeatureCore. As this is a concrete public method that wraps a Core method, we reserve the right to add events and other logic to pre- or post-process data returned by the Core version of the method. In this way, we leave our framework open on our end, but also allow you the developer to extend our logic to suit your needs. If you have questions about this, please contact our support team as we would be happy to work with you on extending our framework.*

**Return Value**

|Type|Description|
|---|---|
|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|Returns a resized feature.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|sourceFeature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|This parameter specifies the source feature to be resized.|
|sourceControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the source control point to resize the feature.|
|targetControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the target control point to resize the feature.|

---
#### `ResizeFeatureCore(Feature,PointShape,PointShape)`

**Summary**

   *This is the Core method of ResizeFeature which encapsulate the logic.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|Returns a resized feature.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|sourceFeature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|This parameter specifies the source feature to be resized.|
|sourceControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the source control point to resize the feature.|
|targetControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the target control point to resize the feature.|

---
#### `RotateFeature(Feature,PointShape,PointShape)`

**Summary**

   *This is the method to Rotate a feature.*

**Remarks**

   *This method is the concrete wrapper for the abstract method RotateFeatureCore. As this is a concrete public method that wraps a Core method, we reserve the right to add events and other logic to pre- or post-process data returned by the Core version of the method. In this way, we leave our framework open on our end, but also allow you the developer to extend our logic to suit your needs. If you have questions about this, please contact our support team as we would be happy to work with you on extending our framework.*

**Return Value**

|Type|Description|
|---|---|
|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|Returns a rotated feature.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|sourceFeature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|This parameter specifies the source feature to be rotated.|
|sourceControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the source control point to rotate the feature.|
|targetControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the target control point to rotate the feature.|

---
#### `RotateFeatureCore(Feature,PointShape,PointShape)`

**Summary**

   *This is the Core method of RotateFeature which encapsulate the logic.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|Returns a resized feature.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|sourceFeature|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|This parameter specifies the source feature to be rotated.|
|sourceControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the source control point to rotate the feature.|
|targetControlPoint|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter specifes the target control point to rotate the feature.|

---
#### `SetSelectedControlPoint(PointShape,Double)`

**Summary**

   *This protected method is to set the control point.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Boolean`|Returns true if control point are found and set correct, other wise, returns false.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|targetPointShape|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter is target point shape we determine to edit.|
|searchingTolerance|`Double`|This parameter is the searchinig tolerance to seach the control point.|

---
#### `SetSelectedControlPointCore(PointShape,Double)`

**Summary**

   *This protected virtual method is the Core method of SetSelectedControlPoint API.*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|[`Feature`](../ThinkGeo.Core/ThinkGeo.Core.Feature.md)|A feature stands for the selected control point.|

**Parameters**

|Name|Type|Description|
|---|---|---|
|targetPointShape|[`PointShape`](../ThinkGeo.Core/ThinkGeo.Core.PointShape.md)|This parameter is target point shape we determine to edit.|
|searchingTolerance|`Double`|This parameter is the searchinig tolerance to seach the control point.|

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

#### ControlPointSelected

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`ControlPointSelectedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.ControlPointSelectedEditInteractiveOverlayEventArgs.md)

#### ControlPointSelecting

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`ControlPointSelectingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.ControlPointSelectingEditInteractiveOverlayEventArgs.md)

#### EditEnded

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`EditEndedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.EditEndedEditInteractiveOverlayEventArgs.md)

#### FeatureDragged

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`FeatureDraggedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureDraggedEditInteractiveOverlayEventArgs.md)

#### FeatureDragging

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`FeatureDraggingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureDraggingEditInteractiveOverlayEventArgs.md)

#### FeatureResized

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`FeatureResizedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureResizedEditInteractiveOverlayEventArgs.md)

#### FeatureResizing

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`FeatureResizingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureResizingEditInteractiveOverlayEventArgs.md)

#### FeatureRotated

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`FeatureRotatedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureRotatedEditInteractiveOverlayEventArgs.md)

#### FeatureRotating

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`FeatureRotatingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.FeatureRotatingEditInteractiveOverlayEventArgs.md)

#### VertexAdded

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`VertexAddedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexAddedEditInteractiveOverlayEventArgs.md)

#### VertexAdding

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`VertexAddingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexAddingEditInteractiveOverlayEventArgs.md)

#### VertexMoved

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`VertexMovedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexMovedEditInteractiveOverlayEventArgs.md)

#### VertexMoving

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`VertexMovingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexMovingEditInteractiveOverlayEventArgs.md)

#### VertexRemoved

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`VertexRemovedEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexRemovedEditInteractiveOverlayEventArgs.md)

#### VertexRemoving

*N/A*

**Remarks**

*N/A*

**Event Arguments**

[`VertexRemovingEditInteractiveOverlayEventArgs`](ThinkGeo.UI.iOS.VertexRemovingEditInteractiveOverlayEventArgs.md)

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


