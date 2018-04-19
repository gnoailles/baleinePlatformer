using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[Tooltip("Player followed by camera")]
	[SerializeField] private GameObject player;
	
	[Tooltip("Adjustable camera offset on x-axis")]
	[SerializeField] private float xOffset;
	
	[Tooltip("Delay for the camera to catch up with player position in seconds")]
	[SerializeField] private float delay = 0.5f;
	
	void Start () {
		xOffset += transform.position.x - player.transform.position.x;
	}

	void LateUpdate () 
	{
		transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x + xOffset, 1f / delay * Time.deltaTime), transform.position.y, transform.position.z);
	}
}
