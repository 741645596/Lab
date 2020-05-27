using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ClickMenuWnd : WndBase,IPointerClickHandler {
    public Button BtnDel;
    public Button BtnCancel;
    protected NDCircuitObject m_curCirObj;
    
	// Use this for initialization
    public override void InitWnd()
    {
        base.InitWnd();
    }

    public override void BindEvents()
    {
        base.BindEvents();
        BtnDel.onClick.AddListener(BtnDelClick);
        BtnCancel.onClick.AddListener(BtnCancelClick);
    }
    void BtnDelClick()
    {
        if (m_curCirObj != null)
        {
            LabEnv.ClearAllHighlightLabObj();

            m_curCirObj.DestroyLabObject();

        }
        WndManager.DestoryWnd<ClickMenuWnd>();
        NDCircuitObject.CreateCirCuit();
    }

    void BtnCancelClick()
    {
        WndManager.DestoryWnd<ClickMenuWnd>();
    }
    public void SetCurCircuitObject(NDCircuitObject curcirObj)
    {
        if(curcirObj != null)
        {
            m_curCirObj = curcirObj;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        WndManager.DestoryWnd<ClickMenuWnd>();
    }

    public static void DeleteSelecetEle()
    {
        List<NDlabObject> l = LabEnv.GetHighLightCircuitObject();
        foreach (NDlabObject obj in l)
        {
            if(obj.PlayerState == false)
                obj.DestroyLabObject();
        }
        LabEnv.ClearAllHighlightLabObj();
    }
}
