// Unlit alpha-cutout shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Transparent Cutout" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_CutoffMarco ("Marco cutoff", Range(0,1)) = 0.5
	[Toggle] _Top ("Top or Bottom Shadow", float) = 0
	[Toggle] _Left ("Left or Right Shadow", float) = 0
}
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 150

	Lighting Off

	Pass {  
	 		// Dont write to the depth buffer
            ZWrite off
            // Don't write pixels we have already written.
            ZTest Less
            // Only render pixels less or equal to the value
            //AlphaTest LEqual [_Cutoff]

            // Set up alpha blending
            Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;				
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Cutoff;
			fixed _CutoffMarco;
			float _Top;
			float _Left;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				//clip(col.a - _Cutoff);				
				//if(col.a-_Cutoff<0){
				
				//if(col.a>_CutoffMarco&&col.a<_Cutoff){				
					//float coef = (col.a-_CutoffMarco)/(_Cutoff-_CutoffMarco);
					//col = fixed4(coef*col.x,coef*col.y,coef*col.z,coef);
					//col = fixed4(1.0f,0.0f,0.0f,1.0f);				
				//}else if(col.a>_Cutoff&&col.a<1.0){
				if(col.a>_Cutoff&&col.a<1.0){		
				
					float coef = (col.a-_Cutoff)/(1.0-_Cutoff);															
					
					col = fixed4(col.x,col.y,col.z,coef);
					//col = fixed4(0.0f,1.0f,0.0f,1.0f);
						
				}else if(col.a<_Cutoff){				
					col = fixed4(0.0f,0.0f,0.0f,0.0f);					
				}
				
				float squareF = 0.34f;
				float textDif = 0.45f;
				float view1F = 0.22f;
				float view2F = 0.32f;
				float viewDifF = 0.03f;
				
				float l = length(i.texcoord-half2(0.5f,0.5f));
				float left = fmod(i.texcoord.x-textDif,view1F);
				float bottom = fmod(i.texcoord.y-textDif,view1F);
				float right = fmod(i.texcoord.x-textDif,view2F);
				float top = fmod(i.texcoord.y-textDif,view2F);
				if(_Left>0){
					if(l<squareF &&left<-1*(view1F-viewDifF)){
						float coef = (abs(left)-(view1F-viewDifF))/viewDifF;
						col = fixed4(0.0f,0.0f,0.0f,0.6f-coef);
					}
				}else{
					if(l<squareF &&right>(view2F-viewDifF)){
						float coef = (abs(right)-(view2F-viewDifF))/viewDifF;
						col = fixed4(0.0f,0.0f,0.0f,0.6f-coef);
					}
				}
				
				if(_Top>0){
					if(l<squareF &&top>(view2F-viewDifF)){
						float coef = (abs(top)-(view2F-viewDifF))/viewDifF;
						col = fixed4(0.0f,0.0f,0.0f,0.6f-coef);
					}
				}else{
					if(l<squareF &&bottom<-1*(view1F-viewDifF)){
						float coef = (abs(bottom)-(view1F-viewDifF))/viewDifF;
						col = fixed4(0.0f,0.0f,0.0f,0.6f-coef);
					}
				}
						
				return col;
			}
		ENDCG
	}
}

}
