using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

/*
 *  获取使用Multiply Render Target 显示Deferred 出来的信息
 */

public class GetMRT : PostEffectsBase
{
    private Material m_matTestMRT = null;
    private Material m_matDepth = null;

    private RenderTexture m_rtAlbedoColor = null;
    private RenderTexture m_rtWorldNormal = null;
    private RenderTexture m_rtWorldPos = null;

    private RenderTexture m_rtBackDepth = null;
    private RenderTexture m_rtFrontDepth = null;

    private RenderBuffer[] m_colorBuffers;
    private RenderBuffer[] m_depthBuffers;

    private Light[] m_directionLights;
    private Light[] m_pointLights;

    private Camera m_camera;

    void OnPreRender()
    {
        //m_camera.SetTargetBuffers(this.m_colorBuffers,)
    }

     void OnPostRender()
    {
        
    }

    public override bool CheckResources()
    {
        m_matTestMRT = CheckShaderAndCreateMaterial(Shader.Find("Deferred Shading/TestMRT"), m_matTestMRT);
        if (m_matTestMRT != null)
            return false;

        m_matDepth = CheckShaderAndCreateMaterial(Shader.Find("Unlit/ViewDepth"), m_matDepth);

        m_camera = GetComponent<Camera>();

        InitRenderTextures();

        CollectLights();

        return base.CheckResources();
    }

    private void InitRenderTextures()
    {
        this.m_rtAlbedoColor = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        this.m_rtWorldNormal = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        this.m_rtWorldPos = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);

        this.m_colorBuffers = new RenderBuffer[3];
        this.m_colorBuffers[0] = this.m_rtAlbedoColor.colorBuffer;
        this.m_colorBuffers[1] = this.m_rtWorldNormal.colorBuffer;
        this.m_colorBuffers[2] = this.m_rtWorldPos.colorBuffer;

        this.m_rtFrontDepth = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        this.m_rtBackDepth = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        this.m_depthBuffers = new RenderBuffer[2];
        this.m_depthBuffers[0] = m_rtFrontDepth.colorBuffer;
        this.m_depthBuffers[1] = m_rtFrontDepth.colorBuffer;
    }

    private void CollectLights()
    {
        this.m_directionLights = Light.GetLights(LightType.Directional, 0);
        this.m_pointLights = Light.GetLights(LightType.Point, 0);
    }
}
