Shader "Custom/Unlit/Displacement"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_DispMap ("Displacement map", 2D) = "black" {}
		_Extrude ("Extrude", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
		Cull Off
        LOD 100

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
				//fixed4 color : COLOR;
            };

            sampler2D 
				_MainTex,
				_DispMap;
			float _Extrude;		

            v2f vert (appdata v)
            {
                v2f o;
				v.vertex += v.normal * _Extrude * tex2Dlod(_DispMap, float4(v.uv, 0, 0)).r;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				//o.color = tex2Dlod(_DispMap, float4(v.uv, 0, 0)).rgba;
				
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
