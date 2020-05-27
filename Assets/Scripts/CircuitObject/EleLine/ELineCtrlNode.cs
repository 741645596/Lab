using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
/// <summary>
///  line leap base
/// </summary>
/// <author>zhulin</author>
/// 
public class ELineCtrlNode : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{

    public LineAction m_lineAction = null;
    public MeshRenderer mRender = null;
    public CtrlNodeType type;
    /// <summary>
    /// if midNode move then OptimezationLine is false
    /// </summary>
    bool bMidNodeChange = false;

    LabCamera m_camera;
    void Awake()
    {
        m_camera = LabEnv.GetCameraByType(CameraType.WireCamera);
    }
    public MeshRenderer GetRender()
    {
        return mRender;
    }

    public void SetMid(Transform s, Transform e)
    {
        if (bMidNodeChange == true) return;
        Vector3 v = (s.position + e.position) / 2;
        //v.y = 5.0f;
        transform.position = v;
    }
    public void SetMidChange()
    {
        if (type == CtrlNodeType.Mid)
            bMidNodeChange = true;
    }

    public void SetBreakPos(Transform s)
    {
        Vector3 v = s.position;
        v.y += 1.0f;
        transform.position = v;
    }

    public void OnDrag(PointerEventData eventData)
    {
        NDLineLeap leap = GetComponent<NDLineLeap>();
        if (leap != null && leap.Link != null)
        {
            if (m_lineAction.m_line != null)
            {
                m_lineAction.SetControling();
                leap.Link.BreakLinkLine(m_lineAction.m_line.LabObjID);
            }
        }

        if (leap != null)
        {
            leap.ShowCanLinkLeap();
        }

        Vector3 globalMousePos;
        if (LabEnv.NodeLab.transform != null && m_camera != null)
        {
            RectTransform m_DraggingPlane = LabEnv.NodeLab.transform as RectTransform;
            if (m_camera.ScreenPointToWorldPointInRectangle(m_DraggingPlane, eventData.position, out globalMousePos))
            {
                if (m_lineAction != null)
                {
                    m_lineAction.SetControling();
                    transform.position = globalMousePos;
                }
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        LabObjectDragMove.LabObjectIsDrag = false;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        LabObjectDragMove.LabObjectIsDrag = true;
    }
    public void SetTransform(PositionInfo Info)
    {
        transform.position = Info.GetPos();
        transform.localScale = Info.GetScale();
    }
}


/// <summary>
/// 导线的三个控制点
/// </summary>
public enum CtrlNodeType
{
    /// <summary>
    /// 起点
    /// </summary>
    Start = 0,  //start
    /// <summary>
    /// 中点
    /// </summary>
    Mid = 1,    //mid1
    /// <summary>
    /// 终点
    /// </summary>
    End = 2,     //en
}
