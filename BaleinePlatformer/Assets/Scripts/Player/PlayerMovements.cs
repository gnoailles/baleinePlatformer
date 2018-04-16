using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour {

    [SerializeField] private string xAxis = "Horizontal";
    [SerializeField] private string jumpAxis = "Jump";
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpAcceleration = 5.0f;
    [SerializeField] private float jumpSpeed = 15.0f;

    private CharacterController characterController;
    private Vector3 jumpDirection = Vector3.zero;
    private float X = 0.0f;
    private float Y = 0.0f;

    private void Start()
    {        
        characterController = gameObject.GetComponent<CharacterController>();
        characterController.detectCollisions = false;
    }

    void Update ()
    {
        X = Input.GetAxis(xAxis);
        Y = Input.GetAxisRaw(jumpAxis);
	}

    void Jump()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f))
        {
            jumpDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            jumpDirection = transform.TransformDirection(jumpDirection);

            if (Input.GetButton("Jump"))
            {
                jumpDirection *= jumpAcceleration;
                jumpDirection.y = jumpSpeed;
            }
            
        }
    }
    void MovePlayer(float horiz, float vert)
    { 
        Vector3 moveDirection = transform.right * horiz * speed;
        moveDirection += transform.forward * vert * speed;
        characterController.SimpleMove(moveDirection);
        
    }
    void FixedUpdate()
    {
        if (Y != 0 || X != 0)
            MovePlayer(X, Y);

        Jump();
        jumpDirection.y -= 9.81f * Time.deltaTime;
        characterController.Move(jumpDirection * Time.deltaTime);
    }
}
