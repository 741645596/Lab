// 一些编译开关配置
using UnityEngine;


/// <summary>
/// 存放客户端应用程序的一些相关信息
/// </summary>
public class App  
{
	/// <summary>
	/// 客户端版本
	/// </summary>
	public static string ver = string.Empty;


	// 是否已经初始化完成
	private static bool bInit = false;

	// 初始化处理
	public static void Init()
	{

		if (bInit)
			// 已经初始化过了
			return;

		// 不锁屏
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		// 登记所有的场景处理器
		RegisterScene();
		// TODO
		// 逻辑待补充

		// 标记初始化完成
		bInit = true;
	}

	/// <summary>
	/// 判断是否已经初始化完成
	/// </summary>
	public static bool IsInit
	{
		get { return bInit; }
	}



	// 登记所有的场景处理器
	private static void RegisterScene()
	{
		SceneM.RegisterScene (EditorScene.GetSceneName() ,new EditorScene());

	}
}
