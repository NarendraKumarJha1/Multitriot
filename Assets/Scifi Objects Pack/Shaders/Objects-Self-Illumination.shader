Shader "Objects/Self-Illumination" {
    Properties {
        _MainTex ("Base (RGB) Self-Illumination (A)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
   }
    SubShader {
		Tags { "RenderType" = "Opaque" }
		Tags { "Queue" = "Geometry" } 
		
		/* Upgrade NOTE: commented out, possibly part of old style per-pixel lighting: Blend AppSrcAdd AppDstAdd */
		
		
		Pass {
			// no lights
			// Tags { "LightMode" = "VertexOrNone" }
			
			Material {
                Diffuse [_Color]
                Ambient [_Color]
            }
			Lighting On
			Cull Back

            SetTexture [_MainTex] {
                combine texture * primary
            }

            SetTexture [_MainTex] {
                combine texture lerp(texture) previous double
            }
        }
    }
}