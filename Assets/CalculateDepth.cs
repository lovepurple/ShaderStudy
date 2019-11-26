using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class CalculateDepth : MonoBehaviour
{

    // [MenuItem("GameObject/3D Object/生成水面深度图(地面最低点为0)")]
    // public static void GenerateDepthPic()
    // {
    //     int terrainLayer = LayerMask.GetMask("terrain");
    //     int waterLayer = LayerMask.GetMask("Water");
    //     // for (int i = 0; i < Selection.objects.Length; i++)
    //     // {
    //     GameObject obj = (GameObject)Selection.objects[0];
    //     MeshFilter meshR = obj.GetComponent<MeshFilter>();
    //     Mesh mesh = meshR.sharedMesh;
    //     MeshCollider mc = obj.GetComponent<MeshCollider>();
    //     bool addMc = false;
    //     if (mc == null)
    //     {
    //         addMc = true;
    //         mc = obj.AddComponent<MeshCollider>();
    //     }
    //     Vector2[] uvs = mesh.uv;
    //     Vector3[] verts = mesh.vertices;
    //     int[] triangles = mesh.triangles;
    //     List<Vector3> realPos = new List<Vector3>();
    //     float minX = float.MaxValue;
    //     float minY = float.MaxValue;
    //     float maxX = float.MinValue;
    //     float maxY = float.MinValue;
    //     for (int i = 0; i < uvs.Length; i++)
    //     {
    //         Debug.Log(uvs[i].ToString("F7"));
    //     }
    //     Debug.Log("---------------------------------------");
    //     for (int i = 0; i < verts.Length; i++)
    //     {
    //         Vector3 v3 = verts[i];
    //         v3.x *= obj.transform.localScale.x;
    //         v3.y *= obj.transform.localScale.y;
    //         v3.z *= obj.transform.localScale.z;
    //         v3 = obj.transform.rotation * v3 + obj.transform.position;
    //         realPos.Add(v3);
    //         Debug.Log(v3.ToString("F7"));
    //         if (v3.x < minX)
    //         {
    //             minX = v3.x;
    //         }
    //         if (v3.x > maxX)
    //         {
    //             maxX = v3.x;
    //         }
    //         if (v3.z < minY)
    //         {
    //             minY = v3.z;
    //         }
    //         if (v3.z > maxY)
    //         {
    //             maxY = v3.z;
    //         }
    //     }
    //     Debug.Log("---------------------------------------");
    //     Debug.Log(mesh.bounds.ToString("F7"));
    //     Debug.Log("---------------------------------------");
    //     for (int i = 0; i < verts.Length; i++)
    //     {
    //         Debug.Log(verts[i].ToString("F7"));
    //         Vector3 v3 = verts[i];
    //     }



    //     int size = 512;
    //     Texture2D tex = new Texture2D(size, size);
    //     Color[,] colors = new Color[size, size];
    //     for (int j = 0; j < size; j++)
    //     {
    //         for (int k = 0; k < size; k++)
    //         {
    //             colors[j, k].r = 0;
    //             colors[j, k].g = 0;
    //             colors[j, k].b = 0;
    //             colors[j, k].a = 0;
    //         }
    //     }
    //     for (int j = 0; j < 2000; j++)
    //     {
    //         for (int k = 0; k < 2000; k++)
    //         {
    //             float x = minX + (maxX - minX) * j / 1999;
    //             float y = minY + (maxY - minY) * k / 1999;
    //             // float theta = obj.transform.rotation.y * Mathf.Deg2Rad;
    //             // float nx = obj.transform.position.x + x * Mathf.Cos(theta) + y * Mathf.Sin(theta);
    //             // float ny = obj.transform.position.z + y * Mathf.Cos(theta) - x * Mathf.Sin(theta);
    //             float nx = x;
    //             float ny = y;
    //             float h = -100;
    //             RaycastHit rayinfo;

    //             int tj = 0;
    //             int tk = 0;

    //             if (Physics.Raycast(new Vector3(nx, 1000, ny), Vector3.down, out rayinfo, 2000, terrainLayer))
    //             {
    //                 h = rayinfo.point.y;
    //             }
    //             float hw = -200;
    //             if (Physics.Raycast(new Vector3(nx, 1000, ny), Vector3.down, out rayinfo, 2000, waterLayer))
    //             {
    //                 hw = rayinfo.point.y;
    //                 var baryCenter = rayinfo.barycentricCoordinate;
    //                 Vector2 uv1 = uvs[triangles[rayinfo.triangleIndex * 3]];
    //                 Vector2 uv2 = uvs[triangles[rayinfo.triangleIndex * 3 + 1]];
    //                 Vector2 uv3 = uvs[triangles[rayinfo.triangleIndex * 3 + 2]];
    //                 Vector3 rp1 = realPos[triangles[rayinfo.triangleIndex * 3]];
    //                 Vector3 rp2 = realPos[triangles[rayinfo.triangleIndex * 3 + 1]];
    //                 Vector3 rp3 = realPos[triangles[rayinfo.triangleIndex * 3 + 2]];
    //                 var v1 = rp3 - rp1;
    //                 var v2 = rp2 - rp1;
    //                 var v3 = rayinfo.point - rp1;
    //                 float k1, k2;
    //                 if (v1.x * v2.y != v2.x * v1.y)
    //                 {
    //                     k1 = (v3.x * v1.y - v1.x * v3.y) / (v2.x * v1.y - v1.x * v2.y);
    //                     k2 = (v3.x * v2.y - v2.x * v3.y) / (v1.x * v2.y - v2.x * v1.y);
    //                 }
    //                 else if (v1.z * v2.y != v2.z * v1.y)
    //                 {
    //                     k1 = (v3.z * v1.y - v1.z * v3.y) / (v2.z * v1.y - v1.z * v2.y);
    //                     k2 = (v3.z * v2.y - v2.z * v3.y) / (v1.z * v2.y - v2.z * v1.y);
    //                 }
    //                 else
    //                 {
    //                     k1 = (v3.x * v1.z - v1.x * v3.z) / (v2.x * v1.z - v1.x * v2.z);
    //                     k2 = (v3.x * v2.z - v2.x * v3.z) / (v1.x * v2.z - v2.x * v1.z);
    //                 }

    //                 // Debug.Log("rp1:" + rp1.ToString("F7") + ",rp2:" + rp2.ToString("F7") + ",rp3:" + rp3.ToString("F7"));
    //                 // Debug.Log("point:" + rayinfo.point.ToString("F7"));
    //                 // Debug.Log("v1:" + v1.ToString("F7") + ",v2:" + v2.ToString("F7") + ",v3:" + v3.ToString("F7"));
    //                 // Debug.Log("k1:" + k1.ToString("F7") + ",k2:" + k2.ToString("F7"));
    //                 var v11 = uv3 - uv1;
    //                 var v22 = uv2 - uv1;
    //                 var v33 = k1 * v11 + k2 * v22;
    //                 var uv = uv1 + v33;
    //                 tj = (int)Mathf.Round((size - 1) * uv.x);
    //                 tk = (int)Mathf.Round((size - 1) * uv.y);
    //                 //rayinfo.
    //                 //rayinfo.triangleIndex
    //             }

    //             // if (tj < size && tk < size)
    //             // {
    //             //     colors[tj, tk].r = Mathf.Clamp01(hw - h);
    //             // }
    //             // if (tj < size && tk >= size)
    //             // {
    //             //     colors[tj, tk - size].g = Mathf.Clamp01(hw - h);
    //             // }
    //             // if (tj >= size && tk < size)
    //             // {
    //             //     colors[tj - size, tk].b = Mathf.Clamp01(hw - h);
    //             // }
    //             // if (tj >= size && tk >= size)
    //             // {
    //             //     colors[tj - size, tk - size].a = Mathf.Clamp01(hw - h);
    //             // }
    //             colors[tj, tk].a = Mathf.Clamp01(hw - h);
    //         }
    //     }
    //     for (int j = 0; j < size; j++)
    //     {
    //         for (int k = 0; k < size; k++)
    //         {
    //             // tex.SetPixel(size - j - 1, size - k - 1, colors[j, k]);
    //             tex.SetPixel(j, k, colors[j, k]);
    //         }
    //     }
    //     if (addMc == true)
    //     {
    //         DestroyImmediate(mc);
    //     }
    //     string path = Application.streamingAssetsPath + "/" + System.DateTime.Now.Ticks.ToString() + ".png";
    //     System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
    // }


    [MenuItem("GameObject/3D Object/生成水面深度图(地面最低点为0)")]
    public static void GenerateDepthPic()
    {
        int terrainLayer = LayerMask.GetMask("terrain");
        int waterLayer = LayerMask.GetMask("Water");
        // for (int i = 0; i < Selection.objects.Length; i++)
        // {
        GameObject obj = (GameObject)Selection.objects[0];
       // int tempLayer = obj.layer;
        //obj.layer = waterLayer;
        MeshFilter meshR = obj.GetComponent<MeshFilter>();
        Mesh mesh = meshR.sharedMesh;
        MeshCollider mc = obj.GetComponent<MeshCollider>();
        bool addMc = false;
        if (mc == null)
        {
            addMc = true;
            mc = obj.AddComponent<MeshCollider>();
        }
        Vector2[] uvs = mesh.uv;
        Vector3[] verts = mesh.vertices;
        int[] triangles = mesh.triangles;
        List<Vector3> realPos = new List<Vector3>();
        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v3 = verts[i];
            v3.x *= obj.transform.localScale.x;
            v3.y *= obj.transform.localScale.y;
            v3.z *= obj.transform.localScale.z;
            v3 = obj.transform.rotation * v3 + obj.transform.position;
            realPos.Add(v3);
        }

        int size = 1024;
        Texture2D tex = new Texture2D(size, size);
        Color[,] colors = new Color[size, size];
        for (int j = 0; j < size; j++)
        {
            for (int k = 0; k < size; k++)
            {
                colors[j, k].r = 0;
                colors[j, k].g = 0;
                colors[j, k].b = 0;
                colors[j, k].a = 0;
            }
        }
        for (int j = 0; j < size; j++)
        {
            for (int k = 0; k < size; k++)
            {
                float x = (float)j / (size - 1);
                float y = (float)k / (size - 1);
                var pos = new Vector2(x, y);
                float nx = float.MaxValue;
                float ny = float.MaxValue;
                for (int l = 0; l < triangles.Length; l += 3)
                {
                    //通过uv坐标推算出像素位置
                    Vector2 uv1 = uvs[triangles[l]];
                    Vector2 uv2 = uvs[triangles[l + 1]];
                    Vector2 uv3 = uvs[triangles[l + 2]];
                    Vector3 rp1 = realPos[triangles[l]];
                    Vector3 rp2 = realPos[triangles[l + 1]];
                    Vector3 rp3 = realPos[triangles[l + 2]];
                    var v1 = uv2 - uv1;
                    var v2 = uv3 - uv1;
                    var v3 = pos - uv1;
                    if ((v1.x * v3.y - v1.y * v3.x) * (v3.x * v2.y - v3.y * v2.x) >= 0)
                    {
                        float k1 = (v3.x * v1.y - v1.x * v3.y) / (v2.x * v1.y - v1.x * v2.y);
                        float k2 = (v3.x * v2.y - v2.x * v3.y) / (v1.x * v2.y - v2.x * v1.y);
                        var v11 = rp3 - rp1;
                        var v22 = rp2 - rp1;
                        var v33 = k1 * v11 + k2 * v22;
                        var rp = rp1 + v33;
                        nx = rp.x;
                        ny = rp.z;
                        break;
                    }
                }

                float h = -100;
                RaycastHit rayinfo;

                if (Physics.Raycast(new Vector3(nx, 1000, ny), Vector3.down, out rayinfo, 2000, terrainLayer))
                {
                    h = rayinfo.point.y;
                }
                float hw = -200;
                if (Physics.Raycast(new Vector3(nx, 1000, ny), Vector3.down, out rayinfo, 2000, waterLayer))
                {
                    hw = rayinfo.point.y;
                }
                colors[j, k].a = Mathf.Clamp01((hw - h)/4);
            }
        }
        for (int j = 0; j < size; j++)
        {
            for (int k = 0; k < size; k++)
            {
                // tex.SetPixel(size - j - 1, size - k - 1, colors[j, k]);
                tex.SetPixel(j, k, colors[j, k]);
            }
        }
        if (addMc == true)
        {
            DestroyImmediate(mc);
        }
       // obj.layer = tempLayer;
        string path = Application.streamingAssetsPath + "/" + System.DateTime.Now.Ticks.ToString() + ".png";
        System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
    }
}
