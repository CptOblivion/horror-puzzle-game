Shader "Unlit/OCCLUSION"
{
    SubShader
    {
        Pass
        {
            Name "DepthOnly"
			Tags {"Queue" = "Geometry-1000"}

			ZWrite On
			ColorMask 0
        }
    }
}
