//
//  NatCamDecls.h
//  NatCam
//
//  Created by Yusuf on 4/14/16.
//  Copyright (c) 2016 Yusuf Olokoba
//

#import "NatCamExtensions.h"

//Callback definitions
typedef void (*RenderCallback) (int request);
typedef void (*UpdateCallback) (void* RGBA32GPUPtr, void* RGBA32Ptr, int width, int height, int size);
typedef void (*ComponentUpdateCallback) (void* Y4Ptr, void* UV2Ptr, void* Y4GPUPtr, int Y4W, int Y4H, int Y4S, int UV2W, int UV2H, int UV2S);
typedef void (*UpdatePhotoCallback) (void* JPGPtr, int width, int height, int size);
typedef void (*UpdateCodeCallback) (string MRCode);

//Callbacks
extern RenderCallback _renderCallback;
extern UpdateCallback _updateCallback;
extern ComponentUpdateCallback _componentUpdateCallback;
extern UpdatePhotoCallback _updatePhotoCallback;
extern UpdateCodeCallback _updateCodeCallback;