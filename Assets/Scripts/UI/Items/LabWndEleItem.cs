using UnityEngine.UI;

public class LabWndEleItem : WndItem {
    public Image ItemImage;
	public string m_eleName;
	private LabObjUnit m_Unit;
	// Use this for initialization
	void Start ()
    {
        AddDragHandle();
        SetUI();
	}
    protected override void SetUI()
    {
		if(ItemImage != null && m_Unit != null)
        {
			ItemImage.sprite = UnityHelper.LoadSprite(m_Unit.ThumbPic);
        }
    }
    public void AddDragHandle()
    {
        EleMentDragHandle handle = gameObject.AddComponent<EleMentDragHandle>();
		if(handle != null && m_Unit != null)
        {
			handle.SetPerfabName(m_Unit.AssestPath);
        }
    }

	public void SetData(LabObjUnit Unit)
	{
		m_Unit = Unit;
	}
}
