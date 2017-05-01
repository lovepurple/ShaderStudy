using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Cao : UIBehaviour
{
    public GameObject TemplateObject = null;

    public GameObject ContentObject = null;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
           GameObject newObj =  GameObject.Instantiate(TemplateObject);
            newObj.transform.parent = ContentObject.transform;
        }

    }

}
