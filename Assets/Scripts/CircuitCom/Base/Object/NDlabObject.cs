using UnityEngine;
using System.Collections;
using System.Collections.Generic ;
/// <summary>
///  LabObject表现
/// </summary>
/// <author>zhulin</author>
public class NDlabObject : MonoBehaviour {
	/// <summary>
	///路元件ID,用于构建力需要
	/// </summary>
	protected int ID;
	public int LabObjID
	{
		get{ return ID;}
	}

	private static int g_AllocationID = 0;
	public  static int LabAllocationID
	{
		get{ return g_AllocationID;}
		set{  g_AllocationID = value;}
	}
	public PositionInfo Position
	{
		get{ return new PositionInfo (transform);} 
	}

	public int LabObjectType
	{
		get{ return (int)GetLabObjType();} 
	}


	private bool IsPlayerState = false;
	public bool PlayerState
	{
		get{ return IsPlayerState;} 
		set{ IsPlayerState = value;}
	}
    /// <summary>
    ///获取元气件速据（用于保存数据到文件系统需要）
    /// </summary>
    public virtual LabObject GetCircuitData()
    {
        return null;
    }
	/// <summary>
	/// 获取类型
	/// </summary>
	public virtual LabObjType GetLabObjType()
	{
		return LabObjType.LabOBJ;
	}
    /// <summary>
    /// 撤销重做
    /// </summary>
    /// <param name="timeStr"></param>
    /// <param name="info"></param>
    public virtual void ResumeInfo(LabObject info)
    {
        transform.position = info.Position.GetPos();
    }
 
	public void SetTransform(Transform t,PositionInfo Info)
	{
		t.position = Info.GetPos();
		t.localScale = Info.GetScale();
	}

	protected static Dictionary<int ,NDlabObject> g_AllLabObject = new Dictionary<int, NDlabObject> ();
	/// <summary>
	/// 对象通道列表 Layer：ID ，Unit：depth
	/// </summary>
	protected static List<DepthData> g_Depth = new List<DepthData> ();
	/// <summary>
	/// 当前深度
	/// </summary>
	public int m_iDepth   = 1 ;
    /// <summary>
    /// 初始深度值
    /// </summary>
	const int InitDepth = 1;
    /// <summary>
    /// 深度步进值
    /// </summary>
    const int DepthStep = 2;
    /// <summary>
    /// 起始深度值（最小值）
    /// </summary>
    const int MinDepth = -1;
    /// <summary>
    /// 统计历史创建的元件总量
    /// </summary>
    protected static List<NumberNameData> g_lNumberNameData = new List<NumberNameData>();

	public static void ClearAllLabObject()
	{
		g_AllocationID = 0;
		List<NDlabObject> l = new List<NDlabObject> ();
		foreach (NDlabObject obj in g_AllLabObject.Values) 
		{
			l.Add (obj);
		}
		foreach (NDlabObject o in l) 
		{
			o.DestroyLabObject ();
		}

		g_AllLabObject.Clear ();
        g_lNumberNameData.Clear();
		ClearDepth ();
	}

	public virtual void DestroyLabObject()
	{
		g_AllLabObject.Remove (ID);
		LabObjectDataFactory.RemoveCircuit (ID);
		GameObject.Destroy (gameObject);
	}

	protected static void AddLabObj(int ID, NDlabObject obj)
	{
		if (obj == null)
			return;
		if (g_AllLabObject.ContainsKey (ID) == true) 
		{
			Debug.Log ("已经存在");
			return;
		}
		g_AllLabObject.Add (ID ,obj);
        CalcNumberNameData(obj);
	}
    /// <summary>
    /// 获取某个元件类型的创建总量
    /// </summary>
    public static int GetNumberNameTotalCount(NDlabObject obj)
    {
        foreach(var item in g_lNumberNameData)
        {
            if (item.LabObjectType == obj.LabObjectType)
            {
                return item.TotalCount;
            }
        }
        return 0;
    }
    /// <summary>
    /// 记录创建同类型元件的总量
    /// </summary>
    protected static void CalcNumberNameData(NDlabObject obj)
    {
        bool exist = false;
        for (int i = 0; i < g_lNumberNameData.Count;i++ )
        {
            if (g_lNumberNameData[i].LabObjectType == obj.LabObjectType)
            {
                NumberNameData data = new NumberNameData(obj.LabObjectType, g_lNumberNameData[i].TotalCount + 1);
                g_lNumberNameData[i] = data;
                exist = true;
            }
        }
        if (!exist)
        {
            NumberNameData data = new NumberNameData(obj.LabObjectType, 1);
            g_lNumberNameData.Add(data);
        }
    }

	/// <summary>
	/// 根据ID 查找对象
	/// </summary>
	public static NDlabObject FindLabObject(int ID)
	{
		if (g_AllLabObject.ContainsKey (ID) == true)
			return g_AllLabObject [ID];
		else
			return null;
	}

	public static int GetCircuitObjectCount(SearchCicuitType type ,bool IsConnect  = false)
	{
		int count = 0;
		foreach (NDlabObject obj in g_AllLabObject.Values) 
		{
			if (CheckSameType (obj, type ,IsConnect) == true)
				count++;
		}
		return count;
	}



	public static List<NDlabObject> SearchLabObject(SearchCicuitType type ,bool IsConnect  = false )
	{
		List<NDlabObject> l = new List<NDlabObject> ();
		foreach (NDlabObject obj in g_AllLabObject.Values) 
		{
            if (obj == null) continue;
			if (CheckSameType (obj, type ,IsConnect) == true)
				l.Add (obj);
		}
		return l;
	}


	private static bool CheckSameType(NDlabObject obj ,SearchCicuitType type ,bool IsConnect  )
	{
		if (obj == null)
			return false;
		
		if (IsConnect == true) 
		{
			if (obj.CheckConnect () == false)
				return false;
		}

		//类型需匹配
		if (type == SearchCicuitType.All)
			return true;
		else if (type == SearchCicuitType.ELELINE) 
		{
			if (obj is EleLine)
				return true;
		} 
		else if (type == SearchCicuitType.NormalCircuit) 
		{
			bool b = (obj is EleLine);
			if (b == false)
				return true;
		}
		else if (type == SearchCicuitType.Power) 
		{
			if (obj is CurrentSourceElement) 
			{
				return true;
			}
		}
		return false;
	}


	/// <summary>
	/// 元器件有连入电路
	/// </summary>
	/// <returns></returns>
	public virtual bool CheckConnect()
	{
		return false;
	}


	/// <summary>
	/// 是否是有用的元器件
	/// </summary>
	/// <returns></returns>
	public virtual bool Useful()
	{
		return true;
	}


	private bool ContainsKey(int id)
	{
		foreach (DepthData item in g_Depth) 
		{
			if (item.ID == id) {
				return true;
			}
		}
		return false;
	}
    /// <summary>
    /// 值越小，深度越大
    /// </summary>
    /// <returns></returns>
	private int FindTopMostDepth()
	{
        int maxDepth = 0;
		foreach(var item in g_Depth)
        {
            if (item.Depth<maxDepth)
                maxDepth = item.Depth;
        }
		
		return maxDepth;
	}

    private int FindIndex(int id)
    {
        int index = 0;
        foreach(var item in g_Depth)
        {
            if (item.ID == id)
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    private bool AlterDepth(int id, int depth)
    {
        for (int i = 0; i < g_Depth.Count; i++ )
        {
            DepthData item = g_Depth[i];
            if (item.ID == id)
            {
                g_Depth[i] = new DepthData(id, depth);
                return true;
            }
        }
            return false;
    }

    private bool DelDepth(int id, int depth)
    {

        return false;
    }

	/// <summary>
	/// 新增的元器件应该在一个合适的深度(仅在新增时调用,导线不计入！)
	/// </summary>
	protected void SetInitDepth()
	{
		int count = GetCircuitObjectCount(SearchCicuitType.NormalCircuit);

        int depth = ((count - 1)*DepthStep + InitDepth) * -1;//深度值越小越靠前
		m_iDepth = depth;
		transform.localPosition = U3DUtil.SetZ(transform.localPosition, depth);
		if (ContainsKey(ID) )
		{
			return;
		}
		g_Depth.Add(new DepthData(ID,depth));
	}
	/// <summary>
	/// 设置元器件新的深度值，这常常是点选元器件后需要将其置顶
	/// </summary>
	public void SetTopDepth()
	{
		//如果当前没有两个及其以上对象，则不需要设置深度
		if (g_Depth.Count < 2) {
            return;
		}
		//如果是最高那个，则不需要设置深度
        int topMostDepth = FindTopMostDepth();
        if (topMostDepth >= m_iDepth)
        {
            return;
        }
        int index = FindIndex(ID);
        if (index==-1)//如果未找到当前组件，则报出异常
        {
            return;
        }
        int startIndex = MinDepth;
        for(int i = 0;i<g_Depth.Count;i++)//设置其他元器件深度
        {
            DepthData item = g_Depth[i];
            if (item.ID == ID)
            {
                startIndex++;
                continue;
            }
			NDlabObject obj = FindLabObject (item.ID);
			if (obj != null)
			{
				obj.SetDepth(startIndex - i);
			}
        }
        SetDepth(topMostDepth);//设置自身置顶深度
	}
    public void SetDepth(int depth)
    {
        m_iDepth = depth;
        AlterDepth(ID, depth);
        transform.localPosition = U3DUtil.SetZ(transform.localPosition, m_iDepth);
    }
    /// <summary>
    /// 深度清理（比如清空元器件、新建时都要调用此方法）
    /// </summary>
	protected static void ClearDepth()
	{
		g_Depth.Clear ();
	}

	/// <summary>
	/// 分配ID
	/// </summary>
	public static int AllocationID()
	{
		return g_AllocationID++;
	}
}

public struct DepthData
{
    public int ID;
    public int Depth;

    public DepthData(int id, int depth)
    {
        ID = id;
        Depth = depth;
    }
}
/// <summary>
/// 编号数据（元件类型、该类型的总数）
/// </summary>
public struct NumberNameData
{
    /// <summary>
    /// 元件类型
    /// </summary>
    public int LabObjectType;
    /// <summary>
    /// 总数
    /// </summary>
    public int TotalCount;

    public NumberNameData(int type,int count)
    {
        this.LabObjectType = type;
        this.TotalCount = count;
    }
}


public enum SearchCicuitType
{
	ELELINE        = 0 ,  //导线
	NormalCircuit  = 1 ,  //普通元器件
	Power          = 2 ,  //电源
    ImageText      = 3,//插入的图片、文本
	All            = 4 ,  //所有
}


