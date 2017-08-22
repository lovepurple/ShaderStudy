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
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(m_shadowDepthTexture, destination);
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

        this.m_depthGrayShader = Shader.Find("Shader Forge/sf-GrayDepthTexture");
    }

    private void UpdateSceneBounds()
    {
        if (!m_sceneBoundsCollider && m_sceneBoundObj)
            m_sceneBoundsCollider = m_sceneBoundObj.GetComponent<BoxCollider>();

        List<Vector3> sceneWorldConnerList = GetSceneWorldAABB(m_sceneBoundsCollider.bounds, m_sceneBoundObj.transform);
        SetLightCameraFitScene(m_light.transform, m_shadowCamera, sceneWorldConnerList);

    }

    private Vector3 boundsCenterPos;
    private Vector3 boundsExtension;

    /// <summary>
    /// 获取场景
    /// </summary>
    /// <param name="sceneBounds"></param>
    /// <param name="sceneBoundsTransform"></param>
    /// <returns></returns>
    /// <remarks>
    /// 变换AABB的算法需要稍后看一下
    /// </remarks>
    private List<Vector3> GetSceneWorldAABB(Bounds sceneBounds, Transform sceneBoundsTransform)
    {
        Vector3 boundsCenterPos = sceneBoundsTransform.localToWorldMatrix.MultiplyPoint(sceneBounds.center);

        m_sceneWorldConnerList.Clear();
        Vector3 boundsExtension = sceneBounds.extents;

        //下部分
        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x - boundsExtension.x, boundsCenterPos.y - boundsExtension.y, boundsCenterPos.z - boundsExtension.z));
        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x - boundsExtension.x, boundsCenterPos.y - boundsExtension.y, boundsCenterPos.z + boundsExtension.z));
        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x + boundsExtension.x, boundsCenterPos.y - boundsExtension.y, boundsCenterPos.z - boundsExtension.z));
        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x + boundsExtension.x, boundsCenterPos.y - boundsExtension.y, boundsCenterPos.z + boundsExtension.z));

        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x - boundsExtension.x, boundsCenterPos.y + boundsExtension.y, boundsCenterPos.z - boundsExtension.z));
        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x - boundsExtension.x, boundsCenterPos.y + boundsExtension.y, boundsCenterPos.z + boundsExtension.z));
        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x + boundsExtension.x, boundsCenterPos.y + boundsExtension.y, boundsCenterPos.z - boundsExtension.z));
        m_sceneWorldConnerList.Add(new Vector3(boundsCenterPos.x + boundsExtension.x, boundsCenterPos.y + boundsExtension.y, boundsCenterPos.z + boundsExtension.z));

        return m_sceneWorldConnerList;
    }

    private Vector3 debugCubeCenter;
    private Vector3 debugCubeSize;

    private void OnDrawGizmos()
    {
        if (debugCubeCenter != default(Vector3) && debugCubeSize != default(Vector3))
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(debugCubeCenter, debugCubeSize);
        }
    }



    private void SetLightCameraFitScene(Transform lightTransform, Camera lightCamara, List<Vector3> sceneConnerList)
    {
        //scene conner 相对于light的坐标()
        List<Vector3> sceneConnersInLightSpace = new List<Vector3>(sceneConnerList.Count);


        float xMin = 0f;
        float xMax = 0f;
        float yMin = 0f;
        float yMax = 0f;
        float zMin = 0f;
        float zMax = 0f;
        sceneConnerList.ForEach(worldConner =>
        {
            Vector3 connerPosInLightSpace = lightTransform.worldToLocalMatrix.MultiplyPoint(worldConner);
            xMin = Mathf.Min(xMin, connerPosInLightSpace.x);
            xMax = Mathf.Max(xMax, connerPosInLightSpace.x);

            yMin = Mathf.Min(yMin, connerPosInLightSpace.y);
            yMax = Mathf.Max(yMax, connerPosInLightSpace.y);

            zMin = Mathf.Min(zMin, connerPosInLightSpace.z);
            zMax = Mathf.Max(zMax, connerPosInLightSpace.z);

            sceneConnersInLightSpace.Add(connerPosInLightSpace);
        });

        //由于lightcamera,位置由x,y确定，z由 camera.near far确定  脑补一下侧视图 还有俯视图
        //x,y的中间就是摄像机位置，z值为near~far 需要保证near~far大于0
        //lightCamara.transform.localPosition = new Vector3((xMax + xMin) / 2, (yMax + yMin) / 2, zMin - 0.05f);
        //lightCamara.orthographicSize = Mathf.Max((xMax - xMin) / 2, (yMax - yMin) / 2);
        //lightCamara.orthographic = true;
        //lightCamara.nearClipPlane = 0.05f;
        //lightCamara.farClipPlane = 0.05f + (zMax - zMin);

        GameObject markCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        markCube.transform.parent = lightTransform;
        markCube.transform.localRotation = Quaternion.identity;
        markCube.transform.localPosition = new Vector3((xMin + xMax) / 2, (yMin + yMax) / 2, (zMin + zMax) / 2);
        markCube.transform.localScale = new Vector3((xMax - xMin) / 2, (yMax - yMin) / 2, (zMax - zMin) / 2);

    }




}
