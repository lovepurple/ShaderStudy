#if AI_TEST

using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.ai
{
    /// <summary>
    /// 定义AI玩家的各项能力指标
    /// </summary>
    public class AIPlayer
    {
        /// 冒险精神指数
        public int AdventureLevel;

        /// 风向预判能力指数
        public int WindPredictLevel;

        /// 对方向掌控能力指数 
        public int DirectionAccuracyLevel;

        /// 对上旋的预测能力指数
        public int TopSpinPredictLevel;

        /// 对下旋的预测能力指数
        public int BackSpinPredictLevel;

        /// 对侧旋的预测能力指数
        public int SideSpinPredictLevel;


        public AIPlayer(int aL, int wL, int dL, int tL, int bL, int sL)
        {
            AdventureLevel = aL;
            WindPredictLevel = wL;
            DirectionAccuracyLevel = dL;
            TopSpinPredictLevel = tL;
            BackSpinPredictLevel = bL;
            SideSpinPredictLevel = sL;
        }

        public AIPlayer()
        {
            AdventureLevel = 50;
            WindPredictLevel = 100;
            DirectionAccuracyLevel = 100;
            TopSpinPredictLevel = 100;
            BackSpinPredictLevel = 100;
            SideSpinPredictLevel = 100;
        }
    }
}

#endif