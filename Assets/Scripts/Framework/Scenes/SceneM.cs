using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景管理器
/// </summary>
/// <author>zhulin</author>
public class SceneM 
{
	// 前一个场景
	private static IScene s_preScene = null;
	
	// 准备载入的场景
	private static IScene s_loadingIScene = null;
	
	// 当前是否正在载入场景
	private static bool bLoading = false;

	// 当前场景
	private static IScene s_CurIScene = null;
	
	// 场景载入器映射表
	private static Dictionary<string, IScene> sceneLoadControl = new Dictionary<string, IScene>();
	//private static Dictionary<string, ILoading> sceneLoadingAnim = new Dictionary<string, ILoading>();


	public static GameObject Schedulergo = null;
			
	// 待载入场景的结构
	private class SceneNode
	{
		public string name;
		public ILoading loading;
		
		public SceneNode(string name, ILoading loading)
		{
			this.name = name;
			this.loading = loading;
		}
	}

	public static IScene GetISceneByName(string sceneName)
	{
		IScene scene;
		sceneLoadControl.TryGetValue(sceneName, out scene);
		return scene;
	}

	//获取当前场景
	public static IScene GetCurIScene()
	{
		return s_CurIScene;
	}

	/// <summary>
	/// 当期那是否正在载入场景
	/// </summary>
	public static bool IsLoading { get { return bLoading; } }
	
	
	public static IScene GetLoadingIScene()
	{
		return s_loadingIScene;
	}

	/// <summary>
	/// 登记场景处理器 
	/// </summary>
	public static void LinkScheduler(GameObject go)
	{
		Schedulergo = go;
	}
	/// <summary>
	/// 登记场景处理器 
	/// </summary>
	public static void RegisterScene(string name, IScene scene)
	{
		sceneLoadControl[name] = scene;
	}

	public static void RegisterLoadingAnim(string name, ILoading lodingAnimaiton)
	{
	//	sceneLoadingAnim[name] = lodingAnimaiton;
	}
	/// <summary>
	/// 载入场景 
	/// </summary>
	/// <param name="sceneName">待载入的场景名称</param>
	/// <param name="sync">通过异步(false)方式还是同步(true)方式载入</param>
	/// <param name="loading">载入动画的实例<see cref="ILoading"/></param>
	/// <param name="force">是否需要保证载入成功(如果当前正在载入某个场景中，会仍到队列而不是直接失败)</param>
	/// <returns>成功处理则返回true</returns>
	public static bool Load(string sceneName, bool sync, ILoading loading, bool force)
	{
		if (!force && bLoading)
		{
			return false;
		}

		if (bLoading && sync)
		{
			return false;
		}

		if (bLoading)
		{
			// 添加到列表中，等待处理
			return true;
		}

		// 如果当前正处于目标场景，无视
		// 任务副本需要做特殊处理
		IScene scene = sceneLoadControl[sceneName];

		// 标记正在载入场景
		s_loadingIScene = scene;
		bLoading = false;
		if (scene == s_CurIScene)
			return true;
		bLoading = true;

		// 异步载入方式
		if (!sync)
		{
			ND.Coroutine.DispatchService(NonsyncLoad(sceneName, loading), Schedulergo, null);
			return true;
		}

		// 取得载入器
		IScene preScene = s_CurIScene;
		s_CurIScene = scene;

		// 同步载入(不会播放任何动画)
		try
		{
			if (null != preScene)
				preScene.Clear();

			// 2 载入新的场景
			scene.Load();

			// 3 数据切换回来
			s_preScene = preScene;


			scene.BuildScene();
			scene.Start();

		}
		catch (Exception e)
		{

		}


		// 载入完毕，进行垃圾回收
		bLoading = false;
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
		Load(sceneName, sync, force);
		return true;
	}

	/// <summary>
	/// 载入场景 
	/// </summary>
	/// <param name="sceneName">待载入的场景名称</param>
	/// <param name="sync">通过异步(false)方式还是同步(true)方式载入</param>
	/// <param name="loading">载入动画的实例<see cref="ILoading"/></param>
	/// <param name="force">是否需要保证载入成功(如果当前正在载入某个场景中，会仍到队列而不是直接失败)</param>
	/// <returns>成功处理则返回true</returns>
	public static bool Load(string sceneName, bool sync, bool force)
	{
		if (!force && bLoading)
		{
			return false;
		}

		if (bLoading && sync)
		{
			return false;
		}

		if (bLoading)
		{
			// 添加到列表中，等待处理
			return true;
		}

		// 如果当前正处于目标场景，无视
		// 任务副本需要做特殊处理
		IScene scene = sceneLoadControl[sceneName];
//		ILoading loading = sceneLoadingAnim[sceneName];

		// 标记正在载入场景
		s_loadingIScene = scene;
		bLoading = false;
		if (scene == s_CurIScene)
			return true;
		bLoading = true;

		// 异步载入方式
		if (!sync)
		{
			ND.Coroutine.DispatchService(NonsyncLoad(sceneName, null), Schedulergo, null);
			return true;
		}

		// 取得载入器
		IScene preScene = s_CurIScene;
		s_CurIScene = scene;


		try
		{
			if (null != preScene)
				preScene.Clear();

			// 2 载入新的场景
			scene.Load();

			// 3 数据切换回来
			s_preScene = preScene;


			scene.BuildScene();
			scene.Start();


		}
		catch (Exception e)
		{
		}


		// 载入完毕，进行垃圾回收
		bLoading = false;
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
		return true;
	}
	
	// 异步载入场景
	private static IEnumerator NonsyncLoad(string sceneName, ILoading loading)
	{		
		// 取得载入器
		IScene scene = sceneLoadControl[sceneName];
		s_preScene= s_CurIScene;
		s_loadingIScene = scene;

		
		// 欲载入的处理
		scene.PrepareLoad();		

		if(s_preScene!=null)
			s_preScene.Clear();


		
		// 3 载入新的场景
		scene.Load();

		/*if (sceneLoadingAnim.ContainsKey(sceneName))
		{
			sceneLoadingAnim[sceneName].SetAsyncLoading(scene.AsyncLoading);
		}*/
			
		// 1 播放开始载入的动画，并等待其播放完毕
		if (loading != null)
		{
			loading.Load();
			while (loading.IsLoading())
				yield return null;
		}
		yield return new ND.WaitForMSeconds(100f);

		
		// 等待载入完成
		while (!scene.IsEnd ()) 
			yield return null;
		// 场景载入完成(一定要在load完毕后才能替换，此时的环境才能称得上载入完成)
		s_CurIScene = scene;
		

		bLoading = false;

		//载入完成。
		scene.BuildScene();
		scene.Start();

		// 4 播放载入完成的动画，并等待其播放完毕
		if (loading != null)
		{
			loading.FadeOut();
			while (loading.IsFadingOut())
				yield return null;

			loading.TryDestroy();
		}

		// 载入完成		
	}
	// 当前场景Update
	public static void Update(float deltaTime)
	{
		if(s_CurIScene != null && s_CurIScene.IsEnd() == true)
		{
			s_CurIScene.Update(deltaTime);
		}
	}
	
	
	// 当前场景LateUpdate
	public static void LateUpdate(float deltaTime) 
	{
		if(s_CurIScene != null && s_CurIScene.IsEnd() == true)
		{
			s_CurIScene.LateUpdate(deltaTime);
		}
	}

	


	// 当前场景LateUpdate
	public static void FixedUpdate(float deltaTime) 
	{
		if(s_CurIScene != null && s_CurIScene.IsEnd() == true)
		{
			s_CurIScene.FixedUpdate(deltaTime);
		}
	}


}

/// <summary>
/// Loading动画接口 
/// </summary>
/// <author>weism</author>
public interface ILoading
{
	/// <summary>
	/// 播放准备加载的动画
	/// </summary>
	void FadeIn();

	/// <summary>
	/// 是否在播放准备加载的动画
	/// </summary>
	bool IsFadingIn();

	/// <summary>
	/// 播放加载中的动画 
	/// </summary>
	void Load();

	/// <summary>
	/// 是否在播加载中的动画
	/// </summary>
	bool IsLoading();

	/// <summary>
	/// 播放加载完毕的动画
	/// </summary>
	void FadeOut();

	/// <summary>
	/// 是否在播加载入完毕的动画 
	/// </summary>
	bool IsFadingOut();
	
	/// <summary>
	/// 播放结束后尝试回收loading资源
	/// </summary>
	void TryDestroy();

	void SetAsyncLoading(AsyncOperation async);
	
}




