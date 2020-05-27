using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 窗口管理类
/// <para>窗口预制路径Resource/Prefabs/UI</para>
/// <para>窗口命名和所挂脚本一致,脚本继承WndBase类</para>
/// </summary>
public class WndManager  {


    private static Transform g_tWndRoot = null;
    private static List<WndBase> m_lwnd = new List<WndBase>();

    /// <summary>
    /// 设置窗体根节点
    /// </summary>
    /// <returns></returns>
    public static void SetWndRoot(Transform tWndRoot)
    {
        g_tWndRoot = tWndRoot;
    }

    public static Transform GetWndRoot()
    {
        return g_tWndRoot;
    }
    /// <summary>
    /// 获取窗口
    /// </summary>
    /// <returns></returns>
	public static T FindWnd<T>()where T:WndBase
	{
		foreach (WndBase w in m_lwnd)
		{
			if(w == null) continue ;
			if ( w is T)
				return (T) w;
		}
		return default(T);;
	}

    public static T FindWnd<T>(int wndID) where T : WndBase
    {
        foreach (WndBase w in m_lwnd)
        {
            if (w == null) continue;
            if (w is T && wndID == w.WndID)
                return (T)w;
        }
        return default(T); ;
    }

    /// <summary>
    /// 获取窗口深度
    /// </summary>
    public static int GetWndDepth<T>() where T:WndBase
    {
        return 0;
    }
    /// <summary>
    /// 销毁窗口，如果不存在该窗口返回false
    /// </summary>
    /// <typeparam name="T">窗口实现类</typeparam>
    /// <returns>删除成功返回true</returns>
    public static bool DestoryWnd<T>() where T:WndBase
    {
        T wndDialog = FindWnd<T>();
        if (wndDialog!=null)
        {
            wndDialog.DestroyWnd();
            return true;
        }
        return false;
    }

    public static bool DestoryWnd<T>(int wndID) where T:WndBase
    {
        T wndDialog = FindWnd<T>(wndID);
        if (wndDialog != null)
        {
            wndDialog.DestroyWnd();
            return true;
        }
        return false;
    }
    /// <summary>
    /// Destroy multiple instance wnd with not equal wndID
    /// </summary>
    public static void DestoryTypeWnd<T>(int wndID) where T : WndBase
    {
        foreach (WndBase w in m_lwnd)
        {
            if (w == null || w.WndID == wndID) continue;
            DestoryWnd<T>(wndID);   
        }
    }

    /// <summary>
    /// 获取窗口，获取失败则创建窗口
    /// </summary>
	public static T GetWnd<T>(bool bFailCreate=true)where T:WndBase
	{
		T  Wnd = FindWnd<T>();

		if(Wnd != null) return Wnd;
		else
		{
            if (bFailCreate)
            {
                Wnd = CreateWnd<T>();
                return Wnd;
            }
            else
                return null;
		}
	}

    /// <summary>
    /// 创建子窗口（非WndRoot下{直接挂在窗口里})）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="strDialogIDD">窗口ID</param>
    /// <param name="parent">父节点</param>
    /// <returns></returns>
	private static T CreateWnd<T>()where T:WndBase
	{
        string strDialogIDD = typeof(T).ToString();

		if(strDialogIDD==WndBase.WndIDD())
			return default(T);

		GameObject go = NDLoad.LoadWnd( strDialogIDD, g_tWndRoot ) as GameObject;
        if (go == null)
        {
            Debug.Log("WndManager CreateWnd " + strDialogIDD + "  not found ! ");
            return default(T);
        }
		else
		{

			T wndDialog = go.GetComponent<T> ();
            if (wndDialog != null)
            {
                wndDialog.WndStart();
                m_lwnd.Add(wndDialog);
                return wndDialog;
            }
            else 
            {
                Debug.Log(strDialogIDD + "  no add wnd component ! ");
                GameObject.Destroy(go);
            }
            return default (T);
		}
	}
    /// <summary>
    /// 射线是否命中UI
    /// </summary>
	public static bool IsHitUGUI()
	{
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
	}
    /// <summary>
    /// 销毁WndRoot下的所有窗口
    /// </summary>
    public static void DestroyAllWnd()
    {
		foreach (WndBase w in m_lwnd)
		{
			if(w != null) 
			{
                w.DestroyWnd();
			}
		}
		m_lwnd.Clear();
		ResetWndDepth();
    }
	/// <summary>
	/// 显示或隐藏窗体
	/// </summary>
	public static T ShowWnd<T>(bool isShow)where T:WndBase
	{
		T wnd = FindWnd<T>();
		if(wnd != null)
		{
			wnd.gameObject.SetActive(isShow);
			return wnd;
		}
		else return default(T);
	}
    /// <summary>
    /// 显示或隐藏WndRoot下的所有窗口
    /// </summary>
    public static void ShowAllWnds(bool isShow )
    {
		foreach (WndBase w in m_lwnd)
		{
			if(w != null)
			{
				w.gameObject.SetActive(isShow);
			}
		}
    }
	/// <summary>
	/// 重置窗体深度
	/// </summary>
	private static void ResetWndDepth()
	{
		
	}

}
