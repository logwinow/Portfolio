Shader "Custom/Sprite/Outline By Offset"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Thickness ("Outline Thickness", Int) = 0
		[HDR]
		_Color ("Color", Color) = (1, 1, 1, 1)
		_Outline ("Outline", Int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="Transparent" "PreviewType"="Plane" "IgnoreProjector" = "True" }
		ZTest Off
		Cull Off
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _Thickness;
			int _Outline;
			fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 c;
				
				c = tex2D(_MainTex, i.uv);
				
				if (_Outline > 0 && _Thickness > 0)
				{
					fixed4 cols[8];
					
					cols[0] = tex2D(_MainTex, i.uv + float2(-_Thickness * _MainTex_TexelSize.x, 0));
					cols[1] = tex2D(_MainTex, i.uv + float2(-_Thickness * _MainTex_TexelSize.x, _Thickness * _MainTex_TexelSize.y));
					cols[2] = tex2D(_MainTex, i.uv + float2(0, _Thickness * _MainTex_TexelSize.y));
					cols[3] = tex2D(_MainTex, i.uv + float2(_Thickness * _MainTex_TexelSize.x, _Thickness * _MainTex_TexelSize.y));
					cols[4] = tex2D(_MainTex, i.uv + float2(_Thickness * _MainTex_TexelSize.x, 0));
					cols[5] = tex2D(_MainTex, i.uv + float2(_Thickness * _MainTex_TexelSize.x, -_Thickness * _MainTex_TexelSize.y));
					cols[6] = tex2D(_MainTex, i.uv + float2(0, -_Thickness * _MainTex_TexelSize.y));
					cols[7] = tex2D(_MainTex, i.uv + float2(-_Thickness * _MainTex_TexelSize.x, -_Thickness * _MainTex_TexelSize.y));
					
					c += (clamp((cols[0]).a + (cols[1]).a + cols[2].a + cols[3].a + cols[4].a + cols[5].a + cols[6].a + cols[7].a, 0, 1) - c.a) * _Color;
				}
					
				return c;
            }
            ENDCG
        }
    }
}
