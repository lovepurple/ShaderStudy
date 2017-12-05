using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// CMS 阴影渲染
/// </summary>
public class CascadeShadowmap : MonoBehaviour
{
    //暂时只考虑一盏灯
    public Light Mainlight = null;

    public ShadowFitMode FitMode = ShadowFitMode.FitScene;
    public BoundsType FrustumBoundsType = BoundsType.AABB;

    public int CascadeStepNum = 1;          //一共分几段


    private Camera m_sceneCamera = null;
    private GameObject m_shadowmapCameraObject = null;
    private Camera m_shadowmapCamera = null;

    private Shader m_shadowmapShader = null;

    private RenderTexture[] m_shadowmaps = null;
    private Matrix4x4[] m_worldToShadowmapUVMatrixs = null;
    private List<List<Vector3>> m_cameraFrustumList = new List<List<Vector3>>();            //摄像机cascaded 后的视锥定点列表 
    private List<CascadedShadowMapCameraParams> m_shadowMapCameraParams = null;

    private Texture2DArray m_shadowmapArray = null;
    private Matrix4x4 ProjectionToUVMatrix = new Matrix4x4();

    void Awake()
    {
        if (CascadeStepNum < 1)
            CascadeStepNum = 1;

        m_shadowmaps = new RenderTexture[CascadeStepNum];
        m_worldToShadowmapUVMatrixs = new Matrix4x4[CascadeStepNum];
        m_shadowMapCameraParams = new List<CascadedShadowMapCameraParams>(CascadeStepNum);

        if (Mainlight != null)
            CreateShadowCamera(Mainlight);

        ProjectionToUVMatrix.SetRow(0, new Vector4(0.5f, 0.0f, 0.0f, 0.5f));
        ProjectionToUVMatrix.SetRow(1, new Vector4(0.0f, 0.5f, 0.0f, 0.5f));
        ProjectionToUVMatrix.SetRow(2, new Vector4(0.0f, 0.0f, 1.0f, 0.0f));
        ProjectionToUVMatrix.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        m_sceneCamera = Camera.main;
    }

    private void OnPreRender()
    {
        for (int i = 0; i < CascadeStepNum; ++i)
        {
            if (m_shadowmaps[i] == null)
                m_shadowmaps[i] = new RenderTexture(m_sceneCamera.pixelWidth, m_sceneCamera.pixelHeight, 24);

            m_shadowmapCamera.targetTexture = m_shadowmaps[i];

            if (m_worldToShadowmapUVMatrixs[i] == null)
                m_worldToShadowmapUVMatrixs[i] = new Matrix4x4();

            CascadedShadowMapCameraParams cascadeInfo = m_shadowMapCameraParams[i];
            if (cascadeInfo == null)
                return;

            m_shadowmapCameraObject.transform.localPosition = cascadeInfo.cameraPosition;
            m_shadowmapCameraObject.transform.localRotation = Quaternion.identity;
            m_shadowmapCamera.nearClipPlane = 0.05f;
            m_shadowmapCamera.farClipPlane = 0.05f + cascadeInfo.cameraZSize;
            m_shadowmapCamera.orthographicSize = cascadeInfo.cameraOrthogonalSize;

            m_worldToShadowmapUVMatrixs[i] = ProjectionToUVMatrix * GL.GetGPUProjectionMatrix(m_shadowmapCamera.projectionMatrix, false) * m_shadowmapCamera.worldToCameraMatrix;

            m_shadowmapCamera.RenderWithShader(m_shadowmapShader, "");
        }

        //把所有的shadowmap传给GPU，所以如果Cascaded太多的话，带宽是个问题
        if (m_shadowmapArray == null)
            m_shadowmapArray = TextureUtility.CreateTextureArray(m_shadowmaps.ToList<RenderTexture>(), m_sceneCamera.pixelWidth, m_sceneCamera.pixelHeight);
        else
            TextureUtility.UpdateTextureArray(m_shadowmapArray, m_shadowmaps.ToList<RenderTexture>());

        Shader.SetGlobalTexture("_ShadowMapArray", m_shadowmapArray);
        Shader.SetGlobalMatrixArray("_WorldToShadowMapUVMatrix", m_worldToShadowmapUVMatrixs);
        Shader.SetGlobalVector("_ShadowMapParams", new Vector4(m_sceneCamera.nearClipPlane, m_sceneCamera.farClipPlane, CascadeStepNum, 0));
    }

    void Update()
    {
        if (m_sceneCamera)
            UpdateCascadedFrustum(CascadeStepNum, FitMode, m_sceneCamera);

        m_shadowMapCameraParams.Clear();
        UpdateShadowmapCameraParams(FrustumBoundsType);

    }

    private void UpdateShadowmapCameraParams(BoundsType boundsType)
    {
        if (boundsType == BoundsType.AABB)
        {
            for (int i = 0; i < CascadeStepNum; ++i)
            {
                List<Vector3> frustumVertex = m_cameraFrustumList[i];
                AABBBounds frustumBoundsOnLightSpace = GeometryUtility.GetAABBBoundsOnTargetSpace(Mainlight.transform, frustumVertex);

                CascadedShadowMapCameraParams cameraParams = new CascadedShadowMapCameraParams();
                cameraParams.cameraPosition = new Vector3(frustumBoundsOnLightSpace.Center.x, frustumBoundsOnLightSpace.Center.y, frustumBoundsOnLightSpace.Min.z - 0.05f);
                cameraParams.cameraOrthogonalSize = Mathf.Max(frustumBoundsOnLightSpace.Extends.x, frustumBoundsOnLightSpace.Extends.y) / 2;           //todo:orthogonalSize算法需要以后好好研究一下
                cameraParams.cameraZSize = frustumBoundsOnLightSpace.Size.z;

                m_shadowMapCameraParams.Add(cameraParams);
            }
        }
    }

    private void CreateShadowCamera(Light light)
    {
        this.m_shadowmapCameraObject = new GameObject("ShadowCamera");
        this.m_shadowmapCameraObject.transform.SetParent(light.transform);
        this.m_shadowmapCameraObject.transform.localScale = Vector3.one;
        this.m_shadowmapCameraObject.transform.localPosition = Vector3.zero;
        this.m_shadowmapCameraObject.transform.localRotation = Quaternion.identity;
        this.m_shadowmapCamera = m_shadowmapCameraObject.AddComponent<Camera>();
        this.m_shadowmapCamera.orthographic = true;
        this.m_shadowmapCamera.enabled = false;

        this.m_shadowmapShader = Shader.Find("Hidden/SceneDepth");
    }


    /// <summary>
    /// 更新Cascaded的锥形空间
    /// </summary>
    /// <param name="cascadeNum"></param>
    /// <param name="fitMode"></param>
    /// <param name="camera"></param>
    private void UpdateCascadedFrustum(int cascadeNum, ShadowFitMode fitMode, Camera camera)
    {
        this.m_cameraFrustumList.Clear();
        float distancePerCascade = (camera.farClipPlane - camera.nearClipPlane) / cascadeNum;
        for (int i = 0; i < cascadeNum; ++i)
        {
            float zNear = camera.nearClipPlane;
            float zFar = camera.farClipPlane;
            if (fitMode == ShadowFitMode.FitCascaded)
            {
                zNear = i * distancePerCascade;
                zFar = zNear + distancePerCascade;
            }
            else
                zFar = zNear + (i + 1) * distancePerCascade;

            List<Vector3> frustumVertexList = GeometryUtility.GetCameraFrustumWorldConnerVerticesByZ(camera, zNear, zFar);

            this.m_cameraFrustumList.Add(frustumVertexList);
        }
    }



    private class CascadedShadowMapCameraParams
    {
        public Vector3 cameraPosition;
        public float cameraOrthogonalSize;
        public float cameraZSize;
    }

}

/// <summary>
/// Cascaded 精度类型
/// </summary>
public enum ShadowFitMode
{
    FitScene,
    FitCascaded
}

/// <summary>
/// Bounds包围盒类型
/// </summary>
public enum BoundsType
{
    AABB,
    Sphere
}