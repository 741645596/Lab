using UnityEngine;
using System.Collections;
using System.Collections.Generic ;
using SharpCircuit ;


/// <summary>
///  实验对象数据工厂
/// </summary>
/// <author>zhulin</author>
public class LabObjectDataFactory  {

	public static Dictionary<int ,CircuitElement > g_CircuitData = new Dictionary<int, CircuitElement>();
	/// <summary>
	///获取元器件引擎数据
	/// </summary>
	public static CircuitElement GetCircuit(int ObjID)
	{
		if (g_CircuitData.ContainsKey (ObjID) == true) 
		{
			return g_CircuitData [ObjID];
		} 
		else
			return null;
	}

	/// <summary>
	///  添加元器件引擎数据
	/// </summary>
	public static void AddCircuit(int ObjID ,LabObjType Type )
	{
		if (g_CircuitData.ContainsKey (ObjID) == true) {
			return;
		} 
		else 
		{
			CircuitElement ele = CreateCircuit (Type);
			if (ele != null)
				g_CircuitData.Add (ObjID ,ele);
		}
	}


	/// <summary>
	///  移除引擎数据
	/// </summary>
	public static void RemoveCircuit(int ObjID )
	{
		if (g_CircuitData.ContainsKey (ObjID) == true) 
		{
			g_CircuitData.Remove (ObjID);
		} 
	}

	/// <summary>
	/// 根据类型创建引擎数据
	/// </summary>
	private static CircuitElement CreateCircuit(LabObjType Type )
	{
		if (Type == LabObjType.Power) 
		{
			return new DCVoltageSource ();
		} 
		else if (Type == LabObjType.Switch) 
		{
			return new  SwitchSPST();
		}
		else if (Type == LabObjType.Light || Type == LabObjType.Resistance) 
		{
			return new Resistor();
		}
		if (Type == LabObjType.Ammeter) 
		{
			return new AmpereMeter ();
		}
        if (Type == LabObjType.EleLine)
            return new Wire();
        Debug.Log("LabObjectDataFactory CreateCircuit fail!");
		return null;
	}


	/// <summary>
	/// 设置开关的关闭
	/// </summary>
	public static bool GetSwitchState(int ObjID )
	{
		CircuitElement cir  = GetCircuit(ObjID) ;
		if (cir == null)
			return false;
		if (cir is SwitchSPST) 
		{
			return (cir as SwitchSPST).IsOpen ;
		}
		return false;
	}
	/// <summary>
	/// 设置开关的关闭
	/// </summary>
	public static void SetSwitch(int ObjID , bool IsOpen)
	{
		CircuitElement cir  = GetCircuit(ObjID) ;
		if (cir == null)
			return;
		if (cir is SwitchSPST) 
		{
            //(cir as SwitchSPST).toggle();
            (cir as SwitchSPST).IsOpen = IsOpen;
		}
	}
	/// <summary>
	/// 设置电源数据
	/// </summary>
	public static void SetPower(int ObjID , float Volatage)
	{
		CircuitElement cir  = GetCircuit(ObjID) ;
		if (cir == null)
			return;
		if (cir is DCVoltageSource) 
		{
			(cir as DCVoltageSource).maxVoltage = Volatage;
		}
	}
    /// <summary>
    /// 设置电阻数据
    /// </summary>
    public static void SetResistance(int ObjID, float resistance)
    {
        CircuitElement cir = GetCircuit(ObjID);
        if (cir == null)
        {
            Debug.Log("ObjID = " + ObjID + " not found!");
            return;
        }
        Resistor r = cir as Resistor;
        if (r != null)
            r.resistance = resistance;
        else
            Debug.Log("ObjID = " + ObjID + "is not a Resistor!"); 
    }

	/// <summary>
	/// 设置电灯数据
	/// </summary>
	public static void SetLight(int ObjID , float Volatage ,float Power)
	{
		CircuitElement cir  = GetCircuit(ObjID) ;
		if (cir == null)
			return;
		if (cir is Resistor) 
		{
			(cir as Resistor).resistance = Volatage * Volatage / Power;
		}
	}
	/// <summary>
	/// 获取引擎运行时数据
	/// </summary>
	public static void GetCircuitRunData(int ObjID ,ref float runCurrent ,ref float runVoltage ,ref float runPower)
	{
		CircuitElement cir = GetCircuit (ObjID);
		if (cir != null) 
		{
			cir.calculateCurrent();
			runCurrent = -1 * (float)cir.getCurrent ();
			runVoltage = -1 * (float)cir.getVoltageDelta ();
			runPower = runCurrent * runVoltage;
		}
	}
	/// <summary>
	/// 获取引擎运行时数据
	/// </summary>
	public static void ResetCircuitRunData(int ObjID )
	{
		CircuitElement cir = GetCircuit (ObjID);
		if (cir != null) 
		{
			cir.reset ();
		}
	}

}
