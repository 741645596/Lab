using System.Collections.Generic;
using System;

/// <summary>
/// 电阻丝（有四个不同的电阻设定）
/// </summary>
public class ResistorWireElement : NDCircuitObject
{
    /// <summary>
    /// 当前电阻值
    /// </summary>
    private float Resistance = float.MaxValue;
    /// <summary>
    /// 当前设置的电阻丝类型（1,2,3,4）
    /// </summary>
    private int m_iType = 0;

    void Awake()
    {
        //this.EnableNumber = true;
        //this.NumberName = "R";
    }

    void Start()
    {
        LabObjectDataFactory.SetResistance(LabObjID, Resistance);
        SetResistanceByType();
    }

    /// <summary>
    ///取元气件速据
    /// </summary>
    public override LabObject GetCircuitData()
    {
        CircuitObject Info = new CircuitObject(ID, LabObjectType, Position);
        Dictionary<string, string> list = new Dictionary<string, string>();
        list.Add("m_iType", m_iType.ToString());
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
    /// <summary>
    /// 根据电阻丝的类型设置电阻值（策划案）
    /// </summary>
    private void SetResistanceByType()
    {
        switch(m_iType)
        {
            case 1:
                Resistance = 4.4f;
                break;
            case 2:
                Resistance = 2.2f;
                break;
            case 3:
                Resistance = 10f;
                break;
            case 4:
                Resistance = 5f;
                break;

            default:
                Resistance = float.MaxValue;
                break;
        }
    }
}
