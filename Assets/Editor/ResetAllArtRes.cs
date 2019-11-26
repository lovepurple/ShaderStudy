using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using JAlgorithm;

public class ResetAllArtRes{
	private static string PrefabsDir = Application.dataPath + "/Art/Art_scene/Common/Items/";
	private static string MaterialsDir = Application.dataPath + "/Art/Art_scene/Common/Materials/";
	private static string ScenesDir = Application.dataPath + "/Art/Art_scene/Common/Sences/";
	private static string TexturesDir = Application.dataPath + "/Art/Art_scene/Common/Textures/";
	private static string MeshsDir = Application.dataPath + "/Art/Art_scene/Common/Meshs/";
	private static string CommonDir = Application.dataPath + "/Art/Art_scene/Common/";
	private static string ArtDir = Application.dataPath + "/Art/Art_scene/";
	private static string NotCommonPrefabsDir = Application.dataPath + "/Art/Art_scene/Distinctive/Items/";
	private static string NotCommonMaterialsDir = Application.dataPath + "/Art/Art_scene/Distinctive/Materials/";
	private static string NotCommonTexturesDir = Application.dataPath + "/Art/Art_scene/Distinctive/Textures/";
	private static string NotCommonMeshsDir = Application.dataPath + "/Art/Art_scene/Distinctive/Meshs/";
	private static System.Security.Cryptography.MD5 md5;
	[MenuItem("Tools/1.删除重复Textures并修改资源引用关系")]
	public static void ResetAllTextures(){
		md5 = new System.Security.Cryptography.MD5CryptoServiceProvider ();
		List<string> artMaterialFilesList = GetAllFileWithExtension (ArtDir, new string[] { ".mat" });
		List<string> artTextureFilesList = GetAllFileWithExtension (ArtDir, new string[] { ".png", ".tga", ".jpg" });

		//选择相同文件集合，并随机选一个作为保留文件,其余为待删除文件
		List<HashSet<int>> sameFiles = FindSameFiles (artTextureFilesList);
		Debug.Log ("相同文件集合数:"+sameFiles.Count);
		if (sameFiles.Count == 0) {
			return;
		}
		List<int> rootList = new List<int> ();
		for (int i = 0; i < artTextureFilesList.Count; i++) {
			rootList.Add (i);
		}
		for (int i = 0; i < sameFiles.Count; i++) {
			int selectToRemainTexture = -1;
			foreach (var j in sameFiles[i]) {
				if (selectToRemainTexture == -1) {
					selectToRemainTexture = j;
				}
				rootList[j] = selectToRemainTexture;
			}
		}
		//读取资源文件
		List<Texture> texturesList = new List<Texture> ();
		Dictionary<Texture, int> texturesDict = new Dictionary<Texture, int> ();
		for (int i = 0; i < artTextureFilesList.Count; i++) {
			texturesList.Add (AssetDatabase.LoadAssetAtPath<Texture> ("Assets" + artTextureFilesList [i].Substring (Application.dataPath.Length)));
			texturesDict.Add (texturesList[i], i);
		}

		for (int i = 0; i < artMaterialFilesList.Count; i++) {
//		for (int i = 0; i < 1; i++) {
			Material obj = AssetDatabase.LoadAssetAtPath<Material> ("Assets" + artMaterialFilesList [i].Substring (Application.dataPath.Length));
//			Material obj = AssetDatabase.LoadAssetAtPath<Material> ("Assets\\Art\\Art_scene\\Common\\Materials\\mat_002n_landscape336.mat");
			List<Texture> textures = GetRefreceTexturesOnMaterials (obj ,(x,tex)=>{
				if(texturesDict.ContainsKey(tex) && rootList[texturesDict[tex]] != texturesDict[tex]){
					obj.SetTexture(x, texturesList[rootList[texturesDict[tex]]]);
				}
			});
		}

		AssetDatabase.SaveAssets ();

		for (int i = 0; i < artTextureFilesList.Count; i++) {
			if (rootList [i] != i) {
				try {
					File.Delete (artTextureFilesList [i]);
					Debug.Log ("delete success:" + artTextureFilesList [i]);
				} catch {
					Debug.LogError ("delete faild:" + artTextureFilesList [i]);
				}
			}
		}

		Debug.Log ("完成!");
	}

	[MenuItem("Tools/2.删除重复Fbx文件并修改资源引用关系")]
	public static void ResetAllFbx(){
		md5 = new System.Security.Cryptography.MD5CryptoServiceProvider ();
		List<string> artMeshFilesList = GetAllFileWithExtension (ArtDir, new string[] { ".fbx" });
		List<string> artScenesFilesList = GetAllFileWithExtension (ArtDir, new string[] { ".unity" });
		List<string> artPrefabFilesList = GetAllFileWithExtension (ArtDir, new string[] { ".prefab" });
		//忽略掉以dao开头的fbx文件
		for (int i = 0; i < artMeshFilesList.Count; i++) {
			if (Path.GetFileName(artMeshFilesList [i]).IndexOf ("dao") == 0) {
				artMeshFilesList.RemoveAt (i);
				i--;
			}
		}

		//选择相同文件集合，并随机选一个作为保留文件,其余为待删除文件
		List<HashSet<int>> sameFiles = FindSameFiles (artMeshFilesList);
		Debug.Log ("相同文件集合数:"+sameFiles.Count);
		if (sameFiles.Count == 0) {
			return;
		}
		for (int i = 0; i < sameFiles.Count; i++) {
			Debug.Log ("---------------");
			foreach (var j in sameFiles[i]) {
				Debug.Log (artMeshFilesList [j]);
			}
		}

		List<int> rootList = new List<int> ();
		for (int i = 0; i < artMeshFilesList.Count; i++) {
			rootList.Add (i);
		}
		for (int i = 0; i < sameFiles.Count; i++) {
			int selectToRemainTexture = -1;
			foreach (var j in sameFiles[i]) {
				if (selectToRemainTexture == -1) {
					selectToRemainTexture = j;
				}
				rootList[j] = selectToRemainTexture;
			}
		}

		//建立地址查询下标的字典
		Dictionary<string, int> meshsDict = new Dictionary<string, int> ();
		for (int i = 0; i < artMeshFilesList.Count; i++) {
			GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject> ("Assets" + artMeshFilesList [i].Substring (Application.dataPath.Length));
			meshsDict.Add (AssetDatabase.GetAssetPath(obj), i);
		}
		//对prefab处理;
		for (int i = 0; i < artPrefabFilesList.Count; i++) {
			try {
				var prefab = AssetDatabase.LoadAssetAtPath<GameObject> ("Assets" + artPrefabFilesList [i].Substring (Application.dataPath.Length));
				GetRefreceMeshsOnGameobject(prefab, (mf)=>{
					var ms = mf.sharedMesh;
					string objPath = AssetDatabase.GetAssetPath (ms);
					if (meshsDict.ContainsKey (objPath) && rootList [meshsDict [objPath]] != meshsDict [objPath]) {
						Debug.Log(ms.name);
						UnityEngine.Object[] meshs = AssetDatabase.LoadAllAssetRepresentationsAtPath ("Assets" + artMeshFilesList [rootList [meshsDict [objPath]]].Substring (Application.dataPath.Length));
						for (int k = 0; k < meshs.Length; k++) {
							if (meshs [k].name == ms.name && typeof(Mesh) == meshs[k].GetType()) {
								Debug.Log(ms.name);
								mf.sharedMesh = (Mesh)meshs [k];
								break;
							}
						}
					}
				}, (mc)=>{
					var ms = mc.sharedMesh;
					string objPath = AssetDatabase.GetAssetPath (ms);
					if (meshsDict.ContainsKey (objPath) && rootList [meshsDict [objPath]] != meshsDict [objPath]) {
						Debug.Log(ms.name);
						UnityEngine.Object[] meshs = AssetDatabase.LoadAllAssetRepresentationsAtPath ("Assets" + artMeshFilesList [rootList [meshsDict [objPath]]].Substring (Application.dataPath.Length));
						for (int k = 0; k < meshs.Length; k++) {
							if (meshs [k].name == ms.name && typeof(Mesh) == meshs[k].GetType()) {
								Debug.Log(ms.name);
								mc.sharedMesh = (Mesh)meshs [k];
								break;
							}
						}
					}
				});
			} catch(Exception ex) {
				Debug.Log (ex.ToString());
				Debug.LogError (artPrefabFilesList[i]);
			}
		}

		AssetDatabase.SaveAssets ();

		for (int i = 0; i < artMeshFilesList.Count; i++) {
			if (rootList [i] != i) {
				try {
					File.Delete (artMeshFilesList [i]);
					Debug.Log ("delete success:" + artMeshFilesList [i]);
				} catch {
					Debug.LogError ("delete faild:" + artMeshFilesList [i]);
				}
			}
		}
//
//		Debug.Log ("完成!");
	}

	[MenuItem("Tools/3.删除重复Mat文件并修改资源引用关系")]
	public static void ResetAllMaterial(){
		md5 = new System.Security.Cryptography.MD5CryptoServiceProvider ();
		List<string> artMaterialsList = GetAllFileWithExtension (ArtDir, new string[] { ".mat" });
		List<string> artPrefabFilesList = GetAllFileWithExtension (ArtDir, new string[] { ".prefab" });

		//选择相同文件集合，并随机选一个作为保留文件,其余为待删除文件
		List<HashSet<int>> sameFiles = FindSameFiles (artMaterialsList);
		Debug.Log ("相同文件集合数:"+sameFiles.Count);
		if (sameFiles.Count == 0) {
			return;
		}

		List<int> rootList = new List<int> ();
		for (int i = 0; i < artMaterialsList.Count; i++) {
			rootList.Add (i);
		}
		for (int i = 0; i < sameFiles.Count; i++) {
			int selectToRemainTexture = -1;
			foreach (var j in sameFiles[i]) {
				if (selectToRemainTexture == -1) {
					selectToRemainTexture = j;
				}
				rootList[j] = selectToRemainTexture;
			}
		}

		//建立地址查询下标的字典
		Dictionary<string, int> matsDict = new Dictionary<string, int> ();
		for (int i = 0; i < artMaterialsList.Count; i++) {
			Material obj = AssetDatabase.LoadAssetAtPath<Material> ("Assets" + artMaterialsList [i].Substring (Application.dataPath.Length));
			matsDict.Add (AssetDatabase.GetAssetPath (obj), i);
		}
		//对prefab处理;
		for (int i = 0; i < artPrefabFilesList.Count; i++) {
			try {
				var prefab = AssetDatabase.LoadAssetAtPath<GameObject> ("Assets" + artPrefabFilesList [i].Substring (Application.dataPath.Length));
				GetRefreceMaterialsOnGameobject(prefab, (mr)=>{
					var mats = mr.sharedMaterials;
					for(int j=0;j<mats.Length;j++){
						string objPath = AssetDatabase.GetAssetPath (mats[j]);
						if (matsDict.ContainsKey (objPath) && rootList [matsDict [objPath]] != matsDict [objPath]) {
							Material mat = AssetDatabase.LoadAssetAtPath<Material> ("Assets" + artMaterialsList [rootList [matsDict [objPath]]].Substring (Application.dataPath.Length));
							mats[j] = mat;
						}
					}
					mr.sharedMaterials = mats;
				});
			} catch {
				Debug.LogError (artPrefabFilesList[i]);
			}
		}

		AssetDatabase.SaveAssets ();

		for (int i = 0; i < artMaterialsList.Count; i++) {
			if (rootList [i] != i) {
				try {
					File.Delete (artMaterialsList [i]);
					Debug.Log ("delete success:" + artMaterialsList [i]);
				} catch {
					Debug.LogError ("delete faild:" + artMaterialsList [i]);
				}
			}
		}

		Debug.Log ("完成!");
	}

	[MenuItem("Tools/4.挪动文件")]
	public static void MoveFilesToRightDirectory(){
		if (Directory.Exists (PrefabsDir) == false) {
			Directory.CreateDirectory (PrefabsDir);
		}
		if (Directory.Exists (MaterialsDir) == false) {
			Directory.CreateDirectory (MaterialsDir);
		}
		if (Directory.Exists (TexturesDir) == false) {
			Directory.CreateDirectory (TexturesDir);
		}
		if (Directory.Exists (MeshsDir) == false) {
			Directory.CreateDirectory (MeshsDir);
		}
		if (Directory.Exists (NotCommonPrefabsDir) == false) {
			Directory.CreateDirectory (NotCommonPrefabsDir);
		}
		if (Directory.Exists (NotCommonMaterialsDir) == false) {
			Directory.CreateDirectory (NotCommonMaterialsDir);
		}
		if (Directory.Exists (NotCommonTexturesDir) == false) {
			Directory.CreateDirectory (NotCommonTexturesDir);
		}
		if (Directory.Exists (NotCommonTexturesDir) == false) {
			Directory.CreateDirectory (NotCommonTexturesDir);
		}

		List<string> artMaterialFilesList = GetAllFileWithExtension (ArtDir, new string[] { ".mat" });
		List<string> artTextureFilesList = GetAllFileWithExtension (ArtDir, new string[] { ".png", ".tga", ".jpg", ".tif" });
		List<string> artMeshFilesList = GetAllFileWithExtension (ArtDir, new string[] { ".fbx", ".obj" });
		List<string> artPrefabFilesList = GetAllFileWithExtension (ArtDir, new string[] { ".prefab" });

		for (int i = 0; i < artMaterialFilesList.Count; i++) {
			string tName = Path.GetFileName (artMaterialFilesList [i]).Replace (' ', '_');
			Debug.Log (tName);
			if (tName.IndexOf ("mat_") == 0) {
				tName = tName;
			} else {
				tName = "mat_" + tName;
			}
			string fileName = tName;
			while (File.Exists (MaterialsDir + fileName) == true) {
				fileName = Path.GetFileNameWithoutExtension(tName) + UnityEngine.Random.Range (0, 100) + Path.GetExtension(tName);
			}
			try {
				File.Move (artMaterialFilesList [i] + ".meta", MaterialsDir + fileName + ".meta");
				File.Move (artMaterialFilesList [i], MaterialsDir + fileName);
			} catch {
			}
		}

		for (int i = 0; i < artTextureFilesList.Count; i++) {
			string tName = Path.GetFileName (artTextureFilesList [i]).Replace (' ', '_');
			if (tName.IndexOf ("tex_") == 0) {
				tName = tName;
			} else {
				tName = "tex_" + tName;
			}
			string fileName = tName;
			while (File.Exists (TexturesDir + fileName) == true) {
				fileName = Path.GetFileNameWithoutExtension(tName) + UnityEngine.Random.Range (0, 100) + Path.GetExtension(tName);
			}
			try {
				File.Move (artTextureFilesList [i] + ".meta", TexturesDir + fileName + ".meta");
				File.Move (artTextureFilesList [i], TexturesDir + fileName);
			} catch {
			}
		}

		for (int i = 0; i < artMeshFilesList.Count; i++) {
			string tName = Path.GetFileName (artMeshFilesList [i]).Replace (' ', '_');
			string targetDir = MeshsDir;
			if (tName.IndexOf ("dao") == 0) {
				targetDir = NotCommonMeshsDir;
			}

			if (tName.IndexOf ("fbx_") == 0) {
				tName = tName;
			} else {
				tName = "fbx_" + tName;
			}
			string fileName = tName;
			while (File.Exists (MeshsDir + fileName) == true) {
				fileName = Path.GetFileNameWithoutExtension(tName) + UnityEngine.Random.Range (0, 100) + Path.GetExtension(tName);
			}
			try {
				File.Move (artMeshFilesList [i] + ".meta", targetDir + fileName + ".meta");
				File.Move (artMeshFilesList [i], targetDir + fileName);
			} catch {
			}
		}

		for (int i = 0; i < artPrefabFilesList.Count; i++) {
			string tName = Path.GetFileName (artPrefabFilesList [i]).Replace (' ', '_');
			string targetDir = PrefabsDir;
			if (tName.IndexOf ("dao") == 0) {
				targetDir = NotCommonPrefabsDir;
			}

			if (tName.IndexOf ("pre_") == 0) {
				tName = tName;
			} else {
				tName = "pre_" + tName;
			}
			string fileName = tName;
			while (File.Exists (PrefabsDir + fileName) == true) {
				fileName = Path.GetFileNameWithoutExtension(tName) + UnityEngine.Random.Range (0, 100) + Path.GetExtension(tName);
			}
			try {
				File.Move (artPrefabFilesList [i] + ".meta", targetDir + fileName + ".meta");
				File.Move (artPrefabFilesList [i], targetDir + fileName);
			} catch {
			}
		}
	}

	[MenuItem("Tools/5.把场景中的FBX文件改成Prefab")]
	public static void ChanggeFbxToPrefab(){
		List<string> artSceneFilesList = GetAllFileWithExtension (ScenesDir, new string[] { ".unity" });
		List<string> artMeshFilesList = GetAllFileWithExtension (ArtDir, new string[] { ".fbx" });
		List<string> artPrefabFilesList = GetAllFileWithExtension (ArtDir, new string[] { ".prefab" });

		Dictionary<string, GameObject> meshToPrefabMap = new Dictionary<string, GameObject> ();
		//对prefab处理;找出所有由fbx文件直接衍生出来的prefab
		for (int i = 0; i < artPrefabFilesList.Count; i++) {
			try {
				var prefab = AssetDatabase.LoadAssetAtPath<GameObject> ("Assets" + artPrefabFilesList [i].Substring (Application.dataPath.Length));
//				var prefab = AssetDatabase.LoadAssetAtPath<GameObject> ("Assets/Art\\Art_scene\\Common\\Items\\pre_palm_0834.prefab");
				List<Mesh> meshs = GetRefreceMeshsOnGameobject (prefab);
				if(meshs.Count == 0){
					continue;
				}
				string meshPath = AssetDatabase.GetAssetPath (meshs[0]);
				var fbxMeshs = AssetDatabase.LoadAllAssetRepresentationsAtPath (meshPath);
				Mesh[] mss = new Mesh[fbxMeshs.Length];
				for(int j=0;j<mss.Length;j++)
				{
					mss[j] = (Mesh)fbxMeshs[j];
				}
				if(CheckPrefabMakeFromFbx(meshs.ToArray(), mss)){
					meshToPrefabMap.Add(meshPath, prefab);
				}
			} catch {
			}
		}

		//对场景处理
		for (int i = 0; i < artSceneFilesList.Count; i++) {
//		for (int i = 4; i < 5; i++) {
			try {
				var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene ("Assets" + artSceneFilesList [i].Substring (Application.dataPath.Length));
//				var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene ("Assets/Art\\Art_scene\\Common\\Sences/game002n.unity");
				GameObject[] objs = scene.GetRootGameObjects ();
				List<GameObject> needDeleteList = new List<GameObject>();
				for (int j = 0; j < objs.Length; j++) {
					DfsAllGameObject (objs [j], (obj) => {
						string objPath = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(obj));
//						Debug.Log(objPath);
						if (string.IsNullOrEmpty(objPath) == false && Path.GetExtension(objPath).ToLower() == ".fbx"){
//							Debug.Log(objPath);
							Debug.Log(obj.name);
							if(meshToPrefabMap.ContainsKey(objPath) == true){
								GameObject new_Obj = PrefabUtility.InstantiatePrefab(meshToPrefabMap[objPath]) as GameObject;
								new_Obj.transform.SetParent(obj.transform.parent);
								new_Obj.transform.localPosition = obj.transform.position;
								new_Obj.transform.localScale = obj.transform.localScale;
								new_Obj.transform.localRotation = obj.transform.localRotation;
								needDeleteList.Add(obj);
							}else{
								string name = Path.GetFileNameWithoutExtension(objPath);
								if(name.IndexOf("fbx_") == 0){
									name = name.Substring(4);
								}
								meshToPrefabMap.Add(objPath, PrefabUtility.CreatePrefab("Assets/Art/Art_scene/Common/Items/pre_" + name + ".prefab", obj));
								GameObject new_Obj = PrefabUtility.InstantiatePrefab(meshToPrefabMap[objPath]) as GameObject;
								new_Obj.transform.SetParent(obj.transform.parent);
								new_Obj.transform.localPosition = obj.transform.position;
								new_Obj.transform.localScale = obj.transform.localScale;
								new_Obj.transform.localRotation = obj.transform.localRotation;
								needDeleteList.Add(obj);
							}
							return true;
						}
						return false;
					});
				}
				for(int j=0;j<needDeleteList.Count;j++)
				{
					GameObject.DestroyImmediate(needDeleteList[j]);
				}
				UnityEditor.SceneManagement.EditorSceneManager.SaveScene (scene);
			} catch {
			}
		}
		AssetDatabase.SaveAssets();
	}

	/// <summary>
	/// 判断两个mesh数组是否相同
	/// </summary>
	private static bool CheckPrefabMakeFromFbx( Mesh[] prefabsMesh, Mesh[] fbxMeshs){
		if (prefabsMesh.Length != fbxMeshs.Length) {
			return false;
		}
		HashSet<Mesh> meshs = new HashSet<Mesh> ();
		for (int i = 0; i < prefabsMesh.Length; i++) {
			meshs.Add (prefabsMesh[i]);
		}
		for (int i = 0; i < fbxMeshs.Length; i++) {
			if (meshs.Contains (fbxMeshs [i]) == false) {
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// 遍历object下面所有对象
	/// </summary>
	private static void DfsAllGameObject( GameObject obj,Func<GameObject,bool> callBack){
		if (callBack (obj)) {
			return;
		}
		for (int i = 0; i < obj.transform.childCount; i++) {
			DfsAllGameObject(obj.transform.GetChild (i).gameObject, callBack);
		}
	}

	/// <summary>
	/// 找到material上面的textures
	/// </summary>
	/// <returns>The refrece materials on gameobject.</returns>
	/// <param name="obj">Object.</param>
	private static List<Texture> GetRefreceTexturesOnMaterials(Material mat, Action<string, Texture> callBack = null){
		List<Texture> ret = new List<Texture> ();
		int cnt = ShaderUtil.GetPropertyCount (mat.shader);
		for(int i=0;i<cnt;i++){
			if (ShaderUtil.GetPropertyType(mat.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv) {
				string propName = ShaderUtil.GetPropertyName (mat.shader, i);
				Texture tex = mat.GetTexture (propName);
				if (tex != null) {
					ret.Add (tex);
					if (callBack != null) {
						callBack (propName, ret [ret.Count - 1]);
					}
				}
			}
		}
		return ret;
	}


	/// <summary>
	/// 找到prefab上面的material
	/// </summary>
	/// <returns>The refrece materials on gameobject.</returns>
	/// <param name="obj">Object.</param>
	private static List<Material> GetRefreceMaterialsOnGameobject(GameObject obj, Action<MeshRenderer> callBack){
		List<Material> ret = new List<Material> ();
		MeshRenderer mr = obj.GetComponent<MeshRenderer>();
		if (mr != null) {
			if (callBack != null) {
				callBack (mr);
			}
			ret.AddRange (mr.sharedMaterials);
		}
		for (int i = 0; i < obj.transform.childCount; i++) {
			ret.AddRange (GetRefreceMaterialsOnGameobject(obj.transform.GetChild (i).gameObject, callBack));
		}
		return ret;
	}

	/// <summary>
	/// 找到prefab上面的fbx
	/// </summary>
	/// <returns>The refrece materials on gameobject.</returns>
	/// <param name="obj">Object.</param>
	private static List<Mesh> GetRefreceMeshsOnGameobject(GameObject obj, Action<MeshFilter> callBack = null, Action<MeshCollider> callBack2 = null){
		List<Mesh> ret = new List<Mesh> ();
		MeshFilter mf = obj.GetComponent<MeshFilter>();
		if (mf != null) {
			if (callBack != null) {
				callBack (mf);
			}
			ret.Add (mf.sharedMesh);
		}
		MeshCollider mc = obj.GetComponent<MeshCollider>();
		if (mc != null) {
			if (callBack2 != null) {
				callBack2 (mc);
			}
		}
		for (int i = 0; i < obj.transform.childCount; i++) {
			ret.AddRange (GetRefreceMeshsOnGameobject(obj.transform.GetChild (i).gameObject, callBack));
		}
		return ret;
	}
		
	/// <summary>
	/// 找到相同的文件并分类后返回
	/// </summary>
	/// <returns>The same files.</returns>
	/// <param name="files">Files.</param>
	private static List<HashSet<int>> FindSameFiles(List<string> files){
		List<HashSet<int>> ret = new List<HashSet<int>> ();
		AndSearchSet ass = new AndSearchSet (files.Count);
		List<byte[]> md5List = new List<byte[]> ();
		FileStream fs;
		for (int i = 0; i < files.Count; i++) {
			fs = new FileStream (files [i], FileMode.Open);
			md5List.Add (md5.ComputeHash (fs));
			fs.Close ();
		}
		for (int i = 0; i < files.Count; i++) {
			for (int j = i + 1; j < files.Count; j++) {
				if (Compare (md5List[i], md5List[j])) {
					ass.And (i, j);
				}
			}
		}
		Dictionary<int, int> map = new Dictionary<int, int>(); 
		for (int i = 0; i < files.Count; i++) {
			if (map.ContainsKey (ass.Search (i)) == false) {
				map [ass.Search (i)] = ret.Count;
				ret.Add (new HashSet<int>());
			}
			ret [map [ass.Search (i)]].Add (i);
		}

		List<HashSet<int>> realRet = new List<HashSet<int>> ();
		for (int i = 0; i < ret.Count; i++) {
			if (ret [i].Count > 1) {
				realRet.Add (ret[i]);
			}
		}
		return realRet;
	}

	/// <summary>
	/// 对比一对byte数组
	/// </summary>
	/// <param name="code1">Code1.</param>
	/// <param name="code2">Code2.</param>
	private static bool Compare(byte[] code1, byte[] code2){
		if (code1.Length != code2.Length) {
			return false;
		}
		for (int i = 0; i < code1.Length; i++) {
			if (code1 [i] != code2 [i]) {
				return false;
			}
		}
		return true;
	}
		
	/// <summary>
	/// 通过后缀名获取相应文件
	/// </summary>
	/// <returns>The all file with extension.</returns>
	/// <param name="dir">Dir.</param>
	/// <param name="extension">Extension.</param>
	private static List<string> GetAllFileWithExtension(string dir, string[] extension){
		if (Path.GetDirectoryName (dir) == "Scenes") {
			return new List<string> ();
		}
		List<string> files = new List<string>(Directory.GetFiles (dir));
		List<string> retFiles = new List<string> ();
		for (int i = 0; i < files.Count; i++) {
			for (int j = 0; j < extension.Length; j++) {
				if (Path.GetExtension (files [i]).ToLower() == extension [j].ToLower()) {
					retFiles.Add (files [i]);
					break;
				}
			}
		}

		string[] dirs = Directory.GetDirectories (dir);
		for (int i = 0; i < dirs.Length; i++) {
			retFiles.AddRange(GetAllFileWithExtension(dirs [i], extension).ToArray());
		}
		return retFiles;
	}
}
