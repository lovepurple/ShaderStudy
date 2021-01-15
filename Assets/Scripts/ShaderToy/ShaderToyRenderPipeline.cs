using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JCDeferredShading;
using UnityEngine.Experimental.Rendering;
using UnityEditor;
using System.IO;
using UnityEngine.Rendering;

public class ShaderToyRenderPipeline : RenderPipeline
{
    //private IRenderPipelineAsset renderPipelineAsset;

    //public ShaderToyRenderPipeline(IRenderPipelineAsset pipelineAsset)
    //{
    //    this.renderPipelineAsset = pipelineAsset;
    //}


    //public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
    //{
    //    base.Render(renderContext, cameras);
    //    CommandBuffer clearBuffer = new CommandBuffer();
    //    clearBuffer.name = "Clear Buffer";
    //    clearBuffer.ClearRenderTarget(true, true, Color.blue);
    //    //FilterRenderersSettings 

    //    //clearBuffer.SetRenderTarget()

    //    //clearBuffer.SetRenderTarget(new RenderTargetIdentifier())
    //    renderContext.ExecuteCommandBuffer(clearBuffer);


    //    clearBuffer.Release();
    //    renderContext.Submit();
    //}
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        CommandBuffer clearBuffer = new CommandBuffer();
        clearBuffer.name = "Clear Buffer";
        clearBuffer.ClearRenderTarget(true, true, Color.blue);
        //FilterRenderersSettings 

        //clearBuffer.SetRenderTarget()

        //clearBuffer.SetRenderTarget(new RenderTargetIdentifier())
        context.ExecuteCommandBuffer(clearBuffer);


        clearBuffer.Release();
        context.Submit();
    }
}

public class ShaderToyRenderPipelineAsset : RenderPipelineAsset
{
    //protected override IRenderPipeline InternalCreatePipeline()
    //{
    //    return new ShaderToyRenderPipeline(this);
    //}

#if UNITY_EDITOR
    public const string PIPELINE_ASSET_PATH = "Assets/shadertoypipeline.asset";

    [MenuItem("ShaderToy/CreateRenderPipelineAsset")]
    public static void CreateShaderToyRenderPipelineAssets()
    {
        ShaderToyRenderPipelineAsset assetInstance = ScriptableObject.CreateInstance<ShaderToyRenderPipelineAsset>();
        if (File.Exists(PIPELINE_ASSET_PATH))
            File.Delete(PIPELINE_ASSET_PATH);

        AssetDatabase.CreateAsset(assetInstance, PIPELINE_ASSET_PATH);
    }

    protected override RenderPipeline CreatePipeline()
    {
        throw new System.NotImplementedException();
    }
#endif
}


