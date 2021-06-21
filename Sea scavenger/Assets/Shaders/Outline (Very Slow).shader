// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// 

Shader "Custom/Sprite/Outline (Very Slow)"
{
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}

		_Outline("Outline", Int) = 1
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
		_OutlineSize("Outline Size", int) = 1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "TransparentCutout"
			"PreviewType" = "Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _OutlineColor;
			int _OutlineSize;

			v2f vert(appdata IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;

				return OUT;
			}

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			int _Outline;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord);

				if (_Outline > 0 && c.a == 0) {
					[unroll(500)]
					for (int i = 1; i < _OutlineSize + 1; i++) {
						fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, i * _MainTex_TexelSize.y));
						fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0,i *  _MainTex_TexelSize.y));
						fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(i * _MainTex_TexelSize.x, 0));
						fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(i * _MainTex_TexelSize.x, 0));
						
						if (pixelUp.a || pixelDown.a || pixelRight.a || pixelLeft.a) {
							c = _OutlineColor;
							break;
						}
					}
				}

				return c;
			}
			ENDCG
		}
	}
}