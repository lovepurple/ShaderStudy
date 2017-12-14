using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 几何Debug帮助类
/// </summary>
public class GeometryDebugHelper : MonoSingleton<GeometryDebugHelper>
{
    private List<GizmosMesh> m_gizmosMeshList = new List<GizmosMesh>();

    private readonly Color defaultColor = new Color(0.7f, 0, 0.7f, 0.5f);

    private event Action<string> GizmosDelegate = null;
    private string CustomGizmosParam = string.Empty;


    /// <summary>
    /// Debug Plane
    /// </summary>
    /// <param name="plane"></param>
    /// <param name="drawColor"></param>
    /// <param name="planeSize"></param>
    /// <param name="clearExist"></param>
    public void DrawPlane(Plane plane, Color drawColor = default(Color), Vector2 planeSize = default(Vector2), bool clearExist = true)
    {
        if (planeSize == default(Vector2))
            planeSize = new Vector2(10f, 10f);

        drawColor = drawColor == default(Color) ? defaultColor : drawColor;

        Quaternion planeRotation = Quaternion.FromToRotation(Vector3.up, plane.normal);
        Vector3 planeScale = new Vector3(planeSize.x, 0, planeSize.y);
        Vector3 planeCenterPosition = plane.normal * -plane.distance;       //plane.distance 的方向
        Matrix4x4 planeMatrix = Matrix4x4.TRS(planeCenterPosition, planeRotation, planeScale);

        GeometryUtility.Triangle triangle0 = new GeometryUtility.Triangle(new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, 0.5f), new Vector3(-0.5f, 0, -0.5f));
        triangle0.ApplyMatrix(planeMatrix);
        GeometryUtility.Triangle triangle1 = new GeometryUtility.Triangle(new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 0, -0.5f), new Vector3(-0.5f, 0, -0.5f));
        triangle1.ApplyMatrix(planeMatrix);

        GeometryUtility.Triangle backTriangle0 = triangle0.Clone() as GeometryUtility.Triangle;
        backTriangle0.WindingOrder = GeometryUtility.TriangleWindingOrder.ClockWise;
        GeometryUtility.Triangle backTriangle1 = triangle1.Clone() as GeometryUtility.Triangle;
        backTriangle1.WindingOrder = GeometryUtility.TriangleWindingOrder.ClockWise;


        Mesh mesh = GeometryUtility.CreateUnityMeshByTriangleList(new List<GeometryUtility.Triangle>() { triangle0, triangle1 });
        Mesh meshBackFace = GeometryUtility.CreateUnityMeshByTriangleList(new List<GeometryUtility.Triangle>() { backTriangle0, backTriangle1 });

        if (clearExist)
            m_gizmosMeshList.Clear();

        m_gizmosMeshList.Add(new GizmosMesh(mesh, defaultColor));
        m_gizmosMeshList.Add(new GizmosMesh(meshBackFace, new Color(0f, 1f, 0f, 0.5f)));

    }

    /// <summary>
    /// Debug Triangle
    /// </summary>
    /// <param name="triangleList"></param>
    /// <param name="color"></param>
    /// <param name="clearExist"></param>
    public void DrawTriangles(List<GeometryUtility.Triangle> triangleList, Color color, bool clearExist = true)
    {
        Mesh mesh = GeometryUtility.CreateUnityMeshByTriangleList(triangleList);
        if (clearExist)
            m_gizmosMeshList.Clear();

        m_gizmosMeshList.Add(new GizmosMesh(mesh, color));

    }

    /// <summary>
    /// Debug Point
    /// </summary>
    /// <param name="drawParams"> position(vector3):color(r,g,b,a):radius </param>
    public void DrawPoint(string drawParams)
    {
        CustomGizmosParam = drawParams;
        GizmosDelegate = GizmosDrawPointInternal;
    }

    private void GizmosDrawPointInternal(string drawParams)
    {
        string[] param = drawParams.Split(':');

        Vector3 pointPosition = Vector3.zero;
        Color drawColor = new Color(0f, 1, 0, 0.8f);
        float pointRadius = 0.05f;
        if (param.Length > 0)
        {
            bool formatCorrect = Utils.Vector3FromString(param[0], out pointPosition);
            if (!formatCorrect)
                return;
        }

        if (param.Length > 1)
        {
            bool formatCorrent = Utils.ColorFromString(param[1], out drawColor);
            if (formatCorrent)
                return;
        }

        if (param.Length > 2)
            pointRadius = float.Parse(param[2]);

        Gizmos.color = drawColor;
        Gizmos.DrawSphere(pointPosition, pointRadius);
    }


    private void OnDrawGizmos()
    {
        if (this.m_gizmosMeshList.Count > 0)
        {
            for (int i = 0; i < m_gizmosMeshList.Count; ++i)
            {
                Gizmos.color = m_gizmosMeshList[i].drawColor;
                Gizmos.DrawMesh(m_gizmosMeshList[i].drawMesh);
            }
        }

        if (this.GizmosDelegate != null)
            this.GizmosDelegate(this.CustomGizmosParam);

    }

    private struct GizmosMesh
    {
        public Mesh drawMesh;
        public Color drawColor;
        public GizmosMesh(Mesh mesh, Color color)
        {
            drawMesh = mesh;
            drawColor = color;
        }
    }
}
