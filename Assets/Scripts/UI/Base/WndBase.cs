using UnityEngine;

/// <summary>
/// 普通窗体基类
/// </summary>
/// <author>zhulin</author>
/// <alter>QFord</alter>
public class WndBase : MonoBehaviour {

    /// <summary>
    /// 用于区分多窗口实例
    /// </summary>
    public int WndID = 0;

    public void WndStart()
    {
        InitWnd();
        BindEvents();
		SetWndDepth();
    }
	/// <summary>
	///  是否为全屏窗口
	/// </summary>
	public virtual bool IsFullWnd()
	{
		return false ;
	}
    /// <summary>
    /// 设置窗口深度（UGUI中实现待定）
    /// </summary>
	public virtual void SetWndDepth()
	{

	}
    /// <summary>
    /// 窗口初始华
    /// </summary>
    public virtual void InitWnd()
    { 
    }
    /// <summary>
    /// 窗口内事件绑定
    /// </summary>
    public virtual void BindEvents()
    {

    }
    /// <summary>
    ///  获取窗口ID，应和prefab、类名保持一致
    /// </summary>
    /// <returns></returns>
	public static string WndIDD()
	{
        //Debug.Log("派生类要覆盖DialogName方法");
		return "WndBase";
	}
    /// <summary>
    /// 显示或隐藏窗口
    /// </summary>
    /// <param name="isShow"></param>
	public virtual void ShowWnd(bool isShow)
	{
		gameObject.SetActive (isShow);
	}
    /// <summary>
    /// 播放窗口动画
    /// </summary>
	public virtual void PlayAnimation(string name)
	{
        Animation ani = gameObject.GetComponent<Animation>();
        if (ani != null)
            ani.Play(name);
	}
    /// <summary>
    /// 销毁窗口（下一帧）
    /// </summary>
	public virtual void DestroyWnd()
	{
		GameObject.Destroy (gameObject);
	}
}

