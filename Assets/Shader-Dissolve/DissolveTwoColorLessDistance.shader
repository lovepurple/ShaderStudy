/*
	根据摄像机距离，当摄像机到物品的距离小于一个值时，开始溶解
*/
Shader "Dissolve/DissolveTwoColorLessDistance"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DissolveTex("Dissolve Tex",2D) = "white"{}
		_DissolveEdgeInnerColor("Inner Edge Color",Color) = (1,0.5,0,1)
		_DissolveEdgeOuterColor("Outer Edge Color",Color) = (1,0,0)
		_DissolveEdgeWidth("Dissolve Edge Width",Range(0,0.1)) = 0.02
		_LessDistance("Less Distance Dissolve",float) = 10
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

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
					float4 worldPos:TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				sampler2D _DissolveTex;
				float4 _DissolveTex_ST;

				float4 _DissolveEdgeInnerColor;
				float4 _DissolveEdgeOuterColor;

				fixed _DissolveEdgeWidth;

				fixed _LessDistance;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.worldPos = mul(unity_ObjectToWorld,v.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					fixed dissolveChannel = tex2D(_DissolveTex, i.uv).r;
					
					fixed distanceToCamera = distance(_WorldSpaceCameraPos.xyz,i.worldPos.xyz);
					
					fixed s = 1-step(_LessDistance,distanceToCamera);
 
					fixed dissolvePercentage = 1- ((distanceToCamera /  _LessDistance) - floor( distanceToCamera /  _LessDistance));
					dissolvePercentage = s * dissolvePercentage;
					
					fixed clipScope = dissolveChannel -dissolvePercentage;

					clip(clipScope);

					fixed v = smoothstep(dissolvePercentage, dissolvePercentage + _DissolveEdgeWidth, dissolveChannel);
					fixed v2 = step(clipScope, _DissolveEdgeWidth);

					col = v2 * lerp(_DissolveEdgeInnerColor, _DissolveEdgeOuterColor, v) + (1 - v2)*col;

					return col;
			}
			ENDCG
		}
		}
}
