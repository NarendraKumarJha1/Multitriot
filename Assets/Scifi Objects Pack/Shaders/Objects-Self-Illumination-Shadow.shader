Shader "Objects/Self-Illumination-Shadow-x" {
    Properties {
        _MainTex ("Base (RGB) Self-Illumination (A)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
   }
    SubShader {
		Tags { "RenderType" = "Opaque" }
		Tags { "Queue" = "Geometry+1" } 
	    
		ZWrite On
		// Alphatest Greater 0.2
		// ColorMask RGBA
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			// no lights
			// Tags { "LightMode" = "VertexOrNone" }

			Material {
                Diffuse [_Color]
                Ambient [_Color]
            }
			Lighting On
			ColorMaterial AmbientAndDiffuse

			SetTexture [_MainTex] {
 				combine texture * primary, primary * previous
			}

            SetTexture [_MainTex] {
                combine texture lerp(texture) previous double, primary * previous
            }
        }
   }
}