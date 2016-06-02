/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

//#define OPENCV_DEVELOPER_MODE //Uncomment this to have access to the PreviewMatrix OpenCV Matrix

#define ALLOCATE_NEW_PHOTO_TEXTURES //Comment this to make NatCam reuse the photo texture it has created instead of allocating new memory
#define ALLOCATE_NEW_FRAME_TEXTURES //Comment this to make NatCam reuse the preview frame texture it has created instead of allocating new memory

using UnityEngine;

#if OPENCV_DEVELOPER_MODE
using OpenCVForUnity;
#endif

namespace NatCamU {
    
    namespace Internals {
        
        [NCDoc(109)]
        public static class NatCamFallbackInterface {
            
            public static Texture2D Photo;
            [NCDoc(125)] public static WebCamTexture Preview {get; private set;}
            public static Texture2D PreviewFrame {
                get {
                    if (Preview == null || PreviewBuffer == null) {
                        "Cannot retrieve preview frame because preview has not started. Returning null.".Warn();
                        return null;
                    }
                    #if ALLOCATE_NEW_FRAME_TEXTURES
                    mPreviewFrame = new Texture2D(Preview.width, Preview.height);
                    #else
                    if (mPreviewFrame == null) mPreviewFrame = new Texture2D(Preview.width, Preview.height);
                    else if (mPreviewFrame.width != Preview.width || mPreviewFrame.height != Preview.height) { //Realloc if size is different
                        MonoBehaviour.Destroy(mPreviewFrame);
                        mPreviewFrame = new Texture2D(Preview.width, Preview.height);
                    }
                    #endif
                    //Check buffer
                    CheckBuffer();
                    Preview.GetPixels32(PreviewBuffer);
                    mPreviewFrame.SetPixels32(PreviewBuffer);
                    mPreviewFrame.Apply();
                    return mPreviewFrame;
                }
            }
            #if OPENCV_DEVELOPER_MODE
            public static Mat PreviewMatrix;
            #endif
            
            public static int mActiveCamera = -1;
            [NCDoc(132)] public static Color32[] PreviewBuffer {get; private set;}
            [NCDoc(133)] public static bool FirstFrameReceived {get; private set;}
            
            //Privates
            private static Texture2D mPreviewFrame;
            private static bool supportedDevice;
            
            
            #region ---Public Ops---
            
            public static void Initialize () {
                if (WebCamTexture.devices.Length == 0) {
                    supportedDevice = false;
                    return;
                }
                //Set state
                supportedDevice = true;
                "Initialized fallback interface".Assert();
            }
            
            public static void Play () {
                if (!supportedDevice) {
                    "Current device has no cameras".Warn();
                    return;
                }
                if (Preview == null) { //First Play()
                    if (((Vector2)(DeviceCamera)mActiveCamera).x == 0) { //Default
                        Preview = new WebCamTexture(WebCamTexture.devices[mActiveCamera].name);
                    }
                    else { //Resolution set
                        Vector2 resolution = (Vector2)(DeviceCamera)mActiveCamera;
                        float frameRate = (float)(DeviceCamera)mActiveCamera;
                        if ((int)frameRate == 0) {
                            Preview = new WebCamTexture(WebCamTexture.devices[mActiveCamera].name, (int)resolution.x, (int)resolution.y);
                        }
                        else {
                            Preview = new WebCamTexture(WebCamTexture.devices[mActiveCamera].name, (int)resolution.x, (int)resolution.y, (int)frameRate);
                        }
                    }
                }
                Preview.Play();
                Camera.onPostRender += Update;
            }
            
            public static void Pause () {
                if (!supportedDevice) {
                    "Current device has no cameras".Warn();
                    return;
                }
                Preview.Pause();
            }
            
            public static void Release () {
                #if !ALLOCATE_NEW_FRAME_TEXTURES //NatCam retains ownership if we aren't reallocating
                if (Photo != null) {MonoBehaviour.Destroy(Photo); Photo = null;}
                #endif
                #if !ALLOCATE_NEW_PHOTO_TEXTURES //NatCam retains ownership if we aren't reallocating
                if (mPreviewFrame != null) {MonoBehaviour.Destroy(mPreviewFrame); mPreviewFrame = null;}
                #endif
                if (Preview.isPlaying) Preview.Stop(); MonoBehaviour.Destroy(Preview); Preview = null;
                #if OPENCV_DEVELOPER_MODE
                if (PreviewMatrix != null) PreviewMatrix.release(); PreviewMatrix = null;
                #endif
                mActiveCamera = -1;
                PreviewBuffer = null;
                FirstFrameReceived = false;
                Camera.onPostRender -= Update;
            }
            
            public static void SwitchActiveCamera () {
                if (!supportedDevice) {
                    "Current device has no cameras".Warn();
                    return;
                }
                Preview.Stop();
                Preview.deviceName = WebCamTexture.devices[mActiveCamera].name;
                Preview.Play();
            }
            
            public static void CapturePhoto () {
                if (!supportedDevice) {
                    "Current device has no cameras".Warn();
                    return;
                }
                if (Preview == null) return;
                if (!Preview.isPlaying) return;
                #if ALLOCATE_NEW_PHOTO_TEXTURES
                Photo = new Texture2D(Preview.width, Preview.height);
                #else
                if (Photo == null) Photo = new Texture2D(Preview.width, Preview.height);
                #endif
                Preview.GetPixels32(PreviewBuffer);
                Photo.SetPixels32(PreviewBuffer);
                Photo.Apply();
                NatCam.PropagatePhoto();
            }
            
            private static void Update (Camera unused) {
                if (Preview == null) return;
                if (!Preview.isPlaying) return;
                //Preview
                if (!FirstFrameReceived && Preview.didUpdateThisFrame) {
                    NatCam.PropagateStart();
                    FirstFrameReceived = true;
                }
                //PreviewBuffer
                CheckBuffer();
                //OpenCV
                #if OPENCV_DEVELOPER_MODE
                if (PreviewMatrix == null) PreviewMatrix = new Mat(new Size(Preview.width, Preview.height), CvType.CV_8UC4);
                Utils.webCamTextureToMat(Preview, PreviewMatrix, PreviewBuffer);
                #endif
                //Update
                NatCam.PropagateUpdate();
            }
            
            private static void CheckBuffer () {
                if (PreviewBuffer == null) PreviewBuffer = new Color32[Preview.width * Preview.height];
                if (PreviewBuffer.Length != Preview.width * Preview.height) PreviewBuffer = new Color32[Preview.width * Preview.height];
            }
            #endregion
        }
    }
}