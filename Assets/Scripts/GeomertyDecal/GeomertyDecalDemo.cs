using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeomertyDecalDemo : MonoBehaviour
{
    public Material DecalMaterial = null;
    public Vector3 DecalSize = Vector3.one;
    public Vector3 DecalRotationEuler = Vector3.zero;

    private GeomertyDecal decal = null;
    private Vector3 m_decalPosition = default(Vector3);
    private Vector3 m_decalPointNormal = default(Vector3);

    private Mesh m_decalMesh = null;

    public bool DebugDecalProjector = false;
    public bool DebugDecalMesh = false;


    private Vector3 m_targetMeshPosition = Vector3.zero;
    private Vector3 m_targetMeshRotation = Vector3.zero;
    private Vector3 m_targetMeshScale = Vector3.one;
    private Matrix4x4 m_targetMeshTRSMatrix = Matrix4x4.identity;

    private Mesh m_debugMesh = null;


    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit rayCastHitInfo = default(RaycastHit);
            if (Physics.Raycast(cameraRay, out rayCastHitInfo))
            {
                this.m_decalPosition = rayCastHitInfo.point;
                this.m_decalPointNormal = rayCastHitInfo.normal;

                this.m_targetMeshPosition = rayCastHitInfo.collider.gameObject.transform.position;
                this.m_targetMeshRotation = rayCastHitInfo.collider.gameObject.transform.localRotation.eulerAngles;
                this.m_targetMeshScale = rayCastHitInfo.collider.gameObject.transform.localScale;
                this.m_targetMeshTRSMatrix = Matrix4x4.TRS(this.m_targetMeshPosition, Quaternion.Euler(this.m_targetMeshRotation), this.m_targetMeshScale);
                StampDecalOnTarget(rayCastHitInfo.collider.gameObject, m_decalPosition, m_decalPointNormal, m_targetMeshTRSMatrix);

            }
        }
    }


    private void StampDecalOnTarget(GameObject targetGameObject, Vector3 decalPosition, Vector3 decalPointNormal, Matrix4x4 originMeshTRS)
    {
        MeshFilter targetMeshFilter = targetGameObject.GetComponent<MeshFilter>();
        if (!targetMeshFilter)
            return;

        Mesh targetMesh = targetMeshFilter.mesh;
        GeomertyDecal decal = new GeomertyDecal(DecalMaterial, originMeshTRS);

        Mesh decalMesh = decal.StampDecal(targetMesh, decalPosition, DecalSize, DecalRotationEuler, decalPointNormal);

        //debugMesh = ss.StampDecal(targetMesh, decalPosition, DecalSize, DecalRotationEuler, this.targetNormal);
        //debugMesh.ApplyTransposeMatrix(Matrix4x4.Scale(m_targetMeshScale));

        m_debugMesh = GameObject.Instantiate(decalMesh);

    }



    private void OnDrawGizmos()
    {

        if (DebugDecalMesh)
            DrawDecalMesh();

    }



    private void DrawDecalMesh()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireMesh(this.m_debugMesh);
    }
}