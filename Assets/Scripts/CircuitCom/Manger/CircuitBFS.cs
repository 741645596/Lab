using System.Collections.Generic ;


/// <summary>
/// 广度优先搜索算法
/// </summary>
/// <returns></returns>
public class CircuitBFS  {


	//搜索用过的绳子
	private static List<EleLine> g_UserLineBFS = new List<EleLine>();
	/// <summary>
	/// 广度优先确认能否联通
	/// </summary>
	public static bool CheckloopConnectBFS(CircuitNode Start, CircuitNode End)
	{
		if (InitBFS (Start, End) == false)
			return false;
        //1.获取周边导线
		List<CircuitNodeLine> llineNode = NodeLinkLine(Start);

		while (true) 
		{
			//判断有没链接导线
			if (llineNode.Count == 0)
				return false;
			//继续搜索
			List<CircuitNode> lNode = GetLinkOtherNode (llineNode); 
			//找到目标，成功
			if (CircuitNode.CheckFindTarget (lNode, End) == true)
				return true;
			//更新使用过的导线
			UpdateUserLine(ref llineNode);
			//继续搜索周边结点
			llineNode = NodeLinkLine(lNode);
		}
		return false;
	}

	/// <summary>
	/// 广度优先确认能否联通
	/// </summary>
	private static bool InitBFS(CircuitNode Start, CircuitNode End)
	{
		if (Start == null || End == null)
			return false;
		//初始化工作
		if (CircuitNode.Compare (Start, End) == true)
			return false;
		g_UserLineBFS.Clear ();
		return true;
	}
	/// <summary>
	/// 获取连接的导线,
	/// </summary>
	private static List<CircuitNodeLine> NodeLinkLine(CircuitNode Node)
	{
		List<CircuitNodeLine> lNodeLine = new List<CircuitNodeLine>();
		if (Node == null)
			return lNodeLine;
		//获取连接的导线
		lNodeLine = Node.GetLinkLineNode ();
		//移除用过的导线
		foreach (EleLine v in g_UserLineBFS) 
		{
			//删除已经拥有的。
			foreach(CircuitNodeLine vn in lNodeLine)
			{
				if(vn.Line == v)
				{
					lNodeLine.Remove(vn);
					break ;
				}
			}
		}
		return lNodeLine;
	}
	/// <summary>
	/// 获取连接的导线,
	/// </summary>
	private static List<CircuitNodeLine> NodeLinkLine(List<CircuitNode> lNode)
	{
		List<CircuitNodeLine> lNodeLine = new List<CircuitNodeLine>();

		foreach (CircuitNode node in lNode) 
		{
			List<CircuitNodeLine> l = NodeLinkLine (node);
			CircuitNodeLine.CombinNodeLine (ref lNodeLine, l);
		}
		return lNodeLine;	
	}

	/// <summary>
	/// 获取导线的连接连接结点
	/// </summary>
	private static List<CircuitNode> GetLinkOtherNode(List<CircuitNodeLine> lLineNode) 
	{
		List<CircuitNode> lNode = new List<CircuitNode> ();
		foreach (CircuitNodeLine v in lLineNode) 
		{
			if (v == null)
				continue;
			//连接到一个元器件的ID,并需把2个端口都考虑进去
			int ObjID = v.GetLinkOtherObjID ();
			CircuitNode n1 = new CircuitNode (ObjID ,ElementLeapType.leadIn);
			if (lNode.Contains (n1) == false) 
			{
				lNode.Add (n1);
			}

			CircuitNode n2 = new CircuitNode (ObjID ,ElementLeapType.leadOut);
			if (lNode.Contains (n2) == false) 
			{
				lNode.Add (n2);
			}

		}
		return lNode;
	}

	/// <summary>
	/// 更新使用过的导线列表
	/// </summary>
	private static void UpdateUserLine(ref List<CircuitNodeLine> lLineNode) 
	{
		if (lLineNode == null)
			return;
		
		foreach (CircuitNodeLine  v in lLineNode) 
		{
			if (v == null || v.Line == null)
				continue;

			if (g_UserLineBFS.Contains (v.Line) == false) 
			{
				g_UserLineBFS.Add (v.Line);
			}

		}
        
		lLineNode.Clear ();
	}
}


/// <summary>
/// 电路结点
/// </summary>
public class CircuitNode  
{
	public int LabID ;
	public ElementLeapType Type;


	public CircuitNode(){}
	public CircuitNode(int LabID,ElementLeapType Type)
	{
		this.LabID = LabID;
		this.Type = Type;
	}

	/// <summary>
	/// 比较是否相等
	/// </summary>
	public static bool  Compare(CircuitNode Node1 ,CircuitNode Node2)
	{
		if (Node1 == null || Node2 == null)
			return false;
		
		if (Node1.LabID != Node2.LabID)
			return false;

		if (Node1.Type != Node2.Type)
			return false;

		return true;
	}


	/// <summary>
	/// 获取连接的导线,
	/// </summary>
	public List<CircuitNodeLine> GetLinkLineNode()
	{
		List<CircuitNodeLine> lLineNode = new List<CircuitNodeLine>();

		NDlabObject obj = NDlabObject.FindLabObject (this.LabID);
		if (obj == null || (obj is NDCircuitObject) == false)
			return lLineNode;
		
		NDCircuitObject cirobj = (obj as NDCircuitObject);
		List<EleLine> l  =  cirobj.GetLeapLinkLine(this.Type) ;

		foreach (EleLine  v in l) 
		{
			if (v == null)
				continue;

			lLineNode.Add (new CircuitNodeLine (this, v));
		}
		return lLineNode;

	}


	/// <summary>
	/// 列表进行合并
	/// </summary>
	public static bool CheckFindTarget(List<CircuitNode> lHave ,CircuitNode Target)
	{
		if (lHave == null || Target == null)
			return false;

		foreach(CircuitNode v in lHave)
		{
			if (Compare (v, Target) == true)
				return true;
			
		}
		return false;
	}


}



/// <summary>
/// 电路结点
/// </summary>
public class CircuitNodeLine : CircuitNode
{
	public EleLine Line;

	public CircuitNodeLine(){}
	public CircuitNodeLine(CircuitNode Node,EleLine line)
	{
		if (Node != null) 
		{
			this.LabID = Node.LabID;
			this.Type = Node.Type;
			this.Line = line;
		}
	}

	/// <summary>
	/// 获取连接的另外一个端口。
	/// </summary>
	public int GetLinkOtherObjID()
	{
		if(Line == null)
			return -1;
		NDCircuitLeap leap1 = Line.StartLineLeap.Link;
		NDCircuitLeap leap2 = Line.EndLineLeap.Link;
		//必须2端都连接的导线
		if (leap1 != null && leap2 != null)
		{
			if (leap1.m_Parent.LabObjID == leap2.m_Parent.LabObjID)
				return -1;
			else 
			{
				if (leap1.m_Parent.LabObjID == this.LabID && leap1.m_Type == this.Type) 
				{
					return leap2.m_Parent.LabObjID;
				} 
				else if (leap2.m_Parent.LabObjID == this.LabID && leap2.m_Type == this.Type) 
				{
					return leap1.m_Parent.LabObjID;
				}
			}
		}
		return -1;
	}
	/// <summary>
	/// 列表进行合并
	/// </summary>
	public static void CombinNodeLine(ref List<CircuitNodeLine> lHave ,List<CircuitNodeLine> l)
	{
		if (lHave == null || l == null)
			return ;

		foreach(CircuitNodeLine v in l)
		{
			if (v == null)
				continue;
			//添加未添加的。
			bool IsHave = false ;
			foreach(CircuitNodeLine vn in lHave)
			{
				if(vn.Line == v.Line)
				{
					IsHave = true;
					break;
				}
			}
			if (IsHave == false) 
			{
				lHave.Add (v);
			}
		}
	}
}