using UnityEngine;

public class LabCamera : MonoBehaviour
{
    public CameraType Type;
    public Camera m_camera;
    // Use this for initialization
    void Start()
    {
        LabEnv.RegisterCamera(this);
    }
    void OnDestroy()
    {
        LabEnv.UnRegisterCamera(this);
    }

    public bool ScreenPointToWorldPointInRectangle(RectTransform dragRect, out Vector3 dragRectPos)
    {
        return RectTransformUtility.ScreenPointToWorldPointInRectangle(dragRect, Input.mousePosition, m_camera, out dragRectPos);
    }
    public bool ScreenPointToWorldPointInRectangle(RectTransform dragRect, Vector3 mousePos, out Vector3 dragRectPos)
    {
        return RectTransformUtility.ScreenPointToWorldPointInRectangle(dragRect, mousePos, m_camera, out dragRectPos);
    }

    public bool CheckPositionInCamera(Vector3 WorldPos)
    {
        Vector3 range = m_camera.WorldToViewportPoint(WorldPos);
        if (range.x > 0 && range.x < 1 && range.y > 0 && range.y < 1)
            return true;
        return false;
    }
}
