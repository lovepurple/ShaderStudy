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
    private Mesh m_decalMesh = null;

    public bool DebugDecalProjector = false;
    public bool DebugDecalMesh = false;


    void Start()
    {
        if (DecalMaterial != null)
            decal = new GeomertyDecal(DecalMaterial);
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit rayCastHitInfo = default(RaycastHit);
            if (Physics.Raycast(cameraRay, out rayCastHitInfo))
            {
                this.m_decalPosition = rayCastHitInfo.point;

                StampDecalOnTarget(rayCastHitInfo.collider.gameObject, this.m_decalPosition);
            }

        }

    }



    private void ModifyTriangles(Mesh mesh)
    {
        int[] triangles = new int[] {  0, 1, 2, 1, 0,  3 };
        mesh.triangles = triangles; 

    }


    private void StampDecalOnTarget(GameObject targetGameObject, Vector3 decalPosition)
    {
        MeshFilter targetMeshFilter = targetGameObject.GetComponent<MeshFilter>();
        if (!targetMeshFilter)
            return;

        Mesh targetMesh = targetMeshFilter.mesh;

        ModifyTriangles(targetMesh);
        return;

        Matrix4x4 meshLocalToWorldMatrix = targetGameObject.transform.localToWorldMatrix;
        this.m_decalMesh = decal.GetDecalMesh(targetMesh, meshLocalToWorldMatrix, decalPosition, DecalSize, DecalRotationEuler);

        GameObject testGameObject = new GameObject();
        MeshFilter mf = testGameObject.AddComponent<MeshFilter>();
        mf.mesh = m_decalMesh;
        MeshRenderer renderer = testGameObject.AddComponent<MeshRenderer>();
        renderer.material = DecalMaterial;


    }


    private void OnDrawGizmos()
    {
        if (DebugDecalProjector)
            DrawDecalProjector();

        if (DebugDecalMesh)
            DrawDecalMesh();

    }


    private void DrawDecalProjector()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.m_decalPosition, DecalSize);
    }

    private void DrawDecalMesh()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireMesh(this.m_decalMesh);
    }


}
