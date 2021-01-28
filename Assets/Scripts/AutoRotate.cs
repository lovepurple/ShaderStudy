using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 rotation;
    public float RotateSpeed = 1;

    void Update()
    {
        this.transform.Rotate(rotation * Time.deltaTime, Space.World);
    }
}
