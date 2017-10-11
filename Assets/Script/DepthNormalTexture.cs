using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class DepthNormalTexture : MonoBehaviour
{
    public DisplayTextureMode DisplayType = DisplayTextureMode.DepthNormal_Normal;

    private Material depthMat;

    void OnEnable()
    {
        CreateMaterialIfNeed();

        //必须强制打开，否则不渲染深度图
        Camera.main.depthTextureMode |= DepthTextureMode.DepthNormals;
        Camera.main.depthTextureMode |= DepthTextureMode.Depth;
    }

    private void CreateMaterialIfNeed()
    {
        if (depthMat == null)
            depthMat = new Material(Shader.Find("Hidden/DepthNormalTexture"));
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (depthMat != null)
        {
            List<string> shaderKeywords = new List<string>();

            switch (DisplayType)
            {
                case DisplayTextureMode.Depth:
                    shaderKeywords.Add("SHOW_DEPTH_ON");
                    shaderKeywords.Add("SHOW_DEPTHNORMAL_DEPTH_OFF");
                    shaderKeywords.Add("SHOW_DEPTHNORMAL_NORMAL_OFF");
                    break;
                case DisplayTextureMode.DepthNormal_Depth:
                    shaderKeywords.Add("SHOW_DEPTH_OFF");
                    shaderKeywords.Add("SHOW_DEPTHNORMAL_NORMAL_OFF");
                    shaderKeywords.Add("SHOW_DEPTHNORMAL_DEPTH_ON");
                    break;
                case DisplayTextureMode.DepthNormal_Normal:
                    shaderKeywords.Add("SHOW_DEPTH_OFF");
                    shaderKeywords.Add("SHOW_DEPTHNORMAL_NORMAL_ON");
                    shaderKeywords.Add("SHOW_DEPTHNORMAL_DEPTH_OFF");
                    break;
                default:
                    shaderKeywords.Add("SHOW_DEPTH_OFF");
                    shaderKeywords.Add("SHOW_DEPTHNORMAL_NORMAL_OFF");
                    shaderKeywords.Add("SHOW_DEPTHNORMAL_DEPTH_OFF");
                    break;
            }

            //使用宏开关Shader中的状态
            depthMat.shaderKeywords = shaderKeywords.ToArray();

            Graphics.Blit(source, destination, depthMat);
        }
    }

    public enum DisplayTextureMode
    {
        DepthNormal_Normal,
        DepthNormal_Depth,
        Depth,
        None
    }
}

