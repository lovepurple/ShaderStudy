// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "JQM/DepthTest_1"
{
	Properties
	{ _Color0("Water Color",Color) = (1,1,1,1)//ˮ����ɫ
		_Color1("Water Depth",Color) = (0,0,0,0)//ˮ����ȵ���ɫ
		_Alpha("Alpha",Range(0,1)) = 1//ˮ�������͸����
		_ColorDepth("ColorDepth",Range(0,1)) = 0//ˮ�����
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" }

		Pass
	{
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct VertexOutput
	{
		float2 uv : TEXCOORD0;
		float4 pos : SV_POSITION;
		float4 scrPos : TEXCOORD1;
	};


	float4 _Color0;
	float4 _Color1;
	float _Alpha;//ˮ��͸����
	float _ColorDepth;

	sampler2D _CameraDepthTexture;

	VertexOutput vert(appdata v)
	{
		VertexOutput o;

		o.pos = UnityObjectToClipPos(v.vertex);
		o.scrPos = ComputeScreenPos(o.pos);//������Ƭ����ɫ������Ļλ��
		COMPUTE_EYEDEPTH(o.scrPos.z);//���㶥��������ռ����ȣ�����ü�ƽ��ľ���
		return o;
	}

	fixed4 frag(VertexOutput i) : COLOR
	{
		//���㵱ǰ�������
		float  depth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r;//UNITY_PROJ_COORD:���ֵ [0,1]
	depth = LinearEyeDepth(depth);//��ȸ�������Ĳü���Χ��ֵ[0.3,1000],�ǽ�����͸��ͶӰ�任�����ֵ��ԭ��
	depth -= i.scrPos.z;
	//����ˮ��͸���ȣ� ʹ�����ֵ
	float alpha = saturate(_Alpha*depth);

	//������ɫ��ȣ�
	float colDepth = saturate(_ColorDepth*depth);
	colDepth = 1 - colDepth;
	colDepth = lerp(colDepth, colDepth*colDepth*colDepth, 0.5);//������ȣ�������ϲ��

	half3 col;
	col.rgb = lerp(_Color0.rgb, _Color1.rgb, colDepth);

	col.rgb = depth;

	return float4(col.rgb, 1);
	}
		ENDCG
	}
	}
}