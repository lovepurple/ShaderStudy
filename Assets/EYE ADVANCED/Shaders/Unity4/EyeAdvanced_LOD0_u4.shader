Shader "EyeAdvanced/Unity4/EyeAdvanced_LOD0_u4" {
Properties {

	_pupilSize("Pupil Dilation", Range(0.0,1.0)) = 0.7
	_irisSize("Eye Iris Size", Range(1.5,5.0)) = 2.15
	_parallax("Parallax Effect", Range(0.0,0.05)) = 0.025
	_scleraSize("Eye Sclera Size", Range(0.85,2.2)) = 1.0

	_scleraColor ("Sclera Color", Color) = (1,1,1,1)
	_irisColor ("Iris Color", Color) = (1,1,1,1)
	_illumColor ("Iris Illumination", Color) = (1,1,1,1)
   	_subDermColor ("SubDermal Color", Color) = (0.5, 0.5, 0.5, 1)
	_brightShift("Brightness Shift", float) = 1.0
	_smoothness("Specular Size", Range(0.0,1.0)) = 0.0

	_IrisColorTex ("Iris Color", 2D) = "white" {}
	_IrisTex ("Iris Texture", 2D) = "white" {}
	_CorneaBump ("Cornea Normal Map", 2D) = "bump" {}
	_EyeBump ("Eye Normal Map", 2D) = "bump" {}
	_MainTex ("Sclera Texture", 2D) = "white" {}

	_SpecularCube ("Specular Cube", CUBE) = "" {}
	_DiffuseCube ("Diffuse Cube", CUBE) = "" {}

}

SubShader { 
	Tags {"RenderType"="Opaque" "Queue"= "Geometry"}
	Cull Back

	
CGPROGRAM
#pragma target 3.0
//#pragma surface surf TanukiDigital_PBR_Eye
    #include "UnityPBSLighting.cginc"
    #pragma surface surf StandardSpecular
#pragma glsl

sampler2D _IrisColorTex;
sampler2D _IrisTex;
sampler2D _MainTex;
samplerCUBE _SpecularCube;
samplerCUBE _DiffuseCube;
float4 _albedoColor;
float4 reflectionMatte;
float4 irradianceTex;
float4 _subDermColor;
float3 albedoColor;
float _roughness;
float _reflective;
float _metalMap;
float _ambientMap;
float _skinMap;
float _irisSize;
float _scleraSize;
float _pupilSize;
sampler2D _CorneaBump;
sampler2D _EyeBump;
float4 _scleraColor;
float4 _irisColor;
float4 _irisColorB;
float4 _pupilColor;
float4 _illumColor;
float _parallax;
float _brightShift;
float irismasktex;
float _smoothness;


struct Input {
	float2 uv_MainTex;
	float3 viewDir;
	float3 worldRefl;
	INTERNAL_DATA
};



fixed4 LightingTanukiDigital_PBR_Eye (SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten){

	fixed4 c;

	//------------------------------
	//##  WORLD LIGHT FUNCTIONS  ##
	//------------------------------
	// REMAP LIGHT
	// For all intents and purposes, this is a hax, and has
	// no place in a Physically-based syste, :D
	half4 inLight = _LightColor0;
	half4 outLight = inLight;
	outLight = saturate(outLight*lerp(1.0,-0.1,_roughness));//*(atten);

	//-------------------------------
	//##  LIGHT TERM CALCULATION  ##
	//-------------------------------
	s.Normal = normalize(s.Normal);
	half NdotV = dot(s.Normal,viewDir);
	half cNdotV = max(0,dot(s.Normal,viewDir));
	half h = max(0,dot(s.Normal,normalize(lightDir+viewDir)));
	
	
	//---------------------------
	//##  INDEX OF REFRACTION  ##
	//---------------------------
	// below formula converts measurable Index of Refraction (IOR)
	// into the specular index (angle of incidence) value at f0
	half IOR = 1.635;
	half3 f0 = (1.0-IOR,2.0)/pow(1.0+IOR,2);
	
	// set f0 of dielectrics to default value 0f 0.045
	f0 = half3(_reflective,_reflective,_reflective);
	
	// set f0 of metal specular color on diffuse map color
	f0 = lerp(f0,albedoColor,_metalMap);
	
	
	//---------------------------
	//##  REFLECTANCE TERM  ##
	//---------------------------
	half3 reflectance = normalize(lightDir + viewDir)*(outLight.rgb*lightDir*max(0,dot(s.Normal,lightDir)));
	//half3 b_reflectance = normalize(lightDir + viewDir)*(outLight.rgb*lightDir*min(0,dot(s.Normal,lightDir)));


	//---------------------------
	//##  FRESNEL CALULATION  ##
	//---------------------------
	half3 fresnel;
	
	// Schlick function
	half3 f_schlick = f0+(1.0-f0)*pow((dot(s.Normal,normalize(lightDir+viewDir))),5);
	f_schlick *= f0+(1.0-f0)*pow((1.0-NdotV),5);
	f_schlick = max(f_schlick,f0+(1.0-f0)*pow((1.0-NdotV),5));
	f_schlick = saturate(f_schlick);
	
	fresnel = f_schlick;


	//--------------------------------------
	//##  NORMAL DISTRIBUTION FUNCTIONS  ##
	//--------------------------------------
	half ndf = 1.0;

	// Phong
	// This is the closest match to the built-in NDF used in Unity 5 Standard Shader
	// Phong is best matched for hard/gloss plastics, rubber, and other man-made materials
	//#if NDF_PHONG
	//float m = pow(8192.0,(1.0-_roughness));
	//half ndf_phong = ((m+2.0)/6.2837)*pow(max(0,dot(s.Normal,normalize(lightDir+viewDir))),m);
	//ndf = ndf_phong;
	//#endif

	// GGX (Trowbrige and Reitz)
	// This NDF has a longer falloff tail than Phong does, and
	// is more useful in natural environments, skin, wood, metal etc.
	//#if NDF_GGX
	float ms = pow(_smoothness,2.5);
	half ndf_ggx = (ms*ms)/pow((h*h)*((ms*ms)-1.0)+1.0,2.0);
	ndf = ndf_ggx;
	//#endif


	//---------------------------
	//##  GEOMETRY FUNCTIONS  ##
	//---------------------------
	half gf = 1.0;
	
	// implicit
	//#if GF_IMPLICIT
	//half gf_implicit = max(0,dot(s.Normal,lightDir))*cNdotV;
	//gf = gf_implicit;
	//#endif
	
	// Keleman
	//#if GF_KELEMAN
	half gf_keleman = saturate((1.0/pow(dot(lightDir,normalize(lightDir+viewDir)),2))*(dot(s.Normal,lightDir)*NdotV));
	gf = gf_keleman;
	//#endif

	//-----------------------------
	//##  IRRADIANCE FUNCTIONS  ##
	//-----------------------------
	// This is a basic irradiance/diffuse lighting relying
	// on a lower mipmap of the current reflection cubemap
	half3 irradiance;
	irradiance = irradianceTex;

	
	//-----------------------------
	//##  SUBSURFACE FUNCTIONS  ##
	//-----------------------------
	// note, this is a completely faked SSS "formula"
	// it ain't based on anything "scientific" sounding.
	half sss;
	sss = min(saturate(lerp(-0.05,0.1,dot(s.Normal,viewDir))),1.0-gf)*max(0.1,dot(s.Normal,-lightDir))*fresnel;


	//------------------------------------
	//------------------------------------
	//##  FINAL COMBINATION FUNCTIONS  ##
	//------------------------------------
	//------------------------------------
	//base color
	c.rgb = s.Albedo;
	
	//Direct Lighting
	//gf = lerp(gf,lerp(gf,1.0,0.25),irismasktex);
	c.rgb = (s.Albedo * _brightShift * outLight.rgb * gf * (atten*1));// * (atten * 2);
	
	//Irradiance (IBL / Diffuse Lighting)
	//c.rgb += s.Albedo*irradiance;

	//Add IRRADIANCE
	c.rgb += (reflectionMatte.rgb*f0*(outLight.a));

	//Fresnel
	//c.rgb += (s.Albedo+reflectionMatte)*fresnel*gf;

	//fresnel
	c.rgb += (s.Albedo+reflectionMatte*(1.0-_metalMap))*fresnel*outLight.a*gf;

	//micro occlusion term
	//half _fresnelOcclusion = lerp(0.0,1.0,saturate(lerp(lerp(-1.0,2.0,1.0-cNdotV),lerp(-1.0,1.0,1.0-cNdotV),_roughness)));
	//c.rgb *= lerp(saturate(_ambientMap),saturate(_ambientMap+0.45),_fresnelOcclusion);
	
	//Sub Surface Scattering
	c.rgb += sss*_skinMap*(1.0-gf)*(1.0-_metalMap)*500.0*(_subDermColor.rgb*(outLight.rgb));
	
	//clamp values
	c.rgb = saturate(c.rgb);
	
	//Add Specular NDF
	c.rgb += (ndf*outLight.rgb);
	
	//re-Map black output
	half bkRemap = lerp(-0.015,0.0,_roughness);
	c.r = lerp(bkRemap,1.0,c.r);
	c.g = lerp(bkRemap,1.0,c.g);
	c.b = lerp(bkRemap,1.0,c.b);

	//Boost eye reflectivity (because why not?)
	c.rgb = lerp(c.rgb,reflectionMatte*lerp(1.0,_brightShift,irismasktex*outLight.a*2),lerp(0.01,0.1,fresnel));

	//------------------------
	//##  SET ALPHA VALUE  ##
	//------------------------
	c.a = s.Alpha;

	c.rgb *= atten;

	return c;
}




void surf (Input IN, inout SurfaceOutputStandardSpecular o) {

	//CALCULATE NORMAL MAPS
	half3 cBump = UnpackNormal(tex2D(_CorneaBump, float2((IN.uv_MainTex.x*_irisSize)-((_irisSize-1.0)/2.0),(IN.uv_MainTex.y*_irisSize)-((_irisSize-1.0)/2.0))));

	//CALCULATE ALBEDO MAP (SCLERA)
	half4 scleratex = tex2D(_MainTex, float2((IN.uv_MainTex.x*_scleraSize)-((_scleraSize-1.0)/2.0),(IN.uv_MainTex.y*_scleraSize)-((_scleraSize-1.0)/2.0)));
	scleratex.rgb = lerp(scleratex.rgb,scleratex.rgb*_scleraColor.rgb,_scleraColor.a);
	half3 eBump = UnpackNormal(tex2D(_EyeBump, float2((IN.uv_MainTex.x*_scleraSize)-((_scleraSize-1.0)/2.0),(IN.uv_MainTex.y*_scleraSize)-((_scleraSize-1.0)/2.0))));
	
	//CALCULATE ALBEDO MAP (IRIS)
	irismasktex = tex2D(_MainTex, float2((IN.uv_MainTex.x*_irisSize)-((_irisSize-1.0)/2.0),(IN.uv_MainTex.y*_irisSize)-((_irisSize-1.0)/2.0))).a;

	//FINAL NORMAL COMBINATION
	o.Normal = lerp(eBump,cBump,irismasktex);


	// get mask texture
	half uvMask = 1.0-tex2D(_IrisTex,IN.uv_MainTex).b;

	
	//CALCULATE IRIS TEXTURE
	half iSize = _irisSize * 0.6;
	float2 irUVc = IN.uv_MainTex;
	irUVc = float2((IN.uv_MainTex.x*iSize)-((iSize-1.0)/2.0),((IN.uv_MainTex.y)*iSize)-((iSize-1.0)/2.0));

	_pupilSize = lerp(lerp(0.5,0.2,iSize/5),lerp(1.2,0.75,iSize/5),_pupilSize);

	irUVc = (irUVc*((-1.0+(uvMask*_pupilSize)))-(0.5*(uvMask*_pupilSize)));


	//CALCULATE PARALLAX COORDS
	float2 irUVp;
	irUVp = float2((IN.uv_MainTex.x*iSize)-((iSize-1.0)/2.0),(IN.uv_MainTex.y*iSize)-((iSize-1.0)/2.0));


	half4 plxtex = tex2D(_IrisTex,irUVp).g;
	float _Parallax = lerp(0.0,_parallax,plxtex);
	//_Parallax = _parallax;//*plxtex;

	//CALCULATE IRIS/PUPIL MASK TEXTURES
	float2 irUV;
	irUV.x = lerp((IN.uv_MainTex.x*0.75)-((0.75-1.0)/2.0),(IN.uv_MainTex.x*_pupilSize)-((_pupilSize-1.0)/2.0),IN.uv_MainTex.x);
	irUV.y = lerp((IN.uv_MainTex.y*0.75)-((0.75-1.0)/2.0),(IN.uv_MainTex.y*_pupilSize)-((_pupilSize-1.0)/2.0),IN.uv_MainTex.y);

	float2 offset = ParallaxOffset(tex2D(_IrisTex,irUV).b, _Parallax, -IN.viewDir);



	//SET SPECIFIC PHYSICAL EYE VALUES
	_metalMap = 0.0;
	_roughness = lerp(0.14,0.1,irismasktex);
	_smoothness = lerp(_roughness,lerp(_roughness,0.3,irismasktex),_smoothness);

	_reflective = 0.045;
	_skinMap = (1.0-irismasktex)*_subDermColor.a;

	//get iris and pupil texture
	half3 irisColTex = tex2D(_IrisColorTex,irUVc-offset).rgb;


	//combine sclera and iris colors
	irisColTex = lerp(irisColTex,irisColTex*_irisColor.rgb,_irisColor.a);
	o.Albedo = lerp(scleratex.rgb*(1.0-irismasktex),irisColTex,irismasktex);

	//backscatter effects
	o.Emission = o.Albedo*(5.0*_illumColor.a)*_illumColor.rgb*irismasktex;
	
	//darken iris edges
	o.Albedo *= lerp(1.0,1.0,(1.0-irismasktex)*irismasktex*2);
	

	//--------------------------
	//##  CALCULATE CUBEMAP  ##
	//--------------------------	
	// currently cubemap is explicitly defined.
	// need to look into reflection probe inheritence
	// and skybox inheritence instead for Unity 5 compatibility.
	
	// CALCULATE CUBE COORDINATES
	float3 cubeCoord = WorldReflectionVector(IN,o.Normal);
	float3 ir_cubeCoord = WorldReflectionVector(IN, lerp(o.Normal,half3(0,0,1.0),1.0));

	// GET TENKOKU CUBE MAPS
	fixed useMip = 5*_roughness;

	// GET EXPLICIT CUBE MAPS
	reflectionMatte = texCUBEbias(_SpecularCube, float4(cubeCoord, 0.0)); //mipmap reflection
	irradianceTex = texCUBEbias(_DiffuseCube, float4(ir_cubeCoord, 0.0)); //diffuse lighting
	reflectionMatte = lerp(reflectionMatte,irradianceTex,saturate(lerp(0.0,2.0,_roughness))); //blend diffuse reflections
	
	o.Albedo = lerp(o.Albedo,albedoColor*reflectionMatte,_metalMap);

	//------------------------------
	//##  CALCULATE ALPHA / CLIP  ##
	//------------------------------
	o.Alpha = 1.0;
	
o.Specular = lerp(0.025,0.05,irismasktex);
o.Smoothness = lerp(0.7,0.9,irismasktex);


}
ENDCG



}

Fallback "Diffuse"

}
