// Unlit alpha-cutout shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Transparent Cutout" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
				
				if(col.a>_Cutoff&&col.a<1.0){
				
					float coef = (col.a-_Cutoff)/(1.0-_Cutoff);
															
					float offSX = 0.01;
  					float offSY = 0.01;
					col*=4;
					
					col		+= 2.0 * tex2D(_MainTex, i.texcoord + float2(+offSX, 0.0));
					col		+= 2.0 * tex2D(_MainTex, i.texcoord + float2(-offSX, 0.0));
					col		+= 2.0 * tex2D(_MainTex, i.texcoord + float2(0.0, +offSY));
					col		+= 2.0 * tex2D(_MainTex, i.texcoord + float2(0.0, -offSY));
					col		+= tex2D(_MainTex, i.texcoord + float2(+offSX, +offSY));
					col		+= tex2D(_MainTex, i.texcoord + float2(-offSX, +offSY));
					col		+= tex2D(_MainTex, i.texcoord + float2(-offSX, -offSY));
					col		+= tex2D(_MainTex, i.texcoord + float2(+offSX, -offSY));
					col = col / 16.0;
					col = fixed4(col.x,col.y,col.z,coef);
						
				}else if(col.a<_Cutoff){
					col = fixed4(0.0f,0.0f,0.0f,0.0f);
				}				
				return col;
			}
		ENDCG
	}
}

}
