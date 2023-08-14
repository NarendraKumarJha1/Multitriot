Shader "SupGames/DOF/Unlit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100


		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			sampler2D _BlurTex;
			half4 _MainTex_ST;
			uniform half _Focus;
			uniform half _Aperture;
			struct appdata
			{
				half4 vertex : POSITION;
				half2 uv: TEXCOORD0;
			};
			struct v2f
			{
				half4 vertex : SV_POSITION;
				half3 uv: TEXCOORD0;
			};
			v2f vert (appdata i)
			{
				v2f o;
				o.uv.xy=TRANSFORM_TEX(i.uv,_MainTex);
				o.vertex = UnityObjectToClipPos(i.vertex);
				o.uv.z = abs( (1 - clamp(-UnityObjectToViewPos(i.vertex).z / _Focus,0,2)) * _Aperture);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(tex2D(_MainTex, i.uv.xy).rgb,i.uv.z);
			}
			ENDCG
		}
	}
}
