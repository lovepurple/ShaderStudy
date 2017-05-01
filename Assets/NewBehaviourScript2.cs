using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewBehaviourScript2 : MonoBehaviour
{

    public LoopScrollRect scrollRect;

    void Start()
    {
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.AddListener(delta =>
            {
                Debug.Log(delta);
            });
        }
    }

}
