using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Show Object to topmost when select
/// </summary>
public class LabObjTopSelect : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        DoTopMost();
    }
    private void DoTopMost()
    {
        NDlabObject obj = GetComponent<NDlabObject>();
        if (obj is EleLine)
            return;

        if (obj != null)
            obj.SetTopDepth();

    }
}
