Shader "Custom/URP_InvertedHullOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0.0, 0.1)) = 0.01
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "RenderPipeline"="UniversalPipeline" 
            "Queue"="Geometry" 
        }
        LOD 100

        Pass
        {
            Name "Outline"
            // Crucial: Cull Front is the core of the Inverted Hull technique
            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // Enable GPU Instancing
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // Using UnityPerMaterial allows compatibility with the SRP Batcher
            // while also supporting GPU Instancing if configured correctly.
            UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
                UNITY_DEFINE_INSTANCED_PROP(float4, _OutlineColor)
                UNITY_DEFINE_INSTANCED_PROP(float, _OutlineWidth)
            UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                float width = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _OutlineWidth);
                
                // Extrude the vertex position along its normal
                float3 posOS = input.positionOS.xyz + (input.normalOS * width);
                
                output.positionCS = TransformObjectToHClip(posOS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                
                half4 color = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _OutlineColor);
                return color;
            }
            ENDHLSL
        }
    }
}