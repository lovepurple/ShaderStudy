using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CalPosByDistanceAndRotation : MonoBehaviour
{
    public Transform LookAtTarget = null;

    public float DistanceToTarget = 5;
    public float HorizontalAngle = 45;
    public float VerticalAngle = 45;

    void Update()
    {
        if (LookAtTarget != null)
        {

            Vector3 targetPosition = Vector3.zero;

            //向量剩四元数必须右乘向量，跟矩阵类似
            //构建射线时需要注意角的顺序
            //Quaternion.Euler(绕x轴旋转，绕y轴旋转，绕z旋转）必须找一个无关的初始值
            transform.position = LookAtTarget.position + Quaternion.Euler(VerticalAngle,HorizontalAngle, 0) * Vector3.forward * DistanceToTarget;
            transform.LookAt(LookAtTarget);

        }

    }


}
