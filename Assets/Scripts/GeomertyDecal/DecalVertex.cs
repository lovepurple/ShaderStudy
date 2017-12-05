using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 贴花Mesh的顶点信息
/// </summary>
public class DecalVertex : ICloneable
{
    private Vector3 m_vertexPosition;
    private Vector3 m_vertexNormal;

    public DecalVertex(Vector3 position, Vector3 normal)
    {
        this.m_vertexNormal = normal;
        this.m_vertexPosition = position;
    }

    public Vector3 Position
    {
        get { return this.m_vertexPosition; }
    }

    public Vector3 Normal
    {
        get { return this.m_vertexNormal; }
    }

    public void ApplyMatrix(Matrix4x4 matrix)
    {
        this.m_vertexPosition = matrix.MultiplyPoint(this.m_vertexPosition);
    }

    public object Clone()
    {
        return new DecalVertex(this.m_vertexPosition, this.m_vertexNormal);
    }
}

/// <summary>
/// 贴花模型信息
/// </summary>
public class DecalMeshInfo
{

    private List<DecalVertex> m_vertexList = new List<DecalVertex>();

    private int m_verticesCount = 0;

    private List<Vector3> m_vertexPositionList = new List<Vector3>();
    private List<Vector3> m_vertexNormalList = new List<Vector3>();
    private List<int> m_triangleList = new List<int>();


    /// <summary>
    /// 添加三角面信息
    /// </summary>
    /// <param name="decalVertex0"></param>
    /// <param name="decalVertex1"></param>
    /// <param name="decalVertex2"></param>
    public void PushDecalTriangle(DecalVertex decalVertex0, DecalVertex decalVertex1, DecalVertex decalVertex2)
    {
        PushDecalVertex(decalVertex0);
        PushDecalVertex(decalVertex1);
        PushDecalVertex(decalVertex2);
    }

    public void PushDecalVertex(DecalVertex decalVertex)
    {
        PushVertex(decalVertex.Position, decalVertex.Normal);
    }

    public void PushVertex(Vector3 vertexPosition, Vector3 vertexNormal)
    {
        this.m_vertexPositionList.Add(vertexPosition);
        this.m_vertexNormalList.Add(vertexNormal);
        this.m_triangleList.Add(m_verticesCount);

        this.m_verticesCount++;
    }

    public void ApplyMatrix(Matrix4x4 matrix)
    {
        for (int i = 0; i < this.m_vertexPositionList.Count; ++i)
        {
            Vector3 vertexPositionApplyMatrix = matrix.MultiplyPoint(this.m_vertexPositionList[i]);
            this.m_vertexPositionList[i] = vertexPositionApplyMatrix;
        }
    }

    public DecalVertex GetDecalVertexByIndex(int decalIndex)
    {
        if (decalIndex < this.DecalVertexCount)
            return new DecalVertex(this.m_vertexPositionList[decalIndex], this.m_vertexNormalList[decalIndex]);

        return null;
    }

    public List<Vector3> VertexPositionList
    {
        get { return this.m_vertexPositionList; }
    }

    public List<Vector3> VertexNormalList
    {
        get { return this.m_vertexNormalList; }
    }
    public int DecalVertexCount
    {
        get { return this.m_verticesCount; }
    }

}

