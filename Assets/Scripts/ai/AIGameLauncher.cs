#if AI_TEST
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGameLauncher : MonoBehaviour
{

    public static AIGameLauncher Instance = null;
    /**
     * 游戏状态机的状态配置表
     */
    public List<FSM.StateBaseConfigure> GameStates;
    /**
     * 相机状态机的状态配置表
     */
    public List<FSM.StateBaseConfigure> CameraStates;
    /**
     * 球状态机的状态配置表
     */
    public List<FSM.StateBaseConfigure> BallStates;
    //#if IN_DEVELOPE 
    public FSM.StateType CurrentGameStateType;
    public FSM.StateType CurrentCameraStateType;
    //#endif

    // Use this for initialization
    void Start()
    {
        if (Instance != null)
        {
            return;
        }
        GameObject.DontDestroyOnLoad(gameObject);
        Instance = this;
//        GameManager.Instance.Init(GameStates, CameraStates);

       // FBHelper.Init();
        // ResourcesUpdateManager.Instance.check(this.updateCeheckOverCallback);
    }

    // Update is called once per frame
    void Update()
    {
//        GameManager.Instance.Update();
#if IN_DEVELOPE
        CurrentGameStateType = GameManager.Instance.stateMachine.currentStateType;
        CurrentCameraStateType = GameManager.Instance.cameraStateMachine.currentStateType;
#endif
    }

    void FixedUpdate()
    {
//        GameManager.Instance.FixedUpdate();
    }

    /// <summary>
    /// 程序退出
    /// </summary>
    void OnDestroy()
    {
        Debug.Log("on game destroy");
//        PlayerIOManager.Instance.Destroy();
    }

    
}

#endif