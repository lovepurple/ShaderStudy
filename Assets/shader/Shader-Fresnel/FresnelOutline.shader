Shader "Unlit/FresnelOutline"
{
	Properties
	{
		_MainColor("Main Color",Color) = (1,1,1,1)
		_RimColor("Rim Color",Color) = (1,0,0,1)
		_RimWidth("Rim Width",float) = 0
		_RimPower("Rim Power",float) = 5
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

			float4 _MainColor;
			float4 _RimColor;
			float _RimWidth;
			float _RimPower;

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv:TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
				float3 viewDir:TEXCOORD2;
				float2 uv:TEXCOORD3;
			};

		VertexOutput vert(VertexInput v)
		{
			VertexOutput o = (VertexOutput)0;
			o.normalDir = UnityObjectToWorldNormal(v.normal);
			o.posWorld = mul(unity_ObjectToWorld, v.vertex);
			o.pos = UnityObjectToClipPos(v.vertex);
			o.viewDir = UnityWorldSpaceViewDir(o.posWorld);
			return o;
		}


		float4 frag(VertexOutput i) : COLOR
		{
			i.normalDir = normalize(i.normalDir);
			i.viewDir = normalize(i.viewDir);

			float NDotV = saturate(dot(i.normalDir, i.viewDir));

			/*float v = saturate((1 - NDotV) + _RimWidth);

			float4 col = lerp(_MainColor,_RimColor , v);
			col = pow(col, _RimPower);*/
			float v = smoothstep(0, _RimWidth, saturate((1 - NDotV)));

			float4 col = lerp(_MainColor, _RimColor, v);

			return col;
		}
			ENDCG
		}
	}
}
