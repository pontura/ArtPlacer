//
//  NatCamRendering.h
//  NatCamS Rendering Pipeline
//
//  Created by Yusuf on 1/9/16.
//  Copyright (c) 2016 Yusuf Olokoba
//

#include <OpenGLES/ES2/gl.h>
#include <OpenGLES/ES2/glext.h>
#include <CoreVideo/CVOpenGLESTextureCache.h>
#include <stdlib.h>
#include <stdint.h>

#import "NatCamDecls.h"
#import "UnityAppController.h"
#include "UnityInterface.h"
#include "UnityMetalSupport.h"

#if UNITY_CAN_USE_METAL
#include <CoreVideo/CVMetalTextureCache.h>
#endif

//Global
extern bool initializedRendering;
extern bool readablePreview;
extern bool componentUpdate;
extern bool disabledRendering;
extern size_t _width;
extern size_t _height;

//Buffers
extern GLubyte* RGBA32;
extern GLubyte* Y4;
extern GLubyte* UV2;
extern size_t RGBA32W, RGBA32H, RGBA32S;
extern size_t Y4W, Y4H, Y4S;
extern size_t UV2W, UV2H, UV2S;

//NatCamU-NatCamX bridge
extern "C" void SetRotation (float rotation, float flip);
extern "C" void PreviewBuffer (void** RGBAPtr, int* RGBAS);

//Control
void UpdateComponentTextures (CMSampleBufferRef sampleBuffer);
void UpdateDimensions (CMSampleBufferRef sampleBuffer);

//Util
static string vertexShaderGL =
"#version 100                                       \n"
"#ifdef GL_ES                                       \n"
"precision lowp float;                              \n"
"#endif                                             \n"

"attribute lowp vec4 a_position;                    \n"
"attribute lowp vec2 a_texCoord;                    \n"
"varying lowp vec2 v_texCoord;                      \n"
"uniform float rot;                                 \n"
"uniform float flipy;                               \n"

"void main() {                                      \n"
"   vec2 modCoord = a_texCoord;                     \n"
"   modCoord -= vec2(0.5, 0.5);                     \n"
"   float Angle = 3.1415926 * rot;                  \n"
"   float s = sin(Angle);                           \n"
"   float c = cos(Angle);                           \n"
"   mat2 rotationMatrix = mat2(c, s, -s ,c);        \n"
"   rotationMatrix *= 0.5;                          \n"
"   rotationMatrix += mat2(0.5, 0.5, 0.5, 0.5);     \n"
"   rotationMatrix *= 2.0;                          \n"
"   rotationMatrix -= mat2(1.0, 1.0, 1.0, 1.0);     \n"
"   modCoord = modCoord * rotationMatrix;           \n"
"   modCoord += vec2(0.5, 0.5);                     \n"
"   gl_Position = a_position;                       \n"
"   if (flipy < 0.5) modCoord.y = 1.0 - modCoord.y; \n"
"   v_texCoord = modCoord;                          \n"
"}                                                  \n";

static string fragmentShaderGL =
"#version 100                                       \n"
"#ifdef GL_ES                                       \n"
"precision lowp float;                              \n"
"#endif                                             \n"

"varying lowp vec2 v_texCoord;                      \n"
"uniform sampler2D y_texture;                       \n"
"uniform sampler2D uv_texture;                      \n"
"uniform float rot;                                 \n"
"uniform float flipy;                               \n"

"void main (void){                                  \n"
"   float r, g, b, y, u, v;                         \n"
"   y = texture2D(y_texture, v_texCoord).r;         \n"
"   u = texture2D(uv_texture, v_texCoord).a - 0.5;  \n"
"   v = texture2D(uv_texture, v_texCoord).r - 0.5;  \n"
"   r = y + 1.13983*v;                              \n"
"   g = y - 0.39465*u - 0.58060*v;                  \n"
"   b = y + 2.03211*u;                              \n"
"   lowp vec4 color = vec4(b, g, r, 1.0);           \n" //Swizzle //b,g,r,1
"   gl_FragColor = color;                           \n"
"}                                                  \n";

static string shaderMetal =
"#include <metal_stdlib>                            \n"
"#include <simd/simd.h>                             \n"

"using namespace metal;                             \n"

"typedef struct {                                   \n"
"   float4 out_pos [[position]];                    \n"
"   float2 texcoord;                                \n"
"} VertOut;                                         \n"

"vertex VertOut vShader (                           \n"
"constant float2 *a_position [[buffer(0)]],         \n"
"constant float2 *a_texCoord [[buffer(1)]],         \n"
"constant float *rot [[buffer(2)]],                 \n"
"constant float *flipy [[buffer(3)]],               \n"
"uint vertexID [[vertex_id]]) {                     \n"
"   float2 modCoord = a_texCoord[vertexID];         \n"
"   modCoord.y = 1 - modCoord.y;                    \n" //Y inversion on Metal
"   modCoord -= float2(0.5, 0.5);                   \n"
"   float Angle = 3.1415926 * rot[0];               \n"
"   float s = sin(Angle);                           \n"
"   float c = cos(Angle);                           \n"
"   float2x2 rotationMatrix = float2x2(float2(c, s), float2(-s ,c));    \n"
"   rotationMatrix *= 0.5;                          \n"
"   rotationMatrix += float2x2(float2(0.5, 0.5), float2(0.5, 0.5));     \n"
"   rotationMatrix *= 2.0;                          \n"
"   rotationMatrix -= float2x2(float2(1.0, 1.0), float2(1.0, 1.0));     \n"
"   modCoord = modCoord * rotationMatrix;           \n"
"   modCoord += float2(0.5, 0.5);                   \n"
"   if (flipy[0] < 0.5) modCoord.y = 1.0 - modCoord.y;                  \n"
"   VertOut out;                                    \n"
"   out.out_pos = float4(a_position[vertexID], 0.0, 1.0);               \n"
"   out.texcoord = modCoord;                        \n"
"   return out;                                     \n"
"}                                                  \n"

"constexpr sampler s (address::clamp_to_edge, filter::linear);          \n"
"fragment half4 fShader (                           \n"
"VertOut input [[stage_in]],                        \n"
"texture2d<half> y_texture [[texture(0)]],          \n"
"texture2d<half> uv_texture [[texture(1)]]) {       \n"
"   half r, g, b, y, u, v;                          \n"
"   y = y_texture.sample(s, input.texcoord).r;      \n"
"   u = uv_texture.sample(s, input.texcoord).g - 0.5;   \n"
"   v = uv_texture.sample(s, input.texcoord).r - 0.5;   \n"
"   r = y + 1.13983*v;                              \n"
"   g = y - 0.39465*u - 0.58060*v;                  \n"
"   b = y + 2.03211*u;                              \n"
"   half4 color = half4(b, g, r, 1.0);              \n" //Swizzle //b,g,r,1
"   return color;                                   \n"
"}                                                  \n";
