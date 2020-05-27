using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Unity3D常用的接口
/// </summary>
/// <author>zhulin&QFord</author>
public class U3DUtil
{
	/// <summary>
	/// 根据名字查找子对象
	/// </summary>
	public static GameObject FindChild(GameObject ob, string name)
	{
		Transform t = ob.transform.Find(name);
		if (t == null)
			return null;
		
		return t.gameObject;
	}
	
	/// <summary>
	/// 在所有下属物件中寻找子对象
	/// </summary>
	public static GameObject FindChildDeep(GameObject ob, string name)
	{
		GameObject o = FindChild(ob, name);
		if (o != null)
			return o;
		
		foreach (Transform t in ob.transform)
		{
			o = FindChildDeep(t.gameObject, name);
			if (o != null)
				return o;
		}
		return null;
	}
	/// <summary>
	/// 区别于系统的GetComponentInChildren，本接口会在所有层级的下属物件中查找
	/// bWantActiveself 是否有显示有要求
	/// </summary>
	public static T GetComponentInChildren<T>(GameObject ob ,bool bWantActiveself)
	{
		// 先在自己的直接下属物件中查找
		foreach (Transform t in ob.transform)
		{
			T c = (T) (object) t.GetComponent(typeof(T));
			if (c != null)
				if(bWantActiveself&&t.gameObject.activeSelf||!bWantActiveself)
				// 找到了，直接返回
				return c;
		}
		
		// 没有找到，则继续在下属的下属中查找
		foreach (Transform t in ob.transform)
		{
			T c = GetComponentInChildren<T>(t.gameObject,bWantActiveself);
			if (c != null)
				if(bWantActiveself&&t.gameObject.activeSelf||!bWantActiveself)
				// 找到了
				return c;
		}
		
		// 没有此组件
		return (T) (object)null;
	}
	
	/// <summary>
    ///遍历查找所有下属附件（isRecusive 是否递归）
	/// </summary>
	public static T[] GetComponentsInChildren<T>(GameObject ob,bool isRecusive = true)
	{
		// 存放结果
		List<T> list = new List<T>();
		
		// 先在自己的直接下属物件中查找
		foreach (Transform t in ob.transform)
		{
			T c = (T) (object) t.GetComponent(typeof(T));
			if (c != null)
				list.Add(c);
		}
        if (isRecusive)
        {
            foreach (Transform t in ob.transform)
            {
                T[] arr = GetComponentsInChildren<T>(t.gameObject);
                foreach (T c in arr)
                    list.Add(c);
            }
        }
		// 继续在下属的下属中查找
		
		return list.ToArray();
	}
    /// <summary>
    /// 对某对象及其下属的所有对象设置激活
    /// 等同于
    /// </summary>
    public static void SetActive(GameObject ob, bool active)
    {
        foreach (Transform t in ob.transform)
            SetActive(t.gameObject, active);
        ob.SetActive(active);
    }
    /// <summary>
    /// 设置vector3分量值的便捷方法
    /// </summary>
    public static Vector3 SetZ(Vector3 pos ,float z)
    {
        Vector3 r = new Vector3(pos.x, pos.y, z);
        return r;
    }
    //end 设置vector3分量值的便捷方法

    /// <summary>
    /// 获取向量列表的总长度
    /// </summary>
    public static float GetVectorListDistance(List<Vector3> vList)
    {
        if (vList.Count <= 1)
            return 0;

        float distance = 0f;
        for (int i = 1; i < vList.Count;i++ )
        {
            distance += Vector3.Distance(vList[i - 1], vList[i]);
        }
        return distance;
    }

    /// <summary>
    /// 修改材质贴图
    /// </summary>
    public static void ChangeTexture(GameObject go,Texture t)
    {
        Renderer ren = go.GetComponent<Renderer>();
        if (go==null || t==null)
        {
            return;
        }
        foreach (Material m in ren.materials)
        {
            m.EnableKeyword("_MainTex");
            m.SetTexture("_MainTex", t);
        }
    }

    /// <summary>
    /// 修改材质贴图
    /// </summary>
    public static void ChangeTexture(Transform go, Texture t)
    {
        ChangeTexture(go.gameObject, t);
    }
}

