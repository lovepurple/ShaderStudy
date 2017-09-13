using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 几何帮助类
/// </summary>
public static partial class GeometryUtility
{
    /// <summary>
    /// 根据Extends添加四个顶点
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="vertexList"></param>
    public static void AddVertexByExtensionList(float x, float y, float z, ref List<Vector3> vertexList)
    {
        vertexList.Add(new Vector3(-x, y, z));
        vertexList.Add(new Vector3(x, y, z));
        vertexList.Add(new Vector3(x, -y, z));
        vertexList.Add(new Vector3(-x, -y, z));
    }

    /// <summary>
    /// 根据Extends添加四个顶点
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="vertexList"></param>
    public static void AddVertexByExtensionList(Vector3 extension, ref List<Vector3> vertexList)
    {
        AddVertexByExtensionList(extension.x, extension.y, extension.z, ref vertexList);
    }

    /// <summary>
    /// 通过顶点列表获取AABB包围盒
    /// </summary>
    /// <param name="vertexList"></param>
    /// <returns></returns>
    public static AABBBounds GetAABBBoundsByVertexList(List<Vector3> vertexList)
    {
        AABBBounds aabbBounds = new AABBBounds();
        for (int i = 0; i < vertexList.Count; ++i)
            aabbBounds.UpdatePoint(vertexList[i]);

        return aabbBounds;
    }

    public static AABBBounds GetAABBBoundsOnTargetSpace(Transform targetSpaceTransform, List<Vector3> worldVertexList)
    {
        AABBBounds aabbBounds = new AABBBounds();
        foreach (Vector3 vertex in worldVertexList)
        {
            Vector3 pointOnTargetSpace = targetSpaceTransform.worldToLocalMatrix.MultiplyPoint(vertex);
            aabbBounds.UpdatePoint(pointOnTargetSpace);
        }

        return aabbBounds;
    }

    public static AABBBounds GetAABBBoundsOnWorldSpace(List<Vector3> vertexList, Transform relativeToSpace)
    {
        AABBBounds aabbBounds = new AABBBounds();
        foreach (Vector3 vertex in vertexList)
        {
            Vector3 worldPos = relativeToSpace.localToWorldMatrix.MultiplyPoint(vertex);

            aabbBounds.UpdatePoint(worldPos);
        }

        return aabbBounds;
    }

    /// <summary>
    /// 获取射线与线段的交点
    /// </summary>
    /// <param name="rayorigin"></param>
    /// <param name="rayDirection"></param>
    /// <param name="segmentStartPoint"></param>
    /// <param name="segmentEndPoint"></param>
    /// <returns></returns>
    public static bool GetRaySegmentIntersectionPoint2D(Vector2 rayorigin, Vector2 rayDirection, Vector2 segmentStartPoint, Vector2 segmentEndPoint, ref Vector2 intersectionPoint)
    {
        Vector2 v1 = rayorigin - segmentStartPoint;
        Vector2 v2 = segmentEndPoint - segmentStartPoint;
        Vector2 v3 = new Vector2(-rayDirection.y, rayDirection.x);

        float t1 = ((v2.x * v1.y) - (v2.y * v1.x)) / Vector2.Dot(v2, v3);
        float t2 = Vector2.Dot(v1, v3) / Vector2.Dot(v2, v3);

        if (t1 >= 0 && t2 >= 0 && t2 <= 1)
        {
            intersectionPoint = rayorigin + t1 * rayDirection;
            return true;
        }

        return false;
    }

    public static int GetRayCircleIntersectionPoint2D(Vector2 rayorigin, Vector2 rayDirection, Vector2 circleCenterPos, float raidus, ref Vector2 intersectionPoint1, ref Vector2 intersectionPoint2)
    {
        Vector2 diff = rayorigin - circleCenterPos;
        float squareR = raidus * raidus;

        if (Vector2.Dot(diff, diff) <= squareR)
        {
            float l = Vector2.Dot(rayDirection, rayDirection);
            if (l != 0)
            {
                l = 1.0f / Mathf.Sqrt(l);
                rayDirection *= l;
            }

            intersectionPoint1 = circleCenterPos + raidus * rayDirection;

            return 1;
        }

        float diffDotRayDir = Vector2.Dot(diff, rayDirection);
        float rayDirDot = Vector2.Dot(rayDirection, rayDirection);

        float distance = diffDotRayDir * diffDotRayDir - rayDirDot * (Vector2.Dot(diff, diff) - squareR);

        if (distance < 0f)
            return 0;
        else if (distance == 0)
        {
            float l = -diffDotRayDir / rayDirDot;
            if (l < 0 || l > 1)
                return 0;
            else
            {
                intersectionPoint1 = rayorigin + l * rayDirection;

                return 1;
            }
        }
        else
        {
            float ds = Mathf.Sqrt(distance);
            float l = (-diffDotRayDir - ds) / rayDirDot;

            if (l >= 0 && l <= 1)
            {
                intersectionPoint1 = rayorigin + l * rayDirection;
            }

            l = (-diffDotRayDir + ds) / rayDirDot;
            if (l >= 0 && l <= 1)
            {
                intersectionPoint2 = rayorigin + l * rayDirection;
            }

            return 2;
        }
    }


    /// <summary>
    /// Vector2 的叉乘
    /// </summary>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public static float Cross(Vector2 vector1, Vector2 vector2)
    {
        return vector1.x * vector2.y - vector2.x * vector1.y;
    }
}
