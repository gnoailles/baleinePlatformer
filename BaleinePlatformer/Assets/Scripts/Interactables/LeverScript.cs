using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour {

    [SerializeField] private GameObject leverStick;

    private void OnTriggerEnter(Collider other)
    {
        leverStick.transform.localScale = new Vector3(0.5f,5,0.5f);
        leverStick.transform.Translate(new Vector3(0, 2.5f, 0));
    }

}
