using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RaycastController : MonoBehaviour {
	#region Raycast

	protected struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	protected void UpdateRaycastOrigins()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(SkinWidth * -2);
		
		raycastOrigins.topLeft 		= new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight 	= new Vector2(bounds.max.x, bounds.max.y);
		raycastOrigins.bottomLeft 	= new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight 	= new Vector2(bounds.max.x, bounds.min.y);
	}

	private void ComputeRaySpacing()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(SkinWidth * -2);

		horizontalRayCount 	= Mathf.Max(horizontalRayCount, 2);
		verticalRayCount 	= Mathf.Max(verticalRayCount, 2);

		horizontalRaySpacing 	= bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing 		= bounds.size.x / (verticalRayCount - 1);
	}
	#endregion
	
	protected const float SkinWidth = 0.015f;
	
	private new BoxCollider collider;
	protected RaycastOrigins raycastOrigins;
	
	[SerializeField] protected int horizontalRayCount = 4;
	[SerializeField] protected int verticalRayCount = 4;
		
	protected float horizontalRaySpacing;
	protected float verticalRaySpacing;
	
	[SerializeField] private string[] collisionsMasks;
	protected int collisionMask;

	

	
	public virtual void Start()
	{
		collider = GetComponent<BoxCollider>();
		ComputeRaySpacing();

		collisionMask = LayerMask.GetMask(collisionsMasks);
	}
}
