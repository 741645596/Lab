using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class EleLineOptimezation : MonoBehaviour, IDragHandler
{
    public ELineCtrlNode m_ctrlNode = null;
    public LineAction m_LineAction = null;
    // Use this for initialization
    void Start()
    {
        if (m_ctrlNode == null)
        {
            m_ctrlNode = GetComponent<ELineCtrlNode>();
        }
        if (m_LineAction == null)
        {
            m_LineAction = GetComponent<LineAction>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_ctrlNode != null)
        {
            if (m_ctrlNode.type != CtrlNodeType.Mid)
            {
                if (m_ctrlNode.m_lineAction.m_line.ConnectLink == false)
                    m_ctrlNode.m_lineAction.OptimezationLine();
            }
            else
            {
                m_ctrlNode.SetMidChange();
            }
        }
        else if (m_LineAction != null && m_LineAction.m_line != null)
        {
            EleLine line = m_LineAction.m_line as EleLine;
            if (line.OneConnectLink == true)
                m_LineAction.SetMidChange();
        }
    }
}
