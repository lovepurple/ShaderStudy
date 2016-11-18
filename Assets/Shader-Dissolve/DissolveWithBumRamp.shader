Shader "Unlit/DissolveWithBumRamp"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SliceGuide("Slice Guide",2D) = "white"{}
		_SliceAmount("Slice Amount",Range(0,1)) = 0.5

		//Dissolve边缘发光的图
		_BumRamp("Bum Ramp",2D) = "white"{}
		_BumSize("Bum Size",Range(0,1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumRamp;
			float4 _BumRamp_ST;
			sampler2D _SliceGuide;
			float4 _SliceGuide_ST;
			fixed _SliceAmount;
			fixed _BumSize;

			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
			}
			
			float4 frag (v2f i) : COLOR
			{
				//1.溶解
				//如果按r通道溶解，r值越小的地方最先被clip掉
				float3 sliceColor = tex2D(_SliceGuide,i.uv).rgb - _SliceAmount;
				 clip(sliceColor.r);

				float4 mainColor = tex2D(_MainTex,i.uv);

				//2.溶解范围的颜色				
				if(sliceColor.r < _BumSize && _SliceAmount>0 && _SliceAmount<1)
				{
					float2 uvRamp = (0,0);


					//当前溶解的贴图是y不变，在x上有渐变
					/*
						1. 溶解图的r值
						2. r值越小，最先被溶解(越黑越小)
						3. 随着r变大，最大是1 上面减去一个系数作用是让溶解的范围变大()
						4. sliceColor.r * 1/_BumSize 作用：
							r越小越驱近于0，r越大，驱近于1，由于上面又 减去一个Amount 更多的r值接近于0,所以ramp texture的最左面看上去特别多
							bumSize是控制范围的：
								bumSize越小， 1 / bumSize越大，层次感的间距离越小
									   越大， 1 / bumSize越小，计算出的坐标跟原始的r值越接近
								倒数的作用！！ 

						r * 1 /BumSize 的作用是把取色的范围又扩大了
							例如，bumSize = 0.5 r= 0.5
							(1) 如果直接使用 r 则 uvRamp.x= 0.5
							(2) 如果使用r * (1 / bumSize) uvRamp.x = 1 取到了贴图的最左边

					*/	
					uvRamp.x = sliceColor.r * 1/_BumSize ;


					float4 col = tex2D(_BumRamp,uvRamp);
					mainColor *= col;
				}


				return mainColor;

			}
			ENDCG
		}
	}
}
