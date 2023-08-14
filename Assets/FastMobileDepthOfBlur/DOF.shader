Shader "SupGames/DepthOfField"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth


		Pass
		{
			CGPROGRAM
			#pragma vertex vertb2
			#pragma fragment fragBlur2
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"

			struct v2fb2
			{
				half4 pos  : SV_POSITION;
				half2  uv  : TEXCOORD0;
				half4  uv1 : TEXCOORD1;
				half4  uv2 : TEXCOORD2;
			};

			sampler2D _MainTex;
			half4 _MainTex_TexelSize;
			uniform half _BlurAmount;
			v2fb2 vertb2(appdata_img v)
			{
				v2fb2 o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				o.uv1.xy = v.texcoord.xy + _MainTex_TexelSize.xy * _BlurAmount;
				o.uv1.zw = v.texcoord.xy + half2(-1.0h, 1.0h) * _MainTex_TexelSize.xy * _BlurAmount;
				o.uv2.xy = v.texcoord.xy - _MainTex_TexelSize.xy * _BlurAmount;
				o.uv2.zw = v.texcoord.xy + half2(1.0h, -1.0h) * _MainTex_TexelSize.xy * _BlurAmount;
				return o;
			}

			fixed4 fragBlur2(v2fb2 i) : COLOR
			{
				fixed4 result = tex2D(_MainTex, i.uv)*0.4h;
				result += tex2D(_MainTex, i.uv1.xy)*0.15h;
				result += tex2D(_MainTex, i.uv1.zw)*0.15h;
				result += tex2D(_MainTex, i.uv2.xy)*0.15h;
				result += tex2D(_MainTex, i.uv2.zw)*0.15h;
				return result;
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"

			struct v2f
			{
				half4 pos  : SV_POSITION;
				half2  uv  : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _BlurTex;
			v2f vert(appdata_img v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 result = tex2D(_MainTex, i.uv);
				fixed4 blur = tex2D(_BlurTex, i.uv);
				return lerp(result,blur,result.a);
			}

			ENDCG
		}

	}



}
