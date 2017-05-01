using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Game.Actor
{
    [ExecuteInEditMode]
    public class ActorWillOnRenderComponent : MonoBehaviour
    {
        public GameObject TargetObj;

        void Update()
        {
            if (TargetObj != null)
            {
                Debug.Log(IsInCameraView(TargetObj, Camera.main));
            }
        }
        /// <summary>
        /// 对象是否在Camera内
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool IsInCameraView(GameObject targetObj, Camera camera = null)
        {
            //if (camera == null)
            //    camera = GOERoot.GOECamera.Camera;

            Vector3 targetObjViewportCoord = camera.WorldToViewportPoint(targetObj.transform.position);
            if (targetObjViewportCoord.x > 0 && targetObjViewportCoord.x < 1 && targetObjViewportCoord.y > 0 && targetObjViewportCoord.y < 1 && targetObjViewportCoord.z > camera.nearClipPlane && targetObjViewportCoord.z < camera.farClipPlane)
                return true;

            return false;
        }

    }
}
