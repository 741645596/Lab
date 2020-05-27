using System.Collections.Generic;
using System;

/// <summary>
/// 直流电源（干电池）
/// </summary>
public class CurrentSourceElement : NDCircuitObject 
{
	/// <summary>
	/// 电池电压
	/// </summary>
	public float Volatage  = 3.0f ;
    /// <summary>
    /// 内阻
    /// </summary>
    private float m_fInternalResistance = 0;
    //private DCVoltageSource m_DCVoltageSource = new DCVoltageSource();
	/// <summary>
	/// 获取类型
	/// </summary>
	public override LabObjType GetLabObjType()
	{
		return LabObjType.Power;
	}

	void Start()
	{ 
		LabObjectDataFactory.SetPower (LabObjID ,Volatage);
	}

    /// <summary>
    ///取元气件速据
    /// </summary>
    public override LabObject GetCircuitData()
    {
		CircuitObject Info = new CircuitObject(ID, LabObjectType, Position);
        Dictionary<string, string> list = new Dictionary<string, string>();
        list.Add("m_fInternalResistance", m_fInternalResistance.ToString());
        return Info;
    }
    /// <summary>
    ///设置元气件速据 （用于从文件系统还原数据）
    /// </summary>
    public override void SetData(CircuitObject Info)
    {
		base.SetData (Info);
    }
    public override void ResumeInfo(LabObject Info)
    {
        base.ResumeInfo(Info);
    }

}
