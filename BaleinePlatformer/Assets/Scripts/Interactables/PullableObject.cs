using UnityEngine;

public class PullableObject : MonoBehaviour
{

	[HideInInspector]
	public bool isPulling;
		
	protected virtual void Start ()
	{
		gameObject.layer = LayerMask.NameToLayer("Pullable");
	}
	
	public virtual void Pull()
	{
		isPulling = true;
	}

	public virtual void Detach()
	{
		isPulling = false;
	}
}
