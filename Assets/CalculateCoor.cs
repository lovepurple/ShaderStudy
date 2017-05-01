using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CalculateCoor : MonoBehaviour
{



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(gameObject.transform.position);

        Debug.Log(screenPoint);

    }
}
