// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/quan" {  
   Properties {  
      _MainTex ("RGBA Texture Image", 2D) = "white" {}   
      _Alpha("Alpha",Range(0,1)) = 0.5
   }  
   SubShader {  
      Tags {"Queue" = "Transparent"}
   
      Pass {      
         Cull Back
		 ZTest off 
         Blend SrcAlpha One   
   
         CGPROGRAM  
   
         #pragma vertex vert    
         #pragma fragment frag   
   
         uniform sampler2D _MainTex;      
         uniform float _Cutoff;  
         uniform float _Alpha;
   
         struct vertexInput {  
            float4 vertex : POSITION;  
            float4 texcoord : TEXCOORD0;  
         };  
         struct vertexOutput {  
            float4 pos : SV_POSITION;  
            float4 tex : TEXCOORD0;  
         };  
   
         vertexOutput vert(vertexInput input)   
         {  
            vertexOutput output;  
   
            output.tex = input.texcoord;  
            output.pos = UnityObjectToClipPos(input.vertex);  
            return output;  
         }  
   
         float4 frag(vertexOutput input) : COLOR  
         {  
            float4 c =  tex2D(_MainTex, input.tex.xy);    
            c.a = _Alpha * c.a;
            return c;
         }  
   
         ENDCG  
      }  
   }  
   // The definition of a fallback shader should be commented out   
   // during development:  
   // Fallback "Unlit/Transparent"  
}  