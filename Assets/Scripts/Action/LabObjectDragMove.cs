using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
/// <summary>
/// Obj drag 
/// </summary>
public class LabObjectDragMove : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    protected Vector3 PickupPos = Vector3.zero;
    protected LabCamera m_labCamera;
    public NDlabObject m_labObject;
    public static bool LabObjectIsDrag = false;
    private Vector3 offsetPos = Vector3.zero;
    void Start()
    {

        if (m_labObject == null)
            m_labObject = GetComponent<NDlabObject>();

        if (m_labObject != null)
        {
            LabObjectOperateCenter.RegeditMove(this, m_labObject);
            if (m_labObject is EleLine)
            {
                m_labCamera = LabEnv.GetCameraByType(CameraType.WireCamera);
            }
            else
            {
                m_labCamera = LabEnv.GetCameraByType(CameraType.LabCamera);
            }
        }

    }
    void Destroy()
    {
        if (m_labObject != null && !(m_labObject is EleLine))
        {
            LabObjectOperateCenter.UnRegeditMove(this, m_labObject);
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        LabObjectIsDrag = true;

        Vector3 globalMousePos;
        RectTransform m_DraggingPlane = LabEnv.NodeLab.transform as RectTransform;

        if (m_labCamera != null && m_labCamera.ScreenPointToWorldPointInRectangle(m_DraggingPlane, eventData.position, out globalMousePos))
        {
            PickupPos = globalMousePos;
        }
        if (m_labObject != null && m_labObject is EleLine)
        {
            (m_labObject as EleLine).m_LineAction.SetLineControl();
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        RectTransform m_DraggingPlane = LabEnv.NodeLab.transform as RectTransform;

        if (m_labCamera != null && m_labCamera.ScreenPointToWorldPointInRectangle(m_DraggingPlane, eventData.position, out globalMousePos))
        {
            if (PickupPos != globalMousePos)
            {
                Vector3 offset = globalMousePos - PickupPos;
                PickupPos = globalMousePos;

                bool result = CheckPositionInCamera(offset + transform.position);
                if (result)
                {
                    List<NDlabObject> l = LabEnv.GetHighLightCircuitObject();
                    List<NDlabObject> list = new List<NDlabObject>();
                    list.AddRange(l);
                    if (list.Count < 1)
                    {
                        list.Add(m_labObject);
                    }
                    LabObjectOperateCenter.NoticeObjToMove(list, offset);
                }
            }
        }
    }
    public void ReceivedMoveCommand(Vector3 moveOffset)
    {
        Vector3 pos = moveOffset + transform.position;

        bool result = CheckPositionInCamera(pos);
        if (result)
        {
            transform.position = pos;
        }
    }

    public bool CheckPositionInCamera(Vector3 pos)
    {
        if (m_labCamera.CheckPositionInCamera(pos))
        {
            return true;
        }
        return false;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        LabObjectIsDrag = false;
    }


}
