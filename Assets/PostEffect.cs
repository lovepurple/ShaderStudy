using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PostEffect : MonoBehaviour
{
    public float TestValue = 0;

    public void Update()
    {
        Debug.Log(Mathf.Acos(TestValue));
    }

}