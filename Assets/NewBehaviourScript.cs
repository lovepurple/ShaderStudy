using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class NewBehaviourScript : MonoBehaviour
{

    ScrollRect scrollView = null;

    void Start()
    {
        if (scrollView != null)
        {
            scrollView.onValueChanged.AddListener(delta =>
            {
                Debug.Log(delta);
            });
        }

    }

    void Update()
    {


    }
}
