using UnityEngine;

public class ConstantData
{
    public static string PathLocalBundle
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            return  Application.streamingAssetsPath +"/";
            //return Application.persistentDataPath + "/";
#elif UNITY_ANDROID
			        return  Application.persistentDataPath + "/" ;
#elif UNITY_IPHONE
					return   Application.persistentDataPath ;
#elif UNITY_WEBPLAYER
					return Application.persistentDataPath ;
#else
					return Application.persistentDataPath ;
#endif

        }
    }

	public static string ABEXT
	{
		get
		{
		#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		return ".PC3D";
		#elif UNITY_ANDROID
			return ".Android3d" ;
		#else
			return ".assetbundle";
		#endif
		}
	}

	public static string g_version = "v0.0.0.1";

	public static string LabPath = Application.dataPath + "/UserData/";
	/// <summary>
	/// 保存编辑器 程序配置文件扩展名
	/// </summary>
	public static string  LabEXT= ".ndlab";

    /// <summary>
    /// 导线接线头离元器件接线头高亮的距离（屏幕坐标）
    /// </summary>
    public static  float m_fHighlightDistance = 30f;
}
