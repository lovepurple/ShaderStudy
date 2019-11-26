#if AI_GUIDE_LINE_WITH_SPIN
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ai {
    public class AISideSpinTest : MonoBehaviour {
        [System.Serializable]
        public class AISideSpinData {

            public float[] shotPt;

            public float[] targetPt;

            public float[] dropPt1;

            public float[] dropPt2;

            public float distance;
            public float sideSpin;

            public float angle;

            public AISideSpinData () {
                shotPt = new float[2];
                dropPt1 = new float[2];
                dropPt2 = new float[2];
                targetPt = new float[2];
            }
        }

        [System.Serializable]
        public class AISideSpinDataList {
            public AISideSpinData[] spinList;

            public AISideSpinDataList (int count) {
                spinList = new AISideSpinData[count];
            }
        }

        public void Start () {
            PhysicsDataManager physics = new PhysicsDataManager ();
            PhysicsManager.Instance.EnterGame ();
            PhysicsManager.Instance.ballController.ballBounciness = 1;
            // int distance = 340;
            for (int dis = 10; dis <= 350; dis = dis + 5) {
                List<AISideSpinData> spinList = new List<AISideSpinData> ();
                Vector2 shotPt = Vector2.zero;
                Vector2 dropPt = new Vector2 (-0, -dis);
                for (float i = -10f; i < 10f + 0.1; i = i + 0.1f) {                    
                    SPhysics.AiLine guideLine = physics.Line (shotPt, dropPt, 0, 4, 0.26f, i);
                    if (guideLine.bounceList.Count > 2) {
                        AISideSpinData spin = new AISideSpinData ();

                        Vector2 dropPt1 = new Vector2 (guideLine.bounceList[0].x, guideLine.bounceList[0].z);
                        Vector2 dropPt2 = new Vector2 (guideLine.bounceList[1].x, guideLine.bounceList[1].z);

                        Vector2 firstLine = dropPt1 - shotPt;
                        Vector2 secondLine = dropPt2 - dropPt1;

                        float angle = AIMathf.AngelWithSides (firstLine, secondLine);
                        spin.shotPt[0] = shotPt.x;
                        spin.shotPt[1] = shotPt.y;

                        spin.dropPt1[0] = dropPt1.x;
                        spin.dropPt1[1] = dropPt1.y;

                        spin.dropPt2[0] = dropPt2.x;
                        spin.dropPt2[1] = dropPt2.y;

                        spin.targetPt[0] = dropPt.x;
                        spin.targetPt[1] = dropPt.y;

                        spin.distance = Vector2.Distance (shotPt, dropPt);
                        spin.sideSpin = i;
                        spin.angle = angle;
                        spinList.Add (spin);
                    }
                }

                AISideSpinDataList list = new AISideSpinDataList (spinList.Count);
                list.spinList = spinList.ToArray ();

                string json = JsonUtility.ToJson (list, true);
                string filePath = Application.persistentDataPath + "//sidespindata26//sideSpinData_" + dis + ".json";
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