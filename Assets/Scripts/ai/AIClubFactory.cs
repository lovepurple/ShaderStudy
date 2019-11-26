#if AI_TEST

using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ai
{
    /// <summary>
    /// 球杆工厂，负责产生、筛选球杆
    /// </summary>
    public class AIClubJsonFactory
    {
        [System.Serializable]
        public class ClubListData
        {
            public ClubData[] clubdata;

            [System.Serializable]
            public class ClubData
            {
                public ClubAbilityData[] ability;
                public int color;
                public string icon;
                public int id;
                public int name;
                public int tag;
                public int tour;
                public int type;
                public int type_name;
                public int tyep_desc;
                public int color_desc;
                public int club_desc;
                public int min_power_of_type;
                public int max_power_of_type;
                public int real_angle;
                public float angle;

                [System.Serializable]
                public class ClubAbilityData
                {
                    public int accuracy;
                    public int force;
                    public int level;
                    public float line;
                    public int sidespin;
                    public int topbackspin;
                    public int total;
                    public int update_card;
                    public int update_price;
                }
            }
        }


        /// <summary>
        /// 从Json读取数据后缓存在内存的球杆对象池
        /// </summary>
        private ClubListData _clubListData;


        /// <summary>
        /// 选中的开球杆的ID，默认值为新手球杆
        /// </summary>
        public int SelectedDriveClubId = 1;


        /// <summary>
        /// 选中的开球杆的等级，默认等级为1级
        /// </summary>
        public int SelectedDriveClubLv = 1;


        /// <summary>
        /// 选中的3号木杆的ID，默认值为新手球杆
        /// </summary>
        public int SelectedWoodClubId = 2;


        /// <summary>
        /// 选中的3号木杆的等级，默认等级为1级
        /// </summary>
        public int SelectedWoodClubLv = 1;


        /// <summary>
        /// 选中的长铁杆的ID，默认值为新手球杆
        /// </summary>
        public int SelectedBigIronClubId = 3;


        /// <summary>
        /// 选中的长铁杆的等级，默认等级为1级
        /// </summary>
        public int SelectedBigIronClubLv = 1;


        /// <summary>
        /// 选中的短铁杆的ID，默认值为新手球杆
        /// </summary>
        public int SelectedSmallIronClubId = 4;


        /// <summary>
        /// 选中的短铁杆的等级，默认等级为1级
        /// </summary>
        public int SelectedSmallIronClubLv = 1;


        /// <summary>
        /// 选中的挖起杆的ID，默认值为新手球杆
        /// </summary>
        public int SelectedPwClubId = 5;


        /// <summary>
        /// 选中的挖起杆的等级，默认等级为1级
        /// </summary>
        public int SelectedPwClubLv = 1;

        /// <summary>
        /// 选中的沙坑杆的ID，默认值为新手球杆
        /// </summary>
        public int SelectedSwClubId = 6;

        /// <summary>
        /// 选中的沙坑杆的等级，默认等级为1级
        /// </summary>
        public int SelectedSwClubLv = 1;

        /// <summary>
        /// 从球杆json配置文件加载球杆数据，缓存在内存中
        /// </summary>
        private void LoadJsonData()
        {
            string filePath = Application.dataPath + "/ClubConfig_5.json";
            string jsonStr = File.ReadAllText(filePath, Encoding.UTF8);
            _clubListData = JsonUtility.FromJson<ClubListData>(jsonStr);
        }

        /// <summary>
        /// 获取1号木杆（开球杆）
        /// </summary>
        /// <returns></returns>
        public AIJsonClub Create1W()
        {
            AIJsonClub club = GetClubByIdAndLevel(SelectedDriveClubId, SelectedDriveClubLv);
//            club.SideSpin = 0;
//            club.TopBackSpin = 0;
//            club.Line = 3;
            return club;
        }

        /// <summary>
        /// 获取3号木杆
        /// </summary>
        /// <returns></returns>
        public AIJsonClub Create3W()
        {
            AIJsonClub club = GetClubByIdAndLevel(SelectedWoodClubId, SelectedWoodClubLv);
            return club;
        }

        /// <summary>
        /// 获取长铁杆
        /// </summary>
        /// <returns></returns>
        public AIJsonClub CreateBigIron()
        {
            AIJsonClub club = GetClubByIdAndLevel(SelectedBigIronClubId, SelectedBigIronClubLv);
            return club;
        }

        /// <summary>
        /// 获取短铁杆
        /// </summary>
        /// <returns></returns>
        public AIJsonClub CreateSmallIron()
        {
            AIJsonClub club = GetClubByIdAndLevel(SelectedSmallIronClubId, SelectedSmallIronClubLv);
            return club;
        }

        /// <summary>
        /// 获取挖起杆
        /// </summary>
        /// <returns></returns>
        public AIJsonClub CreatePW()
        {
            AIJsonClub club = GetClubByIdAndLevel(SelectedPwClubId, SelectedPwClubLv);
            return club;
        }

        /// <summary>
        /// 获取沙坑杆
        /// </summary>
        /// <returns></returns>
        public AIJsonClub CreateSW()
        {
            AIJsonClub club = GetClubByIdAndLevel(SelectedSwClubId, SelectedSwClubLv);
            return club;
        }

        public AIClubJsonFactory()
        {
            LoadJsonData();
        }


        /// <summary>
        /// 获取球杆击打范围的下限
        /// </summary>
        /// <param name="clubType">球杆类型</param>
        /// <returns></returns>
        public float GetMinDistanceByClubType(int clubType)
        {
            float minDis = 0;

            switch (clubType)
            {
                case 1:
                    minDis = 180f;
                    break;
                case 2:
                    minDis = CreateBigIron().MaxDistance;
                    break;
                case 3:
                    minDis = CreateSmallIron().MaxDistance;
                    break;
                case 4:
                    minDis = CreatePW().MaxDistance;
                    break;
            }

            return minDis;
        }

        /// <summary>
        /// 根据id和等级获取球杆实例
        /// </summary>
        /// <param name="id">球杆ID</param>
        /// <param name="level">球杆等级</param>
        /// <returns></returns>
        public AIJsonClub GetClubByIdAndLevel(int id, int level)
        {
            for (int i = 0; i < _clubListData.clubdata.Length; i++)
            {
                ClubListData.ClubData clubData = _clubListData.clubdata[i];
                int clubID = clubData.id;
                if (id == clubID)
                {
                    for (int j = 0; j < clubData.ability.Length; j++)
                    {
                        ClubListData.ClubData.ClubAbilityData abilityData = clubData.ability[j];
                        if (abilityData.level == level)
                        {
                            AIJsonClub club = new AIJsonClub();
                            club.Type = clubData.type;
                            club.Force = abilityData.force;
                            club.Accuracy = abilityData.accuracy;
                            club.SideSpin = abilityData.sidespin / 10f;
                            club.TopBackSpin = abilityData.topbackspin / 10f;
                            club.Line = abilityData.line;
                            club.Level = abilityData.level;
                            club.Id = id;
                            club.Angle = clubData.angle;
                            return club;
                        }
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// 选杆逻辑
        /// </summary>
        /// <param name="groundType">发球点地形材质</param>
        /// <param name="distanceToGoal">离球洞的距离</param>
        /// <returns></returns>
        public AIJsonClub ChooseClub(int groundType, float distanceToGoal)
        {
            AIJsonClub club = null;
            switch (groundType)
            {
                case 1:
                case 5:
                {
                    if (distanceToGoal > GetMinDistanceByClubType(2))
                    {
                        club = Create3W();
                    }
                    else if (distanceToGoal > GetMinDistanceByClubType(3))
                    {
                        club = CreateBigIron();
                    }
                    else if (distanceToGoal > GetMinDistanceByClubType(4))
                    {
                        club = CreateSmallIron();
                    }
                    else
                    {
                        club = CreatePW();
                    }

                    break;
                }
                case 2:
                {
                    if (distanceToGoal > GetMinDistanceByClubType(3))
                    {
                        club = CreateBigIron();
                    }
                    else if (distanceToGoal > GetMinDistanceByClubType(4))
                    {
                        club = CreateSmallIron();
                    }
                    else
                    {
                        club = CreatePW();
                    }

                    break;
                }
                case 3:
                {
                    club = CreateSW();
                    break;
                }
                case 4:
                {
                    club = CreatePW();
                    break;
                }
                case 8:
                {
                    club = Create1W();
                    break;
                }
                default:
                {
                    club = CreatePW();
                    break;
                }
            }

            return club;
        }


        /// <summary>
        /// 切杆逻辑
        /// </summary>
        /// <param name="groundType">发球点地形材质</param>
        /// <param name="club">待切换的球杆</param>
        /// <returns></returns>
        public SwitchClubInfo SwichClub(int groundType, AIJsonClub club)
        {
            SwitchClubInfo result;
            result.IsSuccessed = false;
            result.SwitchedClub = club;
            if (groundType == 3 || groundType == 4 || groundType == 8 || club.Type == 5 ||
                AITerrain.IsGroundTypeOut(club.Type))
            {
                return result;
            }

            result.IsSuccessed = true;
            if (groundType == 2)
            {
                switch (club.Type)
                {
                    case 3:
                        result.SwitchedClub = CreateSmallIron();
                        break;
                    case 4:
                        result.SwitchedClub = CreatePW();
                        break;
                    default:
                        result.SwitchedClub = club;
                        result.IsSuccessed = false;
                        break;
                }
            }
            else if (groundType == 1)
            {
                switch (club.Type)
                {
                    case 2:
                        result.SwitchedClub = CreateBigIron();
                        break;
                    case 3:
                        result.SwitchedClub = CreateSmallIron();
                        break;
                    case 4:
                        result.SwitchedClub = CreatePW();
                        break;
                    default:
                        result.SwitchedClub = club;
                        result.IsSuccessed = false;
                        break;
                }
            }
            else
            {
                result.SwitchedClub = club;
                result.IsSuccessed = false;
            }

            return result;
        }
    }


    public struct SwitchClubInfo
    {
        public AIJsonClub SwitchedClub;

        public bool IsSuccessed;
    }
}

#endif