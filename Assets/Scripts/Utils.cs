using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Convert String to Vector3
    /// </summary>
    /// <param name="strVector3">x,y,z</param>
    /// <param name="outVector3"></param>
    /// <returns></returns>
    public static bool Vector3FromString(string strVector3, out Vector3 outVector3)
    {
        outVector3 = Vector3.zero;
        string[] posArr = strVector3.Split(',');
        if (posArr.Length != 3)
        {
            Debug.LogError("string to vector3 format error " + strVector3);
            return false;
        }
        else
            outVector3.Set(float.Parse(posArr[0]), float.Parse(posArr[1]), float.Parse(posArr[2]));

        return true;
    }

    /// <summary>
    /// Convert Vector3 to string
    /// </summary>
    /// <param name="vector3"></param>
    /// <returns></returns>
    public static string Vector3ToString(Vector3 vector3)
    {
        return string.Format("{0},{1},{2}", vector3.x, vector3.y, vector3.z);
    }


    /// <summary>
    /// Convert string to Color
    /// </summary>
    /// <param name="strColor">r,g,b,a</param>
    /// <param name="outColor"></param>
    /// <returns></returns>
    public static bool ColorFromString(string strColor, out Color outColor)
    {
        outColor = Color.black;
        string[] colorArr = strColor.Split(',');
        if (colorArr.Length == 3)
        {
            outColor = new Color(float.Parse(colorArr[0]), float.Parse(colorArr[1]), float.Parse(colorArr[2]));
            return true;
        }
        else if (colorArr.Length == 4)
        {
            outColor = new Color(float.Parse(colorArr[0]), float.Parse(colorArr[1]), float.Parse(colorArr[2]), float.Parse(colorArr[3]));
            return true;
        }

        return false;
    }
}

