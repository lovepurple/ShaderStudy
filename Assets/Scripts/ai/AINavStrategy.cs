#if AI_TEST
using UnityEngine;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.ai
{
    public class AINavStrategy : MonoBehaviour, IStrategy
    {
        /// <summary>
        /// 地图模型
        /// </summary>
        private AITerrain _terrain;

        /// <summary>
        /// 物理
        /// </summary>
        private PhysicsDataManager _physicsManager;

        /// <summary>
        /// 物理（AI专用）
        /// </summary>
        private AIPhysicsDataManager _aiPhysicsManager;

        /// <summary>
        /// 玩家能力指数模型
        /// </summary>
        private AIPlayer _player;

        /// <summary>
        /// 场景名称
        /// </summary>
        private string _sceneName;


        /// <summary>
        /// Debuff系数
        /// </summary>
        private float _debuffFactor = 1f;

        /// <summary>
        /// 球弹性系数
        /// </summary>
        private float _ballBounceness = 1f;


        private float _scanPercision = 1;

        private TestSpinType _testType;


        private int _testClubId = 1;

        private int _testClubLv = 1;

		private float _power = 1;

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
        /// 风模型
        /// </summary>
        private AIWind _wind;

        private static AINavStrategy Instance;

        public System.Collections.Generic.List<PhysicMaterial> Mats;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            PhysicsManager.Instance.EnterGame();
            AIPhysicsDataManager aiPhysics = new AIPhysicsDataManager(this, Mats);
            PhysicsDataManager physics = new PhysicsDataManager();
            physics.Init();
            Init(physics, aiPhysics, gameObject.name);
            PhysicsManager.Instance.physicsDataManager = aiPhysics;
            PhysicsManager.Instance.ai = this;
            UnityEngine.Debug.Log("AINavStrategy Start");
        }


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


            //////////////////////////////////////////////////////////////////
            /// 测试参数调整
            ////////////////////////////////////////////////////////////////// 

            // 球的弹性
            _ballBounceness = 1f;

            // Debuff系数
            _debuffFactor = 1f;

            // 扫描精度
            _scanPercision = 0.2f;

            // 旋度类型
			_testType = TestSpinType.TestSpinTypeNormal;

            // 球杆Id
            _testClubId = 13;

            // 球杆等级
            _testClubLv = 10;

			_power = 1.0f;

//            AreaToAreaShotSimulation(26, 10, _testType, _ballBounceness, _scanPercision);
//            AreaToAreaShotSimulation(26, 9, _testType, _ballBounceness, _scanPercision);
            
			PointToAreaShotSimulation(_testClubId, _testClubLv, _testType, _ballBounceness, _scanPercision);
        }


        private void PointToAreaShotSimulation(int clubId, int clubLv, TestSpinType type, float ballBounceness,
            float percision)
        {
            AIJsonClub club = _clubFactory.GetClubByIdAndLevel(clubId, clubLv);
            float sideSpin = 0f;
            float topSpin = 0f;
            float suffix = 0f;

            switch (type)
            {
                case TestSpinType.TestSpinTypeLeftSpin:
                    sideSpin = club.SideSpin;
                    suffix = sideSpin;
                    break;
                case TestSpinType.TestSpinTypeRightSpin:
                    sideSpin = -club.SideSpin;
                    suffix = sideSpin;
                    break;
                case TestSpinType.TestSpinTypeTopSpin:
                    topSpin = club.TopBackSpin;
                    suffix = topSpin;
                    break;
                case TestSpinType.TestSpinTypeBackSpin:
                    topSpin = -club.TopBackSpin;
                    suffix = topSpin;
                    break;
            }

            string dirName = "club_" + clubId + "_" + clubLv + "_" + suffix;
            string logDirName = GetMapName() + "/" + dirName;
            string logFileName = "AILog_" + GetMapName();
            _terrain = new AITerrain();
            _terrain.LoadMapData(_physicsManager, _sceneName, GetMapName());

            _ballBounceness = ballBounceness;
            PhysicsManager.Instance.ballController.ballBounciness = _ballBounceness;

            GameObject startObj = GameObject.Find("StartArea");
            Vector3 startPos3D = startObj.transform.position;
            Vector2 ptStart = new Vector2(startPos3D.x, startPos3D.z);
            List<Vector2> endArea = _terrain.GeneratePointArrayByAreaName("EndArea", percision);
            _player = new AIPlayer(80, 80, 80, 100, 100, 100);
            List<AILogInfo.AIGameLogInfo> gameInfoList = new List<AILogInfo.AIGameLogInfo>();
            int totalHitCount = 0;
            int index = 0;

            _wind = new AIWind(0, Vector2.zero);
            // 每个文件打shotCount场比赛
            foreach (var ptEnd in endArea)
            {
                if (Vector2.Distance(ptStart, ptEnd) > club.Force ||
                    AITerrain.IsGroundTypeOut(_terrain.GetTerrainTypeAtPoint(ptStart)))
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

            AILogInfo logInfo = new AILogInfo(gameInfoList.Count, 0, 0);
            // 日志记录总结信息
            AILogger.RecordLogInfo(logInfo, totalHitCount, totalHitCount, 0, 0, _player,
                GetMapName(), _terrain, gameInfoList);

            // 写入到磁盘中
            AILogger.LogToDisk(logInfo, logDirName, logFileName);
        }

        private void AreaToAreaShotSimulation(int clubId, int clubLv, TestSpinType type, float ballBounceness,
            int percision)
        {
            AIJsonClub club = _clubFactory.GetClubByIdAndLevel(clubId, clubLv);
            float sideSpin = 0f;
            float topSpin = 0f;
            float suffix = 0f;

            switch (type)
            {
                case TestSpinType.TestSpinTypeLeftSpin:
                    sideSpin = club.SideSpin;
                    suffix = sideSpin;
                    break;
                case TestSpinType.TestSpinTypeRightSpin:
                    sideSpin = -club.SideSpin;
                    suffix = sideSpin;
                    break;
                case TestSpinType.TestSpinTypeTopSpin:
                    topSpin = club.TopBackSpin;
                    suffix = topSpin;
                    break;
                case TestSpinType.TestSpinTypeBackSpin:
                    topSpin = -club.TopBackSpin;
                    suffix = topSpin;
                    break;
            }

            string dirName = "club_" + clubId + "_" + clubLv + "_" + suffix;
            string logDirName = GetMapName() + "/" + dirName;
            string logFileName = "AILog_" + GetMapName();
            _terrain = new AITerrain();
            _terrain.LoadMapData(_physicsManager, _sceneName, GetMapName());

            _ballBounceness = ballBounceness;
            PhysicsManager.Instance.ballController.ballBounciness = _ballBounceness;

            List<Vector2> startArea = _terrain.GeneratePointArrayByAreaName("StartArea", percision);
            List<Vector2> endArea = _terrain.GeneratePointArrayByAreaName("EndArea", percision);
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
                    if (Vector2.Distance(ptStart, ptEnd) > club.Force ||
                        AITerrain.IsGroundTypeOut(_terrain.GetTerrainTypeAtPoint(ptStart)))
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
            //action.Power = power / _physicsManager.AiDebuff(shotPos.x, shotPos.y, _debuffFactor);
			action.Power = _power;
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
        /// 初始化Lua环境
        /// </summary>
        private void InitLua()
        {
            _luaState = new LuaInterface.LuaState();
            _luaState.Start();
            _luaTable = _luaState.DoFile<LuaInterface.LuaTable>("Assets/Scripts/Lua/Tools/PhysicsHelper");
        }


        /// <summary>
        /// 根据Action击球
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private SPhysics.ShotInfo ShotBallByAction(AIAction action)
        {
            return _physicsManager.Shot(action.TargetPosition, action.SideSpinAfterMoving, action.TopBackSpin,
                action.ClubEmitCoefficient,
                action.TargetPositionAfterMoving, action.Power, _ballBounceness);
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
    }
}

#endif