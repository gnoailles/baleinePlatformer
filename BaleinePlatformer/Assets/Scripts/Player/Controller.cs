using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Controller : MonoBehaviour
{	
	struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;
		public bool climbingSlope;
		public float slopeAngle, previousSlopeAngle;

		public void Reset()
		{
			above = below = left = right = climbingSlope = false;
			
			previousSlopeAngle = slopeAngle;
			slopeAngle = 0f;
		}
	}

	private const float skinWidth = 0.015f;

	public CollisionInfo collisions;
	
	[SerializeField] private LayerMask collisionMask;
	[SerializeField] private int horizontalRayCount = 4;
	[SerializeField] private int verticalRayCount = 4;
	
	[SerializeField] private float maxClimbAmgle = 60f;

	private float horizontalRaySpacing;
	private float verticalRaySpacing;
	
	private BoxCollider collider;
	private RaycastOrigins raycastOrigins;

	private void Start()
	{
		collider = GetComponent<BoxCollider>();
		ComputeRaySpacing();
	}

	void VerticalCollisions(ref Vector3 velocity)
	{

		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;
		
		
		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector3 rayOrigin = (directionY < 0) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector3.right * (verticalRaySpacing * i + velocity.x);

			RaycastHit hit;

			if (Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength))
			{
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				collisions.below = directionY < 0;
				collisions.above = directionY > 0;
				
				if (collisions.climbingSlope)
				{
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}
				
			}

			if (collisions.climbingSlope)
			{
				float directionX = Mathf.Sign(velocity.x);
				rayLength = Mathf.Abs(velocity.x + skinWidth);

				rayOrigin = ((directionX < 0) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
				
				if(Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength))
				{
					float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
					if (slopeAngle != collisions.slopeAngle)
					{
						velocity.x = (hit.distance - skinWidth) * directionX;
						collisions.slopeAngle = slopeAngle;
					}
				}
			}
			
			
			Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLength, Color.red);
		}
	}

	void HorizontalCollisions(ref Vector3 velocity)
	{

		float directionX = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;
		
		
		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector3 rayOrigin = (directionX < 0) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector3.up * (horizontalRaySpacing * i);

			RaycastHit hit;

			if (Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength))
			{
				float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

				if(i == 0 && slopeAngle <= maxClimbAmgle)
				{
					float distanceToSlopeStart = 0f;
					if (slopeAngle != collisions.previousSlopeAngle)
					{
						distanceToSlopeStart = hit.distance - skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}

					Climbslope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxClimbAmgle)
				{
					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope)
					{
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}

					collisions.left = directionX < 0;
					collisions.right = directionX > 0;
				}
			}
			
			
			Debug.DrawRay(rayOrigin, Vector3.right * directionX * rayLength, Color.red);
		}
	}

	private void Climbslope(ref Vector3 velocity, float slopeAngle)
	{
		float moveDistance = Mathf.Abs(velocity.x);
		
		float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= climbVelocityY)
		{
			velocity.y = climbVelocityY;	
			velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
			collisions.below = collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}

	}

	private void UpdateRaycastOrigins()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);
		
		raycastOrigins.topLeft 		= new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight 	= new Vector2(bounds.max.x, bounds.max.y);
		raycastOrigins.bottomLeft 	= new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight 	= new Vector2(bounds.max.x, bounds.min.y);
	}

	private void ComputeRaySpacing()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);

		horizontalRayCount 	= Mathf.Max(horizontalRayCount, 2);
		verticalRayCount 	= Mathf.Max(verticalRayCount, 2);

		horizontalRaySpacing 	= bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing 		= bounds.size.x / (verticalRayCount - 1);
	}


	public void Move(Vector3 velocity)
	{
		collisions.Reset();
		UpdateRaycastOrigins();
		
		if(velocity.x != 0f)
			HorizontalCollisions(ref velocity);
		if(velocity.y != 0f)
		VerticalCollisions(ref velocity);
		
		transform.Translate(velocity);
	}
}
