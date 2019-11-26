#if AI_TEST
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Scripts.ai
{
    public class AITerrain
    {
        #region 变量

        private AITerrainGroundTypeInfo _groundTypeInfo;

        private AITerrainHeightInfo _heightInfo;

        private AITerrainRectInfo _rectInfo;

        private AITerrainRectRegionInfo _regionInfo;

        private AIColliderInfo _colliderInfo;

        private IPhysicsDataManager _physic;

        private HashSet<Vector2> _excludedArea = new HashSet<Vector2>();

        private HashSet<Vector2> _mountainArea = new HashSet<Vector2>();

        private HashSet<Vector2> _unsafeDropPointSet = new HashSet<Vector2>();

        /// <summary>
        /// 代表X轴方向地图的长度
        /// </summary>
        public float MapXLength;

        /// <summary>
        /// 代表Y轴方向地图的长度
        /// </summary>
        public float MapYLength;

        /// <summary>
        /// 代表Z轴方向地图的长度
        /// </summary>
        public float MapZLength;

        public Vector3 MapOrigin;

        /// <summary>
        /// 代表X轴方向最大的引索值
        /// </summary>
        public int MapMaxXIndex
        {
            get { return _groundTypeInfo.MapMaxXIndex; }
        }


        /// <summary>
        /// 代表Y轴方向最大的引索值
        /// </summary>
        public int MapMaxYIndex
        {
            get { return _groundTypeInfo.MapMaxYIndex; }
        }


        /// <summary>
        /// 代表Y轴方向最大的引索值
        /// </summary>
        public int MapMaxZIndex
        {
            get { return _groundTypeInfo.MapMaxZIndex; }
        }

        public Vector2 GoalPosition;

        public Vector2 StartPosition;

        public const float Percision = 1f;

        private const float ColliderScanPercision = 1f;


        private const float ColliderExpandRadiusFactor = 1f;

        private const float ColliderYPerception = 1.1f;

        public float GoalPosY = 0;

        #endregion


        #region 加载地图信息方法

        /// <summary>
        /// 加载地图信息，包括地面材质，地表高度，碰撞体，特殊区域等
        /// </summary>
        /// <param name="physics"></param>
        /// <param name="sceneName">场景名称</param>
        /// <param name="mapName">要保存的地图名称</param>
        public void LoadMapData(IPhysicsDataManager physics, string sceneName, string mapName)
        {
            _physic = physics;

            string dirName = Application.persistentDataPath + "/" + mapName;
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            // 顺序不能互换
            LoadStartAndGoalPosData(physics);
            LoadMapGroundTypeData(physics, mapName);
            LoadMapHeightData(physics, mapName);
//            LoadCollidersData(physics, sceneName, mapName);

            if (AIModeSwitch.IsInExcludeUnsafeAreaMode)
            {
                LoadExcludedStopAreaData();
            }

            if (AIModeSwitch.IsInExcludeCrossMountainAreaMode)
            {
                LoadMountainAreaData();
            }

            if (AIModeSwitch.IsInExcludeUnsafeDropAreaMode)
            {
                LoadUnsafeDropPtAreaData();
            }

            // loadRectangleVertexData(mapName);            
            // loadRegionGroundTypeData(mapName);
        }

        /// <summary>
        /// 加载开球点和球洞位置信息
        /// </summary>
        /// <param name="physics"></param>
        private void LoadStartAndGoalPosData(IPhysicsDataManager physics)
        {
            // 球洞的位置
            Vector3 goalPos3D = physics.GetTargetPos();
            Vector2 goalPos = new Vector2(goalPos3D.x, goalPos3D.z);
            GoalPosition = goalPos;
            GoalPosY = goalPos3D.y;

            // 开球区的位置
            Vector3 startPos3D = physics.GetStartPos();
            StartPosition = new Vector2(startPos3D.x, startPos3D.z);
        }

        /// <summary>
        /// 加载地表材质信息
        /// </summary>
        /// <param name="physics"></param>
        /// <param name="mapName"></param>
        private void LoadMapGroundTypeData(IPhysicsDataManager physics, string mapName)
        {
            // 加载地形信息
            string filePath = Application.persistentDataPath + "//" + mapName + "//" + mapName + "_gt.dat";
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
            {
                using (Stream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter binFormatter = new BinaryFormatter(); //创建二进制序列化器
                    _groundTypeInfo = binFormatter.Deserialize(fStream) as AITerrainGroundTypeInfo;
                }

                // 非空判断
                Assert.IsTrue(_groundTypeInfo != null);

                MapXLength = MapMaxXIndex * Percision;
                MapYLength = MapMaxYIndex * Percision;
                MapZLength = MapMaxZIndex * Percision;
                MapOrigin = new Vector3(_groundTypeInfo.MapOrigin.X, _groundTypeInfo.MapOrigin.Y,
                    _groundTypeInfo.MapOrigin.Z);
            }
            else
            {
                // 从客户端读取map
                MapZLength = physics.getWidth();
                MapXLength = physics.getLength();
                MapYLength = Mathf.Abs(physics.getHighest() - physics.getLowest());

                Vector2 origin = physics.getMinimum();
                MapOrigin.x = AIMathf.FloorToPercision(origin.x, Percision);
                MapOrigin.y = AIMathf.FloorToPercision(physics.getLowest(), Percision);
                MapOrigin.z = AIMathf.FloorToPercision(origin.y, Percision);

                _groundTypeInfo = new AITerrainGroundTypeInfo();
                _groundTypeInfo.MapOrigin = new AISerializedVector3(MapOrigin.x, MapOrigin.y, MapOrigin.z);
                _groundTypeInfo.MapMaxXIndex = Mathf.FloorToInt(MapXLength / Percision);
                _groundTypeInfo.MapMaxZIndex = Mathf.FloorToInt(MapZLength / Percision);
                _groundTypeInfo.MapMaxYIndex = Mathf.FloorToInt(MapYLength / Percision);

                int[] terrainGTData = new int[MapMaxXIndex * MapMaxZIndex];
                for (int i = 0; i < MapMaxXIndex; i++)
                {
                    for (int j = 0; j < MapMaxZIndex; j++)
                    {
                        float mapCoorX = ArrayIndexToMapCoordinateX(i);
                        float mapCoorY = ArrayIndexToMapCoordinateZ(j);
                        int gt = physics.getGroundType(mapCoorX, mapCoorY);

//                        // 界外的多种情况都规划为0
//                        if (gt == -1 || gt == -2 || gt == 7 || gt == 10 || gt == 11)
//                        {
//                            gt = 0;
//                        }
//
//                        // road等同于rough
//                        if (gt == 9)
//                        {
//                            gt = 2;
//                        }

                        terrainGTData[ConvertMapIndex(i, j)] = gt;
                    }
                }

                // 序列化
                _groundTypeInfo.TerrainGTData = terrainGTData;
                using (Stream fStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    BinaryFormatter binFormatter = new BinaryFormatter(); //创建二进制序列化器
                    binFormatter.Serialize(fStream, _groundTypeInfo);
                }
            }
        }

        /// <summary>
        /// 加载地图高度信息
        /// </summary>
        /// <param name="physics"></param>
        /// <param name="mapName"></param>
        private void LoadMapHeightData(IPhysicsDataManager physics, string mapName)
        {
            // 加载地形高度信息
            string filePath = Application.persistentDataPath + "//" + mapName + "//" + mapName + "_h.dat";
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
            {
                using (Stream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter binFormatter = new BinaryFormatter(); //创建二进制序列化器    
                    _heightInfo = binFormatter.Deserialize(fStream) as AITerrainHeightInfo;
                }
            }
            else
            {
                // 从客户端读取高度信息
                float[] terrainHData = new float[MapMaxXIndex * MapMaxZIndex];
                for (int i = 0; i < MapMaxXIndex; i++)
                {
                    for (int j = 0; j < MapMaxZIndex; j++)
                    {
                        float mapCoorX = ArrayIndexToMapCoordinateX(i);
                        float mapCoorY = ArrayIndexToMapCoordinateZ(j);
                        terrainHData[ConvertMapIndex(i, j)] = physics.GetMapHeight(mapCoorX, mapCoorY);
                    }
                }

                _heightInfo = new AITerrainHeightInfo();
                _heightInfo.TerrainHData = terrainHData;
                using (Stream fStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    BinaryFormatter binFormatter = new BinaryFormatter(); //创建二进制序列化器
                    binFormatter.Serialize(fStream, _heightInfo);
                }
            }
        }

        /// <summary>
        /// 加载碰撞体的信息
        /// </summary>
        /// <param name="physics"></param>
        /// <param name="sceneName"></param>
        /// <param name="mapName"></param>
        private void LoadCollidersData(IPhysicsDataManager physics, string sceneName, string mapName)
        {
            // 加载地形高度信息
            string filePath = Application.persistentDataPath + "//" + mapName + "//" + mapName + "_collider.dat";
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
            {
                using (Stream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter binFormatter = new BinaryFormatter(); //创建二进制序列化器
                    _colliderInfo = binFormatter.Deserialize(fStream) as AIColliderInfo;
                }
            }
            else
            {
                // 获取碰撞体信息
                SPhysics.SceneCollider colliders = physics.getCollider(sceneName);

                Dictionary<float, Dictionary<AISerializedVector2, int>> xzColliderPts =
                    new Dictionary<float, Dictionary<AISerializedVector2, int>>();

                // 处理碰撞体
                // 处理球
                List<Dictionary<Vector2, AIRange>> sphereColliderRangeList =
                    ProcessColliderSphere(colliders.sp, xzColliderPts);
                // 处理胶囊
                List<Dictionary<Vector2, AIRange>> capsuleColliderRangeList =
                    ProcessColliderCapsule(colliders.cap, xzColliderPts);

                _colliderInfo = new AIColliderInfo();
                _colliderInfo.YColliderRanges = new Dictionary<AISerializedVector2, AIYColliderInfo>();

                foreach (Dictionary<Vector2, AIRange> sphere in sphereColliderRangeList)
                {
                    foreach (KeyValuePair<Vector2, AIRange> pair in sphere)
                    {
                        Vector2 pt = pair.Key;
                        AIRange range = pair.Value;
                        AddPtAndRangeToColliderYInfo(pt, range);
                    }
                }

                foreach (Dictionary<Vector2, AIRange> sphere in capsuleColliderRangeList)
                {
                    foreach (KeyValuePair<Vector2, AIRange> pair in sphere)
                    {
                        Vector2 pt = pair.Key;
                        AIRange range = pair.Value;
                        AddPtAndRangeToColliderYInfo(pt, range);
                    }
                }

                _colliderInfo.XZColliderPts = xzColliderPts;
                using (Stream fStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    BinaryFormatter binFormatter = new BinaryFormatter(); //创建二进制序列化器
                    binFormatter.Serialize(fStream, _colliderInfo);
                }
            }
        }

        /// <summary>
        /// 加载手动设置的不允许最终停球的区域的信息，用于排除些特殊情况
        /// </summary>
        private void LoadExcludedStopAreaData()
        {
            ProcessAreaCube("ExcludedArea", _excludedArea);
        }

        /// <summary>
        /// 加载手动标记的地图中地表凸起区域的信息
        /// </summary>
        private void LoadMountainAreaData()
        {
            ProcessAreaCube("MountainArea", _mountainArea);
        }


        /// <summary>
        /// 加载手动设置的不允许放置落点的区域的信息，用于排除一些特殊情况
        /// </summary>
        private void LoadUnsafeDropPtAreaData()
        {
            ProcessAreaCube("UnsafeDropPtArea", _unsafeDropPointSet);
        }

        #endregion


        #region 内部工具方法

        /// <summary>
        /// 处理手动标记的Cube区域的方法
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="container"></param>
        private void ProcessAreaCube(string nodeName, HashSet<Vector2> container)
        {
            GameObject objExcludedArea = GameObject.Find(nodeName);

            if (objExcludedArea != null)
            {
                BoxCollider[] boxList = objExcludedArea.GetComponentsInChildren<BoxCollider>();

                foreach (var box in boxList)
                {
                    Quaternion rotation = box.transform.rotation;
                    Vector3 center = box.bounds.center;
                    float width = box.size.x;
                    float height = box.size.z;

                    for (float x = center.x - width / 2f; x < center.x + width / 2f; x += Percision)
                    {
                        for (float y = center.z - height / 2f; y < center.z + height / 2f; y += Percision)
                        {
                            Vector3 pt3D = new Vector3(x, 0, y);
                            Vector3 diff = pt3D - center;
                            diff.y = 0;
                            Vector3 resultDiff = rotation * diff;
                            Vector3 rotatedPt3D = resultDiff + center;
                            Vector2 rotatedPt = new Vector2(rotatedPt3D.x, rotatedPt3D.z);
                            if (!IsPointOutOfMap(rotatedPt))
                            {
                                int xIndex = MapCoordinateToArrayIndexX(rotatedPt.x);
                                int yIndex = MapCoordinateToArrayIndexZ(rotatedPt.y);
                                Vector2 ptIndex = new Vector2(xIndex, yIndex);
                                if (!container.Contains(ptIndex))
                                {
                                    container.Add(ptIndex);
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 将球碰撞体离散成Y方向上的范围。将碰撞体转化为点坐标和碰撞区间的形式。
        /// </summary>
        /// <param name="sphereList"></param>
        /// <param name="xzColliderPts"></param>
        /// <returns></returns>
        private List<Dictionary<Vector2, AIRange>> ProcessColliderSphere(List<SPhysics.SceneSphere> sphereList,
            Dictionary<float, Dictionary<AISerializedVector2, int>> xzColliderPts)
        {
            List<Dictionary<Vector2, AIRange>> sphereColliderRangeList = new List<Dictionary<Vector2, AIRange>>();
            foreach (SPhysics.SceneSphere sphere in sphereList)
            {
                Vector3 center = sphere.center;
                Vector2 center2D = new Vector2(center.x, center.z);
                if (IsPointOutOfMap(center2D))
                {
                    continue;
                }

                Dictionary<Vector2, AIRange> dictSphereRange = new Dictionary<Vector2, AIRange>();
                float radius = sphere.radius * ColliderExpandRadiusFactor;

                Vector3 scale = sphere.scale;

                float scaledRadiusX = radius * scale.x;
                float scaledRadiusY = radius * scale.y;
                float scaledRadiusZ = radius * scale.z;

                float minX = AIMathf.FloorToPercision(center2D.x - scaledRadiusX, 0.1f);
                float maxX = AIMathf.FloorToPercision(center2D.x + scaledRadiusX, 0.1f);
                float minZ = AIMathf.FloorToPercision(center2D.y - scaledRadiusZ, 0.1f);
                float maxZ = AIMathf.FloorToPercision(center2D.y + scaledRadiusZ, 0.1f);
                float minY = AIMathf.FloorToPercision(center.y - scaledRadiusY, 0.1f);
                float maxY = AIMathf.FloorToPercision(center.y + scaledRadiusY, 0.1f);

                // y方向上碰撞体处理成区间
                for (float x = minX; x <= maxX; x += ColliderScanPercision)
                {
                    for (float z = minZ; z <= maxZ; z += ColliderScanPercision)
                    {
                        Vector2 pt = new Vector2(x, z);
                        if (Mathf.Pow((x - center.x) / scale.x, 2) + Mathf.Pow((z - center.z) / scale.z, 2) <
                            Mathf.Pow(radius, 2))
                        {
                            // 在椭圆内，可以计算出与y方向上直线与椭球球的俩交点
                            float diffY = Mathf.Sqrt((Mathf.Pow(radius, 2) - Mathf.Pow((x - center.x) / scale.x, 2) -
                                                      Mathf.Pow((z - center.z) / scale.z, 2)) * Mathf.Pow(scale.y, 2));
                            AIRange range;
                            range.Max = center.y + diffY;
                            range.Min = center.y - diffY;
                            float mapH = GetTerrainHeightAtPoint(pt);
                            if (range.Max <= mapH)
                            {
                                continue;
                            }

                            if (range.Min <= mapH)
                            {
                                range.Min = mapH;
                            }

                            AddColliderYRangeWithFloorAndCeilToDiction(dictSphereRange, pt, range.Min, range.Max);
                        }
                    }
                }

                // xz平面上碰撞体离散成点集
                for (float y = minY + ColliderScanPercision; y < maxY; y += ColliderScanPercision)
                {
                    for (float x = minX + ColliderScanPercision; x < maxX; x++)
                    {
                        float square = (Mathf.Pow(radius, 2) - Mathf.Pow((y - center.y) / scale.y, 2) -
                                        Mathf.Pow((x - center.x) / scale.x, 2)) * scale.z;
                        if (square > 0)
                        {
                            float r = Mathf.Sqrt(square);
                            float z1 = center.z + r;
                            float z2 = center.z - r;

                            float perX = AIMathf.FloorToPercision(x, Percision);
                            float perY = AIMathf.FloorToPercision(y, ColliderYPerception);
                            float perZ1 = AIMathf.FloorToPercision(z1, Percision);
                            float perZ2 = AIMathf.FloorToPercision(z2, Percision);

                            for (float zTemp = perZ2; zTemp <= perZ1; zTemp += Percision)
                            {
                                AddColliderXZPtToDict(xzColliderPts, perY, perX, zTemp);
                            }
                        }
                    }
                }

                sphereColliderRangeList.Add(dictSphereRange);
            }

            return sphereColliderRangeList;
        }

        /// <summary>
        /// 将胶囊体离散成Y方向上的碰撞体范围。将碰撞体转化为点坐标和碰撞区间的形式。胶囊体处理起来较麻烦，不正面处理旋转后的方程而采用取巧方法。将直立的胶囊体分为（椭）圆柱和上下两个半（椭）球
        /// 将竖立的胶囊体离散成点集（包括边界和内部）。如果胶囊体不是竖直放置的，则将离散出的点集全部绕中心旋转，得到另一个旋转后的点集。由于离散和地图精度，为了防止因精度而丢失造成点集出现镂空，
        /// 将不处于网格线上的点处的碰撞体y坐标视为包含该点的方格的四个顶点处都至少有这个碰撞y值。最终处理出的胶囊体的Range应该是包含原胶囊体但比原胶囊体大一些且不规则的集合。但是这不影响判断，
        /// 因为本就不希望打离碰撞体太近的位置，太冒险
        /// </summary>
        /// <param name="capsuleList"></param>
        /// <param name="xzColliderPts"></param>
        /// <returns></returns>
        private List<Dictionary<Vector2, AIRange>> ProcessColliderCapsule(List<SPhysics.SceneCapsule> capsuleList,
            Dictionary<float, Dictionary<AISerializedVector2, int>> xzColliderPts)
        {
            // 处理胶囊体
            List<Dictionary<Vector2, AIRange>> capsuleRangeList = new List<Dictionary<Vector2, AIRange>>();
            foreach (SPhysics.SceneCapsule capsule in capsuleList)
            {
                Dictionary<Vector2, AIRange> dictCapsuleRange = new Dictionary<Vector2, AIRange>();
                float radius = capsule.radius * ColliderExpandRadiusFactor;
                float height = capsule.height * ColliderExpandRadiusFactor;
                Vector3 center = capsule.center;
                Vector2 centerFloor = new Vector2(AIMathf.FloorToPercision(center.x, Percision),
                    AIMathf.FloorToPercision(center.z, Percision));
                if (IsPointOutOfMap(centerFloor))
                {
                    continue;
                }

                // raidans
                float angle = Mathf.Acos(capsule.rotation.w) * 2f;
                float degree1 = 1f * Mathf.Deg2Rad;

                Vector3 scale = capsule.scale;

                float scaledRadiusX = radius * scale.x;
                float scaledRadiusY = radius * scale.y;
                float scaledRadiusZ = radius * scale.z;
                float len = (height - 2 * radius) * scale.y;

                // 旋转程度不大的就当立着的

                if (Mathf.Abs(angle) < degree1 || (2 * Mathf.PI - Mathf.Abs(angle)) < degree1)
                {
                    float minX = AIMathf.FloorToPercision(center.x - scaledRadiusX, 0.1f);
                    float maxX = AIMathf.FloorToPercision(center.x + scaledRadiusX, 0.1f);
                    float minZ = AIMathf.FloorToPercision(center.z - scaledRadiusZ, 0.1f);
                    float maxZ = AIMathf.FloorToPercision(center.z + scaledRadiusZ, 0.1f);
                    float minY = AIMathf.FloorToPercision(center.y - scaledRadiusY / 2f, 0.1f);
                    float maxY = AIMathf.FloorToPercision(center.y + scaledRadiusY / 2f, 0.1f);


                    // y方向上处理成区间
                    for (float x = minX; x <= maxX; x += ColliderScanPercision)
                    {
                        for (float z = minZ; z <= maxZ; z += ColliderScanPercision)
                        {
                            Vector2 pt = new Vector2(x, z);
                            if (Mathf.Pow((x - center.x) / scale.x, 2) + Mathf.Pow((z - center.z) / scale.z, 2) <
                                Mathf.Pow(radius, 2))
                            {
                                // 在圆内，可以计算出与y方向上直线与球的俩交点
                                float diffY = Mathf.Sqrt((Mathf.Pow(radius, 2) -
                                                          Mathf.Pow((x - center.x) / scale.x, 2) -
                                                          Mathf.Pow((z - center.z) / scale.z, 2)) *
                                                         Mathf.Pow(scale.y, 2));
                                AIRange range;
                                range.Max = center.y + diffY + len / 2f;
                                range.Min = center.y - diffY - len / 2f;
                                float mapH = GetTerrainHeightAtPoint(pt);

                                if (range.Max <= mapH)
                                {
                                    continue;
                                }
                                else if (range.Min <= mapH)
                                {
                                    range.Min = mapH;
                                }

                                AddColliderYRangeWithFloorAndCeilToDiction(dictCapsuleRange, pt, range.Min, range.Max);
                            }
                        }
                    }


                    // xz平面上碰撞体离散成点集
                    for (float y = minY + ColliderScanPercision; y < maxY; y += ColliderScanPercision)
                    {
                        for (float x = minX + ColliderScanPercision; x < maxX; x++)
                        {
                            float square;
                            // 半球
                            if (y < center.y - len / 2f || y > center.y + len / 2f)
                            {
                                square = (Mathf.Pow(radius, 2) - Mathf.Pow(
                                                                   (Mathf.Abs(y - center.y) - len / 2f) / scale.y, 2)
                                                               - Mathf.Pow((x - center.x) / scale.x, 2)) *
                                         Mathf.Pow(scale.z, 2);
                            }
                            else // 圆柱体
                            {
                                square = (Mathf.Pow(radius, 2) - Mathf.Pow((x - center.x) / scale.x, 2)) *
                                         Mathf.Pow(scale.z, 2);
                            }

                            if (square > 0)
                            {
                                float r = Mathf.Sqrt(square);
                                float z1 = center.z + r;
                                float z2 = center.z - r;

                                float perX = AIMathf.FloorToPercision(x, Percision);
                                float perY = AIMathf.FloorToPercision(y, ColliderYPerception);
                                float perZ1 = AIMathf.FloorToPercision(z1, Percision);
                                float perZ2 = AIMathf.FloorToPercision(z2, Percision);

                                for (float zTemp = perZ2; zTemp <= perZ1; zTemp += Percision)
                                {
                                    AddColliderXZPtToDict(xzColliderPts, perY, perX, zTemp);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // 否则就得算旋转   

                    // 将立着未经旋转的胶囊离散成点
                    List<Vector3> capsuleSurfacePts = new List<Vector3>();
                    List<Vector3> capsuleAllPts = new List<Vector3>();

                    Vector3 centerToPercision = new Vector3(AIMathf.FloorToPercision(center.x, 0.1f),
                        AIMathf.FloorToPercision(center.y, 0.1f),
                        AIMathf.FloorToPercision(center.z, 0.1f));

                    float scaledRadiusXPercision = AIMathf.FloorToPercision(scaledRadiusX, 0.1f);
                    float scaledRadiusZPercision = AIMathf.FloorToPercision(scaledRadiusZ, 0.1f);

                    float minX = centerToPercision.x - scaledRadiusXPercision;
                    float maxX = centerToPercision.x + scaledRadiusXPercision;
                    float minZ = centerToPercision.z - scaledRadiusZPercision;
                    float maxZ = centerToPercision.z + scaledRadiusZPercision;

                    for (float x = minX; x <= maxX; x += ColliderScanPercision)
                    {
                        for (float z = minZ; z <= maxZ; z += ColliderScanPercision)
                        {
                            float diffToCurl = Mathf.Pow(radius, 2) - Mathf.Pow((x - center.x) / scale.x, 2) -
                                               Mathf.Pow((z - center.z) / scale.z, 2);
                            bool isOnCurl = Mathf.Sqrt(Mathf.Abs(diffToCurl)) < ColliderScanPercision;
                            bool isInCurl = diffToCurl > 0;
                            if (isOnCurl)
                            {
                                // 在圆上，存储一条竖直方向上的母线
                                float maxY = AIMathf.FloorToPercision(centerToPercision.y + len / 2f, 0.1f);
                                float minY = AIMathf.FloorToPercision(centerToPercision.y - len / 2f, 0.1f);
                                for (float y = minY; y <= maxY; y += ColliderScanPercision)
                                {
                                    Vector3 sPt3D = new Vector3(x, y, z);
                                    capsuleSurfacePts.Add(sPt3D);
                                    capsuleAllPts.Add(sPt3D);
                                }
                            }
                            else if (isInCurl)
                            {
                                // 在圆内, 计算顶上和下面两个圆
                                float diffY = Mathf.Sqrt(diffToCurl) * scale.y;
                                float topY = centerToPercision.y + len / 2f + diffY;
                                float bottomY = centerToPercision.y - len / 2f - diffY;

                                float maxY = AIMathf.FloorToPercision(topY, 0.1f);
                                float minY = AIMathf.FloorToPercision(bottomY, 0.1f);
                                capsuleSurfacePts.Add(new Vector3(x, topY, z));
                                capsuleSurfacePts.Add(new Vector3(x, bottomY, z));

                                for (float yTemp = minY; yTemp <= maxY; yTemp = yTemp + ColliderScanPercision)
                                {
                                    capsuleAllPts.Add(new Vector3(x, yTemp, z));
                                }
                            }
                        }
                    }

                    Quaternion quaternion = new Quaternion(capsule.rotation.x, capsule.rotation.y, capsule.rotation.z,
                        capsule.rotation.w);

                    // 将每一个离散的点进行旋转之后，存入到字典中。可能划归到精度之后会有重合的点，此时根据y来调整此点的range
                    Dictionary<Vector2, AIRange> tempDict = new Dictionary<Vector2, AIRange>();
                    foreach (Vector3 surfacePt in capsuleSurfacePts)
                    {
                        Vector3 rotatedPt = quaternion * (surfacePt - centerToPercision) + centerToPercision;

                        float y = AIMathf.FloorToPercision(rotatedPt.y, ColliderScanPercision);
                        AddColliderYRangeWithFloorAndCeilToDiction(tempDict, new Vector2(rotatedPt.x, rotatedPt.z), y,
                            y);
                    }

                    foreach (Vector3 allPt in capsuleAllPts)
                    {
                        Vector3 rotatedPt = quaternion * (allPt - centerToPercision) + centerToPercision;

                        float y = AIMathf.FloorToPercision(rotatedPt.y, ColliderYPerception);
                        float x = AIMathf.FloorToPercision(rotatedPt.x, Percision);
                        float z = AIMathf.FloorToPercision(rotatedPt.z, Percision);
                        AddColliderXZPtToDict(xzColliderPts, y, x, z);
                    }

                    // 切点或者出错的点就不要了
                    foreach (KeyValuePair<Vector2, AIRange> pair in tempDict)
                    {
                        AIRange range = pair.Value;
                        Vector2 pt = pair.Key;
                        float mapH = GetTerrainHeightAtPoint(pt);
                        if (Mathf.Abs(range.Max - mapH) > 0.0001f)
                        {
                            dictCapsuleRange.Add(pair.Key, range);
                        }
                    }
                }

                if (dictCapsuleRange.Count > 0)
                {
                    capsuleRangeList.Add(dictCapsuleRange);
                }
            }

            return capsuleRangeList;
        }

        /// <summary>
        /// 添加XZ平面方向上的碰撞体信息到字典中
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        private void AddColliderXZPtToDict(Dictionary<float, Dictionary<AISerializedVector2, int>> dict, float y,
            float x, float z)
        {
            if (!dict.ContainsKey(y))
            {
                dict.Add(y, new Dictionary<AISerializedVector2, int>());
            }

            var d = dict[y];
            AISerializedVector2 v = new AISerializedVector2(x, z);
            if (!d.ContainsKey(v))
            {
                d.Add(v, 0);
            }
        }

        /// <summary>
        /// 将处理后的Y方向上的碰撞体信息保存到Info数据结构里
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="range"></param>
        private void AddPtAndRangeToColliderYInfo(Vector2 pt, AIRange range)
        {
            if (IsPointOutOfMap(pt))
            {
                return;
            }

            AISerializedVector2 pPt = new AISerializedVector2(AIMathf.FloorToPercision(pt.x, Percision),
                AIMathf.FloorToPercision(pt.y, Percision));

            if (!_colliderInfo.YColliderRanges.ContainsKey(pPt))
            {
                _colliderInfo.YColliderRanges.Add(pPt, new AIYColliderInfo());
            }

            _colliderInfo.YColliderRanges[pPt].RangesAtPoint.AddRange(range);
        }

        /// <summary>
        /// 离散的碰撞体信息加上精度设置导致不精确，因此采取“膨胀”碰撞体的信息，小数点所在的格子的四个顶点都视为碰撞区域
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pt"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        private void AddColliderYRangeWithFloorAndCeilToDiction(Dictionary<Vector2, AIRange> dict, Vector2 pt,
            float min, float max)
        {
            float perFloorX = AIMathf.FloorToPercision(pt.x, Percision);
            float perFloorZ = AIMathf.FloorToPercision(pt.y, Percision);
            float perCeilX = AIMathf.CeilToPercision(pt.x, Percision);
            float perCeilZ = AIMathf.CeilToPercision(pt.y, Percision);

            Vector2 Pt2D = new Vector2(perFloorX, perFloorZ);
            Vector2 Pt2D1 = new Vector2(perCeilX, perCeilZ);
            Vector2 Pt2D2 = new Vector2(perCeilX, perFloorZ);
            Vector2 Pt2D3 = new Vector2(perFloorX, perCeilZ);

            if (!IsPointOutOfMap(Pt2D))
            {
                AddCollideYRangeToDictionary(dict, Pt2D, min, max);
            }

            if (!IsPointOutOfMap(Pt2D1))
            {
                AddCollideYRangeToDictionary(dict, Pt2D1, min, max);
            }

            if (!IsPointOutOfMap(Pt2D2))
            {
                AddCollideYRangeToDictionary(dict, Pt2D2, min, max);
            }

            if (!IsPointOutOfMap(Pt2D3))
            {
                AddCollideYRangeToDictionary(dict, Pt2D3, min, max);
            }
        }

        /// <summary>
        /// 添加y方向上的碰撞体范围到字典中
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pt"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        private void AddCollideYRangeToDictionary(Dictionary<Vector2, AIRange> dict, Vector2 pt, float min, float max)
        {
            if (dict.ContainsKey(pt))
            {
                AIRange range = dict[pt];
                range.Max = Mathf.Max(max, range.Max);
                range.Min = Mathf.Min(min, range.Min);
            }
            else
            {
                AIRange range;
                float mapH = GetTerrainHeightAtPoint(pt);
                range.Min = mapH;
                range.Max = mapH;
                range.Max = Mathf.Max(max, range.Max);
                range.Min = Mathf.Min(min, range.Min);
                dict.Add(pt, range);
            }
        }


        /// <summary>
        /// 二维数组坐标映射到一维
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int ConvertMapIndex(int x, int y)
        {
            return MapMaxXIndex * y + x;
        }

        #endregion

        #region 公有接口，提供地图各种信息

        /// <summary>
        /// 获取pt处Y方向上碰撞体范围集合
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public AISet GetColliderYRangesAtPoint(Vector2 pt)
        {
            AISerializedVector2 pPt = new AISerializedVector2(AIMathf.FloorToPercision(pt.x, Percision),
                AIMathf.FloorToPercision(pt.y, Percision));
            if (_colliderInfo.YColliderRanges.ContainsKey(pPt))
            {
                return _colliderInfo.YColliderRanges[pPt].RangesAtPoint;
            }

            return null;
        }


        /// <summary>
        /// 判断点是否在碰撞体内
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool IsPointInCollider(Vector3 pt)
        {
            Vector2 pt2D = new Vector2(pt.x, pt.z);
            AISet collideRange = GetColliderYRangesAtPoint(pt2D);
            if (collideRange != null && collideRange.Ranges.Count > 0)
            {
                foreach (AIRange range in collideRange.Ranges)
                {
                    if (pt.y <= range.Max && pt.y >= range.Min)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// 检测某y高度处，xz点处是否在碰撞体内
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool IsPointInXZCollider(Vector3 pt)
        {
            bool isCollider = false;

            if (_colliderInfo != null)
            {
                float perX = AIMathf.FloorToPercision(pt.x, Percision);
                float perY = AIMathf.FloorToPercision(pt.y, ColliderYPerception);
                float perZ = AIMathf.FloorToPercision(pt.z, Percision);
                if (_colliderInfo.XZColliderPts.ContainsKey(perY))
                {
                    var dict = _colliderInfo.XZColliderPts[perY];

                    if (dict.ContainsKey(new AISerializedVector2(perX, perZ)))
                    {
                        isCollider = true;
                    }
                }
            }

            return isCollider;
        }

        /// <summary>
        /// 通过物理接口直接获取某点的地形材质
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetTerrainTypeAtPointByPhysic(float x, float y)
        {
            int gt = _physic.getGroundType(x, y);
            // 界外的多种情况都规划为0
            if (gt == -1 || gt == -2 || gt == 7 || gt == 10 || gt == 11)
            {
                gt = 0;
            }

            // road等同于rough
            if (gt == 9)
            {
                gt = 2;
            }

            return gt;
        }


        public int GetTerrainTypeAtPointByPhysic(Vector2 vec)
        {
            return GetTerrainTypeAtPointByPhysic(vec.x, vec.y);
        }


        /// <summary>
        /// 获取点处地表材质
        /// 0: 地图外
        /// 1: 球道
        /// 2: 长草区， 道路
        /// 3: 沙坑
        /// 4: 果岭
        /// 5: 果岭边缘
        /// 6: 球洞
        /// 7: 水
        /// 8: 开球区
        /// </summary>
        /// <param name="x">点x坐标（原值）</param>
        /// <param name="y">点y坐标（原值）</param>
        /// <returns>材质类型</returns>
        public int GetTerrainTypeAtPoint(float x, float y)
        {
            if ((_groundTypeInfo == null) ||
                (x < MapOrigin.x) ||
                (x >= MapOrigin.x + MapXLength) ||
                (y < MapOrigin.z) ||
                (y >= MapOrigin.z + MapZLength))
            {
                return 7;
            }
            else
            {
                int intX = MapCoordinateToArrayIndexX(x);
                int intY = MapCoordinateToArrayIndexZ(y);
                return GetTerrainTypeByIndex(intX, intY);
            }
        }


        /// <summary>
        /// 获取点处地表材质
        /// 0: 地图外
        /// 1: 球道
        /// 2: 长草区， 道路
        /// 3: 沙坑
        /// 4: 果岭
        /// 5: 果岭边缘
        /// 6: 球洞
        /// 7: 水
        /// 8: 开球区
        /// </summary>
        /// <param name="pt">坐标（原值）</param>
        /// <returns>材质类型</returns>
        public int GetTerrainTypeAtPoint(Vector2 pt)
        {
            return GetTerrainTypeAtPoint(pt.x, pt.y);
        }

        /// <summary>
        /// 获取点处地表材质
        /// 0: 地图外
        /// 1: 球道
        /// 2: 长草区， 道路
        /// 3: 沙坑
        /// 4: 果岭
        /// 5: 果岭边缘
        /// 6: 球洞
        /// 7: 水
        /// 8: 开球区
        /// </summary>
        /// <param name="indexX">x坐标对应在地图数组中的引索值</param>
        /// <param name="indexY">y坐标对应在地图数组中的引索值</param>
        /// <returns>材质类型</returns>
        public int GetTerrainTypeByIndex(int indexX, int indexY)
        {
            if ((_groundTypeInfo == null) ||
                (indexX < 0) ||
                (indexX >= MapMaxXIndex) ||
                (indexY < 0) ||
                (indexY >= MapMaxZIndex))
            {
                return 7;
            }
            else
            {
                return _groundTypeInfo.TerrainGTData[ConvertMapIndex(indexX, indexY)];
            }
        }


        /// <summary>
        /// 获取点处地表材质
        /// 0: 地图外
        /// 1: 球道
        /// 2: 长草区， 道路
        /// 3: 沙坑
        /// 4: 果岭
        /// 5: 果岭边缘
        /// 6: 球洞
        /// 7: 水
        /// 8: 开球区
        /// </summary>
        /// <param name="index">坐标对应在地图数组中的引索值</param>
        /// <returns>材质类型</returns>
        public int GetTerrainTypeByIndex(Vector2 index)
        {
            int indexX = (int) index.x;
            int indexY = (int) index.y;
            return GetTerrainTypeByIndex(indexX, indexY);
        }


        /// <summary>
        /// 坐标处地表高度
        /// </summary>
        /// <param name="x">x坐标原值</param>
        /// <param name="y">y坐标原值</param>
        /// <returns></returns>
        public float GetTerrainHeightAtPoint(float x, float y)
        {
            if (_groundTypeInfo == null ||
                x < MapOrigin.x ||
                x >= MapOrigin.x + MapXLength ||
                y < MapOrigin.z ||
                y >= MapOrigin.z + MapZLength)
            {
                return 0;
            }
            else
            {
                int intX = MapCoordinateToArrayIndexX(x);
                int intY = MapCoordinateToArrayIndexZ(y);
                return GetTerrainHeightByIndex(intX, intY);
            }
        }

        /// <summary>
        /// 坐标处地表高度
        /// </summary>
        /// <param name="pt">坐标原值</param>
        /// <returns></returns>
        public float GetTerrainHeightAtPoint(Vector2 pt)
        {
            return GetTerrainHeightAtPoint(pt.x, pt.y);
        }


        /// <summary>
        /// 坐标处地表高度
        /// </summary>
        /// <param name="indexX">x坐标对应地图数组中的引索值</param>
        /// <param name="indexY">y坐标对应地图数组中的引索值</param>
        /// <returns></returns>
        public float GetTerrainHeightByIndex(int indexX, int indexY)
        {
            if (_groundTypeInfo == null ||
                indexX < 0 ||
                indexX >= MapMaxXIndex ||
                indexY < 0 ||
                indexY >= MapMaxZIndex)
            {
                return -186;
            }

            return _heightInfo.TerrainHData[ConvertMapIndex(indexX, indexY)];
        }

        /// <summary>
        /// 坐标处地表高度
        /// </summary>
        /// <param name="index">坐标对应地图数组中的引索值</param>
        /// <returns></returns>
        public float GetTerrainHeightByIndex(Vector2 index)
        {
            int indexX = (int) index.x;
            int indexY = (int) index.y;
            return GetTerrainHeightByIndex(indexX, indexY);
        }


        /// <summary>
        /// 坐标值转换为地图数组引索值
        /// </summary>
        /// <param name="value">x坐标</param>
        /// <returns></returns>
        public int MapCoordinateToArrayIndexX(float value)
        {
            int index = Mathf.FloorToInt((value - MapOrigin.x) / Percision);
            return index;
        }

        /// <summary>
        /// 坐标值转换为地图数组引索值
        /// </summary>
        /// <param name="value">z坐标</param>
        /// <returns></returns>
        public int MapCoordinateToArrayIndexZ(float value)
        {
            int index = Mathf.FloorToInt((value - MapOrigin.z) / Percision);
            return index;
        }

        /// <summary>
        /// 坐标值转换为地图数组引索值
        /// </summary>
        /// <param name="value">坐标</param>
        /// <returns></returns>
        public Vector2 MapCoordinateToArrayIndex(Vector2 value)
        {
            int indexX = MapCoordinateToArrayIndexX(value.x);
            int indexY = MapCoordinateToArrayIndexZ(value.y);
            Vector2 result = new Vector2(indexX, indexY);
            return result;
        }

        /// <summary>
        /// 地图数组引索值转换为坐标值
        /// </summary>
        /// <param name="value">引索值</param>
        /// <returns></returns>
        public Vector2 ArrayIndexToMapCoordinate(Vector2 value)
        {
            Vector2 mapCoor = new Vector2(ArrayIndexToMapCoordinateX((int) value.x),
                ArrayIndexToMapCoordinateZ((int) value.y));
            return mapCoor;
        }

        /// <summary>
        /// 地图数组引索值转换为坐标值
        /// </summary>
        /// <param name="value">x坐标引索值</param>
        /// <returns></returns>
        public float ArrayIndexToMapCoordinateX(int value)
        {
            float mapCoorX = MapOrigin.x + value * Percision;
            float xNormal = AIMathf.FloorToPercision(mapCoorX, Percision);
            return xNormal;
        }

        /// <summary>
        /// 地图数组引索值转换为坐标值
        /// </summary>
        /// <param name="value">z坐标引索值</param>
        /// <returns></returns>
        public float ArrayIndexToMapCoordinateZ(int value)
        {
            float mapCoorY = MapOrigin.z + value * Percision;
            float yNormal = AIMathf.FloorToPercision(mapCoorY, Percision);
            return yNormal;
        }


        /// <summary>
        /// 检查坐标是否在不允许放置落点的区域中
        /// </summary>
        /// <param name="pt">坐标原始值</param>
        /// <returns></returns>
        public bool IsPointInUnsafeDropPtArea(Vector2 pt)
        {
            if (_unsafeDropPointSet.Contains(MapCoordinateToArrayIndex(pt)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查坐标是否在不允许放置落点的区域中
        /// </summary>
        /// <param name="pt">坐标引索值</param>
        /// <returns></returns>
        public bool IsPointInUnsafeDropIndexArea(Vector2 pt)
        {
            if (_unsafeDropPointSet.Contains(pt))
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 检查线段是否穿过手动标记的凸起地形
        /// </summary>
        /// <param name="start">线段起始点</param>
        /// <param name="diff">线段向量（终点 - 起点）</param>
        /// <returns></returns>
        public bool IsLineCrossMountainArea(Vector2 start, Vector2 diff)
        {
            for (float t = 0f; t < diff.magnitude; t += Percision)
            {
                Vector2 curPt = start + diff * (t / diff.magnitude);
                if (IsPointInMountainArea(curPt))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// 检查点是否在手动标记的凸起地形区域内
        /// </summary>
        /// <param name="pt">原始坐标</param>
        /// <returns></returns>
        public bool IsPointInMountainArea(Vector2 pt)
        {
            return _mountainArea.Contains(MapCoordinateToArrayIndex(pt));
        }


        /// <summary>
        /// 检查点是否在手动标记的凸起地形区域内
        /// </summary>
        /// <param name="ptIndex">坐标引索值</param>
        /// <returns></returns>
        public bool IsPointIndexInMountainArea(Vector2 ptIndex)
        {
            return _mountainArea.Contains(ptIndex);
        }


        /// <summary>
        /// 检查点是否在手动标记的不允许停球的区域内
        /// </summary>
        /// <param name="pt">原始坐标</param>
        /// <returns></returns>
        public bool IsPointInExcludedStopArea(Vector2 pt)
        {
            return _excludedArea.Contains(MapCoordinateToArrayIndex(pt));
        }


        /// <summary>
        /// 检查点是否在手动标记的不允许停球的区域内
        /// </summary>
        /// <param name="ptIndex">引索坐标</param>
        /// <returns></returns>
        public bool IsPointIndexInExcludedStopArea(Vector2 ptIndex)
        {
            return _excludedArea.Contains(ptIndex);
        }


        /// <summary>
        /// 检查地形材质是否被定义为“危险”地形
        /// </summary>
        /// <param name="groundType"></param>
        /// <returns></returns>
        public static bool IsGroundTypeDangerZone(int groundType)
        {
            bool isDangerZone;
            if (AIModeSwitch.IsInExcludeRoughDropPointMode)
            {
                isDangerZone = IsGroundTypeRough(groundType) || groundType == 3 || IsGroundTypeOut(groundType);
            }
            else
            {
                isDangerZone = groundType == 3 || IsGroundTypeOut(groundType);
            }

            return isDangerZone;
        }

        /// <summary>
        /// 检查地形材质是否被定义为“出界”地形
        /// </summary>
        /// <param name="groundType"></param>
        /// <returns></returns>
        public static bool IsGroundTypeOut(int groundType)
        {
            bool isOut = groundType <= 0 || groundType == 7 || groundType == 10 || groundType == 11;
            return isOut;
        }


        public static bool IsGroundTypeRough(int groundType)
        {
            return groundType == 2 || groundType == 9;
        }
        
        /// <summary>
        /// 检查引索坐标是否出界
        /// </summary>
        /// <param name="pt">引索坐标</param>
        /// <returns></returns>
        public bool IsPointIndexOutOfMap(Vector2 pt)
        {
            if (pt.x < 0 || pt.x >= MapMaxXIndex || pt.y < 0 || pt.y >= MapMaxZIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查引索坐标是否出界
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsPointIndexOutOfMap(int x, int y)
        {
            if (x < 0 || x >= MapMaxXIndex || y < 0 || y >= MapMaxZIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查原始坐标是否出界
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool IsPointOutOfMap(Vector2 pt)
        {
            return IsPointIndexOutOfMap(MapCoordinateToArrayIndex(pt));
        }


        /// <summary>
        /// 检查原始坐标是否出界
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsPointOutOfMap(float x, float y)
        {
            return IsPointIndexOutOfMap(MapCoordinateToArrayIndexX(x), MapCoordinateToArrayIndexZ(y));
        }


        public List<Vector2> GeneratePointArrayByAreaName(string areaName, float percision)
        {
            GameObject objExcludedArea = GameObject.Find(areaName);
            List<Vector2> container = new List<Vector2>();
            if (objExcludedArea != null)
            {
                BoxCollider[] boxList = objExcludedArea.GetComponentsInChildren<BoxCollider>();

                foreach (var box in boxList)
                {
                    Quaternion rotation = box.transform.rotation;
                    Vector3 center = box.bounds.center;
                    float width = box.size.x;
                    float height = box.size.z;

                    for (float x = center.x - width / 2f; x < center.x + width / 2f; x += percision)
                    {
                        for (float y = center.z - height / 2f; y < center.z + height / 2f; y += percision)
                        {
                            Vector3 pt3D = new Vector3(x, 0, y);
                            Vector3 diff = pt3D - center;
                            diff.y = 0;
                            Vector3 resultDiff = rotation * diff;
                            Vector3 rotatedPt3D = resultDiff + center;
                            Vector2 rotatedPt = new Vector2(rotatedPt3D.x, rotatedPt3D.z);
                            if (!IsPointOutOfMap(rotatedPt))
                            {
                                container.Add(rotatedPt);
                            }
                        }
                    }
                }
            }

            return container;
        }

        #endregion


        #region 内部中间数据结构

        private struct AIGroundTypeMap
        {
            public AIGroundTypeArray[] map;
        }

        private struct AIGroundTypeArray
        {
            public int[] GroundArray;
        }

        [Serializable]
        private class AITerrainRectRegionInfo
        {
            public int[] RectangleRegionData;
        }

        [Serializable]
        private class AITerrainRectInfo
        {
            public AIRect[] RectangleData;
        }

        [Serializable]
        private class AITerrainHeightInfo
        {
            public float[] TerrainHData;
        }

        [Serializable]
        private class AITerrainGroundTypeInfo
        {
            public AISerializedVector3 MapOrigin;

            public int MapMaxXIndex;

            public int MapMaxYIndex;

            public int MapMaxZIndex;

            public int[] TerrainGTData;
        }

        [Serializable]
        private class AIYColliderInfo
        {
            public AISet RangesAtPoint;

            public AIYColliderInfo()
            {
                RangesAtPoint = new AISet();
            }
        }


        [Serializable]
        private class AIColliderInfo
        {
            public Dictionary<AISerializedVector2, AIYColliderInfo> YColliderRanges =
                new Dictionary<AISerializedVector2, AIYColliderInfo>();

            public Dictionary<float, Dictionary<AISerializedVector2, int>> XZColliderPts =
                new Dictionary<float, Dictionary<AISerializedVector2, int>>();
        }

        #endregion
    }
}
#endif