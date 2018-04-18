using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour {

    [SerializeField] Transform playerPos;
    private Vector3 bufferPlayerPosition;
    private bool isGrabbed = false;

	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isGrabbed)
        {
            transform.position = new Vector3(playerPos.position.x - bufferPlayerPosition.x, transform.position.y, 0);
        }
	}

    public void Grabbed()
    {
        isGrabbed = true;
        bufferPlayerPosition = playerPos.position - transform.position;
        
    }
    public void Detach()
    {
        isGrabbed = false;
        Debug.Log("I HAVE BEEN UNGRABBED");
    }
}
