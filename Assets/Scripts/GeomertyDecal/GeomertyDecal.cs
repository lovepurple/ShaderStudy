using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基于几何贴花
/// </summary>
public class GeomertyDecal
{
    public Material DecalMeterial = null;
    public Vector3 DecalPosition = default(Vector3);
    public Vector3 DecalSize = Vector3.one;
    public Vector3 DecalRotationEular = Vector3.zero;

    private Mesh m_targetMesh = null;
    private Matrix4x4 m_targetMeshToWorldMatrix = Matrix4x4.identity;
    private DecalMeshInfo m_decalMeshInfo = null;
    private Matrix4x4 m_worldToDecalProjectorMatrix = Matrix4x4.identity;
    private Matrix4x4 m_decalProjectorToWorldMatrix = Matrix4x4.identity;

    private DecalMeshInfo m_outDecalMeshInfo = null;


    public GeomertyDecal(Material decalMeterial)
    {
        this.DecalMeterial = decalMeterial;
    }

    public GeomertyDecal(Mesh targetMesh, Matrix4x4 targetMeshLocalToWorldMatrix, Material decalMeterial) : this(decalMeterial)
    {
        this.m_targetMesh = targetMesh;
        this.m_targetMeshToWorldMatrix = targetMeshLocalToWorldMatrix;
        this.m_decalMeshInfo = GeomertyDecalUtility.GeneralDecalMeshInfoByMesh(targetMesh, targetMeshLocalToWorldMatrix);
    }

    /// <summary>
    /// 向目标点印贴花
    /// </summary>
    /// <param name="targetMesh"></param>
    /// <param name="position"></param>
    /// <param name="decalSize"></param>
    /// <param name="rotationEular"></param>
    public void StampDecal(Mesh targetMesh, Matrix4x4 targetMeshLocalToWorldMatrix, Vector3 position, Vector3 decalSize, Vector3 rotationEular)
    {
        if (targetMesh != this.m_targetMesh)
        {
            this.m_targetMeshToWorldMatrix = targetMeshLocalToWorldMatrix;
            this.m_decalMeshInfo = GeomertyDecalUtility.GeneralDecalMeshInfoByMesh(targetMesh, targetMeshLocalToWorldMatrix);
        }

        this.DecalPosition = position;
        this.DecalRotationEular = rotationEular;
        this.DecalSize = decalSize;

        UpdateDecalProjectorMatrix(position, decalSize, rotationEular);

        this.m_decalMeshInfo.ApplyMatrix(this.m_worldToDecalProjectorMatrix);

        //上下左右前后裁剪
        this.m_decalMeshInfo = GeomertyDecalUtility.PlaneClipGeometry(this.m_decalMeshInfo, new Vector3(1, 0, 0), decalSize);
        this.m_decalMeshInfo = GeomertyDecalUtility.PlaneClipGeometry(this.m_decalMeshInfo, new Vector3(-1, 0, 0), decalSize);
        this.m_decalMeshInfo = GeomertyDecalUtility.PlaneClipGeometry(this.m_decalMeshInfo, new Vector3(0, 1, 0), decalSize);
        this.m_decalMeshInfo = GeomertyDecalUtility.PlaneClipGeometry(this.m_decalMeshInfo, new Vector3(0, -1, 0), decalSize);
        this.m_decalMeshInfo = GeomertyDecalUtility.PlaneClipGeometry(this.m_decalMeshInfo, new Vector3(0, 0, 1), decalSize);
        this.m_decalMeshInfo = GeomertyDecalUtility.PlaneClipGeometry(this.m_decalMeshInfo, new Vector3(0, 0, -1), decalSize);




    }

    public Mesh GetDecalMesh(Mesh targetMesh, Matrix4x4 targetMeshLocalToWorldMatrix, Vector3 position, Vector3 decalSize, Vector3 rotationEular)
    {
        if (targetMesh != this.m_targetMesh)
        {
            this.m_targetMeshToWorldMatrix = targetMeshLocalToWorldMatrix;
            this.m_decalMeshInfo = GeomertyDecalUtility.GeneralDecalMeshInfoByMesh(targetMesh, targetMeshLocalToWorldMatrix);
        }


        this.DecalPosition = position;
        this.DecalRotationEular = rotationEular;
        this.DecalSize = decalSize;

        UpdateDecalProjectorMatrix(position, decalSize, rotationEular);

        this.m_decalMeshInfo.ApplyMatrix(this.m_worldToDecalProjectorMatrix);

        //return ToUnityMesh();

        //上下左右前后裁剪
        this.m_outDecalMeshInfo = GeomertyDecalUtility.PlaneClipGeometry(this.m_decalMeshInfo, new Vector3(1, 0, 0), decalSize);
        //this.m_outDecalMeshInfo = GeomertyDecalUtility.PlaneClipGeometry(this.m_outDecalMeshInfo, new Vector3(-1, 0, 0), decalSize);
        //this.m_outDecalMeshInfo = GeomertyDecalUtility.PlaneClipGeometry(this.m_outDecalMeshInfo, new Vector3(0, 1, 0), decalSize);
        //this.m_outDecalMeshInfo = GeomertyDecalUtility.PlaneClipGeometry(this.m_outDecalMeshInfo, new Vector3(0, -1, 0), decalSize);
        //this.m_outDecalMeshInfo = GeomertyDecalUtility.PlaneClipGeometry(this.m_outDecalMeshInfo, new Vector3(0, 0, 1), decalSize);
        //this.m_outDecalMeshInfo = GeomertyDecalUtility.PlaneClipGeometry(this.m_outDecalMeshInfo, new Vector3(0, 0, -1), decalSize);

        //创建一个投影大小的Quad
        if (m_outDecalMeshInfo.DecalVertexCount == 0)
        {
            Matrix4x4 scalar = Matrix4x4.Scale(DecalSize);
            DecalVertex quadVertex0 = new DecalVertex(scalar.MultiplyPoint(new Vector3(-1, -1, 0)), new Vector3(0, 0, 1));
            DecalVertex quadVertex1 = new DecalVertex(scalar.MultiplyPoint(new Vector3(1, -1, 0)), new Vector3(0, 0, 1));
            DecalVertex quadVertex2 = new DecalVertex(scalar.MultiplyPoint(new Vector3(1, 1, 0)), new Vector3(0, 0, 1));
            DecalVertex quadVertex3 = new DecalVertex(scalar.MultiplyPoint(new Vector3(-1, 1, 0)), new Vector3(0, 0, 1));

            m_outDecalMeshInfo.PushDecalTriangle(quadVertex0, quadVertex1, quadVertex3);
            m_outDecalMeshInfo.PushDecalTriangle(quadVertex1, quadVertex2, quadVertex3);

        }


        return ToUnityMesh();
    }

    /// <summary>
    /// 更新贴花正交矩阵
    /// </summary>
    /// <param name="decalPosition"></param>
    /// <param name="decalSize"></param>
    /// <param name="decalRotationEular"></param>
    /// <returns></returns>
    private void UpdateDecalProjectorMatrix(Vector3 decalPosition, Vector3 decalSize, Vector3 decalRotationEular)
    {
        this.m_worldToDecalProjectorMatrix = Matrix4x4.TRS(decalPosition, Quaternion.Euler(decalRotationEular), Vector3.one);
        this.m_decalProjectorToWorldMatrix = m_worldToDecalProjectorMatrix.inverse;
    }


    /// <summary>
    /// 转换DecalMesh 到Unity的Mesh
    /// </summary>
    /// <returns></returns>
    private Mesh ToUnityMesh()
    {
        Mesh unityMesh = new Mesh();

        List<Vector2> meshUVList = new List<Vector2>();
        List<int> meshTriangleList = new List<int>();
        List<Vector3> meshPositionList = new List<Vector3>();
        List<Vector3> meshNormalList = new List<Vector3>();

        Matrix4x4 projectorToMeshLocalMatrix = Matrix4x4.Translate(DecalPosition).inverse * m_decalProjectorToWorldMatrix;

        for (int i = 0; i < this.m_outDecalMeshInfo.DecalVertexCount; ++i)
        {
            Vector3 vertexPosition = projectorToMeshLocalMatrix.MultiplyPoint(this.m_outDecalMeshInfo.VertexPositionList[i]);
            meshPositionList.Add(vertexPosition);

            Vector3 vertexNormal = this.m_outDecalMeshInfo.VertexNormalList[i];
            meshNormalList.Add(vertexNormal);

            Vector2 uv = CalculateVertexUV(this.m_outDecalMeshInfo.VertexPositionList[i], this.DecalSize);
            meshUVList.Add(uv);

            meshTriangleList.Add(i);

        }

        unityMesh.SetVertices(meshPositionList);
        unityMesh.SetNormals(meshNormalList);
        unityMesh.SetUVs(0, meshUVList);
        unityMesh.SetTriangles(meshTriangleList, 0);

        return unityMesh;
    }

    /// <summary>
    /// 计算顶点UV
    /// </summary>
    /// <param name="vertexPositionInProjectorSpace"></param>
    /// <returns></returns>
    private Vector2 CalculateVertexUV(Vector3 vertexPositionInProjectorSpace, Vector3 projectorSize)
    {
        float normalizeSizeX = vertexPositionInProjectorSpace.x / (2.0f * projectorSize.x);
        float normlaizeSizeY = vertexPositionInProjectorSpace.y / (2.0f * projectorSize.y);

        Vector2 uv = new Vector2(0.5f, 0.5f) + new Vector2(normalizeSizeX, normlaizeSizeY);

        return uv;
    }

}
