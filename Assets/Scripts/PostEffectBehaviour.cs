using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PostEffectBehaviour : MonoBehaviour
{
    public Material m_renderMateiral;


    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (this.m_renderMateiral != null)
            Graphics.Blit(src, dest, m_renderMateiral);
        else
            Graphics.Blit(src, dest);

    }

}
