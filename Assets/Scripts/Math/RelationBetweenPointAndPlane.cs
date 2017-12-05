using UnityEngine;
using System.Collections;
using GOEngine.Implement;

[ExecuteInEditMode]
public class RelationBetweenPointAndPlane : MonoBehaviour
{
    public GameObject PointObject = null;
    public GameObject PlaneObject = null;
    public Vector3 PlaneSize = Vector3.one;

    private Vector3 m_pointPosition = default(Vector3);
    private Vector3 m_planePosition = default(Vector3);
    private Vector3 m_planeNormal = default(Vector3);


    public Vector3 PlanePoint = Vector3.zero;
    public Vector3 PlaneNormal = Vector3.up;

    public Vector3 SegmentStart = Vector3.one;
    public Vector3 SegmentEnd = -Vector3.one;

    private void UpdateParams()
    {
        PlaneObject.transform.localScale = PlaneSize;
        m_planeNormal = PlaneObject.GetComponent<MeshFilter>().sharedMesh.normals[0];
        Mesh meshInfo = PlaneObject.GetComponent<MeshFilter>().sharedMesh;
        m_planePosition = PointObject.transform.position;

        m_pointPosition = PointObject.transform.position;

        //PlaneObject.GetComponent<MeshFilter>().sharedMesh;
    }

    private void Update()
    {
        if (PointObject != null && PlaneObject != null)
        {
            UpdateParams();

            //Mesh meshInfo = PlaneObject.GetComponent<MeshFilter>().sharedMesh;

            //DecalVertex v = new DecalVertex(meshInfo.vertices[0], meshInfo.normals[0]);
            //DecalVertex v2 = new DecalVertex()

        }

        //Vector3 intersectionPoint = GeometryUtility.GetSegmentPlaneIntersectionPoint(PlanePoint, PlaneNormal, SegmentStart, SegmentEnd);
        //Debug.Log(intersectionPoint);
    }




}
