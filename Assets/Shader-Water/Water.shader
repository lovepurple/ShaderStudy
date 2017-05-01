Shader "Custom/Sea" 
{
	Properties 
	{
		_SeaTex ("Sea", 2D) = "" {}
		_FoamTex ("Foam", 2D) = "" {}
		_MinDepth("Min Depth", Range(-50.0, 50.0)) = 0.0
		_FoamDepth("Foam Depth", Range(0.0, 1.0)) = 0.0
	//	_MaxDepth("Max Depth", Range(-2.0, 0.0)) = 0.0
		_Tint("Water Tint", Color) = (0.1, 1.0, 1.0, 1.0)
	}
	
	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv0 : TEXCOORD0;
		float2 uv1 : TEXCOORD1;
		float4 projPos : TEXCOORD2;
	};
			
	uniform sampler2D _CameraDepthTexture; //the depth texture
	sampler2D _SeaTex;
	sampler2D _FoamTex;
	float4 _SeaTex_ST;
	float4 _FoamTex_ST;
	float4 _Tint;
	float _MinDepth;
	float _FoamDepth;
	//float _MaxDepth;
	
	v2f vert( appdata_img v ) 
	{ 
		v2f o;
		o.uv0 		= TRANSFORM_TEX (v.texcoord, _SeaTex);
		o.uv1 		= TRANSFORM_TEX (v.texcoord, _FoamTex);
		o.pos 		= mul(UNITY_MATRIX_MVP, v.vertex);
		o.projPos 	= ComputeScreenPos(o.pos); 
		return o;
	}

	float4 frag (v2f i) : COLOR 
	{
		float4 seaCol = tex2D(_SeaTex, i.uv0);
		float foamCol = tex2D(_FoamTex, i.uv1);
		
		float terrainDepth 	= Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
		float seaDepth 		= Linear01Depth(i.projPos.z);
		
		float depth = abs(terrainDepth - seaDepth);
		
		float multiplier = Linear01Depth(_MinDepth);
		//depth = pow(depth, 50.0f);
		//depth = depth / 3.0f;
		
		//depth = multiplier - depth;
		depth = smoothstep(0.0f, multiplier, depth);
		//depth = _MinDepth - depth;
		
		//seaCol.rgb = depth;
		seaCol.a = 1.0f;
		
		seaCol.a = depth;
		
		float foamAmount = (1.0f - depth) * (depth * _FoamDepth);
		seaCol *= _Tint;
		
		float4 outCol = lerp(seaCol, foamCol, foamAmount);
		outCol.a += foamAmount;
		
		return outCol;
	}	

	ENDCG 
	
Subshader {
	  Cull Off ZWrite Off
	  Fog { Mode off }  
	  Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
     // ColorMask RGB	  
  		  	
	 Pass {    
	
	      CGPROGRAM
	      #pragma fragmentoption ARB_precision_hint_fastest
	      #pragma vertex vert
	      #pragma fragment frag
	      ENDCG
	  }
  
}

Fallback off
	
} 