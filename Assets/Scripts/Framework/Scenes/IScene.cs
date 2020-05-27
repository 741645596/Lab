using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 场景载入、资源清理接口 
/// </summary>
/// <author>zhulin</author>
public abstract class IScene
{
	private List<SceneObj> m_listSceneObj=new List<SceneObj>();
	protected AsyncOperation async;
	public float LoadingProgress
	{
		get{return async.progress;}
	}

	public AsyncOperation AsyncLoading
	{
		get{return async;}
	}


	public virtual void AddSceneObj(SceneObj go)
	{
		if (null != go&&!m_listSceneObj.Contains(go))
			m_listSceneObj.Add(go);
	}
	public virtual void ReMoveSceneObj(SceneObj go)
	{
		if (null != go&&m_listSceneObj.Contains(go))
			m_listSceneObj.Remove(go);
	}
	/// <summary>
	/// 查找指定类型的
	/// </summary> 
	public T GetSceneObj<T> (string name = "") where T :SceneObj
	{
		foreach (SceneObj obj in m_listSceneObj)
		{
			if (obj == null)  continue;
			if (name == "") {
				if( obj is T)
					return (obj as T);
			} 
			else 
			{
				if( obj is T  && obj.name == name)
					return (obj as T);
			}

		}
		return  null;
	}


	/// <summary>
	/// 获取场景类型
	/// </summary>
	public static string GetSceneName()
	{
		return "IScene";
	}

	/// <summary>
	/// 资源载入入口 
	/// </summary>
	public abstract IEnumerator Load();
	
	/// <summary>
	/// 准备载入场景
	/// </summary>
	public abstract void PrepareLoad();
	
	/// <summary>
	/// 资源卸载
	/// </summary>
	public abstract void Clear();
	
	/// <summary>
	/// 是否已经载入完成
	/// </summary>
	public abstract bool IsEnd();

	/// <summary>
	/// 场景start 函数
	/// </summary>
	public abstract void Start();
	/// <summary>
	/// 接管场景中关注对象的Update
	/// </summary>
	public abstract void Update(float deltaTime);

	
	/// <summary>
	/// 接管场景中关注对象的LateUpdate
	/// </summary>
	public abstract void LateUpdate(float deltaTime) ;

	/// <summary>
	/// 接管场景中关注对象的FixedUpdate
	/// </summary>
	public abstract void FixedUpdate (float deltaTime) ;

	/// <summary>
	/// 构建场景数据
	/// </summary>
	public  void BuildScene()
	{
		BuildWorld();
		BuildUI();
	}

	/// <summary>
	/// 构建UI
	/// </summary>
	public virtual void BuildUI()
	{
		
	}

	/// <summary>
	/// 构建世界空间
	/// </summary>
	public virtual void BuildWorld()
	{
		
	}


	public abstract void OnMouseDown(SceneObj objScene);

	public abstract void OnMouseUp(SceneObj objScene);


}
