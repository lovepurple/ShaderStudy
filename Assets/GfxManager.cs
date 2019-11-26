using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GfxManager{
    private static GfxManager instance = null;
    public static GfxManager Instance{
        get {
            if (instance == null){
                instance = new GfxManager();
            }
            return instance;
        }
    }
    private Dictionary<int, GameObject> map;
	private string dirPath = "External/Prefabs/";
    private int count;
    
    private GfxManager(){
        map = new Dictionary<int, GameObject>();
        count = 0;
    }   

    public int PlayUIGfxByName(string gfxName, Vector3? position = null, Vector3? scale = null, bool autoDestroy = true, float destroyTime = 5, Transform parent = null){
        return PlayGfxByName(gfxName, position, scale, autoDestroy, destroyTime, true, parent);
    }

    public int Play3DGfxByName(string gfxName, Vector3? position = null, Vector3? scale = null, bool autoDestroy = true, float destroyTime = 5){
        return PlayGfxByName(gfxName, position, scale, autoDestroy, destroyTime, false);
    }

//    public int PlayGfxOnBall(string gfxName, Vector3? scale = null, bool autoDestroy = true, float destroyTime = 5){
//        if (scale == null){
//            scale = Vector3.one;
//        }
//        GameObject gfxObj = ResourcesManager.LoadAsset(dirPath + gfxName);
//        gfxObj.transform.SetParent(PhysicsManager.Instance.ballController.transform, false);
//        gfxObj.transform.localScale = (Vector3)scale;
//        gfxObj.transform.localPosition = Vector3.zero;
//        gfxObj.SetActive(true);
//
//        map[count] = gfxObj;
//        ParticleSystem[] particleSystems = gfxObj.GetComponentsInChildren<ParticleSystem>();
//        for(int i = 0;i < particleSystems.Length;i ++){
//            particleSystems[i].Play();
//        }
//        if (autoDestroy == true){
//            GameLauncher.instance.StartCoroutine(WaitForSecondAndDestroyGfx(count, destroyTime));
//        }
//        return count++;
//    }

    public int PlayGfxByName(string gfxName, Vector3? position = null, Vector3? scale = null, bool autoDestroy = true, float destroyTime = 5, bool onCanvas = false, Transform parent = null){
        if (position == null){
            position = Vector3.zero;
        }
        if (scale == null){
            scale = Vector3.one;
        }
		GameObject gfxObj = (GameObject)GameObject.Instantiate(Resources.Load(dirPath + gfxName));
        if (onCanvas == true){
			if (parent == null){
               // gfxObj.transform.SetParent(UIManager.UICanvasObj_2D.transform, false);
			}else
                gfxObj.transform.SetParent(parent, true); 
            gfxObj.transform.localScale = (Vector3)scale;
            gfxObj.transform.localPosition = (Vector3)position;
        }else{
            gfxObj.transform.localScale = (Vector3)scale;
            gfxObj.transform.position = (Vector3)position;
            // gfxObj.transform.LookAt(Camera.main.transform);
        }
        map[count] = gfxObj;
        ParticleSystem[] particleSystems = gfxObj.GetComponentsInChildren<ParticleSystem>();
        for(int i = 0;i < particleSystems.Length;i ++){
            particleSystems[i].transform.localScale = (Vector3)scale;
            particleSystems[i].Play();
        }
        if (autoDestroy == true){
            //GameLauncher.instance.StartCoroutine(WaitForSecondAndDestroyGfx(count, destroyTime));
        }
        return count++;
    }

    private IEnumerator WaitForSecondAndDestroyGfx(int id, float time){
        yield return new WaitForSeconds(time);
        DestroyGfxByID(id);
    }


    public void DestroyGfxByID(int id){
        if (map.ContainsKey(id) == false || map[id] == null){
            return;
        }
        GameObject.Destroy(map[id]);
    }

    public void PlayGfxByID(int id){
        if (map.ContainsKey(id) == false || map[id] == null){
            return;
        }
        GameObject obj = map[id];
        if (obj.activeSelf == false){
            obj.SetActive(true);
        }
        ParticleSystem[] particleSystems = obj.GetComponentsInChildren<ParticleSystem>();
        for(int i = 0;i < particleSystems.Length;i ++){
            particleSystems[i].Play();
        }
    }

    public void SetActiveById(int id, bool show){
        if (map.ContainsKey(id) == false || map[id] == null){
            return;
        }
        map[id].SetActive(show);
    }
}