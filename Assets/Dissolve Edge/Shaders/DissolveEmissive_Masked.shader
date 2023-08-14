Shader "Custom/DissolveEmissive_Masked" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ColorMask ("Color mask", 2D) = "white" {}

        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

[MaterialToggle] _HasEmissionMap("Has Emission Map", Float) = 0
        _EmissionMap ("Emission Map", 2D) = "black" {}
        [HDR] _EmissionColor ("Emission Color", Color) = (0,0,0)

        _SliceGuide ("Slice Guide", 2D) = "white" {}
        _SliceAmount ("Slice Amount", Range(0, 1)) = 0.0
        _BurnRamp ("Burn Ramp", 2D) = "white" {}
        _BurnSize ("Burn Size", Range(0, 1)) = 0.15
        _EmissionAmount ("Emission Amount", float) = 2.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard addshadow
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _ColorMask;
        sampler2D _EmissionMap;
        sampler2D _SliceGuide;
        sampler2D _BurnRamp;

        struct Input {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _EmissionColor;

        half _SliceAmount;
        half _HasEmissionMap;
        half _BurnSize;
        half _EmissionAmount;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            fixed4 colmask = tex2D ( _ColorMask, IN.uv_MainTex);
		    fixed4 carpaint = (c * _Color * (1-colmask)) + (c * colmask);

            half test = tex2D(_SliceGuide, IN.uv_MainTex).rgb - _SliceAmount;
            clip(test);

            // I skipped the _BurnColor here 'cause I was getting enough 
            // colour from the BurnRamp texture already.
            if (test < _BurnSize && _SliceAmount > 0) {
                o.Emission = tex2D(_BurnRamp, float2(test * (1 / _BurnSize), 0)) * _EmissionAmount;
            }

            o.Albedo = carpaint.rgb;

            o.Metallic = _Metallic;

            if(_HasEmissionMap >= 1)
            o.Emission += tex2D(_EmissionMap, IN.uv_MainTex).rgb*_EmissionColor.rgb;
            else
            o.Emission += _EmissionColor.rgb;

            // My Albedo map has smoothness in its Alpha channel.
            o.Smoothness = _Glossiness * c.a;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
