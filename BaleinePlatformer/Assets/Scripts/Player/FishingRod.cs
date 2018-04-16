using System.Collections;
using UnityEngine;

namespace Player
{
	public class FishingRod : MonoBehaviour
	{
		
		[SerializeField] private Camera playerCamera;
		[SerializeField] private GameObject grappling;
		[SerializeField] private float maxLength = 100;
		[SerializeField] private float launchAnimationDuration = 1f;
		[SerializeField] private float anchorTravelSpeed = 2f;
		
		private Vector3 cameraDist;
		private Plane hitPlane;
		
		void Start () 
		{
			cameraDist = new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y, transform.position.z);
			hitPlane = new Plane(Vector3.forward, cameraDist);
		}
	

		void Update ()
		{
			RotateRod();

			if (Input.GetButtonDown("Fire1"))
				LaunchGrappling();
			else if(Input.GetButtonUp("Fire1"))
				DetachGrappling();
		}

		private void RotateRod()
		{
			Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
			float enter;

			if (hitPlane.Raycast(ray, out enter))
			{
				Vector3 hitPoint = ray.GetPoint(enter);
				transform.LookAt(hitPoint);
			}
		}

		private void LaunchGrappling()
		{
			StartCoroutine(GrapplingTravel());
		}

		private IEnumerator GrapplingTravel()
		{
			
			Vector3 rayOrigin = transform.position;
			RaycastHit hit;
			
			Debug.DrawLine(transform.position, transform.forward * maxLength, Color.red);

			Vector3 grapplingAnchor = transform.position + transform.forward * maxLength;
			
			if (Physics.Raycast(transform.position, transform.forward, out hit, maxLength, 1 << LayerMask.NameToLayer("Grappable")))
			{
				grapplingAnchor = hit.point;
			}
			
			yield return new WaitForSeconds(launchAnimationDuration);
			grappling.SetActive(true);

			while ((grappling.transform.position - grapplingAnchor).magnitude >= 0.5f)
			{
				
				Debug.Log("Animating anchor pos");
				grappling.transform.position = Vector3.Lerp(grappling.transform.position, grapplingAnchor, anchorTravelSpeed * Time.deltaTime);
				yield return null;
			}
		}
		private void DetachGrappling()
		{
			grappling.transform.position = transform.position;
			grappling.SetActive(false);
		}
	}
}
