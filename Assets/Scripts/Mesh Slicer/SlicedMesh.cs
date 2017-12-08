using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 切割后的模型
/// </summary>
public class SlicedMesh
{
    private List<GeometryUtility.Triangle> m_upperMeshTriangleList = new List<GeometryUtility.Triangle>();
    private List<GeometryUtility.Triangle> m_underMeshTriangleList = new List<GeometryUtility.Triangle>();
    private List<Vector3> m_crossVertexList = new List<Vector3>();

    private GeometryUtility.TriangleWindingOrder m_windingOrder = GeometryUtility.TriangleWindingOrder.CounterClockWise;

    //模型相关变换(也可以在创建出Mesh后，对Mesh的GameObject 进行变换)
    private Vector3 m_meshScale = Vector3.one;
    private Vector3 m_meshRotationEuler = Vector3.zero;
    private Vector3 m_meshCenterPosition = Vector3.zero;

    private Matrix4x4 m_meshTransposeMatrix = Matrix4x4.identity;

    private Mesh m_upperMesh = null;
    private Mesh m_underMesh = null;
    private Mesh m_upperCrossMesh = null;
    private Mesh m_underCrossMesh = null;

    public SlicedMesh(GeometryUtility.TriangleWindingOrder meshWindingOrder)
    {
        this.m_windingOrder = meshWindingOrder;
    }

    public SlicedMesh(GeometryUtility.TriangleWindingOrder meshWindingOrder, Vector3 meshPosition, Vector3 meshScale, Vector3 meshRotationEuler) : this(meshWindingOrder)
    {
        this.m_meshScale = meshScale;
        this.m_meshRotationEuler = meshRotationEuler;
        this.m_meshCenterPosition = meshPosition;
        this.m_meshTransposeMatrix = Matrix4x4.TRS(this.m_meshCenterPosition, Quaternion.Euler(this.m_meshRotationEuler), m_meshScale);
    }

    public List<GeometryUtility.Triangle> UpperMeshTriangleList
    {
        get { return this.m_upperMeshTriangleList; }
    }
    public List<GeometryUtility.Triangle> UnderMeshTriangleList
    {
        get { return this.m_underMeshTriangleList; }
    }

    public List<Vector3> CrossMeshVertexList
    {
        get { return this.m_crossVertexList; }
    }

    public Mesh UpperMesh
    {
        get
        {
            if (m_upperMeshTriangleList.Count == 0)
                return null;

            if (m_upperMesh == null)
                m_upperMesh = GeometryUtility.CreateUnityMeshByTriangleList(m_upperMeshTriangleList, m_meshTransposeMatrix, this.m_windingOrder);

            return m_upperMesh;
        }
    }
    public Mesh UnderMesh
    {
        get
        {
            if (m_underMeshTriangleList.Count == 0)
                return null;

            if (m_underMesh == null)
                m_underMesh = GeometryUtility.CreateUnityMeshByTriangleList(m_underMeshTriangleList, m_meshTransposeMatrix, this.m_windingOrder);

            return m_underMesh;
        }
    }

    public Mesh GetUpperCrossMesh(Vector3 crossPlaneNormal)
    {
        if (m_crossVertexList.Count == 0)
            return null;

        if (m_upperCrossMesh == null)
            m_upperCrossMesh = GeometryUtility.CreateConvexhullMeshByMonotoneChain(m_crossVertexList, -crossPlaneNormal, GeometryUtility.TriangleWindingOrder.ClockWise).ApplyTransposeMatrix(m_meshTransposeMatrix);

        return m_upperCrossMesh;
    }

    public Mesh GetUnderCrossMesh(Vector3 crossPlaneNormal)
    {
        if (m_crossVertexList.Count == 0)
            return null;

        if (m_underCrossMesh == null)
            m_underCrossMesh = GeometryUtility.CreateConvexhullMeshByMonotoneChain(m_crossVertexList, crossPlaneNormal, GeometryUtility.TriangleWindingOrder.ClockWise).ApplyTransposeMatrix(m_meshTransposeMatrix);

        return m_underCrossMesh;
    }
}
