using UnityEngine; 
using UnityEditor; 

public class ConfigAssetsWnd : EditorWindow {

	[MenuItem( "配置/元器件数据配置" )]
	static void Apply()
	{
		EditorWindow.GetWindow(typeof(ConfigAssetsWnd));
	}

	void OnGUI()
	{
		this.title = "元器件数据配置";
		this.minSize = new Vector2(400, 300);

		if (GUI.Button(new Rect (20,50,120 ,30),"元器件数据配置"))
		{
			CreateObjConfigAsset ();
			EditorUtility.DisplayDialog("提示", "元器件数据配置文件生成", "确定");
		}
	}


	private  void CreateObjConfigAsset ()
	{
		LabObjData asset = ScriptableObject.CreateInstance<LabObjData> ();

		//
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath ("Assets/Resources/Config/" + typeof (LabObjData).ToString() + ".asset" );
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
	}
}
