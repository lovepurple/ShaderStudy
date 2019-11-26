#if AI_TEST
using System;
using UnityEngine;

namespace Assets.Scripts.ai
{
    /// <summary>
    /// 拟合物理规律，此类中的物理规律都是通过在平面上做实验得到的数据，再通过非线性拟合得出的大致的函数式，有一定的误差
    /// </summary>
    public class AISpinFitFunction
    {
        private static float KXmYn(float k, float m, float n, float x, float y)
        {
            float z = Mathf.Pow(x, m) * Mathf.Pow(y, n) * k;
            return z;
        }

        private static float Kx(float k, float x)
        {
            return k * x;
        }

        private static float Y_k(float k, float y)
        {
            if (k == 0)
            {
                return 0;
            }

            return y / k;
        }

        /// <summary>
        /// 计算侧旋导致的球前进方向上偏转的角度
        /// </summary>
        /// <param name="dis">击球点和落点的距离</param>
        /// <param name="sideSpin">侧旋值</param>
        /// <param name="clubEmitCoefficient">球杆抛角</param>
        /// <returns></returns>
        public static float CalcSideSpinAngle(float dis, float sideSpin, float clubEmitCoefficient)
        {
            float absDis = Mathf.Abs(dis);
            float absSpin = Mathf.Abs(sideSpin);
            float angle;
            if (clubEmitCoefficient < 0.3f)
            {
                // 使用0.26的抛角拟合的数据
                if (absDis < 150f)
                {
                    if (absSpin < 5f)
                    {
                        // 方差 Ss = 0.0466649779937
                        angle = KXmYn(0.7428799f, 0.2400052f, 0.9834190f, absDis, absSpin);
                    }
                    else
                    {
                        // 方差 Ss = 0.186310737609
                        angle = KXmYn(1.1974131f, 0.2132272f, 0.7533394f, absDis, absSpin);
                    }
                }
                else
                {
                    if (absSpin < 5f)
                    {
                        // 方差 Ss = 0.0741570398714
                        angle = KXmYn(0.0989051f, 0.6505066f, 0.9699433f, absDis, absSpin);
                    }
                    else
                    {
                        // 方差 Ss = 0.162446009579
                        angle = KXmYn(0.334581f, 0.5352329f, 0.5979879f, absDis, absSpin);
                    }
                }
            }
            else
            {
                // 使用0.34的抛角拟合的数据
                if (absDis < 150f)
                {
                    if (absSpin < 5f)
                    {
                        // 方差 Ss = 0.150302670015
                        angle = KXmYn(0.9018338f, 0.28732030f, 0.9788978f, absDis, absSpin);
                    }
                    else
                    {
                        // 方差 Ss = 0.514718418316
                        angle = KXmYn(1.663972f, 0.2450286f, 0.70314002f, absDis, absSpin);
                    }
                }
                else
                {
                    if (absSpin < 5f)
                    {
                        // 方差 Ss = 0.480882931787
                        angle = KXmYn(0.0749757f, 0.7950360f, 0.9519206f, absDis, absSpin);
                    }
                    else
                    {
                        // 方差 Ss = 0.488329255383
                        angle = KXmYn(0.477345f, 0.5901679f, 0.4814066f, absDis, absSpin);
                    }
                }
            }

            // 侧旋角度修正
            if (absSpin > 5)
            {
                angle = angle + (absSpin - 5f) * 0.08f * (90f - angle);
            }

            int sign = sideSpin >= 0 ? 1 : -1;
            angle *= sign;
            return angle;
        }


        /// <summary>
        /// 计算上旋导致的辅助线除了第一段（起点到落点）外，每段辅助线大致与不加上旋时的长度的比值
        /// </summary>
        /// <param name="dis">起点到落点的距离</param>
        /// <param name="topSpin">上旋值</param>
        /// <param name="clubEmitCoefficient">球杆抛角</param>
        /// <returns></returns>
        public static float CalcTopSpinRatio(float dis, float topSpin, float clubEmitCoefficient)
        {
            float ratio;
            if (clubEmitCoefficient < 0.3f)
            {
                // 使用0.26的抛角拟合的数据
                // 分段拟合
                if (dis < 150f)
                {
                    if (topSpin < 4f)
                    {
                        // 方差 Ss = 2.79465404615e-05
                        ratio = KXmYn(0.00176033f, 0.7039136f, 1.0537396f, dis, topSpin);
                    }
                    else if (topSpin < 7f)
                    {
                        // 方差 Ss = 4.82673128339e-05
                        ratio = KXmYn(0.00183904f, 0.67384384f, 1.10998595f, dis, topSpin);
                    }
                    else
                    {
                        // 方差 Ss = 0.000116508016275
                        ratio = KXmYn(0.001451011f, 0.69148069f, 1.1899987f, dis, topSpin);
                    }
                }
                else
                {
                    if (topSpin < 4f)
                    {
                        // 方差 Ss = 7.64355267096e-05
                        ratio = KXmYn(0.0004308679f, 0.97989283f, 1.03902903f, dis, topSpin);
                    }
                    else if (topSpin < 7f)
                    {
                        // 方差 Ss = 0.000268930041519
                        ratio = KXmYn(0.0002367597f, 1.06035148f, 1.15987592f, dis, topSpin);
                    }
                    else
                    {
                        if (dis < 250f)
                        {
                            // 方差 Ss = 5.28087864017e-05
                            ratio = KXmYn(0.0003214622f, 0.96769921f, 1.25101636f, dis, topSpin);
                        }
                        else
                        {
                            // 方差 Ss = 0.000160211902079
                            ratio = KXmYn(3.770329e-5f, 1.33001334f, 1.3175058f, dis, topSpin);
                        }
                    }
                }
            }
            else
            {
                // 使用0.34的抛角拟合的数据
                // 分段拟合
                if (dis < 200f)
                {
                    if (topSpin < 4f)
                    {
                        // 方差 Ss = 0.000127627179567
                        ratio = KXmYn(0.00186549f, 0.7885429f, 1.04192235f, dis, topSpin);
                    }
                    else if (topSpin < 6f)
                    {
                        // 方差 Ss = 0.000419116642485
                        ratio = KXmYn(0.00131042f, 0.847635f, 1.0946601f, dis, topSpin);
                    }
                    else
                    {
                        if (dis < 100f)
                        {
                            // 方差 Ss = 0.000118866064154
                            ratio = KXmYn(0.00247512f, 0.68077659f, 1.1387324f, dis, topSpin);
                        }
                        else
                        {
                            // 方差 Ss = 0.000277555462862
                            ratio = KXmYn(0.0003747638f, 1.05783837f, 1.2043254f, dis, topSpin);
                        }
                    }
                }
                else
                {
                    if (topSpin < 4f)
                    {
                        // 方差 Ss = 0.000885150371684
                        ratio = KXmYn(1.0130722f, -0.42953184f, 1.1612152f, dis, topSpin);
                    }
                    else if (topSpin < 7f)
                    {
                        if (dis < 275f)
                        {
                            // 方差 Ss = 0.000301660887547
                            ratio = KXmYn(0.04914016f, 0.11091554f, 1.31530612f, dis, topSpin);
                        }
                        else
                        {
                            // 方差 Ss = 0.000535389964175
                            ratio = KXmYn(0.078306f, -0.1012247603f, 1.7430564f, dis, topSpin);
                        }
                    }
                    else
                    {
                        // 方差 Ss = 0.000753101094774
                        ratio = KXmYn(0.003050319f, 0.5426913f, 1.5172466f, dis, topSpin);
                    }
                }
            }

            return ratio;
        }


        /// <summary>
        /// 计算下旋导致的辅助线除了第一段（起点到落点）外，每段辅助线大致与不加上旋时的长度的比值
        /// </summary>
        /// <param name="dis">起点到落点的距离</param>
        /// <param name="backSpin">下旋值</param>
        /// <param name="clubEmitCoefficient">球杆抛角</param>
        /// <returns></returns>
        public static float CalcBackSpinRatio(float dis, float backSpin, float clubEmitCoefficient)
        {
            float ratio;
            float absBackSpin = -backSpin;
            if (clubEmitCoefficient < 0.3f)
            {
                // 使用0.26的抛角拟合的数据
                // 分段拟合
                if (dis < 325f)
                {
                    if (absBackSpin < 0.0307692f * dis)
                    {
                        // Ss = 3.71421421118e-05
                        ratio = KXmYn(7.778428e-6f, 1.556041f, 1.0660202f, dis, absBackSpin);
                    }
                    else
                    {
                        // Ss = 0.000827971603754
                        ratio = KXmYn(1.082239e-5f, 1.5510384f, 1.0f, dis, absBackSpin);
                    }
                }
                else
                {
                    if (absBackSpin < 0.85f)
                    {
                        // Ss = 5.70308222315e-05
                        ratio = KXmYn(1.1843582e-6f, 1.8806819f, 1.0488774f, dis, absBackSpin);
                    }
                    else
                    {
                        // Ss = 0.00022810927532
                        ratio = KXmYn(0.00175316f, 0.81477201f, 0.5614675f, dis, absBackSpin);
                    }
                }
            }
            else
            {
                // 使用0.34的抛角拟合的数据
                // 分段拟合
                if (dis < 155f)
                {
                    if (absBackSpin < 6f)
                    {
                        // Ss = 1.59907770038e-05
                        ratio = KXmYn(0.0004981162f, 0.9263264f, 1.0763706f, dis, absBackSpin);
                    }
                    else
                    {
                        // Ss = 0.000225612877594
                        ratio = KXmYn(0.0006563812f, 0.7937468f, 1.257993f, dis, absBackSpin);
                    }
                }
                else
                {
                    if (absBackSpin < 7.3f)
                    {
                        // Ss = 0.000552682700998
                        ratio = KXmYn(0.01232034f, 0.3435977f, 1.0417394f, dis, absBackSpin);
                    }
                    else
                    {
                        // Ss = 0.000819584179313
                        ratio = KXmYn(0.2832182f, 0.1109268f, 0.1313474f, dis, absBackSpin);
                    }
                }
            }

            return ratio;
        }


        /// <summary>
        /// 计算上旋在峰值处产生的y方向上的差值
        /// </summary>
        /// <param name="segmentIndex">第几段，取值为1或2，1代表从起点到落点，2代表落点后的第一个反弹（目前只考虑第一段的碰撞故只有1）</param>
        /// <param name="spin">上旋值，正数</param>
        /// <param name="clubEmitCoefficient">抛角</param>
        /// <returns></returns>
        public static float CalcTopSpinHeightDiff(int segmentIndex, float spin, float clubEmitCoefficient)
        {
            float diffH = 0f;
            if (clubEmitCoefficient < 0.3f)
            {
                if (segmentIndex == 1)
                {
                    diffH = Kx(1.127385f, spin);
                }
                else
                {
                    diffH = Kx(0.1166257f, spin);
                }
            }
            else
            {
                if (segmentIndex == 1)
                {
                    diffH = Kx(1.65834f, spin);
                }
                else
                {
                    diffH = Kx(0.1465245f, spin);
                }
            }

            return diffH;
        }

        /// <summary>
        /// 计算下旋在峰值处产生的y方向上的差值
        /// </summary>
        /// <param name="segmentIndex">第几段，取值为1或2，1代表从起点到落点，2代表落点后的第一个反弹（目前只考虑第一段的碰撞故只有1）</param>
        /// <param name="spin">下旋值，正数</param>
        /// <param name="clubEmitCoefficient">抛角</param>
        /// <returns></returns>
        public static float CalcBackSpinHeightDiff(int segmentIndex, float spin, float clubEmitCoefficient)
        {
            float diffH = 0f;
            float absSpin = -spin;
            if (clubEmitCoefficient < 0.3f)
            {
                if (segmentIndex == 1)
                {
                    diffH = Kx(2.1843472f, absSpin);
                }
                else
                {
                    diffH = Kx(0.1289461f, absSpin);
                }
            }
            else
            {
                if (segmentIndex == 1)
                {
                    diffH = Kx(3.071252f, absSpin);
                }
                else
                {
                    diffH = Kx(0.1606868f, absSpin);
                }
            }

            return diffH;
        }


        /// <summary>
        /// 通过高度差反推上旋值
        /// </summary>
        /// <param name="segmentIndex">第几段，取值为1或2，1代表从起点到落点，2代表落点后的第一个反弹（目前只考虑第一段的碰撞故只有1）</param>
        /// <param name="diff">上旋产生的峰值处y方向上的高度差</param>
        /// <param name="clubEmitCoefficient">抛角</param>
        /// <returns></returns>
        public static float CalcTopSpinByHeightDiff(int segmentIndex, float diff, float clubEmitCoefficient)
        {
            float spin = 0f;
            if (clubEmitCoefficient < 0.3f)
            {
                if (segmentIndex == 1)
                {
                    spin = Y_k(1.127385f, diff);
                }
                else
                {
                    spin = Y_k(0.1166257f, diff);
                }
            }
            else
            {
                if (segmentIndex == 1)
                {
                    spin = Y_k(1.6583403f, diff);
                }
                else
                {
                    spin = Y_k(0.1465245f, diff);
                }
            }

            return spin;
        }


        /// <summary>
        /// 通过高度差反推下旋值
        /// </summary>
        /// <param name="segmentIndex">第几段，取值为1或2，1代表从起点到落点，2代表落点后的第一个反弹（目前只考虑第一段的碰撞故只有1）</param>
        /// <param name="diff">下旋产生的峰值处y方向上的高度差</param>
        /// <param name="clubEmitCoefficient">抛角</param>
        /// <returns></returns>
        public static float CalcBackSpinByHeightDiff(int segmentIndex, float diff, float clubEmitCoefficient)
        {
            float spin = 0f;
            if (clubEmitCoefficient < 0.3f)
            {
                if (segmentIndex == 1)
                {
                    spin = Y_k(2.1843472f, diff);
                }
                else
                {
                    spin = Y_k(0.1289461f, diff);
                }
            }
            else
            {
                if (segmentIndex == 1)
                {
                    spin = Y_k(3.071252f, diff);
                }
                else
                {
                    spin = Y_k(0.160687f, diff);
                }
            }

            return -spin;
        }


        /// <summary>
        /// 计算侧旋导致的峰值处往左右偏移的距离
        /// </summary>
        /// <param name="sideSpin">侧旋</param>
        /// <param name="clubEmitCoefficient">球杆抛角</param>
        /// <returns></returns>
        public static float CalcSideSpinSidePeakDiff(float sideSpin, float clubEmitCoefficient)
        {
            float diff;
            if (clubEmitCoefficient < 0.3f)
            {
                diff = Kx(1.48301f, sideSpin);
            }
            else
            {
                diff = Kx(2.1105f, sideSpin);
            }

            return diff;
        }

        /// <summary>
        /// 通过峰值处左右偏差距离反推侧旋值
        /// </summary>
        /// <param name="diff">左右偏差距离</param>
        /// <param name="clubEmitCoefficient">球杆抛角</param>
        /// <returns></returns>
        public static float CalcSideSpinBySidePeakDiff(float diff, float clubEmitCoefficient)
        {
            float spin;
            if (clubEmitCoefficient < 0.3f)
            {
                spin = Y_k(1.48301f, diff);
            }
            else
            {
                spin = Y_k(2.1105f, diff);
            }

            return spin;
        }

        private static float RandomErrorFactor(int playerLv)
        {
            float errorFactor = 1f + Mathf.Pow(-1, UnityEngine.Random.Range(0, 2)) * UnityEngine.Random.Range(0, 0.5f) *
                                (1f - playerLv / 100f);
            return errorFactor;
        }
    }
}
#endif