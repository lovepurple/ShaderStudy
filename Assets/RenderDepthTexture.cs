using UnityEngine;

/// <summary>
/// 渲染深度图或法线图
/// </summary>
[RequireComponent(typeof(Camera))]
public class RenderDepthTexture : MonoBehaviour
{
    public bool IsRenderDepthTexture = false;
    public bool IsRenderDepthNormalTexture = false;

    private Camera m_currentCamera = null;
    public Shader m_shader = null;

    private Material mat;
    void Start()
    {
        m_currentCamera = GetComponent<Camera>();
        mat = new Material(m_shader);
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        Graphics.Blit(source, destination, mat);
    }

    void Update()
    {
        if (!m_currentCamera)
            return;

        if (IsRenderDepthTexture)
            m_currentCamera.depthTextureMode |= DepthTextureMode.Depth;
        else
            m_currentCamera.depthTextureMode &= ~DepthTextureMode.Depth;

        if (IsRenderDepthNormalTexture)
            m_currentCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
        else
            m_currentCamera.depthTextureMode &= ~DepthTextureMode.DepthNormals;
    }

    
}
