#if AI_TEST
using System.Collections;
using UnityEngine;
using Assets.Scripts.ai;
//namespace Assets.Scripts.ai
//{
    public class AIEnter : MonoBehaviour
    {
        private static AIEnter Instance;

        public AIStrategy Strategy;
        public System.Collections.Generic.List<PhysicMaterial> Mats;

        void Awake()
        {
            Instance = this;
        }
        
        void Start()
        {
            PhysicsManager.Instance.EnterGame();
            Strategy = new AIStrategy();
            AIPhysicsDataManager aiPhysics = new AIPhysicsDataManager(Strategy, Mats);
            PhysicsDataManager physics = new PhysicsDataManager();
            physics.Init();
            Strategy.Init(physics, aiPhysics, gameObject.name);
            PhysicsManager.Instance.physicsDataManager = aiPhysics;
            PhysicsManager.Instance.ai = Strategy;
            UnityEngine.Debug.Log("AIEnter Start");
        }

    }
//}
#endif