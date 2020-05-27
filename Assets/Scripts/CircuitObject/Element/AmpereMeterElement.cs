using UnityEngine;
using System.Collections.Generic ;
using System;

/// <summary>
/// 安培表
/// </summary>
public class AmpereMeterElement : NDCircuitObject {
	/// <summary>
	/// 指针
	/// </summary>
    public Transform Pointer;
    /// <summary>
    /// 坏指针
    /// </summary>
    public GameObject HuaiZhiZhen;

    /// <summary>
    /// 损毁烟雾表现
    /// </summary>
    public GameObject SmokePos;
    /// <summary>
    /// 3A量程
    /// </summary>
    float MaxRange = 3f;//3A量程
    /// <summary>
    /// 0.6A量程
    /// </summary>
    float MinRange = 0.6f;//0.6A量程
    float MaxAngle = 235f;//指针最大偏转角度（Y轴）
    float MinAngle = 109f;
    
    #region 安培表超量程判断变量
    /// <summary>
    /// 安培表1s超量程不会损坏
    /// </summary>
    public static float fBrokenDelay = 10f;
    /// <summary>
    /// 安培表当前电流
    /// </summary>
    float m_fAmpereNum = 0f;
    /// <summary>
    /// 安培表超量程开始时间
    /// </summary>
    float startBrokenTime = 0f;
    /// <summary>
    /// 安培表是否超量程
    /// </summary>
    bool m_isAmpereOutRang = false;
    Quaternion orgRatation;
    #endregion
    /// <summary>
    /// 内阻
    /// </summary>
	private float m_fInternalResistance = 0 ;
    /// <summary>
    /// 保留小数位数（预留）
    /// </summary>
    private int m_iRetainDecimalNum = 2;
    /// <summary>
    /// 指针表现控制类
    /// </summary>
    private PointerRotate AMPointerRotate;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Awake()
    {
        if(Pointer != null)
        {
            orgRatation = Pointer.rotation;
        }
        AMPointerRotate = GetComponent<PointerRotate>();
        this.EnableNumber = true;
        this.NumberName = "A";
    }
    private Transform WaiKe;
    void Start()
    {
        WaiKe = transform.Find("BiaoPanWaiKe");
        Pointer.RotateAround(WaiKe.transform.position, new Vector3(0, 1, 0), 100f);
        //TestAM(-10);
    }

    void Update()
    {
        if (m_isAmpereOutRang && m_labObjectState == LabObjectState.Good)
        {
            if (Time.realtimeSinceStartup - startBrokenTime >= fBrokenDelay)
            {
                SetAmpereAM(0f);
				State = LabObjectState.Broken;
            }
        }
    }

	public override void DoLabAction( float RunVoltage ,float RunCurrent ,float RunPower)
	{
		SetAmpereAM(RunCurrent ,IsBig());
        DoShowAction(State);
	}
    /// <summary>
    /// 电流表表现
    /// </summary>
    private void DoShowAction(LabObjectState state)
    {
        if (state == LabObjectState.Broken)
        {
            U3DUtil.SetActive(SmokePos, true);
            HuaiZhiZhen.SetActive(true);
            Pointer.gameObject.SetActive(false);
        }
        else
        {
            U3DUtil.SetActive(SmokePos, false);
            HuaiZhiZhen.SetActive(false);
            Pointer.gameObject.SetActive(true);
        }
    }

	/// <summary>
	/// 获取类型
	/// </summary>
	public override LabObjType GetLabObjType()
	{
		return LabObjType.Ammeter;
	}

    /// <summary>
    /// 设置电流表电流和指针偏转表现
    /// </summary>
    /// <param name="am">电流大小（A）电流为负数表示指针反向偏转</param>
    /// <param name="range">量程3还是0.6</param>
	public void SetAmpereAM(float am,bool IsBig = true)
    {
        if (m_labObjectState == LabObjectState.Broken || am == m_fAmpereNum)
        {
            return;
        }
        m_fAmpereNum = am;
        //超量程判断
		bool result = CheckAMOutRange(m_fAmpereNum, IsBig);
        if (result)
        {
            Debug.Log("安培表超量程 i=" +m_fAmpereNum);
            m_isAmpereOutRang = true;
            startBrokenTime = Time.realtimeSinceStartup;
        }

		float maxRange = IsBig == true ? MaxRange : MinRange;
        float maxAngle = m_fAmpereNum < 0 ? MinAngle : MaxAngle;
        //angle算出来的角度是基于初始位置需要偏转的角度
        float Angle = Mathf.Abs(MaxAngle) / maxRange * m_fAmpereNum;
        //超过量程的话多偏转几度
        float outRang = (m_fAmpereNum < 0 ? 4f : 5f) * (m_fAmpereNum < 0 ? -1 : 1);

        Angle = Mathf.Abs(Angle) > Mathf.Abs(maxAngle) ? (maxAngle  + outRang) : Angle;
        if (m_fAmpereNum == 0)
        {
            //直接反转当前的角度
            Angle = -(360-Pointer.rotation.eulerAngles.x);
        }
        Quaternion newRotation = new Quaternion(Pointer.rotation.x, Pointer.rotation.y, Pointer.rotation.z, Pointer.rotation.w); ;
        newRotation *= Quaternion.Euler(0, Angle, 0);
        AMPointerRotate.BeginRotation(Pointer, newRotation, m_fAmpereNum == 0 ? 0.1f : 0.3f);
    }
    /// <summary>
    /// 设置元件状态：是正常或者损坏
    /// </summary>
	public override void DoStateAction(LabObjectState state)
    {
        if (state == LabObjectState.Good)
        {
            m_isAmpereOutRang = false;
        }
    }

    public void StopAmpereMenter()
    {
        AMPointerRotate.Stop();
        m_isAmpereOutRang = false;
        SetAmpereAM(0);
    }
    /// <summary>
    /// 检查电流是否超量程(超量程后 指针变弯 且有 烧坏表现效果)
    /// </summary>
    /// <param name="am">电流值（当前引擎电流值正数为接反）</param>
	public bool CheckAMOutRange(float am, bool isBig)
    {
        if (isBig)//3A
        {
            if (am < 0)
                return (Mathf.Abs(am) > 3.6f);   
            else//接反
                return (Mathf.Abs(am) > 1.2f);  
        }
        else//0.6A
        {
            if (am < 0)
                return (Mathf.Abs(am) > 0.72f);
            else//接反
                return (Mathf.Abs(am) > 0.24f); 
        }
    }

	/// <summary>
	///取元气件速据
	/// </summary>
    public override LabObject GetCircuitData()
	{
		CircuitObject Info = new CircuitObject (ID,LabObjectType,Position);
		Dictionary<string , string> list = new Dictionary<string , string> ();
        list.Add("m_fInternalResistance", m_fInternalResistance.ToString());
        list.Add("m_iRetainDecimalNum", m_iRetainDecimalNum.ToString());
		//导线连接关系
		//Info.SetLinkInfo();
		return  Info;
	}

	/// <summary>
	///设置元气件速据 （用于从文件系统还原数据）
	/// </summary>
	public override void SetData(CircuitObject Info)
	{
	}
    public override void ResumeInfo(LabObject Info)
    {
        base.ResumeInfo(Info);
    }

    /// <summary>
    /// True 连接的3A，False是0.6A量程
    /// </summary>
	public bool IsBig()
	{
		foreach (NDCircuitLeap l  in  m_HaveLeap)
		{
			if (l.m_Type == ElementLeapType.leadOut && l.HaveLine) 
			{
				return l.big;
			}
		}
		return false;
	}

    /// <summary>
    /// 测试电流指针偏转表现方法
    /// </summary>
    private void TestAM(float am ,bool isBig = true)
    {
        SetAmpereAM(am, isBig);
    }
    
}
