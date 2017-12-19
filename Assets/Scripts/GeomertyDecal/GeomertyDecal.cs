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
        this.DecalSize = decalSize * 0.5f;          //整个Projector是1
        this.DecalPointNormal = pointNormal;

        Matrix4x4 decalPointSpace = Matrix4x4.identity;

        //目标点的三轴向坐标(在贴花点构建坐标系)
        Vector3 xAxis = Vector3.Cross(pointNormal, Vector3.up).normalized;
        Vector3 yAxis = Vector3.Cross(pointNormal, xAxis).normalized;

        decalPointSpace.SetRow(0, xAxis);
        decalPointSpace.SetRow(1, yAxis);
        decalPointSpace.SetRow(2, pointNormal);

        Mesh decalOriginMesh = Object.Instantiate<Mesh>(targetMesh);
        decalOriginMesh.ApplyTransposeMatrix(this.m_originMeshTRSMatrix);

        Mesh decalMesh = GetDecalMesh(decalOriginMesh, this.DecalPosition, this.DecalSize, this.DecalRotationEular, decalPointSpace);
        decalMesh = RemapDecalMapUV(decalMesh, pointNormal);

        RenderDecalGameobject(decalMesh, this.DecalMeterial);

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
        MeshSlicer slicer = new MeshSlicer(decalMesh, projectorEdgePlane);

        SlicedMesh slicedMesh = slicer.Slice(false, false);
        if (slicedMesh.UpperMesh == null)
            return null;

        //left
        projectorEdgePlane = GetDecalProjectorEdgePlane(position, rotationEular, decalSize.x, -projectorCoord.GetRow(0));
        slicedMesh = MeshSlicer.SliceTriangleList(slicedMesh.UpperMeshTriangleList, projectorEdgePlane, false, false);
        if (slicedMesh == null)
            return null;

        //right
        projectorEdgePlane = GetDecalProjectorEdgePlane(position, rotationEular, decalSize.x, projectorCoord.GetRow(0));
        slicedMesh = MeshSlicer.SliceTriangleList(slicedMesh.UpperMeshTriangleList, projectorEdgePlane, false, false);
        if (slicedMesh == null)
            return null;


        //up
        projectorEdgePlane = GetDecalProjectorEdgePlane(position, rotationEular, decalSize.y, projectorCoord.GetRow(1));
        slicer = new MeshSlicer(slicedMesh.UpperMesh, projectorEdgePlane);
        slicedMesh = slicer.Slice(false, false);
        if (slicedMesh.UpperMesh == null)
            return null;

        //down
        projectorEdgePlane = GetDecalProjectorEdgePlane(position, rotationEular, decalSize.y, -projectorCoord.GetRow(1));
        slicedMesh = MeshSlicer.SliceTriangleList(slicedMesh.UpperMeshTriangleList, projectorEdgePlane, false, false);
        if (slicedMesh == null)
            return null;

        return slicedMesh.UpperMesh;
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
    /// 对贴花的模型重新映射UV
    /// </summary>
    /// <param name="decalMesh"></param>
    /// <param name="decalNormal"></param>
    /// <returns></returns>
    /// <remarks>UV展开算法需要进一步研究，这里使用最基本的算法</remarks>
    private Mesh RemapDecalMapUV(Mesh decalMesh, Vector3 decalNormal)
    {
        List<Mapped2DVector> vertexMappedTo2DList = new List<Mapped2DVector>();
        float xMin = float.MaxValue;
        float yMin = float.MaxValue;
        float xMax = float.MinValue;
        float yMax = float.MinValue;

        Vector2[] mappedUV = new Vector2[decalMesh.uv.Length];

        for (int i = 0; i < decalMesh.vertexCount; ++i)
        {
            Vector3 vertexPosition = decalMesh.vertices[i];
            Mapped2DVector posMapTo2D = Mapped2DVector.MapVector3ToVector2(vertexPosition, decalNormal);

            xMin = Mathf.Min(xMin, posMapTo2D.MappedVector2.x);
            xMax = Mathf.Max(xMax, posMapTo2D.MappedVector2.x);

            yMin = Mathf.Min(yMin, posMapTo2D.MappedVector2.y);
            yMax = Mathf.Max(yMax, posMapTo2D.MappedVector2.y);

            vertexMappedTo2DList.Add(posMapTo2D);
        }

        float uSize = xMax - xMin;
        float vSize = yMax - yMin;

        for (int i = 0; i < vertexMappedTo2DList.Count; ++i)
        {
            Mapped2DVector mapped2DVector = vertexMappedTo2DList[i];
            float u = (mapped2DVector.MappedVector2.x - xMin) / uSize;
            float v = 1 - (mapped2DVector.MappedVector2.y - yMin) / vSize;       //MapVector3ToVector2里的y坐标系是向下的，与正常uv的相反

            mappedUV[i] = new Vector2(u, v);
        }

        decalMesh.uv = mappedUV;

        return decalMesh;
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

    private GameObject RenderDecalGameobject(Mesh decalMesh, Material decalMaterial)
    {
        GameObject decalGameObject = new GameObject("DecalObj");
        MeshFilter meshFilter = decalGameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = decalMesh;

        MeshRenderer renderer = decalGameObject.AddComponent<MeshRenderer>();
        renderer.material = decalMaterial;

        return decalGameObject;
    }
}
