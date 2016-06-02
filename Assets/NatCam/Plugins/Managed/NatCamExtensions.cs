using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace NatCamU {
    
    namespace Internals {
        
        #region ---Native Delegate Callbacks---
        
        public delegate void RenderCallback (int request);
        public delegate void UpdateCallback (IntPtr RGBA32GPUPtr, IntPtr RGBA32Ptr, int width, int height, int size);
        public delegate void ComponentUpdateCallback (IntPtr Y4Ptr, IntPtr UV2Ptr, IntPtr Y4GPUPtr, int Y4W, int Y4H, int Y4S, int UV2W, int UV2H, int UV2S);
        public delegate void UpdatePhotoCallback (IntPtr JPGPtr, int width, int height, int size);
        public delegate void UpdateCodeCallback (string MRCode);
        #endregion
        
        
        #region ---NatCam Extensions and Helpers---
        public static class NatCamExtensions {
            
            private static bool mVerbose;
            
            #region Vars
            
            //Zoom stuff
            const float ZoomMultiplierConstant = 4f;
            public static float ZoomMultiplier { 
                get {
                    return NatCamPreviewZoomer.zoomOverride ? GameObject.FindObjectsOfType<NatCamPreviewZoomer>()[0].zoomSpeed : ZoomMultiplierConstant;
                }
            }
            #endregion
            
            #region ---Ops---
            
            public static void SetOptions (bool verbose) {
                mVerbose = verbose;
            }
            
            public static byte[] MarshalNativeBuffer (this IntPtr pointer, int size) {
                if (pointer == IntPtr.Zero) return null;
                byte[] buffer = new byte[size];
                Marshal.Copy(pointer, buffer, 0, size);
                return buffer;
            }
            
            public static void PreviewOrientation (bool rearFacing, bool photoCapture, out float rotation, out float flip) {
                //Defines
                float angle = 0.5f;
                float mirror = rearFacing ? 1f : 0f;
                //Mirror if we're capturing photo
                mirror = photoCapture ? Mathf.Repeat(mirror + 1f, 2f) : mirror;
                //Switch
                switch (Screen.orientation) {
                    case ScreenOrientation.Portrait:
                    angle = 0.5f;
                    break;
                    case ScreenOrientation.PortraitUpsideDown:
                    angle = 1.5f;
                    break;
                    case ScreenOrientation.LandscapeLeft:
                    angle = 0f;
                    break;
                    case ScreenOrientation.LandscapeRight:
                    angle = 1f;
                    break;
                    case ScreenOrientation.AutoRotation: //Control should never reach here
                        switch (Input.deviceOrientation) {
                            case DeviceOrientation.Portrait:
                            angle = 0.5f;
                            break;
                            case DeviceOrientation.PortraitUpsideDown:
                            angle = 1.5f;
                            break;
                            case DeviceOrientation.LandscapeLeft:
                            angle = 0f;
                            break;
                            case DeviceOrientation.LandscapeRight:
                            angle = 1f;
                            break;
                        }
                    break;
                }
                #if UNITY_ANDROID
                    //Check if this is the front camera
                    angle += rearFacing ? 0f : 1f;
                    //Check for Nexus 5X rear cam
                    angle += SystemInfo.deviceModel.ToLower().Contains("nexus") && SystemInfo.deviceModel.ToLower().Contains("5x") && rearFacing ? 1f : 0f;
                    //Check for Nexus 6 front cam
                    angle += SystemInfo.deviceModel.ToLower().Contains("nexus") && SystemInfo.deviceModel.ToLower().Contains("6") && !rearFacing ? 1f : 0f;
                    //Wrap
                    angle = Mathf.Repeat(angle, 2f);
                #endif
                //Set vars
                rotation = angle;
                flip = mirror;
            }
            
            public static Vector2 Regularize (this Vector2 vec) {
                Func<bool> isPortrait = () => {
                    bool ret = true;
                    if (Screen.orientation == ScreenOrientation.Landscape || Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight) {
                        ret = false;
                    }
                    else if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
                        ret = true;
                    }
                    else if (Screen.orientation == ScreenOrientation.AutoRotation) {
                        ret = Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown;
                    }
                    Assert("Resolution vector regularize: "+(ret ? "portrait" : "landscape"));
                    return ret;
                };
                return new Vector2{x = isPortrait() ? Mathf.RoundToInt(Mathf.Min(vec.x, vec.y)) : Mathf.RoundToInt(Mathf.Max(vec.x, vec.y)), y = isPortrait() ? Mathf.RoundToInt(Mathf.Max(vec.x, vec.y)) : Mathf.RoundToInt(Mathf.Min(vec.x, vec.y))};
            }
            
            public static float Aspect (this Vector2 vec) {
                return vec.x / vec.y;
            }
            
            public static void Dimensions (this ResolutionPreset preset, out int width, out int height) {
                width = 0;
                height = 0;
                switch (preset) {
                    case ResolutionPreset.FullHD:
                        width = 1920;
                        height = 1080;
                    break;
                    case ResolutionPreset.HD:
                        width = 1280;
                        height = 720;
                    break;
                    case ResolutionPreset.MediumResolution:
                        width = 640;
                        height = 480;
                    break;
                    case ResolutionPreset.HighestResolution:
                        width = 9999; //NatCam will pick the resolution closest to this, hence the highest
                        height = 9999;
                    break;
                    case ResolutionPreset.LowestResolution:
                        width = 50; //NatCam will pick the resolution closest to this, hence the lowest
                        height = 50;
                    break;
                }
            }
            
            public static Coroutine Invoke (this IEnumerator routine, MonoBehaviour mono) {
                return mono.StartCoroutine(routine);
            }
            
            public static void Terminate (this Coroutine routine, MonoBehaviour mono) {
                mono.StopCoroutine(routine);
            }
            
            public static IEnumerator Routine<T> (Action<T> action, YieldInstruction yielder) {
                while (true) {
                    yield return yielder;
                    action(default(T));
                }
            }
            
            public static void Assert (this string log) {
                if (mVerbose) Debug.Log("NatCam Logging: "+log);
            }
            
            public static void Warn (this string warning) {
                Debug.LogWarning("NatCam Error: "+warning);
            }
            #endregion
        }
        #endregion
    }
}