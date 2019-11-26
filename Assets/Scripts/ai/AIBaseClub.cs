#if AI_TEST

namespace Assets.Scripts.ai
{
    /// <summary>
    /// 球杆抽象基类
    /// </summary>
    public abstract class AIBaseClub
    {
        public int Id;

        public float Line;

        public float MaxDistance;

        // 类型
        public int Type;

        // 等级
        public int Level;

        // 力度
        public int Power;

        // 精准度
        public int Accuracy;

        // 上下旋范围
        public float TopBackSpin;

        // 左右旋范围
        public float SideSpin;

        public float ClubEmitCoefficient;

        public abstract float CalcPowerRange(bool isAdventure, int adventureLevel);
    }
}

#endif