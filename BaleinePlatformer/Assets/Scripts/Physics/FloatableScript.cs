using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatableScript : MonoBehaviour {

    [Tooltip("Plus la valeur est grande plus le rebond sera petit")]
    [SerializeField] private float bounceDamp;
    [SerializeField] private float floatLine;
    [SerializeField] private float forceFactor = 0.5f;
    [SerializeField] Vector3 buoyancyCentreOffset = new Vector3(0, 0, 0);
    private Vector3 actionPoint;
    private Vector3 uplift;
 
    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.transform.position.y < -floatLine)
        {
            float height = transform.position.y;
            float waterHeight = transform.position.y;
            actionPoint = collision.transform.position + collision.transform.TransformDirection(buoyancyCentreOffset);
            float force = forceFactor - ((actionPoint.y - waterHeight) / (height));

            if (force > 0f)
            {
                uplift = -Physics.gravity * (force - collision.GetComponent<Rigidbody>().velocity.y * bounceDamp);
                collision.GetComponent<Rigidbody>().AddForceAtPosition(uplift, actionPoint);
            }
        }
     
    }

}
