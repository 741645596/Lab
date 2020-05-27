using UnityEngine;

public class LabNode : MonoBehaviour {

	void OnEnable()
	{
		LabEnv.SetLabNode(this);
	}
	void OnDestroy()
	{
		LabEnv.Reset();
	}

}
