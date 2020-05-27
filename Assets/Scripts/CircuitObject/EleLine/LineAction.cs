using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// 导线线特性
/// </summary>
public class LineAction : MonoBehaviour
{
    //固定的电缆起点
    protected bool m_Selected = false;
    public bool Selected
    {
        get { return m_Selected; }
        set
        {
            m_Selected = value;
        }
    }
    //控制计时
    protected bool m_IsControling = false;
    /// <summary>
    /// 显示圆点时间计数器
    /// </summary>
    protected float m_fShowSphereCounter = 0.0f;
    protected float maxControlTime = 2.0f;
    //选择记时
    protected float m_selectedTime = 0.0f;
    protected float m_selectedHoleTime = 1.5f;

    public LineRenderer Line;
    public float fwidth = 0.1f;

    public ELineCtrlNode m_StartCtrl = null;
    public ELineCtrlNode m_MidCtrl = null;
    public ELineCtrlNode m_EndCtrl = null;


    private Vector3[] pathList;
    private int pointnum = 20;

    public List<Vector3> m_lpoint = new List<Vector3>();

    private NDCircuitLeap m_breakLeap = null;
    //是否优化导线
    public bool IsOptimezationLine = true;


    public EleLine m_line;

    #region Line Add Collider
    //line add collider
    public GameObject ColliderParent = null;
    //collider add to line
    private List<SphereCollider> m_lCollider = new List<SphereCollider>();
    //left point not add collider
    private int nPointNoCollider = 1;
    #endregion

    /// <summary>
    /// only show one line in controlling anytime 
    /// </summary>
    private static LineAction m_ControllingLine = null;

    private float m_fLineMiniDistance;
    private LabCamera wireCamera;

    void Start()
    {
        InitCtrlLine();	
        wireCamera = LabEnv.GetCameraByType(CameraType.WireCamera);
        
    }
    void Update()
    {
        if (IsCanDrawLine() == false)
            return;
        //选择导线
        SelectCtrlLine();
        //导线吸附
        AdsorbentLeap();
        //绘制导线
        DrawLine();
        //绘制控制信息
        DrawCtrlInfo();
    }
    protected void SelectCtrlLine()
    {
        if (LabObjectDragMove.LabObjectIsDrag == true)
            return;
        
#if UNITY_ANDROID 	
        return;
#endif
        if (CheckSelected() == true)
        {
            SetLineControl();
            Selected = true;
            m_selectedTime = 0;
        }
        else
        {
            if (Selected == true)
            {
                m_selectedTime += Time.deltaTime;
                if (m_selectedTime > m_selectedHoleTime)
                {
                    Selected = false;
                    m_selectedTime = 0;
                }
            }
        }
    }

    public void SetLineControl()
    {
        if (m_ControllingLine != null && m_ControllingLine != this)
        {
            //选中新线，旧的需要等候0.1s才能吸附上而且m_control被新线设置后无法吸附，所以这里需要主动调用下
            m_ControllingLine.m_line.AdsorbentLeap(); ;
            m_ControllingLine.StopShowSelectInfo();
        }
        m_ControllingLine = this;
    }
    /// <summary>
    /// 检测能否绘线
    /// </summary>
    public bool IsCanDrawLine()
    {
        if (Line == null) return false;
        if (m_MidCtrl == null) return false;

        if (m_StartCtrl == null && !m_line.StartLink) return false;

        if (m_EndCtrl == null && !m_line.EndLink) return false;

        return true;
    }
    /// <summary>
    /// 获取node
    /// </summary>
    private Transform GetNode(CtrlNodeType Type)
    {
        if (Type == CtrlNodeType.Start)
            return m_StartCtrl.transform;
        if (Type == CtrlNodeType.End)
            return m_EndCtrl.transform;
        if (Type == CtrlNodeType.Mid)
        {
            return m_MidCtrl.transform;
        }
        return null;
    }
    public void InitCtrlLine()
    {
        pathList = new Vector3[3];
        Line.SetVertexCount(pointnum * 3);
        ShowClickSphere(m_IsControling);
    }
    /// <summary>
    /// 判断导线是否被选中
    /// </summary>
    protected bool CheckSelected()
    {
        Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        for (int i = 0; i < m_lpoint.Count; i++)
        {
            Vector3 ScreenSpace = Camera.main.WorldToScreenPoint(m_lpoint[i]);
            if (Vector2.Distance(curScreenSpace, ScreenSpace) < 5f)
            {
                SetControling();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 正在控制导线
    /// </summary>
    public void SetControling()
    {
        m_fShowSphereCounter = 0;
        if (m_IsControling == false)
        {
            m_IsControling = true;
            ShowClickSphere(m_IsControling);
        }
    }
    /// <summary>
    /// 设置两端接线头碰撞位置
    /// </summary>
    public void SetLeapPos()
    {
        if (m_line.StartLineLeap.Link != null)
            m_StartCtrl.transform.position = m_line.StartLineLeap.Link.transform.position;

        if (m_line.EndLineLeap.Link != null)
            m_EndCtrl.transform.position = m_line.EndLineLeap.Link.transform.position;

        OptimezationLine();
    }

    /// <summary>
    /// 更改控制线状态
    /// </summary>
    protected void CountTime(float deltaTime)
    {
        if (LabObjectDragMove.LabObjectIsDrag == true)
        {
            return;
        }
        if (m_IsControling == true && LabObjectDragMove.LabObjectIsDrag == false)
        {
            m_fShowSphereCounter += deltaTime;
            if (m_fShowSphereCounter > maxControlTime)
            {
                ShowClickSphere(false);
                m_IsControling = false;
                m_fShowSphereCounter = 0;
            }
        }
    }

    public void StopShowSelectInfo()
    {
        m_IsControling = false;
        m_fShowSphereCounter = 0;
        ShowClickSphere(false);
        Selected = false;
        m_selectedTime = 0;
    }
    /// <summary>
    /// 检测线是否到最短距离
    /// </summary>
    /// <returns></returns>
    public bool CheckMiniDistance()
    {
        float d = U3DUtil.GetVectorListDistance(m_lpoint);
        if (d <= m_fLineMiniDistance)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 绘制导线
    /// </summary>
    public void DrawLine()
    {
        Vector3 s = m_line.StartLineLeap.Link != null ? m_line.StartLineLeap.Link.transform.position : m_line.StartLineLeap.transform.position;
        Vector3 e = m_line.EndLineLeap.Link != null ? m_line.EndLineLeap.Link.transform.position : m_line.EndLineLeap.transform.position;
        bool change = CheckCtrlChange(s, m_MidCtrl.transform.position, e);

        if (m_lpoint.Count > 0)
        {
            if (change == true)
            {
                Vector3[] resultList = PointController.PointList(pathList, pointnum);
                for (int i = 0; i < resultList.Length; i++)
                {
                    if (i == 0 && m_line.StartLineLeap.Link != null)
                        m_lpoint[0] = m_line.StartLineLeap.Link.transform.position;
                    else if (i == 0 && m_line.StartLineLeap.Link == null)
                        m_lpoint[0] = m_line.StartLineLeap.transform.position;
                    else 
                        m_lpoint[i] = resultList[i];


                }
                if (m_line.EndLineLeap.Link != null)
                    m_lpoint[resultList.Length - 1] = m_line.EndLineLeap.Link.transform.position;
                else
                    m_lpoint[resultList.Length - 1] = m_line.EndLineLeap.transform.position;
                SetColloderPosition();
            }
        }
        else
        {
            Vector3[] resultList = PointController.PointList(pathList, pointnum);
            for (int i = 0; i < resultList.Length; i++)
            {
                m_lpoint.Add(resultList[i]);
            }
            InitCollider();
            m_fLineMiniDistance = U3DUtil.GetVectorListDistance(m_lpoint);
        }
        //if (CheckMiniDistance())
        //{
        //    return;
        //}
        //循环100遍来绘制贝塞尔曲线每个线段
        for (int i = 0; i < m_lpoint.Count; i++)
        {
            Vector3 vec = m_lpoint[i];
            //把每条线段绘制出来 完成白塞尔曲线的绘制
            Line.SetPosition(i, vec);
            Line.SetWidth(fwidth, fwidth);
        }
    }
    /// <summary>
    /// Add collider to line point
    /// </summary>
    void InitCollider()
    {
        ColliderParent = new GameObject("CollParent");
        if (ColliderParent != null)
        {
            ColliderParent.transform.SetParent(transform);
            for (int i = nPointNoCollider; i < m_lpoint.Count - nPointNoCollider; i++)
            {
                SphereCollider col = new GameObject("Collider").AddComponent<SphereCollider>();
                m_lCollider.Add(col);
                col.transform.parent = ColliderParent.transform; // Collider is added as child object of line
            }
        }
        SetColloderPosition();
    }
    void SetColloderPosition()
    {
        for (int i = 0; i < m_lCollider.Count; i++)
        {
            Vector3 startPos = m_lpoint[i + nPointNoCollider];
            Vector3 endPos = m_lpoint[i + nPointNoCollider + 1];
            float lineLength = Vector3.Distance(startPos, endPos); // length of line
            if (lineLength < 0.1f)
                lineLength = 0.1f;
            m_lCollider[i].radius = lineLength;// new Vector3(0.2f, lineLength * 4, 0.2f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
            Vector3 midPoint = (startPos + endPos) / 2;
            m_lCollider[i].transform.position = midPoint; // setting position of collider object
             //Following lines calculate the angle between startPos and endPos
            //float angle = (Mathf.Abs(startPos.y - endPos.y) / Mathf.Abs(startPos.x - endPos.x));
            //if ((startPos.y < endPos.y && startPos.x > endPos.x) || (endPos.y < startPos.y && endPos.x > startPos.x))
            //{
            //    angle *= -1;
            //}
            //angle = Mathf.Rad2Deg * Mathf.Atan(angle);
            //m_lCollider[i].transform.Rotate(0, 0, angle);
        }

    }
    private bool CheckCtrlChange(Vector3 s, Vector3 m, Vector3 e)
    {
        bool change = false;
        if (pathList[0] != s)
        {
            pathList[0] = s;
            change = true;
        }
        if (pathList[1] != m)
        {
            pathList[1] = m;
            change = true;
        }

        if (pathList[2] != e)
        {
            pathList[2] = e;
            change = true;
        }
        return change;
    }


    /// <summary>
    /// 设置电缆控制点
    /// </summary>
    public void SetLineCtrlNode(CtrlNodeType Type, ELineCtrlNode T)
    {
        if (Type == CtrlNodeType.Start)
        {
            m_StartCtrl = T;
        }
        else if (Type == CtrlNodeType.Mid)
        {
            m_MidCtrl = T;
        }
        else if (Type == CtrlNodeType.End)
        {
            m_EndCtrl = T;
        }
    }

    /// <summary>
    /// 获取接线头表现的MeshRenderer
    /// </summary>
    private MeshRenderer GetMeshRender(CtrlNodeType Type)
    {
        if (Type == CtrlNodeType.Start)
        {
            return m_StartCtrl.GetRender();
        }
        else if (Type == CtrlNodeType.End)
        {
            return m_EndCtrl.GetRender();
        }
        else if (Type == CtrlNodeType.Mid)
        {
            return m_MidCtrl.GetRender();
        }
        return null;
    }
    /// <summary>
    /// 设置控制点显隐
    /// </summary>
    private void SetMeshRender(CtrlNodeType Type, bool Show)
    {
        MeshRenderer r = GetMeshRender(Type);
        if (r != null)
            r.enabled = Show;
    }
    /// <summary>
    /// 设置全部控制点显隐
    /// </summary>
    public void ShowClickSphere(bool bShow)
    {
        SetMeshRender(CtrlNodeType.Start, bShow);
        SetMeshRender(CtrlNodeType.Mid, bShow);
        SetMeshRender(CtrlNodeType.End, bShow);
    }

    public void OptimezationLine()
    {
        m_MidCtrl.SetMid(m_StartCtrl.transform, m_EndCtrl.transform);
    }
    public void SetMidChange()
    {
        m_MidCtrl.SetMidChange();
    }

    /// <summary>
    /// 吸附元器件
    /// </summary>
    public void AdsorbentLeap()
    {
        //吸附条件，1.必须正在操控
        if (LabObjectDragMove.LabObjectIsDrag)
            return;         
        //鼠标松开一段时间
        if (m_fShowSphereCounter > 0.1f && m_fShowSphereCounter < maxControlTime )
        {
            m_line.AdsorbentLeap();
        }
    }

    public List<PositionInfo> GetCtrlPositionInfo()
    {
        List<PositionInfo> l = new List<PositionInfo>();
        m_StartCtrl.transform.position = m_lpoint[0];
        m_EndCtrl.transform.position = m_lpoint[m_lpoint.Count - 1];
        l.Add(new PositionInfo(m_StartCtrl.transform));
        l.Add(new PositionInfo(m_MidCtrl.transform));
        l.Add(new PositionInfo(m_EndCtrl.transform));
        return l;
    }

    public void SetCtrlPositionInfo(List<PositionInfo> l)
    {
        m_StartCtrl.SetTransform(l[0]);
        m_MidCtrl.SetTransform(l[1]);
        m_EndCtrl.SetTransform(l[2]);
    }

    /// <summary>
    /// 绘制控制信息
    /// </summary>
    public void DrawCtrlInfo()
    {
        CountTime(Time.deltaTime);
    }
}
