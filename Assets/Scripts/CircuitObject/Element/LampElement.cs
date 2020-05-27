using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 灯泡
/// </summary>
public class LampElement:NDCircuitObject
{
    /// <summary>
    /// 永弟说暂不适用自带灯光
    /// </summary>
    public Light LampLight;
    /// <summary>
    /// 灯泡 灯光 光晕表现的粒子系统
    /// </summary>
    public ParticleSystem EffDengGuang;
    ParticleSystem.EmissionModule psemit;
	/// <summary>
	/// 功率
	/// </summary>
	private float power = 0.75f;

    #region 特殊属性
    /// <summary>
    /// 额定电压
    /// </summary>
    private float m_fRatedVoltage = 3f;
    /// <summary>
    /// 额定功率
    /// </summary>
    private float m_fRatedPower = 0.75f;
    /// <summary>
    /// 显示电流
    /// </summary>
    private bool m_bShowElectricity = false;

    #endregion

    private Gradient m_gradientColor;

    public void Awake()
    {
        this.EnableNumber = true;
        this.NumberName = "L";
        InitGradientColor();
        psemit = EffDengGuang.emission;
    }

	public override void DoLabAction( float RunVoltage ,float RunCurrent ,float RunPower)
	{
        if (m_fRatedVoltage > 0)
            DoIntensity(Mathf.Abs(RunVoltage) / m_fRatedVoltage);
        else
            Debug.Log("Voltage rating must be positive");
	}
	/// <summary>
	/// 获取类型
	/// </summary>
	public override LabObjType GetLabObjType()
	{
		return LabObjType.Light;
	}

	void Start()
	{
        LabObjectDataFactory.SetLight(LabObjID, m_fRatedVoltage, power);
        
	}
    /// <summary>
    ///取元气件速据
    /// </summary>
    public override LabObject GetCircuitData()
    {
		CircuitObject Info = new CircuitObject(ID,LabObjectType, Position);
        Dictionary<string, string> list = new Dictionary<string, string>();
        list.Add("m_fRatedVoltage", m_fRatedVoltage.ToString());
        list.Add("m_fRatedPower", m_fRatedPower.ToString());
        list.Add("m_bShowElectricity", m_bShowElectricity.ToString());
        return Info;
    }

    /// <summary>
    ///设置元气件速据 （用于从文件系统还原数据）
    /// </summary>
    public override void SetData(CircuitObject Info)
    {
		base.SetData (Info);
    }
    public override void ResumeInfo(LabObject Info)
    {
        base.ResumeInfo(Info);
    }
    /// <summary>
    /// let the light intensity change as the current volume
    /// Normal [0,1] , Damage >=1.2;
    /// </summary>
    private void DoIntensity(float intensityRange)
    {
        //UGUIUtil.DebugLog("intensityRange = " + intensityRange);
        if (EffDengGuang != null)
        {
            EffDengGuang.Stop(true);
        }
        if (intensityRange <= 1.0f && intensityRange>0.01)
        {
            //EffDengGuang.gameObject.SetActive(true);
            if (LampLight != null)
            {
                LampLight.intensity = 6 * intensityRange;
                LampLight.color = m_gradientColor.Evaluate(intensityRange);
            }
            if (EffDengGuang!=null)
            {
                EffDengGuang.time = 0;
                EffDengGuang.startSize = 0.05f * intensityRange;
                EffDengGuang.startColor = m_gradientColor.Evaluate(intensityRange);
                EffDengGuang.Play();
            }
        }
        else if (intensityRange < 1.2f)//more 
        {
            if (LampLight != null)
            {
                LampLight.intensity = 6 + 2 * (intensityRange - 1);
                LampLight.color = Color.white;
            }
            if (EffDengGuang != null)
            {
                EffDengGuang.time = 0;
                EffDengGuang.startSize = 0.05f + 0.03f * ((intensityRange - 1)*5);
                EffDengGuang.startColor = Color.white;
                EffDengGuang.Play();
            }
        }
        else if (intensityRange >= 1.2f)//damage
        {
            //TODO 添加烧坏效果
            if (LampLight != null)
            {
                LampLight.intensity = 6 + 2 * (intensityRange - 1);
                LampLight.color = Color.black;
            }
            if (EffDengGuang != null)
            {
                EffDengGuang.time = 0;
                EffDengGuang.startSize = 0.05f + 0.03f * ((intensityRange - 1) * 5);
                EffDengGuang.startColor = Color.black;
                EffDengGuang.Play();
            }
        }
        else if (intensityRange <= 0.01)
        {
            if (EffDengGuang)
            {
                EffDengGuang.time = 0;
                EffDengGuang.startSize = 0;
            }
        }
    }
    /// <summary>
    /// Light gradient color init setting
    /// </summary>
    private void InitGradientColor()
    {
        m_gradientColor = new Gradient();
        GradientColorKey[] gck;
        GradientAlphaKey[] gak;
        gck = new GradientColorKey[2];
        gck[0].color = Color.yellow;
        gck[0].time = 0.8F;//0.8 turn to white color
        gck[1].color = Color.white;
        gck[1].time = 1.0F;
        gak = new GradientAlphaKey[2];
        gak[0].alpha = 0.0F;
        gak[0].time = 0.0F;
        gak[1].alpha = 1.0F;
        gak[1].time = 1.0F;
        m_gradientColor.SetKeys(gck, gak);
    }
}