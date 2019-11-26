#if AI_TEST

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Assets.Scripts.ai
{
    /// <summary>
    /// 连续区间数据结构
    /// </summary>
    [Serializable]
    public struct AIRange
    {
        public float Min;

        public float Max;

        public AIRange(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public bool IsInvalid()
        {
            return Min > Max;
        }
    }


    /// <summary>
    /// 连续区间组成的集合
    /// </summary>
    [Serializable]
    public class AISet
    {
        public List<AIRange> Ranges;

        public AISet()
        {
            Ranges = new List<AIRange>();
        }

        public void AddRange(AIRange range)
        {
            if (range.IsInvalid())
            {
                return;
            }

            AISet newSet = new AISet();
            newSet.Ranges.Add(range);
            UnionWith(newSet);
        }

        public void SubStractRange(AIRange range)
        {
            if (range.IsInvalid())
            {
                return;
            }

            AISet newSet = new AISet();
            newSet.Ranges.Add(range);
            ExceptWith(newSet);
        }

        public void ExceptWith(AISet aiSet)
        {
            SetOperation(aiSet, 1, (isSelfRanges, ori) => isSelfRanges ? 1 : 0);
        }

        public void IntersectWith(AISet aiSet)
        {
            SetOperation(aiSet, 2, (isSelfRanges, ori) => ori + 1);
        }

        public void UnionWith(AISet aiSet)
        {
            SetOperation(aiSet, 1, (isSelfRanges, ori) => 1);
        }

        private delegate int StatusChangeDelegate(bool isSelfRanges, int oriStatus);

        private void SetOperation(AISet aiSet, int threshold, StatusChangeDelegate setOp)
        {
            List<float> bounds = new List<float>();
            // 将所有的端点加入一个数组
            foreach (AIRange r in Ranges)
            {
                bounds.Add(r.Min);
                bounds.Add(r.Max);
            }

            foreach (AIRange r1 in aiSet.Ranges)
            {
                bounds.Add(r1.Min);
                bounds.Add(r1.Max);
            }

            // 排序
            bounds.Sort();

            // 一个用来表示是否在区间内的辅助数组
            int statusCount = bounds.Count - 1;
            List<int> status = new List<int>();
            for (int i = 0; i < statusCount; i++)
            {
                status.Add(0);
            }

            foreach (AIRange r in Ranges)
            {
                int indexLeft = bounds.IndexOf(r.Min);
                int indexRight = bounds.IndexOf(r.Max);
                if (indexLeft != -1 && indexRight != -1)
                {
                    for (int i = indexLeft; i < indexRight; i++)
                    {
                        status[i] = setOp(true, status[i]);
                    }
                }
            }

            foreach (AIRange r1 in aiSet.Ranges)
            {
                int indexLeft = bounds.IndexOf(r1.Min);
                int indexRight = bounds.IndexOf(r1.Max);
                if (indexLeft != -1 && indexRight != -1)
                {
                    for (int i = indexLeft; i < indexRight; i++)
                    {
                        status[i] = setOp(false, status[i]);
                    }
                }
            }

            Ranges = recreateSetByBoundsAndStatus(bounds, status, threshold);
        }


        private List<AIRange> recreateSetByBoundsAndStatus(List<float> bounds, List<int> status, int threshold)
        {
            int statusCount = status.Count;
            List<AIRange> newSet = new List<AIRange>();
            for (int i = 0; i < statusCount; i++)
            {
                if ((i == 0 && status[i] == threshold) || (status[i] == threshold && status[i - 1] < threshold))
                {
                    AIRange range;
                    range.Min = bounds[i];
                    while (i <= statusCount - 1)
                    {
                        if ((i == statusCount - 1 && status[i] == threshold) ||
                            (status[i] == threshold && status[i + 1] < threshold))
                        {
                            range.Max = bounds[i + 1];
                            newSet.Add(range);
                            break;
                        }

                        i++;
                    }
                }
            }

            return newSet;
        }


        public bool IsEmpty()
        {
            if (Ranges == null || Ranges.Count == 0 || (Ranges.Count == 1 && Ranges[0].Max == Ranges[0].Min))
            {
                return true;
            }

            return false;
        }
    }
}

#endif