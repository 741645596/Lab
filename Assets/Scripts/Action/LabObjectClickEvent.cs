using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// labobject click event handle
/// </summary>
public class LabObjectClickEvent : MonoBehaviour, IPointerClickHandler
{
    static bool ControlButtonDown = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            ControlButtonDown = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            ControlButtonDown = false;
        }
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
            ClickMenuWnd.DeleteSelecetEle();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) return;
        NDlabObject obj = GetComponent<NDlabObject>();
        if (obj != null)
        {
            if (obj is EleLine)
            {
                LineAction action = obj.gameObject.GetComponent<LineAction>();
                if (action)
                {
                    action.SetControling();
                }
            }
            if (eventData.clickCount == 2)//show the Menu UI 
            {
                if (obj != null && obj.PlayerState == false)
                {
                    ClickMenuWnd wnd = WndManager.GetWnd<ClickMenuWnd>();
                    if (wnd != null)
                    {
                        wnd.SetCurCircuitObject(obj as NDCircuitObject);
                        wnd.BtnDel.transform.position = new Vector3(eventData.position.x, eventData.position.y, 0);
                    }
                }
            }
            else if (eventData.clickCount == 1 && eventData.dragging == false && eventData.delta.x < 0.1f && eventData.delta.y < 0.1f)
            {
                LabEnv.AddHighlightLabObj(obj, ControlButtonDown);
            }
        }

    }
}
