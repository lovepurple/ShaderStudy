using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// URP 管线中自定义屏幕后处理，使用SPR   
/// 自定义渲染管线脚本需要 继承ScriptableRendererFeature
/// </summary>
public class URPPostProcessConstrast : ScriptableRendererFeature
{
    [SerializeField]
    public Shader m_renderShader;

    private Material m_renderMaterial = null;
    private PostProcessConstractRenderPass m_renderPass;

    [Range(0,1)]
    public float Constrast = 1.0f;


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
    }

    public override void Create()
    {
        if (this.m_renderMaterial == null)
        {
            this.m_renderMaterial = new Material(this.m_renderShader);
        }

        if (m_renderPass == null)
        {
            m_renderPass = new PostProcessConstractRenderPass(this.m_renderMaterial);
            //指定CommandBuffer的事件
            m_renderPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }


    }

    /// <summary>
    /// 要执行的Pass
    /// </summary>
    private class PostProcessConstractRenderPass : ScriptableRenderPass
    {
        private Material m_renderMaterial = null;
        private int m_MainTexID = Shader.PropertyToID("_MainTex");
        private int m_Constrast = Shader.PropertyToID("_Constrast");

        public PostProcessConstractRenderPass(Material material)
        {
            this.m_renderMaterial = material;
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer commandBuffer = CommandBufferPool.Get();
        }
    }
}
