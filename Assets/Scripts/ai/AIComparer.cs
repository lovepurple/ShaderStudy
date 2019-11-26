#if AI_TEST
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ai
{
    /// <summary>
    /// 根据离球洞的距离进行排序
    /// </summary>
    public class AIDistanceToShotPointComparer : IComparer<Vector2>
    {
        private Vector2 shotPt;

        public AIDistanceToShotPointComparer(Vector2 shot)
        {
            shotPt = shot;
        }

        public int Compare(Vector2 vec1, Vector2 vec2)
        {
            float disA = Vector2.Distance(vec1, shotPt);
            float disB = Vector2.Distance(vec2, shotPt);
            if (vec1 == vec2)
            {
                return 0;
            }

            if (Mathf.Abs(disA - disB) < 0.0000001f)
            {
                if (vec1.y < vec2.y)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }

            if (disA > disB)
            {
                return -1;
            }

            return 1;
        }
    }


    /// <summary>
    /// 扫描线角度排序
    /// </summary>
    public class AILineAngelComparer : IComparer<int>
    {
        public int Compare(int key1, int key2)
        {
            int absAngel1 = Mathf.Abs(key1);
            int absAngel2 = Mathf.Abs(key2);

            if (absAngel1 == absAngel2)
            {
                if (key1 == key2)
                {
                    return 0;
                }
                else
                {
                    return key1.CompareTo(key2);
                }
            }

            return absAngel1.CompareTo(absAngel2);
        }
    }


    /// <summary>
    /// 击球停球区域排序
    /// </summary>
    public class AINormalOptimalRegionComparer : IComparer<AIMapRegion>
    {
        public int Compare(AIMapRegion regionA, AIMapRegion regionB)
        {
            if (regionA == null)
            {
                return 1;
            }

            if (regionB == null)
            {
                return -1;
            }

            if (Mathf.Abs(regionA.DistanceToGoal - regionB.DistanceToGoal) < 0.0000001f)
            {
                return 0;
            }

            if (regionA.DistanceToGoal > regionB.DistanceToGoal)
            {
                return 1;
            }

            return -1;
        }
    }


    /// <summary>
    /// 辅助线排序
    /// </summary>
    public class AIGuideLineRegionComparer : IComparer<AIGuideLine>
    {
        public int Compare(AIGuideLine guideLineA, AIGuideLine guideLineB)
        {
            if (guideLineA == null || guideLineA.FinalGuideLine == null || guideLineA.FinalGuideLine.Count <= 0)
            {
                return 1;
            }

            if (guideLineB == null || guideLineB.FinalGuideLine == null || guideLineB.FinalGuideLine.Count <= 0)
            {
                return -1;
            }

            AIMapRegion regionA = guideLineA.FinalRegion;
            AIMapRegion regionB = guideLineB.FinalRegion;

            AIGuideLineSegment lastSegmentA = guideLineA.FinalGuideLine[guideLineA.FinalGuideLine.Count - 1];
            AIGuideLineSegment lastSegmentB = guideLineB.FinalGuideLine[guideLineB.FinalGuideLine.Count - 1];

            if (lastSegmentA.EndGroundType == 4 && lastSegmentB.EndGroundType != 4)
            {
                return -1;
            }

            if (lastSegmentA.EndGroundType != 4 && lastSegmentB.EndGroundType == 4)
            {
                return 1;
            }

            if (Mathf.Abs(regionA.DistanceToGoal - regionB.DistanceToGoal) < 0.0000001f)
            {
                return 0;
            }

            if (regionA.DistanceToGoal > regionB.DistanceToGoal)
            {
                return 1;
            }

            return -1;
        }
    }
}
#endif