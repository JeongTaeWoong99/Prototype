using Unity.Mathematics;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
	[HideInInspector]
	public  SpriteRenderer theBody;			// 스캔시 몸체 보이는지 판단
	public  GameObject     boomEffectToPlayer;		
	public  GameObject     boomEffectToEnemy;
	
	public Sprite hackingStateImage;

	public bool beamBool;   
    public bool missileBool;
    public bool dummyBool;

    public  int   damage;
    public  float speed;     
	public  float minusFixedUpdateSpeed;				// FixedUpdate당 감소
	public  float minusMaxValue;						// 값 감속 최저값
	public  float rotSpeed;								// 회전속도
	
    private float	       angle;
    private Rigidbody2D    theRB;
    private Quaternion     rotTarget;
	private Vector3        dir;
	
	[HideInInspector]
	public Transform currentTarget;

	public float anglePivotValue;

	private void Awake()
	{
		theBody = GetComponent<SpriteRenderer>();
		theRB   = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		// 시작시 미사일 추적 대상 설정
		currentTarget = PlayerController.instance.gameObject.transform;

	}

	private void FixedUpdate()
	{
		// 미사일 이동
		if (missileBool && Time.timeScale == 1f && currentTarget == true)
		{
			// 감속
			speed -= minusFixedUpdateSpeed;	
			if (speed <= minusMaxValue) 
				speed = minusMaxValue;

			// 일정범위 안으로 들어오면, 더이상 위치를 따라가는게 아니라 현재 방향으로 그냥 돌진
			// if (Vector2.Distance(PlayerParing.instance.missileAttackPoint.transform.position, transform.position) >= 1f)
			if (Vector2.Distance(currentTarget.transform.position, transform.position) >= 0.5f)
			{
				dir = (currentTarget.transform.position - transform.position).normalized; 
				angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				rotTarget = Quaternion.AngleAxis(angle, Vector3.forward);
				transform.localRotation = Quaternion.Slerp(transform.rotation, rotTarget,rotSpeed);
			}
			
			theRB.AddForce(transform.right * speed);
			theRB.velocity = new Vector2(dir.x * speed * minusFixedUpdateSpeed, dir.y * speed * minusFixedUpdateSpeed);
			
			//해킹상태 일 때, 이미지 번경
			if (currentTarget != PlayerController.instance.gameObject.transform)
			{
				theBody.sprite = hackingStateImage;
			}
		}
		// 광선이동
		else if (beamBool)
		{
			gameObject.transform.position += transform.right * speed * Time.deltaTime;
		}
		// 더미이동
		else if (dummyBool)
		{
			gameObject.transform.position += transform.right * speed * Time.deltaTime;
		}

		// 총알관리
		BulletEliminate();
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
	    // 미사일 충돌 관리
	    if (missileBool)
	    {
		    // 표적(플레이어)
		    if (other.CompareTag("Player") && PlayerHpController.instance.invincibleCount <= 0 
		                                   && currentTarget.transform == PlayerController.instance.transform)
		    {
			    PlayerHpController.instance.DamagePlayer(gameObject,damage);
			    Instantiate(boomEffectToPlayer, gameObject.transform.position, quaternion.identity);
			    Destroy(gameObject);
		    }
		    
		    // 표적(적)
		    if (currentTarget.transform != PlayerController.instance.transform)
		    {
			    // 보스바디 충돌
			    if ((other.CompareTag("BossBody")))
			    {
				    other.GetComponent<BossBodytakeDamage>().TakeDamage(damage);
				    Instantiate(boomEffectToEnemy, gameObject.transform.position, quaternion.identity);
				    Destroy(gameObject);
			    }
			    // 기본적충돌
			    else if((other.CompareTag("Enemy")))
			    {
				    other.GetComponent<EnemyController>().DamageEnemy(damage);
				    Instantiate(boomEffectToEnemy, gameObject.transform.position, quaternion.identity);
				    Destroy(gameObject);
			    }
		    }
		    
		    if (other.CompareTag("Object"))
		    {
			    if(other.GetComponent<Breakables>()) 
				    other.GetComponent<Breakables>().Smash();
			    Instantiate(boomEffectToPlayer, gameObject.transform.position, quaternion.identity);
			    Destroy(gameObject);
		    }
		    
		    if (other.CompareTag("Ground"))
		    {
			    Instantiate(boomEffectToPlayer, gameObject.transform.position, quaternion.identity);
			    Destroy(gameObject);
		    }
	    }
	    
	    // 광선 충돌 관리
	    if (beamBool)
	    {
		    if (other.CompareTag("Player") && PlayerHpController.instance.invincibleCount <= 0)
            {
			    PlayerHpController.instance.DamagePlayer(gameObject,damage);
			    Instantiate(boomEffectToPlayer, gameObject.transform.position, Quaternion.identity);
			    Destroy(gameObject);
            }
       
            if (other.CompareTag("Object"))
            {
	            if(other.GetComponent<Breakables>())
		            other.GetComponent<Breakables>().Smash();
	            
	            Instantiate(boomEffectToPlayer, gameObject.transform.position, Quaternion.identity);
			    Destroy(gameObject);
            }
             
            if (other.CompareTag("Ground"))
            {
	            Instantiate(boomEffectToPlayer, gameObject.transform.position, quaternion.identity);
	            Destroy(gameObject);
            }
	    }
	    
	    if (dummyBool)
	    {
		    // 충돌없음
		    // 하늘로 날라가다 사라질 예정
	    }
    }
    
    // 사망시 모든 투사체들 터지게
    private void BulletEliminate()
    {
	    // 역추적 하다가 대상 사망
	    if (currentTarget == false)
	    {
		    Instantiate(boomEffectToEnemy, gameObject.transform.position, quaternion.identity);
		    Destroy(gameObject);
	    }
	    
	    // 밖으로 멀리 나가면 삭제
	    if(Vector2.Distance(gameObject.transform.position,PlayerController.instance.gameObject.transform.position)>10f)
		    Destroy(gameObject);

	    // 보스방에서 보스 사망
	    // 모든종류 총알 삭제
	    if (UIEvent.instance.bossRoomState && BossHP.instance.currentHP <= 0)
	    {
		    Instantiate(boomEffectToPlayer, gameObject.transform.position, quaternion.identity); 
		    Destroy(gameObject);
	    }
    }
}