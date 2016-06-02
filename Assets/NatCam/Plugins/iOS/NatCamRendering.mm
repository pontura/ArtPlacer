//
//  NatCamRendering.mm
//  NatCamS Rendering Pipeline
//
//  Created by Yusuf on 1/19/16.
//  Copyright (c) 2016 Yusuf Olokoba
//

#import "NatCamRendering.h"


#pragma mark --Callbacks--
RenderCallback _renderCallback;
UpdateCallback _updateCallback;
ComponentUpdateCallback _componentUpdateCallback;
UpdatePhotoCallback _updatePhotoCallback;
UpdateCodeCallback _updateCodeCallback;

#pragma mark --Buffers--
GLubyte* RGBA32;
GLubyte* Y4;
GLubyte* UV2;
GLubyte* JPEG;

size_t RGBA32W, RGBA32H, RGBA32S;
size_t Y4W, Y4H, Y4S;
size_t UV2W, UV2H, UV2S;

#pragma mark --Rendering Vars--

//State
bool initializedRendering;
bool readablePreview;
bool componentUpdate;
bool disabledRendering;
//Camera Preview Texture
size_t _width, _height;
//Transformations
float previewRotation, previewFlip;
//Rendering
id<MTLTexture> YTexMtl;
id<MTLTexture> UVTexMtl;
GLuint YTexGL;
GLuint UVTexGL;
CVOpenGLESTextureRef glPreviewTexture;
CVMetalTextureRef mtlPreviewTexture;
CVPixelBufferRef glRenderBuffer;
CVPixelBufferRef mtlRenderBuffer;

static const GLfloat vertexData[] = {
    -1.0f,  1.0f,   // Position 0
    -1.0f,  -1.0f,  // Position 1
    1.0f,   -1.0f,  // Position 2
    1.0f,   1.0f   // Position 3
};

static const GLfloat textureData[] = {
    0.0f,   0.0f,   // TexCoord 0
    0.0f,   1.0f,   // TexCoord 1
    1.0f,   1.0f,   // TexCoord 2
    1.0f,   0.0f    // TexCoord 3
};

static const GLushort indexData[] = { 0, 1, 2, 0, 2, 3 };

static CVOpenGLESTextureCacheRef glTextureCache;
static CVMetalTextureCacheRef mtlTextureCache;

static GLuint glVertexBuffer;
static GLuint glTextureBuffer;
static GLuint glFrameBuffer;

static GLint glProgram;
static GLint glYUniform;
static GLint glUVUniform;
static GLint glRotUniform;
static GLint glFlipUniform;
static GLint glPositionLoc;
static GLint glTexCoordLoc;

static GLint glPreviousFBO;
static GLint glPreviousRenderBuffer;
static GLint glPrevious_program;

static MTLRenderPassDescriptor *mtlRenderDescriptor;
static id<MTLRenderPipelineState> mtlPipelineState;
static id<MTLBuffer> mtlVertexBuffer;
static id<MTLBuffer> mtlTextureBuffer;
static id<MTLBuffer> mtlIndexBuffer;
static id<MTLBuffer> mtlRotUniform;
static id<MTLBuffer> mtlFlipUniform;

#pragma mark --NatCamU--
void SetRotation (float rotation, float flip) {
    previewRotation = rotation;
    previewFlip = flip;
}

void PreviewBuffer (void** RGBAPtr, int* RGBAS) {
    *RGBAPtr = RGBA32;
    *RGBAS = (int)RGBA32S;
}

#pragma mark --Utility--
void PreserveRenderingContextGL () {
    glGetIntegerv(GL_FRAMEBUFFER_BINDING, &glPreviousFBO);
    glGetIntegerv(GL_RENDERBUFFER_BINDING, &glPreviousRenderBuffer);
    glGetIntegerv(GL_CURRENT_PROGRAM, &glPrevious_program);
}

void RestoreRenderingContextGL () {
    glBindFramebuffer(GL_FRAMEBUFFER, glPreviousFBO);
    glBindRenderbuffer(GL_RENDERBUFFER, glPreviousRenderBuffer);
    glUseProgram(glPrevious_program);
    glPreviousFBO = 0;
    glPreviousRenderBuffer = 0;
    glPrevious_program = 0;
}

#pragma mark --Initialization--

void InitializeMetal () {
    NSError* err = nil;
    id<MTLLibrary> lib = [UnityGetMetalDevice() newLibraryWithSource:[NSString stringWithUTF8String:shaderMetal] options: nil error: &err];
    if (err) NSLog(@"NatCam Native Error: Failed to compile Metal shader: %@", err);
    id<MTLFunction> mtlVertexShader = [lib newFunctionWithName: @"vShader"];
    id<MTLFunction> mtlFragmentShader = [lib newFunctionWithName: @"fShader"];
    mtlVertexBuffer = [UnityGetMetalDevice() newBufferWithBytes:vertexData length: sizeof(vertexData) options: MTLResourceOptionCPUCacheModeDefault];
    mtlTextureBuffer = [UnityGetMetalDevice() newBufferWithBytes:textureData length:sizeof(textureData) options:MTLResourceOptionCPUCacheModeDefault];
    mtlIndexBuffer = [UnityGetMetalDevice() newBufferWithBytes: indexData length: sizeof(indexData) options: MTLResourceOptionCPUCacheModeDefault];
    float rotUniform[] = {previewRotation};
    float flipUniform[] = {(float)previewFlip};
    mtlRotUniform = [UnityGetMetalDevice() newBufferWithBytes:rotUniform length:sizeof(rotUniform) options:MTLResourceOptionCPUCacheModeDefault];
    mtlFlipUniform = [UnityGetMetalDevice() newBufferWithBytes:flipUniform length:sizeof(flipUniform) options:MTLResourceOptionCPUCacheModeDefault];
    MTLRenderPipelineDescriptor *pipelineStateDescriptor = [[[UnityGetMetalBundle() classNamed: @"MTLRenderPipelineDescriptor"] alloc] init];
    [pipelineStateDescriptor setSampleCount: 1];
    [pipelineStateDescriptor setVertexFunction:mtlVertexShader];
    [pipelineStateDescriptor setFragmentFunction:mtlFragmentShader];
    pipelineStateDescriptor.colorAttachments[0].pixelFormat = MTLPixelFormatBGRA8Unorm;
    pipelineStateDescriptor.depthAttachmentPixelFormat = MTLPixelFormatInvalid;
    NSError* error = NULL;
    mtlPipelineState = [UnityGetMetalDevice() newRenderPipelineStateWithDescriptor:pipelineStateDescriptor error:&error];
    if (error) NSLog(@"NatCam Native Error: Failed to created metal pipeline state: %@", error);
    else Log("NatCam Native: Initialized Metal States");
}

void GenerateFastReadContextGL () {
    CVReturn err = CVOpenGLESTextureCacheCreate(kCFAllocatorDefault, NULL, UnityGetMainScreenContextGLES(), NULL, &glTextureCache);
    if (err) {
        NSLog(@"%s", "NatCam Native Error: Failed to generate fast read context");
        return;
    }
    NSDictionary* options = @{	(__bridge NSString*)kCVPixelBufferPixelFormatTypeKey : @(kCVPixelFormatType_32BGRA),
                                (__bridge NSString*)kCVPixelBufferWidthKey : @(_width),
                                (__bridge NSString*)kCVPixelBufferHeightKey : @(_height),
                                (__bridge NSString*)kCVPixelBufferOpenGLESCompatibilityKey : @(YES),
                                (__bridge NSString*)kCVPixelBufferIOSurfacePropertiesKey : @{}
                                };
    CVPixelBufferCreate(kCFAllocatorDefault, _width, _height, kCVPixelFormatType_32BGRA, (__bridge CFDictionaryRef)options, &glRenderBuffer);
    err = CVOpenGLESTextureCacheCreateTextureFromImage (kCFAllocatorDefault, glTextureCache, glRenderBuffer,
                                                        NULL,
                                                        GL_TEXTURE_2D,
                                                        GL_RGBA,
                                                        (int)_width,
                                                        (int)_height,
                                                        GL_RGBA,
                                                        GL_UNSIGNED_BYTE,
                                                        0,
                                                        &glPreviewTexture);
    if (err) {
        NSLog(@"NatCam Native Error: Failed to generate preview texture: %d", err);
        return;
    }
    Log("NatCam Native: Generated Preview Texture");
    glBindTexture(CVOpenGLESTextureGetTarget(glPreviewTexture), CVOpenGLESTextureGetName(glPreviewTexture));
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, CVOpenGLESTextureGetName(glPreviewTexture), 0);
    NSLog(@"%s", "NatCam Native: Generated FastRead Context");
}

void GenerateFastReadContextMetal () {
    CVReturn err = CVMetalTextureCacheCreate(kCFAllocatorDefault, NULL, UnityGetMetalDevice(), NULL, &mtlTextureCache);
    if (err) {
        NSLog(@"%s", "NatCam Native Error: Failed to generate fast read context");
        return;
    }
    NSDictionary* options = @{	(__bridge NSString*)kCVPixelBufferPixelFormatTypeKey : @(kCVPixelFormatType_32BGRA),
                                (__bridge NSString*)kCVPixelBufferWidthKey : @(_width),
                                (__bridge NSString*)kCVPixelBufferHeightKey : @(_height),
                                (__bridge NSString*)kCVPixelBufferMetalCompatibilityKey : @(YES),
                                (__bridge NSString*)kCVPixelBufferIOSurfacePropertiesKey : @{}
                                };
    CVPixelBufferCreate(kCFAllocatorDefault, _width, _height, kCVPixelFormatType_32BGRA, (__bridge CFDictionaryRef)options, &mtlRenderBuffer);
    err = CVMetalTextureCacheCreateTextureFromImage (kCFAllocatorDefault, mtlTextureCache, mtlRenderBuffer,
                                                     NULL,
                                                     MTLPixelFormatRGBA8Unorm,
                                                     (int)_width,
                                                     (int)_height,
                                                     0,
                                                     &mtlPreviewTexture);
    if (err) {
        NSLog(@"NatCam Native Error: Failed to generate preview texture: %d", err);
        return;
    }
    NSLog(@"%s", "NatCam Native: Generated Preview Texture");
    mtlRenderDescriptor = [[UnityGetMetalBundle() classNamed:@"MTLRenderPassDescriptor"] renderPassDescriptor];
    mtlRenderDescriptor.colorAttachments[0].texture = CVMetalTextureGetTexture(mtlPreviewTexture);
    mtlRenderDescriptor.colorAttachments[0].loadAction = MTLLoadActionClear;
    mtlRenderDescriptor.colorAttachments[0].storeAction = MTLStoreActionStore;
    mtlRenderDescriptor.colorAttachments[0].clearColor = MTLClearColorMake(0.0, 0.0, 0.0, 1.0);
    NSLog(@"%s", "NatCam Native: Generated FastRead Context");
}

bool GenerateVertexAttributesGL () {
    glGenBuffers(1, &glVertexBuffer);
    if (glVertexBuffer <= 0) return false;
    glBindBuffer(GL_ARRAY_BUFFER, glVertexBuffer);
    glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * 8, vertexData, GL_STATIC_DRAW);
    glBindBuffer(GL_ARRAY_BUFFER, 0);
    glGenBuffers(1, &glTextureBuffer);
    if (glTextureBuffer <= 0) return false;
    glBindBuffer(GL_ARRAY_BUFFER, glTextureBuffer);
    glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * 8, textureData, GL_STATIC_DRAW);
    glBindBuffer(GL_ARRAY_BUFFER, 0);
    NSLog(@"%s", "NatCam Native: Generated Attribute Buffers");
    return true;
}

bool GenerateFBOGL () {
    if(glFrameBuffer <= 0) {
        glGenFramebuffers(1, &glFrameBuffer);
        if (glFrameBuffer > 0) {
            NSLog(@"%s", "NatCam Native: Generated FBO");
        }
        else {
            NSLog(@"%s", "NatCam Native Error: Failed to generate FBO");
        }
    }
    glBindFramebuffer(GL_FRAMEBUFFER, glFrameBuffer);
    glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, 0, 0);
    GenerateFastReadContextGL();
    int status = glCheckFramebufferStatus(GL_FRAMEBUFFER);
    if(status != GL_FRAMEBUFFER_COMPLETE) {
        NSLog(@"%s", "NatCam Native Error: Failed to generate valid FBO attachment from preview texture");
        return false;
    }
    return true;
}

void InitializePipeline () {
    if (initializedRendering) return;
    else initializedRendering = true;
    if(UnitySelectedRenderingAPI() == apiOpenGLES2 || UnitySelectedRenderingAPI() == apiOpenGLES3) {
        PreserveRenderingContextGL();
        if (!GenerateVertexAttributesGL()) {
            NSLog(@"%s", "NatCam Native Error: Vertex attributes creation failed");
        }
        if (!GenerateFBOGL()) {
            NSLog(@"%s", "NatCam Native Error: FBO creation failed");
        }
        RestoreRenderingContextGL();
    }
    else if (UnitySelectedRenderingAPI() == apiMetal) {
        InitializeMetal();
        GenerateFastReadContextMetal();
    }
    else {
        Log("NatCam Error: Congratulations, you found an easter egg");
    }
}

void TeardownPipeline () {
    if (!initializedRendering) return;
    else initializedRendering = false;
    if(UnitySelectedRenderingAPI() == apiOpenGLES2 || UnitySelectedRenderingAPI() == apiOpenGLES3) {
        glDeleteBuffers(1, &glVertexBuffer);
        glDeleteBuffers(1, &glTextureBuffer);
        glDeleteFramebuffers(1, &glFrameBuffer);
        glDeleteProgram(glProgram);
        glProgram = 0;
        if (RGBA32 != NULL) {
            delete [] RGBA32;
            RGBA32 = NULL;
        }
        if (Y4 != NULL) {
            delete [] Y4;
            Y4 = NULL;
        }
        if (UV2 !=  NULL) {
            delete [] UV2;
            UV2 = NULL;
        }
        CVOpenGLESTextureCacheFlush(glTextureCache, 0);
        CVPixelBufferRelease(glRenderBuffer);
        GLAssert("Tore down rendering pipeline");
    }
    else if (UnitySelectedRenderingAPI() == apiMetal) {
        [mtlVertexBuffer setPurgeableState:MTLPurgeableStateEmpty];
        [mtlTextureBuffer setPurgeableState:MTLPurgeableStateEmpty];
        [mtlIndexBuffer setPurgeableState:MTLPurgeableStateEmpty];
        [mtlRotUniform setPurgeableState:MTLPurgeableStateEmpty];
        [mtlFlipUniform setPurgeableState:MTLPurgeableStateEmpty];
        mtlVertexBuffer = nil;
        mtlTextureBuffer = nil;
        mtlIndexBuffer = nil;
        mtlRotUniform = nil;
        mtlFlipUniform = nil;
        mtlRenderDescriptor = nil;
        mtlPipelineState = nil;
        if (RGBA32 != NULL) {
            delete [] RGBA32;
            RGBA32 = NULL;
        }
        if (Y4 != NULL) {
            delete [] Y4;
            Y4 = NULL;
        }
        if (UV2 !=  NULL) {
            delete [] UV2;
            UV2 = NULL;
        }
        CVMetalTextureCacheFlush(mtlTextureCache, 0);
        CVPixelBufferRelease(mtlRenderBuffer);
    }
    else {
        Log("NatCam Error: Congratulations, you found an easter egg");
    }
}

bool FrameBufferIsActiveGL () {
    if (glFrameBuffer <= 0) {
        NSLog(@"%s", "NatCam Native Error: FBO not created");
        return false;
    }
    return true;
}

void CheckDimensions() {
    if (UnitySelectedRenderingAPI() == apiOpenGLES2 || UnitySelectedRenderingAPI() == apiOpenGLES3) {
        size_t w = CVPixelBufferGetWidth(glRenderBuffer);
        size_t h = CVPixelBufferGetHeight(glRenderBuffer);
        if (w == _width && h == _height) return;
        Assert([[NSString stringWithFormat:@"Resizing rendering pipeline from %ix%i to %ix%i", (int)w, (int)h, (int)_width, (int)_height] UTF8String]);
        if (RGBA32 != NULL) {
            delete [] RGBA32;
            RGBA32 = NULL;
        }
        CVOpenGLESTextureCacheFlush(glTextureCache, 0);
        CVPixelBufferRelease(glRenderBuffer);
        NSDictionary* options = @{	(__bridge NSString*)kCVPixelBufferPixelFormatTypeKey : @(kCVPixelFormatType_32BGRA),
                                    (__bridge NSString*)kCVPixelBufferWidthKey : @(_width),
                                    (__bridge NSString*)kCVPixelBufferHeightKey : @(_height),
                                    (__bridge NSString*)kCVPixelBufferOpenGLESCompatibilityKey : @(YES),
                                    (__bridge NSString*)kCVPixelBufferIOSurfacePropertiesKey : @{}
                                    };
        CVPixelBufferCreate(kCFAllocatorDefault, _width, _height, kCVPixelFormatType_32BGRA, (__bridge CFDictionaryRef)options, &glRenderBuffer);
        CVReturn err = CVOpenGLESTextureCacheCreateTextureFromImage (kCFAllocatorDefault, glTextureCache, glRenderBuffer,
                                                            NULL,
                                                            GL_TEXTURE_2D,
                                                            GL_RGBA,
                                                            (int)_width,
                                                            (int)_height,
                                                            GL_RGBA,
                                                            GL_UNSIGNED_BYTE,
                                                            0,
                                                            &glPreviewTexture);
        if (err) {
            NSLog(@"NatCam Native Error: Failed to resize preview texture: %d", err);
            return;
        }
        glBindTexture(CVOpenGLESTextureGetTarget(glPreviewTexture), CVOpenGLESTextureGetName(glPreviewTexture));
        glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
        glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
        glBindFramebuffer(GL_FRAMEBUFFER, glFrameBuffer);
        glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, CVOpenGLESTextureGetName(glPreviewTexture), 0);
    }
    else if (UnitySelectedRenderingAPI() == apiMetal) {
        size_t w = CVPixelBufferGetWidth(mtlRenderBuffer);
        size_t h = CVPixelBufferGetHeight(mtlRenderBuffer);
        if (w == _width && h == _height) return;
        Assert([[NSString stringWithFormat:@"Resizing rendering pipeline from %ix%i to %ix%i", (int)w, (int)h, (int)_width, (int)_height] UTF8String]);
        if (RGBA32 != NULL) {
            delete [] RGBA32;
            RGBA32 = NULL;
        }
        CVMetalTextureCacheFlush(mtlTextureCache, 0);
        CVPixelBufferRelease(mtlRenderBuffer);
        NSDictionary* options = @{	(__bridge NSString*)kCVPixelBufferPixelFormatTypeKey : @(kCVPixelFormatType_32BGRA),
                                    (__bridge NSString*)kCVPixelBufferWidthKey : @(_width),
                                    (__bridge NSString*)kCVPixelBufferHeightKey : @(_height),
                                    (__bridge NSString*)kCVPixelBufferMetalCompatibilityKey : @(YES),
                                    (__bridge NSString*)kCVPixelBufferIOSurfacePropertiesKey : @{}
                                    };
        CVPixelBufferCreate(kCFAllocatorDefault, _width, _height, kCVPixelFormatType_32BGRA, (__bridge CFDictionaryRef)options, &mtlRenderBuffer);
        CVReturn err = CVMetalTextureCacheCreateTextureFromImage (kCFAllocatorDefault, mtlTextureCache, mtlRenderBuffer,
                                                         NULL,
                                                         MTLPixelFormatRGBA8Unorm,
                                                         (int)_width,
                                                         (int)_height,
                                                         0,
                                                         &mtlPreviewTexture);
        if (err) {
            NSLog(@"NatCam Native Error: Failed to resize preview texture: %d", err);
            return;
        }
        mtlRenderDescriptor.colorAttachments[0].texture = CVMetalTextureGetTexture(mtlPreviewTexture);
    }
    else Log("NatCam Error: You have found an easter egg");
}

void BindTexturesGL () {
    if (YTexGL > 0) {
        glBindTexture(GL_TEXTURE_2D, YTexGL);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    }
    if (UVTexGL > 0) {
        glBindTexture(GL_TEXTURE_2D, UVTexGL);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    }
}

GLuint LoadShaderGL (GLuint type, string shaderSrc) {
    GLuint shader;
    GLint compiled;
    shader = glCreateShader(type);
    if (shader == 0) {
        NSLog(@"%s", "NatCam Native Error: Unable to create shader object");
        return 0;
    }
    glShaderSource(shader, 1, &shaderSrc, NULL);
    glCompileShader(shader);
    glGetShaderiv(shader, GL_COMPILE_STATUS, &compiled);
    //Error checking
    if (compiled == 0) {
        NSLog(@"%s", "NatCam Native Error: Failed to compile shader");
        glDeleteShader(shader);
        return 0;
    }
    return shader;
}

GLint LoadProgramGL () {
    GLuint vertexShad;
    GLuint fragmentShad;
    GLuint programObject;
    GLint linked;
    vertexShad = LoadShaderGL(GL_VERTEX_SHADER, vertexShaderGL);
    if (vertexShad == 0) {
        //Error handled
        return 0;
    }
    fragmentShad = LoadShaderGL(GL_FRAGMENT_SHADER, fragmentShaderGL);
    if (fragmentShad == 0) {
        //Error handled
        glDeleteShader(vertexShad);
        return 0;
    }
    programObject = glCreateProgram();
    if (programObject == 0) {
        NSLog(@"%s", "NatCam Native Error: Failed to create GL program");
        return 0;
    }
    glAttachShader(programObject, vertexShad);
    glAttachShader(programObject, fragmentShad);
    glLinkProgram(programObject);
    glGetProgramiv(programObject, GL_LINK_STATUS, &linked);
    if (linked == 0) {
        NSLog(@"%s", "NatCam Native Error: Linking GL program failed");
        glDeleteProgram(programObject);
        return 0;
    }
    glYUniform = glGetUniformLocation(programObject, "y_texture");
    glUVUniform = glGetUniformLocation(programObject, "uv_texture");
    glRotUniform = glGetUniformLocation(programObject, "rot");
    glFlipUniform = glGetUniformLocation(programObject, "flipy");
    glPositionLoc = glGetAttribLocation(programObject, "a_position");
    glTexCoordLoc = glGetAttribLocation(programObject, "a_texCoord");
    glDeleteShader(vertexShad);
    glDeleteShader(fragmentShad);
    return programObject;
}

void DrawMetal () {
    if (YTexMtl == nil || UVTexMtl == nil) return;
    ((float*)[mtlRotUniform contents])[0] = previewRotation;
    ((float*)[mtlFlipUniform contents])[0] = (float)previewFlip;
    UnityEndCurrentMTLCommandEncoder();
    bool commit = UnityCurrentMTLCommandBuffer() == nil; //Sometimes Unity's command buffer is null, so we create ours
    id<MTLCommandBuffer> buffer = commit ? [UnityGetMetalCommandQueue() commandBuffer] : UnityCurrentMTLCommandBuffer();
    id<MTLRenderCommandEncoder> encoder = [buffer renderCommandEncoderWithDescriptor:mtlRenderDescriptor];
    [encoder setRenderPipelineState:mtlPipelineState];
    [encoder setCullMode: MTLCullModeNone];
    [encoder setVertexBuffer:mtlVertexBuffer offset:0 atIndex:0];
    [encoder setVertexBuffer:mtlTextureBuffer offset:0 atIndex:1];
    [encoder setVertexBuffer:mtlRotUniform offset:0 atIndex:2];
    [encoder setVertexBuffer:mtlFlipUniform offset:0 atIndex:3];
    [encoder setFragmentTexture:YTexMtl atIndex:0];
    [encoder setFragmentTexture:UVTexMtl atIndex:1];
    [encoder drawIndexedPrimitives: MTLPrimitiveTypeTriangle indexCount: 6 indexType: MTLIndexTypeUInt16 indexBuffer: mtlIndexBuffer indexBufferOffset: 0];
    [encoder endEncoding];
    if (commit) [buffer commit]; //Unity will commit its command buffer
    encoder = nil;
    buffer = nil;
}

void DrawGL () {
    if (YTexGL == 0 || UVTexGL == 0) return;
    glBindFramebuffer(GL_FRAMEBUFFER, glFrameBuffer);
    glViewport(0, 0, (int)_width, (int)_height);
    GLAssert("Bound active frame buffer");
    glDisable(GL_BLEND);
    glDisable(GL_DEPTH_TEST);
    glDepthMask(false);
    glDisable(GL_CULL_FACE);
    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
    glPolygonOffset(0.0f, 0.0f);
    glDisable(GL_POLYGON_OFFSET_FILL);
    if (glProgram <= 0) {
        glProgram = LoadProgramGL();
        if (glProgram <= 0) {
            NSLog(@"%s", "NatCam Native Error: Creating GL program failed");
            return;
        }
    }
    glUseProgram(glProgram);
    glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
    glClear(GL_COLOR_BUFFER_BIT);
    glActiveTexture(GL_TEXTURE0);
    glBindTexture(GL_TEXTURE_2D, YTexGL);
    GLAssert("Bound C0 texture for drawing");
    glActiveTexture(GL_TEXTURE1);
    glBindTexture(GL_TEXTURE_2D, UVTexGL);
    GLAssert("Bound C1 texture for drawing");
    glUniform1i(glYUniform, 0);
    GLAssert("Assigned C0 texture shader resource for drawing");
    glUniform1i(glUVUniform, 1);
    GLAssert("Assigned C1 texture shader resource for drawing");
    glUniform1f(glRotUniform, previewRotation);
    glUniform1f(glFlipUniform, (float)previewFlip);
    GLAssert("Assigned flip term resource for drawing");
    glBindBuffer(GL_ARRAY_BUFFER, glVertexBuffer);
    GLAssert("Bound vertex buffer for drawing");
    glEnableVertexAttribArray(glPositionLoc);
    GLAssert("Bound vertex attributes handle for drawing");
    glVertexAttribPointer(glPositionLoc, 2, GL_FLOAT, GL_FALSE, 0, 0);
    GLAssert("Bound vertex coordinates for drawing");
    glBindBuffer(GL_ARRAY_BUFFER, glTextureBuffer);
    GLAssert("Bound texture buffer for drawing");
    glEnableVertexAttribArray(glTexCoordLoc);
    GLAssert("Bound texture attributes handle for drawing");
    glVertexAttribPointer(glTexCoordLoc, 2, GL_FLOAT, GL_FALSE, 0, 0);
    GLAssert("Bound texture coordinates for drawing");
    glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_SHORT, indexData);
    GLAssert("Performed drawing");
    glFinish(); //Prevents tearing
    GLAssert("Finished drawing");
    //glFlush();
    //GLAssert("Flushed pipeline");
    glBindBuffer(GL_ARRAY_BUFFER, 0);
    GLAssert("Unbound vertex buffer");
    glActiveTexture(GL_TEXTURE0);
    GLAssert("Activated C0 texture for unbind");
    glBindTexture(GL_TEXTURE_2D, 0);
    GLAssert("Unbound C0 texture");
    glActiveTexture(GL_TEXTURE1);
    GLAssert("Activated C1 texture for unbind");
    glBindTexture(GL_TEXTURE_2D, 0);
    GLAssert("Unbound C1 texture");
    glDisableVertexAttribArray(glPositionLoc);
    GLAssert("Disabled vertex uniform 0");
    glDisableVertexAttribArray(glTexCoordLoc);
    GLAssert("Disabled vertex uniform 1");
}

void UpdateRGBABuffer () {
    RGBA32W = _width;
    RGBA32H = _height;
    if (!readablePreview) return;
    CVPixelBufferRef buffer = UnitySelectedRenderingAPI() == apiMetal ? mtlRenderBuffer : glRenderBuffer;
    RGBA32W = CVPixelBufferGetBytesPerRow(buffer) / 4;
    RGBA32H = CVPixelBufferGetHeight(buffer);
    RGBA32S = RGBA32W * RGBA32H * 4;
    CVPixelBufferLockBaseAddress(buffer, kCVPixelBufferLock_ReadOnly);
    void* src = CVPixelBufferGetBaseAddress(buffer);
    if (RGBA32 == NULL) RGBA32 = new GLubyte[RGBA32S];
    memcpy(RGBA32, src, RGBA32S);
    CVPixelBufferUnlockBaseAddress(buffer, kCVPixelBufferLock_ReadOnly);
}

void UpdateComponentBuffers (CVPixelBufferRef pixelBuffer) {
    if (!componentUpdate) return;
    Y4W = CVPixelBufferGetWidthOfPlane(pixelBuffer, 0);
    Y4H = CVPixelBufferGetHeightOfPlane(pixelBuffer, 0);
    Y4S = Y4H * CVPixelBufferGetBytesPerRowOfPlane(pixelBuffer, 0);
    UV2W = CVPixelBufferGetWidthOfPlane(pixelBuffer, 1);
    UV2H = CVPixelBufferGetHeightOfPlane(pixelBuffer, 1);
    UV2S = UV2H * CVPixelBufferGetBytesPerRowOfPlane(pixelBuffer, 1);
    CVPixelBufferLockBaseAddress(pixelBuffer, kCVPixelBufferLock_ReadOnly);
    {
        void* src = CVPixelBufferGetBaseAddressOfPlane(pixelBuffer, 0);
        if (Y4 == NULL) Y4 = new GLubyte[Y4S];
        if (sizeof(Y4) != Y4S) { //In case we switch cameras with different resolutions and thus different sizes
            delete [] Y4;
            Y4 = new GLubyte[Y4S];
        }
        memcpy(Y4, src, Y4S);
    }
    {
        void* src = CVPixelBufferGetBaseAddressOfPlane(pixelBuffer, 1);
        if (UV2 == NULL) UV2 = new GLubyte[UV2S];
        if (sizeof(UV2) != UV2S) {
            delete [] UV2;
            UV2 = new GLubyte[UV2S];
        }
        memcpy(UV2, src, UV2S);
    }
    CVPixelBufferUnlockBaseAddress(pixelBuffer, kCVPixelBufferLock_ReadOnly);
    void* Y4GPU = UnitySelectedRenderingAPI() == apiMetal ? (__bridge void*)YTexMtl : (void*)(uintptr_t)YTexGL;
    //Notify Unity
    _componentUpdateCallback((void*)Y4, (void*)UV2, Y4GPU, (int)Y4W, (int)Y4H, (int)Y4S, (int)UV2W, (int)UV2H, (int)UV2S);
}

void UpdatePreview () {
    Assert("Camera preview update");
    if(UnitySelectedRenderingAPI() == apiOpenGLES2 || UnitySelectedRenderingAPI() == apiOpenGLES3) {
        if (!FrameBufferIsActiveGL()) return;
        PreserveRenderingContextGL();
        CheckDimensions();
        BindTexturesGL();
        DrawGL();
        RestoreRenderingContextGL();
        UpdateRGBABuffer();
        _updateCallback((void*)(uintptr_t)CVOpenGLESTextureGetName(glPreviewTexture), (void*)RGBA32, (int)RGBA32W, (int)RGBA32H, (int)RGBA32S);
    }
    else if (UnitySelectedRenderingAPI() == apiMetal) {
        CheckDimensions();
        DrawMetal();
        UpdateRGBABuffer();
        _updateCallback((__bridge void*)CVMetalTextureGetTexture(mtlPreviewTexture), (void*)RGBA32, (int)RGBA32W, (int)RGBA32H, (int)RGBA32S);
    }
    else {
        Log("NatCam Error: Congratulations, you found an easter egg");
    }
}

void UpdateComponentTextures (CMSampleBufferRef sampleBuffer) {
    CVPixelBufferRef pixelBuffer = CMSampleBufferGetImageBuffer(sampleBuffer);
    if(UnitySelectedRenderingAPI() == apiOpenGLES2 || UnitySelectedRenderingAPI() == apiOpenGLES3) {
        if (glTextureCache == nil) return;
        {
            size_t width = CVPixelBufferGetWidthOfPlane(pixelBuffer, 0);
            size_t height = CVPixelBufferGetHeightOfPlane(pixelBuffer, 0);
            CVOpenGLESTextureRef texture = NULL;
            CVReturn status = CVOpenGLESTextureCacheCreateTextureFromImage(kCFAllocatorDefault,
                                                                           glTextureCache,
                                                                           pixelBuffer,
                                                                           NULL,
                                                                           GL_TEXTURE_2D,
                                                                           GL_LUMINANCE,
                                                                           (int)width,
                                                                           (int)height,
                                                                           GL_LUMINANCE,
                                                                           GL_UNSIGNED_BYTE,
                                                                           0,
                                                                           &texture);
            if(status == kCVReturnSuccess) {
                YTexGL = CVOpenGLESTextureGetName(texture);
                CFRelease(texture);
                Assert("Updated c0 buffer");
            }
            else {
                NSLog(@"%s", "NatCam Native Error: Failed to update c0 buffer GL");
            }
        }
        {
            size_t width = CVPixelBufferGetWidthOfPlane(pixelBuffer, 1);
            size_t height = CVPixelBufferGetHeightOfPlane(pixelBuffer, 1);
            CVOpenGLESTextureRef texture = NULL;
            CVReturn status = CVOpenGLESTextureCacheCreateTextureFromImage(kCFAllocatorDefault,
                                                                           glTextureCache,
                                                                           pixelBuffer,
                                                                           NULL,
                                                                           GL_TEXTURE_2D,
                                                                           GL_LUMINANCE_ALPHA,
                                                                           (int)width,
                                                                           (int)height,
                                                                           GL_LUMINANCE_ALPHA,
                                                                           GL_UNSIGNED_BYTE,
                                                                           1,
                                                                           &texture);
            if(status == kCVReturnSuccess) {
                UVTexGL = CVOpenGLESTextureGetName(texture);
                CFRelease(texture);
                Assert("Updated c1 buffer");
            }
        }
    }
    else if (UnitySelectedRenderingAPI() == apiMetal) {
        if (mtlTextureCache == nil) return;
        {
            size_t width = CVPixelBufferGetWidthOfPlane(pixelBuffer, 0);
            size_t height = CVPixelBufferGetHeightOfPlane(pixelBuffer, 0);
            CVMetalTextureRef texture = NULL;
            CVReturn status = CVMetalTextureCacheCreateTextureFromImage(kCFAllocatorDefault,
                                                                        mtlTextureCache,
                                                                        pixelBuffer,
                                                                        NULL,
                                                                        MTLPixelFormatR8Unorm,
                                                                        width,
                                                                        height,
                                                                        0,
                                                                        &texture);
            if(status == kCVReturnSuccess) {
                YTexMtl = CVMetalTextureGetTexture(texture);
                CFRelease(texture);
                Assert("Updated c0 buffer");
            }
            else {
                NSLog(@"%s", "NatCam Native Error: Failed to update c0 buffer Metal");
            }
        }
        {
            size_t width = CVPixelBufferGetWidthOfPlane(pixelBuffer, 1);
            size_t height = CVPixelBufferGetHeightOfPlane(pixelBuffer, 1);
            CVMetalTextureRef texture = NULL;
            CVReturn status = CVMetalTextureCacheCreateTextureFromImage(kCFAllocatorDefault,
                                                                        mtlTextureCache,
                                                                        pixelBuffer,
                                                                        NULL,
                                                                        MTLPixelFormatRG8Unorm,
                                                                        width,
                                                                        height,
                                                                        1,
                                                                        &texture);
            if(status == kCVReturnSuccess) {
                UVTexMtl = CVMetalTextureGetTexture(texture);
                CFRelease(texture);
                Assert("Updated c1 buffer");
            }
            else {
                NSLog(@"%s", "NatCam Native Error: Failed to update c1 buffer Metal");
            }
        }
    }
    UpdateComponentBuffers(pixelBuffer);
}

void UpdateDimensions (CMSampleBufferRef sampleBuffer) {
    CVImageBufferRef imageBuffer = CMSampleBufferGetImageBuffer(sampleBuffer);
    _width = CVPixelBufferGetWidth(imageBuffer);
    _height = CVPixelBufferGetHeight(imageBuffer);
}

#pragma mark --NatCamU-NatCamS Rendering LLAPI--
extern "C" void UnitySetGraphicsDevice(void* device, int deviceType, int eventType) {
    
}

extern "C" void NatCamNativeCallback (int eventID) {
    switch (eventID) {
        case 1: //Initialize Pipeline
            InitializePipeline();
            break;
        case 2: //Update Preview Texture
            if (!disabledRendering) UpdatePreview();
            break;
        case 3: //Teardown Pipeline
            TeardownPipeline();
            break;
    }
}

@interface NatCamAppController : UnityAppController {}
- (void)shouldAttachRenderDelegate;
@end

@implementation NatCamAppController
- (void)shouldAttachRenderDelegate;
{
    Log("NatCam: Registered for Native Rendering");
    UnityRegisterRenderingPlugin(&UnitySetGraphicsDevice, &NatCamNativeCallback);
}
@end

IMPL_APP_CONTROLLER_SUBCLASS(NatCamAppController)
