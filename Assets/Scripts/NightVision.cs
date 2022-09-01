using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightVision : MonoBehaviour
{

    public Material NightVisionMat;

    // Use this for initialization
    void Start()
    {

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (NightVisionMat != null)
        {
            NightVisionMat.SetTexture("_MainTex", source);
            NightVisionMat.SetFloat("_RandomValue", Random.value);

            Graphics.Blit(source, destination,NightVisionMat);
        }
        else
            Graphics.Blit(source, destination);

    }

}
