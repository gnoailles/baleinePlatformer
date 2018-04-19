using NUnit.Framework.Constraints;
using UnityEngine;

public class Controller : RaycastController
{

	#region Collisions
	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;
		public bool climbingSlope, descendingSlope;
		public float slopeAngle, previousSlopeAngle;

		public void Reset()
		{
			above = below = left = right = false;
			climbingSlope = descendingSlope = false;
			
			previousSlopeAngle = slopeAngle;
			slopeAngle = 0f;
		}
	}

	public CollisionInfo collisions;
	
	#endregion
	
	[SerializeField] private float maxClimbAngle = 60f;
	[SerializeField] private float maxDescendAngle = 45f;

	private Vector2 previousVelocity;

	public override void Start()
	{
		base.Start();
	}

	void VerticalCollisions(ref Vector2 testVelocity)
	{

		float directionY = Mathf.Sign(testVelocity.y);
		float rayLength = Mathf.Abs(testVelocity.y) + SkinWidth;
		
		
		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector3 rayOrigin = (directionY < 0) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += transform.right * (verticalRaySpacing * i + testVelocity.x);

			RaycastHit hit;

			if (Physics.Raycast(rayOrigin, transform.up * directionY, out hit, rayLength, collisionMask))
			{
				testVelocity.y = (hit.distance - SkinWidth) * directionY;
				rayLength = hit.distance;

				collisions.below = directionY < 0;
				collisions.above = directionY > 0;
				
				if (collisions.climbingSlope)
				{
					testVelocity.x = testVelocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(testVelocity.x);
				}
				
			}
			Debug.DrawRay(rayOrigin, transform.up * directionY * rayLength, Color.red);
		}
		
		
		//Slope angle change
		if (collisions.climbingSlope)
		{
			float directionX = Mathf.Sign(testVelocity.x);
			rayLength = Mathf.Abs(testVelocity.x + SkinWidth);

			Vector3 rayOrigin = ((directionX < 0) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight);
			rayOrigin += transform.up * testVelocity.y;
			RaycastHit hit;
				
			if(Physics.Raycast(rayOrigin, Vector2.right * directionX, out hit, rayLength))
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if (slopeAngle != collisions.slopeAngle)
				{
					testVelocity.x = (hit.distance - SkinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	void HorizontalCollisions(ref Vector2 testVelocity)
	{

		float directionX = Mathf.Sign(testVelocity.x);
		float rayLength = Mathf.Abs(testVelocity.x) + SkinWidth;
		
		
		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector3 rayOrigin = (directionX < 0) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += transform.up * (horizontalRaySpacing * i);

			RaycastHit hit;

			if (Physics.Raycast(rayOrigin, transform.right * directionX, out hit, rayLength))
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if(i == 0 && slopeAngle <= maxClimbAngle)
				{
					if (collisions.descendingSlope)
					{
						collisions.descendingSlope = false;
						testVelocity = previousVelocity;
					}
					
					float distanceToSlopeStart = 0f;
					if (slopeAngle != collisions.previousSlopeAngle)
					{
						distanceToSlopeStart = hit.distance - SkinWidth;
						testVelocity.x -= distanceToSlopeStart * directionX;
					}

					ClimbSlope(ref testVelocity, slopeAngle);
					testVelocity.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
				{
					testVelocity.x = (hit.distance - SkinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope)
					{
						testVelocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(testVelocity.x);
					}

					collisions.left = directionX < 0;
					collisions.right = directionX > 0;
				}
			}
			
			
			Debug.DrawRay(rayOrigin, transform.right * directionX * rayLength, Color.red);
		}
	}

	private void ClimbSlope(ref Vector2 testVelocity, float slopeAngle)
	{
		float moveDistance = Mathf.Abs(testVelocity.x);
		
		float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (testVelocity.y <= climbVelocityY)
		{
			testVelocity.y = climbVelocityY;	
			testVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(testVelocity.x);
			collisions.below = collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}

	}
	
	private void DescendSlope(ref Vector2 testVelocity)
	{
		float directionX = Mathf.Sign(testVelocity.x);

		Vector2 rayOrigin = (directionX < 0) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;

		RaycastHit hit;

		if (Physics.Raycast(rayOrigin, Vector2.down, out hit, Mathf.Infinity))
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
			{
				if (Mathf.Sign(hit.normal.x) == directionX)
				{
					if (hit.distance - SkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(testVelocity.x))
					{
						float moveDistance = Mathf.Abs(testVelocity.x);
						float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
						testVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(testVelocity.x);
						testVelocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}

	}



	public Vector2 Collide(Vector2 testVelocity)
	{
		UpdateRaycastOrigins();
		collisions.Reset();
		previousVelocity = testVelocity;

		if(testVelocity.y < 0f)
			DescendSlope(ref testVelocity);
		if(testVelocity.x != 0f)
			HorizontalCollisions(ref testVelocity);
		if(testVelocity.y != 0f)
			VerticalCollisions(ref testVelocity);

		return testVelocity;
	}
}
