using UnityEngine;

public class RotateAround : MonoBehaviour {

    public float x;
    public float y;
    public float z;

	// Use this for initialization
	void Start () {
        transform.Rotate(x, y, z, Space.Self);
	}
	
	
}
