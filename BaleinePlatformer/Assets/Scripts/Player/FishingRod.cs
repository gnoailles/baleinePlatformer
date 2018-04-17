using System.Collections;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.Collections;

namespace Player
{
	public class FishingRod : MonoBehaviour
	{
		[SerializeField] private Camera playerCamera;
		[SerializeField] private Transform fishingRodEnd;
		[SerializeField] private GameObject hook;
		

		[Space(20)]
		
		[Tooltip("Hook launch animation duration in seconds")]
		[SerializeField] private float hookLaunchAnimDuration = 0.5f;
		[Tooltip("Fishing Hook travel time")]
		[SerializeField] private float hookTravelTime = 4f;
		
		[Tooltip("Minimum pole string length")]
		[SerializeField] private float minLength = 1f;
		[Tooltip("Maximum pole string length")]
		[SerializeField] private float maxLength = 100;
		[Tooltip("Minimum distance to hooked object")]
		[SerializeField] private float minHookDistance = 3f;
		[Tooltip("Height change speed in unit/s")]
		[SerializeField] private float pullingSpeed = 1f;
		[Tooltip("Strength of the balancement force added when hooked")]
		[SerializeField] private float balanceForce = 1f;

		private Vector3 cameraDist;
		private Plane hitPlane;

		private ConfigurableJoint playerJoint;
		private SoftJointLimit jointLimit;

		void Start()
		{
			cameraDist = new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y, transform.position.z);
			hitPlane = new Plane(Vector3.forward, cameraDist);
		}


		void Update()
		{
			RotateRod();

			if (Input.GetButtonDown("Fire1"))
				LaunchGrappling();
			else if (Input.GetButtonUp("Fire1"))
				DetachGrappling();

			
			Rigidbody playeRigidbody = transform.root.GetComponent<Rigidbody>();
			float x =Input.GetAxisRaw("Horizontal");
			float y =Input.GetAxisRaw("Vertical");
			
			if (hook.activeSelf)
			{
				playeRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ; 
				playeRigidbody.AddForce(Vector3.right * x * balanceForce);

				if (playerJoint)
				{
					jointLimit.limit -= y * pullingSpeed * Time.deltaTime;
					jointLimit.limit = Mathf.Max(jointLimit.limit, minLength);
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
			if (hook.activeSelf)
			{
				transform.LookAt(hook.transform);
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
			Debug.DrawLine(fishingRodEnd.position, transform.forward * maxLength, Color.red);

			RaycastHit hit;
			Vector3 hookHitPos = fishingRodEnd.position + transform.forward * maxLength;
			bool isGrappable = false;

			if (Physics.Raycast(fishingRodEnd.position, transform.forward, out hit, maxLength))
			{
				if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grappable") &&
				    Vector3.Distance(hit.point, transform.root.position) > minHookDistance)
				{
					isGrappable = true;
					hookHitPos = hit.point;
				}
			}

			yield return new WaitForSeconds(hookLaunchAnimDuration);
			
			hook.transform.position = fishingRodEnd.position;
			hook.SetActive(true);

			float currentLerpTime = 0f;

			while ((hook.transform.position - hookHitPos).magnitude >= 0.5f)
			{
				currentLerpTime += Time.deltaTime;
				if (currentLerpTime > hookTravelTime)
					currentLerpTime = hookTravelTime;

				float easeInTime = Utils.Easing.Exponential.In(currentLerpTime / hookTravelTime);
				
				hook.transform.position = Vector3.Lerp(hook.transform.position, hookHitPos,
					easeInTime);
				yield return null;
			}

			if (!isGrappable) yield break;
			
			hook.transform.rotation = Quaternion.identity;
			SetJoint();
		}

		private void SetJoint()
		{
			playerJoint = transform.root.gameObject.AddComponent<ConfigurableJoint> ();
			playerJoint.connectedBody = hook.GetComponent<Rigidbody>();
			playerJoint.axis = Vector3.back;
			playerJoint.anchor = Vector3.zero;
			

			playerJoint.autoConfigureConnectedAnchor = false;
			playerJoint.connectedAnchor = Vector3.zero;
			
			playerJoint.xMotion = ConfigurableJointMotion.Limited;
			playerJoint.yMotion = ConfigurableJointMotion.Limited;
			playerJoint.zMotion = ConfigurableJointMotion.Locked;


			jointLimit = new SoftJointLimit
			{
				limit = Vector3.Distance(hook.transform.position,transform.root.position) + 1f,
				contactDistance = 1f
			};
			playerJoint.linearLimit = jointLimit;
		}

		private void DetachGrappling()
		{
			hook.transform.position = fishingRodEnd.position;
			StopAllCoroutines();
			hook.SetActive(false);
			if(playerJoint != null) Destroy(playerJoint);
			
//			TODO Reset Orientation on ground
//			if(hook.active)
				transform.root.rotation = Quaternion.Euler(0, 0, transform.root.rotation.z);
		}
	}
}