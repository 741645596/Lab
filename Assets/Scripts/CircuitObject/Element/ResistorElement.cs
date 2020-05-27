using System.Collections.Generic;
using System;

/// <summary>
/// 定制电阻
/// </summary>
public class ResistorElement : NDCircuitObject
{
    /// <summary>
    /// 电阻值（默认是10）
    /// </summary>
    public float Resistance = 10f;
    /// <summary>
    /// 当前设置的电阻值（可以通过弹出面板设置电阻值）
    /// </summary>
    private float m_fCurResistance = 10f;

    void Awake()
    {
        this.EnableNumber = true;
        this.NumberName = "R";
    }

    void Start()
    {
        LabObjectDataFactory.SetResistance(LabObjID, Resistance);
    }

    /// <summary>
    ///取元气件速据
    /// </summary>
    public override LabObject GetCircuitData()
    {
        CircuitObject Info = new CircuitObject(ID, LabObjectType, Position);
        Dictionary<string, string> list = new Dictionary<string, string>();
        list.Add("m_fCurResistance", m_fCurResistance.ToString());
        return Info;
    }

    /// <summary>
    ///设置元气件速据 （用于从文件系统还原数据）
    /// </summary>
    public override void SetData(CircuitObject Info)
    {
        base.SetData(Info);

    }


    /// <summary>
    /// 获取类型
    /// </summary>
    public override LabObjType GetLabObjType()
    {
        return LabObjType.Resistance;
    }
}
