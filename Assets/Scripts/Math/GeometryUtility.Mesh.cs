using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 模型相关计算的工具类
/// </summary>
public static partial class GeometryUtility
{

    /// <summary>
    /// 获取三角面的绕向
    /// </summary>
    /// <param name="triangleVertex0"></param>
    /// <param name="triangleVertex1"></param>
    /// <param name="triangleVertex2"></param>
    /// <returns></returns>
    public static TriangleWindingOrder GetTriangleWindingOrder(Vector3 triangleVertex0, Vector3 triangleVertex1, Vector3 triangleVertex2)
    {
        Vector3 v1CrossV2 = Vector3.Cross(triangleVertex1, triangleVertex2);
        return Vector3.Dot(triangleVertex0, v1CrossV2) >= 0 ? TriangleWindingOrder.CounterClockWise : TriangleWindingOrder.ClockWise;
    }

    /// <summary>
    /// 获取面与三角形相交结果
    /// </summary>
    /// <param name="plane">平面</param>
    /// <param name="triangle">三角形</param>
    /// <param name="includePlaneBack">是否包括Plane的反面</param>
    /// <returns></returns>
    public static PlaneTriangleIntersectionResult GetPlaneTriangleIntersectionResult(Plane plane, Triangle triangle, bool includePlaneBack = false)
    {
        PlaneTriangleIntersectionResult intersectionResult = new PlaneTriangleIntersectionResult();

        Vector3 p0 = triangle.VertexPosition0;
        Vector3 p1 = triangle.VertexPosition1;
        Vector3 p2 = triangle.VertexPosition2;

        SideOfPlane p0SideOfPlane = plane.PointSideOfPlane(p0);
        SideOfPlane p1SideOfPlane = plane.PointSideOfPlane(p1);
        SideOfPlane p2SideOfPlane = plane.PointSideOfPlane(p2);

        //三个点在同一方向
        if (p0SideOfPlane == p1SideOfPlane && p0SideOfPlane == p2SideOfPlane)
        {
            if (p0SideOfPlane == SideOfPlane.UP)
                intersectionResult.UpperTriangleList.Add(triangle);

            if (includePlaneBack && p0SideOfPlane == SideOfPlane.DOWN)
                intersectionResult.UnderTriangleList.Add(triangle);

            return intersectionResult;
        }
        //两个点在Plane上
        if ((p0SideOfPlane == SideOfPlane.ON) && p0SideOfPlane == p1SideOfPlane)
        {
            if (p2SideOfPlane == SideOfPlane.UP)
                intersectionResult.UpperTriangleList.Add(triangle);
            else
            {
                if (includePlaneBack)
                    intersectionResult.UnderTriangleList.Add(triangle);
            }
        }
        else if ((p0SideOfPlane == SideOfPlane.ON) && p0SideOfPlane == p2SideOfPlane)
        {
            if (p1SideOfPlane == SideOfPlane.UP)
                intersectionResult.UpperTriangleList.Add(triangle);
            else
            {
                if (includePlaneBack)
                    intersectionResult.UnderTriangleList.Add(triangle);
            }
        }
        else if ((p1SideOfPlane == SideOfPlane.ON) && p1SideOfPlane == p2SideOfPlane)
        {
            if (p0SideOfPlane == SideOfPlane.UP)
                intersectionResult.UpperTriangleList.Add(triangle);
            else
            {
                if (includePlaneBack)
                    intersectionResult.UnderTriangleList.Add(triangle);
            }
        }

        //相交点
        Vector3 intersectionPoint0;
        Vector3 intersectionPoint1;

        //一个点在面上，两个点在两侧
        if (p0SideOfPlane == SideOfPlane.ON)
        {
            //b - c
            if (plane.GetSegmentPlaneIntersectionPoint(p1, p2, out intersectionPoint0))
            {
                //交点的barycentric
                Vector3 barycentricWeight = PointBarycentricInTriangle(intersectionPoint0, p0, p1, p2);
                Vector2 intersectionPointUV = GetTrianglePointByBarycentricWeight2D(barycentricWeight, triangle.UV0, triangle.UV1, triangle.UV2);

                Triangle triangle1 = new Triangle(p0, p1, intersectionPoint0, triangle.UV0, triangle.UV1, intersectionPointUV);
                Triangle triangle2 = new Triangle(p0, intersectionPoint0, p2, triangle.UV0, intersectionPointUV, triangle.UV1);

                intersectionResult.IntersectionPointList.Add(p0);
                intersectionResult.IntersectionPointList.Add(intersectionPoint0);

                if (p1SideOfPlane == SideOfPlane.UP)
                {
                    intersectionResult.UpperTriangleList.Add(triangle1);

                    if (includePlaneBack)
                        intersectionResult.UnderTriangleList.Add(triangle2);
                }
                else
                {
                    intersectionResult.UpperTriangleList.Add(triangle2);

                    if (includePlaneBack)
                        intersectionResult.UnderTriangleList.Add(triangle1);
                }
            }
        }
        else if (p1SideOfPlane == SideOfPlane.ON)
        {
            //a - c
            if (plane.GetSegmentPlaneIntersectionPoint(p0, p2, out intersectionPoint0))
            {
                Vector3 barycentricWeight = PointBarycentricInTriangle(intersectionPoint0, p0, p1, p2);
                Vector2 intersectionPointUV = GetTrianglePointByBarycentricWeight2D(barycentricWeight, triangle.UV0, triangle.UV1, triangle.UV2);

                Triangle triangle1 = new Triangle(p1, intersectionPoint0, p2, triangle.UV1, intersectionPointUV, triangle.UV2);
                Triangle triangle2 = new Triangle(p0, intersectionPoint0, p1, triangle.UV0, intersectionPointUV, triangle.UV1);

                intersectionResult.IntersectionPointList.Add(p1);
                intersectionResult.IntersectionPointList.Add(intersectionPoint0);

                if (p0SideOfPlane == SideOfPlane.UP)
                {
                    intersectionResult.UpperTriangleList.Add(triangle2);

                    if (includePlaneBack)
                        intersectionResult.UnderTriangleList.Add(triangle1);
                }
                else
                {
                    intersectionResult.UpperTriangleList.Add(triangle1);

                    if (includePlaneBack)
                        intersectionResult.UnderTriangleList.Add(triangle2);
                }
            }
        }
        else if (p2SideOfPlane == SideOfPlane.ON)
        {
            //a - b
            if (plane.GetSegmentPlaneIntersectionPoint(p0, p2, out intersectionPoint0))
            {
                Vector3 barycentricWeight = PointBarycentricInTriangle(intersectionPoint0, p0, p1, p2);
                Vector2 intersectionPointUV = GetTrianglePointByBarycentricWeight2D(barycentricWeight, triangle.UV0, triangle.UV1, triangle.UV2);

                Triangle triangle1 = new Triangle(p0, intersectionPoint0, p2, triangle.UV0, intersectionPointUV, triangle.UV2);
                Triangle triangle2 = new Triangle(p2, intersectionPoint0, p1, triangle.UV2, intersectionPointUV, triangle.UV1);

                intersectionResult.IntersectionPointList.Add(p2);
                intersectionResult.IntersectionPointList.Add(intersectionPoint0);

                if (p0SideOfPlane == SideOfPlane.UP)
                {
                    intersectionResult.UpperTriangleList.Add(triangle1);

                    if (includePlaneBack)
                        intersectionResult.UnderTriangleList.Add(triangle2);
                }
                else
                {
                    intersectionResult.UpperTriangleList.Add(triangle2);

                    if (includePlaneBack)
                        intersectionResult.UnderTriangleList.Add(triangle1);
                }
            }
        }
        //a - b
        else if (plane.GetSegmentPlaneIntersectionPoint(p0, p1, out intersectionPoint0))
        {
            Vector3 barycentricWeight0 = PointBarycentricInTriangle(intersectionPoint0, p0, p1, p2);
            Vector2 intersectionPointUV0 = GetTrianglePointByBarycentricWeight2D(barycentricWeight0, triangle.UV0, triangle.UV1, triangle.UV2);

            intersectionResult.IntersectionPointList.Add(intersectionPoint0);

            // b - c
            if (p0SideOfPlane == p2SideOfPlane)
            {
                if (plane.GetSegmentPlaneIntersectionPoint(p1, p2, out intersectionPoint1))
                {
                    Vector3 barycentricWeight1 = PointBarycentricInTriangle(intersectionPoint1, p0, p1, p2);
                    Vector3 intersectionPointUV1 = GetTrianglePointByBarycentricWeight2D(barycentricWeight1, triangle.UV0, triangle.UV1, triangle.UV2);

                    Triangle triangle0 = new Triangle(p0, intersectionPoint0, intersectionPoint1, triangle.UV0, intersectionPointUV0, intersectionPointUV1);
                    Triangle triangle1 = new Triangle(p0, intersectionPoint1, p2, triangle.UV0, intersectionPointUV1, triangle.UV2);
                    Triangle triangle2 = new Triangle(intersectionPoint0, p1, intersectionPoint1, intersectionPointUV0, triangle.UV1, intersectionPointUV1);

                    intersectionResult.IntersectionPointList.Add(intersectionPoint1);

                    if (p0SideOfPlane == SideOfPlane.UP)
                    {
                        intersectionResult.UpperTriangleList.Add(triangle0);
                        intersectionResult.UpperTriangleList.Add(triangle1);

                        if (includePlaneBack)
                            intersectionResult.UnderTriangleList.Add(triangle2);
                    }
                    else
                    {
                        intersectionResult.UpperTriangleList.Add(triangle2);

                        if (includePlaneBack)
                        {
                            intersectionResult.UnderTriangleList.Add(triangle0);
                            intersectionResult.UnderTriangleList.Add(triangle1);
                        }
                    }
                }
            }
            //a -c
            else
            {
                if (plane.GetSegmentPlaneIntersectionPoint(p0, p2, out intersectionPoint1))
                {
                    Vector3 barycentricWeight1 = PointBarycentricInTriangle(intersectionPoint1, p0, p1, p2);
                    Vector3 intersectionPointUV1 = GetTrianglePointByBarycentricWeight2D(barycentricWeight1, triangle.UV0, triangle.UV1, triangle.UV2);

                    intersectionResult.IntersectionPointList.Add(intersectionPoint1);

                    Triangle triangle0 = new Triangle(p0, intersectionPoint0, intersectionPoint1, triangle.UV0, intersectionPointUV0, intersectionPointUV1);

                    Triangle triangle1 = new Triangle(intersectionPoint0, p1, p2, intersectionPointUV0, triangle.UV1, triangle.UV2);
                    Triangle triangle2 = new Triangle(intersectionPoint1, intersectionPoint0, p2, triangle.UV1, intersectionPointUV0, intersectionPointUV1);

                    if (p0SideOfPlane == SideOfPlane.UP)
                    {
                        intersectionResult.UpperTriangleList.Add(triangle0);

                        if (includePlaneBack)
                        {
                            intersectionResult.UnderTriangleList.Add(triangle1);
                            intersectionResult.UnderTriangleList.Add(triangle2);
                        }
                    }
                    else
                    {
                        intersectionResult.UpperTriangleList.Add(triangle1);
                        intersectionResult.UpperTriangleList.Add(triangle2);

                        if (includePlaneBack)
                            intersectionResult.UnderTriangleList.Add(triangle0);
                    }
                }

            }
        }
        //a-c b -c
        else if (plane.GetSegmentPlaneIntersectionPoint(p0, p2, out intersectionPoint0) && plane.GetSegmentPlaneIntersectionPoint(p1, p2, out intersectionPoint1))
        {
            Vector3 barycentricWeight0 = PointBarycentricInTriangle(intersectionPoint0, p0, p1, p2);
            Vector2 intersectionPointUV0 = GetTrianglePointByBarycentricWeight2D(barycentricWeight0, triangle.UV0, triangle.UV1, triangle.UV2);

            Vector3 barycentricWeight1 = PointBarycentricInTriangle(intersectionPoint1, p0, p1, p2);
            Vector2 intersectionPointUV1 = GetTrianglePointByBarycentricWeight2D(barycentricWeight1, triangle.UV0, triangle.UV1, triangle.UV2);

            intersectionResult.IntersectionPointList.Add(intersectionPoint0);
            intersectionResult.IntersectionPointList.Add(intersectionPoint1);

            Triangle triangle0 = new Triangle(intersectionPoint0, p0, intersectionPoint1, intersectionPointUV0, triangle.UV0, intersectionPointUV1);
            Triangle triangle1 = new Triangle(p0, p1, intersectionPoint1, triangle.UV0, triangle.UV1, intersectionPointUV1);
            Triangle triangle2 = new Triangle(intersectionPoint0, intersectionPoint1, p2, intersectionPointUV0, intersectionPointUV1, triangle.UV2);

            if (p2SideOfPlane == SideOfPlane.UP)
            {
                intersectionResult.UpperTriangleList.Add(triangle2);

                if (includePlaneBack)
                {
                    intersectionResult.UnderTriangleList.Add(triangle0);
                    intersectionResult.UnderTriangleList.Add(triangle1);
                }
            }
            else
            {
                intersectionResult.UpperTriangleList.Add(triangle0);
                intersectionResult.UpperTriangleList.Add(triangle1);

                if (includePlaneBack)
                    intersectionResult.UnderTriangleList.Add(triangle2);
            }
        }

        return intersectionResult;
    }

    /// <summary>
    /// 通过triangleList创建Unity的Mesh
    /// </summary>
    /// <param name="triangleList"></param>
    /// <param name="windingOrder"></param>
    /// <returns></returns>
    public static Mesh CreateUnityMeshByTriangleList(List<Triangle> triangleList, TriangleWindingOrder windingOrder = TriangleWindingOrder.CounterClockWise)
    {
        Mesh unityMesh = new Mesh();

        Vector3[] vertices = new Vector3[triangleList.Count * 3];
        Vector2[] uvs = new Vector2[triangleList.Count * 3];
        int[] triangles = new int[triangleList.Count * 3];

        int triangleIndices = 0;

        for (int i = 0; i < triangleList.Count; ++i)
        {
            int index0 = triangleIndices;
            int index1 = triangleIndices + 1;
            int index2 = triangleIndices + 2;

            Triangle triangle = triangleList[i];
            triangle.WindingOrder = windingOrder;

            vertices[index0] = triangle.VertexPosition0;
            vertices[index1] = triangle.VertexPosition1;
            vertices[index2] = triangle.VertexPosition2;

            uvs[index0] = triangle.UV0;
            uvs[index1] = triangle.UV1;
            uvs[index2] = triangle.UV2;

            triangles[index0] = index0;
            triangles[index1] = index1;
            triangles[index2] = index2;

            triangleIndices += 3;
        }

        unityMesh.vertices = vertices;
        unityMesh.uv = uvs;
        unityMesh.triangles = triangles;
        unityMesh.RecalculateNormals();
        unityMesh.RecalculateTangents();

        return unityMesh;
    }

    /// <summary>
    /// 获取模型的绕向
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
    public static TriangleWindingOrder GetMeshWindingOrder(Mesh mesh)
    {
        if (mesh.vertexCount < 3)
            return TriangleWindingOrder.Invaid;

        Vector3 vertex0 = mesh.vertices[mesh.triangles[0]];
        Vector3 vertex1 = mesh.vertices[mesh.triangles[1]];
        Vector3 vertex2 = mesh.vertices[mesh.triangles[2]];

        return GetTriangleWindingOrder(vertex0, vertex1, vertex2);

    }

    /// <summary>
    /// 构建凸包图
    /// </summary>
    /// <param name="points"></param>
    /// <param name="normal">维度方向</param>
    /// <returns></returns>
    /// <remarks>基于Andrew monotone chain 算法，时间复杂度nlogn 在几个凸包算法中表现比较好</remarks>
    public static List<Vector3> CreateConvexhullByMonotoneChain(List<Vector3> points, Vector3 normal)
    {

        return null;
    }

    /// <summary>
    /// Andrew monotone chain算法，计算凸边形
    /// </summary>
    /// <param name="inVertices">输入的无序2维点</param>
    /// <param name="convexHullVertices">输出凸边形顶点</param>
    /// <param name="triangles">triangle indices</param>
    /// <param name="uvs">输出uv</param>
    /// <returns></returns>
    public static void MonotoneChain(List<Vector2> inVertices, out List<Vector2> convexHullVertices, out List<Vector2> uvs, out List<int> triangles)
    {
        convexHullVertices = new List<Vector2>();
        uvs = new List<Vector2>();
        triangles = new List<int>();

        if (inVertices.Count < 3)
            return;

        inVertices.Sort((lVector, rVector) =>
        {
            return (lVector.x < rVector.x) || ((lVector.x == rVector.x) && (lVector.y < rVector.y)) ? -1 : 1;
        });



        Vector2[] convexVertex = new Vector2[inVertices.Count];

        //算法非常巧妙
        //判断前一个点是否在 当前点与前两个点的正方向，如果在正方向说明前一个点是个无用点（凹点）
        //2D向量的cross直接判断目标点的方向

        int converxIndex = 0;

        //bottom convex hull
        for (int i = 0; i < inVertices.Count; ++i)
        {
            while (converxIndex >= 2)
            {
                Vector2 vertex0 = convexVertex[converxIndex - 2];
                Vector2 vertex1 = convexVertex[converxIndex - 1];
                Vector2 current = inVertices[i];

                float crossValue = Cross((vertex1 - vertex0), (current - vertex0));
                if (crossValue < 0)
                    converxIndex--;
                else
                    break;
            }
            convexVertex[converxIndex++] = inVertices[i];
        }

        ////up convex hull
        //for (int i = inVertices.Count - 2, t = converxIndex + 1; i >= 0; --i)
        //{

        //}
        convexHullVertices.AddRange(convexVertex);
    }

    /// <summary>
    /// 三角面的绕向  
    /// </summary>
    public enum TriangleWindingOrder
    {
        Invaid,
        ClockWise,          //顺时针
        CounterClockWise    //逆向时针
    }
}

