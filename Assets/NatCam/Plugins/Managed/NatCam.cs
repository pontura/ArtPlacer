/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

//#define OPENCV_DEVELOPER_MODE //Uncomment this to have access to the PreviewMatrix OpenCV Matrix

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NatCamU.Internals;
using Native = NatCamU.Internals.NatCamNativeInterface;
using Fallback = NatCamU.Internals.NatCamFallbackInterface;
using Ext = NatCamU.Internals.NatCamExtensions;

#if OPENCV_DEVELOPER_MODE
using OpenCVForUnity;
#endif

namespace NatCamU {
    
    [NCDoc(7)]
    public static class NatCam {
        
        #region ---Events---
        ///<summary>
        ///Event fired when NatCam receives captured photos
        ///</summary>
        [NCDoc(0)] public static event PhotoCallback OnPhotoCapture;
        ///<summary>
        ///Event fired when NatCam detects barcodes
        ///</summary>
        [NCDoc(1)] public static event BarcodeCallback OnBarcodeDetect;
        ///<summary>
        ///Event fired when the preview starts
        ///</summary>
        [NCDoc(2)] public static event PreviewCallback OnPreviewStart;
        ///<summary>
        ///Event fired on each camera preview update
        ///</summary>
        [NCDoc(3)] public static event PreviewCallback OnPreviewUpdate;
        #endregion
        
        
        #region ---Preview Vars---
        ///<summary>
        ///The camera preview as a Texture
        ///</summary>
        [NCDoc(4)] public static Texture Preview {
            get {
                return mInterface == NatCamInterface.NativeInterface ? (Texture)Native.Preview : (Texture)Fallback.Preview;
            }
        }
        ///<summary>
        ///The current camera preview frame
        ///</summary>
        [NCDoc(0, 5)] public static Texture2D PreviewFrame {
            get {
                return mInterface == NatCamInterface.NativeInterface ? Native.PreviewFrame : Fallback.PreviewFrame;
            }
        }
        #if OPENCV_DEVELOPER_MODE
        ///<summary>
        ///The camera preview as an OpenCV Matrix
        ///</summary>
        [NCDoc(1, 6)] [NCCode(6)] public static Mat PreviewMatrix {
            get {
                return mInterface == NatCamInterface.NativeInterface ? Native.PreviewMatrix : Fallback.PreviewMatrix;
            }
        }
        #endif
        #endregion
        
        
        #region ---Public Op Vars---
        ///<summary>
        ///The backing interface NatCam was initialized with
        ///</summary>
        [NCDoc(129)] public static NatCamInterface mInterface {get; private set;}
        ///<summary>
        ///Get or set the active camera.
        ///</summary>
        [NCDoc(12, 119)] [NCRef(11)] [NCCode(4)] public static DeviceCamera ActiveCamera {
            get {
                return mInterface == NatCamInterface.NativeInterface ? Native.mActiveCamera : Fallback.mActiveCamera;
            }
            set {
                if (value == ActiveCamera) {
                    "Camera is already active".Warn();
                    return;
                }
                if (mInterface ==  NatCamInterface.NativeInterface) {
                    Native.mActiveCamera = value;
                    if (IsPlaying) Native.SwitchActiveCamera();
                    #if UNITY_ANDROID
                    else Native.A.Call("SetSessionCamera", (int)value);
                    #endif
                }
                else {
                    Fallback.mActiveCamera = value;
                    if (IsPlaying) Fallback.SwitchActiveCamera();
                }
            }
        }
        ///<summary>
        ///Get or set the photo save mode for subsequent captured photos
        ///</summary>
        [NCDoc(18, 130)] [NCRef(16)] public static PhotoSaveMode PhotoSaveMode {
            get {
                if (mInterface == NatCamInterface.FallbackInterface) {
                    "Photo save mode is not implemented on the fallback interface".Warn();
                    return PhotoSaveMode.DoNotSave;
                }
                return (PhotoSaveMode)Native.mPhotoSaveMode;
            }
            set {
                if (mInterface == NatCamInterface.NativeInterface) Native.SetSaveMode((int)value);
                else "Cannot set photo save mode on fallback interface".Warn();
            }
        }
        ///<summary>
        ///Is this device or platform supporrted?
        ///</summary>
        [NCDoc(122)] public static bool IsSupportedPlatform {
            get {
                return Native.supportedPlatform;
            }
        }
        ///<summary>
        ///Is the preview running?
        ///</summary>
        [NCDoc(123)] public static bool IsPlaying {
            get {
                return mInterface == NatCamInterface.NativeInterface ? Native.isPlaying : Fallback.Preview != null ? Fallback.Preview.isPlaying : false;
            }
        }
        ///<summary>
        ///Does the app has permissions to use the camera? This must be true for NatCam to work properly.
        ///</summary>
        [NCDoc(23, 151)] [NCCode(19)] public static bool HasPermissions {
            get {
                return Native.HasPermission();
            }
        }
        #endregion
        
        
        #region ---Public Ops---
        
        ///<summary>
        ///Execute the supplied callback when the camera preview has started
        ///</summary>
        [NCDoc(19, 131)] [NCRef(15)] [NCCode(16)]
        public static void ExecuteOnPreviewStart (params PreviewCallback[] callbacks) {
            //Check that the OnPreviewStart event hasn't been fired
            if ((mInterface == NatCamInterface.NativeInterface && Preview == null) || (mInterface == NatCamInterface.FallbackInterface && !Fallback.FirstFrameReceived)) {
                for (int i = 0; i < callbacks.Length; i++) {
                    PreviewCallback callback = callbacks[i];
                    PreviewCallback wrapper = null;
                    wrapper = () => {
                        if (callback != null) callback();
                        OnPreviewStart -= wrapper;
                    };
                    OnPreviewStart += wrapper;
                }
            }
            //The preview has already started
            else {
                for (int i = 0; i < callbacks.Length; i++) {
                    if (callbacks[i] != null) callbacks[i]();
                }
            }
        }
        
        ///<summary>
        ///Initialize NatCam
        ///</summary>
        [NCDoc(17, 128)] [NCRef(10)] [NCCode(0)] [NCCode(2)]
        public static void Initialize (NatCamInterface Interface = NatCamInterface.NativeInterface, PreviewType PreviewType = PreviewType.NonReadable, Switch BarcodeDetection = Switch.Off, Switch Verbose = Switch.Off) {
            Ext.SetOptions(Verbose == Switch.On);
            mInterface = Interface;
            if (Interface == NatCamInterface.NativeInterface && !IsSupportedPlatform) {
                "Running on an unsupported platform or a device without cameras. Falling back to Fallback".Warn();
                mInterface = NatCamInterface.FallbackInterface;
            }
            switch (mInterface) {
                case NatCamInterface.NativeInterface:
                    Native.Initialize(PreviewType, BarcodeDetection == Switch.On, Verbose == Switch.On);
                break;
                case NatCamInterface.FallbackInterface:
                    Fallback.Initialize();
                break;
                default:
                    goto case NatCamInterface.FallbackInterface;
            }
        }
        
        ///<summary>
        ///Start the camera preview
        ///</summary>
        [NCDoc(113)] [NCRef(11)]
        public static void Play (DeviceCamera camera = null) {
            switch (mInterface) {
                case NatCamInterface.NativeInterface:
                    if (camera != null) Native.mActiveCamera = camera;
                    Native.Play();
                break;
                case NatCamInterface.FallbackInterface:
                    if (camera != null) Fallback.mActiveCamera = camera;
                    Fallback.Play();
                break;
            }
        }
        
        ///<summary>
        ///Pause the camera preview
        ///</summary>
        [NCDoc(114)]
        public static void Pause () {
            switch (mInterface) {
                case NatCamInterface.NativeInterface:
                    Native.Pause();
                break;
                case NatCamInterface.FallbackInterface:
                    Fallback.Pause();
                break;
            }
        }
        
        ///<summary>
        ///Stop NatCam and release all resources
        ///</summary>
        [NCDoc(115)] [NCCode(11)]
        public static void Release () {
            DeviceCamera.Reset();
            switch (mInterface) {
                case NatCamInterface.NativeInterface:
                    Native.Release();
                break;
                case NatCamInterface.FallbackInterface:
                    Fallback.Release();
                break;
            }
        }
        
        ///<summary>
        ///Capture a photo
        ///</summary>
        [NCDoc(15, 126)] [NCRef(12)] [NCCode(13)] [NCCode(14)] [NCCode(15)]
        public static void CapturePhoto (params PhotoCallback[] callbacks) {
            if (!IsPlaying) {
                "Cannot capture photo when session is not running".Warn();
                return;
            }
            if (callbacks.Length == 0 && OnPhotoCapture == null) {
                "Cannot capture photo because there is no callback subscribed".Warn();
                return;
            }
            else if (callbacks.Length > 0) {
                for (int i = 0; i < callbacks.Length; i++) {
                    PhotoCallback callback = callbacks[i];
                    PhotoCallback captureWrapper = null;
                    captureWrapper = (Texture2D photo) => {
                        if (callback != null) callback (photo);
                        OnPhotoCapture -= captureWrapper;
                    };
                    OnPhotoCapture += captureWrapper;
                }
            }
            if (mInterface == NatCamInterface.NativeInterface) Native.CapturePhoto();
            else Fallback.CapturePhoto();
        }
        
        ///<summary>
        ///Request to be notified of a barcode
        ///</summary>
        [NCDoc(16, 127)] [NCRef(13)] [NCCode(12)]
        public static void RequestBarcode (params BarcodeRequest[] requests) {
            if (mInterface == NatCamInterface.FallbackInterface) {
                "Cannot request barcode detection on fallback interface".Warn();
                return;
            }
            if (!Native.mMachineReadableCodeDetection) {
                "NatCam was initialized without preparing for barcode Detection. Ignoring call".Warn();
                return;
            }
            if (requests.Length == 0) {
                "Cannot request barcode with no requests supplied".Warn();
                return;
            }
            for (int i = 0; i < requests.Length; i++) {
                BarcodeRequest req = requests[i];
                BarcodeCallback temp = null;
                temp  = delegate (List<Barcode> codes) {
                    codes = codes.Where(code => (code.format & req.format) > 0).ToList();
                    if (codes.Count > 0) {
                        if (req.callback != null) req.callback (codes); //Null checking kills none
                        if (req.unsubscribeOnceDetected) OnBarcodeDetect -= temp;
                    }
                };
                OnBarcodeDetect += temp;
            }
        }
        #endregion
        
        
        #region ---Private Ops---
        
        public static void PropagateStart () {
            if (OnPreviewStart != null) OnPreviewStart();
        }
        public static void PropagateUpdate () {
            if (OnPreviewUpdate != null) OnPreviewUpdate();
        }
        public static void PropagatePhoto () {
            if (OnPhotoCapture != null) OnPhotoCapture(mInterface == NatCamInterface.NativeInterface ? Native.Photo : Fallback.Photo);
        }
        public static void PropagateMRCodes (List<Barcode> codes) {
            if (OnBarcodeDetect != null) OnBarcodeDetect(codes);
        }
        public static void SetApplicationFocus (bool focus) {
            if (mInterface == NatCamInterface.NativeInterface) Native.SetApplicationFocus(focus);
        }
        #endregion
    }
}