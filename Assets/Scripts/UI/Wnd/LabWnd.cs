using UnityEngine;

public class LabWnd : WndBase
{
    public Transform TPicTarget;
    public override void InitWnd()
    {
        base.InitWnd();
        InitLoadItem();
    }
    public override void BindEvents()
    {
        base.BindEvents();
    }


    void InitLoadItem()
    {
		foreach (LabObjUnit Unit in AppData.ObjData.m_Items) 
		{
			if (Unit != null) 
			{
				LabWndEleItem Item = NDLoad.LoadWndItem("LabWndEleItem", TPicTarget) as LabWndEleItem;
				if(Item != null)
					Item.SetData (Unit);
			}
		}
    }
}
