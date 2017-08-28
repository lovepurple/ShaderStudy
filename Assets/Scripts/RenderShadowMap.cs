using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RenderShadowMap : MonoBehaviour
{
    public Light m_light = null;
    public GameObject m_sceneBoundObj = null;

    private Camera m_shadowCamera = null;
    private GameObject m_shadowCameraObj = null;
    private RenderTexture m_shadowDepthTexture = null;
    private Shader m_depthGrayShader = null;

    private BoxCollider m_sceneBoundsCollider = null;
    private Bounds m_sceneBounds = default(Bounds);
    private List<Vector3> m_sceneWorldConnerList = new List<Vector3>();

    private void Awake()
    {
        InitLightCamera();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSceneBounds();
    }

    private void OnPreRender()
    {
        m_shadowCamera.RenderWithShader(m_depthGrayShader, "");
        Shader.SetGlobalTexture("_ShadowMap", m_shadowDepthTexture);
    }


    /// <summary>
    /// 初始化
    /// </summary>
    private void InitLightCamera()
    {
        m_shadowCameraObj = new GameObject("Shadow Camera Object");
        m_shadowCameraObj.transform.SetParent(m_light.transform);
        m_shadowCameraObj.transform.localRotation = Quaternion.identity;

        this.m_shadowCamera = this.m_shadowCameraObj.AddComponent<Camera>();
        this.m_shadowCamera.depthTextureMode = DepthTextureMode.Depth;

        this.m_shadowDepthTexture = new RenderTexture(Screen.width, Screen.height, 24);
        this.m_shadowCamera.targetTexture = m_shadowDepthTexture;
        this.m_shadowCamera.enabled = false;
        this.m_shadowCamera.orthographic = true;

        this.m_depthGrayShader = Shader.Find("Hidden/SceneDepth");
    }

    private void UpdateSceneBounds()
    {
        if (!m_sceneBoundsCollider && m_sceneBoundObj)
            m_sceneBoundsCollider = m_sceneBoundObj.GetComponent<BoxCollider>();

        List<Vector3> sceneWorldConnerList = GetSceneWorldAABB(m_sceneBoundsCollider.bounds, m_sceneBoundObj.transform);
        SetLightCameraFitScene(m_light.transform, m_shadowCamera, sceneWorldConnerList);
    }



    /// <summary>
    /// 获取场景AABB包围盒的8个顶点
    /// </summary>
    /// <param name="sceneBounds"></param>
    /// <param name="sceneBoundsTransform"></param>
    /// <returns></returns>
    private List<Vector3> GetSceneWorldAABB(Bounds sceneBounds, Transform sceneBoundsTransform)
    {
        Vector3 boundsCenterPos = sceneBoundsTransform.localToWorldMatrix.MultiplyPoint(sceneBounds.center);

        m_sceneWorldConnerList.Clear();
        Vector3 boundsExtension = sceneBounds.extents;

        //下部分
        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x - boundsExtension.x, boundsCenterPos.y - boundsExtension.y, boundsCenterPos.z - boundsExtension.z));
        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x - boundsExtension.x, boundsCenterPos.y - boundsExtension.y, boundsCenterPos.z + boundsExtension.z));
        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x + boundsExtension.x, boundsCenterPos.y - boundsExtension.y, boundsCenterPos.z + boundsExtension.z));
        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x + boundsExtension.x, boundsCenterPos.y - boundsExtension.y, boundsCenterPos.z - boundsExtension.z));

        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x - boundsExtension.x, boundsCenterPos.y + boundsExtension.y, boundsCenterPos.z - boundsExtension.z));
        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x - boundsExtension.x, boundsCenterPos.y + boundsExtension.y, boundsCenterPos.z + boundsExtension.z));
        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x + boundsExtension.x, boundsCenterPos.y + boundsExtension.y, boundsCenterPos.z + boundsExtension.z));
        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x + boundsExtension.x, boundsCenterPos.y + boundsExtension.y, boundsCenterPos.z - boundsExtension.z));

        return m_sceneWorldConnerList;
    }


    /// <summary>
    /// 摄像机正适应场景
    /// </summary>
    /// <param name="lightTransform"></param>
    /// <param name="lightCamara"></param>
    /// <param name="sceneConnerList"></param>
    private void SetLightCameraFitScene(Transform lightTransform, Camera lightCamara, List<Vector3> sceneConnerList)
    {
        AABBBounds boundsInLightSpace = new AABBBounds();

        sceneConnerList.ForEach(worldConnerVertex =>
        {
            Vector3 connerPosInLightSpace = lightTransform.worldToLocalMatrix.MultiplyPoint(worldConnerVertex);
            boundsInLightSpace.UpdatePoint(connerPosInLightSpace);
        });

        //由于lightcamera,位置由x,y确定，z由 camera.near far确定  脑补一下侧视图 还有俯视图
        //x,y的中间就是摄像机位置，z值为near~far 需要保证near~far大于0
        lightCamara.transform.localPosition = new Vector3(boundsInLightSpace.Center.x, boundsInLightSpace.Center.y, boundsInLightSpace.Min.z - 0.05f);
        lightCamara.orthographicSize = Mathf.Max(boundsInLightSpace.Extends.x, boundsInLightSpace.Extends.y);
        lightCamara.nearClipPlane = 0.05f;
        lightCamara.farClipPlane = 0.05f + (boundsInLightSpace.Size.z);

        Matrix4x4 lightCameraProjectionUVMatrix = new Matrix4x4();
        Vector3 transpose = new Vector3(0.5f, 0.5f, 0f);
        Vector3 scale = new Vector3(0.5f, 0.5f, 1f);
        lightCameraProjectionUVMatrix.SetTRS(transpose, Quaternion.identity, scale);

        //  (P*V) *0.5 +0.5
        lightCameraProjectionUVMatrix = lightCameraProjectionUVMatrix * GL.GetGPUProjectionMatrix(lightCamara.projectionMatrix, false) * lightCamara.worldToCameraMatrix;

        Shader.SetGlobalMatrix("_LightSpaceProjectionUVMatrix", lightCameraProjectionUVMatrix);

    }




}
