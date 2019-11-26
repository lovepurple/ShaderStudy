#if AI_TEST
using UnityEngine;

namespace Assets.Scripts.ai
{
    /// <summary>
    /// 每杆击球的详细决策数据
    /// </summary>
    public class AIAction
    {
        public float SideSpin;

        public float TopBackSpin;

        public float Power;

        public float ClubEmitCoefficient;

        public float Deviation;

        public double ClubAccuracy;

        public double PlayCurl;

        public int ClubLevel;

        public Vector2 TargetPosition;

        public Vector2 WindOffsetDropPt;

        public Vector2 PredictArrivePosition;

        public int ClubId;

        public int ClubType;

        public bool IsChipping;

        public Vector2 TargetPositionAfterMoving;

        public float SideSpinAfterMoving;

        public float MinDisFinal;

        public float MinDisDropPt;

        public float MinDisFrontDropPt;
    }
}

#endif