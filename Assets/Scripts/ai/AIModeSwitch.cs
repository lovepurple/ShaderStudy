#if AI_TEST


namespace Assets.Scripts.ai
{
    /// <summary>
    /// 几种特殊模式的开关
    /// </summary>
    public class AIModeSwitch
    {
        /// <summary>
        /// 是否为Par3场的开关
        /// </summary>
        public const bool IsInPar3Mode = false;

        /// <summary>
        /// 是否有需要排除最终预测停球点停在特殊区域的情况
        /// </summary>
        public const bool IsInExcludeUnsafeAreaMode = false;

        /// <summary>
        /// 是否有需要排除穿过凸起地形的情况
        /// </summary>
        public const bool IsInExcludeCrossMountainAreaMode = false;

        /// <summary>
        /// 是否需要排除落点落在特殊区域的情况
        /// </summary>
        public const bool IsInExcludeUnsafeDropAreaMode = true;

        /// <summary>
        /// 需要强制选最大旋度
        /// </summary>
        public const bool IsMaximizeSpinMode = true;

        /// <summary>
        /// 是否需要把长草区列入危险区域判断
        /// </summary>
        public static bool IsInExcludeRoughDropPointMode
        {
            get { return !IsInPar3Mode; }
        }
    }
}

#endif