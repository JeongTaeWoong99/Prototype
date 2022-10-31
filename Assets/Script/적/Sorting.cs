using System;
using UnityEngine;

public class Sorting : MonoBehaviour
{
    private SpriteRenderer theSR;

    private void Awake()
    {
	    theSR = GetComponent<SpriteRenderer>();   
    }

    private void FixedUpdate()
    {
        if(gameObject.CompareTag("Enemy"))
			theSR.sortingOrder = Mathf.RoundToInt(transform.position.x * -10.0f);
        else if(gameObject.CompareTag("Object"))
	        theSR.sortingOrder = Mathf.RoundToInt(transform.position.y * 10.0f);
	}
}
