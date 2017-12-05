using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class TextureUtility
{
    public static Texture2D CreateTexture2DFromRenderTexture(RenderTexture renderTexture)
    {
        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        return texture2D;
    }

    public static Texture2DArray CreateTextureArray(List<RenderTexture> renderTextureList, int pixelWidth, int pixelHeight)
    {
        List<Texture2D> texture2DList = new List<Texture2D>(renderTextureList.Count);
        renderTextureList.ForEach(renderTexture =>
        {
            Texture2D texture2DAfterConvert = CreateTexture2DFromRenderTexture(renderTexture);
            texture2DList.Add(texture2DAfterConvert);
        });

        return CreateTextureArray(texture2DList, pixelWidth, pixelHeight);
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

        UpdateTextureArray(textureArr, textureList);

        return textureArr;
    }

    public static Texture2DArray UpdateTextureArray(Texture2DArray srcTexture2DArray, Texture2D dstTexture, int textureIndex)
    {
        srcTexture2DArray.SetPixels(dstTexture.GetPixels(), textureIndex);
        srcTexture2DArray.Apply();

        return srcTexture2DArray;
    }

    public static Texture2DArray UpdateTextureArray(Texture2DArray srcArray, List<Texture2D> texture2DList)
    {
        for (int i = 0; i < texture2DList.Count; ++i)
        {
            Texture2D texture = texture2DList[i];
            srcArray.SetPixels(texture.GetPixels(), i);
        }

        srcArray.Apply();

        return srcArray;
    }

    public static Texture2DArray UpdateTextureArray(Texture2DArray srcArray, List<RenderTexture> renderTextureList)
    {
        List<Texture2D> texture2DList = new List<Texture2D>(renderTextureList.Count);
        for (int i = 0; i < renderTextureList.Count; ++i)
            texture2DList.Add(CreateTexture2DFromRenderTexture(renderTextureList[i]));

        return UpdateTextureArray(srcArray, texture2DList);
    }
}
