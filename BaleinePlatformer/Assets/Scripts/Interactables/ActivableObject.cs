using UnityEngine;

public class ActivableObject : MonoBehaviour
{

	[HideInInspector]
	public bool activated;
	
	public virtual void Activate()
	{
		activated = true;
	}

	public virtual void Deactivate()
	{
		activated = false;
	}
}