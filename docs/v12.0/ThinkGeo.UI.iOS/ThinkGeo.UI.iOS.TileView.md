# TileView


## Inheritance Hierarchy

+ `Object`
  + `NSObject`
    + `UIResponder`
      + `UIView`
        + `UIImageView`
          + **`TileView`**
            + [`LayerTileView`](ThinkGeo.UI.iOS.LayerTileView.md)
            + [`UriTileView`](ThinkGeo.UI.iOS.UriTileView.md)

## Members Summary

### Public Constructors Summary


|Name|
|---|
|[`TileView()`](#tileview)|
|[`TileView(CGRect)`](#tileviewcgrect)|

### Protected Constructors Summary


|Name|
|---|
|N/A|

### Public Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`AccessibilityActivationPoint`](#accessibilityactivationpoint)|`CGPoint`|N/A|
|[`AccessibilityAssistiveTechnologyFocusedIdentifiers`](#accessibilityassistivetechnologyfocusedidentifiers)|NSSet<`NSString`>|N/A|
|[`AccessibilityAttributedHint`](#accessibilityattributedhint)|`NSAttributedString`|N/A|
|[`AccessibilityAttributedLabel`](#accessibilityattributedlabel)|`NSAttributedString`|N/A|
|[`AccessibilityAttributedUserInputLabels`](#accessibilityattributeduserinputlabels)|`NSAttributedString[]`|N/A|
|[`AccessibilityAttributedValue`](#accessibilityattributedvalue)|`NSAttributedString`|N/A|
|[`AccessibilityCustomActions`](#accessibilitycustomactions)|`UIAccessibilityCustomAction[]`|N/A|
|[`AccessibilityDragSourceDescriptors`](#accessibilitydragsourcedescriptors)|`UIAccessibilityLocationDescriptor[]`|N/A|
|[`AccessibilityDropPointDescriptors`](#accessibilitydroppointdescriptors)|`UIAccessibilityLocationDescriptor[]`|N/A|
|[`AccessibilityElementsHidden`](#accessibilityelementshidden)|`Boolean`|N/A|
|[`AccessibilityFrame`](#accessibilityframe)|`CGRect`|N/A|
|[`AccessibilityHint`](#accessibilityhint)|`String`|N/A|
|[`AccessibilityIdentifier`](#accessibilityidentifier)|`String`|N/A|
|[`AccessibilityIgnoresInvertColors`](#accessibilityignoresinvertcolors)|`Boolean`|N/A|
|[`AccessibilityLabel`](#accessibilitylabel)|`String`|N/A|
|[`AccessibilityLanguage`](#accessibilitylanguage)|`String`|N/A|
|[`AccessibilityNavigationStyle`](#accessibilitynavigationstyle)|`UIAccessibilityNavigationStyle`|N/A|
|[`AccessibilityPath`](#accessibilitypath)|`UIBezierPath`|N/A|
|[`AccessibilityRespondsToUserInteraction`](#accessibilityrespondstouserinteraction)|`Boolean`|N/A|
|[`AccessibilityTextualContext`](#accessibilitytextualcontext)|`String`|N/A|
|[`AccessibilityTraits`](#accessibilitytraits)|`UIAccessibilityTrait`|N/A|
|[`AccessibilityUserInputLabels`](#accessibilityuserinputlabels)|`String[]`|N/A|
|[`AccessibilityValue`](#accessibilityvalue)|`String`|N/A|
|[`AccessibilityViewIsModal`](#accessibilityviewismodal)|`Boolean`|N/A|
|[`ActivityItemsConfiguration`](#activityitemsconfiguration)|`IUIActivityItemsConfigurationReading`|N/A|
|[`AdjustsImageSizeForAccessibilityContentSizeCategory`](#adjustsimagesizeforaccessibilitycontentsizecategory)|`Boolean`|N/A|
|[`AlignmentRectInsets`](#alignmentrectinsets)|`UIEdgeInsets`|N/A|
|[`Alpha`](#alpha)|`nfloat`|N/A|
|[`AnimationDuration`](#animationduration)|`Double`|N/A|
|[`AnimationImages`](#animationimages)|`UIImage[]`|N/A|
|[`AnimationRepeatCount`](#animationrepeatcount)|`nint`|N/A|
|[`AutoresizingMask`](#autoresizingmask)|`UIViewAutoresizing`|N/A|
|[`AutosizesSubviews`](#autosizessubviews)|`Boolean`|N/A|
|[`BackgroundColor`](#backgroundcolor)|`UIColor`|N/A|
|[`BottomAnchor`](#bottomanchor)|`NSLayoutYAxisAnchor`|N/A|
|[`BoundingBox`](#boundingbox)|[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)|Gets or sets the bounding box.|
|[`Bounds`](#bounds)|`CGRect`|N/A|
|[`CanBecomeFirstResponder`](#canbecomefirstresponder)|`Boolean`|N/A|
|[`CanBecomeFocused`](#canbecomefocused)|`Boolean`|N/A|
|[`CanResignFirstResponder`](#canresignfirstresponder)|`Boolean`|N/A|
|[`Center`](#center)|`CGPoint`|N/A|
|[`CenterXAnchor`](#centerxanchor)|`NSLayoutXAxisAnchor`|N/A|
|[`CenterYAnchor`](#centeryanchor)|`NSLayoutYAxisAnchor`|N/A|
|[`Class`](#class)|`Class`|N/A|
|[`ClassHandle`](#classhandle)|`IntPtr`|N/A|
|[`ClearsContextBeforeDrawing`](#clearscontextbeforedrawing)|`Boolean`|N/A|
|[`ClipsToBounds`](#clipstobounds)|`Boolean`|N/A|
|[`CollisionBoundingPath`](#collisionboundingpath)|`UIBezierPath`|N/A|
|[`CollisionBoundsType`](#collisionboundstype)|`UIDynamicItemCollisionBoundsType`|N/A|
|[`ColumnIndex`](#columnindex)|`Int64`|Gets or sets the index of the column.|
|[`Constraints`](#constraints)|`NSLayoutConstraint[]`|N/A|
|[`ContentMode`](#contentmode)|`UIViewContentMode`|N/A|
|[`ContentScaleFactor`](#contentscalefactor)|`nfloat`|N/A|
|[`ContentStretch`](#contentstretch)|`CGRect`|N/A|
|[`CoordinateSpace`](#coordinatespace)|`IUICoordinateSpace`|N/A|
|[`DebugDescription`](#debugdescription)|`String`|N/A|
|[`Description`](#description)|`String`|N/A|
|[`DirectionalLayoutMargins`](#directionallayoutmargins)|`NSDirectionalEdgeInsets`|N/A|
|[`EditingInteractionConfiguration`](#editinginteractionconfiguration)|`UIEditingInteractionConfiguration`|N/A|
|[`EffectiveUserInterfaceLayoutDirection`](#effectiveuserinterfacelayoutdirection)|`UIUserInterfaceLayoutDirection`|N/A|
|[`Error`](#error)|`Exception`|User can set the customer excpetion and we can paint that.|
|[`ExclusiveTouch`](#exclusivetouch)|`Boolean`|N/A|
|[`FirstBaselineAnchor`](#firstbaselineanchor)|`NSLayoutYAxisAnchor`|N/A|
|[`Focused`](#focused)|`Boolean`|N/A|
|[`FocusItemContainer`](#focusitemcontainer)|`IUIFocusItemContainer`|N/A|
|[`Frame`](#frame)|`CGRect`|N/A|
|[`GestureRecognizers`](#gesturerecognizers)|`UIGestureRecognizer[]`|N/A|
|[`Handle`](#handle)|`IntPtr`|N/A|
|[`HasAmbiguousLayout`](#hasambiguouslayout)|`Boolean`|N/A|
|[`HeightAnchor`](#heightanchor)|`NSLayoutDimension`|N/A|
|[`Hidden`](#hidden)|`Boolean`|N/A|
|[`Highlighted`](#highlighted)|`Boolean`|N/A|
|[`HighlightedAnimationImages`](#highlightedanimationimages)|`UIImage[]`|N/A|
|[`HighlightedImage`](#highlightedimage)|`UIImage`|N/A|
|[`Image`](#image)|`UIImage`|N/A|
|[`ImageSource`](#imagesource)|[`GeoImage`](../ThinkGeo.Core/ThinkGeo.Core.GeoImage.md)|Gets or sets the image source.|
|[`InputAccessoryView`](#inputaccessoryview)|`UIView`|N/A|
|[`InputAccessoryViewController`](#inputaccessoryviewcontroller)|`UIInputViewController`|N/A|
|[`InputAssistantItem`](#inputassistantitem)|`UITextInputAssistantItem`|N/A|
|[`InputView`](#inputview)|`UIView`|N/A|
|[`InputViewController`](#inputviewcontroller)|`UIInputViewController`|N/A|
|[`InsetsLayoutMarginsFromSafeArea`](#insetslayoutmarginsfromsafearea)|`Boolean`|N/A|
|[`Interactions`](#interactions)|`IUIInteraction[]`|N/A|
|[`IntrinsicContentSize`](#intrinsiccontentsize)|`CGSize`|N/A|
|[`IsAccessibilityElement`](#isaccessibilityelement)|`Boolean`|N/A|
|[`IsAnimating`](#isanimating)|`Boolean`|N/A|
|[`IsFirstResponder`](#isfirstresponder)|`Boolean`|N/A|
|[`IsProxy`](#isproxy)|`Boolean`|N/A|
|[`KeyCommands`](#keycommands)|`UIKeyCommand[]`|N/A|
|[`LargeContentImage`](#largecontentimage)|`UIImage`|N/A|
|[`LargeContentImageInsets`](#largecontentimageinsets)|`UIEdgeInsets`|N/A|
|[`LargeContentTitle`](#largecontenttitle)|`String`|N/A|
|[`LastBaselineAnchor`](#lastbaselineanchor)|`NSLayoutYAxisAnchor`|N/A|
|[`Layer`](#layer)|`CALayer`|N/A|
|[`LayoutGuides`](#layoutguides)|`UILayoutGuide[]`|N/A|
|[`LayoutMargins`](#layoutmargins)|`UIEdgeInsets`|N/A|
|[`LayoutMarginsGuide`](#layoutmarginsguide)|`UILayoutGuide`|N/A|
|[`LeadingAnchor`](#leadinganchor)|`NSLayoutXAxisAnchor`|N/A|
|[`LeftAnchor`](#leftanchor)|`NSLayoutXAxisAnchor`|N/A|
|[`MapUnit`](#mapunit)|[`GeographyUnit`](../ThinkGeo.Core/ThinkGeo.Core.GeographyUnit.md)|Gets or sets the map unit.|
|[`MaskView`](#maskview)|`UIView`|N/A|
|[`MotionEffects`](#motioneffects)|`UIMotionEffect[]`|N/A|
|[`MultipleTouchEnabled`](#multipletouchenabled)|`Boolean`|N/A|
|[`NextResponder`](#nextresponder)|`UIResponder`|N/A|
|[`Opaque`](#opaque)|`Boolean`|N/A|
|[`OverrideUserInterfaceStyle`](#overrideuserinterfacestyle)|`UIUserInterfaceStyle`|N/A|
|[`ParentFocusEnvironment`](#parentfocusenvironment)|`IUIFocusEnvironment`|N/A|
|[`PasteConfiguration`](#pasteconfiguration)|`UIPasteConfiguration`|N/A|
|[`PreferredFocusedView`](#preferredfocusedview)|`UIView`|N/A|
|[`PreferredFocusEnvironments`](#preferredfocusenvironments)|`IUIFocusEnvironment[]`|N/A|
|[`PreferredSymbolConfiguration`](#preferredsymbolconfiguration)|`UIImageSymbolConfiguration`|N/A|
|[`PreservesSuperviewLayoutMargins`](#preservessuperviewlayoutmargins)|`Boolean`|N/A|
|[`ReadableContentGuide`](#readablecontentguide)|`UILayoutGuide`|N/A|
|[`RestorationIdentifier`](#restorationidentifier)|`String`|N/A|
|[`RetainCount`](#retaincount)|`nuint`|N/A|
|[`RightAnchor`](#rightanchor)|`NSLayoutXAxisAnchor`|N/A|
|[`RowIndex`](#rowindex)|`Int64`|Gets or sets the index of the row.|
|[`SafeAreaInsets`](#safeareainsets)|`UIEdgeInsets`|N/A|
|[`SafeAreaLayoutGuide`](#safearealayoutguide)|`UILayoutGuide`|N/A|
|[`Scale`](#scale)|`Double`|Gets or sets the scale.|
|[`ScalesLargeContentImage`](#scaleslargecontentimage)|`Boolean`|N/A|
|[`Self`](#self)|`NSObject`|N/A|
|[`SemanticContentAttribute`](#semanticcontentattribute)|`UISemanticContentAttribute`|N/A|
|[`ShouldGroupAccessibilityChildren`](#shouldgroupaccessibilitychildren)|`Boolean`|N/A|
|[`ShowsLargeContentViewer`](#showslargecontentviewer)|`Boolean`|N/A|
|[`Subviews`](#subviews)|`UIView[]`|N/A|
|[`Superclass`](#superclass)|`Class`|N/A|
|[`SuperHandle`](#superhandle)|`IntPtr`|N/A|
|[`Superview`](#superview)|`UIView`|N/A|
|[`Tag`](#tag)|`nint`|N/A|
|[`TextInputContextIdentifier`](#textinputcontextidentifier)|`NSString`|N/A|
|[`TextInputMode`](#textinputmode)|`UITextInputMode`|N/A|
|[`TileHeight`](#tileheight)|`Int32`|Gets or sets the height of the tile.|
|[`TileWidth`](#tilewidth)|`Int32`|Gets or sets the width of the tile.|
|[`TintAdjustmentMode`](#tintadjustmentmode)|`UIViewTintAdjustmentMode`|N/A|
|[`TintColor`](#tintcolor)|`UIColor`|N/A|
|[`TopAnchor`](#topanchor)|`NSLayoutYAxisAnchor`|N/A|
|[`TrailingAnchor`](#trailinganchor)|`NSLayoutXAxisAnchor`|N/A|
|[`TraitCollection`](#traitcollection)|`UITraitCollection`|N/A|
|[`Transform`](#transform)|`CGAffineTransform`|N/A|
|[`Transform3D`](#transform3d)|`CATransform3D`|N/A|
|[`TranslatesAutoresizingMaskIntoConstraints`](#translatesautoresizingmaskintoconstraints)|`Boolean`|N/A|
|[`UndoManager`](#undomanager)|`NSUndoManager`|N/A|
|[`UserActivity`](#useractivity)|`NSUserActivity`|N/A|
|[`UserInteractionEnabled`](#userinteractionenabled)|`Boolean`|N/A|
|[`ViewForBaselineLayout`](#viewforbaselinelayout)|`UIView`|N/A|
|[`ViewForFirstBaselineLayout`](#viewforfirstbaselinelayout)|`UIView`|N/A|
|[`ViewForLastBaselineLayout`](#viewforlastbaselinelayout)|`UIView`|N/A|
|[`ViewPrintFormatter`](#viewprintformatter)|`UIViewPrintFormatter`|N/A|
|[`WidthAnchor`](#widthanchor)|`NSLayoutDimension`|N/A|
|[`Window`](#window)|`UIWindow`|N/A|
|[`Zone`](#zone)|`NSZone`|N/A|
|[`ZoomLevelIndex`](#zoomlevelindex)|`Int32`|Gets or sets the index of the zoom level.|

### Protected Properties Summary

|Name|Return Type|Description|
|---|---|---|
|[`CacheUrl`](#cacheurl)|`String`|N/A|
|[`InFinalizerQueue`](#infinalizerqueue)|`Boolean`|N/A|
|[`IsCanceled`](#iscanceled)|`Boolean`|N/A|
|[`IsDirectBinding`](#isdirectbinding)|`Boolean`|N/A|
|[`IsDisposed`](#isdisposed)|`Boolean`|N/A|
|[`IsRegisteredToggleRef`](#isregisteredtoggleref)|`Boolean`|N/A|
|[`IsStretch`](#isstretch)|`Boolean`|N/A|

### Public Methods Summary


|Name|
|---|
|[`AccessibilityActivate()`](#accessibilityactivate)|
|[`AccessibilityDecrement()`](#accessibilitydecrement)|
|[`AccessibilityElementDidBecomeFocused()`](#accessibilityelementdidbecomefocused)|
|[`AccessibilityElementDidLoseFocus()`](#accessibilityelementdidlosefocus)|
|[`AccessibilityElementIsFocused()`](#accessibilityelementisfocused)|
|[`AccessibilityIncrement()`](#accessibilityincrement)|
|[`AccessibilityPerformEscape()`](#accessibilityperformescape)|
|[`AccessibilityPerformMagicTap()`](#accessibilityperformmagictap)|
|[`AccessibilityScroll(UIAccessibilityScrollDirection)`](#accessibilityscrolluiaccessibilityscrolldirection)|
|[`ActionForLayer(CALayer,String)`](#actionforlayercalayerstring)|
|[`Add(UIView)`](#adduiview)|
|[`AddConstraint(NSLayoutConstraint)`](#addconstraintnslayoutconstraint)|
|[`AddConstraints(NSLayoutConstraint[])`](#addconstraintsnslayoutconstraint[])|
|[`AddGestureRecognizer(UIGestureRecognizer)`](#addgesturerecognizeruigesturerecognizer)|
|[`AddInteraction(IUIInteraction)`](#addinteractioniuiinteraction)|
|[`AddLayoutGuide(UILayoutGuide)`](#addlayoutguideuilayoutguide)|
|[`AddMotionEffect(UIMotionEffect)`](#addmotioneffectuimotioneffect)|
|[`AddObserver(String,NSKeyValueObservingOptions,Action<NSObservedChange>)`](#addobserverstringnskeyvalueobservingoptionsaction<nsobservedchange>)|
|[`AddObserver(NSString,NSKeyValueObservingOptions,Action<NSObservedChange>)`](#addobservernsstringnskeyvalueobservingoptionsaction<nsobservedchange>)|
|[`AddObserver(NSObject,NSString,NSKeyValueObservingOptions,IntPtr)`](#addobservernsobjectnsstringnskeyvalueobservingoptionsintptr)|
|[`AddObserver(NSObject,String,NSKeyValueObservingOptions,IntPtr)`](#addobservernsobjectstringnskeyvalueobservingoptionsintptr)|
|[`AddSubview(UIView)`](#addsubviewuiview)|
|[`AddSubviews(UIView[])`](#addsubviewsuiview[])|
|[`AlignmentRectForFrame(CGRect)`](#alignmentrectforframecgrect)|
|[`AwakeFromNib()`](#awakefromnib)|
|[`BecomeFirstResponder()`](#becomefirstresponder)|
|[`BeginInvokeOnMainThread(Selector,NSObject)`](#begininvokeonmainthreadselectornsobject)|
|[`BeginInvokeOnMainThread(Action)`](#begininvokeonmainthreadaction)|
|[`BringSubviewToFront(UIView)`](#bringsubviewtofrontuiview)|
|[`BuildMenu(IUIMenuBuilder)`](#buildmenuiuimenubuilder)|
|[`CanPaste(NSItemProvider[])`](#canpastensitemprovider[])|
|[`CanPerform(Selector,NSObject)`](#canperformselectornsobject)|
|[`Capture(Boolean)`](#captureboolean)|
|[`ConformsToProtocol(IntPtr)`](#conformstoprotocolintptr)|
|[`ContentCompressionResistancePriority(UILayoutConstraintAxis)`](#contentcompressionresistancepriorityuilayoutconstraintaxis)|
|[`ContentHuggingPriority(UILayoutConstraintAxis)`](#contenthuggingpriorityuilayoutconstraintaxis)|
|[`ConvertPointFromCoordinateSpace(CGPoint,IUICoordinateSpace)`](#convertpointfromcoordinatespacecgpointiuicoordinatespace)|
|[`ConvertPointFromView(CGPoint,UIView)`](#convertpointfromviewcgpointuiview)|
|[`ConvertPointToCoordinateSpace(CGPoint,IUICoordinateSpace)`](#convertpointtocoordinatespacecgpointiuicoordinatespace)|
|[`ConvertPointToView(CGPoint,UIView)`](#convertpointtoviewcgpointuiview)|
|[`ConvertRectFromCoordinateSpace(CGRect,IUICoordinateSpace)`](#convertrectfromcoordinatespacecgrectiuicoordinatespace)|
|[`ConvertRectFromView(CGRect,UIView)`](#convertrectfromviewcgrectuiview)|
|[`ConvertRectToCoordinateSpace(CGRect,IUICoordinateSpace)`](#convertrecttocoordinatespacecgrectiuicoordinatespace)|
|[`ConvertRectToView(CGRect,UIView)`](#convertrecttoviewcgrectuiview)|
|[`Copy(NSObject)`](#copynsobject)|
|[`Copy()`](#copy)|
|[`Cut(NSObject)`](#cutnsobject)|
|[`DangerousAutorelease()`](#dangerousautorelease)|
|[`DangerousRelease()`](#dangerousrelease)|
|[`DangerousRetain()`](#dangerousretain)|
|[`DecodeRestorableState(NSCoder)`](#decoderestorablestatenscoder)|
|[`Delete(NSObject)`](#deletensobject)|
|[`DidChange(NSKeyValueChange,NSIndexSet,NSString)`](#didchangenskeyvaluechangensindexsetnsstring)|
|[`DidChange(NSString,NSKeyValueSetMutationKind,NSSet)`](#didchangensstringnskeyvaluesetmutationkindnsset)|
|[`DidChangeValue(String)`](#didchangevaluestring)|
|[`DidHintFocusMovement(UIFocusMovementHint)`](#didhintfocusmovementuifocusmovementhint)|
|[`DidUpdateFocus(UIFocusUpdateContext,UIFocusAnimationCoordinator)`](#didupdatefocusuifocusupdatecontextuifocusanimationcoordinator)|
|[`DisplayLayer(CALayer)`](#displaylayercalayer)|
|[`Dispose()`](#dispose)|
|[`DoesNotRecognizeSelector(Selector)`](#doesnotrecognizeselectorselector)|
|[`Draw(GeoCanvas)`](#drawgeocanvas)|
|[`Draw(CGRect)`](#drawcgrect)|
|[`DrawLayer(CALayer,CGContext)`](#drawlayercalayercgcontext)|
|[`DrawRect(CGRect,UIViewPrintFormatter)`](#drawrectcgrectuiviewprintformatter)|
|[`DrawViewHierarchy(CGRect,Boolean)`](#drawviewhierarchycgrectboolean)|
|[`EncodeRestorableState(NSCoder)`](#encoderestorablestatenscoder)|
|[`EncodeTo(NSCoder)`](#encodetonscoder)|
|[`Equals(Object)`](#equalsobject)|
|[`Equals(NSObject)`](#equalsnsobject)|
|[`ExchangeSubview(nint,nint)`](#exchangesubviewnintnint)|
|[`ExerciseAmbiguityInLayout()`](#exerciseambiguityinlayout)|
|[`FrameForAlignmentRect(CGRect)`](#frameforalignmentrectcgrect)|
|[`GestureRecognizerShouldBegin(UIGestureRecognizer)`](#gesturerecognizershouldbeginuigesturerecognizer)|
|[`GetConstraintsAffectingLayout(UILayoutConstraintAxis)`](#getconstraintsaffectinglayoutuilayoutconstraintaxis)|
|[`GetDictionaryOfValuesFromKeys(NSString[])`](#getdictionaryofvaluesfromkeysnsstring[])|
|[`GetEnumerator()`](#getenumerator)|
|[`GetFocusItems(CGRect)`](#getfocusitemscgrect)|
|[`GetHashCode()`](#gethashcode)|
|[`GetMethodForSelector(Selector)`](#getmethodforselectorselector)|
|[`GetNativeField(String)`](#getnativefieldstring)|
|[`GetNativeHash()`](#getnativehash)|
|[`GetTargetForAction(Selector,NSObject)`](#gettargetforactionselectornsobject)|
|[`GetType()`](#gettype)|
|[`HitTest(CGPoint,UIEvent)`](#hittestcgpointuievent)|
|[`Init()`](#init)|
|[`InsertSubview(UIView,nint)`](#insertsubviewuiviewnint)|
|[`InsertSubviewAbove(UIView,UIView)`](#insertsubviewaboveuiviewuiview)|
|[`InsertSubviewBelow(UIView,UIView)`](#insertsubviewbelowuiviewuiview)|
|[`InvalidateIntrinsicContentSize()`](#invalidateintrinsiccontentsize)|
|[`Invoke(Action,Double)`](#invokeactiondouble)|
|[`Invoke(Action,TimeSpan)`](#invokeactiontimespan)|
|[`InvokeOnMainThread(Selector,NSObject)`](#invokeonmainthreadselectornsobject)|
|[`InvokeOnMainThread(Action)`](#invokeonmainthreadaction)|
|[`IsDescendantOfView(UIView)`](#isdescendantofviewuiview)|
|[`IsEqual(NSObject)`](#isequalnsobject)|
|[`IsKindOfClass(Class)`](#iskindofclassclass)|
|[`IsMemberOfClass(Class)`](#ismemberofclassclass)|
|[`LayoutIfNeeded()`](#layoutifneeded)|
|[`LayoutMarginsDidChange()`](#layoutmarginsdidchange)|
|[`LayoutSublayersOfLayer(CALayer)`](#layoutsublayersoflayercalayer)|
|[`LayoutSubviews()`](#layoutsubviews)|
|[`MakeTextWritingDirectionLeftToRight(NSObject)`](#maketextwritingdirectionlefttorightnsobject)|
|[`MakeTextWritingDirectionRightToLeft(NSObject)`](#maketextwritingdirectionrighttoleftnsobject)|
|[`MotionBegan(UIEventSubtype,UIEvent)`](#motionbeganuieventsubtypeuievent)|
|[`MotionCancelled(UIEventSubtype,UIEvent)`](#motioncancelleduieventsubtypeuievent)|
|[`MotionEnded(UIEventSubtype,UIEvent)`](#motionendeduieventsubtypeuievent)|
|[`MovedToSuperview()`](#movedtosuperview)|
|[`MovedToWindow()`](#movedtowindow)|
|[`MutableCopy()`](#mutablecopy)|
|[`NeedsUpdateConstraints()`](#needsupdateconstraints)|
|[`ObserveValue(NSString,NSObject,NSDictionary,IntPtr)`](#observevaluensstringnsobjectnsdictionaryintptr)|
|[`Paste(NSObject)`](#pastensobject)|
|[`Paste(NSItemProvider[])`](#pastensitemprovider[])|
|[`PerformSelector(Selector)`](#performselectorselector)|
|[`PerformSelector(Selector,NSObject)`](#performselectorselectornsobject)|
|[`PerformSelector(Selector,NSObject,NSObject)`](#performselectorselectornsobjectnsobject)|
|[`PerformSelector(Selector,NSObject,Double,NSString[])`](#performselectorselectornsobjectdoublensstring[])|
|[`PerformSelector(Selector,NSObject,Double)`](#performselectorselectornsobjectdouble)|
|[`PerformSelector(Selector,NSThread,NSObject,Boolean)`](#performselectorselectornsthreadnsobjectboolean)|
|[`PerformSelector(Selector,NSThread,NSObject,Boolean,NSString[])`](#performselectorselectornsthreadnsobjectbooleannsstring[])|
|[`PointInside(CGPoint,UIEvent)`](#pointinsidecgpointuievent)|
|[`PrepareForInterfaceBuilder()`](#prepareforinterfacebuilder)|
|[`PressesBegan(NSSet<UIPress>,UIPressesEvent)`](#pressesbegannsset<uipress>uipressesevent)|
|[`PressesCancelled(NSSet<UIPress>,UIPressesEvent)`](#pressescancellednsset<uipress>uipressesevent)|
|[`PressesChanged(NSSet<UIPress>,UIPressesEvent)`](#presseschangednsset<uipress>uipressesevent)|
|[`PressesEnded(NSSet<UIPress>,UIPressesEvent)`](#pressesendednsset<uipress>uipressesevent)|
|[`ReloadInputViews()`](#reloadinputviews)|
|[`RemoteControlReceived(UIEvent)`](#remotecontrolreceiveduievent)|
|[`RemoveConstraint(NSLayoutConstraint)`](#removeconstraintnslayoutconstraint)|
|[`RemoveConstraints(NSLayoutConstraint[])`](#removeconstraintsnslayoutconstraint[])|
|[`RemoveFromSuperview()`](#removefromsuperview)|
|[`RemoveGestureRecognizer(UIGestureRecognizer)`](#removegesturerecognizeruigesturerecognizer)|
|[`RemoveInteraction(IUIInteraction)`](#removeinteractioniuiinteraction)|
|[`RemoveLayoutGuide(UILayoutGuide)`](#removelayoutguideuilayoutguide)|
|[`RemoveMotionEffect(UIMotionEffect)`](#removemotioneffectuimotioneffect)|
|[`RemoveObserver(NSObject,NSString,IntPtr)`](#removeobservernsobjectnsstringintptr)|
|[`RemoveObserver(NSObject,String,IntPtr)`](#removeobservernsobjectstringintptr)|
|[`RemoveObserver(NSObject,NSString)`](#removeobservernsobjectnsstring)|
|[`RemoveObserver(NSObject,String)`](#removeobservernsobjectstring)|
|[`ResignFirstResponder()`](#resignfirstresponder)|
|[`ResizableSnapshotView(CGRect,Boolean,UIEdgeInsets)`](#resizablesnapshotviewcgrectbooleanuiedgeinsets)|
|[`RespondsToSelector(Selector)`](#respondstoselectorselector)|
|[`RestoreUserActivityState(NSUserActivity)`](#restoreuseractivitystatensuseractivity)|
|[`SafeAreaInsetsDidChange()`](#safeareainsetsdidchange)|
|[`Select(NSObject)`](#selectnsobject)|
|[`SelectAll(NSObject)`](#selectallnsobject)|
|[`SendSubviewToBack(UIView)`](#sendsubviewtobackuiview)|
|[`SetContentCompressionResistancePriority(Single,UILayoutConstraintAxis)`](#setcontentcompressionresistanceprioritysingleuilayoutconstraintaxis)|
|[`SetContentHuggingPriority(Single,UILayoutConstraintAxis)`](#setcontenthuggingprioritysingleuilayoutconstraintaxis)|
|[`SetNativeField(String,NSObject)`](#setnativefieldstringnsobject)|
|[`SetNeedsDisplay()`](#setneedsdisplay)|
|[`SetNeedsDisplayInRect(CGRect)`](#setneedsdisplayinrectcgrect)|
|[`SetNeedsFocusUpdate()`](#setneedsfocusupdate)|
|[`SetNeedsLayout()`](#setneedslayout)|
|[`SetNeedsUpdateConstraints()`](#setneedsupdateconstraints)|
|[`SetNilValueForKey(NSString)`](#setnilvalueforkeynsstring)|
|[`SetValueForKey(NSObject,NSString)`](#setvalueforkeynsobjectnsstring)|
|[`SetValueForKeyPath(NSObject,NSString)`](#setvalueforkeypathnsobjectnsstring)|
|[`SetValueForKeyPath(IntPtr,NSString)`](#setvalueforkeypathintptrnsstring)|
|[`SetValueForUndefinedKey(NSObject,NSString)`](#setvalueforundefinedkeynsobjectnsstring)|
|[`SetValuesForKeysWithDictionary(NSDictionary)`](#setvaluesforkeyswithdictionarynsdictionary)|
|[`ShouldUpdateFocus(UIFocusUpdateContext)`](#shouldupdatefocusuifocusupdatecontext)|
|[`SizeThatFits(CGSize)`](#sizethatfitscgsize)|
|[`SizeToFit()`](#sizetofit)|
|[`SnapshotView(Boolean)`](#snapshotviewboolean)|
|[`StartAnimating()`](#startanimating)|
|[`StopAnimating()`](#stopanimating)|
|[`SubviewAdded(UIView)`](#subviewaddeduiview)|
|[`SystemLayoutSizeFittingSize(CGSize)`](#systemlayoutsizefittingsizecgsize)|
|[`SystemLayoutSizeFittingSize(CGSize,Single,Single)`](#systemlayoutsizefittingsizecgsizesinglesingle)|
|[`TintColorDidChange()`](#tintcolordidchange)|
|[`ToggleBoldface(NSObject)`](#toggleboldfacensobject)|
|[`ToggleItalics(NSObject)`](#toggleitalicsnsobject)|
|[`ToggleUnderline(NSObject)`](#toggleunderlinensobject)|
|[`ToString()`](#tostring)|
|[`TouchesBegan(NSSet,UIEvent)`](#touchesbegannssetuievent)|
|[`TouchesCancelled(NSSet,UIEvent)`](#touchescancellednssetuievent)|
|[`TouchesEnded(NSSet,UIEvent)`](#touchesendednssetuievent)|
|[`TouchesEstimatedPropertiesUpdated(NSSet)`](#touchesestimatedpropertiesupdatednsset)|
|[`TouchesMoved(NSSet,UIEvent)`](#touchesmovednssetuievent)|
|[`TraitCollectionDidChange(UITraitCollection)`](#traitcollectiondidchangeuitraitcollection)|
|[`UpdateConstraints()`](#updateconstraints)|
|[`UpdateConstraintsIfNeeded()`](#updateconstraintsifneeded)|
|[`UpdateFocusIfNeeded()`](#updatefocusifneeded)|
|[`UpdateTextAttributes(UITextAttributesConversionHandler)`](#updatetextattributesuitextattributesconversionhandler)|
|[`UpdateUserActivityState(NSUserActivity)`](#updateuseractivitystatensuseractivity)|
|[`ValidateCommand(UICommand)`](#validatecommanduicommand)|
|[`ValueForKey(NSString)`](#valueforkeynsstring)|
|[`ValueForKeyPath(NSString)`](#valueforkeypathnsstring)|
|[`ValueForUndefinedKey(NSString)`](#valueforundefinedkeynsstring)|
|[`ViewWithTag(nint)`](#viewwithtagnint)|
|[`WillChange(NSKeyValueChange,NSIndexSet,NSString)`](#willchangenskeyvaluechangensindexsetnsstring)|
|[`WillChange(NSString,NSKeyValueSetMutationKind,NSSet)`](#willchangensstringnskeyvaluesetmutationkindnsset)|
|[`WillChangeValue(String)`](#willchangevaluestring)|
|[`WillDrawLayer(CALayer)`](#willdrawlayercalayer)|
|[`WillMoveToSuperview(UIView)`](#willmovetosuperviewuiview)|
|[`WillMoveToWindow(UIWindow)`](#willmovetowindowuiwindow)|
|[`WillRemoveSubview(UIView)`](#willremovesubviewuiview)|

### Protected Methods Summary


|Name|
|---|
|[`BeginInvokeOnMainThread(SendOrPostCallback,Object)`](#begininvokeonmainthreadsendorpostcallbackobject)|
|[`ClearHandle()`](#clearhandle)|
|[`Dispose(Boolean)`](#disposeboolean)|
|[`DrawCore(GeoCanvas)`](#drawcoregeocanvas)|
|[`Finalize()`](#finalize)|
|[`InitializeHandle(IntPtr)`](#initializehandleintptr)|
|[`InitializeHandle(IntPtr,String)`](#initializehandleintptrstring)|
|[`InvokeOnMainThread(SendOrPostCallback,Object)`](#invokeonmainthreadsendorpostcallbackobject)|
|[`MarkDirty()`](#markdirty)|
|[`MarkDirty(Boolean)`](#markdirtyboolean)|
|[`MemberwiseClone()`](#memberwiseclone)|

### Public Events Summary


|Name|Event Arguments|Description|
|---|---|---|
|N/A|N/A|N/A|

## Members Detail

### Public Constructors


|Name|
|---|
|[`TileView()`](#tileview)|
|[`TileView(CGRect)`](#tileviewcgrect)|

### Protected Constructors


### Public Properties

#### `AccessibilityActivationPoint`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`CGPoint`

---
#### `AccessibilityAssistiveTechnologyFocusedIdentifiers`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

NSSet<`NSString`>

---
#### `AccessibilityAttributedHint`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSAttributedString`

---
#### `AccessibilityAttributedLabel`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSAttributedString`

---
#### `AccessibilityAttributedUserInputLabels`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSAttributedString[]`

---
#### `AccessibilityAttributedValue`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSAttributedString`

---
#### `AccessibilityCustomActions`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIAccessibilityCustomAction[]`

---
#### `AccessibilityDragSourceDescriptors`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIAccessibilityLocationDescriptor[]`

---
#### `AccessibilityDropPointDescriptors`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIAccessibilityLocationDescriptor[]`

---
#### `AccessibilityElementsHidden`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `AccessibilityFrame`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`CGRect`

---
#### `AccessibilityHint`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `AccessibilityIdentifier`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `AccessibilityIgnoresInvertColors`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `AccessibilityLabel`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `AccessibilityLanguage`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `AccessibilityNavigationStyle`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIAccessibilityNavigationStyle`

---
#### `AccessibilityPath`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIBezierPath`

---
#### `AccessibilityRespondsToUserInteraction`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `AccessibilityTextualContext`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `AccessibilityTraits`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIAccessibilityTrait`

---
#### `AccessibilityUserInputLabels`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String[]`

---
#### `AccessibilityValue`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `AccessibilityViewIsModal`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `ActivityItemsConfiguration`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`IUIActivityItemsConfigurationReading`

---
#### `AdjustsImageSizeForAccessibilityContentSizeCategory`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `AlignmentRectInsets`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIEdgeInsets`

---
#### `Alpha`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`nfloat`

---
#### `AnimationDuration`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Double`

---
#### `AnimationImages`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIImage[]`

---
#### `AnimationRepeatCount`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`nint`

---
#### `AutoresizingMask`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIViewAutoresizing`

---
#### `AutosizesSubviews`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `BackgroundColor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIColor`

---
#### `BottomAnchor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSLayoutYAxisAnchor`

---
#### `BoundingBox`

**Summary**

   *Gets or sets the bounding box.*

**Remarks**

   *N/A*

**Return Value**

[`RectangleShape`](../ThinkGeo.Core/ThinkGeo.Core.RectangleShape.md)

---
#### `Bounds`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`CGRect`

---
#### `CanBecomeFirstResponder`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `CanBecomeFocused`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `CanResignFirstResponder`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `Center`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`CGPoint`

---
#### `CenterXAnchor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSLayoutXAxisAnchor`

---
#### `CenterYAnchor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSLayoutYAxisAnchor`

---
#### `Class`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Class`

---
#### `ClassHandle`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`IntPtr`

---
#### `ClearsContextBeforeDrawing`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `ClipsToBounds`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `CollisionBoundingPath`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIBezierPath`

---
#### `CollisionBoundsType`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIDynamicItemCollisionBoundsType`

---
#### `ColumnIndex`

**Summary**

   *Gets or sets the index of the column.*

**Remarks**

   *N/A*

**Return Value**

`Int64`

---
#### `Constraints`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSLayoutConstraint[]`

---
#### `ContentMode`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIViewContentMode`

---
#### `ContentScaleFactor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`nfloat`

---
#### `ContentStretch`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`CGRect`

---
#### `CoordinateSpace`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`IUICoordinateSpace`

---
#### `DebugDescription`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `Description`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `DirectionalLayoutMargins`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSDirectionalEdgeInsets`

---
#### `EditingInteractionConfiguration`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIEditingInteractionConfiguration`

---
#### `EffectiveUserInterfaceLayoutDirection`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIUserInterfaceLayoutDirection`

---
#### `Error`

**Summary**

   *User can set the customer excpetion and we can paint that.*

**Remarks**

   *N/A*

**Return Value**

`Exception`

---
#### `ExclusiveTouch`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `FirstBaselineAnchor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSLayoutYAxisAnchor`

---
#### `Focused`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `FocusItemContainer`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`IUIFocusItemContainer`

---
#### `Frame`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`CGRect`

---
#### `GestureRecognizers`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIGestureRecognizer[]`

---
#### `Handle`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`IntPtr`

---
#### `HasAmbiguousLayout`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `HeightAnchor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSLayoutDimension`

---
#### `Hidden`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `Highlighted`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `HighlightedAnimationImages`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIImage[]`

---
#### `HighlightedImage`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIImage`

---
#### `Image`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIImage`

---
#### `ImageSource`

**Summary**

   *Gets or sets the image source.*

**Remarks**

   *N/A*

**Return Value**

[`GeoImage`](../ThinkGeo.Core/ThinkGeo.Core.GeoImage.md)

---
#### `InputAccessoryView`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIView`

---
#### `InputAccessoryViewController`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIInputViewController`

---
#### `InputAssistantItem`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UITextInputAssistantItem`

---
#### `InputView`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIView`

---
#### `InputViewController`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIInputViewController`

---
#### `InsetsLayoutMarginsFromSafeArea`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `Interactions`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`IUIInteraction[]`

---
#### `IntrinsicContentSize`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`CGSize`

---
#### `IsAccessibilityElement`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `IsAnimating`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `IsFirstResponder`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `IsProxy`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `KeyCommands`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIKeyCommand[]`

---
#### `LargeContentImage`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIImage`

---
#### `LargeContentImageInsets`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIEdgeInsets`

---
#### `LargeContentTitle`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `LastBaselineAnchor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSLayoutYAxisAnchor`

---
#### `Layer`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`CALayer`

---
#### `LayoutGuides`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UILayoutGuide[]`

---
#### `LayoutMargins`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIEdgeInsets`

---
#### `LayoutMarginsGuide`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UILayoutGuide`

---
#### `LeadingAnchor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSLayoutXAxisAnchor`

---
#### `LeftAnchor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSLayoutXAxisAnchor`

---
#### `MapUnit`

**Summary**

   *Gets or sets the map unit.*

**Remarks**

   *N/A*

**Return Value**

[`GeographyUnit`](../ThinkGeo.Core/ThinkGeo.Core.GeographyUnit.md)

---
#### `MaskView`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIView`

---
#### `MotionEffects`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIMotionEffect[]`

---
#### `MultipleTouchEnabled`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `NextResponder`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIResponder`

---
#### `Opaque`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `OverrideUserInterfaceStyle`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIUserInterfaceStyle`

---
#### `ParentFocusEnvironment`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`IUIFocusEnvironment`

---
#### `PasteConfiguration`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIPasteConfiguration`

---
#### `PreferredFocusedView`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIView`

---
#### `PreferredFocusEnvironments`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`IUIFocusEnvironment[]`

---
#### `PreferredSymbolConfiguration`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIImageSymbolConfiguration`

---
#### `PreservesSuperviewLayoutMargins`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `ReadableContentGuide`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UILayoutGuide`

---
#### `RestorationIdentifier`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `RetainCount`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`nuint`

---
#### `RightAnchor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSLayoutXAxisAnchor`

---
#### `RowIndex`

**Summary**

   *Gets or sets the index of the row.*

**Remarks**

   *N/A*

**Return Value**

`Int64`

---
#### `SafeAreaInsets`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIEdgeInsets`

---
#### `SafeAreaLayoutGuide`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UILayoutGuide`

---
#### `Scale`

**Summary**

   *Gets or sets the scale.*

**Remarks**

   *N/A*

**Return Value**

`Double`

---
#### `ScalesLargeContentImage`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `Self`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSObject`

---
#### `SemanticContentAttribute`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UISemanticContentAttribute`

---
#### `ShouldGroupAccessibilityChildren`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `ShowsLargeContentViewer`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `Subviews`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIView[]`

---
#### `Superclass`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Class`

---
#### `SuperHandle`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`IntPtr`

---
#### `Superview`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIView`

---
#### `Tag`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`nint`

---
#### `TextInputContextIdentifier`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSString`

---
#### `TextInputMode`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UITextInputMode`

---
#### `TileHeight`

**Summary**

   *Gets or sets the height of the tile.*

**Remarks**

   *N/A*

**Return Value**

`Int32`

---
#### `TileWidth`

**Summary**

   *Gets or sets the width of the tile.*

**Remarks**

   *N/A*

**Return Value**

`Int32`

---
#### `TintAdjustmentMode`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIViewTintAdjustmentMode`

---
#### `TintColor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIColor`

---
#### `TopAnchor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSLayoutYAxisAnchor`

---
#### `TrailingAnchor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSLayoutXAxisAnchor`

---
#### `TraitCollection`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UITraitCollection`

---
#### `Transform`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`CGAffineTransform`

---
#### `Transform3D`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`CATransform3D`

---
#### `TranslatesAutoresizingMaskIntoConstraints`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `UndoManager`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSUndoManager`

---
#### `UserActivity`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSUserActivity`

---
#### `UserInteractionEnabled`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `ViewForBaselineLayout`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIView`

---
#### `ViewForFirstBaselineLayout`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIView`

---
#### `ViewForLastBaselineLayout`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIView`

---
#### `ViewPrintFormatter`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIViewPrintFormatter`

---
#### `WidthAnchor`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSLayoutDimension`

---
#### `Window`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`UIWindow`

---
#### `Zone`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`NSZone`

---
#### `ZoomLevelIndex`

**Summary**

   *Gets or sets the index of the zoom level.*

**Remarks**

   *N/A*

**Return Value**

`Int32`

---

### Protected Properties

#### `CacheUrl`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`String`

---
#### `InFinalizerQueue`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `IsCanceled`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `IsDirectBinding`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `IsDisposed`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `IsRegisteredToggleRef`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---
#### `IsStretch`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

`Boolean`

---

### Public Methods

#### `AccessibilityActivate()`

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
|N/A|N/A|N/A|

---
#### `AccessibilityDecrement()`

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
#### `AccessibilityElementDidBecomeFocused()`

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
#### `AccessibilityElementDidLoseFocus()`

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
#### `AccessibilityElementIsFocused()`

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
|N/A|N/A|N/A|

---
#### `AccessibilityIncrement()`

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
#### `AccessibilityPerformEscape()`

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
|N/A|N/A|N/A|

---
#### `AccessibilityPerformMagicTap()`

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
|N/A|N/A|N/A|

---
#### `AccessibilityScroll(UIAccessibilityScrollDirection)`

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
|direction|`UIAccessibilityScrollDirection`|N/A|

---
#### `ActionForLayer(CALayer,String)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSObject`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|layer|`CALayer`|N/A|
|eventKey|`String`|N/A|

---
#### `Add(UIView)`

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
|view|`UIView`|N/A|

---
#### `AddConstraint(NSLayoutConstraint)`

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
|constraint|`NSLayoutConstraint`|N/A|

---
#### `AddConstraints(NSLayoutConstraint[])`

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
|constraints|`NSLayoutConstraint[]`|N/A|

---
#### `AddGestureRecognizer(UIGestureRecognizer)`

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
|gestureRecognizer|`UIGestureRecognizer`|N/A|

---
#### `AddInteraction(IUIInteraction)`

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
|interaction|`IUIInteraction`|N/A|

---
#### `AddLayoutGuide(UILayoutGuide)`

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
|guide|`UILayoutGuide`|N/A|

---
#### `AddMotionEffect(UIMotionEffect)`

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
|effect|`UIMotionEffect`|N/A|

---
#### `AddObserver(String,NSKeyValueObservingOptions,Action<NSObservedChange>)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`IDisposable`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|key|`String`|N/A|
|options|`NSKeyValueObservingOptions`|N/A|
|observer|Action<`NSObservedChange`>|N/A|

---
#### `AddObserver(NSString,NSKeyValueObservingOptions,Action<NSObservedChange>)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`IDisposable`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|key|`NSString`|N/A|
|options|`NSKeyValueObservingOptions`|N/A|
|observer|Action<`NSObservedChange`>|N/A|

---
#### `AddObserver(NSObject,NSString,NSKeyValueObservingOptions,IntPtr)`

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
|observer|`NSObject`|N/A|
|keyPath|`NSString`|N/A|
|options|`NSKeyValueObservingOptions`|N/A|
|context|`IntPtr`|N/A|

---
#### `AddObserver(NSObject,String,NSKeyValueObservingOptions,IntPtr)`

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
|observer|`NSObject`|N/A|
|keyPath|`String`|N/A|
|options|`NSKeyValueObservingOptions`|N/A|
|context|`IntPtr`|N/A|

---
#### `AddSubview(UIView)`

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
|view|`UIView`|N/A|

---
#### `AddSubviews(UIView[])`

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
|views|`UIView[]`|N/A|

---
#### `AlignmentRectForFrame(CGRect)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`CGRect`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|frame|`CGRect`|N/A|

---
#### `AwakeFromNib()`

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
#### `BecomeFirstResponder()`

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
|N/A|N/A|N/A|

---
#### `BeginInvokeOnMainThread(Selector,NSObject)`

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
|sel|`Selector`|N/A|
|obj|`NSObject`|N/A|

---
#### `BeginInvokeOnMainThread(Action)`

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
|action|`Action`|N/A|

---
#### `BringSubviewToFront(UIView)`

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
|view|`UIView`|N/A|

---
#### `BuildMenu(IUIMenuBuilder)`

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
|builder|`IUIMenuBuilder`|N/A|

---
#### `CanPaste(NSItemProvider[])`

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
|itemProviders|`NSItemProvider[]`|N/A|

---
#### `CanPerform(Selector,NSObject)`

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
|action|`Selector`|N/A|
|withSender|`NSObject`|N/A|

---
#### `Capture(Boolean)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`UIImage`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|afterScreenUpdates|`Boolean`|N/A|

---
#### `ConformsToProtocol(IntPtr)`

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
|protocol|`IntPtr`|N/A|

---
#### `ContentCompressionResistancePriority(UILayoutConstraintAxis)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Single`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|axis|`UILayoutConstraintAxis`|N/A|

---
#### `ContentHuggingPriority(UILayoutConstraintAxis)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`Single`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|axis|`UILayoutConstraintAxis`|N/A|

---
#### `ConvertPointFromCoordinateSpace(CGPoint,IUICoordinateSpace)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`CGPoint`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|point|`CGPoint`|N/A|
|coordinateSpace|`IUICoordinateSpace`|N/A|

---
#### `ConvertPointFromView(CGPoint,UIView)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`CGPoint`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|point|`CGPoint`|N/A|
|fromView|`UIView`|N/A|

---
#### `ConvertPointToCoordinateSpace(CGPoint,IUICoordinateSpace)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`CGPoint`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|point|`CGPoint`|N/A|
|coordinateSpace|`IUICoordinateSpace`|N/A|

---
#### `ConvertPointToView(CGPoint,UIView)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`CGPoint`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|point|`CGPoint`|N/A|
|toView|`UIView`|N/A|

---
#### `ConvertRectFromCoordinateSpace(CGRect,IUICoordinateSpace)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`CGRect`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|rect|`CGRect`|N/A|
|coordinateSpace|`IUICoordinateSpace`|N/A|

---
#### `ConvertRectFromView(CGRect,UIView)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`CGRect`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|rect|`CGRect`|N/A|
|fromView|`UIView`|N/A|

---
#### `ConvertRectToCoordinateSpace(CGRect,IUICoordinateSpace)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`CGRect`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|rect|`CGRect`|N/A|
|coordinateSpace|`IUICoordinateSpace`|N/A|

---
#### `ConvertRectToView(CGRect,UIView)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`CGRect`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|rect|`CGRect`|N/A|
|toView|`UIView`|N/A|

---
#### `Copy(NSObject)`

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
|sender|`NSObject`|N/A|

---
#### `Copy()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSObject`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `Cut(NSObject)`

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
|sender|`NSObject`|N/A|

---
#### `DangerousAutorelease()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSObject`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `DangerousRelease()`

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
#### `DangerousRetain()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSObject`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `DecodeRestorableState(NSCoder)`

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
|coder|`NSCoder`|N/A|

---
#### `Delete(NSObject)`

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
|sender|`NSObject`|N/A|

---
#### `DidChange(NSKeyValueChange,NSIndexSet,NSString)`

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
|changeKind|`NSKeyValueChange`|N/A|
|indexes|`NSIndexSet`|N/A|
|forKey|`NSString`|N/A|

---
#### `DidChange(NSString,NSKeyValueSetMutationKind,NSSet)`

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
|forKey|`NSString`|N/A|
|mutationKind|`NSKeyValueSetMutationKind`|N/A|
|objects|`NSSet`|N/A|

---
#### `DidChangeValue(String)`

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
|forKey|`String`|N/A|

---
#### `DidHintFocusMovement(UIFocusMovementHint)`

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
|hint|`UIFocusMovementHint`|N/A|

---
#### `DidUpdateFocus(UIFocusUpdateContext,UIFocusAnimationCoordinator)`

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
|context|`UIFocusUpdateContext`|N/A|
|coordinator|`UIFocusAnimationCoordinator`|N/A|

---
#### `DisplayLayer(CALayer)`

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
|layer|`CALayer`|N/A|

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
#### `DoesNotRecognizeSelector(Selector)`

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
|sel|`Selector`|N/A|

---
#### `Draw(GeoCanvas)`

**Summary**

   *This method draws the overlay tile with geoCanvas.*

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

---
#### `Draw(CGRect)`

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
|rect|`CGRect`|N/A|

---
#### `DrawLayer(CALayer,CGContext)`

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
|layer|`CALayer`|N/A|
|context|`CGContext`|N/A|

---
#### `DrawRect(CGRect,UIViewPrintFormatter)`

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
|area|`CGRect`|N/A|
|formatter|`UIViewPrintFormatter`|N/A|

---
#### `DrawViewHierarchy(CGRect,Boolean)`

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
|rect|`CGRect`|N/A|
|afterScreenUpdates|`Boolean`|N/A|

---
#### `EncodeRestorableState(NSCoder)`

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
|coder|`NSCoder`|N/A|

---
#### `EncodeTo(NSCoder)`

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
|encoder|`NSCoder`|N/A|

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
#### `Equals(NSObject)`

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
|obj|`NSObject`|N/A|

---
#### `ExchangeSubview(nint,nint)`

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
|atIndex|`nint`|N/A|
|withSubviewAtIndex|`nint`|N/A|

---
#### `ExerciseAmbiguityInLayout()`

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
#### `FrameForAlignmentRect(CGRect)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`CGRect`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|alignmentRect|`CGRect`|N/A|

---
#### `GestureRecognizerShouldBegin(UIGestureRecognizer)`

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
|gestureRecognizer|`UIGestureRecognizer`|N/A|

---
#### `GetConstraintsAffectingLayout(UILayoutConstraintAxis)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSLayoutConstraint[]`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|axis|`UILayoutConstraintAxis`|N/A|

---
#### `GetDictionaryOfValuesFromKeys(NSString[])`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSDictionary`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|keys|`NSString[]`|N/A|

---
#### `GetEnumerator()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`IEnumerator`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `GetFocusItems(CGRect)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`IUIFocusItem[]`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|rect|`CGRect`|N/A|

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
#### `GetMethodForSelector(Selector)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`IntPtr`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|sel|`Selector`|N/A|

---
#### `GetNativeField(String)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSObject`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|`String`|N/A|

---
#### `GetNativeHash()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`nuint`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `GetTargetForAction(Selector,NSObject)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSObject`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|action|`Selector`|N/A|
|sender|`NSObject`|N/A|

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
#### `HitTest(CGPoint,UIEvent)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`UIView`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|point|`CGPoint`|N/A|
|uievent|`UIEvent`|N/A|

---
#### `Init()`

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
#### `InsertSubview(UIView,nint)`

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
|view|`UIView`|N/A|
|atIndex|`nint`|N/A|

---
#### `InsertSubviewAbove(UIView,UIView)`

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
|view|`UIView`|N/A|
|siblingSubview|`UIView`|N/A|

---
#### `InsertSubviewBelow(UIView,UIView)`

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
|view|`UIView`|N/A|
|siblingSubview|`UIView`|N/A|

---
#### `InvalidateIntrinsicContentSize()`

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
#### `Invoke(Action,Double)`

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
|action|`Action`|N/A|
|delay|`Double`|N/A|

---
#### `Invoke(Action,TimeSpan)`

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
|action|`Action`|N/A|
|delay|`TimeSpan`|N/A|

---
#### `InvokeOnMainThread(Selector,NSObject)`

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
|sel|`Selector`|N/A|
|obj|`NSObject`|N/A|

---
#### `InvokeOnMainThread(Action)`

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
|action|`Action`|N/A|

---
#### `IsDescendantOfView(UIView)`

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
|view|`UIView`|N/A|

---
#### `IsEqual(NSObject)`

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
|anObject|`NSObject`|N/A|

---
#### `IsKindOfClass(Class)`

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
|aClass|`Class`|N/A|

---
#### `IsMemberOfClass(Class)`

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
|aClass|`Class`|N/A|

---
#### `LayoutIfNeeded()`

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
#### `LayoutMarginsDidChange()`

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
#### `LayoutSublayersOfLayer(CALayer)`

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
|layer|`CALayer`|N/A|

---
#### `LayoutSubviews()`

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
#### `MakeTextWritingDirectionLeftToRight(NSObject)`

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
|sender|`NSObject`|N/A|

---
#### `MakeTextWritingDirectionRightToLeft(NSObject)`

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
|sender|`NSObject`|N/A|

---
#### `MotionBegan(UIEventSubtype,UIEvent)`

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
|motion|`UIEventSubtype`|N/A|
|evt|`UIEvent`|N/A|

---
#### `MotionCancelled(UIEventSubtype,UIEvent)`

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
|motion|`UIEventSubtype`|N/A|
|evt|`UIEvent`|N/A|

---
#### `MotionEnded(UIEventSubtype,UIEvent)`

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
|motion|`UIEventSubtype`|N/A|
|evt|`UIEvent`|N/A|

---
#### `MovedToSuperview()`

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
#### `MovedToWindow()`

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
#### `MutableCopy()`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSObject`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|N/A|N/A|N/A|

---
#### `NeedsUpdateConstraints()`

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
|N/A|N/A|N/A|

---
#### `ObserveValue(NSString,NSObject,NSDictionary,IntPtr)`

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
|keyPath|`NSString`|N/A|
|ofObject|`NSObject`|N/A|
|change|`NSDictionary`|N/A|
|context|`IntPtr`|N/A|

---
#### `Paste(NSObject)`

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
|sender|`NSObject`|N/A|

---
#### `Paste(NSItemProvider[])`

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
|itemProviders|`NSItemProvider[]`|N/A|

---
#### `PerformSelector(Selector)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSObject`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|aSelector|`Selector`|N/A|

---
#### `PerformSelector(Selector,NSObject)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSObject`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|aSelector|`Selector`|N/A|
|anObject|`NSObject`|N/A|

---
#### `PerformSelector(Selector,NSObject,NSObject)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSObject`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|aSelector|`Selector`|N/A|
|object1|`NSObject`|N/A|
|object2|`NSObject`|N/A|

---
#### `PerformSelector(Selector,NSObject,Double,NSString[])`

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
|selector|`Selector`|N/A|
|withObject|`NSObject`|N/A|
|afterDelay|`Double`|N/A|
|nsRunLoopModes|`NSString[]`|N/A|

---
#### `PerformSelector(Selector,NSObject,Double)`

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
|selector|`Selector`|N/A|
|withObject|`NSObject`|N/A|
|delay|`Double`|N/A|

---
#### `PerformSelector(Selector,NSThread,NSObject,Boolean)`

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
|selector|`Selector`|N/A|
|onThread|`NSThread`|N/A|
|withObject|`NSObject`|N/A|
|waitUntilDone|`Boolean`|N/A|

---
#### `PerformSelector(Selector,NSThread,NSObject,Boolean,NSString[])`

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
|selector|`Selector`|N/A|
|onThread|`NSThread`|N/A|
|withObject|`NSObject`|N/A|
|waitUntilDone|`Boolean`|N/A|
|nsRunLoopModes|`NSString[]`|N/A|

---
#### `PointInside(CGPoint,UIEvent)`

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
|point|`CGPoint`|N/A|
|uievent|`UIEvent`|N/A|

---
#### `PrepareForInterfaceBuilder()`

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
#### `PressesBegan(NSSet<UIPress>,UIPressesEvent)`

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
|presses|NSSet<`UIPress`>|N/A|
|evt|`UIPressesEvent`|N/A|

---
#### `PressesCancelled(NSSet<UIPress>,UIPressesEvent)`

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
|presses|NSSet<`UIPress`>|N/A|
|evt|`UIPressesEvent`|N/A|

---
#### `PressesChanged(NSSet<UIPress>,UIPressesEvent)`

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
|presses|NSSet<`UIPress`>|N/A|
|evt|`UIPressesEvent`|N/A|

---
#### `PressesEnded(NSSet<UIPress>,UIPressesEvent)`

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
|presses|NSSet<`UIPress`>|N/A|
|evt|`UIPressesEvent`|N/A|

---
#### `ReloadInputViews()`

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
#### `RemoteControlReceived(UIEvent)`

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
|theEvent|`UIEvent`|N/A|

---
#### `RemoveConstraint(NSLayoutConstraint)`

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
|constraint|`NSLayoutConstraint`|N/A|

---
#### `RemoveConstraints(NSLayoutConstraint[])`

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
|constraints|`NSLayoutConstraint[]`|N/A|

---
#### `RemoveFromSuperview()`

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
#### `RemoveGestureRecognizer(UIGestureRecognizer)`

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
|gestureRecognizer|`UIGestureRecognizer`|N/A|

---
#### `RemoveInteraction(IUIInteraction)`

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
|interaction|`IUIInteraction`|N/A|

---
#### `RemoveLayoutGuide(UILayoutGuide)`

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
|guide|`UILayoutGuide`|N/A|

---
#### `RemoveMotionEffect(UIMotionEffect)`

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
|effect|`UIMotionEffect`|N/A|

---
#### `RemoveObserver(NSObject,NSString,IntPtr)`

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
|observer|`NSObject`|N/A|
|keyPath|`NSString`|N/A|
|context|`IntPtr`|N/A|

---
#### `RemoveObserver(NSObject,String,IntPtr)`

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
|observer|`NSObject`|N/A|
|keyPath|`String`|N/A|
|context|`IntPtr`|N/A|

---
#### `RemoveObserver(NSObject,NSString)`

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
|observer|`NSObject`|N/A|
|keyPath|`NSString`|N/A|

---
#### `RemoveObserver(NSObject,String)`

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
|observer|`NSObject`|N/A|
|keyPath|`String`|N/A|

---
#### `ResignFirstResponder()`

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
|N/A|N/A|N/A|

---
#### `ResizableSnapshotView(CGRect,Boolean,UIEdgeInsets)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`UIView`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|rect|`CGRect`|N/A|
|afterScreenUpdates|`Boolean`|N/A|
|capInsets|`UIEdgeInsets`|N/A|

---
#### `RespondsToSelector(Selector)`

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
|sel|`Selector`|N/A|

---
#### `RestoreUserActivityState(NSUserActivity)`

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
|activity|`NSUserActivity`|N/A|

---
#### `SafeAreaInsetsDidChange()`

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
#### `Select(NSObject)`

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
|sender|`NSObject`|N/A|

---
#### `SelectAll(NSObject)`

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
|sender|`NSObject`|N/A|

---
#### `SendSubviewToBack(UIView)`

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
|view|`UIView`|N/A|

---
#### `SetContentCompressionResistancePriority(Single,UILayoutConstraintAxis)`

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
|priority|`Single`|N/A|
|axis|`UILayoutConstraintAxis`|N/A|

---
#### `SetContentHuggingPriority(Single,UILayoutConstraintAxis)`

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
|priority|`Single`|N/A|
|axis|`UILayoutConstraintAxis`|N/A|

---
#### `SetNativeField(String,NSObject)`

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
|name|`String`|N/A|
|value|`NSObject`|N/A|

---
#### `SetNeedsDisplay()`

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
#### `SetNeedsDisplayInRect(CGRect)`

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
|rect|`CGRect`|N/A|

---
#### `SetNeedsFocusUpdate()`

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
#### `SetNeedsLayout()`

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
#### `SetNeedsUpdateConstraints()`

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
#### `SetNilValueForKey(NSString)`

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
|key|`NSString`|N/A|

---
#### `SetValueForKey(NSObject,NSString)`

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
|value|`NSObject`|N/A|
|key|`NSString`|N/A|

---
#### `SetValueForKeyPath(NSObject,NSString)`

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
|value|`NSObject`|N/A|
|keyPath|`NSString`|N/A|

---
#### `SetValueForKeyPath(IntPtr,NSString)`

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
|handle|`IntPtr`|N/A|
|keyPath|`NSString`|N/A|

---
#### `SetValueForUndefinedKey(NSObject,NSString)`

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
|value|`NSObject`|N/A|
|undefinedKey|`NSString`|N/A|

---
#### `SetValuesForKeysWithDictionary(NSDictionary)`

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
|keyedValues|`NSDictionary`|N/A|

---
#### `ShouldUpdateFocus(UIFocusUpdateContext)`

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
|context|`UIFocusUpdateContext`|N/A|

---
#### `SizeThatFits(CGSize)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`CGSize`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|size|`CGSize`|N/A|

---
#### `SizeToFit()`

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
#### `SnapshotView(Boolean)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`UIView`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|afterScreenUpdates|`Boolean`|N/A|

---
#### `StartAnimating()`

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
#### `StopAnimating()`

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
#### `SubviewAdded(UIView)`

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
|uiview|`UIView`|N/A|

---
#### `SystemLayoutSizeFittingSize(CGSize)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`CGSize`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|size|`CGSize`|N/A|

---
#### `SystemLayoutSizeFittingSize(CGSize,Single,Single)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`CGSize`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|targetSize|`CGSize`|N/A|
|horizontalFittingPriority|`Single`|N/A|
|verticalFittingPriority|`Single`|N/A|

---
#### `TintColorDidChange()`

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
#### `ToggleBoldface(NSObject)`

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
|sender|`NSObject`|N/A|

---
#### `ToggleItalics(NSObject)`

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
|sender|`NSObject`|N/A|

---
#### `ToggleUnderline(NSObject)`

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
|sender|`NSObject`|N/A|

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
#### `TouchesBegan(NSSet,UIEvent)`

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
|touches|`NSSet`|N/A|
|evt|`UIEvent`|N/A|

---
#### `TouchesCancelled(NSSet,UIEvent)`

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
|touches|`NSSet`|N/A|
|evt|`UIEvent`|N/A|

---
#### `TouchesEnded(NSSet,UIEvent)`

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
|touches|`NSSet`|N/A|
|evt|`UIEvent`|N/A|

---
#### `TouchesEstimatedPropertiesUpdated(NSSet)`

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
|touches|`NSSet`|N/A|

---
#### `TouchesMoved(NSSet,UIEvent)`

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
|touches|`NSSet`|N/A|
|evt|`UIEvent`|N/A|

---
#### `TraitCollectionDidChange(UITraitCollection)`

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
|previousTraitCollection|`UITraitCollection`|N/A|

---
#### `UpdateConstraints()`

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
#### `UpdateConstraintsIfNeeded()`

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
#### `UpdateFocusIfNeeded()`

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
#### `UpdateTextAttributes(UITextAttributesConversionHandler)`

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
|conversionHandler|`UITextAttributesConversionHandler`|N/A|

---
#### `UpdateUserActivityState(NSUserActivity)`

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
|activity|`NSUserActivity`|N/A|

---
#### `ValidateCommand(UICommand)`

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
|command|`UICommand`|N/A|

---
#### `ValueForKey(NSString)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSObject`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|key|`NSString`|N/A|

---
#### `ValueForKeyPath(NSString)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSObject`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|keyPath|`NSString`|N/A|

---
#### `ValueForUndefinedKey(NSString)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`NSObject`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|key|`NSString`|N/A|

---
#### `ViewWithTag(nint)`

**Summary**

   *N/A*

**Remarks**

   *N/A*

**Return Value**

|Type|Description|
|---|---|
|`UIView`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|tag|`nint`|N/A|

---
#### `WillChange(NSKeyValueChange,NSIndexSet,NSString)`

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
|changeKind|`NSKeyValueChange`|N/A|
|indexes|`NSIndexSet`|N/A|
|forKey|`NSString`|N/A|

---
#### `WillChange(NSString,NSKeyValueSetMutationKind,NSSet)`

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
|forKey|`NSString`|N/A|
|mutationKind|`NSKeyValueSetMutationKind`|N/A|
|objects|`NSSet`|N/A|

---
#### `WillChangeValue(String)`

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
|forKey|`String`|N/A|

---
#### `WillDrawLayer(CALayer)`

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
|layer|`CALayer`|N/A|

---
#### `WillMoveToSuperview(UIView)`

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
|newsuper|`UIView`|N/A|

---
#### `WillMoveToWindow(UIWindow)`

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
|window|`UIWindow`|N/A|

---
#### `WillRemoveSubview(UIView)`

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
|uiview|`UIView`|N/A|

---

### Protected Methods

#### `BeginInvokeOnMainThread(SendOrPostCallback,Object)`

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
|cb|`SendOrPostCallback`|N/A|
|state|`Object`|N/A|

---
#### `ClearHandle()`

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

   *Releases the resources used by the UIImageView object.*

**Remarks**

   *This Dispose method releases the resources used by the UIImageView class.This method is called by both the Dispose() method and the object finalizer (Finalize).    When invoked by the Dispose method, the parameter disposing  is set to  and any managed object references that this object holds are also disposed or released;  when invoked by the object finalizer, on the finalizer thread the value is set to . Calling the Dispose method when you are finished using the UIImageView ensures that all external resources used by this managed object are released as soon as possible.  Once you have invoked the Dispose method, the object is no longer useful and you should no longer make any calls to it.  For more information on how to override this method and on the Dispose/IDisposable pattern, read the ``Implementing a Dispose Method'' document at http:msdn.microsoft.com/en-us/library/fs2xkftw.aspx*

**Return Value**

|Type|Description|
|---|---|
|`Void`|N/A|

**Parameters**

|Name|Type|Description|
|---|---|---|
|disposing|`Boolean`|If set to , the method is invoked directly and will dispose manage and unmanaged resources;   If set to  the method is being called by the garbage collector finalizer and should only release unmanaged resources.|

---
#### `DrawCore(GeoCanvas)`

**Summary**

   *This method draws the overlay tile with geoCanvas.*

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
#### `InitializeHandle(IntPtr)`

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
|handle|`IntPtr`|N/A|

---
#### `InitializeHandle(IntPtr,String)`

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
|handle|`IntPtr`|N/A|
|initSelector|`String`|N/A|

---
#### `InvokeOnMainThread(SendOrPostCallback,Object)`

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
|cb|`SendOrPostCallback`|N/A|
|state|`Object`|N/A|

---
#### `MarkDirty()`

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
#### `MarkDirty(Boolean)`

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
|allowCustomTypes|`Boolean`|N/A|

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

### Public Events


