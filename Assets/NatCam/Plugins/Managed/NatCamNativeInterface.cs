/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

//#define NATCAM_DEVELOPER_MODE //Uncomment this to have access to EnableComponentUpdate() and DisableRenderingPipeline()
//#define OPENCV_DEVELOPER_MODE //Uncomment this to have access to the PreviewMatrix OpenCV Matrix

#define ALLOCATE_NEW_PHOTO_TEXTURES //Comment this to make NatCam reuse the photo texture it has created instead of allocating new memory
#define ALLOCATE_NEW_FRAME_TEXTURES //Comment this to make NatCam reuse the preview frame texture it has created instead of allocating new memory

#pragma warning disable 0414, 0067

using AOT;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Ext = NatCamU.Internals.NatCamExtensions;

#if OPENCV_DEVELOPER_MODE
using OpenCVForUnity;
#endif

namespace NatCamU {
    
    namespace Internals {
        
        [NCDoc(102)]
        public static class NatCamNativeInterface {
            
            #region ---Publics---
            
            [NCDoc(6, 103)] [NCRef(14)] [NCCode(5)] public static event NativePreviewCallback OnNativePreviewUpdate;
            
            public static Texture2D Photo;
            [NCDoc(20, 134)] public static Texture2D Preview {get; private set;}
            public static Texture2D PreviewFrame {
                get {
                    if (!mReadablePreview) return null;
                    #if ALLOCATE_NEW_FRAME_TEXTURES
                    mPreviewFrame = new Texture2D(Preview.width, Preview.height, Preview.format, false, false);
                    #else
                    if (mPreviewFrame == null) mPreviewFrame = new Texture2D(Preview.width, Preview.height, Preview.format, false, false);
                    else if (mPreviewFrame.width != Preview.width || mPreviewFrame.height != Preview.height) { //Realloc if size is different
                        MonoBehaviour.Destroy(mPreviewFrame);
                        mPreviewFrame = new Texture2D(Preview.width, Preview.height, Preview.format, false, false);
                    }
                    #endif
                    IntPtr RGBAPtr;
                    int RGBAS;
                    PreviewBuffer(out RGBAPtr, out RGBAS);
                    if (RGBAPtr == IntPtr.Zero || RGBAS == 0) {
                        "Unable to retrieve preview frame from native layer".Warn();
                        return null;
                    }
                    mPreviewFrame.LoadRawTextureData(RGBAPtr, RGBAS);
                    mPreviewFrame.Apply();
                    return mPreviewFrame;
                }
            }
            #if OPENCV_DEVELOPER_MODE
            public static Mat PreviewMatrix;
            #endif
            
            public static int mActiveCamera = -1;
            public static bool mMachineReadableCodeDetection {get; private set;}
            public static bool supportedPlatform {
                get {
                    bool supportedPlatform = false;
                    #if (UNITY_IOS && !UNITY_EDITOR) || (UNITY_ANDROID && !UNITY_EDITOR)
                        if (WebCamTexture.devices.Length > 0) {
                            supportedPlatform = true;
                        }
                    #endif
                    return supportedPlatform;
                }
            }
            private static bool mReadablePreview;
            public static int mPhotoSaveMode;
            public static bool isPlaying;
            #endregion
            
            
            #region ---Privates---
            
            private static NatCamHelper listener;
            private static Texture2D mPreviewFrame;
            private static bool mVerbose;
            private static bool rearCamera {get {return NatCam.ActiveCamera == null || NatCam.ActiveCamera.Facing == Facing.Rear;}}
            #endregion
            
            
            #region ---Public Ops---
            
            public static void Initialize (PreviewType previewType, bool mrDetection, bool verbose) {
                //Set state
                mReadablePreview = previewType == PreviewType.Readable;
                mPhotoSaveMode = (int)PhotoSaveMode.DoNotSave;
                mMachineReadableCodeDetection = mrDetection;
                mVerbose = verbose;
                //Helper
                listener = new GameObject("NatCamHelper").AddComponent<NatCamHelper>();
                //Dispatch
                NatCamDispatch.Prepare(); //DEBUG //SGX540 BUG
                //Register callbacks
                RegisterCallbacks(Render, Update, ComponentUpdate, UpdatePhoto, UpdateCode);
                //Inspect device
                InspectDeviceCameras();
            }
            
            public static void Play () {
                #if UNITY_IOS
                PlayPreview(mActiveCamera);
                #elif UNITY_ANDROID
                A.Call("PlayPreview", mActiveCamera);
                #endif
                isPlaying = true;
            }
            
            public static void Pause () {
                #if UNITY_IOS
                PausePreview();
                #elif UNITY_ANDROID
                A.Call("PausePreview");
                #endif
                isPlaying = false;
            }
            
            public static void Release () { //FIX //Calling Destory on listener calls this again
                #if !ALLOCATE_NEW_FRAME_TEXTURES //NatCam retains ownership if we aren't reallocating
                if (Photo != null) MonoBehaviour.Destroy(Photo); Photo = null;
                #endif
                #if !ALLOCATE_NEW_PHOTO_TEXTURES //NatCam retains ownership if we aren't reallocating
                if (mPreviewFrame != null) MonoBehaviour.Destroy(mPreviewFrame); mPreviewFrame = null;
                #endif
                if (Preview != null) MonoBehaviour.Destroy(Preview); Preview = null;
                #if OPENCV_DEVELOPER_MODE
                if (PreviewMatrix != null) PreviewMatrix.release(); PreviewMatrix = null;
                #endif
                mActiveCamera = -1;
                mPhotoSaveMode = 0;
                if (listener != null) {
                    listener.WillDestroyMe();
                    MonoBehaviour.Destroy(listener.gameObject); 
                }
                listener = null;
                #if UNITY_IOS
                TerminateOperations();
                #elif UNITY_ANDROID
                A.Call("TerminateOperations");
                NCNA = null;
                #endif
                isPlaying = false;
            }
            
            public static void SwitchActiveCamera () {
                #if UNITY_IOS
                SwitchCamera(mActiveCamera);
                #elif UNITY_ANDROID
                A.Call("SwitchCamera", mActiveCamera);
                #endif
            }
            
            public static void CapturePhoto () {
                #if UNITY_IOS
                CaptureStill();
                #elif UNITY_ANDROID
                A.Call("CaptureStill");
                #endif
            }
            
            public static void SetSaveMode (int saveMode) {
                mPhotoSaveMode = saveMode;
                #if UNITY_IOS
                SetPhotoSaveMode(mPhotoSaveMode);
                #elif UNITY_ANDROID
                A.Call("SetPhotoSaveMode", mPhotoSaveMode);
                #endif
            }
            
            public static bool HasPermission () {
                #if UNITY_IOS && !UNITY_EDITOR
                return HasPermissions();
                #elif UNITY_ANDROID && !UNITY_EDITOR
                bool support = false;
                using (AndroidJavaClass NCNAClass = new AndroidJavaClass("com.yusufolokoba.natcam.NatCamNativeActivity")) {
                    if (NCNAClass != null) {
                        support = NCNAClass.CallStatic<bool>("HasPermissions");
                    }
                }
                return support;
                #else
                return true; //Unity will automatically request and get permissions for WebCamTexture, so we're safe
                #endif
            }
            
            public static void SetApplicationFocus (bool state) {
                #if UNITY_ANDROID
                if (state) A.Call("SuspendProcess");
                else A.Call("ResumeProcess", isPlaying);
                #endif
            }
            #endregion
                        
            
            #region ---Initialization---
            
            private static void InspectDeviceCameras () {
                #if UNITY_IOS
                InspectDevice(mReadablePreview, mMachineReadableCodeDetection, mVerbose);
                #elif UNITY_ANDROID
                A.Call("InspectDevice", mReadablePreview, mMachineReadableCodeDetection, mVerbose);
                #endif
            }
            #endregion
            
            
            #region ---Native Callbacks---
            
            [MonoPInvokeCallback(typeof(RenderCallback))]
            private static void Render (int request) {
                "Dispatching render callback request".Assert();
                //Dispatch on the main thread
                NatCamDispatch.Dispatch(() => {
                    //Orientation
                    float rotation, flip;
                    Ext.PreviewOrientation(rearCamera, false, out rotation, out flip);
                    if (request != 3) {
                        SetRotation(rotation, flip);
                        #if UNITY_ANDROID
                        A.Call("SetRotation", rotation, flip);
                        #endif
                    }
                    //Invocation
                    ("Native requested callback "+request+" on render thread").Assert();
                    #if UNITY_IOS
                        #pragma warning disable 0618
                        GL.IssuePluginEvent(request);
                        #pragma warning restore 0618
                    #elif UNITY_ANDROID
                        GL.IssuePluginEvent(NatCamNativeCallback(), request);
                    #endif
                    //Release
                    if (request == 3) NatCamDispatch.Release();
                });
            }
            
            [MonoPInvokeCallback(typeof(UpdateCallback))]
            private static void Update (IntPtr RGBA32GPUPtr, IntPtr RGBA32Ptr, int width, int height, int size) {
                //Dispatch on main thread
                NatCamDispatch.Dispatch(() => {
                    Ext.Assert("NatCamU received native update: " + string.Format("ptr<{0}>, ptr<{1}>, {2}, {3}, {4}", RGBA32GPUPtr.ToInt32().ToString(), RGBA32Ptr.ToInt32().ToString(), width.ToString(), height.ToString(), size.ToString()));
                    //Initialization
                    if (Preview == null) {
                        Preview = Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, false, RGBA32GPUPtr);
                        NatCam.PropagateStart();
                    }
                    //Size checking
                    if (Preview.width * Preview.height != width * height) {
                        Preview.Resize(width, height, Preview.format, false);
                        NatCam.PropagateStart();
                    }
                    //Update
                    Preview.UpdateExternalTexture(RGBA32GPUPtr);
                    //Propagation
                    if (OnNativePreviewUpdate != null) {
                        OnNativePreviewUpdate(ComponentBuffer.RGBA32GPU, RGBA32GPUPtr, width, height, size);
                        if (mReadablePreview) OnNativePreviewUpdate(ComponentBuffer.RGBA32, RGBA32Ptr, width, height, size);
                    }
                    //OpenCV
                    #if OPENCV_DEVELOPER_MODE
                        if (mReadablePreview) {
                            if (PreviewMatrix == null) PreviewMatrix = new Mat(new Size(width, height), CvType.CV_8UC4);
                            if (PreviewMatrix.cols() != width || PreviewMatrix.rows() != height) Imgproc.resize(PreviewMatrix, PreviewMatrix, new Size(width, height));
                            Utils.copyToMat(RGBA32Ptr, PreviewMatrix);
                        }
                    #endif
                    //Propagation
                    NatCam.PropagateUpdate();
                });
            }
            
            [MonoPInvokeCallback(typeof(ComponentUpdateCallback))]
            private static void ComponentUpdate (IntPtr Y4Ptr, IntPtr UV2Ptr, IntPtr Y4GPUPtr, int Y4W, int Y4H, int Y4S, int UV2W, int UV2H, int UV2S) {
                //Dispatch on main thread
                NatCamDispatch.Dispatch(() => {
                    //Propagation
                    if (OnNativePreviewUpdate != null) {
                        OnNativePreviewUpdate(ComponentBuffer.Y4, Y4Ptr, Y4W, Y4H, Y4S);
                        OnNativePreviewUpdate(ComponentBuffer.UV2, UV2Ptr, UV2W, UV2H, UV2S);
                        OnNativePreviewUpdate(ComponentBuffer.Y4GPU, Y4GPUPtr, Y4W, Y4H, Y4S);
                    }
                });
            }
            
            [MonoPInvokeCallback(typeof(UpdatePhotoCallback))]
            private static void UpdatePhoto (IntPtr JPGPtr, int width, int height, int size) {
                //Dispatch on main thread
                NatCamDispatch.Dispatch(() => {
                    if (JPGPtr == IntPtr.Zero) {
                        "Failed to retrieve captured photo from device".Warn();
                        return;
                    }
                    #if ALLOCATE_NEW_PHOTO_TEXTURES
                        Photo = new Texture2D(width, height, TextureFormat.RGBA32, false);
                    #else
                        if (Photo == null) Photo = new Texture2D(width, height, TextureFormat.RGBA32, false);
                        else if (Photo.width != width || Photo.height != height) { //Realloc if size is different
                            MonoBehaviour.Destroy(Photo);
                            Photo = new Texture2D(width, height, TextureFormat.RGBA32, false);
                        }
                    #endif
                    Photo.LoadRawTextureData(JPGPtr, size);
                    Photo.Apply();
                    ReleasePhotoBuffer();
                    NatCam.PropagatePhoto();
                });
            }
            
            [MonoPInvokeCallback(typeof(UpdateCodeCallback))]
            private static void UpdateCode (string code) {
                //Dispatch on main thread
                NatCamDispatch.Dispatch(() => {
                    string[] codeTokens = code.Split(';');
                    List<Barcode> codes = new List<Barcode>();
                    for (int i = 0; i < codeTokens.Length; i++) {
                        string codeToken = codeTokens[i];
                        if (!string.IsNullOrEmpty(codeToken)) {
                            BarcodeFormat Format = (BarcodeFormat)int.Parse(codeToken.Split(new string[] {"::"}, StringSplitOptions.None)[0]);
                            string Data = codeToken.Split(new string[] {"::"}, StringSplitOptions.None)[1];
                            codes.Add(new Barcode(Data, Format));
                        }
                    }
                    NatCam.PropagateMRCodes(codes);
                });
            }
            #endregion
            
            
            #region ---External---
            
            #if UNITY_IOS
            [DllImport("__Internal")]
            private static extern void RegisterCallbacks (RenderCallback renderCallback, UpdateCallback updateCallback, ComponentUpdateCallback componentUpdateCallback, UpdatePhotoCallback updatePhotoCallback, UpdateCodeCallback updateCodeCallback);
            [DllImport("__Internal")]
            private static extern void InspectDevice (bool _readablePreview, bool mrDetection, bool verbose);
            [DllImport("__Internal")]
            private static extern bool HasPermissions ();
            [DllImport("__Internal")]
            public static extern bool IsRearFacing (int camera);
            [DllImport("__Internal")]
            public static extern bool IsFlashSupported (int camera);
            [DllImport("__Internal")]
            public static extern bool IsTorchSupported (int camera);
            [DllImport("__Internal")]
            public static extern bool IsZoomSupported (int camera);
            [DllImport("__Internal")]
            public static extern float HorizontalFOV (int camera);
            [DllImport("__Internal")]
            public static extern float VerticalFOV (int camera);
            [DllImport("__Internal")]
            public static extern void GetActiveResolution (int camera, out int width, out int height);
            [DllImport("__Internal")]
            public static extern void SetResolution (int camera, int pWidth, int pHeight);
            [DllImport("__Internal")]
            public static extern void SetFramerate (int camera, float framerate);
            [DllImport("__Internal")]
            public static extern bool SetFocus (int camera, float x, float y);
            [DllImport("__Internal")]
            public static extern bool SetFocusMode (int camera, int state);
            [DllImport("__Internal")]
            public static extern bool SetFlash (int camera, int state);
            [DllImport("__Internal")]
            public static extern bool SetTorch (int camera, int state);
            [DllImport("__Internal")]
            public static extern bool SetZoom (int camera, float ratio);
            [DllImport("__Internal")]
            private static extern void PlayPreview (int cameraLocation);
            [DllImport("__Internal")]
            public static extern void PausePreview ();
            [DllImport("__Internal")]
            private static extern void TerminateOperations ();
            [DllImport("__Internal")]
            public static extern void SwitchCamera (int camera);
            [DllImport("__Internal")]
            public static extern void CaptureStill ();
            [DllImport("__Internal")]
            private static extern void SetRotation (float rotation, float flip);
            [DllImport("__Internal")]
            private static extern void PreviewBuffer (out IntPtr RGBAPtr, out int RGBAS);
            [DllImport("__Internal")]
            private static extern void ReleasePhotoBuffer ();
            [DllImport("__Internal")]
            private static extern void SetPhotoSaveMode (int saveMode);
            #if NATCAM_DEVELOPER_MODE
            [DllImport("__Internal")] [NCDoc(21, 135)] [NCRef(14)]
            public static extern void EnableComponentUpdate (bool state);
            [DllImport("__Internal")] [NCDoc(22, 136)]
            public static extern void DisableRenderingPipeline (bool state);
            #endif
            
            #elif UNITY_ANDROID
            [DllImport("NatCam")]
			private static extern IntPtr NatCamNativeCallback ();
            [DllImport("NatCam")]
            private static extern void RegisterCallbacks (RenderCallback renderCallback, UpdateCallback updateCallback, ComponentUpdateCallback componentUpdateCallback, UpdatePhotoCallback updatePhotoCallback, UpdateCodeCallback updateCodeCallback);
            [DllImport("NatCam")]
            private static extern void SetRotation (float rotation, float flip);
            [DllImport("NatCam")]
            private static extern void PreviewBuffer (out IntPtr RGBAPtr, out int RGBAS);
            [DllImport("NatCam")]
            private static extern void ReleasePhotoBuffer ();
            #if NATCAM_DEVELOPER_MODE
            [DllImport("NatCam")] [NCDoc(21, 135)] [NCRef(14)]
            public static extern void EnableComponentUpdate (bool state);
            [DllImport("NatCam")] [NCDoc(22, 136)]
            public static extern void DisableRenderingPipeline (bool state);
            #endif
            public static AndroidJavaObject A {
                get {
                    if (NCNA == null) {
					    using (AndroidJavaClass NCNAClass = new AndroidJavaClass("com.yusufolokoba.natcam.NatCamNativeActivity")) {
                            if (NCNAClass != null) {
                                NCNA = NCNAClass.CallStatic<AndroidJavaObject>("instance");
                            }
					    }
                    }
                    return NCNA;
                }
            }
            private static AndroidJavaObject NCNA;
            #else
            private static void RegisterCallbacks (RenderCallback renderCallback, UpdateCallback updateCallback, ComponentUpdateCallback componentUpdateCallback, UpdatePhotoCallback updatePhotoCallback, UpdateCodeCallback updateCodeCallback) {}
            private static void PreviewBuffer (out IntPtr ptr, out int size) {ptr = IntPtr.Zero; size = 0;}
            private static void ReleasePhotoBuffer () {}
            private static void SetRotation (float rot, float flip) {}
            #if NATCAM_DEVELOPER_MODE
            public static void DisableRenderingPipeline (bool state) {}
            public static void EnableComponentUpdate (bool state) {}
            #endif
            #endif
            #endregion
        }
    }
}

#pragma warning restore 0414, 0067