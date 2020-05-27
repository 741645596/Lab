using UnityEngine;
using System.Collections.Generic;

public class LabEnv
{

    public static string PreCreateLabObjName = "";
    public static Vector3 PreCreateLabObjOffset = Vector3.zero;

    private static List<LabCamera> m_lLabCam = new List<LabCamera>();
    private static LabNode g_NodeLab = null;
    /// <summary>
    /// create a new lab object offset position
    /// </summary>
    public static Vector3 OffsetStep = new Vector3(0.4f, -0.4f, 0);



    /// <summary>
    /// highlight obj list
    /// </summary>
    private static List<NDlabObject> m_lHighlightLabObj = new List<NDlabObject>();

    public static LabNode NodeLab
    {
        get { return g_NodeLab; }
    }

    public static void ResetPreCreateLabObj()
    {
        PreCreateLabObjName = "";
        PreCreateLabObjOffset = Vector3.zero;
    }

    public static void RegisterCamera(LabCamera cam)
    {
        if (cam != null)
            m_lLabCam.Add(cam);
    }
    public static void UnRegisterCamera(LabCamera cam)
    {
        if (cam != null)
            m_lLabCam.Remove(cam);
    }

    public static LabCamera GetCameraByType(CameraType type)
    {
        foreach (LabCamera came in m_lLabCam)
        {
            if (came == null) continue;
            if (came.Type == type)
            {
                return came;
            }
        }
        return null;
    }


    public static void SetLabNode(LabNode Node)
    {
        g_NodeLab = Node;
    }
    public static void Reset()
    {
        g_NodeLab = null;
    }

    public static List<NDlabObject> GetHighLightCircuitObject()
    {
        return m_lHighlightLabObj;
    }
    public static void ClearAllHighlightLabObj()
    {
        m_lHighlightLabObj.Clear();
    }
    /// <summary>
    /// Add highlight obj(exclude others)
    /// </summary>
    public static void AddHighlightLabObj(NDlabObject obj, bool multiSelect = false)
    {
        if (obj is EleLine)//exclude Eleline
            return;
        bool bContain = m_lHighlightLabObj.Contains(obj);
        int count = m_lHighlightLabObj.Count;
        if (multiSelect == false)
        {
            ClearAllHighlightLabObj();
        }

        if (bContain && multiSelect == true)
        {
            m_lHighlightLabObj.Remove(obj);
        }
        else if (bContain && multiSelect == false)
        {
            if (count > 1)
            {
                m_lHighlightLabObj.Add(obj);
            }
        }
        else if (!bContain)
        {
            m_lHighlightLabObj.Add(obj);
        }
        
    }

    public static void RemoveHighlightLabObj(NDlabObject obj)
    {
        if (obj is EleLine)//exclude Eleline
            return;
        if (m_lHighlightLabObj.Contains(obj))
            m_lHighlightLabObj.Remove(obj);
    }
}


public enum CameraType
{
    WireCamera,
    LabCamera,
    MainCamera,
}