using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;



/// <summary>
/// 发布版本向导
/// </summary>
public class NDBuildVersion  {
	// 收集path目录下的所有资源
	private static List<string> CollectAsset( string path)
	{
		List<string> list = new List<string>();
		
		if (path.Contains(".svn") || ! Directory.Exists(path))
			return list;
		
		foreach (string file in Directory.GetFiles(path))
		{
			if (Path.GetExtension(file) == ".meta" ||
			    Path.GetExtension(file) == ".mat")
				continue;
			Object o = AssetDatabase.LoadAssetAtPath(file.Replace("\\", "/"), typeof(Object));
			if (o != null)
				list.Add(o.name + ".prefab");
		}
		return list;
	}

	public static void MoveAsset(string OldPath,string NewPath)
	{
		List<string> l =  CollectAsset(OldPath);
		foreach (string fileName in l)
		{
			AssetDatabase.MoveAsset(OldPath + "/" + fileName ,NewPath + "/" + fileName);
		}


	}
	/// <summary>
	/// 移动文件夹
	/// </summary>
	public static void MoveDir(string OldPath,string NewPath)
	{
		Directory.Move (OldPath ,NewPath);
	}

	/// <summary>
	/// 移动文件
	/// </summary>
	public static void MoveFile(string OldFilePath,string NewFilePath)
	{
		File.Move (OldFilePath ,NewFilePath);
	}
}
