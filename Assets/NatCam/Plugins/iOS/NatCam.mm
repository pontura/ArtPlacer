//
//  NatCam.mm
//  NatCam Control Pipeline
//
//  Created by Yusuf on 1/19/16.
//  Copyright (c) 2016 Yusuf Olokoba
//

#import "NatCam.h"


#pragma mark  --NatCamX Singleton--
static NatCam* _sharedInstance = nil;
NatCam* NatCamInstance () {
    if( !_sharedInstance ) {
        _sharedInstance = [[NatCam alloc] init];
    }
    return _sharedInstance;
}


@implementation NatCam

#pragma mark --Top Level Initialization--

void RegisterCallbacks (RenderCallback renderCallback, UpdateCallback updateCallback, ComponentUpdateCallback componentUpdateCallback, UpdatePhotoCallback updatePhotoCallback, UpdateCodeCallback updateCodeCallback) {
    _renderCallback = renderCallback;
    _updateCallback = updateCallback;
    _componentUpdateCallback = componentUpdateCallback;
    _updatePhotoCallback = updatePhotoCallback;
    _updateCodeCallback = updateCodeCallback;
    Log("NatCam Native: Registered Callbacks");
}

void InspectDevice (bool _readablePreview, bool mrDetection, bool verbose) {
    readablePreview = _readablePreview;
    NatCamInstance()->mrDetection = mrDetection;
    SetOptions(verbose);
    [NatCamInstance() InspectDevice];
}

bool HasPermissions () {
    return [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo] == AVAuthorizationStatusAuthorized;
}

#pragma mark --Learning--

void GetActiveResolution (int camera, int* width, int* height) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = (AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera];
    CMVideoDimensions dimensions = CMVideoFormatDescriptionGetDimensions(device.activeFormat.formatDescription);
    *width = dimensions.width;
    *height = dimensions.height;
}

bool IsRearFacing (int camera) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    return ((AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera]).position == AVCaptureDevicePositionBack;
}

bool IsFlashSupported (int camera) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    return ((AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera]).flashAvailable;
}

bool IsTorchSupported (int camera) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    return ((AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera]).torchAvailable;
}

bool IsZoomSupported (int camera) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    return ((AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera]).activeFormat.videoMaxZoomFactor > 1;
}

float HorizontalFOV (int camera) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    return ((AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera]).activeFormat.videoFieldOfView;
}

float VerticalFOV (int camera) {
    int w, h;
    GetActiveResolution(camera, &w, &h);
    return HorizontalFOV(camera) * (float)h / (float)w;
}

#pragma mark --Control--

void SetResolution (int camera, int pWidth, int pHeight) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = (AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera];
    AVCaptureDeviceFormat* bestFormat = FormatWithBestResolution(device, pWidth, pHeight);
    if ([device lockForConfiguration:NULL] == YES) {
        device.activeFormat = bestFormat;
        Assert([[NSString stringWithFormat:@"Changed camera format for resolution %ix%i", pWidth, pHeight] UTF8String]);
        [device unlockForConfiguration];
    }
}

void SetFramerate (int camera, float framerate) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = (AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera];
    AVFrameRateRange* bestRange = ClosestFramerate(device, framerate);
    if ([device lockForConfiguration:NULL] == YES) {
        device.activeVideoMinFrameDuration = bestRange.minFrameDuration;
        device.activeVideoMaxFrameDuration = bestRange.maxFrameDuration;
        Assert([[NSString stringWithFormat:@"Changed camera framerate to %f", bestRange.maxFrameRate] UTF8String]);
        [device unlockForConfiguration];
    }
}

void SetFocus (int camera, float x, float y) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = [NatCamInstance()->Cameras objectAtIndex:camera];
    Assert([[NSString stringWithFormat:@"Attempting to focus camera at (%f, %f)", x, y] UTF8String]);
    CGPoint focusPoint = CGPointMake(x, y);
    if ([device lockForConfiguration:NULL] == YES) {
        if ([device isFocusPointOfInterestSupported]) [device setFocusPointOfInterest:focusPoint];
        if ([device isExposurePointOfInterestSupported]) [device setExposurePointOfInterest:focusPoint];
        if ([device isFocusModeSupported:AVCaptureFocusModeAutoFocus]) [device setFocusMode:AVCaptureFocusModeAutoFocus];
        if ([device isExposureModeSupported:AVCaptureExposureModeAutoExpose]) [device setExposureMode:AVCaptureExposureModeAutoExpose];
        Assert("Set focus point");
        [device unlockForConfiguration];
    }
}

bool SetFocusMode (int camera, int state) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = [NatCamInstance()->Cameras objectAtIndex:camera];
    bool ret = false;
    if (state % 2 == 0) [NatCamInstance() RegisterForAutofocus:device];
    else [NatCamInstance() UnregisterFromAutofocus:device];
    AVCaptureFocusMode focusMode = state % 2 == 0 ? AVCaptureFocusModeContinuousAutoFocus : AVCaptureFocusModeLocked;
    if ([device lockForConfiguration:NULL] == YES ) {
        if ([device isFocusModeSupported:focusMode]) {
            [device setFocusMode:focusMode];
            ret = true;
        }
        if ([device isExposureModeSupported:AVCaptureExposureModeAutoExpose]) [device setExposureMode:AVCaptureExposureModeAutoExpose];
        Assert([[NSString stringWithFormat:@"Set focus mode to %i", state] UTF8String]);
        [device unlockForConfiguration];
    }
    return ret;
}

bool SetFlash (int camera, int state) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = [NatCamInstance()->Cameras objectAtIndex:camera];
    bool ret = false;
    if (device.flashAvailable) {
        if ([device lockForConfiguration:NULL] == YES ) {
            AVCaptureFlashMode flashMode = FlashMode(state);
            if ([device isFlashModeSupported:flashMode]) {
                [device setFlashMode:flashMode];
                ret = true;
            }
            else Log([[NSString stringWithFormat:@"NatCam Error: Camera does not support %ld", (long)flashMode] UTF8String]);
            [device unlockForConfiguration];
        }
    }
    else {
        Log("NatCam Error: Camera does not have flash");
    }
    return ret;
}

bool SetTorch (int camera, int state) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = [NatCamInstance()->Cameras objectAtIndex:camera];
    bool ret = false;
    if ([device isTorchAvailable]) {
        if ([device lockForConfiguration:NULL] == YES) {
            if (state == 0) {
                [device setTorchMode:AVCaptureTorchModeOff];
                Assert("Disabled torch");
                ret = true;
            }
            else if (state == 1) {
                BOOL success = [device setTorchModeOnWithLevel:AVCaptureMaxAvailableTorchLevel error:nil];
                if (success) {
                    Assert("Enabled torch");
                    ret = true;
                }
                else Log("NatCam Error: Failed to enable torch");
            }
            [device unlockForConfiguration];
        }
    }
    else {
        Log("NatCam Error: Camera does not support torch.");
    }
    return ret;
}

bool SetZoom (int camera, float ratio) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = [NatCamInstance()->Cameras objectAtIndex:camera];
    bool ret = false;
    float zoomRate = 2.0f; //CONST
    if ([[device activeFormat] videoMaxZoomFactor] > 1) {
        float zoomRatio = (([[device activeFormat] videoMaxZoomFactor] - 1) * ratio) + 1;
        if ([device lockForConfiguration:nil]) {
            [device rampToVideoZoomFactor:zoomRatio withRate:zoomRate];
            [device unlockForConfiguration];
            Assert([[NSString stringWithFormat:@"Set zoom ratio: %f", zoomRatio] UTF8String]);
            ret = true;
        }
    }
    return ret;
}

void ReleasePhotoBuffer () {
    if (NatCamInstance()->JPEG == NULL) return;
    delete [] NatCamInstance()->JPEG;
    NatCamInstance()->JPEG = NULL;
}

void SetPhotoSaveMode (int saveMode) {
    NatCamInstance()->photoSaveMode = saveMode;
}

void EnableComponentUpdate (bool state) {
    componentUpdate = state;
}

void DisableRenderingPipeline (bool state) {
    disabledRendering = state;
}

#pragma mark --Operations--

void PlayPreview (int cameraLocation) {
    [NatCamInstance() PlayPreview:cameraLocation];
}

void PausePreview () {
    [NatCamInstance() PausePreview];
}

void TerminateOperations () {
    [NatCamInstance() StopPreview];
}

void SwitchCamera (int camera) {
    [NatCamInstance() SwitchCamera:camera];
}

void CaptureStill () {
    [NatCamInstance() CapturePhoto];
}


#pragma mark --Initilaization--

-(void) InspectDevice {
    Cameras = [AVCaptureDevice devicesWithMediaType:AVMediaTypeVideo];
    inspectedDevice = true;
    Log([[NSString stringWithFormat:@"NatCam: Inspected Device: Found %i cameras", (int)[Cameras count]] UTF8String]);
}

-(void) InitializeSession {
    session = [[AVCaptureSession alloc] init];
    sessionQueue = dispatch_queue_create("session queue", DISPATCH_QUEUE_SERIAL);
    Assert("Created session");
    [session setSessionPreset:AVCaptureSessionPresetInputPriority];
    captureOutput	= [[AVCaptureVideoDataOutput alloc] init];
    captureOutput.alwaysDiscardsLateVideoFrames = YES;
    Assert("Created preview output");
    [captureOutput setSampleBufferDelegate:self queue:dispatch_get_main_queue()]; //GLES thread conflict aversion
    [captureOutput setVideoSettings:[NSDictionary dictionaryWithObject:[NSNumber numberWithInt:kCVPixelFormatType_420YpCbCr8BiPlanarFullRange] forKey:(id)kCVPixelBufferPixelFormatTypeKey]]; //Video range
    if ([session canAddOutput:captureOutput]) { //Safe guard
        [session addOutput:captureOutput];
        Assert("Added preview output");
    }
    else {
        Log("NatCam Error: Failed to add preview output to session. Terminating command");
        return;
    }
    if (mrDetection) {
        metadataOutput = [[AVCaptureMetadataOutput alloc] init];
        Assert("Created MR output");
        if ([session canAddOutput:metadataOutput]) {
            [session addOutput:metadataOutput];
            Assert("Successfully pinned MR output to session");
            [metadataOutput setMetadataObjectsDelegate:self queue:dispatch_get_main_queue()];
            Assert("Set MR detection for 8 formats");
        }
        else {
            NSLog(@"NatCam Error: Failed to initialize machine-readable code detection");
        }
    }
    photoOutput = [[AVCaptureStillImageOutput alloc] init];
    photoOutput.highResolutionStillImageOutputEnabled = YES;
    Assert("Created photo output");
    //NSDictionary *outputSettings = @{AVVideoCodecKey : AVVideoCodecJPEG};
    NSDictionary *outputSettings = @{(id)kCVPixelBufferPixelFormatTypeKey : [NSNumber numberWithInt:kCVPixelFormatType_32BGRA]};
    [photoOutput setOutputSettings:outputSettings];
    Assert("Set photo output to data format JPEG");
    if ([session canAddOutput:photoOutput]) { //Safeguard
        [session addOutput:photoOutput];
        Assert("Successfully pinned photo output to session");
    }
    else {
        NSLog(@"NatCam Error: Failed to pin photo output to session");
    }
    library = [[ALAssetsLibrary alloc] init];
    _width = _height = 0;
    initializedSession = true;
}

#pragma mark --Control--

-(void) RegisterForAutofocus:(AVCaptureDevice*) device {
    //State checking
    if (device.isSubjectAreaChangeMonitoringEnabled == YES) return;
    if ([device lockForConfiguration:NULL] == YES) {
        device.subjectAreaChangeMonitoringEnabled = YES;
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(AutofocusDelegate:) name:AVCaptureDeviceSubjectAreaDidChangeNotification object:device];
        [device unlockForConfiguration];
    }
}

-(void) UnregisterFromAutofocus:(AVCaptureDevice*) device {
    //State checking
    if (device.isSubjectAreaChangeMonitoringEnabled == NO) return;
    if ([device lockForConfiguration:NULL] == YES ) {
        device.subjectAreaChangeMonitoringEnabled = NO;
        [[NSNotificationCenter defaultCenter] removeObserver:self name:AVCaptureDeviceSubjectAreaDidChangeNotification object:device];
        [device unlockForConfiguration];
    }
}

-(void) AutofocusDelegate:(NSNotification*) notification {
    AVCaptureDevice* device = [notification object];
    if (device.focusMode == AVCaptureFocusModeLocked || device.exposureMode == AVCaptureExposureModeLocked){
        if ([device lockForConfiguration:NULL] == YES ) {
            if (device.isFocusPointOfInterestSupported) [device setFocusPointOfInterest:CGPointMake(0.5f, 0.5f)];
            if (device.isExposurePointOfInterestSupported) [device setExposurePointOfInterest:CGPointMake(0.5f, 0.5f)];
            if ([device isFocusModeSupported:AVCaptureFocusModeContinuousAutoFocus]) {
                [device setFocusMode:AVCaptureFocusModeContinuousAutoFocus];
            }
            if ([device isExposureModeSupported:AVCaptureExposureModeAutoExpose]){
                [device setExposureMode:AVCaptureExposureModeAutoExpose];
            }
            [device unlockForConfiguration];
        }
    }
}

#pragma mark --Running--

-(void) PlayPreview:(int)location {
    if (!initializedSession) {
        [self InitializeSession];
    }
    //Remove all session inputs
    for (AVCaptureInput *input in [session inputs]) {
        [session removeInput:input];
    }
    Assert("Disposed all inputs");
    AVCaptureDevice* device = [Cameras objectAtIndex:location];
    AVCaptureDeviceInput *input = [AVCaptureDeviceInput deviceInputWithDevice:device error:nil];
    if ([session canAddInput:input]) {
        [session addInput:input];
        Assert("Added camera input");
    }
    else Log("NatCam Error: Failed to add camera input");
    if (mrDetection) [metadataOutput setMetadataObjectTypes:[metadataOutput availableMetadataObjectTypes]];
    if ([device lockForConfiguration:NULL] == YES) {
        [session startRunning];
        Assert("Started session");
        [device unlockForConfiguration];
    }
    else Log("NatCam Error: Failed to acquire camera lock and start preview");
}

-(void) PausePreview {
    [session stopRunning];
    Assert("Stopped session");
}

-(void) StopPreview {
    if (session.isRunning) [session stopRunning];
    for (AVCaptureInput *input1 in session.inputs) {
        [session removeInput:input1];
    }
    for (AVCaptureOutput *output2 in session.outputs) {
        [session removeOutput:output2];
    }
    [[NSNotificationCenter defaultCenter] removeObserver:self];
    Cameras = nil;
    initializedSession = inspectedDevice = mrDetection = false;
    photoSaveMode = 0;
    sessionQueue = nil;
    library = nil;
    mrCodeToken = nil;
    session = nil;
    photoOutput = nil;
    metadataOutput = nil;
    captureOutput = nil;
    _renderCallback(3);
    _sharedInstance = nil;
}

-(void) SwitchCamera:(int)toCamera {
    //Select the camera
    AVCaptureDevice *device = [Cameras objectAtIndex:toCamera];
    [session beginConfiguration];
    
    NSArray* inputs = [session inputs];
    for (AVCaptureInput *input in inputs)
        [session removeInput:input];
    if (mrDetection) [metadataOutput setMetadataObjectTypes:nil];
    AVCaptureDeviceInput *newVideoInput = [AVCaptureDeviceInput deviceInputWithDevice:device error:nil];
    if ([session canAddInput:newVideoInput]) {
        [session addInput:newVideoInput];
        if (mrDetection) [metadataOutput setMetadataObjectTypes:[metadataOutput availableMetadataObjectTypes]];
        Assert("Added new device camera input to session");
    }
    else {
        Log("NatCam Error: Failed to add new device camera to session");
    }
    [session commitConfiguration];
}

-(void) CapturePhoto {
    //Execute async
    dispatch_async(sessionQueue, ^{
        AVCaptureConnection *videoConnection = [photoOutput connectionWithMediaType:AVMediaTypeVideo];
        //Capture still
        [photoOutput captureStillImageAsynchronouslyFromConnection:videoConnection completionHandler: ^(CMSampleBufferRef sampleBuffer, NSError *error) {
            Log("NatCam: Captured photo");
            CVPixelBufferRef photo = CMSampleBufferGetImageBuffer(sampleBuffer);
            size_t w, h, s;
            CVPixelBufferLockBaseAddress(photo, 0);
            JPEG = CorrectPhoto(photo, w, h, s);
            CVPixelBufferUnlockBaseAddress(photo, 0);
            _updatePhotoCallback(JPEG, (int)w, (int)h, (int)s);
            //Continue with saving image to device
            if (photoSaveMode > 0) {
                 UIImage *image = [[UIImage alloc] initWithCGImage:ImageFromSampleBuffer(sampleBuffer).CGImage scale:1 orientation:UIImageOrientationRight];
                 if (photoSaveMode == 1) { //Save to Photos
                     UIImageWriteToSavedPhotosAlbum(image, nil, nil, nil);
                     Log("NatCam: Saved image to photo album");
                 }
                 else if (photoSaveMode == 2) { //Save to App Album
                     SaveImageToAppAlbum(image, library);
                 }
            }
         }];
    });
}


#pragma mark --Callbacks--

- (void)captureOutput:(AVCaptureOutput*)avcaptureoutput didOutputSampleBuffer:(CMSampleBufferRef)sampleBuffer fromConnection:(AVCaptureConnection*)connection {
    UpdateDimensions(sampleBuffer);
    if (!initializedRendering) {
        _renderCallback(1);
        return;
    }
    UpdateComponentTextures(sampleBuffer); //Might break on multithreaded rendering
    _renderCallback(2);
    Assert("Preview Callback");
}

- (void)captureOutput:(AVCaptureOutput *)captureOutputM didOutputMetadataObjects:(NSArray *)metadataObjects fromConnection:(AVCaptureConnection *)connection {
    if (mrCodeToken == nil) {
        mrCodeToken = [[NSMutableString alloc] initWithString:@""];
    }
    [mrCodeToken setString:@""];
    for (AVMetadataObject *metadataObject in metadataObjects) {
        AVMetadataMachineReadableCodeObject *readableObject = (AVMetadataMachineReadableCodeObject *)metadataObject;
        if (readableObject.type == AVMetadataObjectTypeFace) continue; //Don't want faces
        [mrCodeToken appendFormat:@"%d::%@", MRCodeFormat(readableObject.type), readableObject.stringValue];
    }
    if (![mrCodeToken isEqualToString:@""]) {
        _updateCodeCallback([mrCodeToken UTF8String]);
    }
}

@end