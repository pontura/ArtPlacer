//
//  NatCamExtensions.mm
//  NatCam Extensions
//
//  Created by Yusuf on 1/19/16.
//  Copyright (c) 2016 Yusuf Olokoba
//

#import "NatCamExtensions.h"
#import <OpenGLES/ES2/gl.h>

#define abs(x) ((x)<0 ? -(x) : (x))

static bool verboseMode;

void SetOptions (bool verbose) {
    verboseMode = verbose;
}

void Assert (string log) { //Misnomer :(
    if (verboseMode) NSLog(@"NatCam Logging: %s", log);
}

void GLAssert (string log) {
    if (verboseMode) NSLog(@"NatCam Logging: %s with error %i", log, glGetError());
}

void Log (string log) {
    NSLog(@"%s", log);
}

AVCaptureFlashMode FlashMode (int mode) {
    AVCaptureFlashMode ret = AVCaptureFlashModeAuto;
    switch (mode) {
        case 0:
            ret = AVCaptureFlashModeAuto;
            break;
        case 1 :
            ret = AVCaptureFlashModeOn;
            break;
        case 2 :
            ret = AVCaptureFlashModeOff;
            break;
    }
    return ret;
}

AVCaptureDeviceFormat* FormatWithBestResolution (AVCaptureDevice* device, int pwidth, int pheight) {
    AVCaptureDeviceFormat *bestFormat = device.activeFormat;
    for (AVCaptureDeviceFormat *format in device.formats) {
        CMVideoDimensions currentDimensions = CMVideoFormatDescriptionGetDimensions(format.formatDescription);
        CMVideoDimensions bestDimensions = CMVideoFormatDescriptionGetDimensions(bestFormat.formatDescription);
        int curDelta = abs(currentDimensions.width - pwidth) + abs(currentDimensions.height - pheight);
        int bestDelta = abs(bestDimensions.width - pwidth) + abs(bestDimensions.height - pheight);
        bestFormat = curDelta < bestDelta ? format : bestFormat;
    }
    return bestFormat;
}

AVFrameRateRange* ClosestFramerate (AVCaptureDevice* device, float rate) {
    AVCaptureDeviceFormat* format = device.activeFormat;
    AVFrameRateRange *bestRange = [format.videoSupportedFrameRateRanges objectAtIndex:0];
    for (AVFrameRateRange* range in format.videoSupportedFrameRateRanges) {
        float curDelta = abs((float)range.maxFrameRate - rate);
        float bestDelta = abs((float)bestRange.maxFrameRate - rate);
        bestRange = curDelta < bestDelta ? range : bestRange;
    }
    return bestRange;
}

int MRCodeFormat (NSString* mrType) {
    if ([mrType isEqualToString:AVMetadataObjectTypeQRCode]) return 1;
    if ([mrType isEqualToString:AVMetadataObjectTypeEAN13Code]) return 2;
    if ([mrType isEqualToString:AVMetadataObjectTypeEAN8Code]) return 4;
    if ([mrType isEqualToString:AVMetadataObjectTypeDataMatrixCode]) return 8;
    if ([mrType isEqualToString:AVMetadataObjectTypePDF417Code]) return 16;
    if ([mrType isEqualToString:AVMetadataObjectTypeCode128Code]) return 32;
    if ([mrType isEqualToString:AVMetadataObjectTypeCode93Code]) return 64;
    if ([mrType isEqualToString:AVMetadataObjectTypeCode39Code]) return 128;
    return 255;
}

UIImage* ImageFromSampleBuffer (CMSampleBufferRef sampleBuffer) {
    CVImageBufferRef imageBuffer = CMSampleBufferGetImageBuffer(sampleBuffer);
    CVPixelBufferLockBaseAddress(imageBuffer, 0);
    void *baseAddress = CVPixelBufferGetBaseAddress(imageBuffer);
    size_t bytesPerRow = CVPixelBufferGetBytesPerRow(imageBuffer);
    size_t width = CVPixelBufferGetWidth(imageBuffer);
    size_t height = CVPixelBufferGetHeight(imageBuffer);
    CGColorSpaceRef colorSpace = CGColorSpaceCreateDeviceRGB();
    CGContextRef context = CGBitmapContextCreate(baseAddress, width, height, 8,
                                                 bytesPerRow, colorSpace, kCGBitmapByteOrder32Little | kCGImageAlphaPremultipliedFirst);
    CGImageRef quartzImage = CGBitmapContextCreateImage(context);
    CVPixelBufferUnlockBaseAddress(imageBuffer,0);
    CGContextRelease(context);
    CGColorSpaceRelease(colorSpace);
    UIImage *image = [UIImage imageWithCGImage:quartzImage];
    CGImageRelease(quartzImage);
    //Return
    return image;
}

GLubyte* CorrectPhoto (CVImageBufferRef imageBuffer, size_t& width, size_t& height, size_t& size) {
    bool landscape = previewRotation == 0.0 || previewRotation == 1.0;
    int flip = previewFlip;
    size_t bprIn = CVPixelBufferGetBytesPerRow(imageBuffer);
    size_t w = CVPixelBufferGetWidth(imageBuffer);
    size_t h = CVPixelBufferGetHeight(imageBuffer);
    size_t s = bprIn*h;
    width = landscape ? w : h;
    height = landscape ? h : w;
    size = s;
    size_t bprOut = 4 * width;
    void *srcBuff = CVPixelBufferGetBaseAddress(imageBuffer);
    uint8_t rotationConstant = static_cast<int>(180. * previewRotation) / 90;
    GLubyte *outBuff = new GLubyte[s];
    vImage_Buffer ibuff = { srcBuff, h, w, bprIn};
    vImage_Buffer ubuff = { outBuff, height, width, bprOut};
    uint8_t bgColor[4] = {0, 0, 0, 0};
    vImage_Error err= vImageRotate90_ARGB8888 (&ibuff, &ubuff, rotationConstant, bgColor,0);
    if (flip == 1 && landscape) vImageVerticalReflect_ARGB8888(&ubuff, &ubuff, 0);
    else if (flip == 1) vImageHorizontalReflect_ARGB8888(&ubuff, &ubuff, 0);
    if (err != kvImageNoError) NSLog(@"NatCam Error: %ld", err);
    //Swizzle to RGBA so that we can use Get/SetPixel(s) Unityside //Thanks to JP for the request
    const uint8_t map[4] = { 2, 1, 0, 3 };
    err = vImagePermuteChannels_ARGB8888(&ubuff, &ubuff, map, kvImageNoFlags);
    if (err != kvImageNoError) NSLog(@"NatCam Error: %ld", err);
    //Return
    return outBuff;
}

void SaveImageToAppAlbum (UIImage* image, ALAssetsLibrary* library) {
    if (library == nil) {
        Log("NatCam Error: Cannot save to app album because library is null");
        return;
    }
    //Get app name
    NSString* appName = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleDisplayName"];
    //State preservation
    __block BOOL albumFound = NO;
    __block ALAssetsGroup* groupToAddTo;
    void (^SavePhoto)(UIImage*) = ^ (UIImage* imag) {
        CGImageRef img = [imag CGImage];
        [library writeImageToSavedPhotosAlbum:img
                                  orientation:ALAssetOrientationRight
                              completionBlock:^(NSURL* assetURL, NSError* error) {
                                  if (error.code == 0) {
                                      NSLog(@"NatCam: Saved photo to photo album");
                                      
                                      [library assetForURL:assetURL
                                               resultBlock:^(ALAsset *asset) {
                                                   [groupToAddTo addAsset:asset];
                                                   NSLog(@"NatCam: Saved photo to app album");
                                               }
                                              failureBlock:^(NSError* error) {
                                                  NSLog(@"NatCam Error: Failed to retrieve photo:\nError: %@ ", [error localizedDescription]);
                                              }];
                                  }
                                  else {
                                      NSLog(@"NatCam Error:Saving to app album failed.\nError code %li\n%@", (long)error.code, [error localizedDescription]);
                                  }
                              }];
    };
    ALAssetsLibraryGroupsEnumerationResultsBlock
    enumerationDelegate = ^(ALAssetsGroup *group, BOOL *stop){
        if (group) {
            NSString *thisGroup = [group valueForProperty:ALAssetsGroupPropertyName];
            if ([appName isEqualToString:thisGroup]) {
                groupToAddTo = group;
                albumFound = YES;
                SavePhoto(image);
            }
        }
        else { //Album not found
            if (albumFound)
                return;
            NSLog(@"NatCam: Creating App Album");
            ALAssetsLibraryGroupResultBlock successDelegate = ^(ALAssetsGroup *group){
                groupToAddTo = group;
                SavePhoto(image);
            };
            ALAssetsLibraryAccessFailureBlock failureDelegate = ^(NSError *err){
                NSLog(@"NatCam Error: Failed to create app album with error: %@", [err localizedDescription]);
            };
            [library addAssetsGroupAlbumWithName:appName resultBlock:successDelegate failureBlock:failureDelegate];
        }
    };
    
    [library enumerateGroupsWithTypes:ALAssetsGroupAlbum
                           usingBlock:enumerationDelegate
                         failureBlock:^(NSError *error) {
                             NSLog(@"NatCam Error: App album access denied");
                         }];
}