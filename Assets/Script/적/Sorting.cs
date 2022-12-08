using System.Collections.Generic;
using UnityEngine;

public class Sorting : MonoBehaviour
{
    private SpriteRenderer theSR;			                                 // 본체
    public List<SpriteRenderer> frontSubBody = new List<SpriteRenderer>(); // 팔 머리 등등

    private void Awake()
    {
	    theSR = GetComponent<SpriteRenderer>();   
    }

    private void FixedUpdate()
    {
        if(gameObject.CompareTag("Enemy"))
			theSR.sortingOrder = Mathf.RoundToInt(transform.position.x * -10.0f);
        else if (gameObject.CompareTag("Object"))
	        theSR.sortingOrder = Mathf.RoundToInt(transform.position.y * 10.0f);

        if (frontSubBody.Count != 0)
        {
	        for (int i = 0; i < frontSubBody.Count; i++)
	        {
		        frontSubBody[i].sortingOrder = theSR.sortingOrder + 1;			// 본체보다 앞에 배치
	        }
        }
	}
}
