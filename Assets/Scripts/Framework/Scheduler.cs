using UnityEngine;

/// <summary>
/// 帧调度，本组件绑定到gameobject以发挥作用 
/// </summary>
public class Scheduler : MonoBehaviour
{
	public bool ac = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
		SceneM.LinkScheduler(gameObject);
		App.Init ();
    }

	void Start ()
	{
		Screen.fullScreen = true;
		Screen.SetResolution (1920, 1200, true);

		AppData.LoadLabObjData ();
		WndManager.GetWnd<LabWnd>();
	}

	void Update()
	{	
		try
		{
			if(Input.GetKeyDown(KeyCode.Escape) == true)
			{
				Application.Quit();	
			}
			//定时器调度
			Timer.Update();
			//协程调度
			ND.Coroutine.Update();
			//场景数据调度
			SceneM.Update(Time.deltaTime);
		}
		catch (System.Exception e)
		{
			Debug.LogError(e);
		}
	}

	
	void LateUpdate() 
	{
	}



	void FixedUpdate ()
	{

	}

    // 退出游戏的处理 临时
    bool allowQuit = false;

	[System.Obsolete]
	void OnApplicationQuit()
    {
        if (allowQuit)
            return;
        Application.CancelQuit();
        Application.Quit();

    }
}
