#if AI_TEST

using UnityEngine;

namespace Assets.Scripts.ai
{
    public class AIWind
    {
        /// <summary>
        /// 风的等级
        /// </summary>
        public float WindLevel;


        /// <summary>
        /// 风的方向
        /// </summary>
        public Vector2 WindDirection;

        /// <summary>
        /// 为了模拟玩家对风的掌控能力而引入的人为误差的上限
        /// </summary>
        private const float MaxOffsetError = 10f;


        /// <summary>
        /// 计算玩家预测的风对球产生的偏差
        /// </summary>
        /// <param name="distance">击球点离落点的距离</param>
        /// <param name="playerLv">玩家对风的掌控能力等级</param>
        /// <returns></returns>
        public Vector2 CalcRandomPredictOffset(float distance, float playerLv)
        {
            Vector2 idealOffset = (WindLevel / 100f) * WindDirection * distance;

            float offsetErrorX = Mathf.Pow(-1, Random.Range(0, 2)) *
                                 Random.Range(0, MaxOffsetError * (1f - playerLv / 100f));
            float offsetErrorY = Mathf.Pow(-1, Random.Range(0, 2)) *
                                 Random.Range(0, MaxOffsetError * (1f - playerLv / 100f));

            Vector2 offsetError = new Vector2(offsetErrorX, offsetErrorY);
            Vector2 resultOffset = idealOffset + offsetError;
            return resultOffset;
        }


        public AIWind(float level, Vector2 direction)
        {
            WindDirection = direction.normalized;
            WindLevel = AIMathf.FloorToPercision(level, 0.1f);
        }
    }
}

#endif