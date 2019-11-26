#if AI_GUIDE_LINE_WITH_SPIN
using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Assets.Scripts.ai
{
    public class AISideSpinDiffTest : MonoBehaviour {
        [System.Serializable]
        public class AISideSpinDiffData {

            public float[] shotPt;

            public float[] targetPt;

            public float[] dropPt;

            public float sideSpin;

            public float peakDiff;

            public AISideSpinDiffData () {
                shotPt = new float[2];
                dropPt = new float[2];
                targetPt = new float[2];
            }
        }

        [System.Serializable]
        public class AISideSpinDiffList {
            public AISideSpinDiffData[] spinList;

            public AISideSpinDiffList (int count) {
                spinList = new AISideSpinDiffData[count];
            }
        }

        public class AISideSpinDiffComparer : IComparer<Vector3>
        {
            public int Compare(Vector3 vecA, Vector3 vecB)
            {
                return Mathf.Abs(vecA.x).CompareTo(Mathf.Abs(vecB.x));
            }
        }
        
        public void Start () {
            PhysicsDataManager physics = new PhysicsDataManager ();
            PhysicsManager.Instance.EnterGame ();
            PhysicsManager.Instance.ballController.ballBounciness = 1;
            List<AISideSpinDiffData> spinList = new List<AISideSpinDiffData> ();
                Vector2 shotPt = Vector2.zero;
                Vector2 dropPt = new Vector2 (-0, -200);
                for (float i = 0.1f; i < 10f + 0.1; i = i + 0.1f) {
                    SPhysics.AiLine guideLine = physics.Line (shotPt, dropPt, 0, 4, 0.34f, i);                    
                    AISideSpinDiffData spin = new AISideSpinDiffData ();
                    Vector2 dropPt1 = new Vector2 (guideLine.bounceList[0].x, guideLine.bounceList[0].z);
                        
                    spin.shotPt[0] = shotPt.x;
                    spin.shotPt[1] = shotPt.y;

                    spin.dropPt[0] = dropPt1.x;
                    spin.dropPt[1] = dropPt1.y;

                    spin.targetPt[0] = dropPt.x;
                    spin.targetPt[1] = dropPt.y;
                    spin.sideSpin = i;

                    List<Vector3> posList = guideLine.positionList;
                    posList.Sort(new AISideSpinDiffComparer());
                    spin.peakDiff = posList[posList.Count - 1].x;
                    spinList.Add (spin);          
                }

                AISideSpinDiffList list = new AISideSpinDiffList (spinList.Count);
                list.spinList = spinList.ToArray ();

                string json = JsonUtility.ToJson (list, true);
                string filePath =
 Application.persistentDataPath + "//experimentalData//sidespinDiff//sidespinDiff34.json";
                FileInfo fileInfo = new FileInfo (filePath);
                if (fileInfo.Exists) {
                    File.Delete (filePath);
                }

                File.WriteAllText (filePath, json, Encoding.UTF8);
        }   
    }
}
#endif