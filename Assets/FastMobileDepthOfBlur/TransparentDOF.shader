Shader "Cg texturing with alpha discard" {
   Properties {
      _MainTex ("RGBA Texture Image", 2D) = "white" {} 
      _Cutoff ("Alpha Cutoff", Float) = 0.5
   }
   SubShader {
	  Tags { "RenderType"="Opaque" }
	  LOD 100
      Pass {    
         Cull Off
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
         #pragma fragmentoption ARB_precision_hint_fastest
 		 #include "UnityCG.cginc"
         fixed _Cutoff;
		 sampler2D _MainTex;
		 sampler2D _BlurTex;
		 fixed4 _MainTex_ST;
		 uniform half _Focus;
		 uniform half _Aperture;
         struct appdata {
            fixed4 vertex : POSITION;
            fixed2 texcoord : TEXCOORD0;
         };
         struct v2f {
            fixed4 pos : SV_POSITION;
            fixed3 uv : TEXCOORD0;
         };
 
         v2f vert(appdata i) 
         {
            v2f o;
            o.uv.xy = i.texcoord.xy;
            o.pos = UnityObjectToClipPos(i.vertex);
            o.uv.z=abs( (1 - clamp(-UnityObjectToViewPos(i.vertex).z / _Focus,0,2)) * _Aperture);
            return o;
         }

         fixed4 frag(v2f i) : COLOR
         {
            fixed4 col = tex2D(_MainTex, i.uv.xy);  
            if (col.a < _Cutoff)
            {
               discard; 
            }
            col.a=i.uv.z;
            return col;
         }
 
         ENDCG
      }
   }
   Fallback "Unlit/Transparent Cutout"
}