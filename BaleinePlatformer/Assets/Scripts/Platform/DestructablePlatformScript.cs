using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructablePlatformScript : MonoBehaviour
{

    [SerializeField] float timeBeforeDestruction = 0.0f;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
             StartCoroutine("Destruction");
    }

    IEnumerator Destruction()
    {
        yield return new WaitForSeconds(timeBeforeDestruction); 
        Destroy(gameObject);
    }
}
