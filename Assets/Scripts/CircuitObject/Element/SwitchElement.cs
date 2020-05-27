using UnityEngine;
using System.Collections;
using SharpCircuit;
/// <summary>
/// 开关
/// </summary>
public enum SwitchState
{
    SwitchClose = 0,
    SwitchOpen = 1,
}
public class SwitchElement : NDCircuitObject
{
    /// <summary>
    /// 开关是否打开
    /// </summary>
    public bool IsOpen { get { return m_Switch.IsOpen; } }
	/// <summary>
	/// 开关开关闸
	/// </summary>
	public SwitchHandle m_Switch ;

    /// <summary>
    /// 试触需要最长的时间
    /// </summary>
    private float m_fTryTouchTime = 0;
    private float m_fTryTouchCounter = 0;
    /// <summary>
    /// 初始化
    /// </summary>
    public void Awake()
    {
        m_Switch = GetComponentInChildren<SwitchHandle>();
        this.EnableNumber = true;
        this.NumberName = "S";
    }


	void Start()
	{
		LabObjectDataFactory.SetSwitch (LabObjID ,false);
	}

    void Update()
    {
		CheckSwitchStateChange ();
    }
    /// <summary>
    /// 检测开关是否打开或者关闭，这个在外面需要实时检测
    /// </summary>
    /// <returns></returns>
    public bool CheckSwitchStateChange()
    {
		if (LabObjectDataFactory.GetSwitchState (LabObjID) != IsOpen) 
		{
			LabObjectDataFactory.SetSwitch (LabObjID ,IsOpen);
            if (CheckConnect() == true)
            {
                ModifyCirCuit();
            }
			return true;
		}
        return false;
    }
    /// <summary>
    ///取元气件速据
    /// </summary>
    public override LabObject GetCircuitData()
    {
		CircuitObject Info = new CircuitObject(ID, LabObjectType, Position);
        return Info;
    }
    /// <summary>
    ///设置元气件速据 （用于从文件系统还原数据）开关没特殊属性
    /// </summary>
    public override void SetData(CircuitObject Info)
    {
		base.SetData (Info);
    }

    public override void ResumeInfo(LabObject Info)
    {
        base.ResumeInfo(Info);
    }
	/// <summary>
	/// 获取类型
	/// </summary>
	public override LabObjType GetLabObjType()
	{
		return LabObjType.Switch;
	}
}
