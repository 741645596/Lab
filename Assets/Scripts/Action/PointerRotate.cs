using UnityEngine;
using System.Collections;
/// <summary>
/// 电表指针旋转操作
/// <Author>QFord</Author>
/// </summary>
public class PointerRotate : MonoBehaviour {

    public Quaternion EndRotation ;
    public float Duration;

    private Transform m_target;
    private float m_TimeCount;

    private bool m_bDoRotate = false;
    
	
	void Update ()
	{
        DoRotate();
	}

    void DoRotate()
    {
        if (m_TimeCount >= Duration || !m_bDoRotate)
            return;
        m_target.rotation = Quaternion.Slerp(m_target.rotation, EndRotation, m_TimeCount / Duration);
        m_TimeCount += Time.deltaTime;
    }

    public void Stop()
    {
        m_bDoRotate = false;
        m_TimeCount = 0;
    }

    public void BeginRotation(Transform pointerTarget,Quaternion toRotation,float duration)
    {
        m_target = pointerTarget;
        EndRotation = toRotation;
        Duration = duration;
        m_TimeCount = 0;
        m_bDoRotate = true;
    }
	
    
}
