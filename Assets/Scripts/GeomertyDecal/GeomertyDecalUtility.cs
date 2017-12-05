using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基于几何模型贴花工具类
/// </summary>
public static class GeomertyDecalUtility
{

    /// <summary>
    /// 生成贴花顶点
    /// </summary>
    /// <param name="meshInfo"></param>
    /// <returns></returns>
    public static DecalMeshInfo GeneralDecalMeshInfoByMesh(Mesh meshInfo, Matrix4x4 meshLocalToWorldMatrix)
    {
        DecalMeshInfo decalMeshInfo = new DecalMeshInfo();

        //triangle 是真正的面
        for (int i = 0; i < meshInfo.triangles.Length; ++i)
        {
            int triangleIndex = meshInfo.triangles[i];

            Vector3 vertexPosition = meshLocalToWorldMatrix.MultiplyPoint(meshInfo.vertices[triangleIndex]);
            Vector3 vertexNormal = meshInfo.normals[triangleIndex];

            //DecalVertex decalVertex = new DecalVertex(meshLocalToWorldMatrix.MultiplyPoint(meshInfo.vertices[triangleIndex]), meshInfo.normals[triangleIndex]);

            //decalMeshInfo.PushDecalVertex(decalVertex, triangleIndex);
            decalMeshInfo.PushVertex(vertexPosition, vertexNormal);
        }

        return decalMeshInfo;
    }


    /// <summary>
    /// 剔除 PlaneNormal 方向外的顶点
    /// </summary>
    /// <param name="inDecalVertices"></param>
    /// <param name="planeNormal"></param>
    /// <param name="planeSize"></param>
    /// <returns></returns>
    /// <remarks>
    /// 2017-11-21 16:09:33 裁剪算法有点模糊。
    /// </remarks>
    public static DecalMeshInfo PlaneClipGeometry(DecalMeshInfo inDecalMeshInfo, Vector3 planeNormal, Vector3 planeSize)
    {
        DecalMeshInfo outMeshInfo = new DecalMeshInfo();
        float s = 0.5f * Mathf.Abs(Vector3.Dot(planeSize, planeNormal));

        for (int i = 0; i < inDecalMeshInfo.DecalVertexCount; i += 3)
        {

            DecalVertex inDecalVertex0 = inDecalMeshInfo.GetDecalVertexByIndex(i);
            DecalVertex inDecalVertex1 = inDecalMeshInfo.GetDecalVertexByIndex(i + 1);
            DecalVertex inDecalVertex2 = inDecalMeshInfo.GetDecalVertexByIndex(i + 2);

            bool v1Out = (Vector3.Dot(inDecalVertex0.Position, planeNormal) - s) > 0;
            bool v2Out = (Vector3.Dot(inDecalVertex1.Position, planeNormal) - s) > 0;
            bool v3Out = (Vector3.Dot(inDecalVertex2.Position, planeNormal) - s) > 0;

            int outsideNum = (v1Out ? 1 : 0) + (v2Out ? 1 : 0) + (v3Out ? 1 : 0);

            //被切割后生成的顶点
            DecalVertex newDecalVertex0;
            DecalVertex newDecalVertex1;
            DecalVertex newDecalVertex2;
            DecalVertex newDecalVertex3;

            switch (outsideNum)
            {
                //三个顶点都在Plane内
                case 0:
                    {
                        outMeshInfo.PushDecalVertex(inDecalVertex0);
                        outMeshInfo.PushDecalVertex(inDecalVertex1);
                        outMeshInfo.PushDecalVertex(inDecalVertex2);
                        break;
                    }
                //一个顶点在Plane外，需要生成两个新顶点，组成两个新的Triangle
                case 1:
                    {
                        // one vertex lies outside of the plane, perform clipping
                        if (v1Out)
                        {
                            newDecalVertex0 = inDecalVertex1;
                            newDecalVertex1 = inDecalVertex2;

                            //0-1被分割
                            newDecalVertex2 = ClipDecalVertexByPlane(inDecalVertex0, newDecalVertex1, planeNormal, s);

                            //0-2 被分割
                            newDecalVertex3 = ClipDecalVertexByPlane(inDecalVertex0, newDecalVertex2, planeNormal, s);

                            outMeshInfo.PushDecalTriangle(newDecalVertex0, newDecalVertex2, newDecalVertex1);
                            outMeshInfo.PushDecalTriangle(newDecalVertex1, newDecalVertex2, newDecalVertex3);

                            break;
                        }

                        if (v2Out)
                        {

                            newDecalVertex0 = inDecalVertex0;
                            newDecalVertex1 = inDecalVertex2;

                            //0-1
                            newDecalVertex2 = ClipDecalVertexByPlane(inDecalVertex1, inDecalVertex0, planeNormal, s);

                            //2-1
                            newDecalVertex3 = ClipDecalVertexByPlane(inDecalVertex1, inDecalVertex2, planeNormal, s);

                            outMeshInfo.PushDecalTriangle(newDecalVertex0, newDecalVertex2, newDecalVertex1);
                            outMeshInfo.PushDecalTriangle(newDecalVertex1, newDecalVertex2, newDecalVertex3);

                            break;
                        }

                        if (v3Out)
                        {
                            newDecalVertex0 = inDecalVertex0;
                            newDecalVertex1 = inDecalVertex1;

                            newDecalVertex2 = ClipDecalVertexByPlane(inDecalVertex0, inDecalVertex2, planeNormal, s);
                            newDecalVertex3 = ClipDecalVertexByPlane(inDecalVertex1, inDecalVertex2, planeNormal, s);

                            outMeshInfo.PushDecalTriangle(newDecalVertex0, newDecalVertex2, newDecalVertex1);
                            outMeshInfo.PushDecalTriangle(newDecalVertex1, newDecalVertex2, newDecalVertex3);
                        }
                    }
                    break;
                case 2:
                    {
                        // two vertices lies outside of the plane, perform clipping
                        if (!v1Out)
                        {
                            newDecalVertex0 = inDecalVertex0;
                            newDecalVertex1 = ClipDecalVertexByPlane(inDecalVertex0, inDecalVertex1, planeNormal, s);
                            newDecalVertex2 = ClipDecalVertexByPlane(inDecalVertex0, inDecalVertex2, planeNormal, s);

                            outMeshInfo.PushDecalTriangle(newDecalVertex0, newDecalVertex1, newDecalVertex2);
                        }

                        if (!v2Out)
                        {
                            newDecalVertex0 = inDecalVertex1;

                            newDecalVertex1 = ClipDecalVertexByPlane(inDecalVertex0, inDecalVertex1, planeNormal, s);
                            newDecalVertex2 = ClipDecalVertexByPlane(inDecalVertex1, inDecalVertex2, planeNormal, s);

                            outMeshInfo.PushDecalTriangle(newDecalVertex0, newDecalVertex1, newDecalVertex2);
                        }

                        if (!v3Out)
                        {
                            newDecalVertex0 = inDecalVertex2;

                            newDecalVertex1 = ClipDecalVertexByPlane(inDecalVertex0, inDecalVertex2, planeNormal, s);
                            newDecalVertex2 = ClipDecalVertexByPlane(inDecalVertex1, inDecalVertex2, planeNormal, s);

                            outMeshInfo.PushDecalTriangle(newDecalVertex0, newDecalVertex2, newDecalVertex1);
                        }

                        break;

                    }
                default:
                    break;

            }

        }

        return outMeshInfo;
    }


    /// <summary>
    /// 切割顶点，在切割位置生成新的顶点
    /// </summary>
    /// <param name="decalVertex1"></param>
    /// <param name="decalVertex2"></param>
    /// <param name="clipPlaneNormal"></param>
    /// <param name="s"></param>
    /// <returns></returns>
    private static DecalVertex ClipDecalVertexByPlane(DecalVertex decalVertex1, DecalVertex decalVertex2, Vector3 clipPlaneNormal, float s)
    {
        float d0 = Vector3.Dot(decalVertex1.Position, clipPlaneNormal) - s;
        float d1 = Vector3.Dot(decalVertex2.Position, clipPlaneNormal) - s;

        float s0 = d0 / (d0 - d1);

        Vector3 newDecalVertexPosition = decalVertex1.Position + s0 * (decalVertex2.Position - decalVertex1.Position);
        Vector3 newDecalVertexNormal = decalVertex1.Normal + s0 * (decalVertex2.Normal - decalVertex1.Normal);

        DecalVertex clipDecalVertex = new DecalVertex(newDecalVertexPosition, newDecalVertexNormal);

        return clipDecalVertex;
    }
}
