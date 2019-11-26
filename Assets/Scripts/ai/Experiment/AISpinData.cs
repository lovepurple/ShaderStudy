#if AI_GUIDE_LINE_WITH_SPIN
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace Assets.Scripts.ai
{
    public class AISpinData
    {
        private static AISpinData instance;

        public const float maxDistance = 500f;

        public const float disPercision = 0.1f;

        public const float spinPercision = 0.1f;

        public const float maxSpin = 10f;

        private AITopSpinData mTopSpinData;

        private AISideSpinData mSideSpinData;

        private int maxDistanceIndex = 0;
        private int maxSpinIndex = 0;

        private AISpinData(){
            maxDistanceIndex = Mathf.CeilToInt(maxDistance/disPercision);
            maxSpinIndex = Mathf.CeilToInt(maxSpin/spinPercision);
        }

        public static AISpinData Instance{
            get{
                if(null == instance)
                    instance = new AISpinData();
                return instance;
            }

            set{

            }
        }

        public void initData(){
            initSideSpinData();
            initTopSpinData();
        }

        private void initSideSpinData(){
            string filePath = Application.persistentDataPath + "//sideSpin.dat";
            FileInfo fileInfo = new FileInfo (filePath);

            if (fileInfo.Exists) {
                Stream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器
                mSideSpinData = binFormat.Deserialize(fStream) as AISideSpinData;
                fStream.Close();
            } else {
                float[] sideSpinDataList = new float[maxDistanceIndex * maxSpinIndex];
                int disIndex = 0;
                for(float dis = 0; dis<maxDistance; dis += disPercision){
                    int spinIndex = 0;
                    for(float spin = 0;spin<=maxSpin;spin += spinPercision){
                        float angle = calcSideSpinAngle(dis, spin);
                        sideSpinDataList[convert2DIndexTo1DIndex(disIndex, spinIndex, maxDistanceIndex)] = angle;
                        spinIndex++;
                    }
                    disIndex++;
                }
                Stream fStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器
                mSideSpinData = new AISideSpinData();
                mSideSpinData.sideSpinData = sideSpinDataList;
                binFormat.Serialize(fStream, mSideSpinData);
                fStream.Close();                
            }
        }

        private void initTopSpinData(){
            string filePath = Application.persistentDataPath + "//topSpin.dat";
            FileInfo fileInfo = new FileInfo (filePath);

            if (fileInfo.Exists) {
                Stream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器
                mTopSpinData = binFormat.Deserialize(fStream) as AITopSpinData;
                fStream.Close();
            } else {
                float[] topSpinDataList = new float[maxDistanceIndex * maxSpinIndex];
                int disIndex = 0;
                for(float dis = 0; dis<maxDistance; dis += disPercision){
                    int spinIndex = 0;
                    for(float spin = 0;spin<=maxSpin;spin += spinPercision){
                        float ratio = calcTopSpinRatio(dis, spin);
                        topSpinDataList[convert2DIndexTo1DIndex(disIndex, spinIndex, maxDistanceIndex)] = ratio;
                        spinIndex++;
                    }
                    disIndex++;
                }
                Stream fStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器
                mTopSpinData = new AITopSpinData();
                mTopSpinData.topSpinData = topSpinDataList;
                binFormat.Serialize(fStream, mTopSpinData);
                fStream.Close();                
            }
        }

        public float getSideSpinAngle(float dis, float sideSpin){
            int disIndex = (int)(Mathf.Abs(dis)/disPercision);
            int sideSpinIndex = (int)(Mathf.Abs(sideSpin)/disPercision);
            float angle =
 mSideSpinData.sideSpinData[convert2DIndexTo1DIndex(disIndex, sideSpinIndex, maxDistanceIndex)];
            int sign = sideSpin >= 0 ? 1 : -1;
            angle *= sign;
            return angle;
        }


        public float getTopSpinRatio(float dis, float topSpin){
            int disIndex = (int)(Mathf.Abs(dis)/disPercision);
            int topSpinIndex = (int)(Mathf.Abs(topSpin)/disPercision);
            float ratio = mTopSpinData.topSpinData[convert2DIndexTo1DIndex(disIndex, topSpinIndex, maxDistanceIndex)];
            int sign = topSpin >= 0 ? 1 : -1;
            ratio *= sign;
            return ratio;
        }

        private float calcSideSpinAngle (float dis, float sideSpin) {            
            float angle =
 Mathf.Pow(Mathf.Abs(dis), 0.29321248147933016f)*Mathf.Pow(Mathf.Abs(sideSpin), 0.9331327421135225f) * 0.439308116511f; 
            return angle;
        }

        private float calcTopSpinRatio(float dis, float topSpin){
            float ratio =
 Mathf.Pow(Mathf.Abs(dis), 0.2961317129441604f)*Mathf.Pow(Mathf.Abs(topSpin), 1.1395722744270127f) * 0.0144823954547f; 
            return ratio;
        }

        private int convert2DIndexTo1DIndex(int x , int y, int maxXIndex){
            
            return maxXIndex * y + x;
        }

        [Serializable]
        private class AISideSpinData{
            public float[] sideSpinData;
        }

        [Serializable]
        private class AITopSpinData{
            public float[] topSpinData;
        }

    }
}

#endif