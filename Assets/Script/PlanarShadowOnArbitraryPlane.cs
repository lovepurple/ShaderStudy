using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 场景接受PlanarShadow面
/// </summary>
[Serializable]
public class PlanarShadowOnArbitraryPlane : MonoBehaviour
{
    //阴影颜色
    [SerializeField]
    public Color ShadowColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

    //阴影物体父节点
    [SerializeField]
    public Transform CastShadowParentTransform = null;

    [SerializeField]
    public float ShadowOffset = 0.4f;

    private HashSet<Material> m_shadowMaterialSet = new HashSet<Material>();
    private Material m_defaultShadowMaterial = null;
    private Shader m_shadowShader = null;

    void Awake()
    {
        CreateDefaultShadowMaterial();
    }


    void OnEnable()
    {
        ResetCastShadowMaterials();
    }

    void Update()
    {
        if (m_defaultShadowMaterial)
        {
            m_defaultShadowMaterial.SetColor("_ShadowColor", ShadowColor);
            m_defaultShadowMaterial.SetFloat("_ShadowBias", ShadowOffset);
        }
    }

    /// <summary>
    /// Initial or Reset all Shadow material for children
    /// </summary>
    private void ResetCastShadowMaterials()
    {
        if (!CastShadowParentTransform)
            CastShadowParentTransform = transform;

        m_shadowMaterialSet.Clear();

        for (int i = 0; i < CastShadowParentTransform.childCount; ++i)
        {
            Transform childTransform = CastShadowParentTransform.GetChild(i);
            Renderer[] childRenderers = childTransform.GetComponentsInChildren<Renderer>(true);
            if (childRenderers == null)
                continue;

            for (int k = 0; k < childRenderers.Length; ++k)
            {
                Renderer childRenderer = childRenderers[k];

                bool renderHaveShadowMaterial = false;
                Material[] renderMaterials = childRenderer.sharedMaterials;
                for (int j = 0; j < renderMaterials.Length; ++j)
                {
                    Material renderMaterial = renderMaterials[j];
                    if (!renderMaterial || !renderMaterial.shader)
                        continue;

                    if (renderMaterial.shader.name == m_defaultShadowMaterial.name)
                    {
                        m_shadowMaterialSet.Add(renderMaterial);
                        renderHaveShadowMaterial = true;
                    }
                }

                if (!renderHaveShadowMaterial)
                {
                    List<Material> renderAddShadowMaterial = new List<Material>();
                    renderAddShadowMaterial.AddRange(renderMaterials);
                    renderAddShadowMaterial.Add(m_defaultShadowMaterial);
                    childRenderer.sharedMaterials = renderAddShadowMaterial.ToArray();

                    m_shadowMaterialSet.Add(m_defaultShadowMaterial);
                }
            }

        }

        foreach (Material shadowMat in m_shadowMaterialSet)
        {
            shadowMat.SetMatrix("_World2Ground", transform.worldToLocalMatrix);
            shadowMat.SetMatrix("_Ground2World", transform.localToWorldMatrix);
        }
    }

    /// <summary>
    /// Children Changed then reset Shadow
    /// </summary>
    private void OnTransformChildrenChanged()
    {
        ResetCastShadowMaterials();
    }

    /// <summary>
    /// Create Default PlanarShadow material for children Renders
    /// </summary>
    /// <returns></returns>
    public Material CreateDefaultShadowMaterial()
    {
        if (!m_shadowShader)
        {
            m_shadowShader = Shader.Find("Custom/PlanarShadowOnCustomPlanar");
            if (!m_shadowShader)
            {
                Debug.LogError("Custom/PlanarShadowOnCustomPlanar 不存在");
                return null;
            }
        }

        m_defaultShadowMaterial = new Material(m_shadowShader);
        m_defaultShadowMaterial.SetColor("_ShadowColor", ShadowColor);
        m_defaultShadowMaterial.SetFloat("_ShadowBias", ShadowOffset);

        return m_defaultShadowMaterial;
    }
}
