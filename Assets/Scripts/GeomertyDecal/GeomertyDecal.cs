using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 几何贴花,基于平面裁剪模型算法
/// </summary>
public class GeomertyDecal
{
    public Material DecalMeterial = null;
    public Vector3 DecalPosition = default(Vector3);
    public Vector3 DecalSize = Vector3.one;
    public Vector3 DecalRotationEular = Vector3.zero;
    public Vector3 DecalPointNormal = Vector3.up;

    private Mesh m_targetMesh = null;
    private Matrix4x4 m_targetMeshToWorldMatrix = Matrix4x4.identity;
    private Matrix4x4 m_worldToDecalProjectorMatrix = Matrix4x4.identity;
    private Matrix4x4 m_decalProjectorToWorldMatrix = Matrix4x4.identity;

    private Matrix4x4 m_originMeshTRSMatrix = Matrix4x4.identity;

    private Mesh m_decalMesh = null;

    public GeomertyDecal(Material decalMeterial, Matrix4x4 targetMeshTRSMatrix)
    {
        this.DecalMeterial = decalMeterial;
        this.m_originMeshTRSMatrix = targetMeshTRSMatrix;
    }


    /// <summary>
    /// 向目标点印贴花
    /// </summary>
    /// <param name="targetMesh"></param>
    /// <param name="position"></param>
    /// <param name="decalSize"></param>
    /// <param name="pointNormal">贴花目标点的法线方向</param>
    /// <param name="rotationEular"></param>
    public Mesh StampDecal(Mesh targetMesh, Vector3 position, Vector3 decalSize, Vector3 rotationEular, Vector3 pointNormal)
    {
        this.DecalPosition = position;
        this.DecalRotationEular = rotationEular;
        this.DecalSize = decalSize * 0.5f;      //整个Projector是1
        this.DecalPointNormal = pointNormal;

        Matrix4x4 decalPointSpace = Matrix4x4.identity;

        //目标点的三轴向坐标
        Vector3 xAxis = Vector3.Cross(pointNormal, Vector3.up).normalized;
        Vector3 yAxis = Vector3.Cross(pointNormal, xAxis).normalized;

        decalPointSpace.SetRow(0, xAxis);
        decalPointSpace.SetRow(1, yAxis);
        decalPointSpace.SetRow(2, pointNormal);

        Mesh decalOriginMesh = Object.Instantiate<Mesh>(targetMesh);
        //decalOriginMesh.ApplyTransposeMatrix(this.m_originMeshTRSMatrix);

        Mesh decalMesh = GetDecalMesh(decalOriginMesh, this.DecalPosition, this.DecalSize, this.DecalRotationEular, decalPointSpace);

        return decalMesh;
    }

    /// <summary>
    /// 获取贴花的模型
    /// </summary>
    /// <param name="targetMesh"></param>
    /// <param name="targetMeshLocalToWorldMatrix"></param>
    /// <param name="position"></param>
    /// <param name="decalSize"></param>
    /// <param name="rotationEular"></param>
    /// <returns></returns>
    /// todo: 可以根据点基位置估算一个切割顺序，优化,主要基于动态模型切割算法
    public Mesh GetDecalMesh(Mesh targetMesh, Vector3 position, Vector3 decalSize, Vector3 rotationEular, Matrix4x4 projectorCoord)
    {
        Mesh decalMesh = targetMesh;

        //back
        Plane projectorEdgePlane = GetDecalProjectorEdgePlane(position, rotationEular, decalSize.z, projectorCoord.GetRow(2));
        GeometryDebugHelper.instance.DrawPlane(projectorEdgePlane);
        MeshSlicer slicer = new MeshSlicer(decalMesh, projectorEdgePlane);


        SlicedMesh slicedMesh = slicer.Slice(false, false);
        if (slicedMesh.UpperMesh == null)
            return null;
        return slicedMesh.UpperMesh;
        //up
        projectorEdgePlane = GetDecalProjectorEdgePlane(position, rotationEular, decalSize.y, projectorCoord.GetRow(1));
        slicer = new MeshSlicer(slicedMesh.UpperMesh, projectorEdgePlane);
        slicedMesh = slicer.Slice(false, false);
        //slicedMesh = MeshSlicer.SliceTriangleList(slicedMesh.UpperMeshTriangleList, projectorEdgePlane, false, false);
        if (slicedMesh.UpperMesh == null)
            return null;

        return slicedMesh.UpperMesh;

        /*
        //down
        projectorEdgePlane = GetDecalProjectorEdgePlane(ProjectorEdgeDirection.DOWN, position, rotationEular, decalSize);
        slicedMesh = MeshSlicer.SliceTriangleList(slicedMesh.UpperMeshTriangleList, projectorEdgePlane, false, false);
        if (slicedMesh == null)
            return null;

        //left
        projectorEdgePlane = GetDecalProjectorEdgePlane(ProjectorEdgeDirection.LEFT, position, rotationEular, decalSize);
        slicedMesh = MeshSlicer.SliceTriangleList(slicedMesh.UpperMeshTriangleList, projectorEdgePlane, false, false);
        if (slicedMesh == null)
            return null;

        //right
        projectorEdgePlane = GetDecalProjectorEdgePlane(ProjectorEdgeDirection.RIGHT, position, rotationEular, decalSize);
        slicedMesh = MeshSlicer.SliceTriangleList(slicedMesh.UpperMeshTriangleList, projectorEdgePlane, false, false);
        if (slicedMesh == null)
            return null;

        return slicedMesh.UpperMesh; */
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


    /// <summary>
    /// 贴花投影裁剪边缘Plane
    /// </summary>
    /// <param name="projectorCenterPoint"></param>
    /// <param name="projectorRotation"></param>
    /// <param name="scaleTowardsNormalDirection"></param>
    /// <param name="planeEdgeNormal"></param>
    /// <returns></returns>
    private Plane GetDecalProjectorEdgePlane(Vector3 projectorCenterPoint, Vector3 projectorRotation, float scaleTowardsNormalDirection, Vector3 planeEdgeNormal)
    {
        planeEdgeNormal = Quaternion.Euler(projectorRotation) * planeEdgeNormal;
        Vector3 pointOnPlane = projectorCenterPoint - planeEdgeNormal * scaleTowardsNormalDirection;

        Plane clipPlane = new Plane(planeEdgeNormal, pointOnPlane);

        return clipPlane;
    }
}
