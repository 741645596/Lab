using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EleMentDragHandle : MonoBehaviour ,IBeginDragHandler,IEndDragHandler,IDragHandler,IPointerClickHandler{

	private string  m_PerfabName;
	// Use this for initialization
    private GameObject m_DraggingItem;
    private RectTransform m_DraggingPlane;
    private float m_fScaleNum = 0.5f;
    private NDCircuitObject m_labObject;
    public void OnDrag(PointerEventData data)
    {
        if (m_DraggingItem != null)
        {
            if (m_DraggingItem != null)
                m_DraggingItem.SetActive(true);
            SetDraggedPosition(data);
        }
        else
        {
            Creatm_DraggingItem(data);
        }
    }

    private void SetDraggedPosition(PointerEventData data)
    {
        Vector3 globalMousePos;

        CameraType type = m_labObject != null && m_labObject is EleLine ? CameraType.WireCamera : CameraType.LabCamera;

        LabCamera carme = LabEnv.GetCameraByType(type);
        if (carme != null && carme.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, out globalMousePos))
        {
            Vector3 org = m_DraggingItem.transform.position;
            globalMousePos = U3DUtil.SetZ(globalMousePos, m_DraggingItem.transform.position.z);

            m_DraggingItem.transform.position = globalMousePos;
            if (m_labObject != null && m_labObject is EleLine)
            {
                bool startOut = carme.CheckPositionInCamera((m_labObject as EleLine).m_LineAction.m_StartCtrl.transform.position);
                bool EndOut = carme.CheckPositionInCamera((m_labObject as EleLine).m_LineAction.m_EndCtrl.transform.position);
                if (!startOut || !EndOut)
                {
                    m_DraggingItem.transform.position = org;
                }
            }
            else if (!carme.CheckPositionInCamera(globalMousePos))
            {
                m_DraggingItem.transform.position = org;
            }
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        LabObjectDragMove.LabObjectIsDrag = true;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        m_DraggingItem.transform.localScale /= m_fScaleNum;
        m_DraggingItem = null;
        LabObjectDragMove.LabObjectIsDrag = false;
    }
    void Creatm_DraggingItem(PointerEventData data)
    {
        var canvas = UnityHelper.FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;

        m_DraggingItem = CreateLabObj(1);
        if (m_DraggingItem != null)
        {
            m_DraggingItem.transform.localScale *= m_fScaleNum;
        }
		m_DraggingItem.transform.SetParent(LabEnv.NodeLab.transform, false);
  
		m_DraggingPlane = LabEnv.NodeLab.transform as RectTransform;

        SetDraggedPosition(data);

    }

	public void SetPerfabName(string  Name)
    {
		m_PerfabName = Name;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
		CreateLabObj (0);
    }
    /// <summary>
    /// create lab object by click | drag way.
    /// </summary>
    /// <param name="createWay">0:Click 1:Drag</param>
	private GameObject CreateLabObj(int createWay)
	{
		if (m_PerfabName == "") 
			return null;

		GameObject obj = NDLoad.LoadPrefab(m_PerfabName, LabEnv.NodeLab.transform);

		if (obj == null)
			return null;

        if (createWay == 0)
        {
            SetPosOffset(obj);
            SetPreCreateObjData();
        }
        else
            LabEnv.ResetPreCreateLabObj();

		NDCircuitObject o = obj.GetComponent<NDCircuitObject>();
		if (o != null) 
		{
			o.SetDefaultData ();
            o.SetGameobjectName();
		}
        m_labObject = o;
		return obj;
	}

    private void SetPreCreateObjData()
    {
        if (m_PerfabName != "")
        {
            if (LabEnv.PreCreateLabObjName != "")
            {
                if (LabEnv.PreCreateLabObjName != m_PerfabName)
                    LabEnv.ResetPreCreateLabObj();
            }
            LabEnv.PreCreateLabObjName = m_PerfabName;
        }
    }

    private void SetPosOffset(GameObject go)
    {
        if (go == null || LabEnv.PreCreateLabObjName=="")
            return;
        float z = go.transform.localPosition.z;
        go.transform.localPosition += LabEnv.PreCreateLabObjOffset + LabEnv.OffsetStep;
        go.transform.localPosition = U3DUtil.SetZ(go.transform.localPosition, z);
        LabEnv.PreCreateLabObjOffset += LabEnv.OffsetStep;
    }
    }
