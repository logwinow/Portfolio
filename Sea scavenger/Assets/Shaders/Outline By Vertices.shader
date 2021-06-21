Shader "Custom/Sprite/Outline by Vertices"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Thickness ("Thickness", Float) = 0
		_Color ("Color", Color) = (1, 1, 1, 1)
		_Tex ("Tex", 2D) = "white" {}
    }
    SubShader
    {
        // Tags { "RenderType"="TransparentCutout" "Queue"="Transparent" "PreviewType"="Plane" "IgnoreProjector" = "True" }
		// ZTest Off
		// ZWrite Off
		// Lighting Off
		// Cull Off
		// Blend SrcAlpha OneMinusSrcAlpha
		
		Tags { "RenderType"="Opaque" }
		Cull Off

		// Pass
        // {
            // CGPROGRAM
            // #pragma vertex vert
            // #pragma fragment frag

            // #include "UnityCG.cginc"

            // struct appdata
            // {
                // float4 vertex : POSITION;
                // float2 uv : TEXCOORD0;
            // };

            // struct v2f
            // {
                // float2 uv : TEXCOORD0;
                // float4 vertex : SV_POSITION;
            // };

            // sampler2D _MainTex;

            // v2f vert (appdata v)
            // {
                // v2f o;
                // o.vertex = UnityObjectToClipPos(v.vertex);
				// o.uv = v.uv;
				
                // return o;
            // }

            // fixed4 frag (v2f i) : SV_Target
            // {
                // fixed4 col = tex2D(_MainTex, i.uv);
                // return fixed4(0, 0, 0, 1);
            // }
            // ENDCG
        // }
		
        Pass
        {
            CGPROGRAM
			#pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float4 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
            };

            sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _Thickness;
			fixed4 _Color;
			sampler2D _Tex;

            v2f vert (appdata v)
            {
				v2f o;
				
				o.color = tex2Dlod(_Tex, float4(v.uv, 0, 0));
				v.vertex += v.normal * tex2Dlod(_Tex, float4(v.uv + float2(_MainTex_TexelSize.x, 0), 0, 0)).a * _Thickness;
				v.vertex += v.normal * tex2Dlod(_Tex, float4(v.uv + float2(-_MainTex_TexelSize.x, 0), 0, 0)).a * _Thickness;
				v.vertex += v.normal * tex2Dlod(_Tex, float4(v.uv + float2(0, _MainTex_TexelSize.y), 0, 0)).a * _Thickness;
				v.vertex += v.normal * tex2Dlod(_Tex, float4(v.uv + float2(0, -_MainTex_TexelSize.y), 0, 0)).a * _Thickness;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            { 
				fixed4 col = tex2D(_MainTex, i.uv);
				//col.rgb = _Color;
				
                //return i.color + fixed4(0, 0, 0, 1);
				//return i.color;
				return col;
            }
            ENDCG
        }
    }
}
