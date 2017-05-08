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


        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Camera.main.depthTextureMode |= DepthTextureMode.Depth;
            Graphics.Blit(source, destination, TargetMaterial);


        }



    }
}
