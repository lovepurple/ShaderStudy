#if AI_TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ai
{
    public class AIMathf
    {
        /// <summary>
        /// 从vecB到vecA带符号的夹角（角度制）,如 vecA（1， 0）-> VecB（0，1）为-90
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="vecB"></param>
        /// <returns></returns>
        public static float AngelWithSides(Vector2 vecA, Vector2 vecB)
        {
            Vector3 vecA3D = new Vector3(vecA.x, 0, vecA.y);
            Vector3 vecB3D = new Vector3(vecB.x, 0, vecB.y);
            float sinAngel = Vector3.Cross(vecA3D.normalized, vecB3D.normalized).y;
            float angel = Vector2.Angle(vecA, vecB);
            if (sinAngel < 0)
            {
                angel = -angel;
            }

            return angel;
        }

        /// <summary>
        /// 线段延伸一段距离后的点坐标
        /// </summary>
        /// <param name="pt1">起点</param>
        /// <param name="pt2">终点</param>
        /// <param name="dis">延长长度</param>
        /// <returns></returns>
        public static Vector2 ExtendLine(Vector2 pt1, Vector2 pt2, float dis)
        {
            Vector2 direction = (pt2 - pt1).normalized;
            Vector2 dest = pt2 + direction * dis;
            return dest;
        }

        /// <summary>
        /// 取浮点数小数部分
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float DemicalPart(float value)
        {
            int integerPart = Mathf.FloorToInt(value);
            float demicalPart = value - integerPart;
            return demicalPart;
        }

        /// <summary>
        /// 浮点数向下取到精度
        /// </summary>
        /// <param name="value"></param>
        /// <param name="percision"></param>
        /// <returns></returns>
        public static float FloorToPercision(float value, float percision)
        {
            int validIntegerPart = Mathf.FloorToInt(value / percision);
            float result = validIntegerPart * percision;
            return result;
        }

        /// <summary>
        /// 浮点数向上取到精度
        /// </summary>
        /// <param name="value"></param>
        /// <param name="percision"></param>
        /// <returns></returns>
        public static float CeilToPercision(float value, float percision)
        {
            int validIntegerPart = Mathf.CeilToInt(value / percision);
            float result = validIntegerPart * percision;
            return result;
        }


        /// <summary>
        /// 向量旋转角度（顺时针）之后得到的新向量（角度制）
        /// </summary>
        /// <param name="oriVec"></param>
        /// <param name="angel"></param>
        /// <returns></returns>
        public static Vector2 RotateVectorByAngle(Vector2 oriVec, float angel)
        {
            // 逆时针转顺时针
            angel = -angel;
            // 求旋转后的方向
            Vector3 rotateAxis = new Vector3(0, 1, 0);
            Vector3 oriVec3 = new Vector3(oriVec.x, 0, oriVec.y);
            Vector3 resultDirection3D = oriVec3 * Mathf.Cos(angel * Mathf.Deg2Rad) +
                                        Vector3.Cross(oriVec3, rotateAxis) * Mathf.Sin(angel * Mathf.Deg2Rad);
            return new Vector2(resultDirection3D.x, resultDirection3D.z);
        }

        /// <summary>
        /// 向量旋转角度（顺时针）之后得到的新向量（角度制）
        /// </summary>
        /// <param name="oriVec"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 RotateVector3ByAngle(Vector3 oriVec, Vector3 axis, float angle)
        {
            // 逆时针转顺时针
            angle = -angle;
            // 求旋转后的方向
            float radiansAngle = angle * Mathf.Deg2Rad;
            Vector3 resultDirection3D =
                oriVec * Mathf.Cos(radiansAngle) + Vector3.Cross(oriVec, axis) * Mathf.Sin(radiansAngle)
                                                 + axis * Vector3.Dot(axis, oriVec) * (1 - Mathf.Cos(radiansAngle));
            return resultDirection3D;
        }


        /// <summary>
        /// 向量旋转角度（顺时针）之后得到的新向量（弧度制）
        /// </summary>
        /// <param name="oriVec"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 RotateVector3ByAngleRadians(Vector3 oriVec, Vector3 axis, float angle)
        {
            // 逆时针转顺时针
            angle = -angle;
            // 求旋转后的方向
            Vector3 resultDirection3D = oriVec * Mathf.Cos(angle) + Vector3.Cross(oriVec, axis) * Mathf.Sin(angle)
                                                                  + axis * Vector3.Dot(axis, oriVec) *
                                                                  (1 - Mathf.Cos(angle));
            return resultDirection3D;
        }


        /// <summary>
        /// Power小于1的打法时，将小于1的击打范围内选定的落点映射到击打范围最小值的圆弧线上后，以此为实际选中的落点，并计算对应的小于1的实际Power值
        /// </summary>
        /// <param name="shotPt">发球点</param>
        /// <param name="windOffsetPt">预估原落点被风偏移后的位置</param>
        /// <param name="dropPt">原落点</param>
        /// <param name="minPowerRange">最小击球范围</param>
        /// <returns></returns>
        public static Vector2 CalcShotRangeEdgeDropPoint(Vector2 shotPt, Vector2 windOffsetPt, Vector2 dropPt,
            float minPowerRange)
        {
            if (Vector2.Distance(shotPt, dropPt) > minPowerRange)
            {
                return dropPt;
            }

            Vector2 OW = windOffsetPt - shotPt;
            Vector2 OD = dropPt - shotPt;
            float angle = AngelWithSides(OW, OD);
            Vector2 ON = RotateVectorByAngle(OW, angle > 0 ? 90 : -90).normalized;
            float lenOD1 = Vector2.Dot(OD, ON);
            Vector2 OD1 = ON * lenOD1;
            Vector2 OWN = OW.normalized;
            float lenD1D2 = Mathf.Sqrt(Mathf.Pow(minPowerRange, 2) - Mathf.Pow(OD1.magnitude, 2));
            Vector2 ptD2 = OD1 + OWN * lenD1D2 + shotPt;
            return ptD2;
        }


        /// <summary>
        /// 落点有效范围假设为一个以击球点为圆心，最大击球范围为外径，最小击球范围为内径，靠近球洞的半圆或半圆环（割线与发球点到球洞的连线垂直）
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="center"></param>
        /// <param name="goalPos"></param>
        /// <param name="minRaidus"></param>
        /// <param name="maxRadius"></param>
        /// <returns></returns>
        public static bool IsPointInSemiCircleFaceToGoal(Vector2 pt, Vector2 center, Vector2 goalPos, float minRaidus,
            float maxRadius)
        {
            // 首先在半径范围内
            if (Vector2.Distance(center, pt) > maxRadius || (pt == center) || Vector2.Distance(center, pt) < minRaidus)
            {
                return false;
            }

            // 其次是在面向球洞的半圆内
            Vector2 dirCenterToGoal = (goalPos - center).normalized;
            bool isIn = Vector2.Dot(dirCenterToGoal, pt - center) >= 0;
            return isIn;
        }
    }

    [System.Serializable]
    public struct AIRect
    {
        public AISerializedVector2 Vertex1;

        public AISerializedVector2 Vertex2;

        public AISerializedVector2 Vertex3;

        public AISerializedVector2 Vertex4;

        public float Angel;

        public float Width;

        public float Height;
    }

    [System.Serializable]
    public struct AISerializedVector2
    {
        public float X;

        public float Y;

        public AISerializedVector2(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    [System.Serializable]
    public struct AISerializedVector3
    {
        public float X;

        public float Y;

        public float Z;

        public AISerializedVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}

#endif