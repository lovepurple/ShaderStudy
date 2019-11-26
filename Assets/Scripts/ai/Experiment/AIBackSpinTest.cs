#if AI_GUIDE_LINE_WITH_SPIN
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ai
{
    public class AIBackSpinTest: MonoBehaviour {
        [System.Serializable]
        public class AIBackSpinData {

            public float[] shotPt;

            public float[] targetPt;

            public float[] dropPt1;

            public float[] dropPt2;

            public float distance;

            public float backSpin;

            public float ratio;

            public AIBackSpinData () {
                shotPt = new float[2];
                dropPt1 = new float[2];
                dropPt2 = new float[2];
                targetPt = new float[2];
            }
        }

        [System.Serializable]
        public class AIBackSpinDataList {
            public AIBackSpinData[] spinList;

            public AIBackSpinDataList (int count) {
                spinList = new AIBackSpinData[count];
            }
        }

        public void Start () {
            PhysicsDataManager physics = new PhysicsDataManager ();
            PhysicsManager.Instance.EnterGame ();
            PhysicsManager.Instance.ballController.ballBounciness = 1;
            // int distance = 340;
            for (int dis = 10; dis <= 350; dis = dis + 5) {

                List<AIBackSpinData> spinList = new List<AIBackSpinData> ();

                Vector2 shotPt = Vector2.zero;
                Vector2 dropPt = new Vector2 (-0, -dis);
                SPhysics.AiLine guideLineOri = physics.Line (shotPt, dropPt, 0, 4, 0.26f, 0);
                Vector2 dropPt1Ori = new Vector2 (guideLineOri.bounceList[0].x, guideLineOri.bounceList[0].z);
                Vector2 dropPt2Ori = new Vector2 (guideLineOri.bounceList[1].x, guideLineOri.bounceList[1].z);
                float disOri = Vector2.Distance(dropPt1Ori, dropPt2Ori);

                for (float i = 0f; i > -10f - 0.1; i = i - 0.1f) {
                    SPhysics.AiLine guideLine = physics.Line (shotPt, dropPt, i, 4, 0.26f, 0);
                    if (guideLine.bounceList.Count > 2) {

                        AIBackSpinData spin = new AIBackSpinData ();
                        Vector2 dropPt1 = new Vector2 (guideLine.bounceList[0].x, guideLine.bounceList[0].z);
                        Vector2 dropPt2 = new Vector2 (guideLine.bounceList[1].x, guideLine.bounceList[1].z);
                        
                        float distance1 = Vector2.Distance(shotPt, dropPt);
                        float distance2 = Vector2.Distance(dropPt1, dropPt2);

                        spin.shotPt[0] = shotPt.x;
                        spin.shotPt[1] = shotPt.y;

                        spin.dropPt1[0] = dropPt1.x;
                        spin.dropPt1[1] = dropPt1.y;

                        spin.dropPt2[0] = dropPt2.x;
                        spin.dropPt2[1] = dropPt2.y;

                        spin.targetPt[0] = dropPt.x;
                        spin.targetPt[1] = dropPt.y;

                        spin.distance = distance1;
                        spin.backSpin = i;
                        spin.ratio = distance2 / disOri;
                        spinList.Add (spin);
                    }
                }

                AIBackSpinDataList list = new AIBackSpinDataList (spinList.Count);
                list.spinList = spinList.ToArray ();

                string json = JsonUtility.ToJson (list, true);
                string filePath = Application.persistentDataPath + "//backspindata26//backSpinData_" + dis + ".json";
                FileInfo fileInfo = new FileInfo (filePath);
                if (fileInfo.Exists) {
                    File.Delete (filePath);
                }

                File.WriteAllText (filePath, json, Encoding.UTF8);
            }
        }   
    }
}


#endif