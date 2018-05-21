using UnityEngine;

namespace GOEngine.Implement
{
    /// <summary>
    /// 圆角矩阵
    /// </summary>
    public class RoundConnerRectangle
    {
        private Vector2 m_min = new Vector2(float.MaxValue, float.MaxValue);
        private Vector2 m_max = new Vector2(float.MinValue, float.MinValue);

        public Vector2 m_roundCenterMin = new Vector2(float.MaxValue, float.MaxValue);
        public Vector2 m_roundCenterMax = new Vector2(float.MinValue, float.MinValue);

        private Vector2 m_innerRectangelMin = new Vector2(float.MaxValue, float.MaxValue);
        private Vector2 m_innerRectangelMax = new Vector2(float.MinValue, float.MinValue);


        public void UpdatePoint(Vector3 point)
        {
            UpdatePoint(new Vector2(point.x, point.z));
        }

        public void UpdatePoint(Vector2 point)
        {
            UpdateX(point.x);
            UpdateY(point.y);

            UpdateRoundCenterAndRadius();
            UpdateInnerRectange();
        }

        private void UpdateX(float x)
        {
            m_min.x = Mathf.Min(m_min.x, x);
            m_max.x = Mathf.Max(m_max.x, x);

        }

        private void UpdateY(float y)
        {
            m_min.y = Mathf.Min(m_min.y, y);
            m_max.y = Mathf.Max(m_max.y, y);
        }

        public void ResetRectangle()
        {
            this.m_min = new Vector2(float.MaxValue, float.MaxValue);
            this.m_max = new Vector2(float.MinValue, float.MinValue);
        }

        public void ResetRectangle(Vector2 min, Vector2 max)
        {
            ResetRectangle();

            this.m_max = max;
            this.m_min = min;

            UpdateRoundCenterAndRadius();
            UpdateInnerRectange();
        }

        /// <summary>
        /// 更新圆角的圆心及半径
        /// </summary>
        private void UpdateRoundCenterAndRadius()
        {
            this.m_roundCenterMin = this.m_min + new Vector2(ConnerRaidus, ConnerRaidus);
            this.m_roundCenterMax = this.m_max - new Vector2(ConnerRaidus, ConnerRaidus);
        }

        private void UpdateInnerRectange()
        {
            if (RectangeAspect > 1.0)
            {
                m_innerRectangelMin.x = m_min.x + ConnerRaidus;
                m_innerRectangelMin.y = m_min.y;

                m_innerRectangelMax.x = m_max.x - ConnerRaidus;
                m_innerRectangelMax.y = m_max.y;
            }
            else if (RectangeAspect < 1.0)
            {
                m_innerRectangelMin.x = m_min.x;
                m_innerRectangelMin.y = m_min.y + ConnerRaidus;

                m_innerRectangelMax.x = m_max.x;
                m_innerRectangelMax.y = m_max.y - ConnerRaidus;
            }
            else
            {
                m_innerRectangelMin = Center;
                m_innerRectangelMax = Center;
            }
        }

        /// <summary>
        /// 获取点在圆角矩形上的点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 ClosestPointOnRectangle(Vector2 point)
        {
            Vector2 closestPoint = point;

            if (point.x < this.m_min.x)
                closestPoint.x = this.m_min.x;
            else if (point.x > this.m_max.x)
                closestPoint.x = this.m_max.x;

            if (point.y < this.m_min.y)
                closestPoint.y = this.m_min.y;
            else if (point.y > this.m_max.y)
                closestPoint.y = this.m_max.y;


            //内部点拉到最近的边上
            float distanceToYmin = closestPoint.y - this.m_min.y;
            float distanceToYmax = this.m_max.y - closestPoint.y;
            float distanceToXmin = closestPoint.x - this.m_min.x;
            float distanceToXmax = this.m_max.x - closestPoint.x;

            float smallestDistance = Mathf.Min(distanceToXmax, distanceToXmin, distanceToYmax, distanceToYmin);
            if (smallestDistance == distanceToXmin)
                closestPoint.x = this.m_min.x;
            else if (smallestDistance == distanceToXmax)
                closestPoint.x = this.m_max.x;
            else if (smallestDistance == distanceToYmin)
                closestPoint.y = this.m_min.y;
            else
                closestPoint.y = this.m_max.y;



            if (RectangeAspect > 1.0)
            {
                if (closestPoint.x > this.m_innerRectangelMin.x && closestPoint.x < this.m_innerRectangelMax.x)
                {
                    return closestPoint;
                }
            }
            else
            {
                if (closestPoint.y > this.m_innerRectangelMin.y && closestPoint.y < this.m_innerRectangelMax.y)
                {
                    return closestPoint;

                }
            }


            Vector2 closestRaidusCenter = default(Vector2);
            if (RectangeAspect < 1.0)
            {
                if (closestPoint.y < this.m_innerRectangelMin.y)
                    closestRaidusCenter = m_roundCenterMin;
                else if (closestPoint.y > this.m_innerRectangelMax.y)
                    closestRaidusCenter = m_roundCenterMax;
            }
            else
            {
                if (closestPoint.x < this.m_innerRectangelMin.x)
                    closestRaidusCenter = m_roundCenterMin;
                else if (closestPoint.x > this.m_innerRectangelMax.x)
                    closestRaidusCenter = m_roundCenterMax;
            }

            Vector2 pointToRaidusCenter = closestPoint - closestRaidusCenter;
            float crossZ = Vector3.Cross(pointToRaidusCenter, Vector2.right).z;
            float angle = Vector2.Angle(pointToRaidusCenter, Vector2.right);
            angle = crossZ > 0 ? 360 - angle : angle;

            angle *= Mathf.Deg2Rad;

            closestPoint.x = closestRaidusCenter.x + Mathf.Cos(angle) * ConnerRaidus;
            closestPoint.y = closestRaidusCenter.y + Mathf.Sin(angle) * ConnerRaidus;

            return closestPoint;
        }

        /// <summary>
        /// 由中心点及方向，获取圆角矩形边上的点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="centerTarget"></param>
        /// <param name="yValue"></param>
        /// <returns></returns>
        public Vector2 ClosestPointOnRoundRectange(Vector2 point, Vector2 centerTarget, float yValue)
        {
            Vector2 centerToPoint = (point - centerTarget);

            GeometryUtility.IntersectionResult intersectionResult = null;


            Debug.Log("-------------------------------------------------");

            Debug.Log("inner rect :" + m_innerRectangelMin + " max :" + m_innerRectangelMax);




            //判断与两边相交情况
            Vector2 segmentStart = m_innerRectangelMin;
            Vector2 segmentEnd = new Vector2(m_innerRectangelMax.x, m_innerRectangelMin.y);

            if (RectangeAspect < 1.0f)
                segmentEnd = new Vector2(m_innerRectangelMin.x, m_innerRectangelMax.y);

            intersectionResult = GeometryUtility.RayToLineSegmentIntersection(centerTarget, centerToPoint, segmentStart, segmentEnd);

            Vector3 segmentStart3D = new Vector3(segmentStart.x, yValue, segmentStart.y);
            Vector3 segmentEnd3D = new Vector3(segmentEnd.x, yValue, segmentEnd.y);

            Debug.DrawLine(segmentStart3D, segmentEnd3D, Color.red, 10);

            if (intersectionResult.IntersectionPointCount > 0)
                return intersectionResult.IntersectionPointList[0];

            segmentStart = m_innerRectangelMax;
            segmentEnd = new Vector2(m_innerRectangelMin.x, m_innerRectangelMax.y);
            if (RectangeAspect < 1.0f)
                segmentEnd = new Vector2(m_innerRectangelMax.x, m_innerRectangelMin.y);

            segmentStart3D = new Vector3(segmentStart.x, yValue, segmentStart.y);
            segmentEnd3D = new Vector3(segmentEnd.x, yValue, segmentEnd.y);

            Debug.DrawLine(segmentStart3D, segmentEnd3D, Color.red, 10);


            intersectionResult = GeometryUtility.RayToLineSegmentIntersection(centerTarget, centerToPoint, segmentStart, segmentEnd);
            if (intersectionResult.IntersectionPointCount > 0)
                return intersectionResult.IntersectionPointList[0];



            Vector2 point1 = default(Vector2);
            Vector2 point2 = default(Vector2);

            int count = GetRayCircleIntersectionPoint2D(centerTarget, centerToPoint, m_innerRectangelMin, ConnerRaidus, ref point1, ref point2);

            Debug.Log("count :" + count);

            //if (point1 != default(Vector2))
            //{
            //    Vector3 rayStart = new Vector3(centerTarget.x, yValue, centerTarget.y);
            //    Vector3 rayDirection = new Vector3(point1.x, yValue, point1.y);

            //    Debug.DrawLine(rayStart, rayDirection, Color.black, 10);
            //}

            //if (point2 != default(Vector2))
            //{
            //    Vector3 rayStart = new Vector3(centerTarget.x, yValue, centerTarget.y);
            //    Vector3 rayDirection = new Vector3(point2.x, yValue, point2.y);

            //    Debug.DrawLine(rayStart, rayDirection, Color.black, 10);
            //}



            if (count == 1)
            {

                if (point1 != default(Vector2) && point2 == default(Vector2))
                    point = point1;
                else if (point1 == default(Vector2) && point2 != default(Vector2))
                    point = point2;


            }
            //if (count == 2)
            //{

            //    point = MathUtil.SqrDistanceTo(point1, centerTarget) > MathUtil.SqrDistanceTo(point2, centerTarget) ? point1 : point2;

            //    Debug.Log(MathUtil.SqrDistanceTo(point1, centerTarget) + " point 1");
            //    Debug.Log(MathUtil.SqrDistanceTo(point2, centerTarget) + " point 2");


            //}

            if (RectangeAspect > 1)
            {
                if (point.x <= m_innerRectangelMin.x)
                    return point;
            }

            if (RectangeAspect < 1)
            {
                if (point.y <= m_innerRectangelMin.y)
                    return point;
            }


            //count = GetRayCircleIntersectionPoint2D(centerTarget, centerToPoint, m_roundCenterMax, ConnerRaidus, ref point1, ref point2);

            //Debug.Log("count :" + count);

            //if (point1 != default(Vector2) && point2 == default(Vector2))
            //    point = point1;
            //else if (point1 == default(Vector2) && point2 != default(Vector2))
            //    point = point2;
            //else if (point1 != default(Vector2) && point2 != default(Vector2))
            //{
            //    point = MathUtil.SqrDistanceTo(point1, centerTarget) > MathUtil.SqrDistanceTo(point2, centerTarget) ? point1 : point2;

            //    Vector3 rayStart = new Vector3(centerTarget.x, yValue, centerTarget.y);
            //    Vector3 rayDirection = new Vector3(point2.x, yValue, point2.y);

            //    Debug.DrawLine(rayStart, rayDirection, Color.blue, 10);
            //}


            //if (RectangeAspect > 1)
            //{
            //    if (point.x >= m_innerRectangelMax.x)
            //        return point;
            //}

            //if (RectangeAspect < 1)
            //{
            //    if (point.y >= m_innerRectangelMax.y)
            //        return point;
            //}




            //GetRayCircleIntersectionPoint2D(centerTarget, centerToPoint, m_roundCenterMax, ConnerRaidus, ref point1, ref point2);
            //if (point1 != default(Vector2))
            //{
            //    Vector3 rayStart = new Vector3(centerTarget.x, yValue, centerTarget.y);
            //    Vector3 rayDirection = new Vector3(point1.x, yValue, point1.y);

            //    Debug.DrawLine(rayStart, rayDirection, Color.black, 10);
            //}

            //if (point2 != default(Vector2))
            //{
            //    Vector3 rayStart = new Vector3(centerTarget.x, yValue, centerTarget.y);
            //    Vector3 rayDirection = new Vector3(point2.x, yValue, point2.y);

            //    Debug.DrawLine(rayStart, rayDirection, Color.black, 10);
            //}

            //if (point1 != default(Vector2) && point2 == default(Vector2))
            //    point = point1;
            //else if (point1 == default(Vector2) && point2 != default(Vector2))
            //    point = point2;

            //if (point1 != default(Vector2) && point2 != default(Vector2))
            //{
            //    point = MathUtil.SqrDistanceTo(point1, centerTarget) > MathUtil.SqrDistanceTo(point2, centerTarget) ? point1 : point2;

            //    Vector3 rayStart = new Vector3(centerTarget.x, yValue, centerTarget.y);
            //    Vector3 rayDirection = new Vector3(point2.x, yValue, point2.y);

            //    Debug.DrawLine(rayStart, rayDirection, Color.blue, 10);

            //    return point;
            //}


            ////两弧相交情况
            //Vector2 pointOnCircle = GetFarPointOnCircle(centerTarget, centerToPoint, this.m_roundCenterMin, ConnerRaidus);
            //if (pointOnCircle != default(Vector2))
            //    return pointOnCircle;

            //pointOnCircle = GetFarPointOnCircle(centerTarget, centerToPoint, this.m_roundCenterMax, ConnerRaidus);
            //if (pointOnCircle != default(Vector2))
            //    return pointOnCircle;

            //intersectionResult = GeometryUtility.RayToCircleIntersection(centerTarget, centerToPoint, this.m_innerRectangelMin, ConnerRaidus);
            //if (intersectionResult.IntersectionPointCount > 0)
            //{
            //    for (int i = 0; i < intersectionResult.IntersectionPointCount; ++i)
            //    {
            //        Vector3 rayStart = new Vector3(centerTarget.x, yValue, centerTarget.y);
            //        Vector3 rayDirection = new Vector3(intersectionResult.IntersectionPointList[i].x, yValue, intersectionResult.IntersectionPointList[i].y);


            //        Debug.DrawLine(rayStart, rayDirection, Color.black, 10);
            //    }

            //}


            return point;
        }


        public static int GetRayCircleIntersectionPoint2D(Vector2 rayorigin, Vector2 rayDirection, Vector2 circleCenterPos, float raidus, ref Vector2 intersectionPoint1, ref Vector2 intersectionPoint2)
        {
            Vector2 diff = rayorigin - circleCenterPos;
            float squareR = raidus * raidus;

            if (Vector2.Dot(diff, diff) <= squareR)
            {
                float l = Vector2.Dot(rayDirection, rayDirection);
                if (l != 0)
                {
                    l = 1.0f / Mathf.Sqrt(l);
                    rayDirection *= l;
                }

                intersectionPoint1 = circleCenterPos + raidus * rayDirection;

                return 1;
            }

            float diffDotRayDir = Vector2.Dot(diff, rayDirection);
            float rayDirDot = Vector2.Dot(rayDirection, rayDirection);

            float distance = diffDotRayDir * diffDotRayDir - rayDirDot * (Vector2.Dot(diff, diff) - squareR);

            if (distance < 0f)
                return 0;
            else if (distance == 0)
            {
                float l = -diffDotRayDir / rayDirDot;
                if (l < 0 || l > 1)
                    return 0;
                else
                {
                    intersectionPoint1 = rayorigin + l * rayDirection;

                    return 1;
                }
            }
            else
            {
                float ds = Mathf.Sqrt(distance);
                float l = (-diffDotRayDir - ds) / rayDirDot;

                if (l >= 0 && l <= 1)
                {
                    intersectionPoint1 = rayorigin + l * rayDirection;
                }

                l = (-diffDotRayDir + ds) / rayDirDot;
                if (l >= 0 && l <= 1)
                {
                    intersectionPoint2 = rayorigin + l * rayDirection;
                }

                return 2;
            }
        }


        //private Vector2 GetFarPointOnCircle(Vector2 target, Vector2 rayDirection, Vector2 circleCenter, float circleRadius)
        //{
        //    GeometryUtility.IntersectionResult intersectionResult = null;

        //    intersectionResult = GeometryUtility.RayToCircleIntersection(target, rayDirection, circleCenter, circleRadius);
        //    if (intersectionResult.IntersectionPointCount > 0)
        //    {
        //        if (intersectionResult.IntersectionPointCount == 1)
        //            return intersectionResult.IntersectionPointList[0];
        //        else if (intersectionResult.IntersectionPointCount == 2)
        //        {
        //            if (MathUtil.SqrDistanceTo(intersectionResult.IntersectionPointList[0], target) > MathUtil.SqrDistanceTo(intersectionResult.IntersectionPointList[1], target))
        //                return intersectionResult.IntersectionPointList[0];
        //            else
        //                return intersectionResult.IntersectionPointList[1];
        //        }
        //    }

        //    return default(Vector2);

        //}


        public Vector2 Min
        {
            get { return this.m_min; }
        }

        public Vector2 Max
        {
            get { return this.m_max; }
        }

        public Vector2 Center
        {
            get { return (this.m_max + this.m_min) / 2; }
        }

        public Vector2 Size
        {
            get { return this.m_max - this.m_min; }
        }

        public Vector2 Extends
        {
            get { return Size / 2; }
        }

        public float ConnerRaidus
        {
            get { return Mathf.Min(Extends.x, Extends.y); }
        }

        /// <summary>
        /// 矩形的方向
        /// </summary>
        private float RectangeAspect
        {
            get { return Extends.x / Extends.y; }
        }

    }
}
