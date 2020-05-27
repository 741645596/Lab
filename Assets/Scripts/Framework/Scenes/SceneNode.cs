using UnityEngine;
using System.Collections;


/// <summary>
/// 场景节点
/// </summary>
/// <author>zhulin</author>
public class SceneNode  {

	private static Transform g_tMapNode = null;
	public  static Transform MapNode
	{
		get{return g_tMapNode ;}
	}

	private static Transform g_tUINode = null;
	public  static Transform UINode
	{
		get{return g_tUINode ;}
	}
	/// <summary>
	/// 场景节点
	/// </summary>
	public static void SetMapNode(Transform mapNode)
	{
		g_tMapNode = mapNode;
	}
	/// <summary>
	/// 场景节点
	/// </summary>
	public static void SetUINode(Transform UINode)
	{
		g_tUINode = UINode;
	}
	/// <summary>
	/// 清理节点
	/// </summary>
	public static void Clear()
	{
		g_tMapNode = null;
		g_tUINode = null;
	}
}
