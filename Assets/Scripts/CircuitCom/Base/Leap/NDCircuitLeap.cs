using UnityEngine;
using System.Collections;
using System.Collections.Generic ;
using SharpCircuit;


#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
///  元器件接线头
/// </summary>
/// <author>zhulin</author>
public class NDCircuitLeap : NDleapObject {

	/// <summary>
	/// 接线口类型
	/// </summary>
	public ElementLeapType m_Type;
	public bool big = false;
    /// <summary>
    /// 绑定接线表现
    /// </summary>
	public GameObject GoBindingPost;
	public GameObject Cylinder;
    /// <summary>
    /// 未绑定接线表现
    /// </summary>
	public GameObject GoNotBindingPost;
    /// <summary>
    /// 是否连有导线
    /// </summary>
	public  bool HaveLine
	{
		get{ return CheckLinkLine();}
	}
	/// <summary>
	/// 接线柱连接的导线
	/// </summary>
	private List<EleLine> m_linkEleLine = new List<EleLine> ();
	public  List<EleLine> LinkLine
	{
		get { return m_linkEleLine;}
	}
	private Vector3 headpositon = Vector3.zero;
   

	void Start () 
	{
		ShowLeap(HaveLine);
		ShowCylinder(false);
		AddElementLeap (this);
	}


	void Update()
	{
#if UNITY_ANDROID
        if (Cylinder != null && Cylinder.gameObject.activeSelf && LabObjectDragMove.LabObjectIsDrag == false)
        {
            ShowCylinder(false);
        }
#endif
        if (CalcScreenDistance() > ConstantData.m_fHighlightDistance)
        {
            ShowCylinder(false);
        }
	}
	/// <summary>
	/// 开启或隐藏接线柱
	/// </summary>
	/// <param name="bActive"></param>
	private void ShowLeap(bool bShow)
	{
		if (GoBindingPost != null && GoNotBindingPost != null)
		{
			GoBindingPost.SetActive(bShow);
			GoNotBindingPost.SetActive(!bShow);
		}

	}
    /// <summary>
    /// 开启或隐藏可以连接高亮闪烁表现
    /// </summary>
	public void ShowCylinder(bool bShow)
	{
		if (Cylinder!=null)
		{
			if (bShow)
			{
				Cylinder.SetActive(true);
			}
			else
				Cylinder.SetActive(false);
		}
	}
    /// <summary>
    /// 断开所有导线
    /// </summary>
	public void BreakLinkLine()
	{
		if (m_linkEleLine == null || m_linkEleLine.Count == 0)
			return;
		List<EleLine> EleLineList = new List<EleLine>();
		EleLineList.AddRange(m_linkEleLine);
		foreach (EleLine line in EleLineList)
		{
			if (line != null)
			{
				line.BeakLinkLeap(this);
			}
		}
		ShowCylinder(false);
	}

    public void BreakLinkLine(int lineID)
    {
        if (m_linkEleLine == null || m_linkEleLine.Count == 0)
            return;
        List<EleLine> EleLineList = new List<EleLine>();
        EleLineList.AddRange(m_linkEleLine);
        foreach (EleLine line in EleLineList)
        {
            if (line != null && line.LabObjID == lineID)
            {
				line.BeakLinkLeap(this);
            }
        }
        ShowCylinder(false);
    }
	/// <summary>
	/// 构建电路
	/// </summary>
	public void JionCircuit(ref Circuit  sim)
	{
		if (sim == null) return;
		foreach (EleLine line in m_linkEleLine) 
		{
            if (line == null || line.ConnectLink != true)
				continue;
			NDCircuitLeap other = line.GetOtherElementLeap (this);
			if (other == null || other.m_Parent == null)
				continue;

			CircuitElement myCircuit = LabObjectDataFactory.GetCircuit( m_Parent.LabObjID);
			CircuitElement OtherCircuit = LabObjectDataFactory.GetCircuit (other.m_Parent.LabObjID);


			if (myCircuit == null || OtherCircuit == null)
				return;

			if (m_Type == ElementLeapType.leadOut) 
			{
				if(other.m_Type == ElementLeapType.leadIn)
				{
					sim.Connect(myCircuit.leadOut,OtherCircuit.leadIn);
					Debug.Log ("["+ m_Parent.LabObjID +"]"+ "[leadOut]" + "------->" + "[" + other.m_Parent.LabObjID + "]" + "[leadIn]" );
				}
				else
				{
					sim.Connect(myCircuit.leadOut, OtherCircuit.leadOut);
					Debug.Log ("["+ m_Parent.LabObjID +"]"+ "[leadOut]" + "------->" + "[" + other.m_Parent.LabObjID + "]" + "[leadOut]" );
				}
			} 
			else 
			{
				if (other.m_Type == ElementLeapType.leadIn) 
				{
					sim.Connect (myCircuit.leadIn, OtherCircuit.leadIn);
					Debug.Log ("["+ m_Parent.LabObjID +"]"+ "[leadIn]" + "------->" + "[" + other.m_Parent.LabObjID + "]" + "[leadIn]" );
				} 
				else 
				{
					sim.Connect (myCircuit.leadIn, OtherCircuit.leadOut);
					Debug.Log ("["+ m_Parent.LabObjID +"]"+ "[leadIn]" + "------->" + "[" + other.m_Parent.LabObjID + "]" + "[leadOut]" );
				}
			}
		}
	}
	/// <summary>
	/// 获取连接的线
	/// </summary>
	/// <returns></returns>
	public List<EleLine> GetLinkLine()
	{
		List<EleLine> l = new List<EleLine>();
		foreach (EleLine line in m_linkEleLine) 
		{
			if (line != null) 
			{
				l.Add (line);
			}
		}
		return l;
	}
	/// <summary>
	/// 连接导线
	/// </summary>
	public void AddLinkLine(EleLine line)
	{
		if (line == null)
			return;
		if (m_linkEleLine.Contains (line) == false) 
		{
			m_linkEleLine.Add (line);
		}
		bool IslinkLine = CheckLinkLine ();
		ShowLeap (IslinkLine);

		NDCircuitObject.CreateCirCuit ();
	}
	/// <summary>
	/// 移除导线
	/// </summary>
	public void RemoveLinkLine(EleLine line)
	{
		if (line == null)
			return;
		if (m_linkEleLine.Contains (line) == true) 
		{
			m_linkEleLine.Remove (line);
		}
        //line.RemoveLinkLeap(this);

		bool IslinkLine = CheckLinkLine ();
		ShowLeap (IslinkLine);
		NDCircuitObject.CreateCirCuit ();
	}
	/// <summary>
	/// 判断是否连有导线
	/// </summary>
	public bool CheckLinkLine()
	{
		if (m_linkEleLine == null || m_linkEleLine.Count == 0)
			return false;
		foreach (EleLine line in m_linkEleLine) 
		{
			if (line != null)
				return true;
		}
		return false;
	}

	public static NDCircuitLeap FindNearLeap()
	{
        float distance = ConstantData.m_fHighlightDistance;
		NDCircuitLeap near = null;
		List<NDCircuitLeap> l = GetAllLeap ();
		foreach (NDCircuitLeap v in l)
		{
			if (v == null)
				continue;
			float d = v.ScreenDistance;
			if (d <= distance)
			{
				near = v;
				distance = d;
			}
		}
		return near;
	}


	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Vector3 pos = transform.position;
		pos.y += 0.2f;
		DrawUnit (pos, "LeapIndex: " + LeapIndex);
		pos.y += 0.2f;
		DrawUnit (pos, "Type: " + m_Type);
	}

	private void DrawUnit(Vector3 pos ,string Text)
	{
		Gizmos.DrawSphere(pos, 0.05f);
#if UNITY_EDITOR
		Vector3 pos1 = pos;
		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.blue;
		pos1.y += 0.1f;
		pos1.x += 0.1f;
		Handles.Label(pos1, Text,style);
#endif
	}



	void OnDestroy()
	{
		RemoveElementLeap (this);
	}
}

/// <summary>
/// 元件类型
/// </summary>
public enum ElementLeapType
{
	/// <summary>
	/// 正极
	/// </summary>
	leadOut = 0 , 
	/// <summary>
	/// 负极
	/// </summary>
	leadIn  = 1 ,
}
