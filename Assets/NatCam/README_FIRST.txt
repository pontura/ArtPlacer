NatCam v1.2b5

FEATURES:
+ Completely rebuilt API to have a more object-oriented programming pattern, and to be much cleaner.
+ Immediate native-to-managed callbacks, deprecated UnitySendMessage for MonoPInvokeCallback with function pointers.
+ Added NativePreviewCallback and NatCamNativeInterface.EnableComponentUpdate() to get access to the raw luma (Y) and chroma (UV) buffers from the camera.
+ Added the NatCam rendering pipeline to NatCam iOS.
+ Added NatCamNativeInterface.DisableRenderingPipeline() to disable NatCam's native rendering pipeline.
+ Added NatCamPreviewGestures component for easy detection of focusing and zooming gestures.
+ Added UnitygramBase component for quickly implementing NatCam.
+ Removed NatCam.AutoTapToFocus, NatCam.AutoPinchToZoom.
+ Camera preview is now accessible through NatCam.Preview and returns a Texture (not Texture2D).
+ Camera switching on iOS and Android is now stable and responsive.
+ Added SetFramerate() in the DeviceCamera class and FrameratePreset enum.
+ Added HorizontalFOV, VerticalFOV, IsTorchSupported, IsFlashSupported, and IsZoomSupported in the DeviceCamera class.
+ Added DeviceCamera.Cameras list of cameras on the device, not just DeviceCamera.Front/RearCamera.
+ Added ability to specify NatCam interface (native or fallback) and NatCamInterface enum.
+ Added verboseMode switch to NatCam.Initialize() for debugging.
+ Added NatCam.RequestBarcodeDetection(), BarcodeRequest struct.
+ Added ability to use bitwise operators to request multiple barcode formats when creating barcode detection requests.
+ Added NatCam.HasPermissions to check if the app has camera permissions.
+ Deprecated DeviceCamera.SupportedResolutions and Resolution struct. Use ResolutionPreset instead.
+ Deprecated CapturePhoto overloads for NatCam.CapturePhoto(params PhotoCaptureCallback[] callbacks).
+ Removed '#if CALLIGRAPHY_DOC_GEN_MODE' conditional directives making code much cleaner.
+ Fix camera preview lagging on iOS.
+ OpenCV PreviewMatrix now updates from the native pixel buffer. This gives some memory savings and performance increase.
+ Captured photo is now the highest resolution that the camera supports by default.
+ Captured photo is now RGBA32 format. This means you can use Get/SetPixels(s), EncodeToJPG/PNG, and Apply.
+ Preview is now RGBA32 format on both iOS and Android.
+ Fixed "Error Creating Optimization Context" when using Readable preview on Galaxy S6 and Galaxy Tab.
+ Fixed rare scan line jitter when NatCam corrects padding with Readable preview on some Android devices.
+ Added ALLOCATE_NEW_PHOTO_TEXTURES macro for optimizing memory usage.
+ Added ALLOCATE_NEW_FRAME_TEXTURES macro for optimizing memory usage.
+ Fixed Android crash on Stop().
+ Removed NatCam detectTouchesForFocus and detectPinchToZoom for NatCamPreviewGestures component.
+ Fixed error when LoadLevel is called--something along the lines of "SendMessage: NatCamHelper not found".
+ Added editor-serialized variables for Unitygram example. Now, you can set Unitygram's variables from the editor instead of code.
+ Unitygram example is now a camera app featuring photo capture with flash, switching cameras, and barcode detection.
+ Automatically link required frameworks on iOS.
+ Deprecated OPTIMIZATION_USE_NATIVE_BUFFER macro, and as a result, direct support for Unity 5.2 has been stopped.
+ NatCamPreviewUIPanelScaler and NatCamPreviewUIPanelZoomer have been renamed to NatCamPreviewScaler and NatCamPreviewZoomer respectively.
+ Deprecated NatCam.Stop() for NatCam.Release().
+ Fixed NatCam.Release() (formerly 'Stop') being ignored when NatCam.Pause() is called immediately before.
+ Fixed NatCamPreviewScaler not correctly scaling HD and FullHD preview on iOS.
+ Fixed rotation and skewing when using CapturedPhoto with OpenCV.
+ Fixed memory leak when calling Release() (formerly named 'Stop') and Initialize() several times on iOS.
+ Fixed occasional tearing when running very high resolution previews on iOS and Android.
+ Completely rebuilt the documentation.
+ Added Easter Eggs on iOS.

FIXES TO BETA 4:
+ Sped up NatCam.CapturedPhoto() on Android.
+ Captured photo format was changed from RGB24 to RGBA32.
+ Fixed very rare crash when running the camera preview on Android.
+ Fixed crash on iPad when NatCam is restarted after NatCam.Release() is called.

KNOWN ISSUES:
- Due to a driver issue, NatCam is incompatible with Galaxy Nexus 1.

QUICK TIPS:
- Please peruse the included scripting reference under Editor>NatCam>Scripting Reference.
- The meaning of 'Readable preview' has changed significantly. For more info, look at the documentation under Enumerations>PreviewType>Readable.
- If you face build errors when building to Android because of duplicate google-play-services.jar files, delete the one with NatCam.
- To use the OpenCV support wrapper, uncomment "#define OPENCV_DEVELOPER_MODE" in NatCam.cs, NatCamNativeInterface.cs, and NatCamFallbackInterface.cs. This will give you access to NatCam.PreviewMatrix.
- To use the native preview update callbacks for your native code, check the documentation under NatCamNativeInterface>OnNativePreviewUpdate.
- For a quick start, check out the example scene and the Readme.pdf. The scripting reference is also very rich in code examples.
- To discuss or report an issue, visit Unity forums here: http://forum.unity3d.com/threads/natcam-device-camera-api.374690/#post-2464791 or email me at olokobayusuf@gmail.com.
- On iOS, NatCam requires iOS 7 and up.
- On Android, NatCam requires API Level 16 and up.


Thank you very much!