Shader "Hidden/Custom/WaterRipple"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
		TEXTURE2D_SAMPLER2D(_Noise, sampler_Noise);
		float2 _Speed;
		float _Intensity;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
			float2 offset = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, i.texcoord + float2(_Time.x * _Speed.x, _Time.x * _Speed.y)).rg * _Intensity; 
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + offset);
			
            return color;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Off

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}

