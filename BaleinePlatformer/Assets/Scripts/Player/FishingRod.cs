using System.Collections;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.Collections;

public class FishingRod : MonoBehaviour
{
	[SerializeField] private Camera playerCamera;
	[SerializeField] private Transform fishingRodEnd;
	[SerializeField] private Transform hook;
    [SerializeField] private Transform CrossHair;

	[Space(20)]
	
	[Tooltip("Hook launch animation duration in seconds")]
	[SerializeField] private float hookLaunchAnimDuration = 0.5f;
	[Tooltip("Fishing Hook travel time")]
	[SerializeField] private float hookTravelTime = 4f;
	
	[Tooltip("Minimum pole string length")]
	[SerializeField] private float minLength = 2f;
	[Tooltip("Maximum pole string length")]
	[SerializeField] private float maxLength = 100;
	[Tooltip("Minimum distance to hooked object")]
	[SerializeField] private float minHookDistance = 3f;
	[Tooltip("Height change speed in unit/s")]
	[SerializeField] private float pullingSpeed = 1f;

	private LineRenderer lineRenderer;	
	private Vector3 cameraDist;
	private Plane hitPlane;
	
	private float ropeLength;
	private bool isHooked;

	void Start()
	{
		cameraDist = new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y, transform.position.z);
		hitPlane = new Plane(Vector3.forward, cameraDist);
		lineRenderer = fishingRodEnd.GetComponent<LineRenderer>();
	}


	void Update()
	{
		RotateRod();

		if (Input.GetButtonDown("Fire1"))
			LaunchGrappling();
		else if (Input.GetButtonUp("Fire1"))
			DetachHook();

		float y =Input.GetAxisRaw("Vertical");
		if (y != 0f)
		{
			ropeLength -= y * PullingSpeed * Time.deltaTime;
			ropeLength = Mathf.Max(ropeLength, minLength);
		}

		if(hook.gameObject.activeSelf)
			DrawLine();
		
	}

	private void DrawLine()
	{		
		lineRenderer.enabled = true;
		lineRenderer.positionCount = 2;
		lineRenderer.SetPosition(0,fishingRodEnd.position);
		lineRenderer.SetPosition(1,hook.position);
	}

	private void RotateRod()
	{
		if (isHooked)
		{
			transform.LookAt(hook.position);
		}
		else
		{

         // transform.localEulerAngles = new Vector3(Input.GetAxis("Mouse X") * 60, Input.GetAxis("Mouse Y") * 50, transform.localEulerAngles.z);


            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
           // Ray ray = playerCamera.ScreenPointToRay(new Vector3(Input.GetAxis("Mouse X") * 60, Input.GetAxis("Mouse Y") * 50, 0));

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
			if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grappable") &&
				Vector3.Distance(hit.point, transform.root.position) > minHookDistance)
			{
              
                isGrappable = true;
				hookHitPos = hit.point;
			}

            if (hit.collider.tag == "Pullable" &&
                Vector3.Distance(hit.point, transform.root.position) > minHookDistance)
            {
                hit.collider.gameObject.GetComponent<MovablePlatform>().Grabbed();
            }
		}

		yield return new WaitForSeconds(hookLaunchAnimDuration);
		
		hook.position = fishingRodEnd.position;
		hook.gameObject.SetActive(true);
           
        float currentLerpTime = 0f;

		while ((hook.position - hookHitPos).magnitude >= 0.5f)
		{
			currentLerpTime += Time.deltaTime;
			if (currentLerpTime > hookTravelTime)
				currentLerpTime = hookTravelTime;

			float easeInTime = Utils.Easing.Exponential.In(currentLerpTime / hookTravelTime);
			
			hook.position = Vector3.Lerp(hook.position, hookHitPos,
				easeInTime);
			yield return null;
		}          

        if (!isGrappable)
		{
            DetachHook();
            yield break;
		}

        if (hit.collider.tag == "Pullable")
            hit.collider.gameObject.GetComponent<MovablePlatform>().Detach();

        isHooked = true;
		hook.transform.rotation = Quaternion.identity;

        ropeLength = Vector3.Distance(transform.root.position, hook.position);
	}
	
	private void DetachHook()
	{
		hook.position = fishingRodEnd.position;
		isHooked = false;
		StopAllCoroutines();
		hook.gameObject.SetActive(false);
		lineRenderer.enabled = false;
		lineRenderer.positionCount = 0;
	}

	public Transform Hook
	{
		get { return hook; }
	}

	public bool IsHooked
	{
		get { return isHooked; }
	}

	public float RopeLength
	{
		get { return ropeLength; }
	}

	public float PullingSpeed
	{
		get { return pullingSpeed; }
	}
}