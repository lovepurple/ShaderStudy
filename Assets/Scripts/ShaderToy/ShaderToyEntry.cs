/********************************************************************
	created:  2019-08-12 17:46:12
	filename: ShaderToyEntry.cs
	author:	  songguangze@outlook.com
	
	purpose: 用于ShaderToy效果预览
*********************************************************************/
using EngineCore;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderToyEntry : MonoBehaviour
{
    public Shader ImageEffectShader = null;

    //private ObjectPool<CommandBuffer> m_commandBufferPool = new ObjectPool<CommandBuffer>();
    private CommandBuffer m_currentCommandBuffer = null;
    private Material m_mat = null;

    private void Awake()
    {
        if (ImageEffectShader == null)
            m_mat = new Material(Shader.Find("Legacy Shaders/Diffuse Fast"));
        else
            m_mat = new Material(ImageEffectShader);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, m_mat);
    }

    private void OnPostRender()
    {
        return;
        if (ImageEffectShader != m_mat.shader)
        {
            m_mat.shader = ImageEffectShader;

            ResetCommandBuffer();
        }
    }


    private void ResetCommandBuffer()
    {
        //if (m_currentCommandBuffer != null)
        //    Camera.main.RemoveCommandBuffer(CameraEvent.AfterImageEffects, m_currentCommandBuffer);

        //m_currentCommandBuffer = m_commandBufferPool.Get();

        ////CommandBuffer，相当于申请一个名叫_ScreenCopyTexture的 Buffer
        ////-1,-1 表示当前屏幕大小？
        //int screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");
        //m_currentCommandBuffer.GetTemporaryRT(screenCopyID, -1, -1, 0, FilterMode.Bilinear);

        ////将当前缓冲区中的Buffer 给到刚申请的
        //m_currentCommandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, screenCopyID);

        ////相当于Graphic.Blit
        //m_currentCommandBuffer.Blit(screenCopyID, BuiltinRenderTextureType.CurrentActive, m_mat);

        //Camera.main.AddCommandBuffer(CameraEvent.AfterImageEffects, m_currentCommandBuffer);
    }
}
