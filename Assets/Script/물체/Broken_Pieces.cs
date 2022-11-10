using UnityEngine;

public class Broken_Pieces : MonoBehaviour
{
    private Rigidbody2D   theRB;		
    public  float         maxspeed;		  // 날아가는 속도 최대
    public  float         minSpeed;		  // 날아가는 속도 최소
    
    private  SpriteRenderer theSR;         // 투명도 참조 
    public  float          lifetime;	  // 잔여 시간   
    public  float          fadeSpeed;     // 사라지는 시간

    private void Start()
    {
	    theRB = GetComponent<Rigidbody2D>();
	    theSR = GetComponent<SpriteRenderer>();

	    float randomSpeed = Random.Range(minSpeed, maxspeed);
	    theRB.AddForce(transform.right * randomSpeed);
    }

    void Update()
    {

	    lifetime -= Time.deltaTime;
  
        if (lifetime <= 0)
		{
            // 조각 사라짐
            // theSR.color.a가 1.0f에서 0.0f 로 변화
            theSR.color = new Color(theSR.color.r, theSR.color.g, theSR.color.b,
                                    Mathf.MoveTowards(theSR.color.a, 0.0f,fadeSpeed * Time.deltaTime));
            if(theSR.color.a == 0.0f)
			{
			    Destroy(gameObject);
			}
		}
	}
}