using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class SSAO : MonoBehaviour
{
    public float SSAOIntension = 0.1f;
    public float SSAODistance = 1f;
    public float SSOSampleRadius = 1f;
    public Texture2D SampleNoiseTexture = null;

    public Color SSAOColor = Color.black;
    public bool DebugSSAOTexture = false;

    public Material m_ssaoMat = null;
    private Camera m_SSAOCamera = null;

    private void Awake()
    {
        if (m_SSAOCamera == null)
        {
            m_SSAOCamera = GetComponent<Camera>();
            m_SSAOCamera.depthTextureMode |= DepthTextureMode.Depth;
            m_SSAOCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
        }
    }

    void OnEnable()
    {
        CreateMaterialIfNeed();
    }

    private void OnPreRender()
    {
        SetRenderParams();
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_ssaoMat != null)
        {
            int passIndex = 0;

            RenderTexture temp = RenderTexture.GetTemporary(source.width, source.width);

            //如果不手动传入_MainTex Shader里_MainTex_TexelSize没有值
            m_ssaoMat.SetTexture("_MainTex", source);
            Graphics.Blit(source, temp, m_ssaoMat, passIndex);

            //if (DebugSSAOTexture)
            Graphics.Blit(temp, destination);
            //else
            //{
            //    //Blur
            //    passIndex++;
            //    RenderTexture tempBlur = RenderTexture.GetTemporary(source.width, source.width);
            //    m_ssaoMat.SetVector("_BlurDirection", new Vector4(1 / source.width, 0, 0, 0));
            //    Graphics.Blit(temp, tempBlur, m_ssaoMat, passIndex);

            //    RenderTexture tempBlur2 = RenderTexture.GetTemporary(source.width, source.width);
            //    m_ssaoMat.SetVector("_BlurDirection", new Vector4(0, 1 / source.height, 0, 0));
            //    Graphics.Blit(tempBlur, tempBlur2, m_ssaoMat, passIndex);

            //    //composite
            //    passIndex++;

            //    Graphics.Blit(tempBlur2, destination, m_ssaoMat, passIndex);

            //    RenderTexture.ReleaseTemporary(tempBlur);
            //    RenderTexture.ReleaseTemporary(tempBlur2);
            //}

            RenderTexture.ReleaseTemporary(temp);

        }
    }


    private void SetRenderParams()
    {
        if (m_ssaoMat != null)
        {
            Matrix4x4 gpuProjectionMatrix = GL.GetGPUProjectionMatrix(m_SSAOCamera.projectionMatrix, false);

            m_ssaoMat.SetMatrix("_CameraProjectionMatrix_IT", gpuProjectionMatrix.inverse);
            m_ssaoMat.SetFloat("_AOIntension", SSAOIntension);
            m_ssaoMat.SetFloat("_Distance", SSAODistance);
            m_ssaoMat.SetFloat("_SampleRadius", SSOSampleRadius);
            m_ssaoMat.SetColor("_CustomColor", SSAOColor);
            m_ssaoMat.SetTexture("_NoiseTexture", SampleNoiseTexture);
            m_ssaoMat.SetFloat("_NoiseTextureSize", SampleNoiseTexture.width);


            UpdateMaterialMacroParams();
        }

    }

    /// <summary>
    /// 更新宏状态
    /// </summary>
    private void UpdateMaterialMacroParams()
    {
        List<string> materialKeyWords = new List<string>();

        materialKeyWords.Add(SampleNoiseTexture != null ? "USE_NOISETEXTURE_ON" : "USE_NOISETEXTURE_OFF");

        materialKeyWords.Add(DebugSSAOTexture ? "DEBUG_SSAO_ON" : "DEBUG_SSAO_OFF");

        materialKeyWords.Add(SSAOColor != Color.black ? "USE_CUSTOM_COLOR_ON" : "USE_CUSTOM_COLOR_OFF");

        m_ssaoMat.shaderKeywords = materialKeyWords.ToArray();
    }

    private void CreateMaterialIfNeed()
    {
        if (m_ssaoMat == null)
        {
            m_ssaoMat = new Material(Shader.Find("Hidden/PostEffect/SSAO"));
        }

    }

}
