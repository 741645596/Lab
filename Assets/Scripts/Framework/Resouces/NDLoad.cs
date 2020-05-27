using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;

/// <summary>
/// 资源加载模块 
/// </summary>
/// <author>zhulin</author>
/// Server--> Local (down) ---> perfab ====> gameobject 
public class NDLoad  {
	/// <summary>
	/// 加载预制。
	/// </summary>
	public static GameObject LoadPrefab(string prefabName, Transform parent ,bool IsAB = false)
	{
		GameObject Prefab = null;

		if(IsAB == false)
		{
			Prefab = LoadPrefabInRes (ResoucesPathConfig.Res_Path + prefabName) ;
		}
		else
		{			
			Prefab = LoadPrefabInBundle (ConstantData.PathLocalBundle + ResoucesPathConfig.Bundle_Path + prefabName + ConstantData.ABEXT ) ;
		}

		if (Prefab == null) 
			return null;


		return Instantiate(Prefab ,parent);
	}

	/// <summary>
	/// 从Resourcer 加载预置
	/// </summary>
	private static GameObject LoadPrefabInRes(string PrefabName)
	{
		GameObject Prefab =Resources.Load (PrefabName ) as GameObject;
		if (Prefab == null) 
		{
			return null;
		}
		return Prefab;
	}
	/// <summary>
	/// 从Bundle 加载预置
	/// </summary>
	private static GameObject LoadPrefabInBundle( string PrefabName)
	{
	   GameObject Prefab = null;
		AssetBundle bundle = LoadLocalAssestBundle(PrefabName);
		if(bundle == null) 
			return null;
		else
		{
			Object ob = null;
			/*if (bundle.mainAsset != null) 
			{
				ob = bundle.mainAsset as Object;
				if (ob != null) {
					Prefab = ob as GameObject;
					bundle.Unload (false);
				} 
			}
			else */
			{
				foreach (string name in bundle.GetAllAssetNames ()) 
				{
					ob = bundle.LoadAsset (name) as GameObject;
					Prefab = ob as GameObject;
					bundle.Unload (false);
					break;
				}
			}
		}
		return Prefab;
	}

	/// <summary>
	/// 实例化预制
	/// </summary>
	public static GameObject Instantiate(GameObject Prefab ,Transform parent)
	{
		if (Prefab == null) 
			return null;

		GameObject go = GameObject.Instantiate(Prefab,Vector3.zero,Quaternion.identity) as GameObject;
		if (go == null)
			return null;
		
		go.SetActive(false);
		if(parent !=null)
		{
			//go.transform.parent = parent;
            go.transform.SetParent(parent);
		}
		go.name = Prefab.name;
		go.transform.localPosition = Prefab.transform.localPosition;
		go.transform.localRotation = Prefab.transform.localRotation;
		go.transform.localScale = Prefab.transform.localScale;
		go.SetActive(true);
		return go;
	}



	/// <summary>
	/// 只限制于加载本地的Bundle 对象。
	/// </summary>
	private static AssetBundle LoadLocalAssestBundle(string UrlPath)
	{
		byte[] stream = null;
		stream = File.ReadAllBytes(UrlPath);
		AssetBundle Bundle = AssetBundle.LoadFromMemory(stream);

	//	string[] lname = Bundle.GetAllAssetNames;

		foreach (string name in Bundle.GetAllAssetNames ()) 
		{
			Debug.Log ("assestname:" + name);
		}

		
		if(Bundle != null)
		{
			Debug.Log("加载成功");
			return Bundle;
		}
		else 
		{
			Debug.Log("加载失败");
			return null;
		}
	}

    #region

    public static WndItem LoadWndItem(string name, Transform t, bool isAB = false)
    {
		GameObject Prefab = null;
		if(isAB == false)
		{
			Prefab = LoadPrefabInRes (ResoucesPathConfig.Res_WndItemPath + name) ;
		}
		else
		{
			return null;
		}

		if (Prefab == null)  return null;

		GameObject go = Instantiate(Prefab ,t);
		if(go != null)
			return go.GetComponent<WndItem>() ;
		return null;
    }
    /// <summary>
    /// 加载UI项（按钮等，非窗口类）
    /// <para>UI项路径 UI/Items/</para>
    /// </summary>
    public static GameObject LoadUIBtn(string name, bool isAB = false)
    {
        GameObject Prefab = null;
        if (isAB == false)
        {
            Prefab = LoadPrefabInRes(ResoucesPathConfig.Res_WndItemPath + name);
        }
        else
        {
            return null;
        }
        if (Prefab == null) return null;
        GameObject go = Instantiate(Prefab, WndManager.GetWndRoot());
        return go;
    }


	public static GameObject LoadWnd(string name, Transform t, bool isAB = false)
	{
		GameObject Prefab = null;

		if (isAB == false) 
		{
			Prefab = LoadPrefabInRes (ResoucesPathConfig.Res_WndPath + name);
		}
		else
			return null;

		if (Prefab == null) 
			return null;


		return Instantiate(Prefab ,t);
	}

    #endregion
}
