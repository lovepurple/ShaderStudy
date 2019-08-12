using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestMeshClip : MonoBehaviour
{
    public Vector3 slicerNormal = Vector3.up;
    public float slicerDistance = 0;

    public GameObject TargetGameobject;

    public Material sss;

    public bool IncludeOppsiteSide = false;
    public bool IncludeIntersection = false;

    public Material debugMat;

    public Vector3 vector0;
    public Vector3 vector1;
    public Vector3 vector2;

    public void Update()
    {
        gameObject.transform.position += new Vector3(float.MaxValue, 0,0 );
    }

}
