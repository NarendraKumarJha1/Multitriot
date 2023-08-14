Shader "SupGames/DOF/Diffuse" {
	Properties{
	   _MainTex("Texture", 2D) = "white" {}
	}
	SubShader{
	   	Pass {
		  Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag 
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			uniform fixed4 _LightColor0;
			sampler2D _MainTex;
			sampler2D _BlurTex;
			half4 _MainTex_ST;
			uniform half _Focus;
			uniform half _Aperture;

			 struct appdata {
				fixed4 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed2 uv : TEXCOORD0;
			 };

			 struct v2f {
				fixed4 pos : SV_POSITION;
				fixed2 uv : TEXTCOORD0;
				fixed4 ref : TEXTCOORD1;
			 };

			 v2f vert(appdata i)
			 {
				v2f o;
				o.ref = fixed4(UNITY_LIGHTMODEL_AMBIENT.rgb,0.0h)+fixed4(_LightColor0.rgb * max(0.0h, dot(normalize(mul(fixed4(i.normal, 0.0h), unity_WorldToObject).xyz), normalize(_WorldSpaceLightPos0.xyz))), abs( (1 - clamp(-UnityObjectToViewPos(i.vertex).z / _Focus,0,2)) * _Aperture));
				o.pos = UnityObjectToClipPos(i.vertex);
				o.uv.xy = TRANSFORM_TEX(i.uv,_MainTex);
				return o;
			 }

			 fixed4 frag(v2f i) : COLOR
			 {
				return tex2D(_MainTex, i.uv)*i.ref;
			 }

			 ENDCG
			}


	}
		Fallback "Diffuse"
}