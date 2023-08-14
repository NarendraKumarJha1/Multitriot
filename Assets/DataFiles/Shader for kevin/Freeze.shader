Shader "Unlit/Freeze"{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _GradientPoint ("Gradient Point", Vector) = (0, 0, 0, 0)
        _GradientDistance ("Gradient Distance", Range(0, 10)) = 5
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float distance : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _Color;
            float4 _GradientPoint;
            float _GradientDistance;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // Calculate the distance from the vertex to the gradient point
                o.distance = distance(v.vertex, _GradientPoint);
                return o;
            }

fixed4 frag (v2f i) : SV_Target {
    // Sample the texture using the UV coordinates
    fixed4 texColor = tex2D(_MainTex, i.uv);
    // Calculate the gradient color based on the distance from the vertex to the gradient point
    float4 gradientColor = lerp(_Color, float4(0, 0, 0, 1), i.distance / _GradientDistance);
    // Multiply the texture color with the gradient color to get the final color
    fixed4 finalColor = texColor * gradientColor;
    return finalColor;
}
            ENDCG
        }
    }
}
