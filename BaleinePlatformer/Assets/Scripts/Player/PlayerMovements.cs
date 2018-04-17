using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour {

    [SerializeField] private string xAxis = "Horizontal";
    [SerializeField] private string jumpAxis = "Jump";
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpHeight = 5.0f;

    private Rigidbody playerRigidbody;
    private Vector3 gravity = Vector3.zero;
    private Vector3 jumpDirection = Vector3.zero;
    private float X = 0.0f;
    private float Y = 0.0f;
    private float jumpForce = 0.0f;
    private bool isGrabbed;

    private void Start()
    {        
        playerRigidbody = gameObject.GetComponent<Rigidbody>();
        jumpForce = Mathf.Sqrt((jumpHeight) * 2f * gravity.magnitude) * playerRigidbody.mass;
        gravity = Physics.gravity;
    }

    void Update ()
    {
        X = Input.GetAxisRaw(xAxis);
        Y = Input.GetAxisRaw(jumpAxis);
	}

    void Jump()
    {    
        if (Input.GetButton(jumpAxis) && Physics.Raycast(transform.position, Vector3.down, 0.5f) )
        {

#if UNITY_EDITOR
            jumpForce = Mathf.Sqrt((jumpHeight) * 2f * gravity.magnitude) * playerRigidbody.mass;
#endif
            playerRigidbody.AddForce(jumpForce * transform.up, ForceMode.Impulse);
                    
        }
    }
    void MovePlayer(float horiz)
    {
        Vector3 moveDirection = playerRigidbody.velocity;
        moveDirection.x = horiz * speed;
        playerRigidbody.velocity = moveDirection ;       
    }
    void FixedUpdate()
    {
        if (!isGrabbed)
        {
            MovePlayer(X);
            Jump();
        }
    }

    public bool IsGrabbed
    {
        get { return isGrabbed; }
        set { isGrabbed = value; }
    }
}
