using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharpCircuit;


/// <summary>
/// 元器件管理
/// </summary>
public class CircuitM
{
    private static Circuit g_sim = new Circuit();
    public static bool g_IsCreateCircuit = false;
    private static List<EleLine> g_HaveConnect = new List<EleLine>();
    /// <summary>
    /// 清理数据
    /// </summary>
    public static void Clear()
    {
        g_IsCreateCircuit = false;
        ClearCircuit();
    }
    /// <summary>
    /// 清理电路
    /// </summary>
    private static void ClearCircuit()
    {
        g_HaveConnect.Clear();
        g_sim.NdClearElements();
    }
    /// <summary>
    /// 构建电路
    /// </summary>
    public static bool CreateCircuit()
    {
        ClearCircuit();
		//
		if (CheckCircuitLoop() == false) 
		{
			Debug.Log ("电路不连通");
			g_IsCreateCircuit = false;
			return false;
		}
		else Debug.Log ("电路连通");
		//
		List<NDlabObject> l = NDlabObject.SearchLabObject (SearchCicuitType.NormalCircuit ,false) ;

        //先加入元气件
        foreach (NDlabObject Lab in l)
        {
            if (Lab == null)
                continue;
            else
            {
				CircuitElement e = LabObjectDataFactory.GetCircuit (Lab.LabObjID);
                if (e != null)
                    g_sim.AddElement(e);
            }

        }
        //元气件进行连接
        foreach (NDlabObject Lab in l)
        {
            if (Lab == null)
                continue;
            else
                (Lab as NDCircuitObject).JionCircuit(ref g_sim);
        }
        //JionCircuit();
        try

        {
            //g_sim.needAnalyze();
			if(g_sim.doTick() == false)
			{
                //g_IsCreateCircuit = false;
				Debug.Log("CreateCircuit is false");
				return false ;
			}
			else
			{
				Debug.Log("CreateCircuit is true");
				g_IsCreateCircuit = true;
			}
        }
        catch (SharpCircuit.Circuit.CircuitException e)
        {
            Debug.Log(e.ToString());
            return false;
        }
        return true;
    }

    public static void JionCircuit()
    {
        List<NDlabObject> lineList = NDlabObject.SearchLabObject(SearchCicuitType.ELELINE, false);
        foreach(NDlabObject obj in lineList)
        {
            if(obj != null && obj is EleLine)
            {
                EleLine eleLine = obj as EleLine;
                if (eleLine.ConnectLink == true)
                {
                    NDlabObject start = eleLine.StartLineLeap.Link.m_Parent;
                    NDlabObject end = eleLine.EndLineLeap.Link.m_Parent;
                    CircuitElement myCircuit = LabObjectDataFactory.GetCircuit(start.LabObjID);
                    CircuitElement OtherCircuit = LabObjectDataFactory.GetCircuit(end.LabObjID);

                    Circuit.Lead startLead = eleLine.StartLineLeap.Link.m_Type == ElementLeapType.leadIn?myCircuit.leadIn:myCircuit.leadOut;
                    Circuit.Lead endLead = eleLine.EndLineLeap.Link.m_Type == ElementLeapType.leadIn ? OtherCircuit.leadIn : OtherCircuit.leadOut;
                    g_sim.Connect(startLead, endLead);
                }
            }
        }

    }

	public static void UpdataCircuit()
	{
        if (g_sim != null)
        {
            g_sim.NDDoticks();
        }
	}
		
    /// <summary>
    /// 电路是否是闭合的。
    /// </summary>
    /// <returns></returns>
	public static bool CheckCircuitLoop()
    {
		//获取联通的电源
		List<NDlabObject> l = NDlabObject.SearchLabObject(SearchCicuitType.Power , true);
		foreach (NDlabObject Lab in l)
		{
			if (Lab == null)
				continue;
			if((Lab as CurrentSourceElement).CheckConnectLoop () == true )
				return true;
		}
        return false;
    }
		

    public static bool CheckConnectCircuit(EleLine line)
    {
        if (line == null)
            return false;
        if (g_HaveConnect.Contains(line) == true)
            return true;
        else
        {
            g_HaveConnect.Add(line);
            return false;
        }
    }




}
