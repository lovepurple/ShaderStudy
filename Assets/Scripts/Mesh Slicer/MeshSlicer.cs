﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 模型切割类
/// MeshSlicer输入的原始Mesh 是进行transform TRS变换之后的模型，需要在传入的时候把原始的Mesh 进行坐标变换,
/// 否则如果原始Mesh所在的Transform 的position,rotation,scale有自定义数据会导致切割结果错误
/// 在切割后的模型的尺寸是TRS之后的，如果需要变换回原始的尺寸需要把Mesh * TRS().inverse
/// </summary>
public class MeshSlicer
{
    //原始模型列表
    private Mesh m_originMesh = null;

    //切割刀片
    private Plane m_slicerPlane = default(Plane);

    //切割后的模型
    private Mesh m_slicedUpperMesh = null;
    private Mesh m_slicedUnderMesh = null;
    private Mesh m_slicedUpperCrossMesh = null;
    private Mesh m_slicedUnderCrossMesh = null;

    private GameObject m_slicedUpperGameObject = null;
    private GameObject m_slicedUnderGameObject = null;

    private bool m_includeSection = true;
    private bool m_includeOppositeFace = false;

    public MeshSlicer(Mesh originMesh, Plane slicer)
    {
        this.m_originMesh = originMesh;
        this.m_slicerPlane = slicer;
    }

    public MeshSlicer(Mesh originMesh, Vector3 slicerNormal, float distanceFromOrigin)
    {
        this.m_originMesh = originMesh;
        this.m_slicerPlane = new Plane(slicerNormal, distanceFromOrigin);
    }



    /// <summary>
    /// 切割模型
    /// </summary>
    /// <param name="includeSection"></param>
    /// <param name="includeOppositeFace"></param>
    /// <param name="remainOrigin">是否保留原始模型</param>
    /// <returns></returns>
    public SlicedMesh Slice(bool includeSection, bool includeOppositeFace)
    {
        this.m_includeSection = includeSection;
        this.m_includeOppositeFace = includeOppositeFace;

        Mesh sliceMesh = m_originMesh;

        GeometryUtility.TriangleWindingOrder meshWindingOrder = GeometryUtility.GetMeshWindingOrder(sliceMesh);
        SlicedMesh slicedMesh = new SlicedMesh(meshWindingOrder);

        for (int j = 0; j < sliceMesh.triangles.Length; j += 3)
        {
            int vertexIndex0 = sliceMesh.triangles[j];
            int vertexIndex1 = sliceMesh.triangles[j + 1];
            int vertexIndex2 = sliceMesh.triangles[j + 2];

            Vector3 vertexPosition0 = sliceMesh.vertices[vertexIndex0];
            Vector3 vertexPosition1 = sliceMesh.vertices[vertexIndex1];
            Vector3 vertexPosition2 = sliceMesh.vertices[vertexIndex2];

            Vector2 uv0 = Vector2.zero;
            Vector2 uv1 = Vector2.zero;
            Vector2 uv2 = Vector2.zero;

            Vector3 vertexNormal0 = sliceMesh.normals[vertexIndex0];
            Vector3 vertexNormal1 = sliceMesh.normals[vertexIndex1];
            Vector3 vertexNormal2 = sliceMesh.normals[vertexIndex2];

            if (sliceMesh.uv.Length != 0)
            {
                uv0 = sliceMesh.uv[vertexIndex0];
                uv1 = sliceMesh.uv[vertexIndex1];
                uv2 = sliceMesh.uv[vertexIndex2];
            }

            GeometryUtility.Triangle triangle = new GeometryUtility.Triangle(vertexPosition0, vertexPosition1, vertexPosition2, uv0, uv1, uv2, vertexNormal0, vertexNormal1, vertexNormal2);
            GeometryUtility.PlaneTriangleIntersectionResult intersectionResult = GeometryUtility.GetPlaneTriangleIntersectionResult(this.m_slicerPlane, triangle, includeOppositeFace);

            slicedMesh.UpperMeshTriangleList.AddRange(intersectionResult.UpperTriangleList);
            slicedMesh.UnderMeshTriangleList.AddRange(intersectionResult.UnderTriangleList);
            slicedMesh.CrossMeshVertexList.AddRange(intersectionResult.IntersectionPointList);
        }

        this.m_slicedUpperMesh = slicedMesh.UpperMesh;

        if (includeOppositeFace)
            this.m_slicedUnderMesh = slicedMesh.UnderMesh;

        //交界处的mesh
        if (includeSection)
        {
            //上交界处的法线是切面的反方向
            this.m_slicedUpperCrossMesh = slicedMesh.GetUpperCrossMesh(m_slicerPlane.normal);

            if (includeOppositeFace)
                this.m_slicedUnderCrossMesh = slicedMesh.GetUnderCrossMesh(m_slicerPlane.normal);
        }

        return slicedMesh;
    }


    /// <summary>
    /// 提供方便方法直接切割TriangleList
    /// </summary>
    /// <param name="triangleList"></param>
    /// <param name="includeSection"></param>
    /// <param name="includeOppositeFace"></param>
    /// <returns></returns>
    public static SlicedMesh SliceTriangleList(List<GeometryUtility.Triangle> triangleList, Plane slicerPlane, bool includeSection, bool includeOppositeFace)
    {
        if (triangleList.Count < 1)
            return null;

        GeometryUtility.TriangleWindingOrder windingOrder = triangleList[0].WindingOrder;
        SlicedMesh slicedMesh = new SlicedMesh(windingOrder);

        for (int i = 0; i < triangleList.Count; ++i)
        {
            GeometryUtility.Triangle sliceTriangle = triangleList[i];
            GeometryUtility.PlaneTriangleIntersectionResult planeTriangleIntersectionResult = GeometryUtility.GetPlaneTriangleIntersectionResult(slicerPlane, sliceTriangle, includeOppositeFace);

            if (!planeTriangleIntersectionResult.IsIntersection)
                continue;

            slicedMesh.UpperMeshTriangleList.AddRange(planeTriangleIntersectionResult.UpperTriangleList);
            if (includeSection)
                slicedMesh.CrossMeshVertexList.AddRange(planeTriangleIntersectionResult.IntersectionPointList);
        }

        return slicedMesh;
    }


    /// <summary>
    /// 渲染切割后的模型
    /// </summary>
    /// <param name="upperHullMaterial"></param>
    /// <param name="underHullMaterial"></param>
    /// <param name="upperCrossMaterial"></param>
    /// <param name="underCrossMaterial"></param>
    public void RenderSlicedGameObject(Material upperHullMaterial, Material underHullMaterial = null, Material upperCrossMaterial = null, Material underCrossMaterial = null, string originMeshGameObjectName = "")
    {
        if (underHullMaterial == null)
            underHullMaterial = upperHullMaterial;

        if (upperCrossMaterial == null)
            upperCrossMaterial = upperHullMaterial;

        if (underCrossMaterial == null)
            underCrossMaterial = upperHullMaterial;

        this.m_slicedUpperGameObject = new GameObject(originMeshGameObjectName + "_Upper");
        RenderMesh(this.m_slicedUpperMesh, upperHullMaterial, this.m_slicedUpperGameObject);

        if (this.m_includeSection)
        {
            GameObject upperSection = new GameObject(originMeshGameObjectName + "_UpperSection");
            upperSection.transform.SetParent(this.m_slicedUpperGameObject.transform);
            upperSection.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            RenderMesh(m_slicedUpperCrossMesh, upperCrossMaterial, upperSection);
        }

        if (this.m_includeOppositeFace)
        {
            this.m_slicedUnderGameObject = new GameObject(originMeshGameObjectName + "_Under");
            RenderMesh(this.m_slicedUnderMesh, underHullMaterial, this.m_slicedUnderGameObject);
            if (this.m_includeSection)
            {
                GameObject underSection = new GameObject(originMeshGameObjectName + "_UnderSection");
                underSection.transform.SetParent(this.m_slicedUnderGameObject.transform);
                underSection.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                RenderMesh(m_slicedUnderCrossMesh, underCrossMaterial, underSection);
            }
        }
    }


    /// <summary>
    /// 渲染切割后的模型
    /// </summary>
    /// <param name="targetMesh"></param>
    /// <param name="renderMaterial"></param>
    /// <param name="gameObject"></param>
    private void RenderMesh(Mesh targetMesh, Material renderMaterial, GameObject gameObject)
    {
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = renderMaterial;

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = targetMesh;
    }

    public GameObject SlicedUpperHullGameObject
    {
        get
        {
            return m_slicedUpperGameObject;
        }
    }

    public GameObject SlicedUnderHullGameObject
    {
        get
        {
            return m_slicedUnderGameObject;
        }
    }

    public Mesh UpperMesh
    {
        get { return m_slicedUpperMesh; }
    }
}
