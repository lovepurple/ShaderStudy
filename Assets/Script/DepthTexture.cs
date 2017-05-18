using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Game.Actor
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class DepthTexture : MonoBehaviour
    {
        public Material TargetMaterial;

        void OnEnable()
        {
            //必须强制打开，否则不渲染深度图
            //  Camera.main.depthTextureMode |= DepthTextureMode.DepthNormals;
            Camera.main.depthTextureMode = DepthTextureMode.Depth;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (TargetMaterial != null)
                Graphics.Blit(source, destination, TargetMaterial);
        }


    }
}
