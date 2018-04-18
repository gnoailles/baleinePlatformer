using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal.Input;

[RequireComponent(typeof(Controller))]
public class Player : MonoBehaviour
{
	
	
	[SerializeField] private FishingRod fishingRod;
	
	
	[SerializeField] private float gravity = 20f;
	[SerializeField] private float mass = 1f;
	[SerializeField] private float moveSpeed = 5f;
	[SerializeField] private float accelerationTimeAirbourne = 0.2f;
	[SerializeField] private float accelerationTimeGrounded = 0.1f;

	[Space(20)]
	
	[SerializeField] private float jumpHeight = 4f;
	[SerializeField] private float timeToJumpApex = .4f;
	
	[Space(20)]
	
	[SerializeField] private float hookBalanceForce = 0.1f;
	[SerializeField] private float hookMaxAngle = 50f;
	
	private Controller controller;
	private Vector2 acceleration;
	private Vector2 velocity;
	private Vector2 position;
	private float jumpGravity;
	private float jumpVelocity;

	private float smoothedXVelocity;
	private bool isJumping;

	void Start()
	{
		controller = GetComponent<Controller>();
		jumpGravity = (2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
		jumpVelocity = jumpGravity * timeToJumpApex;
	}

	private void Update()
	{
		position = transform.position;
		
		Vector3 up = fishingRod.Hook.position - transform.position;
		transform.rotation = Quaternion.LookRotation( Vector3.forward, ((fishingRod.IsHooked && !controller.collisions.below )? up : Vector3.up));


		if (controller.collisions.below || fishingRod.IsHooked)
			isJumping = false;
		
		if (controller.collisions.above || controller.collisions.below || fishingRod.IsHooked)
			velocity.y = 0;

		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		


		if (!fishingRod.IsHooked)
		{
			if (Input.GetButtonDown("Jump") && controller.collisions.below)
			{
				isJumping = true;
				velocity.y = jumpVelocity;
			}

			float targetVelocityX = input.x * moveSpeed;
			velocity.x = Mathf.SmoothDamp(
				velocity.x,
				targetVelocityX,
				ref smoothedXVelocity,
				(controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirbourne);

		}
		else
		{
			ApplyForce(transform.right * hookBalanceForce *input.x);
			if (input.y < 0f)
			{
				Debug.Log("Increasing length");
//				Vector2 vec2Up = transform.up;
//				transform.Translate(input.y * fishingRod.PullingSpeed * Time.deltaTime * transform.up);
//				velocity += input.y * vec2Up * fishingRod.PullingSpeed * Time.deltaTime;
			}
		}



		
		velocity += acceleration * Time.deltaTime;
		velocity += ((isJumping)? jumpGravity  : gravity) * Vector2.down * Time.deltaTime;
		acceleration = Vector2.zero;
		
		
		if (fishingRod.IsHooked)
		{
			//Modify velocity
			LimitHookAngle(position + velocity * Time.deltaTime);
		}
		
		Vector2 testPos = position + velocity * Time.deltaTime;
		
		if (fishingRod.IsHooked)
		{
			testPos = HandleHook(testPos);
		}
		Vector2 constrainedVel = controller.Collide(testPos - position);

		velocity = constrainedVel / Time.deltaTime;
		transform.Translate(constrainedVel);
	}

	private void ApplyForce(Vector2 force)
	{
		acceleration += force / mass;
	}

	private Vector2 HandleHook(Vector2 testPosition)
	{
		Vector2 hookPosition = new Vector2(fishingRod.Hook.position.x, fishingRod.Hook.position.y);
		if (Vector2.Distance(testPosition, hookPosition) > fishingRod.RopeLength)
		{
			testPosition = hookPosition + (testPosition - hookPosition).normalized * fishingRod.RopeLength;
		}

		return testPosition;

	}

	private void LimitHookAngle(Vector2 testPosition)
	{
		Vector2 hookPosition = new Vector2(fishingRod.Hook.position.x, fishingRod.Hook.position.y);
		Vector2 hookLine = (testPosition - hookPosition);
		
		if (Vector2.Angle(Vector2.down, hookLine) > hookMaxAngle)
		{
			acceleration = -2 *acceleration;
			velocity += acceleration * Time.deltaTime;
		}
	}
}