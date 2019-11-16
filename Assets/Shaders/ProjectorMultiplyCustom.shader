// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/MultiplyCustom" {
	Properties {
		_ShadowTex ("Cookie", 2D) = "gray" {}
		_FalloffTex ("FallOff", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)

		_BackfaceSharpness("Backface Sharpness", Range(0.5, 10)) = 5.0
		_BackfaceOffset("Backface Offset", Range(-1.0, 1.0)) = 0.0
	}
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend DstColor Zero
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float3 normal : TEXCOORD3;
				float4 pos : SV_POSITION;
			};

			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			
			//v2f vert (float4 vertex : POSITION)
			v2f vert (float4 vertex : POSITION, float3 normal : NORMAL)
			{

				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.uvShadow = mul (unity_Projector, vertex);
				o.uvFalloff = mul (unity_ProjectorClip, vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				o.normal = mul((float3x3)unity_Projector, normal);
				return o;
			}
			
			sampler2D _ShadowTex;
			sampler2D _FalloffTex;
			fixed4 _Color;

			float _BackfaceOffset;
			float _BackfaceSharpness;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 texS = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				texS = 1 - texS;
				texS *= 1 - _Color;
				texS *= _Color.a;
				texS = 1 - texS;
				texS.a = 1.0-texS.a;

				fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				
				half3 normal = normalize(i.normal.xyz / i.uvShadow.w);
                texF.a *= saturate(-normal.z * _BackfaceSharpness + (0.5 + _BackfaceOffset * _BackfaceSharpness * (1.0 + 0.5 / _BackfaceSharpness)));
 
				fixed4 res = lerp(fixed4(1,1,1,0), texS, texF.a);

				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(1,1,1,1));
				
				return res;
			}
			ENDCG
		}
	}
}

/*
Shader "Projector/AdditiveTint" {
	Properties {
		_Color ("Tint Color", Color) = (1,1,1,1)
		_Attenuation ("Falloff", Range(0.0, 1.0)) = 1.0
		_ShadowTex ("Cookie", 2D) = "gray" {}
	}
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend SrcAlpha One // Additive blending
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 pos : SV_POSITION;
			};
			
			float4x4 _Projector;
			float4x4 _ProjectorClip;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, vertex);
				o.uvShadow = mul (_Projector, vertex);
				return o;
			}
			
			sampler2D _ShadowTex;
			fixed4 _Color;
			float _Attenuation;
			
			fixed4 frag (v2f i) : SV_Target
			{
				// Apply alpha mask
				fixed4 texCookie = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				fixed4 outColor = _Color * texCookie.a;
				// Attenuation
				float depth = i.uvShadow.z; // [-1 (near), 1 (far)]
				return outColor * clamp(1.0 - abs(depth) + _Attenuation, 0.0, 1.0);
			}
			ENDCG
		}
	}
}
*/