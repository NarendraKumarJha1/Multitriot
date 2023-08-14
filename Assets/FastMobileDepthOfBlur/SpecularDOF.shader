Shader "SupGames/DOF/Specular" {
	Properties{
		_MainTex("Texture For Diffuse Material Color", 2D) = "white" {}
		_SpecColor("Specular Material Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Range(1,50)) = 0.03
	}
SubShader{
	Pass 
		{
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc" 
			uniform fixed4 _LightColor0;
			sampler2D _MainTex;
			uniform fixed4 _SpecColor;
			uniform fixed _Shininess;
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
				fixed3 uv : TEXCOORD0;
				fixed3 diff : TEXCOORD1;
				fixed3 spec : TEXCOORD2;
			};

			v2f vert(appdata i)
			{
				v2f o;
				fixed3 normalDirection = normalize(mul(float4(i.normal, 0.0h), unity_WorldToObject).xyz);
				fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				o.diff = fixed4(UNITY_LIGHTMODEL_AMBIENT.rgb,0.0h)+_LightColor0.rgb * max(0.0h, dot(normalDirection, lightDirection));;
				o.spec = _LightColor0.rgb * _SpecColor.rgb * pow(max(0.0h, dot( reflect(-lightDirection, normalDirection), normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, i.vertex).xyz))), _Shininess);;
				o.uv.xy = TRANSFORM_TEX(i.uv,_MainTex);
				o.uv.z = abs( (1 - clamp(-UnityObjectToViewPos(i.vertex).z / _Focus,0,2)) * _Aperture);
				o.pos = UnityObjectToClipPos(i.vertex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.uv.xy);
				return fixed4(i.spec * (1.0h - col.a) + i.diff * col.rgb, i.uv.z);
			}
			ENDCG
		}
	}
	Fallback "Specular"
}