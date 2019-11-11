Shader "Hidden/DownscaleTexure"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_TargetWidth("TargetWidth", Int) = 480
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;
			uint _TargetWidth;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = {0,0,0,1};
                
				uint sampleCount =_MainTex_TexelSize.z / _TargetWidth;
				float sampleOffset = sampleCount / 2 - .5;

				float2 position = {0,0};
				for (uint x = 0; x < sampleCount; x++){
					for (uint y = 0; y < sampleCount; y++){
						position = float2((x - sampleOffset) * _MainTex_TexelSize.x, (y - sampleOffset) * _MainTex_TexelSize.y);
						//let's do this math in linear space:
						col += pow(tex2D(_MainTex, i.uv + position), .4545);
					}
				}
				col /= sampleCount * sampleCount;
				col = pow(col, 2.2); //...and back to gamma

                return col;
            }
            ENDCG
        }
    }
}
