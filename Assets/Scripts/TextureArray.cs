using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureArray : MonoBehaviour
{
    public List<Texture2D> m_targetTextureList = new List<Texture2D>();

    public Vector2 TextureResolution = new Vector2(512, 512);

    public int SamplerTextureIndex = 0;

    private Texture2DArray m_textureArray = null;
    private Material mat;

    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }


    /// <summary>
    /// 创建Texture2DArray
    /// </summary>
    /// <param name="textureList"></param>
    /// <param name="pixelWidth"></param>
    /// <param name="pixelHeight"></param>
    /// <returns></returns>
    public static Texture2DArray CreateTextureArray(List<Texture2D> textureList, int pixelWidth, int pixelHeight)
    {
        Texture2DArray textureArr = new Texture2DArray(pixelWidth, pixelHeight, textureList.Count, TextureFormat.RGB24, false);

        for (int i = 0; i < textureList.Count; ++i)
        {
            Texture2D texture = textureList[i];
            textureArr.SetPixels(texture.GetPixels(), i);
        }

        textureArr.Apply();

        return textureArr;
    }

    private void Update()
    {
        mat.SetFloat("_SamplerTextureIndex", SamplerTextureIndex);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 70, 50), "Generate TextureArray"))
        {
            m_textureArray = CreateTextureArray(m_targetTextureList, (int)TextureResolution.x, (int)TextureResolution.y);
            mat.SetTexture("_TextureArr", m_textureArray);          //Texture2DArray 也是Texture ,直接可以传给Shader
        }
    }


}
