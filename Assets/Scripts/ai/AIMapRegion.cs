#if AI_TEST
using UnityEngine;

namespace Assets.Scripts.ai
{
    public class AIMapRegion
    {
        /// <summary>
        /// 停球点周围离危险区域的最小阈值
        /// </summary>
        private const float DisToDangerZoneThreshold = 5f;

        /// <summary>
        /// 最终停球区域危险地形占比阈值，超出此阈值的绝不选
        /// </summary>
        public const float DangerousTerrainTypeThreshold = 0.3f;


        /// <summary>
        /// 原始辅助线最终停球点到球洞的距离
        /// </summary>
        public float DistanceToGoal;


        /// <summary>
        /// 原始辅助线经过旋度计算后最终停球点离球洞的距离
        /// </summary>
        public float DisToGoalNotSpin;

        /// <summary>
        /// 原始落点
        /// </summary>
        public Vector2 DropPoint;

        /// <summary>
        /// 预估的被风吹动偏差后的落点
        /// </summary>
        public Vector2 WindOffsetDropPt;


        /// <summary>
        /// 停球窄方形区域的中心
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// 选定的前后旋
        /// </summary>
        public float TopBackSpin;

        /// <summary>
        /// 选定的侧旋
        /// </summary>
        public float SideSpin;

        /// <summary>
        /// 最终停球区域中心左右两侧离危险地形的距离
        /// </summary>
        public float SideDisFromCenterToDangerZone;

        /// <summary>
        /// 落点左右两侧离危险地形的距离
        /// </summary>
        public float SideDisFromDropPtToDangerZone;


        /// <summary>
        /// 最终停球区域中心前后离危险地形的距离
        /// </summary>
        public float FrontDisFromCenterToDangerZone;

        /// <summary>
        /// 最终停球区域里各地形材质数量
        /// </summary>
        public int[] TerrainTypeArray;

        /// <summary>
        /// 最终停球区域里各地形材质占比
        /// </summary>
        private float[] ProportionArray;


        /// <summary>
        /// 标志位，是否已经更新过占比计算
        /// </summary>
        private bool HasUpdate;


        public AIMapRegion()
        {
            DistanceToGoal = 0;
            TerrainTypeArray = new int[12];
            ProportionArray = new float[12];
            TopBackSpin = 0.0F;
            HasUpdate = false;
        }


        /// <summary>
        /// 更新地形材质占比数组
        /// </summary>
        private void UpdateProportionArray()
        {
            float total = 0;
            for (int i = 0; i < 12; i++)
            {
                total += TerrainTypeArray[i];
            }

            for (int i = 0; i < 12; i++)
            {
                if (total > 0)
                {
                    ProportionArray[i] = TerrainTypeArray[i] / total;
                }
                else
                {
                    ProportionArray[i] = 0;
                }
            }

            HasUpdate = true;
        }


        /// <summary>
        /// 创建无效地形
        /// </summary>
        /// <returns></returns>
        public static AIMapRegion CreateInvalidRegion()
        {
            AIMapRegion invalidRegion = new AIMapRegion();
            invalidRegion.Center = Vector2.zero;
            invalidRegion.DropPoint = Vector2.zero;
            invalidRegion.DistanceToGoal = 10000000;
            invalidRegion.TerrainTypeArray[0]++;
            invalidRegion.TopBackSpin = 0;
            invalidRegion.SideSpin = 0;
            invalidRegion.UpdateProportionArray();
            return invalidRegion;
        }


        /// <summary>
        /// 在无可选项时初略朝球洞打一杆，仅作容错
        /// </summary>
        /// <param name="startPos">发球点</param>
        /// <param name="goalPos">球洞</param>
        /// <param name="maxShotRange">最大击打范围</param>
        /// <param name="wind">风模型</param>
        /// <param name="player">玩家模型</param>
        /// <returns></returns>
        public static AIMapRegion CreateFaultTolerantRegion(Vector2 startPos, Vector2 goalPos, float maxShotRange,
            AIWind wind, AIPlayer player)
        {
            AIMapRegion ftRegion = new AIMapRegion();
            Vector2 startToGoal = goalPos - startPos;
            Vector2 dropPt;
            Vector2 center;
            if (Vector2.Distance(startPos, goalPos) < maxShotRange)
            {
                dropPt = startPos + startToGoal.normalized * Vector2.Distance(startPos, goalPos) * 0.8f;
                center = goalPos;
            }
            else
            {
                dropPt = startPos + startToGoal.normalized * maxShotRange;
                center = startPos + startToGoal.normalized * maxShotRange * 1.2f;
            }

            ftRegion.DropPoint = dropPt;
            ftRegion.Center = center;
            ftRegion.DistanceToGoal = Vector2.Distance(center, goalPos);
            ftRegion.TerrainTypeArray[0]++;
            ftRegion.TopBackSpin = 0;
            ftRegion.SideSpin = 0;
            ftRegion.UpdateProportionArray();
            ftRegion.WindOffsetDropPt =
                dropPt + wind.CalcRandomPredictOffset(Vector2.Distance(startPos, dropPt), player.WindPredictLevel);
            return ftRegion;
        }


        /// <summary>
        /// 判断停球区域地形材质组成是否危险
        /// </summary>
        /// <returns></returns>
        public bool IsRegionInDanger()
        {
            if (!HasUpdate)
            {
                UpdateProportionArray();
            }

            // 危险区域：出界、长草、沙坑、水、其他障碍
            bool isInDanger = ProportionArray[2]
                              + ProportionArray[3]
                              + ProportionArray[7]
                              + ProportionArray[9]
                              + ProportionArray[10]
                              + ProportionArray[11]>= DangerousTerrainTypeThreshold;
            return isInDanger;
        }


        /// <summary>
        /// 判断停球区域是否为出界
        /// </summary>
        /// <returns></returns>
        public bool IsRegionOut()
        {
            if (!HasUpdate)
            {
                UpdateProportionArray();
            }

            bool isOut = 
                         ProportionArray[7]
                         + ProportionArray[10]
                         + ProportionArray[11]
                         >= DangerousTerrainTypeThreshold * 0.5f;
            return isOut;
        }


        /// <summary>
        /// 最终停球区域中心两侧是否离危险区域太靠近
        /// </summary>
        /// <returns></returns>
        public bool IsFinalRegionSideTooNearToDangerZone()
        {
            return SideDisFromCenterToDangerZone < DisToDangerZoneThreshold;
        }

        /// <summary>
        /// 落点两侧是否离危险区域太靠近
        /// </summary>
        /// <returns></returns>
        public bool IsDropPtTooNearToDangerZone()
        {
            return SideDisFromDropPtToDangerZone < DisToDangerZoneThreshold;
        }

        /// <summary>
        /// 落点前后是否离危险区域太近
        /// </summary>
        /// <returns></returns>
        public bool IsDropPtFrontTooNearToDangerZone()
        {
            return FrontDisFromCenterToDangerZone < DisToDangerZoneThreshold;
        }
    }
}

#endif