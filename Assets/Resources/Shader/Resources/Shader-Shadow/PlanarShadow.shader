// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/PlanarShadow"
{
	Properties
	{
		_ShadowColor("Shadow Color",Color) = (0.5,0.5,0.5,0.3)
		_PlanarParam("Planar Params",Vector) = (0,1,0,0)
	}
		SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off



			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			float4 _ShadowColor;
			float4 _PlanarParam;


			v2f vert(appdata v)
			{
				v2f o;

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos.xyz));
				float3 worldLightPos = _WorldSpaceLightPos0.xyz;
				worldLightPos = worldLightDir;

				float3 planeNormal = normalize(_PlanarParam.xyz);
				float planeDistance = _PlanarParam.w;

				float NDotL = dot(planeNormal, worldLightPos);


				 float3 posOnPlane = worldLightPos - ((planeDistance + NDotL) / dot(planeNormal, worldLightDir)) *  worldLightDir;

				 float4 posOnPlanePos = mul(unity_WorldToObject, float4(posOnPlane,1.0));

				 float4 row0 = float4((NDotL + planeDistance - worldLightPos.x * planeNormal.x), -worldLightPos.x * planeNormal.y, -worldLightPos.x * planeNormal.z, -worldLightPos.x * planeDistance);
				 float4 row1 = float4(-worldLightPos.y * planeNormal.x, (NDotL + planeDistance - worldLightPos.y * planeNormal.y), -worldLightPos.y * planeNormal.z, -worldLightPos.y * planeDistance);
				 float4 row2 = float4(-worldLightPos.z * planeNormal.x, -worldLightPos.z * planeNormal.y, (NDotL + planeDistance - worldLightPos.z * planeNormal.z), -worldLightPos.z * planeDistance);
				 float4 row3 = float4(0,0,0, NDotL);

				 float4x4 projectionMatrix = float4x4(row0, row1, row2, row3);
				 float4 worldPosOnPlanar = mul(projectionMatrix, worldPos);
				 float4 posOnPlanar = mul(unity_WorldToObject, worldPosOnPlanar);

				 /*
					B*Ly + C*Lz	 	 	 	 	 	-B*Lx	 	 	-C*Lx	 	 	 	-D*Lx
					-A*Ly	 	 	 	 	 	A*Lx + C*Lz	 	 	-C*Ly	 	 	 	-D*Ly
					-A*Lz	 	 	 	 	 	-B*Lz	 	 	A*LX + B*Ly	 	 	 	-D*Lx
						0	 	 	 	 	 	0	 	 	0	 	 	 	A*Lx + B*Ly + C*Lz
				 */

				 row0 = float4((planeNormal.y * worldLightDir.y + planeNormal.z *worldLightDir.z), -planeNormal.y * worldLightDir.x, -planeNormal.z * worldLightDir.x, -planeDistance*worldLightDir.x);
				 row1 = float4(-planeNormal.x * worldLightDir.y, (planeNormal.x * worldLightDir.x + planeNormal.z *worldLightDir.z), -planeNormal.z * worldLightDir.y, -planeDistance*worldLightDir.y);
				 row2 = float4(-planeNormal.x * worldLightDir.z, -planeNormal.y * worldLightDir.z, (planeNormal.x * worldLightDir.x + planeNormal.y *worldLightDir.y),  -planeDistance*worldLightDir.z);
				 row3 = float4(0, 0, 0, NDotL);

				 projectionMatrix = float4x4(row0, row1, row2, row3);
				  worldPosOnPlanar = mul(projectionMatrix, worldPos);
				  posOnPlanar = mul(unity_WorldToObject, worldPosOnPlanar);


				 o.vertex = UnityObjectToClipPos(posOnPlanar);

				 return o;
			 }

			 fixed4 frag(v2f i) : SV_Target
			 {
				 return _ShadowColor;
			 }
		 ENDCG
	 }
	}
}
