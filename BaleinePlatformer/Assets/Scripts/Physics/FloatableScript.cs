using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatableScript : MonoBehaviour {

    [SerializeField] private float forceFactor;
    [SerializeField] private float waterLevel;
    [SerializeField] private float Height;
    [SerializeField] private float bounceDamp;
    private Vector3 actionPoint;
    private Vector3 uplift;
    [SerializeField] Vector3 buoyancyCentreOffset;

    void Update()
    {
        actionPoint = transform.position + transform.TransformDirection(buoyancyCentreOffset);
        forceFactor = 1f - ((actionPoint.y - waterLevel) / Height);

        if (forceFactor > 0f)
        {
            uplift = -Physics.gravity * (forceFactor - GetComponent<Rigidbody>().velocity.y * bounceDamp);
            GetComponent<Rigidbody>().AddForceAtPosition(uplift, actionPoint);
        }
    }
}
