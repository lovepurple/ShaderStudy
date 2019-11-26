#if AI_GUIDE_LINE_WITH_SPIN
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ai {
    public class AIBounceTest : MonoBehaviour {
        [System.Serializable]
        public class AIBounceData {

            public float[] shotPt;

            public float[] targetPt;

            public float[] dropPt1;

            public float[] dropPt2;

            public float[] dropPt3;

            public float[] dropPt4;

            public float distance;

            public float ratio1;

            public float ratio2;

            public float ratio3;

            public AIBounceData () {
                shotPt = new float[2];
                dropPt1 = new float[2];
                dropPt2 = new float[2];
                dropPt3 = new float[2];
                dropPt4 = new float[2];
                targetPt = new float[2];
            }
        }

        [System.Serializable]
        public class AIBounceDataList {
            public AIBounceData[] spinList;

            public AIBounceDataList (int count) {
                spinList = new AIBounceData[count];
            }
        }

        public void Start () {
            PhysicsDataManager physics = new PhysicsDataManager ();
            PhysicsManager.Instance.EnterGame ();
            PhysicsManager.Instance.ballController.ballBounciness = 1;
            List<AIBounceData> bounceDataList = new List<AIBounceData> ();
            // int distance = 340;
            for (int dis = 50; dis <= 350; dis = dis + 5) {

                Vector2 shotPt = Vector2.zero;
                Vector2 dropPt = new Vector2 (-0, -dis);
                SPhysics.AiLine guideLine = physics.Line (shotPt, dropPt, 0, 4, 0.3f, 0);
                if(guideLine.bounceList.Count < 5){
                    continue;
                }
                if (guideLine.bounceList.Count > 2) {
                    AIBounceData bounceData = new AIBounceData ();

                    Vector2 dropPt1 = new Vector2 (guideLine.bounceList[0].x, guideLine.bounceList[0].z);
                    Vector2 dropPt2 = new Vector2 (guideLine.bounceList[1].x, guideLine.bounceList[1].z);
                    Vector2 dropPt3 = new Vector2 (guideLine.bounceList[2].x, guideLine.bounceList[2].z);
                    Vector2 dropPt4 = new Vector2 (guideLine.bounceList[3].x, guideLine.bounceList[3].z);

                    bounceData.shotPt[0] = shotPt.x;
                    bounceData.shotPt[1] = shotPt.y;

                    bounceData.dropPt1[0] = dropPt1.x;
                    bounceData.dropPt1[1] = dropPt1.y;

                    bounceData.dropPt2[0] = dropPt2.x;
                    bounceData.dropPt2[1] = dropPt2.y;

                    bounceData.dropPt3[0] = dropPt3.x;
                    bounceData.dropPt3[1] = dropPt3.y;

                    bounceData.dropPt4[0] = dropPt4.x;
                    bounceData.dropPt4[1] = dropPt4.y;

                    bounceData.targetPt[0] = dropPt.x;
                    bounceData.targetPt[1] = dropPt.y;

                    bounceData.distance = Vector2.Distance (shotPt, dropPt);

                    bounceData.ratio1 = Vector2.Distance (dropPt2, dropPt1) / Vector2.Distance (dropPt1, shotPt);
                    bounceData.ratio2 = Vector2.Distance (dropPt3, dropPt2) / Vector2.Distance (dropPt2, dropPt1);
                    bounceData.ratio3 = Vector2.Distance (dropPt4, dropPt3) / Vector2.Distance (dropPt3, dropPt2);

                    bounceDataList.Add (bounceData);
                }
            }

            AIBounceDataList list = new AIBounceDataList (bounceDataList.Count);
            list.spinList = bounceDataList.ToArray ();

            string json = JsonUtility.ToJson (list, true);
            string filePath = Application.persistentDataPath + "//bounceTest//bounceTest.json";
            FileInfo fileInfo = new FileInfo (filePath);
            if (fileInfo.Exists) {
                File.Delete (filePath);
            }

            File.WriteAllText (filePath, json, Encoding.UTF8);
        }
    }
}

#endif