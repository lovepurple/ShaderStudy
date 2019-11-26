#if AI_TEST
namespace Assets.Scripts.ai
{
    /// <summary>
    /// 球杆配置json文件对应的球杆数据模型
    /// </summary>
    public class AIJsonClub
    {
        /// <summary>
        /// 球杆ID
        /// </summary>
        public int Id;


        /// <summary>
        /// 辅助线长度
        /// </summary>
        public float Line;

        /// <summary>
        /// 球杆类型
        /// 1-开球杆 2-3Wood(三号木杆） 3-长铁杆 4-短铁杆 5-挖起杆 6-沙坑杆
        /// </summary>
        public int Type;

        /// <summary>
        /// 球杆等级
        /// </summary>
        public int Level;

        /// <summary>
        /// 精准度
        /// </summary>
        public int Accuracy;

        /// <summary>
        /// 前后旋能力范围
        /// </summary>
        public float TopBackSpin;

        /// <summary>
        /// 侧旋能力范围
        /// </summary>
        public float SideSpin;

        /// <summary>
        /// 球杆抛角，同Angle，仅作兼容性
        /// </summary>
        public float ClubEmitCoefficient;

        private int _force;

        /// <summary>
        /// 球杆力度
        /// </summary>
        public int Force
        {
            set { _force = value; }
            get { return _force; }
        }

        private float _angle;

        /// <summary>
        /// 球杆抛角，同ClubEmitCoefficient，仅作兼容性
        /// </summary>
        public float Angle
        {
            set
            {
                _angle = value;
                ClubEmitCoefficient = value;
            }
            get { return _angle; }
        }

        /// <summary>
        /// 能击打的最大范围（即力度）
        /// </summary>
        public float MaxDistance
        {
            get { return _force; }
        }


        /// <summary>
        /// 计算Power > 1时能击打的最大范围
        /// </summary>
        /// <param name="isAdventure">是否需要冒险</param>
        /// <param name="adventureLevel">玩家冒险等级指数</param>
        /// <returns></returns>
        public float CalcPowerRange(bool isAdventure, int adventureLevel)
        {
            float maxAdventureRange = 0.1f * ((float) adventureLevel - 60f) / 40f;
            float powerRange = 0f;
            if (isAdventure)
            {
                powerRange = Force * (1f + maxAdventureRange);
            }
            else
            {
                powerRange = Force;
            }

            return powerRange;
        }


        /// <summary>
        /// 深拷贝一个球杆
        /// </summary>
        /// <returns></returns>
        public AIJsonClub Clone()
        {
            AIJsonClub newClub = new AIJsonClub();
            newClub.Type = Type;
            newClub.Force = Force;
            newClub.Accuracy = Accuracy;
            newClub.SideSpin = SideSpin;
            newClub.TopBackSpin = TopBackSpin;
            newClub.Line = Line;
            newClub.Level = Level;
            newClub.Id = Id;
            newClub.Angle = Angle;
            return newClub;
        }
    }
}

#endif