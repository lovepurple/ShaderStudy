#if AI_TEST
using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using SPhysics;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEditor;

namespace Assets.Scripts.ai
{

    public enum TestSpinType
    {
        TestSpinTypeNormal,
        TestSpinTypeLeftSpin,
        TestSpinTypeRightSpin,
        TestSpinTypeTopSpin,
        TestSpinTypeBackSpin
    }
    
    public class AIStrategy : IStrategy
    {
        /// <summary>
        /// 地图模型
        /// </summary>
        private AITerrain _terrain;

        /// <summary>
        /// 玩家能力指数模型
        /// </summary>
        private AIPlayer _player;

        /// <summary>
        /// 物理
        /// </summary>
        private PhysicsDataManager _physicsManager;

        /// <summary>
        /// 物理（AI专用）
        /// </summary>
        private AIPhysicsDataManager _aiPhysicsManager;

        /// <summary>
        /// 风模型
        /// </summary>
        private AIWind _wind;

        /// <summary>
        /// 球杆工厂
        /// </summary>
        private AIClubJsonFactory _clubFactory;

        /// <summary>
        /// Lua状态
        /// </summary>
        private LuaInterface.LuaState _luaState;

        /// <summary>
        /// Lua表
        /// </summary>
        private LuaInterface.LuaTable _luaTable;

        /// <summary>
        /// 扫描线角度扫描精度
        /// </summary>
        private const float ScanAngelPercision = 1;

        /// <summary>
        /// 场景名称
        /// </summary>
        private string _sceneName;

        /// <summary>
        /// Debuff系数
        /// </summary>
        private const float DebuffFactor = 1f;

        /// <summary>
        /// 球弹性系数
        /// </summary>
        private float _ballBounceness = 1f;

        /// <summary>
        /// 主要测试球杆的旋度类型
        /// </summary>
        public static TestSpinType MainTestSpinType = TestSpinType.TestSpinTypeNormal;


        /// <summary>
        /// 主要测试球杆的id
        /// </summary>
        public static int MainTestClubId = 0;

        /// <summary>
        /// 主要测试的球杆的lv
        /// </summary>
        public static int MainTestClubLv = 1;
        
        /// <summary>
        /// 是否当前回合就是主要测试的球杆
        /// </summary>
        public static bool isCurActionMainTestClub = false;

        /// <summary>
        /// 主要测试球杆在那回合发挥作用
        /// </summary>
        private int _mainTestAction = 0;
        
        /// <summary>
        /// 程序入口
        /// </summary>
        /// <param name="physics"></param>
        /// <param name="aiPhysics"></param>
        /// <param name="sceneName"></param>
        public void Init(PhysicsDataManager physics, AIPhysicsDataManager aiPhysics, string sceneName)
        {
            // 存值
            _physicsManager = physics;
            _aiPhysicsManager = aiPhysics;
            _sceneName = sceneName;

            // 检查宏定义
            CheckMacroSetting();

            // 初始化Lua相关配置，后面调用Lua层计算位置偏差和侧旋用到Lua层
            InitLua();

            // 物理参数初始化
            InitPhysicsParams();
            
//            int startType = _physicsManager.getGroundType(-180.2f, 141.3f);
            AreaToAreaShotSimulation(31, 1, false, true, false, false, 1f);
//            AreaToAreaShotSimulation(31, 10, false, false, false, true, 1f);
//             bool isTestCaseMode = false;
//
//             if (isTestCaseMode)
//             {
//                 // 测试用例
//                 SinglePointTest();
//             }
//             else
//             {
//                 // 模拟打球
//                 StartGameSimulation();
//             }
        }


        /// <summary>
        /// 测试专用
        /// </summary>
        private void SinglePointTest()
        {
            // 测试专用
            _player = new AIPlayer(100, 80, 80, 100, 100, 100);
            _terrain = new AITerrain();
            _terrain.LoadMapData(_physicsManager, _sceneName, GetMapName());

            _physicsManager.AiInit();
//            Vector2 startPos = _terrain.StartPosition;
//            _physicsManager.AISetBallPosition(new Vector2(9.276371955871582f, -157.76046752929688f));
//            Vector2 startPos = new Vector2(44.7354496520996097f, -99.39167785644531f);

            Vector2 dropPt = new Vector2(-245.6f, 127f);
//            int groundType = _terrain.GetTerrainTypeAtPoint(dropPt);
//            int gt = _physicsManager.getGroundType(dropPt.x, dropPt.y);
//            
//            Vector2 first = new Vector2(0.081470944f, -68.4221802f);
//            int fGt = _terrain.GetTerrainTypeAtPoint(first);
//            int fg = _physicsManager.getGroundType(first.x, first.y);
//            
//            Vector2 oriDropPt = new Vector2(-1.0f, -78.0f);
            SPhysics.AiLine line =
                RequestGuideLine(_terrain.StartPosition, dropPt, 0f, 2.5f, 0.26f);
            AIGuideLine guide = AIGuideLineHelper.CalcPredictGuideLine(_terrain.StartPosition, line, 2.5f, dropPt,
                _terrain, 0f, 0f, 0.26f, _player, false);
//            SPhysics.ShotInfo shot = _physicsManager.Shot(dropPt, 0.47077473998069765f, -0.4000000059604645f, 0.26f, dropPt, 1f);
        }


        /// <summary>
        /// 检查宏定义
        /// </summary>
        private void CheckMacroSetting()
        {
            // 判断是否开启AI_TEST宏
            bool isInAiTest = false;
#if AI_TEST
            isInAiTest = true;
#endif
            Assert.IsTrue(isInAiTest, "AI需要开启AI_TEST宏");

            // 判断是否关闭AI_SHOT宏
            bool isInAiShot = false;
#if AI_SHOT
            isInAiShot = true;
#endif
            Assert.IsFalse(isInAiShot, "AI不能开启AI_SHOT宏");
        }


        /// <summary>
        /// 初始化必要的物理参数
        /// </summary>
        private void InitPhysicsParams()
        {
            PhysicsManager.Instance.ballController.ballBounciness = _ballBounceness;
            _clubFactory = new AIClubJsonFactory();
        }


        /// <summary>
        /// 初始化Lua环境
        /// </summary>
        private void InitLua()
        {
            _luaState = new LuaInterface.LuaState();
            _luaState.Start();
            _luaTable = _luaState.DoFile<LuaInterface.LuaTable>("Assets/Scripts/Lua/Tools/PhysicsHelper");
        }


        /// <summary>
        /// 判断游戏结束的条件
        /// </summary>
        /// <param name="hitCount">已经击打的杆数</param>
        /// <param name="arriveGroundType">上一杆停球地形材质</param>
        /// <param name="arrivePointInfo">上一杆击球结果信息</param>
        private delegate bool GameFinishedConditionDelegate(int hitCount, int arriveGroundType,
            SPhysics.ShotInfo arrivePointInfo);


        /// <summary>
        /// 加载地图信息
        /// </summary>
        /// <returns>加载地图耗时</returns>
        private double LoadMapData()
        {
            double timeForLoadMap;

            DateTime startTimeStamp = DateTime.Now;

            // 加载地图
            _terrain = new AITerrain();
            _terrain.LoadMapData(_physicsManager, _sceneName, GetMapName());

            // 计时
            DateTime endTimeStamp = DateTime.Now;
            TimeSpan elipseTime = endTimeStamp.Subtract(startTimeStamp).Duration();
            timeForLoadMap = elipseTime.TotalMilliseconds;

            return timeForLoadMap;
        }


        /// <summary>
        /// 模拟打球
        /// </summary>
        private void StartGameSimulation()
        {
            // 加载地图
            double timeForLoadMap = LoadMapData();
            // 每场测试打100次比赛
            int shotCount = 100;
            // 初始化玩家模型
            if (AIModeSwitch.IsInPar3Mode)
            {
                // 三杆场
                _player = new AIPlayer(80, 80, 80, 100, 100, 100);
                SimulateGamePlanGroup(timeForLoadMap, shotCount,
                    (hitCount, arriveGroundType, arrivePointInfo) => hitCount < 1);
            }
            else
            {
                _player = new AIPlayer(80, 80, 80, 100, 100, 100);
                SimulateGamePlanGroup(timeForLoadMap, shotCount, (hitCount, arriveGroundType, arrivePointInfo) =>
                    hitCount < 3 && arriveGroundType != 4 && arriveGroundType != 5 && arriveGroundType != 6 &&
                    !arrivePointInfo.inhole && !(hitCount == 2 && arriveGroundType != 1));
            }
        }

        /// <summary>
        /// 配置实验数据组，根据需要自行调整循环次数和球杆的配置
        /// </summary>
        /// <param name="timeForLoadMap"></param>
        /// <param name="shotCount"></param>
        /// <param name="condition"></param>
        private void SimulateGamePlanGroup(double timeForLoadMap, int shotCount,
            GameFinishedConditionDelegate condition)
        {
            for (int groupCount = 0; groupCount < 1; groupCount++)
            {
                int dId = 0, wId = 0;
                int maxA = 0, maxB = 0;
                string dirName = "";
                switch (groupCount)
                {
                    // 实验组1
                    case 0:
                        dId = 32;
                        wId = 25;
                        maxA = 2;
                        maxB = 7;
                        dirName = "club_32";
                        MainTestSpinType = TestSpinType.TestSpinTypeLeftSpin;
                        MainTestClubId = dId;
                        MainTestClubLv = maxA;
                        _mainTestAction = 2;
                        _ballBounceness = 1f;
                        break;
                    // 实验组2
                    case 1:
                        dId = 25;
                        wId = 2;
                        maxA = 3;
                        maxB = 1;
                        dirName = "club_25";
                        _ballBounceness = 1f;
                        break;
                    // 实验组3
                    case 2:
                        dId = 7;
                        wId = 8;
                        maxA = 5;
                        maxB = 1;
                        dirName = "club_7";
                        _ballBounceness = 1f;
                        break;
                    // 实验组2
                    case 3:
                        dId = 19;
                        wId = 2;
                        maxA = 5;
                        maxB = 1;
                        dirName = "club_19";
                        _ballBounceness = 1f;
                        break;
                    // 实验组3
                    case 4:
                        dId = 31;
                        wId = 8;
                        maxA = 5;
                        maxB = 1;
                        dirName = "club_31";
                        _ballBounceness = 1f;
                        break;
                }

                // 设置球的弹性
                PhysicsManager.Instance.ballController.ballBounciness = _ballBounceness;

                // 设置球杆组合
                for (int clubALv = 2; clubALv <= maxA; clubALv += 1)
                {
                    for (int clubBLv = 7; clubBLv <= maxB; clubBLv += 1)
                    {
                        _clubFactory.SelectedDriveClubId = wId;
                        _clubFactory.SelectedDriveClubLv = clubBLv;
                        _clubFactory.SelectedWoodClubId = dId;
                        _clubFactory.SelectedWoodClubLv = clubALv;
                        
                        int fileIndex = clubALv;
                        string logDirName = GetMapName() + "/" + dirName;
                        string logFileName = "AILog_" + GetMapName() + "_" + fileIndex;

                        SimulateGame(timeForLoadMap, shotCount, logDirName, logFileName, condition);
                    }
                }
            }
        }


        private void AreaToAreaShotSimulation(int clubId, int clubLv, bool isLeftSideSpin, bool isRightSideSpin, bool isTopSpin, bool isBackSpin, float ballBounceness)
        {
            
            _terrain = new AITerrain();
            _terrain.LoadMapData(_physicsManager, _sceneName, GetMapName());
            
            AIJsonClub club = _clubFactory.GetClubByIdAndLevel(clubId, clubLv);
            float sideSpin = isLeftSideSpin ? club.SideSpin : (isRightSideSpin ? -club.SideSpin : 0);
            float topSpin = isTopSpin ? club.TopBackSpin : (isBackSpin ? -club.TopBackSpin : 0);

            float suffix = (isLeftSideSpin || isRightSideSpin) ? sideSpin : topSpin;
            string dirName = "club_" + clubId + "_" + clubLv + "_" + suffix;
            
            string logDirName = GetMapName() + "/" + dirName;
            string logFileName = "AILog_" + GetMapName();
            

            _ballBounceness = ballBounceness;
            PhysicsManager.Instance.ballController.ballBounciness = _ballBounceness;
            
            List<Vector2> startArea = _terrain.GeneratePointArrayByAreaName("StartArea", 1);
            List<Vector2> endArea = _terrain.GeneratePointArrayByAreaName("EndArea",1);
            _player = new AIPlayer(80, 80, 80, 100, 100, 100);
            List<AILogInfo.AIGameLogInfo> gameInfoList = new List<AILogInfo.AIGameLogInfo>();
            int totalHitCount = 0;
            int index = 0;
            
            _wind = new AIWind(0, Vector2.zero);
            // 每个文件打shotCount场比赛
            foreach (var ptStart in startArea)
            {
                foreach (var ptEnd in endArea)
                {
                    if (Vector2.Distance(ptStart, ptEnd) > club.Force || _terrain.GetTerrainTypeAtPoint(ptStart) == 3 || AITerrain.IsGroundTypeOut(_terrain.GetTerrainTypeAtPoint(ptStart)))
                    {
                        continue;
                    }
                    index++;
                    // 场次
                    List<AILogInfo.AIGameActionLogInfo> logActionList =
                        new List<AILogInfo.AIGameActionLogInfo>();

                    // 随机风力
//                    RandomWind();
                    
                    _physicsManager.AISetBallPosition(ptStart);
                    
                    AIMapRegion region = new AIMapRegion();
                    region.DropPoint = ptEnd;
                    region.SideSpin = sideSpin;
                    region.TopBackSpin = topSpin;
                    region.Center = ptEnd;
                    region.WindOffsetDropPt = ptEnd;
                    AIAction action = GenerateAction(region, club, 0, 1f, false, ptStart);
                    action.ClubEmitCoefficient = club.Angle;
                    SPhysics.ShotInfo arrivePointInfo = ShotBallByAction(action);
                    int arriveGroundType = _terrain.GetTerrainTypeAtPoint(arrivePointInfo.finalPosition);
                    

                    AILogInfo.AIGameActionLogInfo logAction = new AILogInfo.AIGameActionLogInfo();

                    // 日志记录AIAction的决策信息
                    AILogger.RecordActionLog(logAction, action);
                    // 日志记录实际击球结果的信息
                    AILogger.RecordArriveLog(logAction, arrivePointInfo, ptStart,
                        _terrain.GoalPosition, arriveGroundType, 1);
                    logActionList.Add(logAction);
                    
                    // 总击球次数
                    totalHitCount += 1;

                    // 日志记录一轮比赛的相关信息
                    AILogInfo.AIGameLogInfo gameInfo = new AILogInfo.AIGameLogInfo();
                    AILogger.RecordGameInfoLog(gameInfo, _wind, 1, index, logActionList);
                    gameInfoList.Add(gameInfo);
                }
            }

            AILogInfo logInfo = new AILogInfo(gameInfoList.Count, 0, 0);
            // 日志记录总结信息
            AILogger.RecordLogInfo(logInfo, totalHitCount, totalHitCount, 0, 0, _player,
                GetMapName(), _terrain, gameInfoList);

            // 写入到磁盘中
            AILogger.LogToDisk(logInfo, logDirName, logFileName);
        }

        /// <summary>
        /// 模拟打球并记录log
        /// </summary>
        /// <param name="timeForLoadMap">加载地图耗时，只用来记录log</param>
        /// <param name="shotCount">总共场数</param>
        /// <param name="logDirName">log文件目录</param>
        /// <param name="logFileName">log文件名</param>
        /// <param name="condition">判断游戏结束的条件</param>
        /// <param name="useBounceBallHitRound">可选，是否单独哪一杆使用弹性球，比如第2杆则传2</param>
        private void SimulateGame(double timeForLoadMap, int shotCount, string logDirName, string logFileName,
            GameFinishedConditionDelegate condition, int useBounceBallHitRound = -1)
        {
            DateTime startTimeStamp = DateTime.Now;
            int totalHitCount = 0;
            List<AILogInfo.AIGameLogInfo> gameInfoList = new List<AILogInfo.AIGameLogInfo>();
            Dictionary<int, int> clubTypeDict = new Dictionary<int, int>();
            Dictionary<int, int> clubIdDict = new Dictionary<int, int>();

            // 每个文件打shotCount场比赛
            for (int i = 0; i < shotCount; i++)
            {
                SPhysics.ShotInfo lastArrivePointInfo = new ShotInfo
                {
                    finalPosition = _terrain.StartPosition,
                    firstBouncePosition = _terrain.StartPosition,
                    inhole = false
                };

                SPhysics.ShotInfo arrivePointInfo = new ShotInfo
                {
                    finalPosition = _terrain.StartPosition,
                    firstBouncePosition = _terrain.StartPosition,
                    finalBouncePosition = _terrain.StartPosition,
                    inhole = false,
                    isBallOut = false
                };

                int arriveGroundType = 8;
                // 该次比赛杆数
                int hitCount = 0;
                // 场次
                int round = i + 1;
                List<AILogInfo.AIGameActionLogInfo> logActionList =
                    new List<AILogInfo.AIGameActionLogInfo>();

                // 随机风力
                RandomWind();
                // 球放回起点
                _physicsManager.AiInit();

                while (condition(hitCount, arriveGroundType, arrivePointInfo))
                {
                    // 打球决策
                    isCurActionMainTestClub = _mainTestAction == hitCount;
                    AIAction action = CalcShotAction(lastArrivePointInfo.finalPosition);
                    arrivePointInfo = ShotBallByAction(action);
                    arriveGroundType = _terrain.GetTerrainTypeAtPoint(arrivePointInfo.finalPosition);
                    hitCount++;

                    AILogInfo.AIGameActionLogInfo logAction = new AILogInfo.AIGameActionLogInfo();

                    // 日志记录AIAction的决策信息
                    AILogger.RecordActionLog(logAction, action);
                    // 日志记录实际击球结果的信息
                    AILogger.RecordArriveLog(logAction, arrivePointInfo, lastArrivePointInfo.finalPosition,
                        _terrain.GoalPosition, arriveGroundType, hitCount);
                    logActionList.Add(logAction);

                    // 球杆使用情况的数据统计        
                    AILogger.AddClubInfoToDictionary(clubIdDict, action.ClubId);
                    AILogger.AddClubInfoToDictionary(clubTypeDict, action.ClubType);


                    bool isBallOut = arrivePointInfo.isBallOut;
                    // 是否掉到水里或者出界
                    if (!isBallOut)
                    {
                        lastArrivePointInfo = arrivePointInfo;
                    }
                    else
                    {
                        // 把球重置到上一次到达的位置
                        _physicsManager.AISetBallPosition(lastArrivePointInfo.finalPosition);
                    }
                }

                // 总击球次数
                totalHitCount += hitCount;

                // 日志记录一轮比赛的相关信息
                AILogInfo.AIGameLogInfo gameInfo = new AILogInfo.AIGameLogInfo();
                AILogger.RecordGameInfoLog(gameInfo, _wind, hitCount, round, logActionList);
                gameInfoList.Add(gameInfo);
            }

            // 运行时间统计
            DateTime endTimeStamp = DateTime.Now;
            TimeSpan elipseTime = endTimeStamp.Subtract(startTimeStamp).Duration();
            double timeForGame = elipseTime.TotalMilliseconds;

            AILogInfo logInfo = new AILogInfo(gameInfoList.Count, clubIdDict.Count, clubTypeDict.Count);
            // 日志记录总结信息
            AILogger.RecordLogInfo(logInfo, shotCount, totalHitCount, timeForLoadMap, timeForGame, _player,
                GetMapName(), _terrain, gameInfoList);
            // 日志记录球杆统计信息
            AILogger.RecordClubIdLogInfo(logInfo, clubIdDict);
            AILogger.RecordClubTypeLogInfo(logInfo, clubTypeDict);

            // 写入到磁盘中
            AILogger.LogToDisk(logInfo, logDirName, logFileName);
        }


        /// <summary>
        /// 随机风力和风向
        /// </summary>
        private void RandomWind()
        {
            float randomX = UnityEngine.Random.Range(0f, 50f) *
                            (UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1);
            float randomY = UnityEngine.Random.Range(0f, 50f) *
                            (UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1);
            float randomWindLv = (float) UnityEngine.Random.Range(0F, 3000f) * 0.001F;
            _wind = new AIWind(randomWindLv, new Vector2(randomX, randomY));
        }

        /// <summary>
        /// 根据Action击球
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private SPhysics.ShotInfo ShotBallByAction(AIAction action)
        {
            _physicsManager.SetLayerTrunk(LayerMask.GetMask("trunk"));
            _aiPhysicsManager.SetLayerTrunk(LayerMask.GetMask("trunk"));
            PhysicsManager.Instance.physicsDataManager.SetLayerTrunk(LayerMask.GetMask("trunk"));
            PhysicsManager.Instance.oldphysicsDataManager.SetLayerTrunk(LayerMask.GetMask("trunk"));

            _physicsManager.SetLayerLeaf(LayerMask.GetMask("leaf"));
            _aiPhysicsManager.SetLayerLeaf(LayerMask.GetMask("leaf"));
            PhysicsManager.Instance.physicsDataManager.SetLayerLeaf(LayerMask.GetMask("leaf"));
            PhysicsManager.Instance.oldphysicsDataManager.SetLayerLeaf(LayerMask.GetMask("leaf"));
            return _physicsManager.Shot(action.TargetPosition, action.SideSpinAfterMoving, action.TopBackSpin,
                action.ClubEmitCoefficient,
                action.TargetPositionAfterMoving, action.Power, _ballBounceness);
        }


        /// <summary>
        /// 请求辅助线
        /// </summary>
        /// <param name="start_position">发球点</param>
        /// <param name="target_position">目标点</param>
        /// <param name="top_spin">上旋</param>
        /// <param name="cue_bounceline">辅助线长度</param>
        /// <param name="clubEmitCoefficient">抛角</param>
        /// <param name="side_spin">侧旋</param>
        /// <returns></returns>
        private SPhysics.AiLine RequestGuideLine(Vector2 start_position, Vector2 target_position, float top_spin,
            float cue_bounceline, float clubEmitCoefficient, float side_spin = 0)
        {
            _physicsManager.SetLayerTrunk(LayerMask.GetMask("through_trunk"));
            _aiPhysicsManager.SetLayerTrunk(LayerMask.GetMask("through_trunk"));
            PhysicsManager.Instance.physicsDataManager.SetLayerTrunk(LayerMask.GetMask("through_trunk"));
            PhysicsManager.Instance.oldphysicsDataManager.SetLayerTrunk(LayerMask.GetMask("through_trunk"));


            _physicsManager.SetLayerLeaf(LayerMask.GetMask("through_leaf"));
            _aiPhysicsManager.SetLayerLeaf(LayerMask.GetMask("through_leaf"));
            PhysicsManager.Instance.physicsDataManager.SetLayerLeaf(LayerMask.GetMask("through_leaf"));
            PhysicsManager.Instance.oldphysicsDataManager.SetLayerLeaf(LayerMask.GetMask("through_leaf"));

            return _aiPhysicsManager.Line(start_position, target_position, top_spin, cue_bounceline,
                clubEmitCoefficient, side_spin);
        }


        /// <summary>
        /// 根据开球地形材质来选球杆
        /// </summary>
        /// <param name="shotPoint">击球位置</param>
        /// <returns></returns>
        private AIJsonClub ChooseClub(Vector2 shotPoint)
        {
            if (isCurActionMainTestClub)
            {
                return _clubFactory.GetClubByIdAndLevel(MainTestClubId, MainTestClubLv);
            }
            else
            {
                int groundType = _terrain.GetTerrainTypeAtPoint(shotPoint);
                return _clubFactory.ChooseClub(groundType, Vector2.Distance(shotPoint, _terrain.GoalPosition));
            }
            //            return _clubFactory.Create3W();
//            return _clubFactory.Create1W();
//            return _clubFactory.CreateBigIron();
        }


        /// <summary>
        /// 切杆判断
        /// </summary>
        /// <param name="shotPoint">发球点</param>
        /// <param name="useClub">切换前的球杆</param>
        /// <returns></returns>
        private SwitchClubInfo SwitchClub(Vector2 shotPoint, AIJsonClub useClub)
        {
            int gt = _terrain.GetTerrainTypeAtPoint(shotPoint);
            SwitchClubInfo result = _clubFactory.SwichClub(gt, useClub);
            return result;
        }


        /// <summary>
        /// 计算打球决策的Action
        /// </summary>
        /// <param name="startPos">开球位置</param>
        /// <returns></returns>
        private AIAction CalcShotAction(Vector2 startPos)
        {
            // 根据发球点选择球杆
            AIJsonClub useClub = ChooseClub(startPos);

            float powerRange = useClub.CalcPowerRange(false, _player.AdventureLevel);
            float minDis = _clubFactory.GetMinDistanceByClubType(useClub.Type);

            // 生成扫描线
            SortedList<int, SortedList<Vector2, Vector2>> angelLineDict = GenerateAngleLineDict(startPos,
                _terrain.GoalPosition,
                minDis, powerRange);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 1. 尝试正常打法


            // 正常打法遍历扫描线
            AIGuideLineResult regionList =
                EnumerateAngelLineDict(angelLineDict, startPos, _terrain.GoalPosition, useClub);
            // 选择最优打法
            AIMapRegion optimalRegion = OptimalDecisionForNormal(regionList);


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 2.尝试Power > 1f的冒险打法

            // 首先要玩家模型的冒险水平 > 60才会尝试，保守玩家绝不这么打
            bool isPlayerLvSatisfied = _player.AdventureLevel > 60f;

            // 正常打法没法打
            bool isNormalRegionInvalid = optimalRegion == null;

            if (isPlayerLvSatisfied && isNormalRegionInvalid)
            {
                AIMapRegion optimalAdventureRegion = CalcAdventrueShotRegion(startPos, useClub);
                // 冒险区域可行
                if (optimalAdventureRegion != null)
                {
                    AIAction adventureAction = CalcAdventureShotAction(optimalAdventureRegion, startPos, useClub,
                        isNormalRegionInvalid, optimalRegion == null ? 0 : optimalRegion.DistanceToGoal);
                    if (adventureAction != null)
                    {
                        return adventureAction;
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 3.尝试Power < 1f 的打法

            // 是否当前选择的球杆最小击打范围就已经超过球洞了
            bool isAlreadyTooFar =
                Vector2.Distance(startPos, _terrain.GoalPosition) <
                _clubFactory.GetMinDistanceByClubType(useClub.Type) && useClub.Type != 5 && useClub.Type != 6;
            int startGroundType = _terrain.GetTerrainTypeAtPoint(startPos);
            if (isNormalRegionInvalid || isAlreadyTooFar)
            {
                if (startGroundType == 8)
                {
                    // 开球区只能考虑轻点打
                    AIMapRegion optimalLightlyRegion = CalcLightlyShotRegion(startPos, useClub);
                    AIAction lightAction = CalcLightShotAction(optimalLightlyRegion, startPos, useClub);
                    if (lightAction != null)
                    {
                        return lightAction;
                    }
                }
                else
                {
                    // 非开球区换杆
                    AIAction switchAction = CalcSwitchClubAction(startPos, useClub);
                    if (switchAction != null)
                    {
                        return switchAction;
                    }

                    // 区间连续，必须能找到合适的落点，不然肯定是地图错了
                    Debug.Log("switch failed: Start Pos x = " + startPos.x + " y = " + startPos.y + " gt = " +
                              startGroundType + " club type = " + useClub.Type);
                }

                // 最后考虑是不是所有情况都被排除了，那没办法了，朝着球洞随便打一杆
                optimalRegion =
                    AIMapRegion.CreateFaultTolerantRegion(startPos, _terrain.GoalPosition, useClub.MaxDistance, _wind,
                        _player);
            }


            return GenerateAction(optimalRegion, useClub, 0, 1f, false, startPos);
        }


        /// <summary>
        /// 计算Power小于1f打法的Action
        /// </summary>
        /// <param name="optimalLightlyRegion"></param>
        /// <param name="startPos"></param>
        /// <param name="useClub"></param>
        /// <returns></returns>
        private AIAction CalcLightShotAction(AIMapRegion optimalLightlyRegion, Vector2 startPos, AIJsonClub useClub)
        {
            float lightlyRatio = 0f;
            bool isLightlyRegionInvalid = optimalLightlyRegion == null;
            float minDis = _clubFactory.GetMinDistanceByClubType(useClub.Type);
            if (!isLightlyRegionInvalid)
            {
                Vector2 windoffset = optimalLightlyRegion.WindOffsetDropPt - optimalLightlyRegion.DropPoint;
                Vector2 oldWindOffsetDropPt = optimalLightlyRegion.WindOffsetDropPt;
                optimalLightlyRegion.DropPoint =
                    AIMathf.CalcShotRangeEdgeDropPoint(startPos, optimalLightlyRegion.WindOffsetDropPt,
                        optimalLightlyRegion.DropPoint, minDis);
                optimalLightlyRegion.WindOffsetDropPt = optimalLightlyRegion.DropPoint + windoffset;

                float disNewDrop = Vector2.Distance(startPos, optimalLightlyRegion.WindOffsetDropPt);
                float disOld = Vector2.Distance(startPos, oldWindOffsetDropPt);
                float newPower = disOld / disNewDrop;
                lightlyRatio = 1f - newPower;

                return GenerateAction(optimalLightlyRegion, useClub, lightlyRatio, newPower, false, startPos);
            }

            return null;
        }


        /// <summary>
        /// 计算冒险打法Action
        /// </summary>
        /// <param name="optimalAdventureRegion"></param>
        /// <param name="startPos"></param>
        /// <param name="useClub"></param>
        /// <param name="isNormalRegionInvalid"></param>
        /// <param name="normalDisToGoal"></param>
        /// <returns></returns>
        private AIAction CalcAdventureShotAction(AIMapRegion optimalAdventureRegion, Vector2 startPos,
            AIJsonClub useClub, bool isNormalRegionInvalid, float normalDisToGoal)
        {
            const float diffFactor = 1.1f;
            float powerRange = useClub.CalcPowerRange(false, _player.AdventureLevel);
            float adventurePowerRange = useClub.CalcPowerRange(true, _player.AdventureLevel);

            Vector2 windOffset = optimalAdventureRegion.WindOffsetDropPt - optimalAdventureRegion.DropPoint;
            Vector2 oldWindOffsetPt = optimalAdventureRegion.WindOffsetDropPt;
            optimalAdventureRegion.DropPoint =
                AIMathf.CalcShotRangeEdgeDropPoint(startPos, optimalAdventureRegion.WindOffsetDropPt,
                    optimalAdventureRegion.DropPoint, powerRange);
            optimalAdventureRegion.WindOffsetDropPt = optimalAdventureRegion.DropPoint + windOffset;

            float disStart2Drop = Vector2.Distance(optimalAdventureRegion.WindOffsetDropPt, startPos);
            float disStart2OldDrop = Vector2.Distance(oldWindOffsetPt, startPos);
            float newPower = disStart2OldDrop / disStart2Drop;
            float adventureAspect = newPower - 1f;

            if (isNormalRegionInvalid)
            {
                return GenerateAction(optimalAdventureRegion, useClub, adventureAspect,
                    newPower, false, startPos);
            }

            // 最终落点的中心离球洞距离大于某阈值才有用，而且玩家的冒险指数大于某阈值才会冒险
            float diffStop = normalDisToGoal - optimalAdventureRegion.DistanceToGoal;
            float diffPowerRange = adventurePowerRange - powerRange;
            if (diffStop > diffPowerRange * diffFactor)
            {
                return GenerateAction(optimalAdventureRegion, useClub, adventureAspect,
                    newPower, false, startPos);
            }

            return null;
        }


        /// <summary>
        /// 计算切杆打法Action
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="useClub"></param>
        /// <returns></returns>
        private AIAction CalcSwitchClubAction(Vector2 startPos, AIJsonClub useClub)
        {
            SwitchClubInfo switchInfo = SwitchClub(startPos, useClub);
            while (switchInfo.IsSuccessed)
            {
                AIJsonClub switchedClub = switchInfo.SwitchedClub;
                SortedList<int, SortedList<Vector2, Vector2>> switchAngelLineDict = GenerateAngleLineDict(startPos,
                    _terrain.GoalPosition, _clubFactory.GetMinDistanceByClubType(switchedClub.Type),
                    switchedClub.MaxDistance);
                // 正常打球
                AIGuideLineResult switchRegionList =
                    EnumerateAngelLineDict(switchAngelLineDict, startPos, _terrain.GoalPosition, switchedClub);
                AIMapRegion optimalSwitchClubRegion = OptimalDecisionForNormal(switchRegionList);
                bool isSwitchRegionInvalid = optimalSwitchClubRegion == null;

                if (!isSwitchRegionInvalid)
                {
                    return GenerateAction(optimalSwitchClubRegion, switchedClub, 0f, 1f, false, startPos);
                }

                switchInfo = SwitchClub(startPos, switchedClub);
            }

            return null;
        }


        /// <summary>
        /// 计算Power小于1f的打法时停球区域信息
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="useClub"></param>
        /// <returns></returns>
        private AIMapRegion CalcLightlyShotRegion(Vector2 startPos, AIJsonClub useClub)
        {
            // 不允许打上下旋
            AIJsonClub noSpinClub = useClub.Clone();
            noSpinClub.TopBackSpin = 0;

            float minDis = _clubFactory.GetMinDistanceByClubType(noSpinClub.Type);
            SortedList<int, SortedList<Vector2, Vector2>> angelLineDict =
                GenerateAngleLineDict(startPos, _terrain.GoalPosition, 0, minDis);
            AIGuideLineResult regionList =
                EnumerateAngelLineDict(angelLineDict, startPos, _terrain.GoalPosition, noSpinClub);
            AIMapRegion optimalRegion = OptimalDecisionForNormal(regionList);
            return optimalRegion;
        }


        /// <summary>
        /// 计算冒险打法时得到的停球区域信息
        /// </summary>
        /// <param name="startPos">发球位置</param>
        /// <param name="useClub">球杆</param>
        /// <returns></returns>
        private AIMapRegion CalcAdventrueShotRegion(Vector2 startPos, AIJsonClub useClub)
        {
            float powerRange = useClub.CalcPowerRange(false, _player.AdventureLevel);
            float adventurePowerRange = useClub.CalcPowerRange(true, _player.AdventureLevel);
            SortedList<int, SortedList<Vector2, Vector2>> adventureAngelLineDict = GenerateAngleLineDict(startPos,
                _terrain.GoalPosition, powerRange, adventurePowerRange);
            AIGuideLineResult adventureRegionList =
                EnumerateAngelLineDict(adventureAngelLineDict, startPos, _terrain.GoalPosition, useClub);
            return OptimalDecisionForNormal(adventureRegionList);
        }


        /// <summary>
        /// 生成扫描线
        /// </summary>
        /// <param name="startPos">发球点</param>
        /// <param name="goalPos">球洞位置</param>
        /// <param name="minRadius">最小击球范围</param>
        /// <param name="maxRadius">最大击球范围</param>
        /// <returns></returns>
        private SortedList<int, SortedList<Vector2, Vector2>> GenerateAngleLineDict(Vector2 startPos, Vector2 goalPos,
            float minRadius, float maxRadius)
        {
            SortedList<int, SortedList<Vector2, Vector2>> angelLineDict =
                new SortedList<int, SortedList<Vector2, Vector2>>(new AILineAngelComparer());
            float distanceToGoal = Vector2.Distance(startPos, goalPos);

            // 遍历外接正方形
            int minX = Mathf.Max(0, _terrain.MapCoordinateToArrayIndexX(startPos.x - maxRadius));
            int minY = Mathf.Max(0, _terrain.MapCoordinateToArrayIndexZ(startPos.y - maxRadius));
            int maxX = Mathf.Min(_terrain.MapCoordinateToArrayIndexX(startPos.x + maxRadius), _terrain.MapMaxXIndex);
            int maxY = Mathf.Min(_terrain.MapCoordinateToArrayIndexZ(startPos.y + maxRadius), _terrain.MapMaxZIndex);

            Vector2 dropPoint;
            Vector2 start2Goal = goalPos - startPos;
            Vector2 start2OffsetDrop;
            // Vector2 startOffset = startPos + wind.calcPredictOffset;

            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                    float pX = _terrain.ArrayIndexToMapCoordinateX(i);
                    float pY = _terrain.ArrayIndexToMapCoordinateZ(j);
                    dropPoint.x = pX;
                    dropPoint.y = pY;
                    Vector2 offsetDropPt = new Vector2(dropPoint.x, dropPoint.y);

                    // 落在半圆(环）范围内
                    if (!_terrain.IsPointOutOfMap(dropPoint) &&
                        !_terrain.IsPointOutOfMap(offsetDropPt) &&
                        AIMathf.IsPointInSemiCircleFaceToGoal(dropPoint, startPos, goalPos, minRadius, maxRadius))
                    {
                        offsetDropPt += _wind.CalcRandomPredictOffset(Vector2.Distance(startPos, dropPoint),
                            _player.WindPredictLevel);
                        start2OffsetDrop = offsetDropPt - startPos;
                        float angel = AIMathf.AngelWithSides(start2Goal, start2OffsetDrop);

                        //需要控制小数点位数，float本身不精确，应该不能用来当key
                        int angelKey = Mathf.FloorToInt(angel * ScanAngelPercision);

                        if (angelLineDict.ContainsKey(angelKey))
                        {
                            SortedList<Vector2, Vector2> item = angelLineDict[angelKey];
                            if (!item.ContainsKey(offsetDropPt))
                            {
                                item.Add(offsetDropPt, dropPoint);
                            }
                        }
                        else
                        {
                            SortedList<Vector2, Vector2> item =
                                new SortedList<Vector2, Vector2>(new AIDistanceToShotPointComparer(startPos));
                            item.Add(offsetDropPt, dropPoint);
                            angelLineDict.Add(angelKey, item);
                        }
                    }
                }
            }

            return angelLineDict;
        }


        /// <summary>
        /// 正常击球时，遍历扫描线，生成最终评估落点区域List
        /// </summary>
        /// <param name="angelLineList"></param>
        /// <param name="startPos"></param>
        /// <param name="goalPos"></param>
        /// <param name="club"></param>
        /// <returns></returns>
        private AIGuideLineResult EnumerateAngelLineDict(
            SortedList<int, SortedList<Vector2, Vector2>> angelLineList, Vector2 startPos, Vector2 goalPos,
            AIJsonClub club)
        {
            List<AIMapRegion> safeMapRegionList = new List<AIMapRegion>();
            List<AIMapRegion> unsafeMapRegionsList = new List<AIMapRegion>();
            Dictionary<Vector2, AIGuideLine> dictGuideLine = new Dictionary<Vector2, AIGuideLine>();

            float disStartToGoal = Vector2.Distance(startPos, goalPos);
            float minDisToGoal = Vector2.Distance(startPos, goalPos);
            float minAngelToScan = 180f;
            int intMinAngelToScan = Mathf.FloorToInt(minAngelToScan * ScanAngelPercision);

            foreach (KeyValuePair<int, SortedList<Vector2, Vector2>> pair in angelLineList)
            {
                // 遍历每个角度对应的数组
                SortedList<Vector2, Vector2> linePtList = pair.Value;
                int curAngel = pair.Key;
                if (Mathf.Abs(curAngel) < Mathf.Abs(intMinAngelToScan))
                {
                    float lastDisInCurLine = -1;
                    foreach (KeyValuePair<Vector2, Vector2> dropPtPair in linePtList)
                    {
                        Vector2 offsetDropPt = dropPtPair.Key;
                        Vector2 dropPoint = dropPtPair.Value;

                        int dropPtGroundType = _terrain.GetTerrainTypeAtPoint(offsetDropPt);
                        if (AITerrain.IsGroundTypeDangerZone(dropPtGroundType))
                        {
                            continue;
                        }

                        if (AIModeSwitch.IsInExcludeUnsafeDropAreaMode)
                        {
                            if (_terrain.IsPointInUnsafeDropPtArea(offsetDropPt))
                            {
                                continue;
                            }
                        }

                        AIGuideLine guideLine = null;
                        if (dictGuideLine.ContainsKey(offsetDropPt))
                        {
                            // 已经请求过的辅助线直接从字典获取
                            guideLine = dictGuideLine[offsetDropPt];
                        }
                        else
                        {
                            SPhysics.AiLine aiLine = RequestGuideLine(startPos, offsetDropPt, 0f, club.Line,
                                club.ClubEmitCoefficient);
                            // 未请求过的辅助线存起来，下次不再请求
                            guideLine = AIGuideLineHelper.CalcPredictGuideLine(startPos, aiLine, club.Line,
                                offsetDropPt, _terrain, club.TopBackSpin, club.SideSpin, club.ClubEmitCoefficient,
                                _player, club.Id == MainTestClubId);
                            dictGuideLine.Add(offsetDropPt, guideLine);
                        }

                        if (guideLine.FinalState == 2
                            || guideLine.FinalState == 5
                            || guideLine.IsInvalid
                            || guideLine.FinalRegion == null
                            || guideLine.FinalGuideLine == null
                            || guideLine.FinalGuideLine.Count == 0)
                        {
                            continue;
                        }

                        AIMapRegion region = guideLine.FinalRegion;
                        region = guideLine.FinalRegion;
                        region.WindOffsetDropPt = offsetDropPt;
                        region.DropPoint = dropPoint;

                        bool isTooFar = Vector2.Distance(startPos, _terrain.GoalPosition) <
                                        Vector2.Distance(startPos, region.Center);
                        bool isTooNearToStart = Vector2.Distance(startPos, region.WindOffsetDropPt) < 20f ||
                                                Vector2.Distance(startPos, region.DropPoint) < 20f;
//                        bool isWrongDirection = Mathf.Abs(curAngel) > 45f;
                        bool canAdventureToGreen = CanDangerDropBounceToGreen(region);
                        bool isSatisfied;
                        bool isSatisfiedNotConsiderFar;
                        if (!AIModeSwitch.IsInPar3Mode)
                        {
                            isSatisfiedNotConsiderFar =
                                !region.IsRegionInDanger() && !region.IsFinalRegionSideTooNearToDangerZone() &&
                                !region.IsDropPtFrontTooNearToDangerZone() &&
                                (!region.IsDropPtTooNearToDangerZone() || canAdventureToGreen) &&
                                !isTooNearToStart;// && !isWrongDirection;
                            isSatisfied = isSatisfiedNotConsiderFar && !isTooFar;
                        }
                        else
                        {
                            isSatisfiedNotConsiderFar = !region.IsRegionOut() && !isTooNearToStart;// && !isWrongDirection;
                            isSatisfied = isSatisfiedNotConsiderFar && !isTooFar;
//                            isSatisfiedNotConsiderFar = !region.IsRegionInDanger() && !isTooNearToStart && !isWrongDirection;
//                            isSatisfied = isSatisfiedNotConsiderFar && !isTooFar;
                        }

                        if (AIModeSwitch.IsInExcludeUnsafeAreaMode)
                        {
                            isSatisfied = isSatisfied && !_terrain.IsPointInExcludedStopArea(region.Center);
                        }

                        if (isSatisfied)
                        {
                            safeMapRegionList.Add(region);
                            // 所有的线的最小距离
                            minDisToGoal = Mathf.Min(minDisToGoal, region.DisToGoalNotSpin);
                            minAngelToScan = Mathf.Asin(minDisToGoal / disStartToGoal) * Mathf.Rad2Deg;
                            intMinAngelToScan = Mathf.FloorToInt(minAngelToScan * ScanAngelPercision);

                            // 当前扫描线上，这一次是否反而比上一次远了
                            bool isShorter = lastDisInCurLine > 0 && lastDisInCurLine < region.DisToGoalNotSpin;

                            if (isShorter)
                            {
                                break;
                            }

                            lastDisInCurLine = region.DisToGoalNotSpin;
                        }
                        else
                        {
                            if (isSatisfiedNotConsiderFar)
                            {
                                safeMapRegionList.Add(region);
                            }
                            else if (!region.IsRegionOut() && !isTooNearToStart)
                            {
                                unsafeMapRegionsList.Add(region);
                            }
                        }
                    } // 线
                } // 扇形
            } // 扫描线 

            AIGuideLineResult result = new AIGuideLineResult();
            result.SafeRegions = safeMapRegionList;
            result.UnsafeRegions = unsafeMapRegionsList;
            return result;
        }


        /// <summary>
        /// 判断危险地形能否最终摊上果岭
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        private bool CanDangerDropBounceToGreen(AIMapRegion region)
        {
            if (region == null)
            {
                return false;
            }

            int dropType = _terrain.GetTerrainTypeAtPoint(region.WindOffsetDropPt);
            int arriveType = _terrain.GetTerrainTypeAtPoint(region.Center);
            bool isTooNear = region.IsDropPtTooNearToDangerZone();
            bool isArriveGreen = arriveType == 4 || arriveType == 5 || arriveType == 6;
            bool isDropRough = AITerrain.IsGroundTypeRough(dropType);

            return isTooNear && isArriveGreen && isDropRough;
        }


        /// <summary>
        /// 正常击球时，从落点区域List中选出最优的落点区域
        /// </summary>
        /// <param name="regionResult"></param>
        /// <returns></returns>
        private AIMapRegion OptimalDecisionForNormal(AIGuideLineResult regionResult)
        {
            AIMapRegion result = null;

            if (regionResult.SafeRegions.Count > 0)
            {
                // 去掉不满足条件的点之后，根据离球洞的距离进行排序
                regionResult.SafeRegions.Sort(new AINormalOptimalRegionComparer());
                result = regionResult.SafeRegions[0];
                return result;
            }


            if (regionResult.UnsafeRegions.Count > 0)
            {
                regionResult.UnsafeRegions.Sort(new AINormalOptimalRegionComparer());
                result = regionResult.UnsafeRegions[0];
                return result;
            }

            return result;
        }


        /// <summary>
        /// 根据最终选择的辅助线的停球区域和球杆等信息来得到Action
        /// </summary>
        /// <param name="region"></param>
        /// <param name="club"></param>
        /// <param name="adventureAspect"></param>
        /// <param name="power"></param>
        /// <param name="isChipping"></param>
        /// <param name="shotPos"></param>
        /// <returns></returns>
        private AIAction GenerateAction(AIMapRegion region, AIJsonClub club, float adventureAspect, float power,
            bool isChipping, Vector2 shotPos)
        {
            AIAction action = new AIAction();

            // 一堆赋值
            action.PredictArrivePosition = region.Center;
            action.SideSpin = region.SideSpin;
            action.TopBackSpin = region.TopBackSpin;
            action.ClubAccuracy = club.Accuracy;
            action.PlayCurl = club.Line;
            action.TargetPosition = region.DropPoint;
            action.ClubId = club.Id;
            action.ClubType = club.Type;
            action.ClubLevel = club.Level;
            action.IsChipping = isChipping;
            action.ClubEmitCoefficient = club.ClubEmitCoefficient;
            action.WindOffsetDropPt = region.WindOffsetDropPt;
            action.MinDisDropPt = region.SideDisFromDropPtToDangerZone;
            action.MinDisFinal = region.SideDisFromCenterToDangerZone;
            action.MinDisFrontDropPt = region.FrontDisFromCenterToDangerZone;


            // 计算偏差
            int shotGroundType = _terrain.GetTerrainTypeAtPoint(shotPos);
            bool isDangerTerrain = AITerrain.IsGroundTypeRough(shotGroundType) || shotGroundType == 3;
            action.Deviation = AIDeviation.GetShotDerivation(_player.DirectionAccuracyLevel, club.Accuracy,
                adventureAspect, isDangerTerrain);
//            action.Deviation = UnityEngine.Random.Range(0.25f, 0.4f);
            action.Deviation = 0;

            // Debuff作用
            action.Power = power / _physicsManager.AiDebuff(shotPos.x, shotPos.y, DebuffFactor);

            // 通过Lua接口移落点并计算偏差产生的侧旋
            float targetH = GetHeightAtPoint(action.TargetPosition);
            float shotH = GetHeightAtPoint(shotPos);
            float windX = _wind.WindDirection.x;
            float windZ = _wind.WindDirection.y;
            float windLv = _wind.WindLevel;
            Vector3 windVec = new Vector3(windX * windLv, 0,
                -windZ * windLv);
//            Vector3 windVec = Vector3.zero;
            LuaInterface.LuaTable table =
                _luaTable
                    .Invoke<float, Vector3, Vector3, float, float, float, Vector3, float, float, float,
                        LuaInterface.LuaTable>(
                        "CalculateRealTargetPosAndSideSpin",
                        action.Deviation,
                        new Vector3(action.TargetPosition.x, targetH, action.TargetPosition.y),
                        new Vector3(shotPos.x, shotH, shotPos.y),
                        0,
                        action.TopBackSpin * 5f,
                        -action.SideSpin * 5f,
                        windVec,
                        club.ClubEmitCoefficient,
                        club.Accuracy, 1f);
            float sideSpin = table.RawGet<string, float>("sideSpin");
            Vector3 target = table.RawGet<string, Vector3>("position");

            action.TargetPositionAfterMoving = new Vector2(target.x, target.z);
            action.SideSpinAfterMoving = sideSpin;
//            action.TargetPositionAfterMoving = action.TargetPosition;
//            action.SideSpinAfterMoving = action.SideSpin;

            return action;
        }


        /// <summary>
        /// 获取当前地图Scene的名称
        /// </summary>
        /// <returns></returns>
        private string GetMapName()
        {
//            return SceneLoadManager.GetCurrentSceneName();
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }
        


        /// <summary>
        /// 给物理用的接口，获取坐标处地图高度信息
        /// </summary>
        /// <param name="pos">坐标</param>
        /// <returns></returns>
        public float GetHeightAtPoint(Vector2 pos)
        {
            return _terrain.GetTerrainHeightAtPoint(pos);
        }


        /// <summary>
        /// 给物理的接口，获取坐标处地图地形信息
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int GetGroundTypeAtPoint(Vector2 pos)
        {
            return _terrain.GetTerrainTypeAtPoint(pos);
        }


        /// <summary>
        /// 中间数据结构，处理后的待选辅助线数组
        /// </summary>
        private class AIGuideLineResult
        {
            public List<AIMapRegion> SafeRegions;

            public List<AIMapRegion> UnsafeRegions;

            public AIGuideLineResult()
            {
                SafeRegions = new List<AIMapRegion>();

                UnsafeRegions = new List<AIMapRegion>();
            }
        }
    }
}
#endif