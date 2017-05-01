Shader "Unlit/ToonDiscrete"
{
	Properties
	{
		_Steps("Steps",float) = 1
		_MainColor("Main Color",Color) = (0.1,0.8,0.7,1.0)
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque"  "Queue" = "Geometry"  }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			#include "UnityCG.cginc"


			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 lightDir:TEXCOORD0;
				float3 normal:TEXCOORD1;
			};

			fixed _Steps;
			fixed4 _MainColor;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.normal = normalize( worldNormal);

				float4 vertexWorldPos = mul(unity_ObjectToWorld, v.vertex);
				float3 lightDir = UnityWorldSpaceLightDir(vertexWorldPos.xyz);

				o.lightDir = normalize(lightDir);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//
				fixed NDotL = max(0,dot(normalize(i.normal),normalize(i.lightDir)));
				
				//直接使用vertex里的输出的 有棱角，使用上面的更平滑，原因是fragment里再normalize后，做了像素级的差值
				//fixed NDotL = max(0,dot(i.normal,i.lightDir));
				//Half Lambert
			fixed diff = NDotL * 0.5 + 0.5;

				fixed4 finalColor = floor(_MainColor * diff * _Steps) / _Steps;
				return finalColor;
			}

		ENDCG
	}
	}
}
