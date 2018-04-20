using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[Tooltip("Player followed by camera")]
	[SerializeField] private GameObject player;
	
	[Tooltip("Adjustable camera offset on x-axis")]
	[SerializeField] private float xOffset;	
	[Tooltip("Adjustable camera offset on y-axis")]
	[SerializeField] private float yOffset;
	
	[Tooltip("Delay for the camera to catch up with player position in seconds")]
	[SerializeField] private float xDelay = 0.5f;
	[SerializeField] private float yDelay = 0.2f;
	
	[Tooltip("Maximum y difference before camera snaps to player")]
	[SerializeField] private float maxYDifference = 0.5f;
	
	void Start () {
		xOffset += transform.position.x - player.transform.position.x;
		yOffset += transform.position.y - player.transform.position.y;
	}

	void LateUpdate ()
	{
		Vector3 targetPos =
			new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x + xOffset, 1f / xDelay * Time.deltaTime),
				transform.position.y, transform.position.z);
		if (Mathf.Abs(player.transform.position.y - transform.position.y) > maxYDifference)
		{
			float diffSign = Mathf.Sign(player.transform.position.y - transform.position.y);
			targetPos.y = Mathf.Lerp(transform.position.y, player.transform.position.y + yOffset, 1f / yDelay * Time.deltaTime);
		}
		transform.position = targetPos;
	}
}
