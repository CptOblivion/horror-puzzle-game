Shader "Unlit/OCCLUSION"
{
    SubShader
    {
        Pass
        {
            Name "DepthOnly"
			Tags {"LightMode" = "DepthOnly"}

			ZWrite On
			ColorMask 0
        }
    }
}
