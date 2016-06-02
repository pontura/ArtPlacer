//
//  NatCamExtensions.h
//  NatCam Extensions
//
//  Created by Yusuf on 1/9/16.
//  Copyright (c) 2016 Yusuf Olokoba
//

#import <AVFoundation/AVFoundation.h>
#import <Accelerate/Accelerate.h>
#import <AssetsLibrary/AssetsLibrary.h>

typedef const char * string;

//Transformations
extern float previewRotation, previewFlip;

//Helpers
void SetOptions (bool verbose);
void Assert (string log);
void GLAssert (string log);
void Log (string log);
AVCaptureFlashMode FlashMode (int mode);
AVCaptureDeviceFormat* FormatWithBestResolution (AVCaptureDevice* device, int pwidth, int pheight);
AVFrameRateRange* ClosestFramerate (AVCaptureDevice* device, float rate);
int MRCodeFormat (NSString* mrType);
UIImage* ImageFromSampleBuffer (CMSampleBufferRef sampleBuffer);
GLubyte* CorrectPhoto (CVImageBufferRef imageBuffer, size_t& width, size_t& height, size_t& size);
void SaveImageToAppAlbum (UIImage* image, ALAssetsLibrary* library);