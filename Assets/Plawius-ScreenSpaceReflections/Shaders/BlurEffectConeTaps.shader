// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/BlurEffectConeTap" {
	Properties { _MainTex ("", any) = "" {} }
	CGINCLUDE
	#include "UnityCG.cginc"

	#if UNITY_VERSION < 540
	half2 UnityStereoScreenSpaceUVAdjust(half2 uv, half4 st) {	return uv;	}
	float4 UnityObjectToClipPos(float3 pos) { return UnityObjectToClipPos(float4(pos, 1.0)); }
	#endif

	struct v2f {
		float4 pos : POSITION;
		half2 uv : TEXCOORD0;
		half2 taps[4] : TEXCOORD1; 
	};
	sampler2D _MainTex;
	half4 _MainTex_TexelSize;
	half4 _MainTex_ST;
	half4 _BlurOffsets;
	v2f vert( appdata_img v ) {
		v2f o; 
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord - _BlurOffsets.xy * _MainTex_TexelSize.xy;
		o.taps[0] = UnityStereoScreenSpaceUVAdjust(o.uv + _MainTex_TexelSize * _BlurOffsets.xy, _MainTex_ST);
		o.taps[1] = UnityStereoScreenSpaceUVAdjust(o.uv - _MainTex_TexelSize * _BlurOffsets.xy, _MainTex_ST);
		o.taps[2] = UnityStereoScreenSpaceUVAdjust(o.uv + _MainTex_TexelSize * _BlurOffsets.xy * half2(1,-1), _MainTex_ST);
		o.taps[3] = UnityStereoScreenSpaceUVAdjust(o.uv - _MainTex_TexelSize * _BlurOffsets.xy * half2(1,-1), _MainTex_ST);
		o.uv = UnityStereoScreenSpaceUVAdjust(o.uv, _MainTex_ST);
		return o;
	}
	half4 frag(v2f i) : COLOR {
	
		half4 color = tex2D(_MainTex, i.taps[0]);
		color += tex2D(_MainTex, i.taps[1]);
		color += tex2D(_MainTex, i.taps[2]);
		color += tex2D(_MainTex, i.taps[3]); 
		return color * 0.25;
	}
	ENDCG
	SubShader {
		 Pass {
			  ZTest Always Cull Off ZWrite Off
			  Fog { Mode off }      

			  CGPROGRAM
			  #pragma fragmentoption ARB_precision_hint_fastest
			  #pragma vertex vert
			  #pragma fragment frag
			  ENDCG
		  }
	}
	Fallback off
}