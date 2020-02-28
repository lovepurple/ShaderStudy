Shader "March/Diffuse"
{
    Properties
    {
        _MainTex("Main Texture",2D) = "white"{}
        _TintColor("Tint Color" , Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Geometry"
            "RenderType"="Opaque"
            "IgnoreProjector"="True"
        }

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _TintColor;
            sampler2D _MainTex;

            float4 _MainTex_ST;

            sampler2D _TempGrabTex;
			sampler2D _CameraDepthTexture;


            struct appdata
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos: SV_POSITION;
                float4 uv : TEXCOORD0;
				float4 screenPos:TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

				o.screenPos = ComputeScreenPos(o.pos);
				COMPUTE_EYEDEPTH(o.screenPos.z);

                return o;

            }


            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex , i.uv.xy);
                fixed mask = tex2D(_TempGrabTex,i.screenPos.xy).r;
				fixed3 albedo = tex.rgb * _TintColor.rgb;

				float sceneDepth = LinearEyeDepth( SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
				if (i.screenPos.z < sceneDepth)
				{ 
					
					
				
				}
				else
					discard;
				return fixed4(albedo, 1);
				//return fixed4(mask, mask, mask, 1);
                /*fixed3 albedo = tex.rgb * _TintColor.rgb * mask;

				

                return fixed4(albedo , 1);*/
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse" 
}
