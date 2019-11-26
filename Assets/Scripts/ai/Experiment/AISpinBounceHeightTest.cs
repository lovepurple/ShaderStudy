#if AI_GUIDE_LINE_WITH_SPIN
using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Assets.Scripts.ai
{
    public class AISpinBounceHeightTest : MonoBehaviour {
        [Serializable]
        public class AISpinBounceHeightData {

            public float[] shotPt;

            public float[] targetPt;

            public float[] dropPt1;

            public float[] dropPt2;

            public float height1;

            public float height2;
            public float spin;

            public float diffH1;
            public float diffH2;
            public AISpinBounceHeightData () {
                shotPt = new float[2];
                dropPt1 = new float[2];
                dropPt2 = new float[2];
                targetPt = new float[2];
            }
        }

        [System.Serializable]
        public class AISpinBounceHeightList {
            public AISpinBounceHeightData[] spinList;

            public AISpinBounceHeightList (int count) {
                spinList = new AISpinBounceHeightData[count];
            }
        }

        public void Start () {
            PhysicsDataManager physics = new PhysicsDataManager ();
            PhysicsManager.Instance.EnterGame ();
            PhysicsManager.Instance.ballController.ballBounciness = 1;

            Vector2 dropPt0 = new Vector2(-38.929542541503909f, -194.64974975585938f);
            SPhysics.AiLine line = physics.Line(Vector2.zero, dropPt0, 0f, 4, 0.25999999046325686f);
            SPhysics.AiLine line1 = physics.Line(Vector2.zero, dropPt0, 10f, 4, 0.25999999046325686f);


            return;
            // int distance = 340;
            for (int dis = 20; dis <= 350; dis = dis + 5) {

                List<AISpinBounceHeightData> spinList = new List<AISpinBounceHeightData> ();

                Vector2 shotPt = Vector2.zero;
                Vector2 dropPt = new Vector2 (-0, -dis);
                SPhysics.AiLine guideLineOri = physics.Line (shotPt, dropPt, 0, 4, 0.34f, 0);
                Vector2 dropPt1Ori = new Vector2 (guideLineOri.bounceList[0].x, guideLineOri.bounceList[0].z);
                Vector2 dropPt2Ori = new Vector2 (guideLineOri.bounceList[1].x, guideLineOri.bounceList[1].z);
                float oriHeight1 = guideLineOri.peakList[0].y;
                float oriHeight2 = guideLineOri.peakList[1].y;

                // for (float i = 0f; i < 10f + 0.1; i = i + 0.1f) {
                for (float i = 0f; i > -10f - 0.1; i = i - 0.1f) {
                    SPhysics.AiLine guideLine = physics.Line (shotPt, dropPt, i, 4, 0.34f, 0);
                    if (guideLine.bounceList.Count > 2) {

                        AISpinBounceHeightData spin = new AISpinBounceHeightData ();
                        Vector2 dropPt1 = new Vector2 (guideLine.bounceList[0].x, guideLine.bounceList[0].z);
                        Vector2 dropPt2 = new Vector2 (guideLine.bounceList[1].x, guideLine.bounceList[1].z);
                        
                        float height1 = guideLine.peakList[0].y;
                        float height2 = guideLine.peakList[1].y;

                        spin.shotPt[0] = shotPt.x;
                        spin.shotPt[1] = shotPt.y;

                        spin.dropPt1[0] = dropPt1.x;
                        spin.dropPt1[1] = dropPt1.y;

                        spin.dropPt2[0] = dropPt2.x;
                        spin.dropPt2[1] = dropPt2.y;

                        spin.targetPt[0] = dropPt.x;
                        spin.targetPt[1] = dropPt.y;

                        spin.height1 = height1;
                        spin.height2 = height2;
                        
                        spin.spin = i;
                        spin.diffH1 = -(oriHeight1 - height1);
                        spin.diffH2 = -(oriHeight2 - height2);
                        
                        spinList.Add (spin);
                    }
                }

                AISpinBounceHeightList list = new AISpinBounceHeightList (spinList.Count);
                list.spinList = spinList.ToArray ();

                string json = JsonUtility.ToJson (list, true);
                string filePath =
 Application.persistentDataPath + "//backspinBounceHeight34//backSpinHeightData_" + dis + ".json";
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