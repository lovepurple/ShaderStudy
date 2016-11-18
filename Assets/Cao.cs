using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Cao : MonoBehaviour
{
    public Slider SliderObject;
    public float ToValue;
    public float Duration;

    public bool StartMove = false;

    float startTime;
    public void Start()
    {
        ToValue = Mathf.Clamp01(ToValue);

        
    }

    float elapsed = 0;

    public void Update()
    {
        if (StartMove)
        {
            elapsed += Time.deltaTime;

            SliderObject.value = Mathf.Lerp(0, ToValue, elapsed / 10);
        }

    }

    public void OnEnable()
    {
        startTime = Time.time;
    }
}
