using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipMeshDemo : MonoBehaviour
{

    public Vector3 Vertex0 = new Vector3(3, 3, 0);
    public Vector3 Vertex1 = new Vector3(-3, -3, 0);
    public Vector3 Vertex2 = new Vector3(-3, 3, 0);
    public Vector3 Vertex3 = new Vector3(3, -3, 0);

    public Vector3 Normal = new Vector3(0, 0, 1);

    private Mesh m_mesh1 = null;
    private Mesh m_mesh2 = null;
    private Material m_renderMaterial = null;

    public Vector3 ClipPlaneNormal = new Vector3(1, 0, 0);
    public Vector3 ClipPlanePosition = new Vector3(-1, 0, 0);

    public Mesh TestMesh;

    private void Start()
    {
        m_renderMaterial = new Material(Shader.Find("Legacy Shaders/Diffuse"));
    }


    private void OnGUI()
    {
        if (GUILayout.Button("create mesh 1"))
        {
            m_mesh1 = new Mesh();
            m_mesh1.SetVertices(new List<Vector3>() { Vertex1, Vertex2, Vertex0, Vertex3 });
            m_mesh1.SetTriangles(new int[6] { 0, 1, 2, 0, 2, 3 }, 0);
            m_mesh1.SetNormals(new List<Vector3>() { Normal, Normal, Normal, Normal });

            RenderMesh(m_mesh1);

        }

        if (GUILayout.Button("create mesh 2"))
        {
            m_mesh2 = new Mesh();
            m_mesh2.SetVertices(new List<Vector3>() { Vertex3, Vertex2, Vertex0, Vertex1 });
            m_mesh2.SetTriangles(new int[6] { 3, 2, 0, 3, 1, 2 }, 0);
            m_mesh2.SetNormals(new List<Vector3>() { Normal, Normal, Normal, Normal });
            RenderMesh(m_mesh2);
        }

        if (GUILayout.Button("clip mesh1"))
        {
            //Mesh m = PlaneClipGeometry(m_mesh2, ClipPlaneNormal, Vector3.one);
            //m = PlaneClipGeometry(m, new Vector3(-1, -1, 0), Vector3.one);
            //RenderMesh(m);
        }

        if(GUILayout.Button("sssss"))
        {
            Debug.Log(TestMesh);
        }
    }




    private void RenderMesh(Mesh meshInfo)
    {
        GameObject testGameObject = new GameObject();
        MeshFilter mf = testGameObject.AddComponent<MeshFilter>();
        mf.mesh = meshInfo;
        MeshRenderer renderer = testGameObject.AddComponent<MeshRenderer>();
        renderer.material = new Material(m_renderMaterial);

    }

}
