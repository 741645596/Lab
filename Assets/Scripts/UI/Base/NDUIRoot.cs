using UnityEngine;

public class NDUIRoot : MonoBehaviour {

	// Use this for initialization
    public Canvas UICanvas; 
    
    void Awake()
    {
        WndManager.SetWndRoot(UICanvas.transform);
    }

}
