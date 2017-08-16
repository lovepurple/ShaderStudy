using UnityEngine;
using System.Collections;
using System;

// @author : xulin //
// @usage : add this to camera & drag a target to this, you can get an easy camera control //

public class FollowTargetEditor : MonoBehaviour {

    private GameObject target;
    private int curTargetIndex = 0;
	public GameObject[] targets;

	private const float fMaxDT=1.0f / 33;
	
	public bool RotateMoreFree = true;
	
	public float mDistance = 6f;
	public float mTargetDistance = 3f;
	
	public float mDegree = 0f;
	public float mTargetDegree = 0f;
	
	public float mPitch = 0f;
	public float mTargetPitch = -30f;
	
	public float MAXDISTANCE = 6f;
	public float MINDISTANCE = 1f;
	
	public float FOCUSHEIGHT = 1f;
	public float MINHEIGHT = 0f;
	
	public float SmoothTime = 4f;
	
	public float dt = 0;

    private Vector2 oldPosition;
    private Vector2 oldPosition1;
    private Vector2 oldPosition2;

    private float distance = 5;
    private bool flag = false;

    private void OnGUI()
    {

        float rowHeight = Screen.height / 12, colWidth = Screen.width / 10;
        GUI.color = Color.red;
        GUI.Label(new Rect(Screen.width / 2, 0, colWidth, rowHeight), @"左键点击移动，右键按住旋转");

        //int row = 0;
        //if( GUI.Button(new Rect(0, rowHeight * row, colWidth, rowHeight), "AutoRotate"))
        //{
        //    if( target != null )
        //    {
        //        SelfRotate sr = target.GetComponent<SelfRotate>();
        //        if (sr == null)
        //        {
        //            sr = target.AddComponent<SelfRotate>();
        //        }
        //        else
        //        {
        //            sr.enabled = !sr.enabled;
        //        }
        //    }
        //}

        //++row;
        //if (GUI.Button(new Rect(0, rowHeight * row, colWidth, rowHeight), "NextTarget"))
        //{
        //    if (targets.Length > 0)
        //    {
        //        curTargetIndex = (++curTargetIndex) % targets.Length;
        //    }
        //    if (targets.Length > curTargetIndex && targets[curTargetIndex] != null)
        //    {
        //        target = targets[curTargetIndex];
        //    }
        //}

        //++row;
        //if (GUI.Button(new Rect(0, rowHeight * row, colWidth, rowHeight), "View Lightmap"))
        //{
        //    Renderer[] renders = UnityEngine.Object.FindObjectsOfType<Renderer>();
        //    for (int i = 0; i < renders.Length; i++)
        //    {
        //        Renderer render = renders[i];
        //        Material[] materials = render.materials;
        //        if (materials == null || materials.Length == 0) continue;

        //        foreach (Material mat in materials)
        //        {
        //            if (mat == null || mat.name.Contains("fSQ01")) continue;
        //            mat.mainTexture = null;
        //        }
        //    }
        //}

        
    }

    // Use this for initialization
    void Start () {
	    if( targets.Length > 0 )
        {
            target = targets[0];
        }
	}
	
	// Update is called once per frame
	void Update () {
	
		//Touch touch = Input.GetTouch (0);

		if ( target == null )
		{
			return;
		}
		
		dt = Time.deltaTime;
		if ( dt > fMaxDT ) dt = fMaxDT;
		
		DrawLine ();
		
		double phi = mPitch * 2 * Math.PI / 360;
		double xzDis = mDistance * Math.Cos( phi );
        double dy = mDistance * Math.Sin(phi);

        double theta = mDegree * 2 * Math.PI / 360;
		double dx = xzDis * Math.Sin ( theta );
		double dz = xzDis * Math.Cos ( theta );
		
		Vector3 trueTargetPos = GetTrueTarPos ();
		Vector3 pos = trueTargetPos - new Vector3 ( (float)dx, (float)dy, (float)dz );

		this.GetComponent<Camera>().transform.position = pos;

		this.GetComponent<Camera>().transform.LookAt ( trueTargetPos, Vector3.up );
		
		UpdateZoom ();
		
		UpdateRot ();
	}

	private Vector3 GetTrueTarPos ()
	{	
		Vector3 targetPos = target.transform.position;
		targetPos += new Vector3 ( 0, FOCUSHEIGHT, 0 );
		return targetPos;
	}
	
	private Vector3 mousePosRot = Vector3.zero;
	
	public float zoomSpeed = 1f;
	public float rotateSpeed = 0.2f;
	
	public float rotateSpeedX = 200f;
	public float rotateSpeedY = 100f;
	
	private void UpdateZoom ()
	{
		float axis = Input.GetAxis ( "Mouse ScrollWheel" );
        if (axis < 0)
        {
            mTargetDistance += zoomSpeed;
        }
        else if (axis > 0)
        {
            mTargetDistance -= zoomSpeed;
        }

        if (Input.touchCount > 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                Vector2 tempPosition1 = Input.GetTouch(0).position;
                Vector2 tempPosition2 = Input.GetTouch(1).position;

                if (IsEnlarge(oldPosition1, oldPosition2, tempPosition1, tempPosition2))
                {
                    if (distance < 10)
                    {
                        mTargetDistance += zoomSpeed;
                        UnityEngine.Debug.Log("zoom+");
                    }
                }
                else {
                    if (distance > 1)
                    {
                        mTargetDistance -= zoomSpeed;
                        UnityEngine.Debug.Log("zoom-");
                    }
                }

                oldPosition1 = tempPosition1;
                oldPosition2 = tempPosition2;
            }
        }
        
		mTargetDistance = Mathf.Clamp ( mTargetDistance, MINDISTANCE, MAXDISTANCE );
		
		float ddis = mDistance - mTargetDistance;
		mDistance -= ddis * dt * SmoothTime;
		
		mDistance = Mathf.Clamp ( mDistance, MINDISTANCE, MAXDISTANCE );

	}
	
	private void UpdateRot ()
	{
		if ( RotateMoreFree )
		{
			float h = 0;
			float v = 0;

			if( Input.GetMouseButton ( 1 ) )
			{
			 	h = Input.GetAxis ( "Mouse X" );
				v = Input.GetAxis ( "Mouse Y" );
			}

            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    oldPosition = Event.current.mousePosition;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    Vector2 tempPosition = Event.current.mousePosition;

                    float x = tempPosition.x - oldPosition.x;
                    float y = tempPosition.y - oldPosition.y;

                    UnityEngine.Debug.Log("x:" + x);
                    UnityEngine.Debug.Log("y:" + y);

                    h = x/25f;
                    v = -y/25f;
                    oldPosition = tempPosition;
                }
            }
			
			if (  h != 0 || v != 0 )
			{
				if ( h != 0 || v != 0 )
				{
					int i = 0;
					++i;
				}
				
				float degree = rotateSpeedX * h;
				degree *= dt;
				mTargetDegree += degree;
				
				float pitch = rotateSpeedY * v;
				pitch *= dt;
				mTargetPitch += pitch;
				
			}
			
			float dDegree = mDegree - mTargetDegree;
			mDegree -= dDegree * dt * SmoothTime;
			
			float dPitch = mPitch - mTargetPitch;
			mPitch -= dPitch * dt * SmoothTime;
			
		}
		
	}

    bool IsEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
    {
        //old distance  
        float oldDistance = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
        //new distance  
        float newDistance = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));

        if (oldDistance < newDistance)
        {
            //zoom+  
            return true;
        }
        else {
            //zoom-  
            return false;
        }
    }

    private void DrawLine ()
	{
		Vector3 camPos = this.GetComponent<Camera>().transform.position;
		Debug.DrawLine ( camPos, GetTrueTarPos (), Color.red );
	}
}
