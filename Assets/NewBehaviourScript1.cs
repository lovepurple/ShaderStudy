using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class NewBehaviourScript1 : UIBehaviour
{
    public ScrollRect Scroll = null;

    private ScrollRect.ScrollRectEvent scrollEvent = new ScrollRect.ScrollRectEvent();

    protected override void Awake()
    {
        scrollEvent.AddListener(OnValueChanged);

        if (Scroll != null)
        {
            Scroll.onValueChanged = scrollEvent;
        }
    }

    private void OnValueChanged(Vector2 delta)
    {
        Debug.Log(delta);
    }

}
