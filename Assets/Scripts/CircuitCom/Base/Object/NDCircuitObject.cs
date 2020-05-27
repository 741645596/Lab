using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SharpCircuit;


#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
///  Circuit object base
/// </summary>
/// <author>zhulin</author>
public class NDCircuitObject : NDlabObject
{
    /// <summary>
    /// 拥有的接线口。
    /// </summary>
    public List<NDleapObject> m_HaveLeap = new List<NDleapObject>();

    private float m_runVoltage = 0.0f;
    private float m_runCurrent = 0.0f;
    private float m_runPower = 0.0f;

    /// <summary>
    /// 表现材质
    /// </summary>
    public Transform ShowMaterial;
    /// <summary>
    /// 表现贴图 0:完好，1:损毁(应与LabObjectState枚举一一对应)
    /// </summary>
    public Texture[] ShowTextures;

    public TextMesh NumberNameText;
    /// <summary>
    /// 该元器件是否启用编号
    /// </summary>
    public bool EnableNumber
    {
        get;
        set;
    }
    /// <summary>
    /// 编号名称
    /// </summary>
    private string m_numberName;
    /// <summary>
    /// 编号名称设定
    /// </summary>
    public string NumberName
    {
        get
        {
            return m_numberName;
        }
        set
        {
            if (m_numberName != value)
            {
                int count = (NDlabObject.GetNumberNameTotalCount(this) + 1);
                m_numberName = value + count;
                if (NumberNameText != null)
                {
                    NumberNameText.text = value + HtmlMarkUp.GetFontSizeString(count.ToString(), 10);
                    U3DUtil.SetActive(NumberNameText.gameObject, EnableNumber);

                }

            }
        }
    }

    public void SetGameobjectName()
    {
        if (m_numberName != "")
        {
            gameObject.name += m_numberName;
        }
    }

    protected LabObjectState m_labObjectState = LabObjectState.Good;
    public LabObjectState State
    {
        get { return m_labObjectState; }
        set
        {
            if (m_labObjectState != value)
            {
                m_labObjectState = value;
                DoStateAction(m_labObjectState);
                if (State == LabObjectState.Broken)
                {
                    CreateCirCuit();
                }
            }
        }
    }
    
    /// <summary>
    ///设置元气件速据 （用于从文件系统还原数据）
    /// </summary>
    public virtual void SetData(CircuitObject Info)
    {
        SetTransform(transform, Info.Position);
        ID = Info.ID;
        if (ID >= LabAllocationID)
        {
            LabAllocationID = ID + 1;
        }
        JoinLabObject();
    }



    public virtual void SetDefaultData()
    {
        ID = AllocationID();
        JoinLabObject();
    }


    protected void JoinLabObject()
    {
        NDlabObject.AddLabObj(ID, this);
        LabObjectDataFactory.AddCircuit(ID, GetLabObjType());
        if (!(this is EleLine))//导线不加入元器件层级管理
        {
            SetInitDepth();
        }
    }

    public virtual void DoStateAction(LabObjectState state)
    {
        //设置完好或损毁表现
        if (ShowMaterial == null || ShowTextures.Length != Enum.GetNames(typeof(LabObjectState)).Length)
            return;
        U3DUtil.ChangeTexture(ShowMaterial, ShowTextures[(int)state]);
    }

    /// <summary>
    /// 构建电路
    /// </summary>
    public void JionCircuit(ref Circuit sim)
    {
        foreach (NDCircuitLeap leap in m_HaveLeap)
        {
            if (leap == null || leap.CheckLinkLine() == false)
                continue;
            else
                leap.JionCircuit(ref sim);
        }
    }

    /// <summary>
    /// 根据类型查找接线柱
    /// </summary>
    /// <returns></returns>
    public List<NDCircuitLeap> SearchLeap(ElementLeapType Type)
    {
        List<NDCircuitLeap> l = new List<NDCircuitLeap>();
        foreach (NDCircuitLeap leap in m_HaveLeap)
        {
            if (leap == null)
                continue;
            if (leap.m_Type == Type)
            {
                l.Add(leap);
            }
        }
        return l;
    }

    /// <summary>
    /// 获取正极或负极 连接的所有导线
    /// </summary>
    /// <returns></returns>
    public List<EleLine> GetLeapLinkLine(ElementLeapType Type)
    {
        //先获取所有正极或负极的接线柱
        List<NDCircuitLeap> l = SearchLeap(Type);

        List<EleLine> lline = new List<EleLine>();
        foreach (NDCircuitLeap leap in l)
        {
            if (leap == null)
                continue;
            foreach (EleLine line in leap.LinkLine)
            {
                if (line == null)
                    continue;
                lline.Add(line);
            }
        }
        return lline;
    }


    public NDleapObject FindLeap(int leapIndex)
    {
        foreach (NDleapObject leap in m_HaveLeap)
        {
            if (leap.LeapIndex == leapIndex)
                return leap;
        }
        return null;
    }
    /// <summary>
    /// 元器件有连入电路
    /// </summary>
    /// <returns></returns>
    public override bool CheckConnect()
    {
        //坏掉也不具有连通性
        //临时屏蔽
        if (State == LabObjectState.Broken)
            return false;
        bool Isleadout = false;
        bool IsleadIn = false;
        foreach (NDleapObject leap in m_HaveLeap)
        {
            if (leap != null && leap is NDCircuitLeap)
            {
                NDCircuitLeap l = leap as NDCircuitLeap;
                if (l.HaveLine == false)
                    continue;
                bool lineIn = CheckLineInConnect(leap);
                if (l.m_Type == ElementLeapType.leadOut)
                    Isleadout = true && lineIn;
                if (l.m_Type == ElementLeapType.leadIn)
                    IsleadIn = true && lineIn;
            }
        }
        return Isleadout && IsleadIn;
    }

    public bool CheckLineInConnect(NDleapObject leap)
    {
        if (leap != null && leap is NDCircuitLeap)
        {
            NDCircuitLeap l = leap as NDCircuitLeap;
            if (l.HaveLine == false)
                return false;
            foreach (EleLine line in l.LinkLine)
            {
                if (line.ConnectLink)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 是否是有用的元器件
    /// </summary>
    /// <returns></returns>
    public override bool Useful()
    {
        int leadoutCount = 0;
        int leadInCount = 0;


        return true;
    }
    /// <summary>
    /// 是否是联通的
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckConnectLoop()
    {
        return CircuitBFS.CheckloopConnectBFS(new CircuitNode(LabObjID, ElementLeapType.leadOut), new CircuitNode(LabObjID, ElementLeapType.leadIn));
    }
    /// <summary>
    /// 修改电路属性
    /// </summary>
    protected void ModifyCirCuit()
    {
        //CreateCirCuit();
        //return;
        List<NDlabObject> l = SearchLabObject(SearchCicuitType.NormalCircuit,false);
        foreach (NDlabObject o in l)
        {
            LabObjectDataFactory.ResetCircuitRunData(o.LabObjID);
        }

        CircuitM.UpdataCircuit();

        foreach (NDlabObject o in l)
        {
            if (o != null)
                (o as NDCircuitObject).CalcCircuit();
        }
    }
    /// <summary>
    /// 创建电路
    /// </summary>
    public static void CreateCirCuit()
    {
        List<NDlabObject> l = SearchLabObject(SearchCicuitType.NormalCircuit, false);
        foreach (NDlabObject o in l)
        {
            LabObjectDataFactory.ResetCircuitRunData(o.LabObjID);
        }

        CircuitM.CreateCircuit();
        foreach (NDlabObject o in l)
        {
            if (o != null)
                (o as NDCircuitObject).CalcCircuit();
        }
    }

    private void CalcCircuit()
    {
        if (CircuitM.g_IsCreateCircuit == true && CheckConnect() == true)
        {
            LabObjectDataFactory.GetCircuitRunData(LabObjID, ref m_runCurrent, ref m_runVoltage, ref m_runPower);
        }
        else
        {
            m_runCurrent = 0;
            m_runVoltage = 0;
            m_runPower = 0;
        }
        DoLabAction(m_runVoltage, m_runCurrent, m_runPower);
    }

    public virtual void DoLabAction(float RunVoltage, float RunCurrent, float RunPower)
    {

    }

    public override void DestroyLabObject()
    {
        base.DestroyLabObject();
    }

    void OnDrawGizmos()
    {
        if ((this is EleLine) == false)
        {
            Gizmos.color = Color.green;
            Vector3 pos = transform.position;
            pos.y += 0.2f;
            DrawUnit(pos, "ID: " + LabObjID);
            pos.y += 0.2f;
            DrawUnit(pos, "RunVoltage: " + m_runVoltage);
            pos.y += 0.2f;
            DrawUnit(pos, "RunCurrent: " + m_runCurrent);
        }
        else
        {

        }
    }
    protected virtual void DrawUnit(Vector3 pos, string Text)
    {
        Gizmos.DrawSphere(pos, 0.05f);
#if UNITY_EDITOR
        Vector3 pos1 = pos;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        pos1.y += 0.1f;
        pos1.x += 0.1f;
        Handles.Label(pos1, Text, style);
#endif
    }
}



public enum LabObjectState
{
    /// <summary>
    /// 完好
    /// </summary>
    Good = 0,
    /// <summary>
    /// 损毁
    /// </summary>
    Broken = 1,

}