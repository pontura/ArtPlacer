//
//  NatCam.h
//  NatCam
//
//  Created by Yusuf on 1/9/16.
//  Copyright (c) 2016 Yusuf Olokoba
//

#import "NatCamDecls.h"
#import "NatCamRendering.h"
#import "UnityAppController.h"
#include "UnityInterface.h"

//NatCamU-NatCamX bridge
extern "C" {
    //Initialization
    void RegisterCallbacks (RenderCallback renderCallback, UpdateCallback updateCallback, ComponentUpdateCallback componentUpdateCallback, UpdatePhotoCallback updatePhotoCallback, UpdateCodeCallback updateCodeCallback);
    void InspectDevice (bool _readablePreview, bool mrDetection, bool verbose);
    bool HasPermissions ();
    //Learning
    void GetActiveResolution (int camera, int* width, int* height);
    bool IsRearFacing (int camera);
    bool IsFlashSupported (int camera);
    bool IsTorchSupported (int camera);
    bool IsZoomSupported (int camera);
    float HorizontalFOV (int camera);
    float VerticalFOV (int camera);
    //Control
    void SetResolution (int camera, int pWidth, int pHeight);
    void SetFramerate (int camera, float framerate);
    void SetFocus (int camera, float x, float y);
    bool SetFocusMode (int camera, int state);
    bool SetFlash (int camera, int state);
    bool SetTorch (int camera, int state);
    bool SetZoom (int camera, float ratio);
    void ReleasePhotoBuffer ();
    void SetPhotoSaveMode (int saveMode);
    void EnableComponentUpdate (bool state);
    void DisableRenderingPipeline (bool state);
    //Running
    void PlayPreview (int cameraLocation);
    void PausePreview ();
    void TerminateOperations ();
    void SwitchCamera (int camera);
    void CaptureStill ();
}

@interface NatCam : NSObject <AVCaptureVideoDataOutputSampleBufferDelegate, AVCaptureMetadataOutputObjectsDelegate> {
@public
    //Cameras
    NSArray* Cameras;
    //State
    bool initializedSession, inspectedDevice;
    bool mrDetection;
    int photoSaveMode;
    //Session
    AVCaptureSession *session;
    AVCaptureStillImageOutput *photoOutput;
    AVCaptureMetadataOutput *metadataOutput;
    AVCaptureVideoDataOutput *captureOutput;
    //Utils
    dispatch_queue_t sessionQueue;
    ALAssetsLibrary *library;
    NSMutableString* mrCodeToken;
    GLubyte* JPEG;
}

@end

//Singleton
NatCam* NatCamInstance ();
