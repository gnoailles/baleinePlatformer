using UnityEngine;

[RequireComponent(typeof(Controller))]
public class Player : MonoBehaviour
{
	
	
	[SerializeField] private FishingRod fishingRod;
	
	
	[SerializeField] private float gravity = 20f;
	[SerializeField] private float moveSpeed = 5f;
	[SerializeField] private float accelerationTimeAirbourne = 0.2f;
	[SerializeField] private float accelerationTimeGrounded = 0.1f;

	[Space(20)]
	
	[SerializeField] private float jumpHeight = 4f;
	[SerializeField] private float timeToJumpApex = .4f;
	
	private Controller controller;
	private Rigidbody rigidbody;
	private Vector2 velocity;
	private Vector2 position;
	private float jumpGravity;
	private float jumpVelocity;

	private float smoothedXVelocity;
	private bool isJumping;

	void Start()
	{
		controller = GetComponent<Controller>();
		rigidbody = GetComponent<Rigidbody>();
		jumpGravity = (2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
		jumpVelocity = jumpGravity * timeToJumpApex;
	}

	private void Update()
	{
		position = transform.position;
		
		Vector3 up = fishingRod.HookInfos.hook.position - transform.position;
		transform.rotation = Quaternion.LookRotation( Vector3.forward, ((fishingRod.HookInfos.state == FishingRod.HookState.HOOKED && !controller.collisions.below )? up : Vector3.up));


		if (controller.collisions.below || fishingRod.HookInfos.state == FishingRod.HookState.HOOKED)
			isJumping = false;
		
		if (controller.collisions.above || controller.collisions.below || fishingRod.HookInfos.state == FishingRod.HookState.HOOKED)
			velocity.y = 0;

		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		


		if (fishingRod.HookInfos.state == FishingRod.HookState.IDLE)
		{
			rigidbody.isKinematic = true;
			
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
			rigidbody.isKinematic = false;
		}

		velocity += ((isJumping)? jumpGravity  : gravity) * Vector2.down * Time.deltaTime;

		Vector2 testPos = position + velocity * Time.deltaTime;

		Vector2 constrainedVel = controller.Collide(testPos - position);

        if(Time.deltaTime > 0)
		    velocity = constrainedVel / Time.deltaTime;

		transform.Translate(constrainedVel);
	}
}