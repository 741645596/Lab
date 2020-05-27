using UnityEngine;
using System.Collections;


/// <summary>
/// 程序数据
/// </summary>
public class AppData  {

	/// <summary>
	/// AR app 配置 
	/// </summary>
	private static LabObjData m_LabObjData = null;
	public static LabObjData ObjData
	{
		get{return m_LabObjData;}
	}
	/// <summary>
	/// 加载数据
	/// </summary>
	public static void LoadLabObjData()
	{		
		m_LabObjData = Resources.Load ("Config/LabObjData") as LabObjData;
	}
}
