using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RenderShadowMap : MonoBehaviour
{
    public Light m_light = null;
    public GameObject m_sceneBoundObj = null;
    public LightFitMode FitMode = LightFitMode.FIT_SCENE;

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

    void Update()
    {
        UpdateSceneBounds();

        if (this.FitMode == LightFitMode.FIT_SCENE)
            SetLightCameraFitScene(m_light.transform, m_shadowCamera, m_sceneWorldConnerList);
        else
            SetLightCameraFitView(m_light.transform, m_shadowCamera, Camera.main, m_sceneWorldConnerList);


    }

    private void OnPreRender()
    {
        m_shadowCamera.RenderWithShader(m_depthGrayShader, "");
        Shader.SetGlobalTexture("_ShadowMap", m_shadowDepthTexture);
        Shader.SetGlobalVector("_PixelSize", new Vector2(1.0f / Camera.main.pixelWidth, 1.0f / Camera.main.pixelHeight));
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

        this.m_shadowDepthTexture = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 24);         //必须要有深度
        this.m_shadowCamera.targetTexture = m_shadowDepthTexture;
        this.m_shadowCamera.enabled = false;
        this.m_shadowCamera.orthographic = true;

        this.m_depthGrayShader = Shader.Find("Hidden/SceneDepth");
    }

    private void UpdateSceneBounds()
    {
        if (!m_sceneBoundsCollider && m_sceneBoundObj)
            m_sceneBoundsCollider = m_sceneBoundObj.GetComponent<BoxCollider>();

        UpdateSceneWorldAABB(m_sceneBoundsCollider.bounds, m_sceneBoundObj.transform);
    }



    /// <summary>
    /// 获取场景AABB包围盒的8个顶点
    /// </summary>
    /// <param name="sceneBounds"></param>
    /// <param name="sceneBoundsTransform"></param>
    /// <returns></returns>
    private List<Vector3> UpdateSceneWorldAABB(Bounds sceneBounds, Transform sceneBoundsTransform)
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

    private AABBBounds GetSceneAABBBoundsInTargetSpace(List<Vector3> worldConnerList, Transform targetSpace)
    {
        AABBBounds boundsInLightSpace = new AABBBounds();
        worldConnerList.ForEach(worldConnerVertex =>
        {
            Vector3 connerPosInLightSpace = targetSpace.worldToLocalMatrix.MultiplyPoint(worldConnerVertex);
            boundsInLightSpace.UpdatePoint(connerPosInLightSpace);
        });

        return boundsInLightSpace;
    }


    /// <summary>
    /// 摄像机正适应场景
    /// </summary>
    /// <param name="lightTransform"></param>
    /// <param name="lightCamara"></param>
    /// <param name="sceneConnerList"></param>
    private void SetLightCameraFitScene(Transform lightTransform, Camera lightCamara, List<Vector3> sceneConnerList)
    {
        AABBBounds boundsInLightSpace = GetSceneAABBBoundsInTargetSpace(sceneConnerList, lightTransform);

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

    private GameObject testCube = null;

    /// <summary>
    /// 设置LightCamera 适应视角
    /// </summary>
    /// <param name="lightTransform"></param>
    /// <param name="lightCamera"></param>
    /// <param name="mainCamera"></param>
    /// <param name="sceneConnerList"></param>
    private void SetLightCameraFitView(Transform lightTransform, Camera lightCamera, Camera mainCamera, List<Vector3> sceneConnerList)
    {

        List<Vector3> mainCameraFrustumConners = GetCameraPerspectiveFrustumConners(mainCamera);
        AABBBounds frustumBoundsInLightSpace = new AABBBounds();

        mainCameraFrustumConners.ForEach(connerVertex =>
        {
            Vector3 worldFrustumConner = mainCamera.transform.localToWorldMatrix.MultiplyPoint(connerVertex);

            Vector3 pointInLightSpace = lightTransform.worldToLocalMatrix.MultiplyPoint(worldFrustumConner);

            frustumBoundsInLightSpace.UpdatePoint(pointInLightSpace);
        });


        AABBBounds sceneBounds = GetSceneAABBBoundsInTargetSpace(sceneConnerList, lightTransform);

        //计算Frustum 时，需要注意，如果Camera.farpanel 特别大，计算出的AABB太大， 所以可以采取AABBFrustum 跟 SceneAABB 相交作为最终的投影包围体
        //或者设置farpanel neaarpanel到合理的大小

        //lightCamera.transform.localPosition = new Vector3(frustumBoundsInLightSpace.Center.x, frustumBoundsInLightSpace.Center.y, frustumBoundsInLightSpace.Min.z - 0.05f);
        //lightCamera.orthographicSize = Mathf.Max(frustumBoundsInLightSpace.Extends.x, frustumBoundsInLightSpace.Extends.y);
        //lightCamera.nearClipPlane = 0.05f;
        //lightCamera.farClipPlane = 0.05f + (frustumBoundsInLightSpace.Size.z);

        Vector3 shadowCameraMin = new Vector3(
            Mathf.Max(sceneBounds.Min.x, frustumBoundsInLightSpace.Min.x),
            Mathf.Max(sceneBounds.Min.y, frustumBoundsInLightSpace.Min.y),
            Mathf.Max(sceneBounds.Min.z, frustumBoundsInLightSpace.Min.z));

        Vector3 shadowCameraMax = new Vector3(
            Mathf.Min(sceneBounds.Max.x, frustumBoundsInLightSpace.Max.x),
            Mathf.Min(sceneBounds.Max.y, frustumBoundsInLightSpace.Max.y),
            Mathf.Min(sceneBounds.Max.z, frustumBoundsInLightSpace.Max.z));

        Vector3 center = (shadowCameraMax + shadowCameraMin) / 2;
        Vector3 extends = (shadowCameraMax - shadowCameraMin) / 2;

        lightCamera.transform.localPosition = new Vector3(center.x, center.y, shadowCameraMin.z - 0.05f);
        lightCamera.orthographicSize = Mathf.Max(extends.x, extends.y);
        lightCamera.nearClipPlane = 0.05f;
        lightCamera.farClipPlane = 0.05f + (extends.z * 2);


        Matrix4x4 lightCameraProjectionUVMatrix = new Matrix4x4();
        Vector3 transpose = new Vector3(0.5f, 0.5f, 0);
        Vector3 scale = new Vector3(0.5f, 0.5f, 1);

        lightCameraProjectionUVMatrix.SetTRS(transpose, Quaternion.identity, scale);
        lightCameraProjectionUVMatrix = lightCameraProjectionUVMatrix * GL.GetGPUProjectionMatrix(lightCamera.projectionMatrix, false) * lightCamera.worldToCameraMatrix;

        Shader.SetGlobalMatrix("_LightSpaceProjectionUVMatrix", lightCameraProjectionUVMatrix);
    }


    /// <summary>
    /// 获取透视摄像机视锥的顶点坐标
    /// </summary>
    /// <param name="camera"></param>
    /// <returns></returns>
    /// <remarks>
    /// 根据Fov nearPanel,farPanel,计算视锥面
    /// </remarks>
    private List<Vector3> GetCameraPerspectiveFrustumConners(Camera camera)
    {
        List<Vector3> connerVertexList = new List<Vector3>();

        float farZ = camera.farClipPlane;
        float nearZ = camera.nearClipPlane;

        //Fov 是在Y轴上的夹角
        float tanHalfFov = Mathf.Tan(camera.fieldOfView / 2 * Mathf.Deg2Rad);

        float nearY = nearZ * tanHalfFov;

        //camera.aspect = x / y (width/height)          
        float nearX = nearY * camera.aspect;

        //使用的是camera 所在的local->world 不是camera->world
        GeometryUtility.AddVertexByExtensionList(nearX, nearY, nearZ, ref connerVertexList);

        float farY = farZ * tanHalfFov;
        float farX = farY * camera.aspect;
        GeometryUtility.AddVertexByExtensionList(farX, farY, farZ, ref connerVertexList);

        return connerVertexList;
    }




}

public enum LightFitMode
{
    FIT_SCENE,
    FIT_VIEW,
}