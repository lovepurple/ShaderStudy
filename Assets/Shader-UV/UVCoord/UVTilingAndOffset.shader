/*
    UV 坐标系练习,通过Tiling(Scale) 及Offset 控制贴图在面上的缩放 及偏移
    Offset 正负的方向
    Tiling UV的大小
*/
Shader "UVOperate/UVTilingAndOffset"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DecalTex("Decal Texture",2D)="black"{}
        _DecalTilingX("Decal Tiling X",Float)=1.0
        _DecalTilingY("Decal Tiling Y",Float) = 1.0
        _DecalOffsetU("Decal Offset U",Float) = 0.0
        _DecalOffsetV("Decal Offset V",Float) = 0.0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #include "UnitySprites.cginc"

            sampler2D _DecalTex;
            float _DecalOffsetU;
            float _DecalOffsetV;
            float _DecalTilingX;
            float _DecalTilingY;


            float4 frag (v2f i) : SV_Target
            {
                float4 mainCol = tex2D(_MainTex,i.texcoord);

                /*
                    1. Tiling 的理解：
                            1> 0 < tiling < 1 时，原图uv范围由[0,1] => [0,<=1] ，mesh上的uv范围被压缩，
                                例如:tiling = 0.8时,原uv=(0,1)
                                    压缩后，变成(0,0.8) , 也就是本来应该采样的是(0,1) 现在变为(0,0.8) 因此看上去贴图被放大
                            2> tiling > 1时，图图范围由[0,1] =>[0,>1] 
                                例如:tiling = 2 ，原uv=(0,1)
                                    uv变成(0,2) ，本来采样的是(0,1) 现在变为(0,2) ，对于Warp Mode = Repeat的，重复采样两次，对于Warp Mode = Clamp的 超出1之后都按边缘像素采样，因此看上去被压缩
                    2. Offset的理解
                            Offset >0 时，原uv采样的是(0,1)=>(offset ,1+offset) 原(0,0)=> (0,offset) 看上去图像向(0,0)偏移
                            Offset < 0 时，原uv(0,1)=>(-offset,1-offset) 原(0,1）采样变成(0,1-offset)看上去向远离(0,0)偏移 
                
                */
                
                float4 decalCol = tex2D(_DecalTex,i.texcoord*float2(_DecalTilingX,_DecalTilingY) + float2(_DecalOffsetU,_DecalOffsetV));

                float4 col =lerp(mainCol,decalCol,decalCol.a);
                col.rgb *= col.a;

                return  col;
            }
            ENDCG
        }
    }
}
