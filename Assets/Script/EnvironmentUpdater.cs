using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentUpdater : MonoBehaviour
{

    public Cubemap m_targetEnvironmentCubemap;

    public Camera m_realtimeEnvironmentCamera = null;

    private Material m_material = null;

    private float m_timeElapsed = 0f;
    public float m_updateCubemapInterval = 1f;


    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        m_material = renderer.material;

        if (!m_targetEnvironmentCubemap)
        {
            m_targetEnvironmentCubemap = new Cubemap(256, TextureFormat.RGB24, false);
            m_material.SetTexture("_EnvironmentCube", m_targetEnvironmentCubemap);
        }

        if (!m_realtimeEnvironmentCamera)
        {
            m_realtimeEnvironmentCamera = gameObject.AddComponent<Camera>();
            m_realtimeEnvironmentCamera.allowHDR = false;
        }

    }

    private void Update()
    {
        m_timeElapsed += Time.deltaTime;
        if (m_timeElapsed >= m_updateCubemapInterval)
        {
            m_realtimeEnvironmentCamera.RenderToCubemap(this.m_targetEnvironmentCubemap);
            m_timeElapsed = 0;
        }


    }

}
