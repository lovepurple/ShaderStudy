using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestMeshClip : MonoBehaviour
{
    public Vector3 slicerNormal = Vector3.up;
    public float slicerDistance = 0;

    public GameObject TargetGameobject;

    public Material sss;

    public bool IncludeOppsiteSide = false;
    public bool IncludeIntersection = false;

    public Material debugMat;

    public Vector3 vector0;
    public Vector3 vector1;
    public Vector3 vector2;


    private void OnGUI()
    {
        //if (GUILayout.Button("slice"))
        //{
        //    Plane plane = new Plane(slicerNormal, slicerDistance);
        //    MeshSlicer slicer = new MeshSlicer(TargetGameobject.GetComponent<MeshFilter>().mesh, plane);
        //    //DebugMeshWindingOrder(TargetGameobject.GetComponent<MeshFilter>().mesh);
        //    List<Mesh> meshs = slicer.Slice(IncludeIntersection, IncludeOppsiteSide, false);

        //    for (int i = 0; i < meshs.Count; ++i)
        //        RenderMesh(meshs[i]);

        //    TargetGameobject.SetActive(false);



        //}

        if (GUILayout.Button("cross"))
        {
            Vector3 r = new Vector3(1, 0, 0);
            Vector3 v = Vector3.Cross(r, vector0).normalized;
            Vector3 u = Vector3.Cross(vector0, v).normalized;

            Debug.Log("u:" + u.ToString() + "        v:" + v.ToString());
            Debug.Log(new Vector3(1, 1, 0).normalized);
        }


        if (GUILayout.Button(" a  xiba"))
        {
            List<Vector2> originList = new List<Vector2>();

            Debug.Log("origin list:");
            for (int i = 0; i < 4; ++i)
            {
                Vector2 ramdonPoint = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
                originList.Add(ramdonPoint);

                Debug.Log(ramdonPoint.ToString());
            }

            


        }

        if (GUILayout.Button(" da sha bi"))
        {
            Vector3 v1 = new Vector3(1, 0, 1);
            Vector3 v2 = new Vector3(0, 1, 0);
            Vector3 v3 = new Vector3(0, 0, 1);

            Debug.Log(GeometryUtility.GetTriangleWindingOrder(v1, v2, v3).ToString());

            Debug.Log(GeometryUtility.GetTriangleWindingOrder(v1, v3, v2).ToString());



        }


    }


    private void DebugMeshWindingOrder(Mesh meshInfo)
    {
        Color[] colors = new Color[meshInfo.vertexCount];
        for (int i = 0; i < meshInfo.triangles.Length; i += 3)
        {
            int vertexIndex0 = meshInfo.triangles[i];
            int vertexIndex1 = meshInfo.triangles[i + 1];
            int vertexIndex2 = meshInfo.triangles[i + 2];

            Vector3 vertex0 = meshInfo.vertices[vertexIndex0];
            Vector3 vertex1 = meshInfo.vertices[vertexIndex1];
            Vector3 vertex2 = meshInfo.vertices[vertexIndex2];


            GeometryUtility.TriangleWindingOrder windingOrder = GeometryUtility.GetTriangleWindingOrder(vertex0, vertex1, vertex2);
            Color vertexColor = windingOrder == GeometryUtility.TriangleWindingOrder.ClockWise ? Color.green : Color.red;

            colors[vertexIndex0] = vertexColor;
            colors[vertexIndex1] = vertexColor;
            colors[vertexIndex2] = vertexColor;
        }
        meshInfo.SetColors(colors.ToList());

        RenderMesh(meshInfo);
    }


    private void RenderMesh(Mesh meshInfo)
    {
        GameObject testGameObject = new GameObject();
        MeshFilter mf = testGameObject.AddComponent<MeshFilter>();
        mf.mesh = meshInfo;
        MeshRenderer renderer = testGameObject.AddComponent<MeshRenderer>();
        renderer.material = sss;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(0, slicerDistance, 0), new Vector3(5, 0, 5));
    }

}
