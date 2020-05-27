using UnityEngine;
/// <summary>
/// Use hot key to（↑↓）change scene's light intensity
/// </summary>
public class LightIntensity : MonoBehaviour
{
    public float InitIntensity = 2.0f;
    public float Factor = 1.0f;

    private Light m_light;

	void Start () 
    {
        m_light = GetComponent<Light>();
        if (m_light)
            m_light.intensity = InitIntensity;
	}
	
	void Update () 
    {
        LightControl();
	}

    void LightControl()
    {
        if (m_light == null)
        {
            return;
        }
            
        if (Input.GetKey(KeyCode.DownArrow))
        {
            float temp = m_light.intensity;
            temp -= Time.deltaTime * Factor;
            temp = Mathf.Clamp(temp, 0f, 8f);
            m_light.intensity = temp;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            float temp = m_light.intensity;
            temp += Time.deltaTime * Factor;
            temp = Mathf.Clamp(temp, 0f, 8f);
            m_light.intensity = temp;
        }
    }
}
