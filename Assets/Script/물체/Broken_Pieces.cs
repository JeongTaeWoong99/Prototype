using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Broken_Pieces : MonoBehaviour
{
    public float          maxspeed;
    public float          minSpeed;
    private Rigidbody2D   theRB;

    // public  float          deceleration;  // 감속	   
    // public float           lifetime;	  // 잔여 시간   
    // public  SpriteRenderer theSR;         // 투명도 참조 
    // public  float          fadeSpeed;     // 사라지는 시간

    private void Start()
    {
	    theRB = GetComponent<Rigidbody2D>();

	    float randomSpeed = Random.Range(minSpeed, maxspeed);
	    theRB.AddForce(transform.right * randomSpeed);
    }

    void Update()
    {
	    
	    
	    
	    // transform.position += moveDirection * Time.deltaTime;
     //    moveDirection       = Vector3.Lerp(moveDirection, Vector3.zero, Time.deltaTime * deceleration); // 이동속도 감속
     //                                                                                                          // (moveDirection에서 Vector3.zero로 5.0f 빠르게 감속
                                                                                                              
                                                                                                              
                                                                                                              
  //       lifetime -= Time.deltaTime;
  //
  //       if (lifetime <= 0)
		// {
  //           // 조각 사라짐
  //           // theSR.color.a (1.0f)에서 0.0f 로 변화
  //           theSR.color = new Color(theSR.color.r, theSR.color.g, theSR.color.b,
  //                                   Mathf.MoveTowards(theSR.color.a, 0.0f,fadeSpeed * Time.deltaTime));
  //           if(theSR.color.a == 0.0f)
		// 	{
		// 	    Destroy(gameObject);
		// 	}
		// }
	}
}