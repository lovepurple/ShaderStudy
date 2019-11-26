using UnityEngine;

namespace Assets.Scripts.ai
{
    public class AIDeviation
    {
        /// <summary>
        /// 随机偏差绝对值在0-0.05之间的概率
        /// </summary>
        /// <param name="directionAccuracyLevel">玩家对方向的掌控能力指数</param>
        /// <param name="clubAccuracyLevel">球杆精准等级</param>
        /// <param name="adventureAspect">Power大于1部分的值</param>
        /// <param name="isDangerTerrain">是否在危险地形</param>
        /// <returns></returns>
        private static float P0_05(int directionAccuracyLevel, int clubAccuracyLevel, float adventureAspect,
            bool isDangerTerrain)
        {
            const float maxP = 0.4f;
            const float minP = 0.25f;
            float p = minP + (maxP - minP) * (directionAccuracyLevel / 100f) * (clubAccuracyLevel / 100f) *
                      (1f - Mathf.Abs(adventureAspect) * 8f) * (isDangerTerrain ? 0.5f : 1f);
            return p;
        }


        /// <summary>
        /// 随机偏差绝对值在0.05 - 0.1之间的概率
        /// </summary>
        /// <param name="directionAccuracyLevel">玩家对方向的掌控能力指数</param>
        /// <param name="clubAccuracyLevel">球杆精准等级</param>
        /// <param name="adventureAspect">Power大于1部分的值</param>
        /// <param name="isDangerTerrain">是否在危险地形</param>
        private static float P05_1(int directionAccuracyLevel, int clubAccuracyLevel, float adventureAspect,
            bool isDangerTerrain)
        {
            const float maxP = 0.45f;
            const float minP = 0.35f;
            float p = minP + (maxP - minP) * (directionAccuracyLevel / 100f) * (clubAccuracyLevel / 100f) *
                      (1f - Mathf.Abs(adventureAspect) * 8f) * (isDangerTerrain ? 0.5f : 1f);
            return p;
        }


        /// <summary>
        /// 随机偏差绝对值在0.1-0.15之间的概率
        /// </summary>
        /// <param name="directionAccuracyLevel">玩家对方向的掌控能力指数</param>
        /// <param name="clubAccuracyLevel">球杆精准等级</param>
        /// <param name="adventureAspect">Power大于1部分的值</param>
        /// <param name="isDangerTerrain">是否在危险地形</param>
        private static float P1_15(int directionAccuracyLevel, int clubAccuracyLevel, float adventureAspect,
            bool isDangerTerrain)
        {
            const float maxP = 0.15f;
            const float minP = 0.06f;
            float p = maxP - (maxP - minP) * (directionAccuracyLevel / 100f) * (clubAccuracyLevel / 100f) *
                      (1f - Mathf.Abs(adventureAspect) * 8f) * (isDangerTerrain ? 0.5f : 1f);
            return p;
        }


        /// <summary>
        /// 随机偏差绝对值在0.15-0.2之间的概率
        /// </summary>
        /// <param name="directionAccuracyLevel">玩家对方向的掌控能力指数</param>
        /// <param name="clubAccuracyLevel">球杆精准等级</param>
        /// <param name="adventureAspect">Power大于1部分的值</param>
        /// <param name="isDangerTerrain">是否在危险地形</param>
        private static float P15_2(int directionAccuracyLevel, int clubAccuracyLevel, float adventureAspect,
            bool isDangerTerrain)
        {
            const float maxP = 0.1f;
            const float minP = 0.04f;
            float p = maxP - (maxP - minP) * (directionAccuracyLevel / 100f) * (clubAccuracyLevel / 100f) *
                      (1f - Mathf.Abs(adventureAspect) * 8f) * (isDangerTerrain ? 0.5f : 1f);
            return p;
        }


        /// <summary>
        /// 随机偏差绝对值在0.2-0.25之间的概率
        /// </summary>
        /// <param name="directionAccuracyLevel">玩家对方向的掌控能力指数</param>
        /// <param name="clubAccuracyLevel">球杆精准等级</param>
        /// <param name="adventureAspect">Power大于1部分的值</param>
        /// <param name="isDangerTerrain">是否在危险地形</param>
        private static float P2_25(int directionAccuracyLevel, int clubAccuracyLevel, float adventureAspect,
            bool isDangerTerrain)
        {
            const float maxP = 0.1f;
            const float minP = 0.035f;
            float p = maxP - (maxP - minP) * (directionAccuracyLevel / 100f) * (clubAccuracyLevel / 100f) *
                      (1f - Mathf.Abs(adventureAspect) * 8f) * (isDangerTerrain ? 0.5f : 1f);
            return p;
        }


        /// <summary>
        /// 随机偏差绝对值在0.25-0.5之间的概率
        /// </summary>
        /// <param name="directionAccuracyLevel">玩家对方向的掌控能力指数</param>
        /// <param name="clubAccuracyLevel">球杆精准等级</param>
        /// <param name="adventureAspect">Power大于1部分的值</param>
        /// <param name="isDangerTerrain">是否在危险地形</param>
        private static float P25_5(int directionAccuracyLevel, int clubAccuracyLevel, float adventureAspect,
            bool isDangerTerrain)
        {
            float p = 1f - P0_05(directionAccuracyLevel, clubAccuracyLevel, adventureAspect, isDangerTerrain)
                         - P05_1(directionAccuracyLevel, clubAccuracyLevel, adventureAspect, isDangerTerrain)
                         - P1_15(directionAccuracyLevel, clubAccuracyLevel, adventureAspect, isDangerTerrain)
                         - P15_2(directionAccuracyLevel, clubAccuracyLevel, adventureAspect, isDangerTerrain)
                         - P2_25(directionAccuracyLevel, clubAccuracyLevel, adventureAspect, isDangerTerrain);
            return p;
        }


        /// <summary>
        /// 计算击球时的偏差值
        /// </summary>
        /// <param name="directionAccuracyLevel">玩家对方向的掌控能力指数</param>
        /// <param name="clubAccuracyLevel">球杆精准等级</param>
        /// <param name="adventureAspect">Power大于1部分的值</param>
        /// <param name="isDangerTerrain">是否在危险地形</param>
        public static float GetShotDerivation(int directionAccuracyLevel, int clubAccuracyLevel, float adventureAspect,
            bool isDangerTerrain)
        {
            float deviation = 0.0F;
            float p0_05 = P0_05(directionAccuracyLevel, clubAccuracyLevel, adventureAspect, isDangerTerrain);
            float p05_1 = P05_1(directionAccuracyLevel, clubAccuracyLevel, adventureAspect, isDangerTerrain);
            float p1_15 = P1_15(directionAccuracyLevel, clubAccuracyLevel, adventureAspect, isDangerTerrain);
            float p15_2 = P15_2(directionAccuracyLevel, clubAccuracyLevel, adventureAspect, isDangerTerrain);
            float p2_25 = P2_25(directionAccuracyLevel, clubAccuracyLevel, adventureAspect, isDangerTerrain);
            float p25_5 = P25_5(directionAccuracyLevel, clubAccuracyLevel, adventureAspect, isDangerTerrain);

            // 概率范围
            int range0_05 = Mathf.FloorToInt(p0_05 * 10000); // 如p0_1为70%时，range0_1 = 7000,则ranInt落在0--7000的概率为70%
            int range05_1 =
                range0_05 + Mathf.FloorToInt(
                    p05_1 * 10000); // 如p1_2为20%时，range1_2 = range0_1+2000 = 9000,则ranInt落在7000--9000的概率为20%
            int range1_15 = range05_1 + Mathf.FloorToInt(p1_15 * 10000); // 依此类推
            int range15_2 = range1_15 + Mathf.FloorToInt(p15_2 * 10000);
            int range2_25 = range15_2 + Mathf.FloorToInt(p2_25 * 10000);
            int range25_5 = range2_25 + Mathf.FloorToInt(p25_5 * 10000);

            // 随机数落在某个区间
            int ranInt = Random.Range(1, 10001);
            int ranSign = Random.Range(0, 2);
            if (ranInt > 0 && ranInt <= range0_05)
            {
                deviation = Random.Range(0f, 0.05f);
            }
            else if (ranInt > range0_05 && ranInt <= range05_1)
            {
                deviation = Random.Range(0.05f, 0.1f);
            }
            else if (ranInt > range05_1 && ranInt <= range1_15)
            {
                deviation = Random.Range(0.1f, 0.15f);
            }
            else if (ranInt > range1_15 && ranInt <= range15_2)
            {
                deviation = Random.Range(0.15f, 0.2f);
            }
            else if (ranInt > range15_2 && ranInt <= range2_25)
            {
                deviation = Random.Range(0.2f, 0.25f);
            }
            else if (ranInt > range2_25 && ranInt <= range25_5)
            {
                deviation = Random.Range(0.25f, 0.5f);
            }

            deviation = Mathf.Pow(-1, ranSign) * deviation;
            return deviation;
        }
    }
}