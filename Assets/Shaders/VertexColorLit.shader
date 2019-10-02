Shader " Vertex Colored" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _Emission ("Emmisive Color", Color) = (0,0,0,0)
    _MainTex ("Base (RGB)", 2D) = "white" {}
}
 
SubShader {
    Pass {
        Material {
            Emission [_Emission]    
        }
        ColorMaterial AmbientAndDiffuse
        Lighting On
        SetTexture [_MainTex] {
            Combine texture * primary, texture * primary
        }
        SetTexture [_MainTex] {
            constantColor [_Color]
            Combine previous * constant DOUBLE, previous * constant
        } 
    }
}
}