#if AI_TEST
using System;
using System.Collections.Generic;
using SPhysics;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.ai
{
    public class AIGuideLineHelper
    {
        /// 辅助线穿过球洞的偏差允许值
        private const float CrossGoalE = 0.6f;

        /// 当请求的辅助线都没有弹跳完整时，假设滚动的距离
        private const float ScrollError = 5.0f;

        /// 旋度计算精度 
        private const float SpinPercision = 0.5f;

        /// 弹跳距离阈值，小于该阈值就不再弹跳
        private const float BounceThreshold = 0.2f;

        /// 辅助线方向上的误差因子
        private const float ErrorFactor = 2.0f;

        /// 预测球道滚动的距离参数
        private const float ScrollFactor = -0.4f;

        /// 预测长草滚动的距离参数
        private const float RoughScrollFactor = -0.6f;


        /// 预测沙坑滚动的距离参数
        private const float BunkerScrollFactor = -0.8f;

        /// 下旋情况下预测滚动的距离参数
        private const float BackSpinScrollDis = 0.5f;

        /// 下旋滚动阈值
        private const float BackSpinBounceThreshold = 5f;

        /// 检测碰撞范围时遍历从起点到落点的第一段辅助线的步长
        private const int FirstSegmentScanStep = 3;

        /// 采用原始辅助线给出的滚动长度进行估计时滚动的系数
        private const float OriGuideLineScrollFactor = 2f;

        /// 弹跳距离突变导致无法滚动的阈值参数
        private const float ScrollRatioThreshold = 0.1f;

        /// 最后一段弹跳如果从长草弹到果岭此时快速滚动的系数
        private const float Rough2GreenScrollFactor = 3f;


        /// <summary>
        /// 根据物理得到的辅助线信息进行计算，得到最终该落点带合适的旋度以及最终停球区域等信息的AI辅助线
        /// </summary>
        /// <param name="shotPos">发球点</param>
        /// <param name="aiLine">物理辅助线结构体</param>
        /// <param name="guideLine">辅助线长度</param>
        /// <param name="dropPt">落点</param>
        /// <param name="terrain">地图</param>
        /// <param name="clubTopBackSpin">球杆能打的前后旋范围</param>
        /// <param name="clubSideSpin">球杆能打的侧旋范围</param>
        /// <param name="clubEmitCoefficient">球杆抛角</param>
        /// <param name="player">玩家模型</param>
        /// <returns></returns>
        public static AIGuideLine CalcPredictGuideLine(Vector2 shotPos, SPhysics.AiLine aiLine, float guideLine,
            Vector2 dropPt, AITerrain terrain, float clubTopBackSpin, float clubSideSpin, float clubEmitCoefficient,
            AIPlayer player, bool isMainTestClub)
        {
            float completeGuideLineLen = 0;

            bool isCompleteBounce = false;

            int finalState = 0;

            int bounceCount = 0;

            float decrementFactor = 0.15f;

            Vector2 goalPos = terrain.GoalPosition;

            if (aiLine.bounceList.Count == 0 || aiLine.fringeDrop)
            {
                return AIGuideLine.CreateInvalidGuideLine();
            }

            bool isCollision = CheckTrunkCollision(terrain, aiLine.positionList);

            // 1.处理屋里返回的数据，预测不带旋度的原始辅助线
            List<Vector2> oriPoints = PreProcessGuideLinePoints(shotPos, aiLine, guideLine, dropPt,
                out isCompleteBounce, out finalState, out completeGuideLineLen);

            List<AIGuideLineSegment> oriGuideLine = PredictOriginalGuideLine(terrain, oriPoints, guideLine,
                isCompleteBounce, completeGuideLineLen, finalState, decrementFactor, out bounceCount);


            // 2. 计算出错或者没有符合要求的辅助线， 做无效处理
            if (oriGuideLine == null)
            {
                return AIGuideLine.CreateInvalidGuideLine();
            }

            // 3.穿过树叶或撞树且球杆不支持旋度时也不该打的
            if ((aiLine.throughLeaf || finalState == 2 || isCollision) && clubTopBackSpin <= 0 && clubSideSpin <= 0)
            {
                return AIGuideLine.CreateInvalidGuideLine();
            }

            // 排除特殊区域开关开启是 
            if (AIModeSwitch.IsInExcludeCrossMountainAreaMode)
            {
                foreach (AIGuideLineSegment segment in oriGuideLine)
                {
                    if (segment.IsPredictedPoint)
                    {
                        Vector2 start = segment.StartPt;
                        Vector2 diff = segment.EndPt - segment.StartPt;
                        bool hasCrossMountainArea = terrain.IsLineCrossMountainArea(start, diff);
                        if (hasCrossMountainArea)
                        {
                            return AIGuideLine.CreateInvalidGuideLine();
                        }
                    }
                }
            }
            
            AIGuideLine resultGuideLine;

            if (AIModeSwitch.IsMaximizeSpinMode && AIStrategy.isCurActionMainTestClub && isMainTestClub)
            {
                // 击球方向上的误差跟估计的弹跳次数正相关
                float errorWidth = 0;
                if (!isCompleteBounce || bounceCount == 0)
                {
                    errorWidth = ScrollError;
                }
                else
                {
                    errorWidth = ErrorFactor * bounceCount;
                }
                float errorHeight = ErrorFactor * (2f - player.DirectionAccuracyLevel / 100f);
                
                List <AIGuideLineSegment> spinGuideLine = new List<AIGuideLineSegment>();
                resultGuideLine = new AIGuideLine();
                switch (AIStrategy.MainTestSpinType)
                {
                    case TestSpinType.TestSpinTypeLeftSpin:
                        spinGuideLine = CalcGuideLineWithSideSpin(oriGuideLine, finalState, clubSideSpin, terrain,
                            decrementFactor, clubEmitCoefficient, isCompleteBounce);
                        resultGuideLine.SideSpin = clubSideSpin;
                        resultGuideLine.TopBackSpin = 0;
                        break;
                    case TestSpinType.TestSpinTypeRightSpin:
                        spinGuideLine = CalcGuideLineWithSideSpin(oriGuideLine, finalState, -clubSideSpin, terrain,
                            decrementFactor, clubEmitCoefficient, isCompleteBounce);
                        resultGuideLine.SideSpin = clubSideSpin;
                        resultGuideLine.TopBackSpin = 0;
                        break;
                    case TestSpinType.TestSpinTypeTopSpin:
                        spinGuideLine = CalcGuideLineWithTopBackSpin(oriGuideLine, finalState, clubTopBackSpin, terrain,
                            decrementFactor, clubEmitCoefficient, isCompleteBounce);
                        resultGuideLine.SideSpin = 0;
                        resultGuideLine.TopBackSpin = clubTopBackSpin;
                        break;
                    case TestSpinType.TestSpinTypeBackSpin:
                        spinGuideLine = CalcGuideLineWithTopBackSpin(oriGuideLine, finalState, clubTopBackSpin, terrain,
                            decrementFactor, clubEmitCoefficient, isCompleteBounce);
                        resultGuideLine.SideSpin = 0;
                        resultGuideLine.TopBackSpin = clubTopBackSpin;
                        break;
                }
                
                resultGuideLine.FinalGuideLine = spinGuideLine;
                AIGuideLineSegment lastSegment = spinGuideLine[spinGuideLine.Count - 1];
                AIGuideLineSegment firstSegment = spinGuideLine[0];
                Vector2 dropPoint = firstSegment.EndPt;
                // bool isCompleteBounce = lastSegment.type != AIGuideLineSegmentType.StraightLineSegment;
                // AIMapRegion region = estimateRectStopRegion(terrain, dropPoint, lastSegment.startPt, lastSegment.endPt, isCompleteBounce, getGuideLinePredictBounceCount(sGuideLine));                        
                AIMapRegion region = EstimateRectStopRegion(terrain, dropPoint, lastSegment.StartPt,
                    lastSegment.EndPt, errorWidth, errorHeight);

                region.TopBackSpin = resultGuideLine.TopBackSpin;
                region.SideSpin = resultGuideLine.SideSpin;
                resultGuideLine.FinalRegion = region;
            }
            else
            {
                // 4.计算能打上下旋的范围
                AISet topSpinRange = null;
                AISet sideSpinRange = null;
                if (aiLine.peakList != null && aiLine.peakList.Count > 0)
                {
                    topSpinRange = CalcTopBackSpinRange(terrain, aiLine.positionList, aiLine.peakList[0], clubTopBackSpin,
                        clubEmitCoefficient);
                    sideSpinRange = CalcSideSpinRange(terrain, aiLine.positionList, aiLine.peakList[0], clubSideSpin,
                        clubEmitCoefficient);
                }

                if (topSpinRange == null || topSpinRange.Ranges.Count <= 0)
                {
                    topSpinRange = new AISet();
                    topSpinRange.Ranges.Add(new AIRange(0, 0));
                }

                if (sideSpinRange == null || sideSpinRange.Ranges.Count <= 0)
                {
                    sideSpinRange = new AISet();
                    sideSpinRange.Ranges.Add(new AIRange(0, 0));
                }

                // 5. 即便能打侧旋和上下旋，结果选不出合适的，那依旧是撞树或者穿树叶的，无效处理
                if (topSpinRange.IsEmpty() && sideSpinRange.IsEmpty() &&
                    (finalState == 2 || aiLine.throughLeaf || isCollision))
                {
                    return AIGuideLine.CreateInvalidGuideLine();
                }

                // 产生球杆能打出的上下旋和侧旋的所有组合，优化
                List<AIGuideLine> guideLineList = generateSpinGuideLineList(terrain, oriGuideLine, finalState, topSpinRange,
                    sideSpinRange, clubEmitCoefficient, isCompleteBounce, bounceCount, player, decrementFactor);
            

                resultGuideLine = AIModeSwitch.IsInPar3Mode
                    ? OptimalGuideLinePar3(guideLineList)
                    : OptimalGuideLine(guideLineList);
            }
            
            

            // 计算出一些其他相关的值。只在最优的辅助线计算，以节省计算量
            if (resultGuideLine.FinalRegion == null)
            {
                resultGuideLine.FinalRegion = AIMapRegion.CreateInvalidRegion();
            }

            if (oriGuideLine.Count > 0)
            {
                Vector2 lastPt = oriGuideLine[oriGuideLine.Count - 1].EndPt;
                resultGuideLine.FinalRegion.DisToGoalNotSpin = Vector2.Distance(goalPos, lastPt);
            }
            else
            {
                resultGuideLine.FinalRegion.DisToGoalNotSpin = 10000f;
            }

            List<AIGuideLineSegment> segementList = resultGuideLine.FinalGuideLine;
            if (segementList != null && segementList.Count > 0)
            {
                AIGuideLineSegment firstSegment = segementList[0];
                AIGuideLineSegment lastSegment = segementList[segementList.Count - 1];
                resultGuideLine.FinalRegion.SideDisFromDropPtToDangerZone =
                    CalcNearestSideDangerZoneDistance(terrain, firstSegment.StartPt, firstSegment.EndPt);
                resultGuideLine.FinalRegion.SideDisFromCenterToDangerZone =
                    CalcNearestSideDangerZoneDistance(terrain, lastSegment.StartPt, lastSegment.EndPt);
                resultGuideLine.FinalRegion.FrontDisFromCenterToDangerZone =
                    CalcFrontNearestDangerZoneDistance(terrain, firstSegment.StartPt, firstSegment.EndPt);
                if (!AITerrain.IsGroundTypeOut(lastSegment.EndGroundType))
                {
                    resultGuideLine.FinalState = 1;
                }
                else
                {
                    resultGuideLine.FinalState = 5;
                }

                if (finalState == 4)
                {
                    resultGuideLine.HasGuideLineCrossGoal = true;
                }
                else
                {
                    resultGuideLine.HasGuideLineCrossGoal = GuideLineHasCrossGoal(segementList, goalPos);
                }
            }

            return resultGuideLine;
        }

        
        /// <summary>
        /// 根据与物理约定好的数据格式，预处理物理返回的辅助线信息，转化为Ai要用的数据形式
        /// 注：PhysicsManager返回的辅助线落点的bounceList的格式：
        /// （1）最后一个Vector3的z分量表示最终状态，如0代表在空中，1代表在地上，5代表出界，需特殊提取出来
        /// （2）当请求的辅助线长度为整数值，且弹跳次数能达到请求的长度时，将重复最后一个落点。如：请求长度为2的辅助线，则正常应返回（落点1， 落点2， 落点2，标志位），长度为4
        /// （3）当请求的辅助线长度非整数值，且弹跳次数能达到请求的长度时，不重复最后一个落点。如：请求长度为1.4的辅助线，则正常返回（落点1， 空中点（时间上是0.4位置而不是空间上），标志位为0）），长度为3
        /// （4）当弹跳次数不能达到请求的长度时，返回到最后一个落点且带一小段滚动。如:请求长度为3.5，但是在第二次弹跳就开始滚动，则返回（落点1， 落点2， 滚动路径上某点3， 标志位为1），长度为4
        /// </summary>
        /// <param name="shotPos">发球点</param>
        /// <param name="aiLine">物理辅助线结构体</param>
        /// <param name="guideLine">辅助线长</param>
        /// <param name="dropPt">落点</param>
        /// <param name="isCompleteBounce">计算判断该辅助线是否提前退出弹跳而开始滚动</param>
        /// <param name="finalState">辅助线最后的状态</param>
        /// <param name="completeGuideLineLen">实际辅助线长度（可能比请求的短因为可能提前开始滚动）</param>
        /// <returns></returns>
        private static List<Vector2> PreProcessGuideLinePoints(Vector2 shotPos, SPhysics.AiLine aiLine, float guideLine,
            Vector2 dropPt, out bool isCompleteBounce,
            out int finalState, out float completeGuideLineLen)
        {
            List<Vector3> list = aiLine.bounceList;
            isCompleteBounce = true;
            int specialPtCount = 0;
            int count = list.Count;

            List<Vector2> oriPoints = new List<Vector2>();

            // 发球点加进去
            oriPoints.Add(shotPos);
            // 需要处理的特殊点，数组最后一个点是球最后的状态
            finalState = (int) ((Vector3) list[count - 1]).z;
            specialPtCount++;

            if (AIMathf.DemicalPart(guideLine) > 0)
            {
                // 说明存在小数点，则若最终状态不在空中说明是没有弹完

                // 此时在空中才算弹完
                if (finalState != 0)
                {
                    isCompleteBounce = false;
                }
                else
                {
                    isCompleteBounce = true;
                }
            }
            else
            {
                // 说明不存在小数点
                if (count >= 3)
                {
                    Vector3 last2 = list[count - 2];
                    Vector3 last3 = list[count - 3];
                    if (Mathf.Abs(last2.x - last3.x) < 0.0000001f && Mathf.Abs(last2.z - last3.z) < 0.00000001f)
                    {
                        // 如果弹完了，则最后的两个位置重复
                        if (count < guideLine - 1)
                        {
                            isCompleteBounce = false;
                        }
                        else
                        {
                            isCompleteBounce = true;
                        }

                        specialPtCount++;
                    }
                    else
                    {
                        // 如果没有弹完。则最后会伸出一段滚动，此时不重复
                        isCompleteBounce = false;
                    }
                }
                else
                {
                    // 第一步就没弹起来的情况
                    isCompleteBounce = false;
                }
            }

            // 把不是特殊点的点放进数组
            for (int i = 0; i < count - specialPtCount; i++)
            {
                Vector3 pt = list[i];
                Vector2 pt2 = new Vector2(pt.x, pt.z);
                if (i == 0)
                {
                    if (pt2 != dropPt)
                    {
                        oriPoints.Add(pt2);
                    }
                }
                else
                {
                    oriPoints.Add(pt2);
                }
            }

            // 计算完成的弹跳次数
            int aiGuideLinePtCount = oriPoints.Count - 1;
            if (AIMathf.DemicalPart(guideLine) > 0)
            {
                // 特殊点：落点，最后的点，无论是在空中的还是在滚的
                completeGuideLineLen = aiGuideLinePtCount - 2;
            }
            else
            {
                if (isCompleteBounce)
                {
                    // 特殊点只有落点
                    completeGuideLineLen = aiGuideLinePtCount - 1;
                }
                else
                {
                    // 特殊点：落点和最后滚动的
                    completeGuideLineLen = aiGuideLinePtCount - 2;
                }
            }

            return oriPoints;
        }


        /// <summary>
        /// 根据经过预处理的物理辅助线信息，将辅助线进行延长。此时辅助线不带任何旋度，以此辅助线作为带旋度的辅助线的基准
        /// 需分两种情况，即是否没有达到请求辅助线长度就提前开始滚动。若提前开始滚动，则只需预测滚动长度即可；若没有提前开始滚动，则需预测会继续弹跳次数，并预测滚动长度
        /// </summary>
        /// <param name="terrain">地图</param>
        /// <param name="oriPoints">物理辅助线落点数组</param>
        /// <param name="idealGuideLineLen">请求辅助线时的长度</param>
        /// <param name="isCompleteBounce">是否提前退出弹跳进入滚动状态</param>
        /// <param name="completeGuideLineLen">实际辅助线长度</param>
        /// <param name="finalState">原始辅助线最终状态码</param>
        /// <param name="decrementFactor">衰减系数</param>
        /// <param name="bounceCount">弹跳次数</param>
        /// <returns></returns>
        private static List<AIGuideLineSegment> PredictOriginalGuideLine(AITerrain terrain, List<Vector2> oriPoints,
            float idealGuideLineLen, bool isCompleteBounce, float completeGuideLineLen, int finalState,
            float decrementFactor, out int bounceCount)
        {
            bounceCount = 0;
            int ptCount = oriPoints.Count;
            if (ptCount < 2)
            {
                return null;
            }

            List<AIGuideLineSegment> oriGuideLine = new List<AIGuideLineSegment>();

            // 辅助线片段
            for (int i = 1; i < oriPoints.Count - 1; i++)
            {
                Vector2 startPt = oriPoints[i - 1];
                Vector2 endPt = oriPoints[i];

                AIGuideLineSegment segment = new AIGuideLineSegment(AIGuideLineSegmentType.ParabolaSegment, startPt,
                    endPt, terrain.GetTerrainTypeAtPoint(endPt), false);
                oriGuideLine.Add(segment);
            }

            Vector2 stopPos;
            Vector2 ptLast = oriPoints[ptCount - 1];
            Vector2 ptLast2 = oriPoints[ptCount - 2];
            if (!isCompleteBounce)
            {
                // 提前结束弹跳，进入滚动，或者出界。只延长滚动部分
                if (finalState == 5)
                {
                    stopPos = ptLast;
                    // 预测的滚动线段
                    AIGuideLineSegment segment = new AIGuideLineSegment(AIGuideLineSegmentType.StraightLineSegment,
                        ptLast2, stopPos, 0, false);
                    oriGuideLine.Add(segment);
                }
                else
                {
                    AIGuideLineSegment segmentScroll;
                    if(oriGuideLine.Count == 0){
                        return oriGuideLine;
                    }
                    AIGuideLineSegment finalBounce = oriGuideLine[oriGuideLine.Count - 1];
                    float scrollDis = 0;
                    if (oriGuideLine.Count == 1)
                    {
                        AIGuideLineSegment shotBounce = oriGuideLine[0];
                        float scrollF = shotBounce.EndGroundType == 2
                            ? RoughScrollFactor
                            : (shotBounce.EndGroundType == 3 ? BunkerScrollFactor : ScrollFactor);
                        scrollDis = Vector2.Distance(shotBounce.StartPt, shotBounce.EndPt) * 0.2f *
                                    (1f + scrollF);

                        stopPos = AIMathf.ExtendLine(finalBounce.StartPt, finalBounce.EndPt, scrollDis);
                        int stopGt = terrain.GetTerrainTypeAtPointByPhysic(stopPos);
                        int lastStartGt = terrain.GetTerrainTypeAtPointByPhysic(finalBounce.StartPt);
                        if (AITerrain.IsGroundTypeRough(lastStartGt) && (stopGt == 4 || stopGt == 5))
                        {
                            scrollDis *= Rough2GreenScrollFactor;
                            stopPos = AIMathf.ExtendLine(finalBounce.StartPt, finalBounce.EndPt, scrollDis);
                        }

                        // 预测的滚动线段
                        segmentScroll = new AIGuideLineSegment(AIGuideLineSegmentType.StraightLineSegment,
                            finalBounce.EndPt, stopPos, terrain.GetTerrainTypeAtPoint(stopPos), true);
                        oriGuideLine.Add(segmentScroll);
                    }
                    else if (oriGuideLine.Count > 1)
                    {
                        AIGuideLineSegment firstBounce = oriGuideLine[1];


                        float firstBounceLen = Vector2.Distance(firstBounce.StartPt, firstBounce.EndPt);
                        int lastStartGt = terrain.GetTerrainTypeAtPointByPhysic(finalBounce.StartPt);
                        int lastEndGt = terrain.GetTerrainTypeAtPointByPhysic(finalBounce.EndPt);
                        int firstStartGt = terrain.GetTerrainTypeAtPointByPhysic(firstBounce.StartPt);
                        int firstEndGt = terrain.GetTerrainTypeAtPointByPhysic(firstBounce.EndPt);

                        if (oriGuideLine.Count > 2 && Vector2.Distance(oriGuideLine[2].StartPt, oriGuideLine[2].EndPt) <
                            firstBounceLen * ScrollRatioThreshold)
                        {
                            // 极特殊不滚动的情况
                            scrollDis = BackSpinScrollDis;
                            stopPos = AIMathf.ExtendLine(finalBounce.StartPt, finalBounce.EndPt, scrollDis);
                            // 预测的滚动线段
                            segmentScroll = new AIGuideLineSegment(AIGuideLineSegmentType.StraightLineSegment,
                                finalBounce.EndPt, stopPos, terrain.GetTerrainTypeAtPoint(stopPos), true);
                            oriGuideLine.Add(segmentScroll);
                        }
                        else
                        {
                            float scrollF = lastEndGt == 2
                                ? RoughScrollFactor
                                : (lastEndGt == 3 ? BunkerScrollFactor : ScrollFactor);

                            scrollDis = firstBounceLen * (1f + scrollF);


                            float guideLineScrollLen = Vector2.Distance(ptLast, ptLast2);
                            if (scrollDis < guideLineScrollLen)
                            {
                                scrollDis = guideLineScrollLen * OriGuideLineScrollFactor;
                            }

                            stopPos = AIMathf.ExtendLine(finalBounce.StartPt, finalBounce.EndPt, scrollDis);
                            int stopGt = terrain.GetTerrainTypeAtPointByPhysic(stopPos);

                            if ((lastStartGt == 2 && (stopGt == 4 || stopGt == 5))
                                || (firstStartGt == 2 && (firstEndGt == 4 || firstEndGt == 5)))
                            {
                                scrollDis *= Rough2GreenScrollFactor;
                                stopPos = AIMathf.ExtendLine(finalBounce.StartPt, finalBounce.EndPt, scrollDis);
                            }

                            // 预测的滚动线段
                            segmentScroll = new AIGuideLineSegment(AIGuideLineSegmentType.StraightLineSegment,
                                finalBounce.EndPt, stopPos, terrain.GetTerrainTypeAtPoint(stopPos), true);
                            oriGuideLine.Add(segmentScroll);
                        }
                    }
                }
            }
            else
            {
                // 没有提前退出弹跳
                
                // 辅助线最后一段是否为完整的抛物线，如果请求辅助线长度带小数部分，则将其补全，如果为整数，则取此段的一个比值
                float guideLineLastBounce = AIMathf.DemicalPart(idealGuideLineLen);
                float guideLineLastDistance = Vector2.Distance(ptLast2, ptLast);
                float eDistance = guideLineLastBounce > 0
                    ? (guideLineLastDistance / guideLineLastBounce) * 0.8f
                    : guideLineLastDistance;
                
                Vector2 startPt = ptLast2;
                Vector2 endPt;
                if (oriPoints.Count == 2)
                {
                    // 起点到落点
                    AIGuideLineSegment segment = new AIGuideLineSegment(AIGuideLineSegmentType.ParabolaSegment, ptLast2,
                        ptLast, terrain.GetTerrainTypeAtPoint(ptLast), false);
                    oriGuideLine.Add(segment);
                    // 预测的一段
                    eDistance *= 0.2f;
                    endPt = AIMathf.ExtendLine(ptLast2, ptLast, eDistance);
                    segment = new AIGuideLineSegment(AIGuideLineSegmentType.ParabolaSegment, ptLast2, endPt,
                        terrain.GetTerrainTypeAtPoint(endPt), false);
                    oriGuideLine.Add(segment);
                }
                else if (guideLineLastBounce <= 0)
                {
                    endPt = ptLast;
                    AIGuideLineSegment segment = new AIGuideLineSegment(AIGuideLineSegmentType.ParabolaSegment, ptLast2,
                        ptLast, terrain.GetTerrainTypeAtPoint(endPt), false);
                    oriGuideLine.Add(segment);
                }
                else
                {
                    endPt = AIMathf.ExtendLine(ptLast2, ptLast, eDistance - guideLineLastDistance);
                    AIGuideLineSegment segment = new AIGuideLineSegment(AIGuideLineSegmentType.ParabolaSegment, startPt,
                        endPt, terrain.GetTerrainTypeAtPoint(endPt), false);
                    oriGuideLine.Add(segment);
                }

                // 判断出界
                int guideLineFinalGroundType = terrain.GetTerrainTypeAtPoint(endPt);
                if (!AITerrain.IsGroundTypeOut(guideLineFinalGroundType))
                {
                    // 沙坑不弹
                    if (guideLineFinalGroundType != 3)
                    {
                        // 开始估算的第一段弹跳的距离是辅助线最后一段的长度乘以衰减系数
                        float dFactor = guideLineFinalGroundType == 2 ? decrementFactor * 0.5f : decrementFactor;
                        eDistance = eDistance * dFactor;
                        Vector2 lastStartPt;
                        // 弹跳数次
                        while (eDistance > BounceThreshold && guideLineFinalGroundType != 3)
                        {
                            bounceCount++;
                            lastStartPt = startPt;

                            startPt = endPt;
                            endPt = AIMathf.ExtendLine(lastStartPt, startPt, eDistance);
                            guideLineFinalGroundType = terrain.GetTerrainTypeAtPoint(endPt);
                            // 预测的辅助线线段
                            AIGuideLineSegment segmentBounce = new AIGuideLineSegment(
                                AIGuideLineSegmentType.ParabolaSegment,
                                startPt,
                                endPt, guideLineFinalGroundType, true);
                            oriGuideLine.Add(segmentBounce);

                            if (AITerrain.IsGroundTypeOut(guideLineFinalGroundType))
                            {
                                return oriGuideLine;
                            }

                            // 开始估算的第一段弹跳的距离是辅助线最后一段的长度乘以衰减系数
                            dFactor = guideLineFinalGroundType == 2 ? decrementFactor * 0.5f : decrementFactor;
                            eDistance = eDistance * dFactor;
                        }
                    }

                    // 预测滚动
                    AIGuideLineSegment lastBounce = oriGuideLine[oriGuideLine.Count - 1];
                    float scrollDis = 0;

                    if (oriGuideLine.Count >= 2)
                    {
                        AIGuideLineSegment firstBounce = oriGuideLine[1];

                        float firstLen = Vector2.Distance(firstBounce.StartPt, firstBounce.EndPt);

                        int lastStartGt = terrain.GetTerrainTypeAtPointByPhysic(lastBounce.StartPt);
                        int firstStartGt = terrain.GetTerrainTypeAtPointByPhysic(firstBounce.StartPt);
                        int lastEndGt = terrain.GetTerrainTypeAtPointByPhysic(lastBounce.EndPt);
                        int firstEndGt = terrain.GetTerrainTypeAtPointByPhysic(firstBounce.EndPt);

                        if (oriGuideLine.Count > 2 && Vector2.Distance(oriGuideLine[2].StartPt, oriGuideLine[2].EndPt) <
                            firstLen * ScrollRatioThreshold)
                        {
                            // 极特殊不滚动的情况
                            scrollDis = BackSpinScrollDis;
                        }
                        else
                        {
                            float scrollF = lastEndGt == 2
                                ? RoughScrollFactor
                                : (lastEndGt == 3 ? BunkerScrollFactor : ScrollFactor);

                            scrollDis = firstLen * (1f + scrollF);
                        }

                        // 根据前面算得的弹跳和滚动距离，将直线向前延伸一段，估计停止区域中心的坐标
                        stopPos = AIMathf.ExtendLine(lastBounce.StartPt, lastBounce.EndPt, scrollDis);
                        int stopGt = terrain.GetTerrainTypeAtPointByPhysic(stopPos);

                        if ((lastStartGt == 2 && (stopGt == 4 || stopGt == 5))
                            || (firstStartGt == 2 && (firstEndGt == 4 || firstEndGt == 5)))
                        {
                            scrollDis *= Rough2GreenScrollFactor;
                            stopPos = AIMathf.ExtendLine(lastBounce.StartPt, lastBounce.EndPt, scrollDis);
                        }

                        // 预测的滚动线段
                        AIGuideLineSegment segment = new AIGuideLineSegment(AIGuideLineSegmentType.StraightLineSegment,
                            lastBounce.EndPt, stopPos, terrain.GetTerrainTypeAtPoint(stopPos), true);
                        oriGuideLine.Add(segment);
                    }
                    else if (oriGuideLine.Count == 1)
                    {
                        float scrollF = lastBounce.EndGroundType == 2
                            ? RoughScrollFactor
                            : (lastBounce.EndGroundType == 3 ? BunkerScrollFactor : ScrollFactor);
                        float firstLen = Vector2.Distance(lastBounce.StartPt, lastBounce.EndPt);
                        scrollDis = firstLen * 0.2f * (1f + scrollF);

                        // 根据前面算得的弹跳和滚动距离，将直线向前延伸一段，估计停止区域中心的坐标
                        stopPos = AIMathf.ExtendLine(lastBounce.StartPt, lastBounce.EndPt, scrollDis);
                        int stopGt = terrain.GetTerrainTypeAtPointByPhysic(stopPos);
                        int lastStartGt = terrain.GetTerrainTypeAtPointByPhysic(lastBounce.StartPt);
                        if (lastStartGt == 2 && (stopGt == 4 || stopGt == 5))
                        {
                            scrollDis *= Rough2GreenScrollFactor;
                            stopPos = AIMathf.ExtendLine(lastBounce.StartPt, lastBounce.EndPt, scrollDis);
                        }

                        // 预测的滚动线段
                        AIGuideLineSegment segment = new AIGuideLineSegment(AIGuideLineSegmentType.StraightLineSegment,
                            lastBounce.EndPt, stopPos, terrain.GetTerrainTypeAtPoint(stopPos), true);
                        oriGuideLine.Add(segment);
                    }
                }
            }

            return oriGuideLine;
        }


        /// <summary>
        /// 检测是否碰树
        /// </summary>
        private static bool CheckTrunkCollision(AITerrain terrain, List<Vector3> positionList)
        {
            List<Vector3> checkPts = new List<Vector3>();
            const int step = 3;

            // 间隔着查找
            for (int i = 0; i < positionList.Count; i += step)
            {
                checkPts.Add(positionList[i]);
            }

            // 虽然不是已排序的情况，但是树的分布一般不复杂，只需找出一处就说明碰到了，没有必要按顺序，可以二分查找
            return BinarySearchCollisionPoint(terrain, checkPts, 0, checkPts.Count - 1);
        }

        /// <summary>
        /// 二分查找碰树点
        /// </summary>
        private static bool BinarySearchCollisionPoint(AITerrain terrain, List<Vector3> ptList, int left, int right)
        {
            if (left > right)
            {
                return false;
            }

            if (left == right)
            {
                return terrain.IsPointInCollider(ptList[right]);
            }

            if (left + 1 == right)
            {
                return terrain.IsPointInCollider(ptList[left]) || terrain.IsPointInCollider(ptList[right]);
            }

            int mid = (left + right) / 2;
            if (terrain.IsPointInCollider(ptList[left])
                || terrain.IsPointInCollider(ptList[mid])
                || terrain.IsPointInCollider(ptList[right]))
            {
                return true;
            }

            return BinarySearchCollisionPoint(terrain, ptList, left + 1, mid - 1) ||
                   BinarySearchCollisionPoint(terrain, ptList, mid + 1, right - 1);
        }


        /// <summary>
        /// 根据预处理的碰撞体信息和球杆前后旋能力，计算能安全击打的前后旋的范围
        /// （1）原理：只考虑辅助线第一段，预估加了球杆能力上（下）旋的最大值之后，第一段辅助线每一个点处y方向大致会下（上）偏移多少位移，并在y方向上用从原辅助线到偏移后的辅助线的偏移区域与地图预处理的碰撞体区域进行差运算，
        /// 以求出能打的位移，然后逆推出能打的旋度范围
        /// （2）旋度和位移的对应关系建模：取原始辅助线的第一段的峰值点，通过在平面上取一定距离打一定旋度的多组实验来得到数据，通过Python非线性拟合来大致拟合出函数式。辅助线上其他点则以峰值点为标准，分为两段二次函数拟合，第
        /// 一段为发球点到峰值的二次函数，第二段为峰值到落点的二次函数，自变量为离发球点距离，因变量为y的偏移量。（更新：下旋改了物理规律，直接用物理公式计算）
        /// </summary>
        /// <param name="terrain">地图</param>
        /// <param name="allPts">原始辅助线第一段上所有的点</param>
        /// <param name="peakPt">第一段峰值点</param>
        /// <param name="maxTBSpin">球杆前后旋能力范围</param>
        /// <param name="clubEmitCoefficient">球杆抛角</param>
        /// <returns></returns>
        private static AISet CalcTopBackSpinRange(AITerrain terrain, List<Vector3> allPts, Vector3 peakPt, float maxTBSpin,
            float clubEmitCoefficient)
        {
            AISet spinRangeSet = new AISet();
            int allPtCount = allPts.Count;
            if (allPtCount <= 0 || maxTBSpin == 0)
            {
                return spinRangeSet;
            }

            Vector2 peakPt2D = new Vector2(peakPt.x, peakPt.z);

            Vector3 startPt3D = allPts[0];
            Vector2 startPt2D = new Vector2(startPt3D.x, startPt3D.z);

            Vector3 endPt3D = allPts[allPts.Count - 1];
            Vector2 endPt2D = new Vector2(endPt3D.x, endPt3D.z);

            float disPeakPt = Vector2.Distance(peakPt2D, startPt2D);
            float len = Vector2.Distance(startPt2D, endPt2D);


            float peakTopSpinDiff = AISpinFitFunction.CalcTopSpinHeightDiff(1, maxTBSpin, clubEmitCoefficient);

            //拟合的方式不太准，尝试用物理的下旋公式
//            float peakBackSpinDiff = AISpinFitFunction.CalcBackSpinHeightDiff(1, -maxTBSpin, clubEmitCoefficient);
//            int peakIndex = allPts.FindIndex(item => item.Equals(peakPt));
//            float peakAtRatio = peakIndex > 0 && peakIndex < allPtCount
//                ? ((float) peakIndex / (float) allPtCount + 0.8f) * 0.5f
//                : (disPeakPt / len + 0.8f) * 0.5f;
//
//            int newPeakIndex = (int) (allPtCount * peakAtRatio);
//            Vector3 newPeakPt = allPts[newPeakIndex];
//            float absPeakDiff = Mathf.Abs(newPeakPt.y - (startPt3D.y + endPt3D.y) * 0.5f);
//            float peakBackSpinDiff = MathHelper.Instance.BlowUp(peakAtRatio, -maxTBSpin) * absPeakDiff;


            // 上升段假设为二次函数
            float aTop1 = -peakTopSpinDiff / Mathf.Pow(disPeakPt, 2);
            float bTop1 = 2f * peakTopSpinDiff / disPeakPt;

            // 下降段假设为二次函数
            float squareXpAddXe = Mathf.Pow(disPeakPt + len, 2);
            float aTop2 = -peakTopSpinDiff / squareXpAddXe;
            float bTop2 = 2 * disPeakPt * peakTopSpinDiff / squareXpAddXe;
            float cTop2 = (peakTopSpinDiff * Mathf.Pow(len, 2) + 2 * disPeakPt * len * peakTopSpinDiff) / squareXpAddXe;

//            // 上升段假设为二次函数
//            float aBack1 = -peakBackSpinDiff / Mathf.Pow(peakAtRatio, 2);
//            float bBack1 = 2f * peakBackSpinDiff / peakAtRatio;
//
//            // 下降段假设为二次函数
//            float squareXpAddXeBack = Mathf.Pow(peakAtRatio + 1f, 2);
//            float aBack2 = -peakBackSpinDiff / squareXpAddXeBack;
//            float bBack2 = 2 * peakAtRatio * peakBackSpinDiff / squareXpAddXeBack;
//            float cBack2 = (peakBackSpinDiff * Mathf.Pow(1f, 2) + 2 * peakAtRatio * 1f * peakBackSpinDiff) /
//                           squareXpAddXeBack;

            float calcPercision = 0.0001f;
            List<AISet> spinRangeSetList = new List<AISet>();

            int ptCount = allPts.Count;
            int skipCount = 5;
            for (int ptIndex = skipCount + 1; ptIndex < ptCount - 1 - skipCount; ptIndex += FirstSegmentScanStep)
            {
                Vector3 pt = allPts[ptIndex];
                Vector2 pt2D = new Vector2(pt.x, pt.z);

                AISet colliderRanges = terrain.GetColliderYRangesAtPoint(pt2D);
                AISet spinSet = new AISet();

                if (colliderRanges != null)
                {
                    float dis = Vector2.Distance(pt2D, startPt2D);
                    float maxTSYAtPointDiff = 0f;
                    float maxBSYAtPointDiff = 0f;
                    float disRatio = ((float) ptIndex + 1) / (float) allPtCount;
                    // 上旋差值
                    if (dis < disPeakPt)
                    {
                        maxTSYAtPointDiff = aTop1 * dis * dis + bTop1 * dis;
                    }
                    else
                    {
                        maxTSYAtPointDiff = aTop2 * dis * dis + bTop2 * dis + cTop2;
                    }


//                // 下旋差值
//                if (disRatio < peakAtRatio)
//                {
//                    maxBSYAtPointDiff = aBack1 * disRatio * disRatio + bBack1 * disRatio;
//                }
//                else
//                {
//                    maxBSYAtPointDiff = aBack2 * disRatio * disRatio + bBack2 * disRatio + cBack2;
//                }

                    maxBSYAtPointDiff = MathHelper.Instance.BlowUp(disRatio, -maxTBSpin) * (pt.y - terrain.GoalPosY);

                    float maxTSYAtPoint = pt.y - maxTSYAtPointDiff;
                    float maxBSYAtPoint = pt.y + maxBSYAtPointDiff;

                    AIRange spinYRange;
                    spinYRange.Min = maxTSYAtPoint;
                    spinYRange.Max = maxBSYAtPoint;

                    AISet topSpinYSet = new AISet();
                    topSpinYSet.AddRange(new AIRange(AIMathf.CeilToPercision(maxTSYAtPoint, calcPercision),
                        AIMathf.CeilToPercision(pt.y, calcPercision)));

                    AISet backSpinYSet = new AISet();
                    backSpinYSet.AddRange(new AIRange(AIMathf.CeilToPercision(pt.y, calcPercision),
                        AIMathf.CeilToPercision(maxBSYAtPoint, calcPercision)));

                    topSpinYSet.ExceptWith(colliderRanges);
                    backSpinYSet.ExceptWith(colliderRanges);


                    foreach (AIRange rTop in topSpinYSet.Ranges)
                    {
                        AIRange spinRange;
                        //最小值
                        float diffY = pt.y - rTop.Max;
                        float ratio = diffY / maxTSYAtPointDiff;
                        float oriDiffY = peakTopSpinDiff * ratio;
                        float spin = AISpinFitFunction.CalcTopSpinByHeightDiff(1, oriDiffY, clubEmitCoefficient);
                        spinRange.Min = AIMathf.CeilToPercision(spin, 0.1f);

                        // 最大值
                        diffY = pt.y - rTop.Min;
                        ratio = diffY / maxTSYAtPointDiff;
                        oriDiffY = peakTopSpinDiff * ratio;
                        spin = AISpinFitFunction.CalcTopSpinByHeightDiff(1, oriDiffY, clubEmitCoefficient);
                        spinRange.Max = AIMathf.CeilToPercision(spin, 0.1f);

                        spinSet.AddRange(spinRange);
                    }

                    foreach (AIRange rBack in backSpinYSet.Ranges)
                    {
//                    AIRange spinRange;
//
//                    float diffY = rBack.Max - pt.y;
//                    float ratio = diffY / maxBSYAtPointDiff;
//                    float oriDiffY = peakBackSpinDiff * ratio;
////                    float spin = AISpinFitFunction.CalcBackSpinByHeightDiff(1, oriDiffY, clubEmitCoefficient);
//                    float spin = MathHelper.Instance.BlowUpInverse(peakAtRatio, oriDiffY / absPeakDiff);
//                    spinRange.Min = AIMathf.CeilToPercision(spin, 0.1f);
//
//                    diffY = rBack.Min - pt.y;
//                    ratio = diffY / maxBSYAtPointDiff;
//                    oriDiffY = peakBackSpinDiff * ratio;
////                    spin = AISpinFitFunction.CalcBackSpinByHeightDiff(1, oriDiffY, clubEmitCoefficient);
//                    spin = MathHelper.Instance.BlowUpInverse(peakAtRatio, oriDiffY / absPeakDiff);
//                    spinRange.Max = AIMathf.CeilToPercision(spin, 0.1f);
//                    spinSet.AddRange(spinRange);

                        AIRange spinRange;
//                    float spin = AISpinFitFunction.CalcBackSpinByHeightDiff(1, oriDiffY, clubEmitCoefficient);
                        float diffY = rBack.Max - pt.y;
                        float diffRatio = diffY / (pt.y - terrain.GoalPosY);
                        float spin = MathHelper.Instance.BlowUpInverse(disRatio, diffRatio);
                        spinRange.Min = AIMathf.CeilToPercision(spin, 0.1f);

                        diffY = rBack.Min - pt.y;
                        diffRatio = diffY / (pt.y - terrain.GoalPosY);
//                    spin = AISpinFitFunction.CalcBackSpinByHeightDiff(1, oriDiffY, clubEmitCoefficient);
                        spin = MathHelper.Instance.BlowUpInverse(disRatio, diffRatio);
                        spinRange.Max = AIMathf.CeilToPercision(spin, 0.1f);
                        spinSet.AddRange(spinRange);
                    }
                }
                else
                {
                    spinSet.AddRange(new AIRange(-maxTBSpin, maxTBSpin));
                }

                spinRangeSetList.Add(spinSet);
            }

            int count = spinRangeSetList.Count;
            if (count > 0)
            {
                spinRangeSet.UnionWith(spinRangeSetList[0]);
            }

            for (int i = 1; i < count; i++)
            {
                spinRangeSet.IntersectWith(spinRangeSetList[i]);
            }

            return spinRangeSet;
        }


        /// <summary>
        /// 根据预处理的碰撞体信息和球杆侧旋能力计算能击打的侧旋范围
        /// 1）原理：只考虑辅助线第一段，预估加了球杆能力左（右）旋的最大值之后，第一段辅助线每一个点处在与辅助线第一段垂直的方向上大致会左（有）偏移多少位移，并在垂直方向上用从原辅助线到偏移后的辅助线的偏移区域与地图预
        /// 处理的碰撞体区域进行差运算，以求出能打的位移，然后逆推出能打的旋度范围
        /// （2）旋度和位移的对应关系建模：取原始辅助线的第一段的峰值点，通过在平面上取一定距离打一定旋度的多组实验来得到数据，通过Python非线性拟合来大致拟合出函数式。辅助线上其他点则以峰值点为标准，分为两段二次函数拟合，第
        /// 一段为发球点到峰值的二次函数，第二段为峰值到落点的二次函数，自变量为离发球点距离，因变量为垂直方向上的偏移量。
        /// </summary>
        /// <param name="terrain">地图</param>
        /// <param name="allPts">原始辅助线的第一段上所有的点</param>
        /// <param name="peakPt"></param>
        /// <param name="maxSSpin"></param>
        /// <param name="clubEmitCoefficient"></param>
        /// <returns></returns>
        private static AISet CalcSideSpinRange(AITerrain terrain, List<Vector3> allPts, Vector3 peakPt, float maxSSpin,
            float clubEmitCoefficient)
        {
            AISet spinRangeSet = new AISet();
            if (allPts.Count <= 0 || maxSSpin == 0)
            {
                return spinRangeSet;
            }

            Vector2 peakPt2D = new Vector2(peakPt.x, peakPt.z);

            Vector3 startPt3D = allPts[0];
            Vector2 startPt2D = new Vector2(startPt3D.x, startPt3D.z);

            Vector3 endPt3D = allPts[allPts.Count - 1];
            Vector2 endPt2D = new Vector2(endPt3D.x, endPt3D.z);

            float peakSideSpinDiff = AISpinFitFunction.CalcSideSpinSidePeakDiff(maxSSpin, clubEmitCoefficient);


            float disPeakPt = Vector2.Distance(peakPt2D, startPt2D);
            float len = Vector2.Distance(startPt2D, endPt2D);
            // 上升段假设为二次函数
            float aSide1 = -peakSideSpinDiff / Mathf.Pow(disPeakPt, 2);
            float bSide1 = 2f * peakSideSpinDiff / disPeakPt;

            // 下降段假设为二次函数
            float squareXpAddXe = Mathf.Pow(disPeakPt + len, 2);
            float aSide2 = -peakSideSpinDiff / squareXpAddXe;
            float bSide2 = 2 * disPeakPt * peakSideSpinDiff / squareXpAddXe;
            float cSide2 = (peakSideSpinDiff * Mathf.Pow(len, 2) + 2 * disPeakPt * len * peakSideSpinDiff) /
                           squareXpAddXe;

            float calcPercision = 0.0001f;
            List<AISet> spinRangeSetList = new List<AISet>();

            Vector2 dirOri = endPt2D - startPt2D;
            Vector2 dirRight = AIMathf.RotateVectorByAngle(dirOri, 90f).normalized;
            Vector2 dirLeft = AIMathf.RotateVectorByAngle(dirOri, -90f).normalized;
            const float stepPercision = AITerrain.Percision / 2f;
            int ptCount = allPts.Count;
            for (int ptIndex = 1; ptIndex < ptCount - 1; ptIndex += FirstSegmentScanStep)
            {
                Vector3 pt = allPts[ptIndex];

                Vector2 pt2D = new Vector2(pt.x, pt.z);
                float dis = Vector2.Distance(pt2D, startPt2D);
                float maxSSXAtPointDiff = 0f;
                if (dis < disPeakPt)
                {
                    maxSSXAtPointDiff = aSide1 * dis * dis + bSide1 * dis;
                }
                else
                {
                    maxSSXAtPointDiff = aSide2 * dis * dis + bSide2 * dis + cSide2;
                }

                AISet leftSpinSet = new AISet();
                AISet rightSpinSet = new AISet();

                // 左旋为正
                for (float sideDiff = 0; sideDiff <= maxSSXAtPointDiff; sideDiff += stepPercision)
                {
                    Vector2 ptLeft = pt2D + dirLeft * sideDiff;
                    Vector3 ptLeft3D = new Vector3(ptLeft.x, pt.y, ptLeft.y);

                    if (!terrain.IsPointInXZCollider(ptLeft3D))
                    {
                        AIRange range;
                        float minRatio = sideDiff / maxSSXAtPointDiff;
                        float oriMinDiff = peakSideSpinDiff * minRatio;
                        range.Min = AIMathf.CeilToPercision(
                            AISpinFitFunction.CalcSideSpinBySidePeakDiff(oriMinDiff, clubEmitCoefficient), 0.1f);

                        while (sideDiff <= maxSSXAtPointDiff)
                        {
                            sideDiff += stepPercision;
                            Vector2 ptTempCur = pt2D + dirLeft * sideDiff;
                            Vector3 ptTemp3DCur = new Vector3(ptTempCur.x, pt.y, ptTempCur.y);
                            bool isCurPtCollider = terrain.IsPointInXZCollider(ptTemp3DCur);
                            if (isCurPtCollider)
                            {
                                float maxRatio = (sideDiff - stepPercision) / maxSSXAtPointDiff;
                                float oriMaxDiff = peakSideSpinDiff * maxRatio;
                                range.Max = AIMathf.CeilToPercision(
                                    AISpinFitFunction.CalcSideSpinBySidePeakDiff(oriMaxDiff, clubEmitCoefficient),
                                    0.1f);
                                leftSpinSet.AddRange(range);
                                break;
                            }
                            else if (Mathf.Abs(sideDiff - maxSSXAtPointDiff) < stepPercision)
                            {
                                float oriMaxDiff = peakSideSpinDiff;
                                range.Max = AIMathf.CeilToPercision(
                                    AISpinFitFunction.CalcSideSpinBySidePeakDiff(oriMaxDiff, clubEmitCoefficient),
                                    0.1f);
                                leftSpinSet.AddRange(range);
                                break;
                            }
                        }
                    }
                    else
                    {
                        sideDiff += stepPercision;
                    }
                }


                // 右旋为负
                for (float sideDiff = 0; sideDiff <= maxSSXAtPointDiff; sideDiff += stepPercision)
                {
                    Vector2 ptRight = pt2D + dirRight * sideDiff;
                    Vector3 ptRight3D = new Vector3(ptRight.x, pt.y, ptRight.y);

                    if (!terrain.IsPointInXZCollider(ptRight3D))
                    {
                        AIRange range;
                        float maxRatio = sideDiff / maxSSXAtPointDiff;
                        float oriMaxDiff = peakSideSpinDiff * maxRatio;
                        range.Max = -AIMathf.CeilToPercision(
                            AISpinFitFunction.CalcSideSpinBySidePeakDiff(oriMaxDiff, clubEmitCoefficient), 0.1f);

                        while (sideDiff <= maxSSXAtPointDiff)
                        {
                            sideDiff += stepPercision;
                            Vector2 ptTempCur = pt2D + dirRight * sideDiff;
                            Vector3 ptTemp3DCur = new Vector3(ptTempCur.x, pt.y, ptTempCur.y);
                            bool isCurPtCollider = terrain.IsPointInXZCollider(ptTemp3DCur);
                            if (isCurPtCollider)
                            {
                                float minRatio = (sideDiff - stepPercision) / maxSSXAtPointDiff;
                                float oriMinDiff = peakSideSpinDiff * minRatio;
                                range.Min = -AIMathf.CeilToPercision(
                                    AISpinFitFunction.CalcSideSpinBySidePeakDiff(oriMinDiff, clubEmitCoefficient),
                                    0.1f);
                                rightSpinSet.AddRange(range);
                                break;
                            }
                            else if (Mathf.Abs(sideDiff - maxSSXAtPointDiff) < stepPercision)
                            {
                                float oriMinDiff = peakSideSpinDiff;
                                range.Min = -AIMathf.CeilToPercision(
                                    AISpinFitFunction.CalcSideSpinBySidePeakDiff(oriMinDiff, clubEmitCoefficient),
                                    0.1f);
                                rightSpinSet.AddRange(range);
                                break;
                            }
                        }
                    }
                    else
                    {
                        sideDiff += stepPercision;
                    }
                }

                leftSpinSet.UnionWith(rightSpinSet);
                spinRangeSetList.Add(leftSpinSet);
            }

            int count = spinRangeSetList.Count;
            if (count > 0)
            {
                spinRangeSet.UnionWith(spinRangeSetList[0]);
            }

            for (int i = 1; i < count; i++)
            {
                spinRangeSet.IntersectWith(spinRangeSetList[i]);
            }

            return spinRangeSet;
        }


        /// <summary>
        /// 判断辅助线是否穿过了球洞
        /// </summary>
        /// <param name="guideLine">辅助线数组</param>
        /// <param name="goalPos">球洞位置</param>
        /// <returns></returns>
        private static bool GuideLineHasCrossGoal(List<AIGuideLineSegment> guideLine, Vector2 goalPos)
        {
            bool hasCross = false;
            if (guideLine.Count <= 0)
            {
                return false;
            }
            else
            {
                Vector2 lastEndPt = guideLine[guideLine.Count - 1].EndPt;
                Vector2 lastStartPt = guideLine[guideLine.Count - 1].StartPt;
                Vector2 shotPoint = guideLine[0].StartPt;
                Vector2 lastStart2LastEnd = lastEndPt - lastStartPt;
                Vector2 vLastStart2LastEnd = AIMathf.RotateVectorByAngle(lastStart2LastEnd, 90f).normalized;
                Vector2 lastStart2Goal = goalPos - lastStartPt;

                bool isFarther = (Vector2.Distance(shotPoint, lastEndPt) >= Vector2.Distance(shotPoint, goalPos));
                bool isInLine = Mathf.Abs(Vector2.Dot(vLastStart2LastEnd, lastStart2Goal)) < CrossGoalE;
                hasCross = (isFarther && isInLine);
            }

            return hasCross;
        }

        
        /// <summary>
        /// 根据能打的前后旋和上下旋的所有范围，组合出所有可行的（上下旋 x 前后旋）的打法
        /// </summary>
        /// <param name="terrain">地图</param>
        /// <param name="oriGuideLine">不带旋度的原始辅助线</param>
        /// <param name="finalState">原始辅助线最终停球状态</param>
        /// <param name="topBacSpinRange">可旋的前后旋范围</param>
        /// <param name="sideSpinRange">可选的侧旋范围</param>
        /// <param name="clubEmitCoefficient">球杆抛角</param>
        /// <param name="isCompleteBounce">原始辅助线是否提前结束弹跳，进入滚动状态</param>
        /// <param name="bounceCount">原始辅助线弹跳次数</param>
        /// <param name="player"></param>
        /// <param name="decrementFactor"></param>
        /// <returns></returns>
        private static List<AIGuideLine> generateSpinGuideLineList(AITerrain terrain,
            List<AIGuideLineSegment> oriGuideLine, int finalState, AISet topBacSpinRange, AISet sideSpinRange,
            float clubEmitCoefficient, bool isCompleteBounce, int bounceCount, AIPlayer player, float decrementFactor)
        {
            List<AIGuideLine> guideLineList = new List<AIGuideLine>();

            // 击球方向上的误差跟估计的弹跳次数正相关
            float errorWidth = 0;
            if (!isCompleteBounce || bounceCount == 0)
            {
                errorWidth = ScrollError;
            }
            else
            {
                errorWidth = ErrorFactor * bounceCount;
            }

            float errorHeight = ErrorFactor * (2f - player.DirectionAccuracyLevel / 100f);

            foreach (AIRange tbRange in topBacSpinRange.Ranges)
            {
                for (float tbSpin = tbRange.Max; tbSpin >= tbRange.Min; tbSpin -= SpinPercision)
                {
                    List<AIGuideLineSegment> tbGuideLine = CalcGuideLineWithTopBackSpin(oriGuideLine, finalState,
                        tbSpin, terrain, decrementFactor, clubEmitCoefficient, isCompleteBounce);
                    foreach (AIRange sSRange in sideSpinRange.Ranges)
                    {
                        for (float sSpin = sSRange.Max; sSpin >= sSRange.Min; sSpin -= SpinPercision)
                        {
                            List<AIGuideLineSegment> sGuideLine = CalcGuideLineWithSideSpin(tbGuideLine, finalState,
                                sSpin,
                                terrain, decrementFactor, clubEmitCoefficient, isCompleteBounce);
                            bool isDanger =
                                AIModeSwitch.IsInPar3Mode
                                    ? IsGuideLineInDangerPar3(sGuideLine)
                                    : IsGuideLineInDanger(sGuideLine);
                            if (isDanger)
                            {
                                continue;
                            }

                            if (sGuideLine.Count > 0)
                            {
                                AIGuideLine guideLine = new AIGuideLine();
                                guideLine.TopBackSpin = tbSpin;
                                guideLine.SideSpin = sSpin;
                                guideLine.FinalGuideLine = sGuideLine;
                                AIGuideLineSegment lastSegment = sGuideLine[sGuideLine.Count - 1];
                                AIGuideLineSegment firstSegment = sGuideLine[0];
                                Vector2 dropPoint = firstSegment.EndPt;
                                // bool isCompleteBounce = lastSegment.type != AIGuideLineSegmentType.StraightLineSegment;
                                // AIMapRegion region = estimateRectStopRegion(terrain, dropPoint, lastSegment.startPt, lastSegment.endPt, isCompleteBounce, getGuideLinePredictBounceCount(sGuideLine));                        
                                AIMapRegion region = EstimateRectStopRegion(terrain, dropPoint, lastSegment.StartPt,
                                    lastSegment.EndPt, errorWidth, errorHeight);

                                region.TopBackSpin = tbSpin;
                                region.SideSpin = sSpin;
                                guideLine.FinalRegion = region;
                                guideLineList.Add(guideLine);
                            }
                        }
                    }
                }
            }

            return guideLineList;
        }


        /// <summary>
        /// 在原有辅助线上加入前后旋的影响，计算出带前后旋的新辅助线
        /// 模型：
        /// 假设上下旋影响的会使原始辅助线的第二段开始（物理上，理论上不移动落点，只改速度，除非旋度极大会有计算误差，ai对此忽略不计），每段的长度增长或者减小一定的比值，
        /// 即len = len * (1 ± ratio)，将原始辅助线进行延伸（裁剪）。
        /// 假设上下旋不影响方向，即只在原始辅助线的方向和长度的基础上往前延伸或裁剪。
        /// 关于比值，通过在平面上组合距离和旋度采集实验数据，使用Python拟合曲面得出表达式
        /// </summary>
        /// <param name="originGuideLine">原始辅助线</param>
        /// <param name="finalStateWithoutSpin">原始辅助线最终停球状态</param>
        /// <param name="spin">侧旋值</param>
        /// <param name="terrain">地图</param>
        /// <param name="decrementFactor">衰减系数</param>
        /// <param name="clubEmitCoefficient">球杆抛角</param>
        /// <param name="isCompletedBounce">原始辅助线是否没有弹跳完就进入滚动的状态</param>
        /// <returns></returns>
        private static List<AIGuideLineSegment> CalcGuideLineWithTopBackSpin(List<AIGuideLineSegment> originGuideLine,
            int finalStateWithoutSpin, float spin, AITerrain terrain, float decrementFactor, float clubEmitCoefficient,
            bool isCompletedBounce)
        {
            if (Mathf.Abs(spin) < 0.0000001f || originGuideLine.Count < 1)
            {
                return originGuideLine;
            }

            List<AIGuideLineSegment> segementList = new List<AIGuideLineSegment>();
            // 第一个落点不变
            int segmentCount = originGuideLine.Count;
            AIGuideLineSegment shotSegment = originGuideLine[0];
            segementList.Add(shotSegment);
            float distance = Vector2.Distance(shotSegment.StartPt, shotSegment.EndPt);

            float spinFactor = 0;
            float spinLevel = spin;
            if (spin > 0 && distance > 0)
            {
                spinFactor = AISpinFitFunction.CalcTopSpinRatio(distance, spin, clubEmitCoefficient);
            }
            else
            {
                spinFactor =
                    -AISpinFitFunction.CalcBackSpinRatio(distance, spin, clubEmitCoefficient); // 当下旋时，为负数，即实现向反方向缩短辅助线
            }

            bool isOut = false;
            for (int i = 1; i < segmentCount; i++)
            {
                // 未加Spin的辅助线当前段，起点指向终点的向量（保持了方向和长度）
                AIGuideLineSegment segment = originGuideLine[i];
                Vector2 diffVec = segment.EndPt - segment.StartPt;

                if (i == segmentCount - 1)
                {
                    if (segment.Type == AIGuideLineSegmentType.StraightLineSegment)
                    {
                        break;
                    }
                }

                // 加上spin后的上一段的终点
                AIGuideLineSegment preSegment = segementList[i - 1];
                Vector2 preEndPt = preSegment.EndPt;

                // 该段在Spin作用下延长的长度
                float segmentLen = Vector2.Distance(segment.StartPt, segment.EndPt);
                float diffLen = segmentLen * spinFactor;

                // 未延长前应该指向的终点
                Vector2 endPtBeforeExtend = preEndPt + diffVec;

                // 延长
                Vector2 endPt = AIMathf.ExtendLine(preEndPt, endPtBeforeExtend, diffLen);

                int endGT = terrain.GetTerrainTypeAtPoint(endPt);
                AIGuideLineSegment spinSegment =
                    new AIGuideLineSegment(segment.Type, preEndPt, endPt, endGT, segment.IsPredictedPoint);
                segementList.Add(spinSegment);

                if (AITerrain.IsGroundTypeOut(endGT))
                {
                    isOut = true;
                    break;
                }

                // 侧旋极可能只弹起来一下就不弹了
                if (spin < 0 && i > 1 &&
                    Vector2.Distance(spinSegment.StartPt, spinSegment.EndPt) < BackSpinBounceThreshold)
                {
                    break;
                }

                // 侧旋弹起一次之后，继续弹跳没有之前强劲了
                if (i == 1 && spin < 0)
                {
                    spinFactor += spin / 10f * 0.1f;
                }
            }

            if (segementList.Count == 0)
            {
                return segementList;
            }

            AIGuideLineSegment lastSeg = segementList[segementList.Count - 1];
            // 如果没有加旋度的时候掉水里或者出界了，但是加了旋度则不会的话，则必须继续往前预测
            if ((!isOut && segmentCount > 0 && finalStateWithoutSpin == 5) ||
                (Vector2.Distance(lastSeg.StartPt, lastSeg.EndPt) > BounceThreshold))
            {
                // 弹跳数次
                bool isOutAgain = false;
                AIGuideLineSegment segment;
                int lastSegmentGt = lastSeg.EndGroundType;
                bool canBounceAgain = isCompletedBounce || (!isCompletedBounce && lastSegmentGt == 1);
                if (canBounceAgain && spin > 0 && lastSegmentGt != 3)
                {
                    Vector2 startPt = lastSeg.StartPt;
                    Vector2 endPt = lastSeg.EndPt;
                    Vector2 pt2 = startPt;
                    Vector2 pt1 = endPt;

                    float lastDis = Vector2.Distance(startPt, endPt);
                    float dFactor = lastSeg.EndGroundType == 2 ? decrementFactor * 0.5f : decrementFactor;

                    // 延伸的第一段是否符合要求
                    lastDis = lastDis * dFactor;
                    Vector2 lastStartPt;
                    while (lastDis > BounceThreshold && lastSegmentGt != 3)
                    {
                        // 预测的抛物线
                        lastStartPt = startPt;
                        startPt = endPt;
                        endPt = AIMathf.ExtendLine(lastStartPt, startPt, lastDis);
                        lastSegmentGt = terrain.GetTerrainTypeAtPoint(endPt);
                        // 预测的辅助线线段
                        segment = new AIGuideLineSegment(AIGuideLineSegmentType.ParabolaSegment, startPt, endPt,
                            lastSegmentGt,
                            true);
                        segementList.Add(segment);
                        dFactor = segment.EndGroundType == 2 ? decrementFactor * 0.5f : decrementFactor;
                        lastDis = lastDis * dFactor;
                        if (AITerrain.IsGroundTypeOut(lastSegmentGt))
                        {
                            isOutAgain = true;
                            break;
                        }
                    }
                }

                if (!isOutAgain)
                {
//                    if (spin < 0)
//                    {
//                        // 下旋几乎不滚
//                        AIGuideLineSegment lastBounce = segementList[segementList.Count - 1];
//
//                        float scrollDis = BackSpinScrollDis;
//                        
//                        AIGuideLineSegment oriLastSegment = originGuideLine[originGuideLine.Count - 1];
//                        if (oriLastSegment.Type == AIGuideLineSegmentType.StraightLineSegment)
//                        {
//                            float guideLineScrollLen = Vector2.Distance(oriLastSegment.StartPt, oriLastSegment.EndPt) * (1 + spinFactor) * (1 + spinFactor);
//                                
//                            if (scrollDis < guideLineScrollLen)
//                            {
//                                scrollDis = guideLineScrollLen * 1.1f;
//                            }
//                        }
//                        
//                        Vector2 stopPos = AIMathf.ExtendLine(lastBounce.StartPt, lastBounce.EndPt, scrollDis);
//                        segment = new AIGuideLineSegment(AIGuideLineSegmentType.StraightLineSegment, lastBounce.EndPt,
//                            stopPos, terrain.GetTerrainTypeAtPoint(stopPos), true);
//                        segementList.Add(segment);
//                    }
//                    else
//                    {
//                        // 滚动一段，假设为弹跳的最后一次的距离
//                        if (segementList.Count > 1)
//                        {
//                            AIGuideLineSegment firstBounce = segementList[1];
//                            AIGuideLineSegment finalBounce = segementList[segementList.Count - 1];
//                            float firstLen = Vector2.Distance(firstBounce.StartPt, firstBounce.EndPt);
//                            float scrollDis;
//                            
//                            if (segementList.Count > 2 && Vector2.Distance(segementList[2].StartPt, segementList[2].EndPt) < firstLen * ScrollRatioThreshold)
//                            {
//                                // 极特殊不滚动的情况
//                                scrollDis = 0;
//                            }
//                            else
//                            {
//                                
//                                float scrollF = finalBounce.EndGroundType == 2
//                                    ? RoughScrollFactor
//                                    : (finalBounce.EndGroundType == 3 ? BunkerScrollFactor : ScrollFactor);
//                                scrollDis = firstLen * (1f + scrollF);
//                                
//                                AIGuideLineSegment oriLastSegment = originGuideLine[originGuideLine.Count - 1];
//                                if (oriLastSegment.Type == AIGuideLineSegmentType.StraightLineSegment)
//                                {
//                                    float guideLineScrollLen = Vector2.Distance(oriLastSegment.StartPt, oriLastSegment.EndPt) * (1 + spinFactor);
//                                    float fastScrollF = oriLastSegment.EndGroundType == 2
//                                        ? FastRollFactor * 0.5f
//                                        : (oriLastSegment.EndGroundType == 3 ? FastRollFactor * 0.2f : FastRollFactor);
//                                    if (scrollDis < guideLineScrollLen)
//                                    {
//                                        scrollDis = guideLineScrollLen * fastScrollF;
//                                    }
//                                }
//                            }
//                                                        
//                            AIGuideLineSegment lastBounce = segementList[segementList.Count - 1];
//                            Vector2 stopPos = AIMathf.ExtendLine(lastBounce.StartPt, lastBounce.EndPt, scrollDis);
//                            // 预测的滚动线段
//                            segment = new AIGuideLineSegment(AIGuideLineSegmentType.StraightLineSegment,
//                                lastBounce.EndPt, stopPos, terrain.GetTerrainTypeAtPoint(stopPos), true);
//                            segementList.Add(segment);
//                        }
//                        else
//                        {
//                            AIGuideLineSegment firstBounce = segementList[0];
//                            float scrollF = firstBounce.EndGroundType == 2
//                                ? RoughScrollFactor
//                                : (firstBounce.EndGroundType == 3 ? BunkerScrollFactor : ScrollFactor);
//                            float scrollDis = Vector2.Distance(firstBounce.StartPt, firstBounce.EndPt) * 0.2f *
//                                              (1f + scrollF);
//                            
//                            AIGuideLineSegment oriLastSegment = originGuideLine[originGuideLine.Count - 1];
//                            if (oriLastSegment.Type == AIGuideLineSegmentType.StraightLineSegment)
//                            {
//                                float guideLineScrollLen = Vector2.Distance(oriLastSegment.StartPt, oriLastSegment.EndPt) * (1 + spinFactor);
//                                float fastScrollF = oriLastSegment.EndGroundType == 1
//                                    ? FastRollFactor
//                                    : (oriLastSegment.EndGroundType == 3 ? FastRollFactor * 0.2f : FastRollFactor * 0.5f);
//                                if (scrollDis < guideLineScrollLen)
//                                {
//                                    scrollDis = guideLineScrollLen * fastScrollF;
//                                }
//                            }
//                            
//                            Vector2 stopPos = AIMathf.ExtendLine(firstBounce.StartPt, firstBounce.EndPt, scrollDis);
//                            // 预测的滚动线段
//                            segment = new AIGuideLineSegment(AIGuideLineSegmentType.StraightLineSegment,
//                                firstBounce.EndPt, stopPos, terrain.GetTerrainTypeAtPoint(stopPos), true);
//                            segementList.Add(segment);
//                        }
//                    }

                    AIGuideLineSegment lastBounce = segementList[segementList.Count - 1];
                    float scrollDis;
                    if (spin < 0)
                    {
                        // 下旋几乎不滚

                        scrollDis = BackSpinScrollDis;

                        AIGuideLineSegment oriLastSegment = originGuideLine[originGuideLine.Count - 1];
                        if (oriLastSegment.Type == AIGuideLineSegmentType.StraightLineSegment)
                        {
                            float guideLineScrollLen = Vector2.Distance(oriLastSegment.StartPt, oriLastSegment.EndPt) *
                                                       (1 + spinFactor);

                            scrollDis = Mathf.Max(scrollDis, guideLineScrollLen);
                        }
                    }
                    else
                    {
                        AIGuideLineSegment oriLastSegment = originGuideLine[originGuideLine.Count - 1];

                        if (oriLastSegment.Type == AIGuideLineSegmentType.StraightLineSegment)
                        {
                            scrollDis = Vector2.Distance(oriLastSegment.StartPt, oriLastSegment.EndPt) *
                                        (1 + spinFactor);
                        }
                        else
                        {
                            scrollDis = BackSpinScrollDis * (1 + spinFactor);
                        }
                    }

                    Vector2 stopPos = AIMathf.ExtendLine(lastBounce.StartPt, lastBounce.EndPt, scrollDis);
                    segment = new AIGuideLineSegment(AIGuideLineSegmentType.StraightLineSegment, lastBounce.EndPt,
                        stopPos, terrain.GetTerrainTypeAtPoint(stopPos), true);
                    segementList.Add(segment);
                }
            }

            return segementList;
        }

        /// <summary>
        /// 在原有辅助线上加入侧旋的影响，计算出带侧旋的新辅助线
        /// 模型：
        /// 假设上下旋影响的会使原始辅助线的第二段偏转一定的角度（物理上，理论上不移动落点，只改速度，除非旋度极大会有计算误差，ai对此忽略不计），从第二段偏转之后，第三段及往后的辅助线段
        /// 沿着第二段的基础上来确定方向
        /// 假设策旋不影响长度，即只在原始辅助线的长度的基础上偏转。
        /// 关于比值，通过在平面上组合距离和旋度采集实验数据，使用Python拟合曲面得出表达式
        /// </summary>
        /// <param name="originGuideLine">原始辅助线</param>
        /// <param name="finalStateWithoutSpin">原始辅助线最终停球状态</param>
        /// <param name="spin">侧旋值</param>
        /// <param name="terrain">地图</param>
        /// <param name="decrementFactor">衰减系数</param>
        /// <param name="clubEmitCoefficient">球杆抛角</param>
        /// <param name="isCompletedBounce">原始辅助线是否没有弹跳完就进入滚动的状态</param>
        /// <returns></returns>
        private static List<AIGuideLineSegment> CalcGuideLineWithSideSpin(List<AIGuideLineSegment> originGuideLine,
            int finalStateWithoutSpin, float spin, AITerrain terrain, float decrementFactor, float clubEmitCoefficient,
            bool isCompletedBounce)
        {
            if (Mathf.Abs(spin) < 0.0000001f)
            {
                return originGuideLine;
            }

            List<AIGuideLineSegment> segementList = new List<AIGuideLineSegment>();
            // 第一个落点不变
            int segmentCount = originGuideLine.Count;

            if (segmentCount > 0)
            {
                AIGuideLineSegment shotSegment = originGuideLine[0];
                segementList.Add(shotSegment);
            }

            bool isOut = false;
            for (int i = 1; i < segmentCount; i++)
            {
                AIGuideLineSegment segment = originGuideLine[i];
                Vector2 diffVec = segment.EndPt - segment.StartPt;

                AIGuideLineSegment preSegment = segementList[i - 1];
                Vector2 preEndPt = preSegment.EndPt;

                if (i == segmentCount - 1)
                {
                    if (segment.Type == AIGuideLineSegmentType.StraightLineSegment)
                    {
                        break;
                    }
                }

                Vector2 endPt;

                if (i == 1)
                {
                    float distance = Vector2.Distance(preSegment.StartPt, preSegment.EndPt);
                    float sideAngle = AISpinFitFunction.CalcSideSpinAngle(distance, spin, clubEmitCoefficient);
                    Vector2 rotateVec = AIMathf.RotateVectorByAngle(diffVec, sideAngle);
                    endPt = preEndPt + rotateVec;
                }
                else
                {
                    endPt = preEndPt + diffVec;
                }

                int endGT = terrain.GetTerrainTypeAtPoint(endPt);
                AIGuideLineSegment spinSegment =
                    new AIGuideLineSegment(segment.Type, preEndPt, endPt, endGT, segment.IsPredictedPoint);
                segementList.Add(spinSegment);

                if (AITerrain.IsGroundTypeOut(endGT))
                {
                    isOut = true;
                    break;
                }
            }

            if (segementList.Count == 0)
            {
                return segementList;
            }

            AIGuideLineSegment lastSeg = segementList[segementList.Count - 1];
            // 如果没有加旋度的时候掉水里或者出界了，但是加了旋度则不会的话，则必须继续往前预测
            if ((!isOut && segmentCount > 0 && finalStateWithoutSpin == 5 ||
                 Vector2.Distance(lastSeg.StartPt, lastSeg.EndPt) > BounceThreshold) && lastSeg.EndGroundType != 3)
            {
                Vector2 startPt = lastSeg.StartPt;
                Vector2 endPt = lastSeg.EndPt;
                Vector2 pt2 = startPt;
                Vector2 pt1 = endPt;

                float lastDis = Vector2.Distance(startPt, endPt);
                float evaluatedDistance = 0.0f;
                AIGuideLineSegment segment;
                // 弹跳数次
                bool isOutAgain = false;

                int lastSegEndGt = lastSeg.EndGroundType;
                bool canBounceAgain = isCompletedBounce || (!isCompletedBounce && lastSegEndGt == 1);
                if (canBounceAgain && lastSegEndGt != 3)
                {
                    float dFactor = AITerrain.IsGroundTypeRough(lastSeg.EndGroundType) ? decrementFactor * 0.5f : decrementFactor;
                    lastDis = lastDis * dFactor;

                    while (lastDis > BounceThreshold && lastSegEndGt != 3)
                    {
                        evaluatedDistance += lastDis;
                        // 预测的抛物线
                        startPt = endPt;
                        endPt = AIMathf.ExtendLine(pt2, pt1, evaluatedDistance);
                        lastSegEndGt = terrain.GetTerrainTypeAtPoint(endPt);
                        // 预测的辅助线线段
                        segment = new AIGuideLineSegment(AIGuideLineSegmentType.ParabolaSegment, startPt, endPt,
                            lastSegEndGt,
                            true);
                        segementList.Add(segment);

                        dFactor = lastSeg.EndGroundType == 2 ? decrementFactor * 0.5f : decrementFactor;
                        lastDis = lastDis * dFactor;
                        if (AITerrain.IsGroundTypeOut(lastSegEndGt))
                        {
                            isOutAgain = true;
                            break;
                        }
                    }
                }


                if (!isOutAgain)
                {
//                    // 滚动一段，假设为弹跳的最后一次的距离
//                    if (segementList.Count > 1)
//                    {
//                        AIGuideLineSegment firstBounce = segementList[1];
//                        AIGuideLineSegment finalBounce = segementList[segementList.Count - 1];
//                        float firstLen = Vector2.Distance(firstBounce.StartPt, firstBounce.EndPt);
//                        float scrollDis;
//                        
//                        if (segementList.Count > 2 && Vector2.Distance(segementList[2].StartPt, segementList[2].EndPt) < firstLen * ScrollRatioThreshold)
//                        {
//                            // 极特殊不滚动的情况
//                            scrollDis = 0;
//                        }
//                        else
//                        {
//                            float scrollF = finalBounce.EndGroundType == 2
//                                ? RoughScrollFactor
//                                : (finalBounce.EndGroundType == 3 ? BunkerScrollFactor : ScrollFactor);
//                        
//                            scrollDis =  firstLen * (1f + scrollF);
//                        
//                            AIGuideLineSegment oriLastSegment = originGuideLine[originGuideLine.Count - 1];
//                            if (oriLastSegment.Type == AIGuideLineSegmentType.StraightLineSegment)
//                            {
//                                float guideLineScrollLen = Vector2.Distance(oriLastSegment.StartPt, oriLastSegment.EndPt);
//                                float fastScrollF = oriLastSegment.EndGroundType == 1
//                                    ? FastRollFactor
//                                    : (oriLastSegment.EndGroundType == 3 ? FastRollFactor * 0.2f : FastRollFactor * 0.5f);
//                                if (scrollDis < guideLineScrollLen)
//                                {
//                                    scrollDis = guideLineScrollLen * fastScrollF;
//                                }
//                            }
//                        }
//                        
//                        AIGuideLineSegment lastBounce = segementList[segementList.Count - 1];
//                        Vector2 stopPos = AIMathf.ExtendLine(lastBounce.StartPt, lastBounce.EndPt, scrollDis);
//                        // 预测的滚动线段
//                        segment = new AIGuideLineSegment(AIGuideLineSegmentType.StraightLineSegment, lastBounce.EndPt,
//                            stopPos, terrain.GetTerrainTypeAtPoint(stopPos), true);
//                        segementList.Add(segment);
//                    }
//                    else
//                    {
//                        AIGuideLineSegment firstBounce = segementList[0];
//                        float scrollF = firstBounce.EndGroundType == 2
//                            ? RoughScrollFactor
//                            : (firstBounce.EndGroundType == 3 ? BunkerScrollFactor : ScrollFactor);
//                        float scrollDis = Vector2.Distance(firstBounce.StartPt, firstBounce.EndPt) * 0.2f *
//                                          (1f + scrollF);
//                        
//                        AIGuideLineSegment oriLastSegment = originGuideLine[originGuideLine.Count - 1];
//                        if (oriLastSegment.Type == AIGuideLineSegmentType.StraightLineSegment)
//                        {
//                            float guideLineScrollLen = Vector2.Distance(oriLastSegment.StartPt, oriLastSegment.EndPt);
//                            float fastScrollF = oriLastSegment.EndGroundType == 2
//                                ? FastRollFactor * 0.5f
//                                : (oriLastSegment.EndGroundType == 3 ? FastRollFactor * 0.2f : FastRollFactor);
//                            if (scrollDis < guideLineScrollLen)
//                            {
//                                scrollDis = guideLineScrollLen * fastScrollF;
//                            }
//                        }
//                        
//                        Vector2 stopPos = AIMathf.ExtendLine(firstBounce.StartPt, firstBounce.EndPt, scrollDis);
//                        // 预测的滚动线段
//                        segment = new AIGuideLineSegment(AIGuideLineSegmentType.StraightLineSegment, firstBounce.EndPt,
//                            stopPos, terrain.GetTerrainTypeAtPoint(stopPos), true);
//                        segementList.Add(segment);
//                    }
                    AIGuideLineSegment lastBounce = segementList[segementList.Count - 1];
                    float scrollDis;
                    if (spin < 0)
                    {
                        // 下旋几乎不滚

                        scrollDis = BackSpinScrollDis;

                        AIGuideLineSegment oriLastSegment = originGuideLine[originGuideLine.Count - 1];
                        if (oriLastSegment.Type == AIGuideLineSegmentType.StraightLineSegment)
                        {
                            float guideLineScrollLen = Vector2.Distance(oriLastSegment.StartPt, oriLastSegment.EndPt);

                            scrollDis = Mathf.Max(scrollDis, guideLineScrollLen);
                        }
                    }
                    else
                    {
                        AIGuideLineSegment oriLastSegment = originGuideLine[originGuideLine.Count - 1];

                        if (oriLastSegment.Type == AIGuideLineSegmentType.StraightLineSegment)
                        {
                            scrollDis = Vector2.Distance(oriLastSegment.StartPt, oriLastSegment.EndPt);
                        }
                        else
                        {
                            scrollDis = BackSpinScrollDis;
                        }
                    }

                    Vector2 stopPos = AIMathf.ExtendLine(lastBounce.StartPt, lastBounce.EndPt, scrollDis);
                    segment = new AIGuideLineSegment(AIGuideLineSegmentType.StraightLineSegment, lastBounce.EndPt,
                        stopPos, terrain.GetTerrainTypeAtPoint(stopPos), true);
                    segementList.Add(segment);
                }
            }

            return segementList;
        }


        /// <summary>
        /// 预估的辅助线延长线中弹跳的次数
        /// </summary>
        /// <param name="guideLine">辅助线</param>
        /// <returns></returns>
        private static int GetGuideLinePredictBounceCount(List<AIGuideLineSegment> guideLine)
        {
            int bounceCount = 0;
            for (int i = 0; i < guideLine.Count; i++)
            {
                AIGuideLineSegment segment = guideLine[i];
                if (segment.IsPredictedPoint == true && segment.Type == AIGuideLineSegmentType.ParabolaSegment)
                {
                    bounceCount++;
                }
            }

            return bounceCount;
        }

        /// <summary>
        /// 判断普通场的辅助线是否处于危险地形
        /// </summary>
        /// <param name="guideLine"></param>
        /// <returns></returns>
        private static bool IsGuideLineInDanger(List<AIGuideLineSegment> guideLine)
        {
            bool isInDanger = false;
            for (int i = 0; i < guideLine.Count; i++)
            {
                AIGuideLineSegment segment = guideLine[i];

                if (AITerrain.IsGroundTypeDangerZone(segment.EndGroundType))
                {
                    isInDanger = true;
                    break;
                }
            }

            return isInDanger;
        }


        /// <summary>
        /// 判断Par3场的辅助线是否处于危险地形
        /// </summary>
        /// <param name="guideLine"></param>
        /// <returns></returns>
        private static bool IsGuideLineInDangerPar3(List<AIGuideLineSegment> guideLine)
        {
            bool isInDanger = false;
            for (int i = 0; i < guideLine.Count; i++)
            {
                AIGuideLineSegment segment = guideLine[i];

                if (AITerrain.IsGroundTypeOut(segment.EndGroundType))
                {
                    isInDanger = true;
                    break;
                }
            }

            return isInDanger;
        }


        /// <summary>
        /// 普通场选优逻辑
        /// </summary>
        /// <param name="guideLineList">待选辅助线集合</param>
        /// <returns></returns>
        private static AIGuideLine OptimalGuideLine(List<AIGuideLine> guideLineList)
        {
            List<AIGuideLine> safeGuideLineList = new List<AIGuideLine>();
            for (int i = 0; i < guideLineList.Count; i++)
            {
                AIGuideLine guideLine = guideLineList[i];
                if (!guideLine.FinalRegion.IsRegionInDanger())
                {
                    safeGuideLineList.Add(guideLine);
                }
            }

            if (safeGuideLineList.Count > 0)
            {
                safeGuideLineList.Sort(new AIGuideLineRegionComparer());
                return safeGuideLineList[0];
            }
            else if (guideLineList.Count > 0)
            {
                guideLineList.Sort(new AIGuideLineRegionComparer());
                return guideLineList[0];
            }
            else
            {
                return new AIGuideLine();
            }
        }

        /// <summary>
        /// Par3场选优逻辑
        /// </summary>
        /// <param name="guideLineList">待选辅助线集合</param>
        /// <returns></returns>
        private static AIGuideLine OptimalGuideLinePar3(List<AIGuideLine> guideLineList)
        {
            List<AIGuideLine> safeGuideLineList = new List<AIGuideLine>();
            for (int i = 0; i < guideLineList.Count; i++)
            {
                AIGuideLine guideLine = guideLineList[i];
                if (!guideLine.FinalRegion.IsRegionOut())
                {
                    safeGuideLineList.Add(guideLine);
                }
            }

            if (safeGuideLineList.Count > 0)
            {
                safeGuideLineList.Sort(new AIGuideLineRegionComparer());
                return safeGuideLineList[0];
            }
            else if (guideLineList.Count > 0)
            {
                guideLineList.Sort(new AIGuideLineRegionComparer());
                return guideLineList[0];
            }
            else
            {
                return new AIGuideLine();
            }
        }

        /// <summary>
        /// 根据预估最终停球位置和误差估计出一条小窄方形区域作为可能的落点区域。统计区域内的地形信息作为停球区域是否安全的判断标准
        /// </summary>
        /// <param name="terrain">地图</param>
        /// <param name="dropPoint">落点</param>
        /// <param name="lastStartPt">辅助线最后一段的起点</param>
        /// <param name="lastEndPt">辅助线最后一段的起点</param>
        /// <param name="errorWidth">落点区域宽（与辅助线最后一段的方向垂直）</param>
        /// <param name="errorHeight">落点区域长（与辅助线最后一段的方向相同）</param>
        /// <returns></returns>
        private static AIMapRegion EstimateRectStopRegion(AITerrain terrain, Vector2 dropPoint, Vector2 lastStartPt,
            Vector2 lastEndPt, float errorWidth, float errorHeight)
        {
            AIMapRegion evaluatedRegion = new AIMapRegion();

            evaluatedRegion.Center = lastEndPt;
            Vector2 direction1 = (lastEndPt - lastStartPt).normalized;
            Vector2 direction2 = AIMathf.RotateVectorByAngle(direction1, 90);
            Vector2 direction3 = (lastStartPt - lastEndPt).normalized;
            Vector2 direction4 = AIMathf.RotateVectorByAngle(direction3, 90);

            Vector2 pt1 = direction1 * errorWidth / 2f + direction2 * errorHeight + lastEndPt;
            Vector2 pt2 = direction3 * errorWidth / 2f + direction2 * errorHeight + lastEndPt;
            Vector2 pt3 = direction3 * errorWidth / 2f + direction4 * errorHeight + lastEndPt;
            Vector2 pt4 = direction1 * errorWidth / 2f + direction4 * errorHeight + lastEndPt;

            // 外接矩形边界
            float maxX = AIMathf.FloorToPercision(Mathf.Max(Mathf.Max(Mathf.Max(pt1.x, pt2.x), pt3.x), pt4.x),
                AITerrain.Percision);
            float maxY = AIMathf.FloorToPercision(Mathf.Max(Mathf.Max(Mathf.Max(pt1.y, pt2.y), pt3.y), pt4.y),
                AITerrain.Percision);
            float minX = AIMathf.FloorToPercision(Mathf.Min(Mathf.Min(Mathf.Min(pt1.x, pt2.x), pt3.x), pt4.x),
                AITerrain.Percision);
            float minY = AIMathf.FloorToPercision(Mathf.Min(Mathf.Min(Mathf.Min(pt1.y, pt2.y), pt3.y), pt4.y),
                AITerrain.Percision);

            for (float i = minX; i <= maxX; i = i + AITerrain.Percision)
            {
                for (float j = minY; j <= maxY; j = j + AITerrain.Percision)
                {
                    Vector2 pt = new Vector2(i - lastEndPt.x, j - lastEndPt.y);

                    bool isInRect = Mathf.Abs(Vector2.Dot(pt, direction1)) < errorWidth &&
                                    Mathf.Abs(Vector3.Dot(pt, direction2)) < errorHeight;
                    if (isInRect)
                    {
                        int mapType = terrain.GetTerrainTypeAtPoint(i, j);

//                        if (mapType < 0 || mapType > 9)
//                        {
//                            mapType = 0;
//                        }

                        evaluatedRegion.TerrainTypeArray[mapType]++;
                    }
                }
            }

            // 其他需要的值和后续的统计
            evaluatedRegion.DropPoint = dropPoint;
            evaluatedRegion.DistanceToGoal = Vector2.Distance(terrain.GoalPosition, lastEndPt);

            return evaluatedRegion;
        }


        /// <summary>
        /// 计算前后离不利地形的距离
        /// </summary>
        /// <param name="terrain">地图</param>
        /// <param name="startPt">最后一段辅助线的起点</param>
        /// <param name="endPt">最后一段辅助线的终点</param>
        /// <returns></returns>
        private static float CalcFrontNearestDangerZoneDistance(AITerrain terrain, Vector2 startPt, Vector2 endPt)
        {
            if (startPt == endPt)
            {
                return 0;
            }

            Vector2 firstDangerPt = Vector2.zero;

            const float percision = 0.5f;
            float dis = percision;
            int frontGroundType = 1;
            int backGroundType = 1;

            bool hasFindDangerZone = false;
            bool hasFrontFindDangerZone = false;
            bool hasBackFindDangerZone = false;

            Vector2 direction = (endPt - startPt).normalized;
            Vector2 normal1 = direction;
            Vector2 normal2 = AIMathf.RotateVectorByAngle(direction, 180f);
            float curDis = dis;
            Vector2 curFrontPt = Vector2.zero;
            Vector2 curBackPt = Vector2.zero;
            while (!hasFindDangerZone && (curFrontPt.x < terrain.MapMaxXIndex && curBackPt.x < terrain.MapMaxXIndex &&
                                          curFrontPt.y < terrain.MapMaxZIndex && curBackPt.y < terrain.MapMaxZIndex))
            {
                curFrontPt = endPt + normal2 * curDis;
                curBackPt = endPt + normal1 * curDis;
                curDis += dis;

                frontGroundType = terrain.GetTerrainTypeAtPoint(curFrontPt);
                backGroundType = terrain.GetTerrainTypeAtPoint(curBackPt);

                hasFrontFindDangerZone = AITerrain.IsGroundTypeDangerZone(frontGroundType);
                hasBackFindDangerZone = AITerrain.IsGroundTypeDangerZone(backGroundType);
                hasFindDangerZone = (hasFrontFindDangerZone || hasBackFindDangerZone);

                if (hasFrontFindDangerZone)
                {
                    firstDangerPt = curFrontPt;
                }

                if (hasBackFindDangerZone)
                {
                    firstDangerPt = curBackPt;
                }
            }

            float minDis = Vector2.Distance(firstDangerPt, endPt);

            return minDis;
        }


        /// <summary>
        /// 计算两侧离不利地形的距离
        /// </summary>
        /// <param name="terrain">地图</param>
        /// <param name="startPt">最后一段辅助线起点</param>
        /// <param name="endPt">最后一段辅助线终点</param>
        /// <returns></returns>
        private static float CalcNearestSideDangerZoneDistance(AITerrain terrain, Vector2 startPt, Vector2 endPt)
        {
            if (startPt == endPt)
            {
                return 0;
            }

            Vector2 firstDangerPt = Vector2.zero;

            const float percision = 0.5f;
            float dis = percision;
            int leftGroundType = 1;
            int rightGroundType = 1;

            bool hasFindDangerZone = false;
            bool hasLeftFindDangerZone = false;
            bool hasRightFindDangerZone = false;

            Vector2 direction = (endPt - startPt).normalized;
            Vector2 normal1 = AIMathf.RotateVectorByAngle(direction, 90f);
            Vector2 normal2 = AIMathf.RotateVectorByAngle(direction, -90f);
            float curDis = dis;
            Vector2 curLeftPt = Vector2.zero;
            Vector2 curRightPt = Vector2.zero;
            while (!hasFindDangerZone && (curLeftPt.x < terrain.MapMaxXIndex && curRightPt.x < terrain.MapMaxXIndex &&
                                          curLeftPt.y < terrain.MapMaxZIndex && curRightPt.y < terrain.MapMaxZIndex))
            {
                curLeftPt = endPt + normal2 * curDis;
                curRightPt = endPt + normal1 * curDis;
                curDis += dis;

                leftGroundType = terrain.GetTerrainTypeAtPoint(curLeftPt);
                rightGroundType = terrain.GetTerrainTypeAtPoint(curRightPt);

                hasLeftFindDangerZone = AITerrain.IsGroundTypeDangerZone(leftGroundType);
                hasRightFindDangerZone = AITerrain.IsGroundTypeDangerZone(rightGroundType);
                hasFindDangerZone = (hasLeftFindDangerZone || hasRightFindDangerZone);

                if (hasLeftFindDangerZone)
                {
                    firstDangerPt = curLeftPt;
                }

                if (hasRightFindDangerZone)
                {
                    firstDangerPt = curRightPt;
                }
            }

            float minDis = Vector2.Distance(firstDangerPt, endPt);

            return minDis;
        }
    }


    /// <summary>
    /// AI处理后辅助线的数据结构
    /// </summary>
    public class AIGuideLine
    {
        public float SideSpin;

        public float TopBackSpin;

        public List<AIGuideLineSegment> FinalGuideLine;

        public int BounceCount;

        public AIMapRegion FinalRegion;

        public bool HasGuideLineCrossGoal;

        public bool IsInvalid;

        /// 0：空中 1：地面 2：撞到树上 3：进洞 4：在洞的边缘 5：出界
        public int FinalState;

        public static AIGuideLine CreateInvalidGuideLine()
        {
            AIGuideLine guideLine = new AIGuideLine
            {
                SideSpin = 0,
                TopBackSpin = 0,
                FinalGuideLine = new List<AIGuideLineSegment>(),
                FinalRegion = AIMapRegion.CreateInvalidRegion(),
                FinalState = 5,
                HasGuideLineCrossGoal = false,
                IsInvalid = true
            };
            return guideLine;
        }
    }


    /// <summary>
    /// AI辅助线的线段，一段抛物线或者一段滚动直线
    /// </summary>
    public class AIGuideLineSegment
    {
        public AIGuideLineSegmentType Type;

        public Vector2 StartPt;

        public Vector2 EndPt;

        public int EndGroundType;

        /// 是否为预估的落点，true：预估的，false：辅助线给出的
        public bool IsPredictedPoint;

        public AIGuideLineSegment(AIGuideLineSegmentType t, Vector2 sPt, Vector2 ePt, int eType, bool isP)
        {
            Type = t;
            StartPt = sPt;
            EndPt = ePt;
            EndGroundType = eType;
            IsPredictedPoint = isP;
        }
    }

    /// <summary>
    /// 辅助线片段的类型枚举
    /// </summary>
    public enum AIGuideLineSegmentType
    {
        ParabolaSegment, // 抛物线
        StraightLineSegment // 直线
    }
}

#endif