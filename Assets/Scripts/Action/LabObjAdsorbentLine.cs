using UnityEngine;
/// <summary>
/// Line follow the obj that line link
/// </summary>
public class LabObjAdsorbentLine : MonoBehaviour {

    private NDCircuitObject m_obj = null;
    void Start()
    {
        m_obj = GetComponent<NDCircuitObject>(); 
    }
	// Update is called once per frame
	void Update () 
    {
        ElelineFollow();
	}
    
    private void ElelineFollow()
    {
        if (m_obj != null)
        {
            var lLeap = m_obj.m_HaveLeap;
            foreach (var item in lLeap)
            {
                if (item != null)
                {
                    NDCircuitLeap cl = item.GetComponent<NDCircuitLeap>();
                    if (cl)
                    {
                        var lLine = cl.GetLinkLine();
                        foreach (var item1 in lLine)
                        {
                            if (item1 != null)
                                item1.m_LineAction.SetLeapPos();

                        }
                    }
                }
            }
        }
    }
}
