using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// 开关开关闸
/// </summary>
/// <param name="bSwitchState"></param>
public delegate void SwitchStateChangeCallBack(SwitchState bSwitchState);
public class SwitchHandle : MonoBehaviour, IDragHandler,IEndDragHandler,IBeginDragHandler
{
    public GameObject GoHandle;
    public GameObject GoShine;
    private SwitchState m_bSwitchState = SwitchState.SwitchOpen;
    public SwitchStateChangeCallBack SwitchCallBack;

	public bool IsOpen {get {return m_bSwitchState == SwitchState.SwitchOpen;}}

    public  void OnDrag(PointerEventData data)
	{
        float y = data.delta.y;
        float RotaionX = GoHandle.transform.localRotation.eulerAngles.z;
        if (RotaionX + y >= 355)
        {
            return;
        }
        if (RotaionX + y <= 336)
        {
            return;
        }
        Quaternion newRotation = new Quaternion(GoHandle.transform.rotation.x, GoHandle.transform.rotation.y, GoHandle.transform.rotation.z, GoHandle.transform.rotation.w);
        newRotation *= Quaternion.Euler(0, 0, y);
        GoHandle.transform.rotation = newRotation;

        float finalX = GoHandle.transform.localEulerAngles.z;
        if (finalX <= 343 && finalX >= 317)
        {
            //Debug.Log("Swich Close");
            if (m_bSwitchState == SwitchState.SwitchOpen)
            {
                m_bSwitchState = SwitchState.SwitchClose;
                SwitchCallBack?.Invoke(m_bSwitchState);
            }               
        }
        else 
        {
            //Debug.Log("Swich open");
            if (m_bSwitchState == SwitchState.SwitchClose)
            {
                m_bSwitchState = SwitchState.SwitchOpen;
                SwitchCallBack?.Invoke(m_bSwitchState);
            }
        }

	}

    public void OnEndDrag(PointerEventData eventData)
    {
        LabObjectDragMove.LabObjectIsDrag = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        LabObjectDragMove.LabObjectIsDrag = true;
    }
}
