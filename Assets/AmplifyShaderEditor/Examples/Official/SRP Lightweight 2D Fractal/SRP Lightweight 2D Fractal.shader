// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/SRP Lightweight/2D Fractal"
{
	Properties
	{
		_MaxIter("MaxIter", Int) = 0
		_Threshold("Threshold", Float) = 0
		_ZoomOffset("ZoomOffset", Float) = 2
		_ZoomScale("ZoomScale", Float) = 3
		_Center("Center", Vector) = (-0.412,0.609,0,0)
		_ZoomBase("ZoomBase", Float) = 0.25
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="LightweightPipeline" }
		Cull Back
		HLSLINCLUDE
		#pragma target 3.0
		ENDHLSL
		Pass
		{
			Tags { "LightMode"="LightweightForward" }
			Name "Base"
			
			Blend One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

		    HLSLPROGRAM
		    // Required to compile gles 2.0 with standard srp library
		    #pragma prefer_hlslcc gles
		    #pragma exclude_renderers d3d11_9x
		
		    #pragma vertex vert
		    #pragma fragment frag
		
			

		    // Lighting include is needed because of GI
		    #include "LWRP/ShaderLibrary/Core.hlsl"
		    #include "LWRP/ShaderLibrary/Lighting.hlsl"
		    #include "CoreRP/ShaderLibrary/Color.hlsl"
		    #include "LWRP/ShaderLibrary/InputSurfaceUnlit.hlsl"
		    #include "ShaderGraphLibrary/Functions.hlsl"
			
			uniform float _ZoomBase;
			uniform float _ZoomScale;
			uniform float _ZoomOffset;
			uniform float2 _Center;
			uniform int _MaxIter;
			uniform float _Threshold;
			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float Mandlebrot3( float2 UV , int MaxIter , float Threshold )
			{
				float2 r = UV;
				int step = 0;
				for (int i = 0; i < MaxIter; i++) 
				{
					if (length(r) > Threshold) 
						break;
					
					r = mul( float2x2(r.x,-r.y,r.y,r.x) , r) + UV;
					step++;
				}
				return (float)step/(float)MaxIter;
			}
			
					
			struct GraphVertexInput
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
	
		    struct GraphVertexOutput
		    {
		        float4 position : POSITION;
				float4 ase_texcoord : TEXCOORD0;
		        UNITY_VERTEX_INPUT_INSTANCE_ID
		    };
		
		    GraphVertexOutput vert (GraphVertexInput v )
			{
		        GraphVertexOutput o = (GraphVertexOutput)0;
		        UNITY_SETUP_INSTANCE_ID(v);
		        UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				v.vertex.xyz +=  float3( 0, 0, 0 ) ;
				v.normal =  v.normal ;
		        o.position = TransformObjectToHClip(v.vertex.xyz);
		        return o;
			}
		
		    half4 frag (GraphVertexOutput IN) : SV_Target
		    {
		        UNITY_SETUP_INSTANCE_ID(IN);
				float2 uv4 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( -1,-1 );
				float2 UV3 = ( ( pow( _ZoomBase , (_SinTime.z*_ZoomScale + _ZoomOffset) ) * uv4 ) + _Center );
				int MaxIter3 = _MaxIter;
				float Threshold3 = _Threshold;
				float localMandlebrot3 = Mandlebrot3( UV3 , MaxIter3 , Threshold3 );
				float temp_output_38_0 = ( 1.0 - localMandlebrot3 );
				float3 appendResult18 = (float3(( 0.9 - ( 0.9 * temp_output_38_0 ) ) , temp_output_38_0 , temp_output_38_0));
				float3 break24 = appendResult18;
				float3 hsvTorgb23 = HSVToRGB( float3(break24.x,break24.y,break24.z) );
				
		        float3 Color = hsvTorgb23;
		        float Alpha = localMandlebrot3;
		        float AlphaClipThreshold = 0;
		#if _AlphaClip
		        clip(Alpha - AlphaClipThreshold);
		#endif
		    	return half4(Color, Alpha);
		    }
		    ENDHLSL
		}
		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			
			HLSLPROGRAM
			#pragma prefer_hlslcc gles
    
			#pragma multi_compile_instancing

			#pragma vertex vert
			#pragma fragment frag

			#include "LWRP/ShaderLibrary/Core.hlsl"
			
			uniform float _ZoomBase;
			uniform float _ZoomScale;
			uniform float _ZoomOffset;
			uniform float2 _Center;
			uniform int _MaxIter;
			uniform float _Threshold;
			float Mandlebrot3( float2 UV , int MaxIter , float Threshold )
			{
				float2 r = UV;
				int step = 0;
				for (int i = 0; i < MaxIter; i++) 
				{
					if (length(r) > Threshold) 
						break;
					
					r = mul( float2x2(r.x,-r.y,r.y,r.x) , r) + UV;
					step++;
				}
				return (float)step/(float)MaxIter;
			}
			

			struct GraphVertexInput
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct GraphVertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			GraphVertexOutput vert (GraphVertexInput v)
			{
				GraphVertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;

				v.vertex.xyz +=  float3(0,0,0) ;
				v.normal =  v.normal ;
				o.clipPos = TransformObjectToHClip(v.vertex.xyz);
				return o;
			}

			half4 frag (GraphVertexOutput IN ) : SV_Target
		    {
		    	UNITY_SETUP_INSTANCE_ID(IN);

				float2 uv4 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( -1,-1 );
				float2 UV3 = ( ( pow( _ZoomBase , (_SinTime.z*_ZoomScale + _ZoomOffset) ) * uv4 ) + _Center );
				int MaxIter3 = _MaxIter;
				float Threshold3 = _Threshold;
				float localMandlebrot3 = Mandlebrot3( UV3 , MaxIter3 , Threshold3 );
				

				float Alpha = localMandlebrot3;
				float AlphaClipThreshold = AlphaClipThreshold;
				
				#if _AlphaClip
					clip(Alpha - AlphaClipThreshold);
				#endif
				return Alpha;
				return 0;
		    }
			ENDHLSL
		}
	}	
	FallBack "Hidden/InternalErrorShader"
}
/*ASEBEGIN
Version=15308
321;92;1100;654;362.0984;642.1493;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;39;-1200.375,-974.9979;Float;False;666.0259;385.254;Animating Zoom;6;16;25;31;26;7;9;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1150.375,-758.6487;Float;False;Property;_ZoomScale;ZoomScale;3;0;Create;True;0;0;False;0;3;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;41;-887.9313,-469.8014;Float;False;945.6219;589.0108;Calculating Fractal;10;43;42;1;3;5;12;6;13;8;4;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SinTimeNode;7;-1141.701,-924.9979;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;16;-1148.969,-681.7601;Float;False;Property;_ZoomOffset;ZoomOffset;2;0;Create;True;0;0;False;0;2;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;42;-850.629,-391.4146;Float;False;Constant;_Tiling;Tiling;6;0;Create;True;0;0;False;0;2,2;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;43;-848.379,-251.4149;Float;False;Constant;_Offset;Offset;6;0;Create;True;0;0;False;0;-1,-1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ScaleAndOffsetNode;31;-904.4308,-792.9454;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-886.5097,-900.7584;Float;False;Property;_ZoomBase;ZoomBase;5;0;Create;True;0;0;False;0;0.25;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;25;-687.3093,-874.158;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-659.6089,-370.2908;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,2;False;1;FLOAT2;-1,-1;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-430.1088,-417.2908;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;13;-606.1089,-241.2908;Float;False;Property;_Center;Center;4;0;Create;True;0;0;False;0;-0.412,0.609;-0.766,-0.1009;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.IntNode;5;-424.6088,-84.29077;Float;False;Property;_MaxIter;MaxIter;0;0;Create;True;0;0;False;0;0;250;0;1;INT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-268.6803,-350.8014;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-382.1088,14.70922;Float;False;Property;_Threshold;Threshold;1;0;Create;True;0;0;False;0;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;40;102.3927,-356.119;Float;False;1269.819;382;Coloring the Fractal;8;38;22;24;23;20;18;19;21;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CustomExpressionNode;3;-175.3088,-193.0908;Float;False;float2 r = UV@$int step = 0@$for (int i = 0@ i < MaxIter@ i++) ${$	if (length(r) > Threshold) $		break@$	$	r = mul( float2x2(r.x,-r.y,r.y,r.x) , r) + UV@$	step++@$}$return (float)step/(float)MaxIter@;1;False;3;True;UV;FLOAT2;0,0;In;True;MaxIter;INT;0;In;True;Threshold;FLOAT;0;In;Mandlebrot;True;False;3;0;FLOAT2;0,0;False;1;INT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;38;152.3927,-92.86803;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;161.1993,-257.6866;Float;False;Constant;_HueScale;HueScale;2;0;Create;True;0;0;False;0;0.9;2.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;329.2115,-306.119;Float;False;Constant;_HueOffset;HueOffset;3;0;Create;True;0;0;False;0;0.9;0.95;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;329.2115,-210.1189;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;20;535.5114,-259.0188;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;18;697.2116,-130.1189;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;24;857.2116,-146.1189;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.HSVToRGBNode;23;1129.211,-162.1189;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;0,0;Float;False;False;2;Float;ASEMaterialInspector;0;3;ASETemplateShaders/LightWeightSRPUnlit;e2514bdcf5e5399499a9eb24d175b9db;0;1;DepthOnly;0;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderType=Opaque;Queue=Geometry;RenderPipeline=LightweightPipeline;True;2;0;0;0;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;0;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1494.506,-322.1884;Float;False;True;2;Float;ASEMaterialInspector;0;3;ASESampleShaders/SRP Lightweight/2D Fractal;e2514bdcf5e5399499a9eb24d175b9db;0;0;Base;5;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderType=Opaque;Queue=Geometry;RenderPipeline=LightweightPipeline;True;2;0;0;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=LightweightForward;False;0;0;0;5;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;0
WireConnection;31;0;7;3
WireConnection;31;1;9;0
WireConnection;31;2;16;0
WireConnection;25;0;26;0
WireConnection;25;1;31;0
WireConnection;4;0;42;0
WireConnection;4;1;43;0
WireConnection;8;0;25;0
WireConnection;8;1;4;0
WireConnection;12;0;8;0
WireConnection;12;1;13;0
WireConnection;3;0;12;0
WireConnection;3;1;5;0
WireConnection;3;2;6;0
WireConnection;38;0;3;0
WireConnection;22;0;21;0
WireConnection;22;1;38;0
WireConnection;20;0;19;0
WireConnection;20;1;22;0
WireConnection;18;0;20;0
WireConnection;18;1;38;0
WireConnection;18;2;38;0
WireConnection;24;0;18;0
WireConnection;23;0;24;0
WireConnection;23;1;24;1
WireConnection;23;2;24;2
WireConnection;0;0;23;0
WireConnection;0;1;3;0
ASEEND*/
//CHKSM=830738EC5F19C04A77F9C6CBDA8853324DBA4A40