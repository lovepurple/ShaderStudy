#if AI_TEST

using UnityEngine;

namespace Assets.Scripts.ai
{
    public interface IStrategy
    {
        
        int GetGroundTypeAtPoint(Vector2 pos);
        
        float GetHeightAtPoint(Vector2 pos);

        void Init(PhysicsDataManager physics, AIPhysicsDataManager aiPhysics, string sceneName);
    }
}

#endif