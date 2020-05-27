using UnityEngine;
using System.Collections;

/// <summary>
/// Circuit Data
/// </summary>
/// <author>zhulin</author>
public class LeapObject  {
	
	public int LeapIndex  = 0;
	public int linkCircuitObjectID = 0;
	public int linkCircuitLeapIndex = 0 ;


	public LeapObject(){}
	public LeapObject(int LeapIndex,int linkCircuitObjectID ,int linkCircuitLeapIndex)
	{
		this.LeapIndex = LeapIndex;
		this.linkCircuitObjectID = linkCircuitObjectID;
		this.linkCircuitLeapIndex = linkCircuitLeapIndex;
	}

    public void Copy(LeapObject leapObj)
    {
        this.LeapIndex = leapObj.LeapIndex;
        this.linkCircuitObjectID = leapObj.linkCircuitObjectID;
        this.linkCircuitLeapIndex = leapObj.linkCircuitLeapIndex;
    }
}
