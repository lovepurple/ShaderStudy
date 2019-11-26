#if AI_TEST
using UnityEngine;
using System.Collections.Generic;
using SPhysics;
using UnityEngine.Assertions;

public class AITest : MonoBehaviour
{
    //public AIStrategy strategy;
    public System.Collections.Generic.List<PhysicMaterial> mats;
    private SPhysics.TestTrajectory[] testShot = new SPhysics.TestTrajectory[2];
    private GameObject obj;
    private int i;
    private float ballBounciness = 1;
    public static Vector3 startPosition;
    public static bool again = false;
    private Vector3[] tar = new Vector3 [1];
    private Vector3 t;

    void Start()
    {
        // 判断是否开启AI_SHOT宏
        bool isInAiShot = false;

#if AI_SHOT
        isInAiShot = true;
#endif
		float mmm = PhysicsParameterCaculate.Instance.sideLength (5, 200, false);
        Assert.IsTrue(isInAiShot, "模拟打球必须开启AI_SHOT宏");

        //Time.timeScale
//			Debug.LogError(SceneLoadManager.GetCurrentSceneName());
//			GameObject terrain;
//			terrain = GameObject.Find ("botany001");
//			CapsuleCollider[] aaa =  GetComponentsInChildren<CapsuleCollider> ();
////			Vector3 mm = aaa [0].transform.position;
////			Vector3 mm1 = aaa [0].bounds.center;
////			Quaternion nn = aaa [0].transform.rotation;
        SPhysics.SceneCollider la = PhysicsManager.Instance.physicsDataManager.getCollider(gameObject.name);
        //PhysicsManager.Instance.ballController.ballBounciness = 10;
		PhysicsManager.Instance.video = false;
        again = false;
        i = 0;
        obj =  (GameObject)GameObject.Instantiate( Resources.Load ("External/Prefabs/Ball"));
        if (obj != null)
        {
            PhysicsManager.Instance.ballController = obj.GetComponent<BallController>();
        }

        PhysicsManager.Instance.EnterGame();
        startPosition = GameObject.Find("GameObject").transform.position;
		bool lll = PhysicsManager.Instance.physicsDataManager.GetTop (startPosition);
        Vector3 a = PhysicsManager.Instance.physicsDataManager.GetNormal(startPosition).normalized *
                    BallController.radius;
        Vector3 b = new Vector3(startPosition.x, PhysicsManager.Instance.physicsDataManager.GetMapHeight(startPosition),
            startPosition.z);
        startPosition = a + b;
		if (PhysicsManager.Instance.physicsDataManager.getGroundType (startPosition.x, startPosition.z) == 8) {
			startPosition.y += 0.25f;
		}
        PhysicsManager.Instance.ballController.transform.position = startPosition;

        //startPosition = PhysicsManager.Instance.ballController.transform.position;
        for (int j = 0; j < tar.Length; j++)
        {
            string bb = "GameObject" + j.ToString();
            GameObject aa = GameObject.Find(bb);
            if (aa != null)
            {
                tar[j] = aa.transform.position;
            }
        }

        SetPara();
//        PhysicsManager.Instance.physicsDataManager.NormalShot(testShot[i].sideSpin, testShot[i].topSpin,
//            testShot[i].angle, testShot[i].target, testShot[i].power,
//            ballBounciness); // (0,0, 0, 100, 0, new Vector2 (-25f, -280f), 1);
		Trajectory tra = new Trajectory();
		List<Vector3> list11 = tra.CaculateLineGround (startPosition, 1, 0);
		foreach (Vector3 k in list11) {
			GameObject ss1 =
				Instantiate(obj, k, Quaternion.identity) as
				GameObject;
		}
		PhysicsManager.Instance.GreenShot(1,0);

    }

    void Update()
    {
        PhysicsManager.Instance.ballController.ballStateMachine.Update(0.06f);
        if (PhysicsManager.Instance.ballController.ballStateMachine.currentStateType == FSM.StateType.BALL_STATE_STOP)
        {
            i++;
            again = true;
            if (i < testShot.Length)
            {
                PhysicsManager.Instance.ballController.Init();
                PhysicsManager.Instance.ballController.transform.position = startPosition;
                PhysicsManager.Instance.physicsDataManager.NormalShot(testShot[i].sideSpin, testShot[i].topSpin,
                    testShot[i].angle, testShot[i].target, testShot[i].power,
                    ballBounciness); // (0,0, 0, 100, 0, new Vector2 (-25f, -280f), 1);
                PhysicsManager.Instance.ballController.firstCollision = true;
            }
        }

        if (again == true)
        {
            PhysicsManager.Instance.ballController.transform.position = startPosition;
            again = false;
        }

        GameObject s1 =
            Instantiate(obj, PhysicsManager.Instance.ballController.transform.position, Quaternion.identity) as
                GameObject;
        //Debug.Log (PhysicsManager.Instance.ballController.ballStateMachine.currentStateType.ToString ());
    }

    void FixedUpdate()
    {
        PhysicsManager.Instance.ballController.ballStateMachine.FixedUpdate();
        if (PhysicsManager.Instance.ballController.ballStateMachine.currentStateType == FSM.StateType.BALL_STATE_STOP)
        {
            i++;
            if (i < testShot.Length)
            {
                PhysicsManager.Instance.ballController.Init();
                PhysicsManager.Instance.ballController.transform.position = startPosition;
                PhysicsManager.Instance.physicsDataManager.NormalShot(testShot[i].sideSpin, testShot[i].topSpin,
                    testShot[i].angle, testShot[i].target, testShot[i].power,
                    ballBounciness); // (0,0, 0, 100, 0, new Vector2 (-25f, -280f), 1);
                PhysicsManager.Instance.ballController.firstCollision = true;
            }
        }

        if (again == true)
        {
            PhysicsManager.Instance.ballController.transform.position = startPosition;
            again = false;
        }

        //GameObject s2 = Instantiate (obj, PhysicsManager.Instance.ballController.transform.position, Quaternion.identity)as GameObject;
        //Debug.Log (PhysicsManager.Instance.ballController.ballStateMachine.currentStateType.ToString ());
    }

    private void SetPara()
    {
		PhysicsManager.Instance.ballController.debuffPara = 1f;//tour2的场是0.8，tour3以上都是1
		PhysicsManager.Instance.ballController.useDebuff = false;//想要落点一定要在gameobject时候是false，想要看实际击球情况时设置为true


		ballBounciness = 1f;
        testShot[0].angle = 0.26f;
		testShot[0].sideSpin = 0f;
        testShot[0].topSpin = 0f;
        testShot[0].target = new Vector2(tar[0].x, tar[0].z);
        testShot[0].power = 1f;
        testShot[0].windLevel = 0;
        testShot[0].windDirection = Vector2.right;

		testShot[1].angle = 0.26f;
		testShot[1].sideSpin = 0f;
		testShot[1].topSpin = -0f;
		testShot[1].target = new Vector2(tar[0].x, tar[0].z);
		testShot[1].power = 1f;
		testShot[1].windLevel = 0;
		testShot[1].windDirection = Vector2.right;


//		testShot[1].angle = 0.56f;
//		testShot[1].sideSpin = 0f;
//		testShot[1].topSpin =  -6.5f;
//		testShot[1].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[1].power = 1.0f;
//		testShot[1].windLevel = 0;
//		testShot[1].windDirection = Vector2.right;
//////
//		testShot[2].angle = 0.56f;
//		testShot[2].sideSpin = 0f;
//		testShot[2].topSpin =  -7.4f;
//		testShot[2].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[2].power = 1.1f;
//		testShot[2].windLevel = 0;
//		testShot[2].windDirection = Vector2.right;


//		testShot[0].angle = 0.26f;
//		testShot[0].sideSpin = -0f;
//		testShot[0].topSpin = -3.7f;
//		testShot[0].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[0].power = 1.0f;
//		testShot[0].windLevel = 0;
//		testShot[0].windDirection = Vector2.right;
//
//		testShot[1].angle = 0.26f;
//		testShot[1].sideSpin = -0f;
//		testShot[1].topSpin = -4.6f;
//		testShot[1].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[1].power = 1.0f;
//		testShot[1].windLevel = 0;
//		testShot[1].windDirection = Vector2.right;
//
//		testShot[3].angle = 0.7f;
//		testShot[3].sideSpin = 0f;
//		testShot[3].topSpin =  -3f;
//		testShot[3].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[3].power = 1.1f;
//		testShot[3].windLevel = 0;
//		testShot[3].windDirection = Vector2.right;
//
//		testShot[4].angle = 0.56f;
//		testShot[4].sideSpin = 0f;
//		testShot[4].topSpin =  -8f;
//		testShot[4].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[4].power = 1f;
//		testShot[4].windLevel = 0;
//		testShot[4].windDirection = Vector2.right;
//
//		testShot[5].angle = 0.56f;
//		testShot[5].sideSpin = 0f;
//		testShot[5].topSpin =  -10f;
//		testShot[5].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[5].power = 1f;
//		testShot[5].windLevel = 0;
//		testShot[5].windDirection = Vector2.right;

//		testShot[0].angle = 0.7f;
//		testShot[0].sideSpin = 0f;
//		testShot[0].topSpin =  -3f;
//		testShot[0].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[0].power = 1.0f;
//		testShot[0].windLevel = 0;
//		testShot[0].windDirection = Vector2.right;
//
//		testShot[1].angle = 0.7f;
//		testShot[1].sideSpin = 0f;
//		testShot[1].topSpin =  -4f;
//		testShot[1].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[1].power = 1.0f;
//		testShot[1].windLevel = 0;
//		testShot[1].windDirection = Vector2.right;

//		testShot[2].angle = 0.7f;
//		testShot[2].sideSpin = 0f;
//		testShot[2].topSpin =  -3f;
//		testShot[2].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[2].power = 1.1f;
//		testShot[2].windLevel = 0;
//		testShot[2].windDirection = Vector2.right;
//
//		testShot[3].angle = 0.7f;
//		testShot[3].sideSpin = 0f;
//		testShot[3].topSpin =  -4f;
//		testShot[3].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[3].power = 1.1f;
//		testShot[3].windLevel = 0;
//		testShot[3].windDirection = Vector2.right;

//		testShot[2].angle = 0.7f;
//		testShot[2].sideSpin = -3f;
//		testShot[2].topSpin =  0f;
//		testShot[2].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[2].power = 1.1f;
//		testShot[2].windLevel = 0;
//		testShot[2].windDirection = Vector2.right;
//
//		testShot[3].angle = 0.7f;
//		testShot[3].sideSpin = -7.5f;
//		testShot[3].topSpin =  0f;
//		testShot[3].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[3].power = 1.1f;
//		testShot[3].windLevel = 0;
//		testShot[3].windDirection = Vector2.right;
//
//		testShot[2].angle = 0.7f;
//		testShot[2].sideSpin = 3f;
//		testShot[2].topSpin =  0f;
//		testShot[2].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[2].power = 1.1f;
//		testShot[2].windLevel = 0;
//		testShot[2].windDirection = Vector2.right;
//
//		testShot[3].angle = 0.7f;
//		testShot[3].sideSpin = 7.5f;
//		testShot[3].topSpin =  0f;
//		testShot[3].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[3].power = 1.1f;
//		testShot[3].windLevel = 0;
//		testShot[3].windDirection = Vector2.right;
//
//		testShot[4].angle = 0.7f;
//		testShot[4].sideSpin = 5f;
//		testShot[4].topSpin = 0f;
//		testShot[4].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[4].power = 1f;
//		testShot[4].windLevel = 0;
//		testShot[4].windDirection = Vector2.right;
//
//		testShot[5].angle = 0.7f;
//		testShot[5].sideSpin = 1f;
//		testShot[5].topSpin =  0f;
//		testShot[5].target = new Vector2(tar[0].x, tar[0].z);
//		testShot[5].power = 1f;
//		testShot[5].windLevel = 0;
//		testShot[5].windDirection = Vector2.right;

//		testShot[1].angle = 0.34f;
//		testShot[1].sideSpin = 0;
//		testShot[1].topSpin =  -5;
//		testShot[1].target = new Vector2 (tar[0].x, tar[0].z);
//		testShot[1].power = 1;
//		testShot[1].windLevel = 0;
//		testShot[1].windDirection = Vector2.right;
//
//		testShot[2].angle = 0.26f;
//		testShot[2].sideSpin = 0;
//		testShot[2].topSpin =  6;
//		testShot[2].target = new Vector2 (tar[0].x, tar[0].z);
//		testShot[2].power = 1088;
//		testShot[2].windLevel = 0;
//		testShot[2].windDirection = Vector2.right;

//		testShot[2].angle = 0.34f;
//		testShot[2].sideSpin = 0;
//		testShot[2].topSpin =  -8x`;
//		testShot[2].target = new Vector2 (tar[0].x, tar[0].z);
//		testShot[2].power = 1;
//		testShot[2].windLevel = 0;
//		testShot[2].windDirection = Vector2.right;
//
//		testShot[3].angle = 0.34f;
//		testShot[3].sideSpin = 0;
//		testShot[3].topSpin =  6;
//		testShot[3].target = new Vector2 (tar[0].x, tar[0].z);
//		testShot[3].power = 1;
//		testShot[3].windLevel = 0;
//		testShot[3].windDirection = Vector2.right;
//
//		testShot[4].angle = 0.34f;
//		testShot[4].sideSpin = 0;
//		testShot[4].topSpin =  8;
//		testShot[4].target = new Vector2 (tar[0].x, tar[0].z);
//		testShot[4].power = 1;
//		testShot[4].windLevel = 0;
//		testShot[4].windDirection = Vector2.right;
//
//		testShot[5].angle = 0.34f;
//		testShot[5].sideSpin = 0;
//		testShot[5].topSpin =  10;
//		testShot[5].target = new Vector2 (tar[0].x, tar[0].z);
//		testShot[5].power = 1;
//		testShot[5].windLevel = 0;
//		testShot[5].windDirection = Vector2.right;


//		testShot[1].angle = 0.34f;
//		testShot[1].sideSpin = 0;
//		testShot[1].topSpin =  10;
//		testShot[1].target = new Vector2 (tar[0].x, tar[0].z);
//		testShot[1].power = 1;
//		testShot[1].windLevel = 0;
//		testShot[1].windDirection = Vector2.right;

//		testShot[2].angle = 0.26f;
//		testShot[2].sideSpin = 0;
//		testShot[2].topSpin = 0;
//		testShot[2].target = new Vector2 (tar[1].x, tar[1].z);
//		testShot[2].power = 1;
//		testShot[2].windLevel = 0;
//		testShot[2].windDirection = Vector2.right;
//
//		testShot[3].angle = 0.26f;
//		testShot[3].sideSpin = 0;
//		testShot [3].topSpin = -10;
//		testShot[3].target = new Vector2 (tar[1].x, tar[1].z);
//		testShot[3].power = 1;
//		testShot[3].windLevel = 0;
//		testShot[3].windDirection = Vector2.right;

//		testShot[0].angle = 0.34f;
//		testShot[0].sideSpin = 0;
//		testShot[0].topSpin =  0;
//		testShot[0].target = new Vector2 (0, -200f);
//		testShot[0].power = 1;
//		testShot[0].windLevel = 0;
//		testShot[0].windDirection = Vector2.right;
//
//		testShot[1].angle = 0.34f;
//		testShot[1].sideSpin = 0;
//		testShot [1].topSpin = 2;
//		testShot[1].target = new Vector2 (0, -200f);
//		testShot[1].power = 1;
//		testShot[1].windLevel = 0;
//		testShot[1].windDirection = Vector2.right;
//
//		testShot[2].angle = 0.34f;
//		testShot[2].sideSpin = 0;
//		testShot [2].topSpin = 4;
//		testShot[2].target = new Vector2 (0, -200f);
//		testShot[2].power = 1;
//		testShot[2].windLevel = 0;
//		testShot[2].windDirection = Vector2.right;
//
//		testShot[3].angle = 0.34f;
//		testShot[3].sideSpin = 0;
//		testShot [3].topSpin = 6;
//		testShot[3].target = new Vector2 (0, -200f);
//		testShot[3].power = 1;
//		testShot[3].windLevel = 0;
//		testShot[3].windDirection = Vector2.right;
//
//		testShot[4].angle = 0.34f;
//		testShot[4].sideSpin = 0;
//		testShot [4].topSpin = 8;
//		testShot[4].target = new Vector2 (0, -200f);
//		testShot[4].power = 1;
//		testShot[4].windLevel = 0;
//		testShot[4].windDirection = Vector2.right;
//
//		testShot[5].angle = 0.34f;
//		testShot[5].sideSpin = 0;
//		testShot [5].topSpin = 10;
//		testShot[5].target = new Vector2 (0, -200f);
//		testShot[5].power = 1;
//		testShot[5].windLevel = 0;
//		testShot[5].windDirection = Vector2.right;

//		testShot[0].angle = 0.26f;
//		testShot[0].sideSpin = 0;
//		testShot[0].topSpin =  -10;
//		testShot[0].target = new Vector2 (0, -290f);
//		testShot[0].power = 1;
//		testShot[0].windLevel = 0;
//		testShot[0].windDirection = Vector2.right;

//		testShot[0].angle = 0.7f;
//		testShot[0].sideSpin = 0;
//		testShot[0].topSpin =  -10;
//		testShot[0].target = new Vector2 (0, -80f);
//		testShot[0].power = 1;
//		testShot[0].windLevel = 0;
//		testShot[0].windDirection = Vector2.right;
//
////		testShot [0].sideSpin += windSideSpin (testShot [0].windLevel * testShot [0].windDirection, new Vector2 (startPosition.x,startPosition.z), testShot [0].target, testShot [0].angle);
////		testShot[0].target += testShot [0].windLevel * testShot [0].windDirection / 100 * (new Vector2 (startPosition.x,startPosition.z) - testShot[0].target).magnitude;
//
//		testShot[1].angle = 0.56f;
//		testShot[1].sideSpin = 0;
//		testShot[1].topSpin =  -10;
//		testShot[1].target = new Vector2 (0, -120f);
//		testShot[1].power = 1;
//		testShot[1].windLevel = 0;
//		testShot[1].windDirection = Vector2.right;
//
//		testShot[2].angle = 0.46f;
//		testShot[2].sideSpin = 0;
//		testShot[2].topSpin =  -10;
//		testShot[2].target = new Vector2 (0, -140f);
//		testShot[2].power = 1;
//		testShot[2].windLevel = 0;
//		testShot[2].windDirection = Vector2.right;
//
//		testShot[3].angle = 0.34f;
//		testShot[3].sideSpin = 0;
//		testShot[3].topSpin =  -10;
//		testShot[3].target = new Vector2 (0, -200f);
//		testShot[3].power = 1;
//		testShot[3].windLevel = 0;
//		testShot[3].windDirection = Vector2.right;
//
//		testShot[4].angle = 0.26f;
//		testShot[4].sideSpin = 0;
//		testShot[4].topSpin =  -10;
//		testShot[4].target = new Vector2 (0, -220f);
//		testShot[4].power = 1;
//		testShot[4].windLevel = 0;
//		testShot[4].windDirection = Vector2.right;
//
//		testShot[5].angle = 0.26f;
//		testShot[5].sideSpin = 0;
//		testShot[5].topSpin =  0;
//		testShot[5].target = new Vector2 (0, -220f);
//		testShot[5].power = 1;
//		testShot[5].windLevel = 0;
//		testShot[5].windDirection = Vector2.right;
//
//		testShot[6].angle = 0.26f;
//		testShot[6].sideSpin = 0;
//		testShot[6].topSpin =  -10;
//		testShot[6].target = new Vector2 (0, -380f);
//		testShot[6].power = 1;
//		testShot[6].windLevel = 0;
//		testShot[6].windDirection = Vector2.right;
//
//		testShot[7].angle = 0.26f;
//		testShot[7].sideSpin = 0;
//		testShot[7].topSpin =  0;
//		testShot[7].target = new Vector2 (0, -380f);
//		testShot[7].power = 1;
//		testShot[7].windLevel = 0;
//		testShot[7].windDirection = Vector2.right;


//				testShot[1].angle = 0.3f;
//				testShot[1].sideSpin = 0f;
//				testShot[1].topSpin = -10;
//				testShot[1].target = new Vector2 (0, -220f);
//				testShot[1].power = 1;
//				testShot[1].windLevel = 0;
//				testShot[1].windDirection = Vector2.right;
//
//		testShot[2].angle = 0.3f;
//		testShot[2].sideSpin = 0;
//		testShot[2].topSpin =  10;
//		testShot[2].target = new Vector2 (0, -220f);
//		testShot[2].power = 1;
//		testShot[2].windLevel = 0;
//		testShot[2].windDirection = Vector2.right;
//
//		testShot[3].angle = 0.3f;
//		testShot[3].sideSpin = -10f;
//		testShot[3].topSpin = 0;
//		testShot[3].target = new Vector2 (0, -220f);
//		testShot[3].power = 1;
//		testShot[3].windLevel = 0;
//		testShot[3].windDirection = Vector2.right;
//
//		testShot[4].angle = 0.3f;
//		testShot[4].sideSpin = -10;
//		testShot[4].topSpin =  -10;
//		testShot[4].target = new Vector2 (0, -220f);
//		testShot[4].power = 1;
//		testShot[4].windLevel = 0;
//		testShot[4].windDirection = Vector2.right;
//
//		testShot[5].angle = 0.3f;
//		testShot[5].sideSpin = -10f;
//		testShot[5].topSpin = 10;
//		testShot[5].target = new Vector2 (0, -220f);
//		testShot[5].power = 1;
//		testShot[5].windLevel = 0;
//		testShot[5].windDirection = Vector2.right;
//
//		testShot[6].angle = 0.3f;
//		testShot[6].sideSpin = 10;
//		testShot[6].topSpin =  10;
//		testShot[6].target = new Vector2 (0, -220f);
//		testShot[6].power = 1;
//		testShot[6].windLevel = 0;
//		testShot[6].windDirection = Vector2.right;
//		testShot[1].angle = 0.3f;
//		testShot[1].sideSpin = 10f;
//		testShot[1].topSpin = 0;
//		testShot[1].target = new Vector2 (0, -220f);
//		testShot[1].power = 1;
//		testShot[1].windLevel = 0;
//		testShot[1].windDirection = Vector2.right;
//
//		testShot[2].angle = 0.3f;
//		testShot[2].sideSpin = -10f;
//		testShot[2].topSpin = 0;
//		testShot[2].target = new Vector2 (0, -220f);
//		testShot[2].power = 1;
//		testShot[2].windLevel = 0;
//		testShot[2].windDirection = Vector2.right;
//
//		testShot[3].angle = 0.64f;
//		testShot[3].sideSpin = 0f;
//		testShot[3].topSpin = 0;
//		testShot[3].target = new Vector2 (0, -100f);
//		testShot[3].power = 1;
//		testShot[3].windLevel = 0;
//		testShot[3].windDirection = Vector2.right;
//			testShot[1].angle = 0.34f;
//			testShot[1].sideSpin = 0f;
//			testShot[1].topSpin = 0;
//			testShot[1].target = new Vector2 (0, -200f);
//			testShot[1].power = 1;
//			testShot[1].windLevel = 0;
//			testShot[1].windDirection = Vector2.right;
//
//		testShot[2].angle = 0.46f;
//		testShot[2].sideSpin = 0f;
//		testShot[2].topSpin = 0;
//		testShot[2].target = new Vector2 (0, -140f);
//		testShot[2].power = 1;
//		testShot[2].windLevel = 0;
//		testShot[2].windDirection = Vector2.right;
//
//		testShot[3].angle = 0.56f;
//		testShot[3].sideSpin = 0f;
//		testShot[3].topSpin = 0;
//		testShot[3].target = new Vector2 (0, -120f);
//		testShot[3].power = 1;
//		testShot[3].windLevel = 0;
//		testShot[3].windDirection = Vector2.right;
//
//		testShot[4].angle = 0.7f;
//		testShot[4].sideSpin = 0f;
//		testShot[4].topSpin = 0;
//		testShot[4].target = new Vector2 (0, -80f);
//		testShot[4].power = 1;
//		testShot[4].windLevel = 0;
//		testShot[4].windDirection = Vector2.right;
    }

    private float windSideSpin(Vector2 wind, Vector2 startp, Vector2 targetp, float angle)
    {
        Vector2 v1 = startp - targetp;
        float l = v1.magnitude;
        Vector2 newtargetP = targetp + wind / 100 * l;
        float alpha = Mathf.PI / 2 * angle;
        float vx, vy;
        vx = 0;
        vy = 0;
        if (angle > 0)
        {
            vx = Mathf.Sqrt(l * PhysicsManager.g / 2 / Mathf.Tan(alpha));
            vy = Mathf.Sqrt(l * PhysicsManager.g / 2 * Mathf.Tan(alpha));
        }

        float costheta, sintheta;

        Vector2 v2 = startp - newtargetP;
        costheta = Vector2.Dot(v1.normalized, v2.normalized);
        sintheta = Mathf.Sqrt(1 - costheta * costheta);

        if (vx < 0.00001f)
            return 0;
        float sideAngle = Mathf.Atan(vx * sintheta / vy);
        if (v1.x * v2.y - v1.y * v2.x > 0)
        {
            sideAngle *= -1;
        }

        return (sideAngle * Mathf.Rad2Deg / 4.5f);
    }
}


#endif