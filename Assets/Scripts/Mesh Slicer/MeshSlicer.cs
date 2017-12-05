using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSlicer
{
    //原始模型列表
    private List<Mesh> m_originMeshList = new List<Mesh>();

    //切割刀片
    private Plane m_slicerPlane = default(Plane);

    //切割后的模型列表
    private List<Mesh> m_slicedMeshList = new List<Mesh>();

    public MeshSlicer(Mesh originMesh, Plane slicer)
    {
        m_originMeshList.Add(originMesh);
        this.m_slicerPlane = slicer;
    }

    public MeshSlicer(Mesh originMesh, Vector3 slicerNormal, float distanceFromOrigin)
    {
        m_originMeshList.Add(originMesh);
        this.m_slicerPlane = new Plane(slicerNormal, distanceFromOrigin);
    }

    public MeshSlicer(List<Mesh> originMeshList, Plane slicer)
    {
        this.m_originMeshList = originMeshList;
        this.m_slicerPlane = slicer;
    }


    /// <summary>
    /// 切割模型
    /// </summary>
    /// <param name="includeSection"></param>
    /// <param name="includeOppositeFace"></param>
    /// <param name="remainOrigin">是否保留原始模型</param>
    /// <returns></returns>
    public List<Mesh> Slice(bool includeSection, bool includeOppositeFace, bool remainOrigin = true)
    {
        List<Mesh> meshAfterSlice = new List<Mesh>();
        for (int i = 0; i < m_originMeshList.Count; ++i)
        {
            Mesh sliceMesh = m_originMeshList[i];

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

                if (sliceMesh.uv.Length != 0)
                {
                    uv0 = sliceMesh.uv[vertexIndex0];
                    uv1 = sliceMesh.uv[vertexIndex1];
                    uv2 = sliceMesh.uv[vertexIndex2];
                }

                GeometryUtility.Triangle triangle = new GeometryUtility.Triangle(vertexPosition0, vertexPosition1, vertexPosition2, uv0, uv1, uv2);
                GeometryUtility.PlaneTriangleIntersectionResult intersectionResult = GeometryUtility.GetPlaneTriangleIntersectionResult(this.m_slicerPlane, triangle, includeOppositeFace);

                slicedMesh.UpperMeshTriangleList.AddRange(intersectionResult.UpperTriangleList);
                slicedMesh.UnderMeshTriangleList.AddRange(intersectionResult.UnderTriangleList);
                slicedMesh.CrossMeshVertexList.AddRange(intersectionResult.IntersectionPointList);
            }

            meshAfterSlice.Add(slicedMesh.UpperMesh);

            if (includeOppositeFace)
                meshAfterSlice.Add(slicedMesh.UndererMesh);

            //交界处的mesh
            if (includeSection)
            {
                //上交界处的法线是切面的反方向
                Mesh upperSection = GeometryUtility.CreateConvexhullMeshByMonotoneChain(slicedMesh.CrossMeshVertexList, -m_slicerPlane.normal, GeometryUtility.TriangleWindingOrder.ClockWise);
                meshAfterSlice.Add(upperSection);

                if (includeOppositeFace)
                {
                    Mesh underSection = GeometryUtility.CreateConvexhullMeshByMonotoneChain(slicedMesh.CrossMeshVertexList, m_slicerPlane.normal, GeometryUtility.TriangleWindingOrder.ClockWise);
                    meshAfterSlice.Add(underSection);
                }
            }

            if (!remainOrigin)
                Object.Destroy(sliceMesh);
        }
        return meshAfterSlice;
    }






}
