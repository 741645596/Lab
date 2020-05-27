using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


/// <summary>
/// 缓存资源列表
/// </summary>
/// <author>zhulin</author>
public class LabObjData : ScriptableObject {
	public List<LabObjUnit> m_Items  = new List<LabObjUnit>();


	public LabObjUnit FindObjUnit(int LabObjType)
	{
		foreach (LabObjUnit unit in m_Items) 
		{
			if (unit == null)
				continue;
			if (LabObjType == (int)unit.type)
				return unit;
		}
		return null;
	}
		
}



/// <summary>
/// 缓存资源列表
/// </summary>
/// <author>zhulin</author>
[Serializable]
public class LabObjUnit  {
	public string ThumbPic; 
	public string AssestPath;
	public LabObjType type ;
}

public enum LabObjType
{
	LabOBJ       = 0xffffff ,
	//circuit obj
	Power        = 0x110000 ,
	Switch       = 0x120000 ,
	Resistance   = 0x130000 ,
	Light        = 0x131000 ,
	Ammeter      = 0x132000 ,
	Voltmeter    = 0x133000 ,
    Image        = 0x134000,
    Text         = 0x135000,
	//导线
	EleLine      = 0x180000 ,

}