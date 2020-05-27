using UnityEngine;
using System.Collections;
using System.Collections.Generic ;


/// <summary>
///  实验对象创建工厂
/// </summary>
/// <author>zhulin</author>
public class LabObjectFactory  {


	public static NDlabObject  LoadObjType(int LabObjType ,Transform parent)
	{
		LabObjUnit unit = AppData.ObjData.FindObjUnit (LabObjType);
		if (unit != null) 
		{
			GameObject g = NDLoad.LoadPrefab (unit.AssestPath,parent ,false);
			if (g == null)
				return null;
			return  g.GetComponent<NDlabObject> ();
		}
		return null;
	}
}
