using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// 电缆
/// </summary>
public class EleLine : NDCircuitObject
{

    public LineAction m_LineAction = null;
    public bool NoneConnectLink
    {
        get { return !StartLink && !EndLink; }
    }
    public bool OneConnectLink
    {
        get { return StartLink || EndLink; }
    }
    public bool ConnectLink
    {
        get { return StartLink && EndLink; }
    }
    public bool StartLink
    {
        get { return StartLineLeap.Link != null; }
    }
    public bool EndLink
    {
        get { return EndLineLeap.Link != null; }
    }


    public NDLineLeap StartLineLeap
    {
        get { return m_HaveLeap[0] as NDLineLeap; }
    }

    public NDLineLeap EndLineLeap
    {
        get { return m_HaveLeap[1] as NDLineLeap; }
    }
    /// <summary>
    /// 是否是有用的元器件
    /// </summary>
    /// <returns></returns>
    public override bool Useful()
    {
        NDCircuitLeap leap1 = StartLineLeap.Link;
        NDCircuitLeap leap2 = EndLineLeap.Link;
        if (leap1 == null)
            return false;
        if (leap2 == null)
            return false;

        if (leap1.m_Parent != leap2.m_Parent)
            return true;
        else
        {
            if (leap1.m_Type == leap2.m_Type)
                return false;
            else
                return true;
        }
    }
    //是否已连接过电路
    public bool IsConnectCircuit
    {
        get { return CircuitM.CheckConnectCircuit(this); }
    }
    /// <summary>
    /// 获取类型
    /// </summary>
    public override LabObjType GetLabObjType()
    {
        return LabObjType.EleLine;
    }


    public void BeakLinkLeap(NDCircuitLeap leap)
    {
        if (leap != null)
        {
            NDLineLeap l = null;
            if (leap == StartLineLeap.Link)
            {
                StartLineLeap.SetBreakPos(leap.transform);
                StartLineLeap.Link = null;
            }
            else if (leap == EndLineLeap.Link)
            {
                EndLineLeap.SetBreakPos(leap.transform);
                EndLineLeap.Link = null;
            }
            leap.RemoveLinkLine(this);
            m_LineAction.SetControling();
        }
    }
    public void RemoveLinkLeap(NDCircuitLeap leap)
    {
        if (StartLineLeap.Link != null && StartLineLeap.Link == leap)
        {
            StartLineLeap.Link = null;
        }
        else if (EndLineLeap.Link != null && EndLineLeap.Link == leap)
        {
            EndLineLeap.Link = null;
        }
    }

    /// <summary>
    /// 元器件有连入电路
    /// </summary>
    /// <returns></returns>
    public override bool CheckConnect()
    {
        return ConnectLink;
    }
    /// <summary>
    /// 获取导线的另一端接头
    /// </summary>
    public NDCircuitLeap GetOtherElementLeap(NDCircuitLeap leap)
    {
        if (leap == null)
            return null;
        if (leap == StartLineLeap.Link)
            return EndLineLeap.Link;
        else if (leap == EndLineLeap.Link)
            return StartLineLeap.Link;
        else
            return null;
    }
    /// <summary>
    /// 查找鼠标靠近的接头
    /// </summary>
    public NDCircuitLeap FindNearLineLeap()
    {
        if (NoneConnectLink) return null;
        float maxdistance = 80.0f;


        if (StartLink && !EndLink)
        {
            float distance1 = StartLineLeap.Link.CalcScreenDistance();
            float distance2 = EndLineLeap.CalcScreenDistance();
            if (distance1 < distance2 && distance1 < maxdistance)
            {
                return StartLineLeap.Link;
            }
        }
        else if (!StartLink && EndLink)
        {
            float distance1 = EndLineLeap.Link.CalcScreenDistance();
            float distance2 = StartLineLeap.CalcScreenDistance();
            if (distance1 < distance2 && distance1 < maxdistance)
            {
                return EndLineLeap.Link;
            }
        }
        else if (ConnectLink)
        {
            float distance1 = StartLineLeap.Link.CalcScreenDistance();
            float distance2 = EndLineLeap.Link.CalcScreenDistance();

            if (distance1 < distance2)
            {
                if (distance1 < maxdistance)
                    return StartLineLeap.Link;
            }
            else
            {
                if (distance2 < maxdistance)
                    return EndLineLeap.Link;
            }
        }
        return null;
    }
    /// <summary>
    /// 吸附元器件
    /// </summary>
    public void AdsorbentLeap()
    {
        if (StartLineLeap.Link == null)
        {
            StartLineLeap.AdsorbentLeap();

        }
        //end 点进行吸附
        if (EndLineLeap.Link == null)
        {
            EndLineLeap.AdsorbentLeap();
        }
    }

    void OnDrawGizmos()
    {
        if (m_LineAction != null && m_LineAction.m_MidCtrl != null)
        {
            Vector3 pos = m_LineAction.m_MidCtrl.transform.position;
            DrawUnit(pos, "ID: " + LabObjID);

            if (StartLink == true)
            {
                pos.y += 0.2f;
                DrawUnit(pos, "StartLink: " + StartLineLeap.Link.CircuitObjectID);
            }

            if (EndLink == true)
            {
                pos.y += 0.2f;
                DrawUnit(pos, "EndLink: " + EndLineLeap.Link.CircuitObjectID);
            }
        }
    }

    protected override void DrawUnit(Vector3 pos, string Text)
    {
        Gizmos.DrawSphere(pos, 0.05f);
#if UNITY_EDITOR
        Vector3 pos1 = pos;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.blue;
        pos1.y += 0.1f;
        pos1.x += 0.1f;
        Handles.Label(pos1, Text, style);
#endif
    }
    /// <summary>
    ///取元气件速据
    /// </summary>
    public override LabObject GetCircuitData()
    {
        CircuitObject Info = new CircuitObject(ID, LabObjectType, Position);
        List<PositionInfo> l = new List<PositionInfo>();
        l.AddRange(m_LineAction.GetCtrlPositionInfo());
        Info.SetCtrlInfo(l);

        List<LeapObject> ll = new List<LeapObject>();
        ll.Add(new LeapObject(StartLineLeap.LeapIndex, StartLineLeap.LinkCuiCircuitObjectID, StartLineLeap.LinkLeapIndex));
        ll.Add(new LeapObject(EndLineLeap.LeapIndex, EndLineLeap.LinkCuiCircuitObjectID, EndLineLeap.LinkLeapIndex));
        Info.SetLinkInfo(ll);

        return Info;
    }
    
    private void SetLinkInfo(CircuitObject Info)
    {
        m_LineAction.SetCtrlPositionInfo(Info.GetCtrlInfo());

        foreach (NDleapObject leap in m_HaveLeap)
        {
            if (leap == null)
                continue;
            LeapObject obj = Info.GetLinkInfo(leap.LeapIndex);
            if (obj != null)
            {
                //link
                NDlabObject lab = NDlabObject.FindLabObject(obj.linkCircuitObjectID);
                if (lab != null && lab is NDCircuitObject)
                {
                    NDleapObject leapobj = (lab as NDCircuitObject).FindLeap(obj.linkCircuitLeapIndex);
                    if (leapobj != null && leapobj is NDCircuitLeap)
                    {
                        (leap as NDLineLeap).Link = (leapobj as NDCircuitLeap);
                        (leapobj as NDCircuitLeap).AddLinkLine(this);
                    }
                }
            }
        }
    }
    /// <summary>
    ///设置元气件速据 （用于从文件系统还原数据）开关没特殊属性
    /// </summary>
    public override void SetData(CircuitObject Info)
    {
        base.SetData(Info);
        SetLinkInfo(Info);

    }
    public override void ResumeInfo(LabObject Info)
    {
        base.ResumeInfo(Info);
        if (Info != null)
        {
            SetLinkInfo(Info as CircuitObject);
        }
    }
}
