#if AI_TEST

using UnityEngine;
namespace Assets.Scripts.ai
{
    public class AINavEnter : MonoBehaviour
    {
        private static AINavEnter Instance;

        public IStrategy Strategy;
        public System.Collections.Generic.List<PhysicMaterial> Mats;

        void Awake()
        {
            Instance = this;
        }
        
        void Start()
        {
            PhysicsManager.Instance.EnterGame();
            Strategy = new AINavStrategy();
            AIPhysicsDataManager aiPhysics = new AIPhysicsDataManager(Strategy, Mats);
            PhysicsDataManager physics = new PhysicsDataManager();
            physics.Init();
            Strategy.Init(physics, aiPhysics, gameObject.name);
            PhysicsManager.Instance.physicsDataManager = aiPhysics;
            PhysicsManager.Instance.ai = Strategy;
            UnityEngine.Debug.Log("AIEnter Start");
        }
    }
}

#endif