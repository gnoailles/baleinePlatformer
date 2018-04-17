using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller))]
public class Player : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 5f;
	[SerializeField] private float accelerationTimeAirbourne = 0.2f;
	[SerializeField] private float accelerationTimeGrounded = 0.1f;

	[SerializeField] private float jumpHeight = 4f;
	[SerializeField] private float timeToJumpApex = .4f;
	

	private float gravity;
	private Controller controller;
	private Vector3 velocity;
	private float jumpVelocity;

	private float smoothedXVelocity;

	void Start()
	{
		controller = GetComponent<Controller>();
		gravity = (2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
	}

	private void Update()
	{
		if (controller.collisions.above || controller.collisions.below)
			velocity.y = 0;

		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (Input.GetButton("Jump") && controller.collisions.below)
		{
			velocity.y = jumpVelocity;
		}

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp(
			velocity.x, 
			targetVelocityX, 
			ref smoothedXVelocity,
			(controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirbourne);
		velocity.y -= gravity * Time.deltaTime;

		controller.Move(velocity * Time.deltaTime);
	}
}