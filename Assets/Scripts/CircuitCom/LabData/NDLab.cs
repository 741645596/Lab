//#define DebugMode
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
/// <summary>
/// Circuit Data
/// </summary>
/// <author>zhulin</author>
public class NDLab
{
    private static CircuitData m_lab = new CircuitData();
    /// <summary>
    /// save Circuit Data to path
    /// </summary>
    public static bool SaveCircuitData(string FileName, string TimeStamp = "")
    {
        m_lab.GetLabData();

        FileStream fsFile = new FileStream(FileName, FileMode.Create);
        StreamWriter swWriter = new StreamWriter(fsFile);
        string text = JsonUtility.ToJson(m_lab);
        swWriter.WriteLine(text);
        swWriter.Close();
        return true;
    }
    /// <summary>
    /// load circuti data from path
    /// </summary>
    public static bool LoadCircuitData(string labfile)
    {
        try
        {
            FileStream fsFile = new FileStream(labfile, FileMode.Open);
            StreamReader srReader = new StreamReader(fsFile);
            string Text = srReader.ReadToEnd();
            srReader.Close();
            m_lab.Clear();
            LoadCircuitDatafromjson(Text, ref m_lab);
        }
        catch (Exception e)
        {
            throw e;
        }
        return true;
    }
    public static bool LoadCircuitData(string labfile, ref CircuitData lab)
    {
        try
        {
            FileStream fsFile = new FileStream(labfile, FileMode.Open);
            StreamReader srReader = new StreamReader(fsFile);
            string Text = srReader.ReadToEnd();
            srReader.Close();

            LoadCircuitDatafromjson(Text, ref lab);
        }
        catch (Exception e)
        {
            throw e;
        }
        return true;
    }
    /// <summary>
    /// load circuti data from path
    /// </summary>
    public static bool LoadCircuitDatafromjson(string JsonData, ref CircuitData lab)
    {
        if (string.IsNullOrEmpty(JsonData) == true)
            return false;
        lab = JsonUtility.FromJson<CircuitData>(JsonData);
        return true;
    }

    /// <summary>
    /// load 2 scene
    /// </summary>
    public static bool LoadCirCuit2Scene(Transform parent)
    {
        m_lab.LoadCircuitObj(parent);
        return true;
    }

}

[System.Serializable]
public class CircuitObject : LabObject
{
    public string CircuitInfo = "{}";
    public List<PositionInfo> CtrlNode = new List<PositionInfo>();
    public List<LeapObject> ListLeap = new List<LeapObject>();


    public CircuitObject() { }
    public CircuitObject(int ID, int ObjType, PositionInfo Info)
        : base(ID, ObjType, Info)
    {
    }
    /// <summary>
    /// load circuti data from path
    /// </summary>
    public override NDlabObject LoadPerfab(Transform parent)
    {
        NDlabObject n = LabObjectFactory.LoadObjType(this.ObjType, parent);
        if (n == null)
            return null;
        if (n is NDCircuitObject)
        {
            (n as NDCircuitObject).SetData(this);
        }
        return n;
    }

    public override void Copy(LabObject info)
    {
        base.Copy(info);
        if(info is CircuitObject)
        {
            CircuitObject cirObj = info as CircuitObject;

            this.CircuitInfo = cirObj.CircuitInfo;

            this.CtrlNode.Clear();
            this.CtrlNode.AddRange(cirObj.CtrlNode);

            this.CtrlNode.Clear();
            this.CtrlNode.AddRange(cirObj.CtrlNode);
        }
    }
    public void SetCtrlInfo(List<PositionInfo> l)
    {
        CtrlNode.Clear();
        if (l != null && l.Count > 0)
            CtrlNode.AddRange(l);
    }

    public List<PositionInfo> GetCtrlInfo()
    {
        return CtrlNode;
    }

    public void SetLinkInfo(List<LeapObject> l)
    {
        ListLeap.Clear();
        if (l != null && l.Count > 0)
            ListLeap.AddRange(l);
    }

    public LeapObject GetLinkInfo(int LeapIndex)
    {
        if (CtrlNode == null || CtrlNode.Count == 0)
            return null;
        foreach (LeapObject v in ListLeap)
        {
            if (v.LeapIndex == LeapIndex)
                return v;
        }
        return null;
    }
}

[System.Serializable]
public class CircuitData
{
    public string LabTitle = "";
    public List<LabObject> m_Circuitobj = new List<LabObject>();

    public bool GetLabData()
    {
        GetCircuitData();
        return true;
    }

    public void Clear()
    {
        m_Circuitobj.Clear();
    }
    #region 获取加载元器件信息
    /// <summary>
    /// Get circuti data 
    /// </summary>
    public bool GetCircuitData()
    {
        m_Circuitobj.Clear();

        List<NDlabObject> l = NDlabObject.SearchLabObject(SearchCicuitType.NormalCircuit);

        foreach (NDlabObject Obj in l)
        {
            if (Obj == null)
                continue;
            CircuitObject o = (Obj as NDCircuitObject).GetCircuitData() as CircuitObject;
            if (o == null)
                continue;
            m_Circuitobj.Add(o);
        }
        List<NDlabObject> l2 = NDlabObject.SearchLabObject(SearchCicuitType.ELELINE);
        foreach (NDlabObject Obj in l2)
        {
            if (Obj == null)
                continue;
            CircuitObject o = (Obj as NDCircuitObject).GetCircuitData() as CircuitObject;
            if (o == null)
                continue;
            m_Circuitobj.Add(o);
        }
        return true;
    }

    public void LoadCircuitObj(Transform parent)
    {
        foreach (LabObject o in m_Circuitobj)
        {
            if (o == null)
                continue;
            o.LoadPerfab(parent);
        }
    }
    #endregion
}

