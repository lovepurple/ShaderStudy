Shader "Water\CoastFoam"
{
	Properties
	{
		_MainColor("Main Color".Color) = (0,0.5,1,1)
		_WaveNormal("Wave Normal",2D)="normal"{}
		_FoamTex("Foam Texture",2D)="foam"{}
		_WaveSpeedX("Wave Speed X",float) = 0.1
		_WaveSpeedY("Wave Speed Y",float) = 0.1

	}
	SubShader
	{
		Tags{ "LightMode" = "ForwardBase" }

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma vertex vert 
			#pragma fragment frag 

			uniform sampler2D _FoamDiffuse,_IslandMask,_WaveNormal;
			uniform float _WaveHeight,_WaterContrast,_WaveTile1,_WaveTile2,_WaterTile,_FoamTileX,_FoamTileY,_WaveWind,_WaterDepth,_WaveSpeed,_WaterSpeed,_SpecularGloss,_SpecularPower;
			uniform float4 _Normal,_IslandFoam, _CoastColor, _CoastDepth, _OceanColor, _OceanDepth;

			float4 costMaskF(float2 posUV) { return tex2Dlod(_IslandMask, float4(posUV,1.0,1.0)); }

			struct vertexOutput 
			{
				float4 pos : SV_POSITION;
				float3 _Color : TEXCOORD0;
				float3 _Depth : TEXCOORD1;
				float4 _Normal : TEXCOORD2;
				float4 _IslandFoam : TEXCOORD3;
			};

			struct v2f
			{
				float4 pos:SV_POSITION;
			};

			vertexOutput vert(appdata_base a)
			{
				vertexOutput o;
				float2 uv = a.texcoord.xy;
				float4 pos = a.vertex;
				// coast setup
				float2 posUV = uv;
				float4 coastMask = costMaskF(posUV); // mask for coast in blue channel
				float animTimeX = uv.y *_WaveTile1 + _Time.w * _WaveSpeed; // add time for shore X
				float animTimeY = uv.y *_WaveTile2 + _Time.w * _WaveSpeed; // add time for shore Y
				float waveXCos = cos(animTimeX) + 1;
				float waveYCos = cos(animTimeY);
				// coast waves
				pos.z += (waveXCos * _WaveWind * coastMask) * coastMask;
				pos.y += (waveYCos * _WaveHeight * _WaveWind * 0.25) * coastMask;
				o.pos = mul(UNITY_MATRIX_MVP, pos);
				// custom uv
				float2 foamUV = float2(a.vertex.x *_FoamTileX, a.vertex.z *_FoamTileY);
				float2 normalUV = float2(uv.x * 2.0, uv.y * 2.0) * 4.0;
				// reflections
				float3 lightPos = float3(-22.0, -180.0, -6.80);
				float3 lightDir = float3(15.0, 1.0, 10.0);
				float3 lightVec = normalize(lightPos - o.pos.xyz);
				float lightRef = (1.0 - (dot(lightDir, lightVec)));
				lightRef = lightRef * 0.25 + (lightRef * lightRef); // get rid of left side
				// edge and depth water
				float step = saturate(_WaterDepth);
				float depthX = (a.vertex.x * 0.22 - 1.0); // centering depth area
				float depthY = (a.vertex.z * 0.22 - 1.5); // centering depth area
				float depth = pow((depthX * depthX + depthY * depthY) * 0.006,3);
				float edge = saturate(step - (1.0 - depth) * 0.5);
				// Vertex Custom Output
				o._Color.rgb = lerp(_CoastColor.rgb, _OceanColor.rgb, edge);
				o._Depth.rgb = lerp(_CoastDepth.rgb, _OceanDepth.rgb, edge);
				o._IslandFoam.xy = posUV;
				o._IslandFoam.zw = foamUV + float2(1 - _Time.x, 1 - _Time.x)*0.5;
				o._Normal.xy = normalUV*_WaterTile + float2(0, _Time.x * _WaterSpeed);
				o._Normal.w = 1.0 - saturate(lightRef *(length((lightPos.x - (o.pos.z - 35.0))) * 0.002) * _SpecularGloss); // spec coeff
				o._Normal.z = (sin((a.vertex.x - _Time.w) - (a.vertex.z + _Time.x) * 5) + 1.0) * 0.5; // normal coeff
				return o;
			}

			float4 frag(vertexOutput i) : COLOR 
			{
				float4 normal = tex2D(_WaveNormal, i._Normal.xy);
				float3 foam = float3(tex2D(_FoamDiffuse, float2(i._IslandFoam.z, i._IslandFoam.w - _Time.x)).r, 1.0, 1.0);
				float3 mask = tex2D(_IslandMask, i._IslandFoam.xy).rgb*foam;
				mask.g += _WaterContrast; // contrast point
				float4 color = float4(lerp(i._Depth, i._Color, (normal.x * i._Normal.z) + (normal.y * (1.0 - i._Normal.z))), 0.5)
				+ exp2(log2((((normal.z) * i._Normal.z)*(1 - mask.b*0.75) // waves light
				+ (normal.w * (1.0 - i._Normal.z))*(1 - mask.b*0.75) // waves light
				)* i._Normal.w) * 3) * _SpecularPower; // narrowing 
				color = float4(lerp(color, float4(1, 1, 1, mask.x), mask.x) * mask.yyyy); // blend with foam
				color.w *= mask.g; // adjust an alpha to add more colors 
				return color;
			}
		ENDCG
		}
	}
	Fallback "Unlit/Texture"
}