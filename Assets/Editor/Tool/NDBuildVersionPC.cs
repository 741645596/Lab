using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class NDBuildVersionPC : EditorWindow {

	[@MenuItem("TOOL/发布PC版本")]
	static void Apply()
	{
		PlayerSettings.bundleVersion = ConstantData.g_version ;
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
		EditorWindow.GetWindow(typeof(NDBuildVersionPC));
	}
	
	void OnGUI()
	{
		this.title = "版本发布工具";
		this.minSize = new Vector2(400, 300);
		
		if (GUI.Button(new Rect (150,10,120 ,30),"发布实验编辑器"))
		{
			PlayerSettings.companyName = "ND";
			PlayerSettings.productName = "电路实验编辑器";
			MoveAssest();
			BuildVersionEditor();
			EditorUtility.DisplayDialog("提示", "发布实验编辑器发布完成", "确定");
			ResetAssest();
		}


		if (GUI.Button(new Rect (150,50,120 ,30),"发布实验播放器"))
		{
			PlayerSettings.companyName = "ND";
			PlayerSettings.productName = "电路实验播放器";
			MoveAssest();
			BuildVersionPlayer();
			EditorUtility.DisplayDialog("提示", "发布实验播放器发布完成", "确定");
			ResetAssest();
		}
	}


	private void MoveAssest()
	{
	}


	private void BuildVersionEditor()
	{
		List<string> lScenePath = new List<string>();
		foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
		{
			string filename = Path.GetFileName(scene.path);
			if (filename == "Lauch.unity" || filename == "Editor.unity") 
			{
				UnityEngine.Debug.Log (filename);
				lScenePath.Add(scene.path);
			}
		}
		BuildPipeline.BuildPlayer(lScenePath.ToArray(), "./Product/PC/Editor/LabEditor.exe", 
		                          BuildTarget.StandaloneWindows, BuildOptions.None);

		string path = Application.dataPath;
		string path1 = path.Replace("Assets","Product/PC/Editor");
		UnityEngine.Debug.Log (path1);
		OpenFolderAndSelectFile(path1);

        EditorWindow wnd = EditorWindow.GetWindow(typeof(NDBuildVersionPC));
        wnd.Close();
        
	}

	private void BuildVersionPlayer()
	{
		List<string> lScenePath = new List<string>();
		foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
		{
			string filename = Path.GetFileName(scene.path);
			if (filename == "PlayerLauch.unity" || filename == "Player.unity") 
			{
				UnityEngine.Debug.Log (filename);
				lScenePath.Add(scene.path);
			}
		}
		BuildPipeline.BuildPlayer(lScenePath.ToArray(), "./Product/PC/Player/LabPlayer.exe", 
			BuildTarget.StandaloneWindows, BuildOptions.None);

		string path = Application.dataPath;
		string path1 = path.Replace("Assets","Product/PC/Player");
		UnityEngine.Debug.Log (path1);
		OpenFolderAndSelectFile(path1);

        EditorWindow wnd = EditorWindow.GetWindow(typeof(NDBuildVersionPC));
        wnd.Close();
	}


	private void ResetAssest()
	{

	}
		

	private void OpenFolderAndSelectFile(String fileFullName)
	{
		System.Diagnostics.Process.Start("explorer.exe", fileFullName.Replace("/", "\\"));
	}
}
