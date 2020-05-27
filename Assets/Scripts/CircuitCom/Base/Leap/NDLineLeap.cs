using UnityEngine;
using System.Collections;
using System.Collections.Generic ;


#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
///  导线接线头
/// </summary>
/// <author>zhulin</author>
/// <reviser >QFord</reviser >
public class NDLineLeap : NDleapObject {



	public  NDCircuitLeap Link
	{
		get{ return m_LinkLeap;}
		set{ m_LinkLeap = value;}
	}

	public  int LinkCuiCircuitObjectID
	{
		get{ 
			if (Link == null) return -1;
			else return Link.CircuitObjectID;
		}
	}

	public  int LinkLeapIndex
	{
		get{ 
			if (Link == null)return -1;
			else return Link.LeapIndex;
		}
	}

    private NDCircuitLeap m_LinkLeap = null;		

    
	public void SetBreakPos(Transform s)
	{
		Vector3 v = s.position;
		v.y += 2.0f;
		transform.position = v;
	}

	/// <summary>
	/// 检测线是否连接的是接线柱
	/// </summary>
	public void ShowCanLinkLeap()
	{
		NDCircuitLeap leap = null;
		if (Link == null) 
		{
			leap = FindNearLeap ();
		}
		if( leap != null)
		{
			leap.ShowCylinder (true);
		}
	}

	/// <summary>
	/// 查找附近的导线接线头
	/// </summary>
	public NDCircuitLeap FindNearLeap()
	{
        float distance = ConstantData.m_fHighlightDistance;
		List<NDCircuitLeap> l = GetAllLeap();
		NDCircuitLeap near = null;
		foreach (NDCircuitLeap v in l) 
		{
			if (v == null)
				continue;
            //forbidden the two LineLeap of line link to one circutLeap.  link to one eleMent by identical type of CircuitLeapType is also not allowed.
            EleLine line = m_Parent as EleLine;
            if(line.StartLineLeap.Link != null)
            {
                if (v.m_Parent == line.StartLineLeap.Link.m_Parent && v.m_Type == line.StartLineLeap.Link.m_Type) continue;
            }
            else if (line.EndLineLeap.Link != null)
            {
                if (v.m_Parent == line.EndLineLeap.Link.m_Parent && v.m_Type == line.EndLineLeap.Link.m_Type) continue;
            }
            
			float d = CalcDistance (v, this);
			if (d <= distance) 
			{
				near = v;
				distance = d;
			}
		}
		return near;
	}

	/// <summary>
	/// 吸附元器件
	/// </summary>
	public void AdsorbentLeap()
	{
		if (Link == null) 
		{
			Link = FindNearLeap (); 
			if(Link != null)
			{
				transform.position = Link.transform.position;
				Link.AddLinkLine (m_Parent as EleLine);
                Link.ShowCylinder(false);
			}
		}
	}
 

	void OnDrawGizmos()
	{
	//	Gizmos.color = Color.yellow;
	//	Vector3 pos = transform.position;
	//	pos.y += 0.2f;
	//	DrawUnit (pos, "LeapIndex: " + LeapIndex);
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

}


