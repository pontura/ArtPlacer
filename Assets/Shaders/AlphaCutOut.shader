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
				
				float squareFL = 0.36f;
				float squareFR = 0.36f;
				float squareFT = 0.36f;
				float squareFB = 0.36f;
				float textDif = 0.45f;
				float viewL = 0.225f;
				float viewR = 0.33f;
				float viewT = 0.32f;
				float viewB = 0.215f;				
				float viewDifL = 0.03f;
				float viewDifR = 0.03f;
				float viewDifT = 0.03f;
				float viewDifB = 0.015f;
				
				float l = length(i.texcoord-half2(0.5f,0.5f));
				float left = fmod(i.texcoord.x-textDif+0.01,viewL);
				float bottom = fmod(i.texcoord.y-textDif,viewB);
				float right = fmod((i.texcoord.x-textDif)-0.005,viewR);
				float top = fmod(i.texcoord.y-textDif,viewT);
				if(_Left>0){
					if(l<squareFL &&left<-1*(viewL-viewDifL)){
						float coef = (abs(left)-(viewL-viewDifL))/viewDifL;
						//col = fixed4(0.0f,0.0f,0.0f,0.6f-coef);
						col = fixed4(0.0f,0.0f,0.0f,0.3f-(coef*0.3f));
						
					}
				}else{
					if(l<squareFR &&right>(viewR-viewDifR)){
						float coef = (abs(right)-(viewR-viewDifR))/viewDifR;
						col = fixed4(0.0f,0.0f,0.0f,0.3f-(coef*0.3f));
						//col = fixed4(0.0f,0.0f,0.0f,1.0f-coef);						
					}
				}
				
				if(_Top>0){
					if(l<squareFT &&top>(viewT-viewDifT)){
						float coef = (abs(top)-(viewT-viewDifT))/viewDifT;
						col = fixed4(0.0f,0.0f,0.0f,0.5f-coef);
					}
				}else{
					if(l<squareFB &&bottom<-1*(viewB-viewDifB)){
						float coef = (abs(bottom)-(viewB-viewDifB))/viewDifB;
						//col = fixed4(0.0f,0.0f,0.0f,0.5f-coef);						
						col = fixed4(0.0f,0.0f,0.0f,0.2f-(coef*0.2f));
					}
				}
						
				return col;
			}
		ENDCG
	}
}

}
