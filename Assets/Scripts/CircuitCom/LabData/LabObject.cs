using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 撤销重做中标识操作类型
/// </summary>
public enum LabObjectOpera
{
    /// <summary>
    /// 修改元器件
    /// </summary>
    Modify,
    Delete,
    Create,
}
/// <summary>
/// Circuit Data
/// </summary>
/// <author>zhulin</author>
public class LabObject 
{
	public int ID;
	public string LabObjectType = "CircuitObject";
	public int ObjType;
	public PositionInfo Position = new PositionInfo();

    /// <summary>
    /// 是否被删除，在撤销重做中需要标识
    /// </summary>
    public bool isDelete = false;
    public bool isCreate = false;

	public LabObject(){}
	public LabObject(int ID,int ObjType ,PositionInfo Info)
	{
		this.ID = ID;
		this.ObjType = ObjType;
		this.Position.Copy(Info);
        
	}
	/// <summary>
	/// load circuti data from path
	/// </summary>
	public virtual  NDlabObject LoadPerfab(Transform parent)
	{
		return  null;
	}

    public virtual void Copy(LabObject info)
    {
        this.ID = info.ID;
        this.LabObjectType = info.LabObjectType;
        this.ObjType = info.ObjType;
        this.Position.Copy(info.Position);
    }

}
	
public class PositionInfo
{
	public double x;
	public double y;
	public double z;

	public double rotateX;
	public double rotateY;
	public double rotateZ;

	public double scaleX;
	public double scaleY;
	public double scaleZ;

	public PositionInfo(){}

	public PositionInfo(PositionInfo a)
	{
		this.x = a.x;
		this.y = a.y;
		this.z = a.z;

		this.rotateX = a.rotateX;
		this.rotateY = a.rotateY;
		this.rotateZ = a.rotateZ;

		this.scaleX = a.scaleX;
		this.scaleY = a.scaleY;
		this.scaleZ = a.scaleZ;
	}
	public PositionInfo(Transform tran)
	{
		SetPositionInfo (tran);
	}

	public void SetPositionInfo(Transform tran)
	{
		this.x = tran.position.x;
		this.y = tran.position.y;
		this.z = tran.position.z;

		this.rotateX = tran.rotation.x;
		this.rotateY = tran.rotation.y;
		this.rotateZ = tran.rotation.z;

		this.scaleX = tran.localScale.x;
		this.scaleY = tran.localScale.y;
		this.scaleZ = tran.localScale.z;
	}

	public void Copy(PositionInfo a)
	{
		this.x = a.x;
		this.y = a.y;
		this.z = a.z;

		this.rotateX = a.rotateX;
		this.rotateY = a.rotateY;
		this.rotateZ = a.rotateZ;

		this.scaleX = a.scaleX;
		this.scaleY = a.scaleY;
		this.scaleZ = a.scaleZ;
	}

	public Vector3 GetPos()
	{
		return new Vector3 ((float) x,(float) y,(float) z);
	}

	public Vector3 GetScale()
	{
		return new Vector3 ((float) scaleX,(float) scaleY,(float) scaleZ);
	}


	public Vector3 GetRotation()
	{
		return new Vector3 ((float) rotateX,(float) rotateY,(float) rotateZ);
	}
}
