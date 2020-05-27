using UnityEngine;
using System.Collections;


public class SceneObj : MonoBehaviour {
	private bool bDown = false;
	private Vector3 vDownPos;
	private IScene m_ISceneOwer;

	public virtual void Start () {
		m_ISceneOwer = SceneM.GetCurIScene ();
		if (SceneM.IsLoading)
			m_ISceneOwer = SceneM.GetLoadingIScene ();
		if(null!=m_ISceneOwer)
		{
			m_ISceneOwer.AddSceneObj(this);
		}
	
	}

	void OnMouseDown()
	{
		IScene scene = SceneM.GetCurIScene ();
		if (null != scene) 
		{
			scene.OnMouseDown(this);
		}

	}
	void OnMouseUp()
	{
		IScene scene = SceneM.GetCurIScene ();
		if (null != scene) 
		{
			scene.OnMouseUp(this);
		}
	}

    public virtual void OnDestroy()
	{
		if(null!=m_ISceneOwer)
			m_ISceneOwer.ReMoveSceneObj(this);
    }


	/// <summary>
	/// FixedUpdate
	/// </summary>
	protected virtual void NDStart ()
	{

	}
	/// <summary>
	/// Update
	/// </summary>
	public virtual void NDUpdate (float deltaTime) 
	{

	}
	/// <summary>
	/// FixedUpdate
	/// </summary>
	public virtual void NDFixedUpdate (float deltaTime)
	{

	}
	/// <summary>
	/// LateUpdate
	/// </summary>
	public virtual void NDLateUpdate(float deltaTime) 
	{

	}
}
