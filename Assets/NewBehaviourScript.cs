using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class NewBehaviourScript : MonoBehaviour
{

    // Use this for initialization
    void OnEnable()
    {
        Material material = GetComponent<MeshRenderer>().sharedMaterial;
        Debug.Log(material.GetFloat("_Cao"));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
