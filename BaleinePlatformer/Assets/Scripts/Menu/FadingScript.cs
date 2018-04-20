using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FadingScript : MonoBehaviour {

    [SerializeField] private float secondsBeforeFading = 3f;
    [SerializeField] private float speedFading = 3f;
    private bool mustFade = false;

    // Use this for initialization
    void Awake ()
    {
            StartCoroutine("Fading");
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (mustFade)
            Fade();
        if (gameObject.GetComponent<Image>().color.a <= 0.01f)
            gameObject.SetActive(false);
    }

    IEnumerator Fading()
    {
        yield return new WaitForSeconds(secondsBeforeFading);
        mustFade = true;
    }

    void Fade()
    {
        Color temp = gameObject.GetComponent<Image>().color;
        temp.a -= speedFading / 255;
        gameObject.GetComponent<Image>().color = temp;
    }
}
