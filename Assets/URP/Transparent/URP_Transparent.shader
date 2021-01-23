Shader "URP/URP_Transparent" {
    Properties 
    {
    }
    SubShader 
    {
        Tags 
        {
            "RenderPipeline" = "UniversalRenderPipeline"
            "LightMode"="UniversalForward"
            "RenderType" = "Transparent"  
        }
        Pass 
        {
            Tags 
            {
                
            }
            
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_FOG_COORDS(0)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                ////// Lighting:
                float3 finalColor = 0;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
