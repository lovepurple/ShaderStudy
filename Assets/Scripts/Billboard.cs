using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Material mat;

    private void Update()
    {
        if (mat == null)
            mat = GetComponentInChildren<Renderer>().material;

        if (mat == null)
            return;

        //模型自身缩放
        mat.SetMatrix("_ObjectScaleMatrix", Matrix4x4.Scale(transform.localScale));

    }


}
