using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class TestMeshClip : MonoBehaviour
{
    private Vector3[] m_vertexList = new Vector3[2500];

    private void Start()
    {
        string strHeight = Resources.Load<TextAsset>("mapHeight").text;
        StringReader reader = new StringReader(strHeight);
        string lineContent = reader.ReadLine();
        int rowIndex = 0;
        int colIndex = 0;
        while (!string.IsNullOrEmpty(lineContent))
        {
            string[] rowDatas = lineContent.Split(' ');
            colIndex = 0;

            for (int i = 0; i < rowDatas.Length; ++i)
            {
                if (string.IsNullOrEmpty(rowDatas[i]))
                    continue;

                Vector3 point = new Vector3(colIndex * 20, float.Parse(rowDatas[i]), rowIndex * 20);
                m_vertexList[rowIndex * 50 + colIndex] = point;
                colIndex++;
            }
            rowIndex++;
            lineContent = reader.ReadLine();
        }

    }

    private void OnGUI()
    {
        if (GUILayout.Button("draw terrain"))
        {
            Mesh mesh = new Mesh();
            mesh.vertices = this.m_vertexList;

            Color[] colors = new Color[mesh.vertices.Length];
            for (int i = 0; i < this.m_vertexList.Length; ++i)
            {
                float channel = this.m_vertexList[i].y - Mathf.FloorToInt(this.m_vertexList[i].y);
                colors[i] = new Color(channel, channel, channel, 1.0f);
            }
            mesh.colors = colors;


            List<int> triangleList = new List<int>();
            for (int i = 0; i < 49; ++i)
            {
                for (int j = 0; j < 49; ++j)
                {
                    int globalIndex = i * 50 + j;

                    int index0 = globalIndex;
                    int index1 = index0 + 1;
                    int index2 = index0 + 50;
                    int index3 = index2 + 1;

                    triangleList.AddRange(new int[] { index0, index2, index1 });
                    triangleList.AddRange(new int[] { index1, index2, index3 });
                }
            }
            mesh.triangles = triangleList.ToArray();
            //mesh.

            GetComponent<MeshFilter>().sharedMesh = mesh;

        }
    }



}
