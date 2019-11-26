#if AI_TEST

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ai
{
    [System.Serializable]
    public class AILogInfo
    {
        public AIPlayerLogInfo player;

        public AITerrainLogInfo map;

        public AIGameLogInfo[] game_round;

        public AIClubIdLogInfo[] club_id;

        public AIClubTypeLogInfo[] club_type;

        public AISummaryInfo summary;

        public AILogInfo(int gameRoundCount, int clubIdCount, int clubTypeCount)
        {
            player = new AIPlayerLogInfo();
            map = new AITerrainLogInfo();
            game_round = new AIGameLogInfo[gameRoundCount];
            club_id = new AIClubIdLogInfo[clubIdCount];
            club_type = new AIClubTypeLogInfo[clubTypeCount];
            summary = new AISummaryInfo();
        }

        [System.Serializable]
        public class AIPlayerLogInfo
        {
            public int adventure_level;

            // 风向预判能力指数
            public int windPredict_level;

            // 对方向掌控能力指数
            public int direction_accuracy_level;
        }

        [System.Serializable]
        public class AITerrainLogInfo
        {
            public string map_name;

            public float[] start_position;

            public float[] goal_position;

            public AITerrainLogInfo()
            {
                start_position = new float[2];

                goal_position = new float[2];
            }
        }

        [System.Serializable]
        public class AIClubIdLogInfo
        {
            public int club_id;

            public int use_count;
        }

        [System.Serializable]
        public class AIClubTypeLogInfo
        {
            public int club_type;

            public int use_count;
        }

        [System.Serializable]
        public class AIGameLogInfo
        {
            public int game_round;

            public float wind_level;

            public float[] wind_direction;

            public AIGameActionLogInfo[] actions;

            public int hit_count;
        }

        [System.Serializable]
        public class AIGameActionLogInfo
        {
            public int actionId;

            public int arrive_ground_type;

            public int club_type;

            public int club_id;

            public int club_level;

            public float club_emit_coefficient;

            public float top_back_spin;

            public float deviation;

            public float side_spin;

            public float moved_side_spin;

            public float predict_arrive_point_danger_distance;

            public float drop_point_danger_distance;

            public float drop_point_front_danger_distance;

            public float power;

            public bool is_chipping;

            public bool is_in_hole;

            public float distance_to_hole;

            public float[] shot_point;

            public float[] drop_point;

            public float[] wind_offset_drop_point;

            public float[] moved_drop_point;

            public float[] real_drop_point;

            public float[] last_bounce_point;

            public float[] predict_arrive_point;

            public float[] real_arrive_point;

            public AIGameActionLogInfo()
            {
                shot_point = new float[2];
                real_arrive_point = new float[2];
                predict_arrive_point = new float[2];
                drop_point = new float[2];
                moved_drop_point = new float[2];
                wind_offset_drop_point = new float[2];
                last_bounce_point = new float[2];
                real_drop_point = new float[2];
            }
        }


        [System.Serializable]
        public class AISummaryInfo
        {
            public int total_game_count;
            public int total_hit_count;
            public double time_for_load_map_ms;
            public double time_for_game_ms;
            public double avg_distance_to_hole;
            public int out_count;
            public double ground_type_1_count;
            public double ground_type_2_count;
            public double ground_type_3_count;
            public double ground_type_4_count;
            public double ground_type_5_count;
            public double ground_type_6_count;
            public double ground_type_7_count;
            public double ground_type_8_count;
            public double ground_type_9_count;
            public double ground_type_10_count;
            public double ground_type_11_count;
            
        }
    }


    /// <summary>
    /// 把击球过程的数据以日志形式写入到Json文件中
    /// </summary>
    public static class AILogger
    {
        /// <summary>
        /// 将日志对象转化为Json字符串
        /// </summary>
        /// <param name="info">日志对象</param>
        /// <returns></returns>
        private static string ConvertToJsonStr(AILogInfo info)
        {
            string json = JsonUtility.ToJson(info, true);
            return json;
        }

        /// <summary>
        /// 将日志对象写入到Json文件存储到磁盘
        /// </summary>
        /// <param name="info">日志对象</param>
        /// <param name="path">存储路径</param>
        /// <param name="fileName">存储文件名</param>
        public static void LogToDisk(AILogInfo info, string path, string fileName)
        {
            path = Application.persistentDataPath + "/" + path;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filePath = path + "/" + fileName + ".json";

            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                File.Delete(filePath);
            }

            string json = ConvertToJsonStr(info);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }


        /// <summary>
        /// 把AIAction中击球的决策信息
        /// </summary>
        /// <param name="logAction"></param>
        /// <param name="action"></param>
        public static void RecordActionLog(AILogInfo.AIGameActionLogInfo logAction, AIAction action)
        {
            logAction.drop_point[0] = action.TargetPosition.x;
            logAction.drop_point[1] = action.TargetPosition.y;
            logAction.wind_offset_drop_point[0] = action.WindOffsetDropPt.x;
            logAction.wind_offset_drop_point[1] = action.WindOffsetDropPt.y;
            logAction.predict_arrive_point[0] = action.PredictArrivePosition.x;
            logAction.predict_arrive_point[1] = action.PredictArrivePosition.y;
            logAction.moved_drop_point[0] = action.TargetPositionAfterMoving.x;
            logAction.moved_drop_point[1] = action.TargetPositionAfterMoving.y;
            logAction.is_chipping = action.IsChipping;
            logAction.club_type = action.ClubType;
            logAction.club_id = action.ClubId;
            logAction.club_level = action.ClubLevel;
            logAction.top_back_spin = action.TopBackSpin;
            logAction.deviation = action.Deviation;
            logAction.side_spin = action.SideSpin;
            logAction.moved_side_spin = action.SideSpinAfterMoving;
            logAction.club_emit_coefficient = action.ClubEmitCoefficient;
            logAction.drop_point_danger_distance = action.MinDisDropPt;
            logAction.drop_point_front_danger_distance = action.MinDisFrontDropPt;
            logAction.predict_arrive_point_danger_distance = action.MinDisFinal;
            logAction.power = action.Power;
        }

        /// <summary>
        /// 记录每次击球之后实际击球结果
        /// </summary>
        /// <param name="logAction"></param>
        /// <param name="arrivePointInfo"></param>
        /// <param name="shotPos"></param>
        /// <param name="goalPos"></param>
        /// <param name="arriveGroundType"></param>
        /// <param name="actionId"></param>
        public static void RecordArriveLog(AILogInfo.AIGameActionLogInfo logAction, SPhysics.ShotInfo arrivePointInfo,
            Vector2 shotPos, Vector2 goalPos, int arriveGroundType, int actionId)
        {
            // 记录发球点
            logAction.shot_point[0] = shotPos.x;
            logAction.shot_point[1] = shotPos.y;
            
            // 实际第一个落点
            logAction.real_drop_point[0] = arrivePointInfo.firstBouncePosition.x;
            logAction.real_drop_point[1] = arrivePointInfo.firstBouncePosition.y;
            // 是否进洞
            logAction.is_in_hole = arrivePointInfo.inhole;
            // 记录到达点
            logAction.real_arrive_point[0] = arrivePointInfo.finalPosition.x;
            logAction.real_arrive_point[1] = arrivePointInfo.finalPosition.y;
            // 最后一个落点
            logAction.last_bounce_point[0] = arrivePointInfo.finalBouncePosition.x;
            logAction.last_bounce_point[1] = arrivePointInfo.finalBouncePosition.y;
            // 记录参数值
            logAction.arrive_ground_type = arriveGroundType;
            logAction.actionId = actionId;
            logAction.distance_to_hole =
                Vector2.Distance(arrivePointInfo.finalPosition, goalPos);
        }

        public static void RecordGameInfoLog(AILogInfo.AIGameLogInfo gameInfo, AIWind wind, int hitCount, int round,
            List<AILogInfo.AIGameActionLogInfo> logActionList)
        {
            // 日志记录风的情况

            List<float> windDirection = new List<float>();
            windDirection.Add(wind.WindDirection.x);
            windDirection.Add(wind.WindDirection.y);
            gameInfo.wind_direction = windDirection.ToArray();
            gameInfo.wind_level = wind.WindLevel;

            gameInfo.game_round = round;
            gameInfo.hit_count = hitCount;
            gameInfo.actions = logActionList.ToArray();
        }

        public static void RecordClubIdLogInfo(AILogInfo logInfo, Dictionary<int, int> clubIdDict)
        {
            int index = 0;
            foreach (int key in clubIdDict.Keys)
            {
                AILogInfo.AIClubIdLogInfo clubIdInfo = new AILogInfo.AIClubIdLogInfo();
                clubIdInfo.club_id = key;
                clubIdInfo.use_count = clubIdDict[key];
                logInfo.club_id[index] = clubIdInfo;
                index++;
            }
        }

        public static void RecordClubTypeLogInfo(AILogInfo logInfo, Dictionary<int, int> clubTypeDict)
        {
            int index = 0;
            foreach (int key in clubTypeDict.Keys)
            {
                AILogInfo.AIClubTypeLogInfo clubIdInfo = new AILogInfo.AIClubTypeLogInfo();
                clubIdInfo.club_type = key;
                clubIdInfo.use_count = clubTypeDict[key];
                logInfo.club_type[index] = clubIdInfo;
                index++;
            }
        }

        public static void RecordLogInfo(AILogInfo logInfo, int shotCount, int totalHitCount, double timeForLoadMap,
            double timeForGame, AIPlayer player, string mapName, AITerrain terrain,
            List<AILogInfo.AIGameLogInfo> gameInfoList)
        {
            // 概况
            logInfo.summary.total_game_count = shotCount;
            logInfo.summary.total_hit_count = totalHitCount;
            logInfo.summary.time_for_load_map_ms = timeForLoadMap;
            logInfo.summary.time_for_game_ms = timeForGame;
            // 玩家信息
            logInfo.player.adventure_level = player.AdventureLevel;
            logInfo.player.direction_accuracy_level = player.DirectionAccuracyLevel;
            logInfo.player.windPredict_level = player.WindPredictLevel;
            // 地图信息
            logInfo.map.map_name = mapName;
            logInfo.map.start_position[0] = terrain.StartPosition.x;
            logInfo.map.start_position[1] = terrain.StartPosition.y;
            logInfo.map.goal_position[0] = terrain.GoalPosition.x;
            logInfo.map.goal_position[1] = terrain.GoalPosition.y;
            logInfo.game_round = gameInfoList.ToArray();

            double totalDis = 0;
            int outCount = 0;
            int[] gtArray = new int[11];
            for (int i = 0; i < gameInfoList.Count; i++)
            {
                var dis = gameInfoList[i].actions[0].distance_to_hole;
                var gt = gameInfoList[i].actions[0].arrive_ground_type;
                if (gt > 0 && gt <= 11)
                {
                    gtArray[gt]++;
                }
                
                if (!AITerrain.IsGroundTypeOut(gt))
                {
                    totalDis += dis;
                }
                else
                {
                    outCount += 1;
                }
            }

            logInfo.summary.avg_distance_to_hole = totalDis / (shotCount - (double)outCount);
            logInfo.summary.out_count = outCount;

            logInfo.summary.ground_type_1_count = ((float) gtArray[0]) / ((float) shotCount);
            logInfo.summary.ground_type_2_count = ((float) gtArray[1]) / ((float) shotCount);
            logInfo.summary.ground_type_3_count = ((float) gtArray[2]) / ((float) shotCount);
            logInfo.summary.ground_type_4_count = ((float) gtArray[3]) / ((float) shotCount);
            logInfo.summary.ground_type_5_count = ((float) gtArray[4]) / ((float) shotCount);
            logInfo.summary.ground_type_6_count = ((float) gtArray[5]) / ((float) shotCount);
            logInfo.summary.ground_type_7_count = ((float) gtArray[6]) / ((float) shotCount);
            logInfo.summary.ground_type_8_count = ((float) gtArray[7]) / ((float) shotCount);
            logInfo.summary.ground_type_9_count = ((float) gtArray[8]) / ((float) shotCount);
            logInfo.summary.ground_type_10_count = ((float) gtArray[9]) / ((float) shotCount);
            logInfo.summary.ground_type_11_count = ((float) gtArray[10]) / ((float) shotCount);            
        }

        /// <summary>
        /// 添加球杆使用情况统计信息到字典中
        /// </summary>
        /// <param name="dictClub"></param>
        /// <param name="key"></param>
        public static void AddClubInfoToDictionary(Dictionary<int, int> dictClub, int key)
        {
            if (dictClub.ContainsKey(key))
            {
                dictClub[key] += 1;
            }
            else
            {
                dictClub.Add(key, 1);
            }
        }
    }
}

#endif