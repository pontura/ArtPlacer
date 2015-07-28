Shader "Custom/textHomography"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Alpha (A)", 2D) = "white" {}
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
		
		matrixRow_1 ("matrixRow_1", Vector) = (1, 0, 0, 0)
		matrixRow_2 ("matrixRow_2", Vector) = (0, 1, 0, 0)
		matrixRow_3 ("matrixRow_3", Vector) = (0, 0, 1, 0)
		matrixRow_4 ("matrixRow_4", Vector) = (0, 0, 0, 1)
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType"="Plane"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex : POSITION;
					half4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : POSITION;
					half4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _Color;
				
				float4 matrixRow_1;
				float4 matrixRow_2;
				float4 matrixRow_3;
				float4 matrixRow_4;

				v2f vert (appdata_t v)
				{	float4x4 matrixH = { matrixRow_1.x, matrixRow_1.y, matrixRow_1.z, matrixRow_1.w,
								  matrixRow_2.x, matrixRow_2.y, matrixRow_2.z, matrixRow_2.w,
								  matrixRow_3.x, matrixRow_3.y, matrixRow_3.z, matrixRow_3.w,
								  matrixRow_4.x, matrixRow_4.y, matrixRow_4.z, matrixRow_4.w };
				
				
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.vertex = o.vertex / o.vertex.w;
					o.vertex = mul (matrixH, o.vertex);
					o.vertex.x = o.vertex.x / o.vertex.w;
					o.vertex.y = o.vertex.y / o.vertex.w;
					o.vertex.w = o.vertex.w / o.vertex.w;
                    //o.normal = v.normal;
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.color = v.color;
#ifdef UNITY_HALF_TEXEL_OFFSET
					o.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
					return o;
				}

				half4 frag (v2f i) : COLOR
				{
					half4 col = i.color;
					col.a *= tex2D(_MainTex, i.texcoord).a;
					col = col * _Color;
					clip (col.a - 0.01);
					return col;
				}
			ENDCG
		}
	}
}