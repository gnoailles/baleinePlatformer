using System;
using System.Collections;
using UnityEngine;

public class FishingRod : MonoBehaviour
{

	public enum HookState
	{
		IDLE,
		HOOKED,
		PULLING
	}

	[Serializable]
	public struct HookInfo
	{
		[SerializeField] public Transform hook;
		[HideInInspector] public HookState state;
		[HideInInspector] public GameObject hookedObject;
	}
	
	[SerializeField] private Camera playerCamera;
	[SerializeField] private Transform fishingRodEnd;
	[SerializeField] private HookInfo hookInfo;
	
	[SerializeField] private string[] collisionMasks;
//	[SerializeField] private Transform CrossHair;

	[Space(20)] [Tooltip("Hook launch animation duration in seconds")] [SerializeField]
	private float hookLaunchAnimDuration = 0.5f;

	[Tooltip("Fishing hook travel time")] [SerializeField]
	private float hookTravelTime = 4f;

	[Tooltip("Minimum pole string length")] [SerializeField]
	private float minLength = 2f;

	[Tooltip("Maximum pole string length")] [SerializeField]
	private float maxLength = 100;

	[Tooltip("Minimum distance to hooked object")] [SerializeField]
	private float minHookDistance = 3f;

	[Tooltip("Height change speed in unit/s")] [SerializeField]
	private float pullingSpeed = 1f;

	[Tooltip("Strength of the balancement force added when hooked")] [SerializeField]
	private float balanceForce = 1f;

	private ConfigurableJoint playerJoint;
	private SoftJointLimit jointLimit;
	
	private LineRenderer lineRenderer;
	private Vector3 cameraDist;
	private Plane hitPlane;

	private int collisionMask;
	
	private float ropeLength;
//	private bool isHooked;


	void Start()
	{
		cameraDist = new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y, transform.position.z);
		hitPlane = new Plane(Vector3.forward, cameraDist);
		lineRenderer = fishingRodEnd.GetComponent<LineRenderer>();
		collisionMask = LayerMask.GetMask(collisionMasks);
		hookInfo.state = HookState.IDLE;
	}

	void Update()
	{
		RotateRod();

		if (Input.GetButtonDown("Fire1"))
			LaunchGrappling();
		else if (Input.GetButtonUp("Fire1"))
			DetachHook();


		Rigidbody playeRigidbody = transform.root.GetComponent<Rigidbody>();
		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");

		if (hookInfo.hook.gameObject.activeSelf)
		{
			DrawLine();
		}

		if (hookInfo.state == HookState.HOOKED)
		{
			playeRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | 										 RigidbodyConstraints.FreezeRotationY |
			                             RigidbodyConstraints.FreezePositionZ;
			playeRigidbody.AddForce(Vector3.right * x * balanceForce);

			if (playerJoint)
			{
				jointLimit.limit -= y * pullingSpeed * Time.deltaTime;
				jointLimit.limit = Mathf.Clamp(jointLimit.limit, minLength, maxLength);
				playerJoint.linearLimit = jointLimit;
			}
		}
		else
		{
			playeRigidbody.freezeRotation = true;
		}
	}
	
	private void RotateRod()
	{
		if (hookInfo.state == HookState.HOOKED)
		{
			transform.LookAt(hookInfo.hook.position);
		}
		else
		{
			Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

			float enter;

			if (hitPlane.Raycast(ray, out enter))
			{
				Vector3 hitPoint = ray.GetPoint(enter);
				transform.LookAt(hitPoint);
			}
		}
	}


	private void LaunchGrappling()
	{
		StartCoroutine(GrapplingTravel());
	}

	private IEnumerator GrapplingTravel()
	{
		Debug.DrawLine(transform.root.position, transform.forward * maxLength, Color.red);

		RaycastHit hit;
		Vector3 hookHitPos = transform.root.position + transform.forward * maxLength;
		bool isGrappable = false;

		if (Physics.Raycast(transform.root.position, transform.forward, out hit, maxLength))
		{
			Debug.Log(hit.collider.gameObject.layer);
			if (((1 << hit.collider.gameObject.layer) & collisionMask) != 0 &&
			    Vector3.Distance(hit.point, transform.root.position) > minHookDistance)
			{
				isGrappable = true;
				hookHitPos = hit.point;
				hookInfo.hookedObject = hit.collider.gameObject;
			}

			
		}

		yield return new WaitForSeconds(hookLaunchAnimDuration);

		hookInfo.hook.position = fishingRodEnd.position;
		hookInfo.hook.gameObject.SetActive(true);

		float currentLerpTime = 0f;

		while ((hookInfo.hook.position - hookHitPos).magnitude >= 0.5f)
		{
			currentLerpTime += Time.deltaTime;
			if (currentLerpTime > hookTravelTime)
				currentLerpTime = hookTravelTime;

			float easeInTime = Utils.Easing.Exponential.In(currentLerpTime / hookTravelTime);

			hookInfo.hook.position = Vector3.Lerp(hookInfo.hook.position, hookHitPos,
				easeInTime);
			yield return null;
		}

		if (!isGrappable)
		{
			DetachHook();
			yield break;
		}
		
		if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Pullable"))
		{
			hookInfo.state = HookState.PULLING;
			hit.collider.gameObject.GetComponent<PullableObject>().Pull();
		}
		else
		{

			hookInfo.state = HookState.HOOKED;
			hookInfo.hook.transform.rotation = Quaternion.identity;

			SetJoint();

			ropeLength = Vector3.Distance(transform.root.position, hookInfo.hook.position);
		}
	}

	private void SetJoint()
	{
		playerJoint = transform.root.gameObject.AddComponent<ConfigurableJoint>();
		playerJoint.connectedBody = hookInfo.hook.GetComponent<Rigidbody>();
		playerJoint.axis = Vector3.back;
		playerJoint.anchor = Vector3.zero;


		playerJoint.autoConfigureConnectedAnchor = false;
		playerJoint.connectedAnchor = Vector3.zero;

		playerJoint.xMotion = ConfigurableJointMotion.Limited;
		playerJoint.yMotion = ConfigurableJointMotion.Limited;
		playerJoint.zMotion = ConfigurableJointMotion.Locked;


		jointLimit = new SoftJointLimit
		{
			limit = Vector3.Distance(hookInfo.hook.transform.position, transform.root.position) + 1f,
			contactDistance = 1f
		};
		playerJoint.linearLimit = jointLimit;
	}

	private void DetachHook()
	{
		hookInfo.hook.position = fishingRodEnd.position;
		hookInfo.hook.gameObject.SetActive(false);
		StopAllCoroutines();
		if (playerJoint != null) Destroy(playerJoint);

		transform.root.rotation = Quaternion.Euler(0, 0, transform.root.rotation.z);
		if (hookInfo.state == HookState.PULLING)
			hookInfo.hookedObject.GetComponent<PullableObject>().Detach();
		hookInfo.state = HookState.IDLE;
		lineRenderer.enabled = false;
		lineRenderer.positionCount = 0;
	}

	private void DrawLine()
	{
		lineRenderer.enabled = true;
		lineRenderer.positionCount = 2;
		lineRenderer.SetPosition(0, fishingRodEnd.position);
		lineRenderer.SetPosition(1, hookInfo.hook.position);
	}


	#region Fields


	public HookInfo HookInfos
	{
		get { return hookInfo; }
	}

	public float RopeLength
	{
		get { return ropeLength; }
	}

	public float PullingSpeed
	{
		get { return pullingSpeed; }
	}

	#endregion
}