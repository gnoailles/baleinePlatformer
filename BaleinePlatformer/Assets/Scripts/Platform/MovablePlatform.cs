using UnityEngine;

public class MovablePlatform : PullableObject {

    [SerializeField] Transform playerPos;
    private Vector3 bufferPlayerPosition;

	void FixedUpdate () {
        if (isPulling)
        {
            transform.position = new Vector3(playerPos.position.x - bufferPlayerPosition.x, transform.position.y, 0);
        }
	}

    public override void Pull()
    {
        base.Pull();
        bufferPlayerPosition = playerPos.position - transform.position;
        
    }
    public override void Detach()
    {
        base.Detach();
    }
}
