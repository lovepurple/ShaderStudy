using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 使用任意平面切割模型
/// </summary>
public class SliceMeshByArbitraryPlane : MonoBehaviour
{

    public GameObject TargetGameobject;

    public bool IncludeOppsiteSide = false;
    public bool IncludeIntersection = false;
    public bool RemainOriginGameObject = true;

    public Material SlicedUpperMaterial = null;
    public Material SlicedUpperIntersectionMaterial = null;
    public Material SlicedUnderMaterial = null;
    public Material SlicedUnderIntersectionMaterial = null;

    private MeshSlicer m_meshSlicer = null;

    public float SlicerOffset = 0.1f;


    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit rayCastHitInfo = default(RaycastHit);
            if (Physics.Raycast(cameraRay, out rayCastHitInfo))
            {

                Mesh originMesh = rayCastHitInfo.collider.gameObject.GetComponent<MeshFilter>().sharedMesh;

                Vector3 slicerNormal = rayCastHitInfo.normal;
                Vector3 pointOnSlicer = rayCastHitInfo.point - slicerNormal * SlicerOffset;
                Plane slicerPlane = new Plane(slicerNormal, pointOnSlicer);

                Matrix4x4 trs = Matrix4x4.TRS(rayCastHitInfo.collider.gameObject.transform.position, rayCastHitInfo.collider.gameObject.transform.rotation, rayCastHitInfo.collider.gameObject.transform.localScale);

                Mesh m = Object.Instantiate<Mesh>(originMesh);
                m.ApplyTransposeMatrix(trs);

                MeshSlicer slicer = new MeshSlicer(m, slicerPlane);


                slicer.Slice(false, false);

                slicer.RenderSlicedGameObject(SlicedUpperMaterial);

            }
        }
    }
}
