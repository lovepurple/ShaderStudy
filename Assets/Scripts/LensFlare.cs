using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 镜头光晕，Shader里 Queue = Overlay 在最近渲染，
/// 需要修改Render上的Mesh
/// </summary>
public class LensFlare : MonoBehaviour
{
    public Material FlareMaterial;

    // Use this for initialization
    void Start()
    {
        CreateMesh();
        CreateRenderer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CreateMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();

        }
        meshFilter.mesh = new Mesh();
        Mesh mesh = meshFilter.mesh;
        mesh.Clear();

        mesh.vertices = new Vector3[]
        {
            new Vector3(-1,1,0),new Vector3(-1,-1,0),
            new Vector3(1,-1,0),new Vector3(1,1,0)
        };

        mesh.uv = new Vector2[]
        {
            new Vector2(0,1),new Vector2(0,0),
            new Vector2(1,0),new Vector2(1,1)
        };

        //Unity triangles的index排序是左手坐标系
        mesh.triangles = new int[]
        {
            0,2,1,3,2,0
        };

        mesh.RecalculateNormals();
        Bounds bounds = mesh.bounds;
        bounds.SetMinMax(new Vector3(-100, -100, -100), new Vector3(100, 100, 100));
        mesh.bounds = bounds;
    }


    private void CreateRenderer()
    {
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.material = FlareMaterial;
    }

}
