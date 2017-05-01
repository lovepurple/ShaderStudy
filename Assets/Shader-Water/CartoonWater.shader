Shader "Water/CartoonWater"
{
	Properties
	{
		_MainColor("Main Color",Color) = (1,1,1,1)
		_OpaqueTex("Opaque Texture",2D) = "white"{}
		_NormalTex("Normal",2D) = "white"{}
		_FoamTex("Foam Texture",2D) = "foam"{}
	}
		SubShader
	{

		Pass
		{
			Tags
			{
				"RenderType" = "Transparent"
				"Queue" = "Transparent"
			}

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
		  cull back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			sampler2D _OpaqueTex;
			float4 _OpaqueTex_ST;

			sampler2D _NormalTex;
			float4 _NormalTex_ST;

			sampler2D _MainTex;

			float4 _MainColor;

			sampler2D _FoamTex;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv:TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent:TANGENT;
			};


			struct v2f {
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float2 uv:TEXCOORD2;
				float3 worldTangent:TEXCOORD3;
				float3 worldBinormal:TEXCOORD4;
			};


			v2f vert(appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.posWorld = mul(unity_ObjectToWorld,v.vertex);
				o.uv = v.uv;
				o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
				o.worldTangent = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
				o.worldBinormal = normalize(cross(o.worldNormal,o.worldTangent) * v.tangent.w);

				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				float3x3 worldTBNMatrix_T = transpose(float3x3((i.worldTangent),(i.worldBinormal),(i.worldNormal)));
				float3 normal = normalize(mul(worldTBNMatrix_T,UnpackNormal(tex2D(_NormalTex,i.uv + float2(0.1,0) * _Time.y))));
				float4 opaqueCol = tex2D(_OpaqueTex,i.uv);

				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);

				float3 diffuseCol = _LightColor0 * _MainColor * (max(0,dot(normal,lightDir))) * tex2D(_FoamTex,i.uv + float2(0.1,0) *_Time.y);

				float3 H = normalize(lightDir + viewDir);
				float3 spec = pow(max(0,dot(H,normal)),128) *_LightColor0;

				float4 col = float4(diffuseCol + spec,1);
				col.a = opaqueCol.r;

				return col;
			}
			ENDCG
		}
	}
}
