using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>

/// <para>QFord</para>
/// </summary>
public class EditorScene : IScene {


	public new static string GetSceneName()
	{
		return "Editor";
	}
 
	public new AsyncOperation AsyncLoading
	{
		get{return async;}
	}

	/// <summary>
	/// 资源载入入口 
	/// </summary> 
	//private AsyncOperation async;
	public override IEnumerator Load()
	{
		async = SceneManager.LoadSceneAsync (EditorScene.GetSceneName());
		return null;
	}

	/// <summary>
	/// 准备载入场景
	/// </summary>
	public override void PrepareLoad()
	{
	}

	/// <summary>
	/// 资源卸载
	/// </summary>
	public override void Clear()
	{

	}

	/// <summary>
	/// 是否已经载入完成
	/// </summary>
	public override bool IsEnd()
	{		
		if(async != null)
		{
			return async.isDone;
		}
		else return false;
	}

	public override void BuildUI()
	{
	}


	/// <summary>
	/// 构建世界空间
	/// </summary>
	public override void BuildWorld()
	{

	}

	/// <summary>
	/// 场景start 接口
	/// </summary>
	public override void Start()
	{

	}
	/// <summary>
	/// 接管场景中关注对象的Update
	/// </summary>
	public override void Update(float deltaTime)
	{
	}
	/// <summary>
	/// 接管场景中关注对象的LateUpdate
	/// </summary>
	public override void LateUpdate(float deltaTime) 
	{

	}
	/// <summary>
	/// 接管场景中关注对象的FixedUpdate
	/// </summary>
	public override void FixedUpdate (float deltaTime) 
	{

	}
	public override void OnMouseDown(SceneObj objScene)
	{
	}

	public override void OnMouseUp(SceneObj objScene)
	{

	}
	
}
