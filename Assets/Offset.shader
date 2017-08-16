// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader 'Custom/Cartoon_Offset'
{
	Properties{ _MainTex('Texture', 2D) = 'white' { }
	}
		SubShader{ //描边 
		pass
	{
		Cull front
		offset - 5,-1
			CGPROGRAM
#pragma vertex vert 
#pragma fragment frag 
#include 'UnityCG.cginc' 
			sampler2D _MainTex;
		float4 _MainTex_ST;
		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};
		v2f vert(appdata_base v)
		{
			v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
		return o;
		}
		float4 frag(v2f i) : COLOR { return float4(0,0,0,0); }
			ENDCG
	} //绘制物体 
	pass {
		offset 2,-1
			CGPROGRAM
#pragma vertex vert 
#pragma fragment frag 
#include 'UnityCG.cginc' 
			sampler2D _MainTex;
		float4 _MainTex_ST;
		struct v2f
		{
			float4 pos : SV_POSITION; float2 uv : TEXCOORD0;
		};
		v2f vert(appdata_base v)
		{

			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
			return o;
		}
		float4 frag(v2f i) : COLOR
{
			float4 texCol = tex2D(_MainTex,i.uv);
		float4 outp = texCol;
		return outp;
		}
			ENDCG
	}
	}
}