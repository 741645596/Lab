using UnityEngine;
using System.Collections;
using System.Collections.Generic ;

/// <summary>
///  leap base
/// </summary>
/// <author>zhulin</author>
public class NDleapObject : MonoBehaviour {

	public int LeapIndex = 0 ;
	public NDCircuitObject m_Parent;

	public int CircuitObjectID 
	{
		get{ 
			if (m_Parent == null)
				return -1;
			else
				return m_Parent.LabObjID;}
	}
    /// <summary>
    /// 离鼠标光标的屏幕距离
    /// </summary>
	public float ScreenDistance
	{
		get{ return CalcScreenDistance ();}
	}

	public Vector3 ScreenPos
	{
		get { return GetScreenPos ();}
	}


	/// <summary>
	/// 接入点
	/// </summary>
	private static List<NDCircuitLeap> g_leap = new List<NDCircuitLeap>();


	/// <summary>
	/// 添加接入点
	/// </summary>
	public static void AddElementLeap(NDCircuitLeap Leap)
	{
		if (Leap == null)
			return;
		if (g_leap == null)
			return;
		if (g_leap.Contains(Leap) == false)
		{
			g_leap.Add(Leap);
		}
	}

	public static void RemoveElementLeap(NDCircuitLeap Leap)
	{
		if (Leap == null)
			return;
		if (g_leap == null)
			return;
		if (g_leap.Contains(Leap) == true)
		{
			g_leap.Remove(Leap);
		}
	}

	/// <summary>
	/// 计算距离
	/// </summary>
	protected float CalcDistance(NDleapObject leap1 ,NDleapObject leap2 )
	{
		if (leap1 == null || leap2 == null) return 10000.0f;
        Vector3 v1 = U3DUtil.SetZ(leap1.ScreenPos, 0);
        Vector3 v2 = U3DUtil.SetZ(leap2.ScreenPos, 0);
		return Vector2.Distance(v1 ,v2);
	}

	/// <summary>
	/// 计算离鼠标光标的屏幕距离
	/// </summary>
	public float CalcScreenDistance()
	{
		Vector3 MouseScreen =  new Vector3(Input.mousePosition.x,Input.mousePosition.y,0); 
		float dis1 = Vector2.Distance(MouseScreen ,ScreenPos);
		return dis1;
	}
	/// <summary>
	/// 计算离鼠标的屏幕距离
	/// </summary>
	public Vector3 GetScreenPos()
	{
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position); 
		pos.z = 0;
		return pos;
	}

	/// <summary>
	/// 管理接入点。
	/// </summary>
	public static List<NDCircuitLeap> GetAllLeap()
	{
		return g_leap;
	}

	/// <summary>
	/// 清理数据
	/// </summary>
	public static void Clear()
	{
		g_leap.Clear ();
	}
}
