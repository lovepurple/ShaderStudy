using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CullModel : MonoBehaviour
{

    public SkinnedMeshRenderer meshRenderer = null;
    public GameObject obj;
    // Use this for initialization
    void Start()
    {
        meshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
    }
    public static GameObject CloneMesh(Mesh _mesh, Material mat)
    {
        GameObject gameObject = new GameObject();
        MeshFilter meshFilter = null;
        MeshRenderer meshRenderer = null;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = mat;
        Mesh mesh = meshFilter.mesh;
        mesh.vertices = _mesh.vertices;
        mesh.colors = _mesh.colors;
        mesh.uv = _mesh.uv;
        mesh.triangles = _mesh.triangles;
        return gameObject;
        //meshRenderer.material = mat;	
    }
    public static int[] Score(int[] ary)
    {
        List<int> lists = new List<int>();
        foreach (int i in ary)
        {
            lists.Add(i);
        }
        lists.Sort();
        return lists.ToArray();
    }
    public static GameObject CutMesh(Mesh _mesh, Material mat, Plane plane, bool dir)
    {
        GameObject gameObject = new GameObject();
        MeshFilter meshFilter = null;
        MeshRenderer meshRenderer = null;
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = mat;
        Mesh mesh = meshFilter.mesh;



        List<int> notDelBeginIndex = new List<int>();
        int _vLen = _mesh.vertices.Length;
        int _len = _mesh.triangles.Length;
        int[] cmpAry = new int[_vLen];
        for (int i = 0; i < _len; i += 3)
        {
            int notSameSizeCount = 0;
            bool res = false;
            for (int j = 0; j < 3; j++)
            {
                int trIndex = _mesh.triangles[i + j];
                if (cmpAry[trIndex] == 0)
                {
                    Vector3 v = _mesh.vertices[trIndex];
                    float f = Vector3.Dot(v, plane.normal) + plane.distance;
                    res = (f > 0);
                    cmpAry[trIndex] = res ? 1 : 2;
                }
                else if (cmpAry[trIndex] == 1)
                {
                    res = true;
                }
                if (dir == res)
                {
                    notSameSizeCount++;
                }
                else
                {
                    break;
                }
            }

            if (notSameSizeCount < 3)
            {
                notDelBeginIndex.Add(i);
            }
            //notDelBeginIndex.Add(i);
        }
        int finalCount = _vLen;
        List<int> triangles = new List<int>();
        /*int index = 0;
		Vector3 [] vertices = new Vector3[finalCount];
		Color[] colors  = new Color[finalCount];
		Vector2[] uv = new Vector2[finalCount];
		for (int i = 0 ; i < _vLen ; i++)
		{
			vertices[index] = _mesh.vertices;
	        colors[index] = Color.white;
	        uv[index] = _mesh.uv;
        	index++;
		}
		mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.uv = uv;*/


        mesh.vertices = _mesh.vertices;
        mesh.colors = _mesh.colors;
        mesh.uv = _mesh.uv;


        int finalTrilen = notDelBeginIndex.Count * 3;
        int[] trianglesAry = new int[finalTrilen];
        int cpyIndex = 0;
        foreach (int i in notDelBeginIndex)
        {
            for (int k = 0; k < 3; k++)
            {
                int _index = _mesh.triangles[i + k];

                trianglesAry[cpyIndex++] = _index;

            }
        }
        mesh.triangles = trianglesAry;
        //mesh.triangles = _mesh.triangles;
        return gameObject;
    }

    GameObject obj1 = null;
    GameObject obj2 = null;
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.T))
        {
            Plane plane = new Plane(new Vector3(0, 0, 1), new Vector3(0, 0, 1));
            Destroy(obj1);
            Mesh mesh = meshRenderer.sharedMesh;
            obj1 = CutMesh(mesh, meshRenderer.material, plane, false);
            obj1.transform.position = new Vector3(-4, 0, 0);
            obj1.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            obj1.transform.rotation = Quaternion.Euler(270, 180, 0);
            //obj1.transform.forward = new Vector3(0,-1,0);

        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Destroy(obj2);
            Mesh mesh = meshRenderer.sharedMesh;
            obj2 = CloneMesh(mesh, meshRenderer.material);
            obj2.transform.position = new Vector3(5, 0, 0);
            obj2.transform.rotation = Quaternion.Euler(270, 0, 0);
        }

    }
}