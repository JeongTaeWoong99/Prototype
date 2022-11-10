using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
	[HideInInspector]
	public SpriteRenderer theBody;
	[HideInInspector]
	public Rigidbody2D theRB;
	[HideInInspector]
	public Animator	   animator;
	[Header("Common")]
	public  float	   moveSpeed;           
	public  int		   currentHealth;       
	public  int		   maxHealth;			
	public  int        penalty;				
	public  float	   rangeToChasePlayer;  
	private float	   moveDirection;

	public  float      wanderLength,  pauseLength;     
	private float      wanderCounter, pauseCounter;	
	private float      wanderDirection;                  

	public LayerMask  whatIsLayer;                     
	public int		  damageToGive = 1;                

	public GameObject deathAnim;                       

	public Slider healthSlider;

	public float attackDistance;		               

	[Header("Knight")]
	public GameObject damagePoint;                     
	[Header("Archer")]
	public Transform  shootPoint;                      
	public GameObject arrowPrefab;					   
	
	[HideInInspector]
	public bool         takeHitState   = false; 
	[HideInInspector]
	public bool         attackState   = false;

	public bool weaknessState = false;

	
	
	private void Awake()
	{
		theRB	 = GetComponent<Rigidbody2D>();
		theBody  = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		pauseCounter = Random.Range(pauseLength * 0.75f, pauseLength * 1.25f);	// 초기 퍼즈카운터
		if (damagePoint == true)		// Knight 공격거리
		{
			attackDistance = Random.Range(0.3f, 0.8f);
		}
		else if(shootPoint == true)	// Archer 공격거리
		{
			attackDistance = Random.Range(2f, 3f);
		}
		
		currentHealth		  = maxHealth;
		currentHealth        -= penalty;			// 패널티
		healthSlider.maxValue = maxHealth;							   
		healthSlider.value	  = currentHealth;						  
	}

	private void Update()
	{
		StateCheckTimer();
		// 해당 if 및 wander상태 아닌 경우
		moveDirection = 0.0f;
		
		if (PlayerController.instance.gameObject.activeInHierarchy && UIStoryTalk.instance.storyTalkEndState == true)
		{
			if (attackState == true && takeHitState == true && PlayerController.instance.gameObject.activeInHierarchy == true)
			{
				// 쫓기 범위 안(이동방향설정)
				if ((Vector2.Distance(transform.position, PlayerController.instance.transform.position) < rangeToChasePlayer))
				{
					moveDirection = PlayerController.instance.transform.position.x - transform.position.x;
				}
				// 범위밖 (wander 상태)
				else
				{
					// 원덜 카운트 작동(랜덤할당 wanderDirection = moveDirection으로 걸어다님), 끝나면 pauseCounter
					if (wanderCounter > 0)
					{
						wanderCounter -= Time.deltaTime;
						moveDirection  = wanderDirection;

						if (wanderCounter <= 0)
						{
							pauseCounter = Random.Range(pauseLength * 0.75f, pauseLength * 1.25f);
						}
					}
					
					// 퍼즈 카운터 작동 및 카운터, 끝나면 방향 + 시간 초기화
					if (pauseCounter > 0)
					{
						pauseCounter -= Time.deltaTime;

						if (pauseCounter <= 0)
						{
							wanderCounter   = Random.Range(wanderLength * 0.75f, wanderLength * 1.25f);
							wanderDirection = Random.Range(-1.0f, 1.0f);
						}
					}

				}
				
				// 공격 범위 안(공격)
				if ((Vector2.Distance(transform.position, PlayerController.instance.transform.position) <= attackDistance)) 
				{
					animator.SetTrigger("Attack");
				}

			}
			
			// 좌우반전
			if (moveDirection < 0.0f)
			{
				transform.localScale = new Vector2(-1f, 1f);
				moveDirection = -1.0f;
			}
			else if (moveDirection > 0.0f)
			{
				transform.localScale = new Vector2(1f,1f);
				moveDirection = 1.0f;
			}
			else if (moveDirection == 0.0f)
			{
				var transform1 = transform;
				transform1.localScale = transform1.localScale; ;
			}
			
			// 최종 이동
			theRB.velocity = new Vector2(moveDirection * moveSpeed, theRB.velocity.y);
			
			// Walking 상태 전환
			if (moveDirection != 0.0f)
			{
				animator.SetBool("Walking", true);
			}
			else
			{
				animator.SetBool("Walking", false);
			}
		}
		else
		{
			animator.SetBool("Walking", false);
			theRB.velocity = Vector2.zero;
		}
	}
    
	public void DamageEnemy(int damage)
	{
		currentHealth -= damage;
		animator.SetTrigger("TakeHit");
		moveDirection = 0.0f;
		
		//너백
		int random = Random.Range(300, 500);
		if(PlayerController.instance.gameObject.transform.position.x - transform.position.x >= 0)
			theRB.AddForce(-transform.right * random);
		else
			theRB.AddForce(transform.right * random);

		if (currentHealth <= 0)
		{
			Destroy(gameObject);
			var transform1 = transform;
			deathAnim.transform.localScale = transform1.localScale;
			Instantiate(deathAnim,transform1.position, Quaternion.identity);
		}

		healthSlider.value = currentHealth;
	}

	// 파괴가능 오브젝트 충돌 시
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.CompareTag("Object"))
		{
			animator.SetTrigger("Attack");
		}
	}

	private void StateCheckTimer()
	{
		// 공격
		if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")))          
		{
			attackState = false;
		}
		else
		{
			attackState = true;              
		}
		
		// 타격
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit"))
		{
			takeHitState = false;          //    타격 중
		}
		else
		{
			takeHitState = true;		   //    타격 중 X
		}
	}

	public void MakeLaser()
	{
		// if(transform.localScale.x > 0)
		// {
		// 	int angle = Random.Range(0, 30);
		// 	Instantiate(arrowPrefab, shootPoint.position, Quaternion.Euler(0, 0, angle));     // 0 ~ 30
		// }
		// else if (transform.localScale.x <= 0)
		// {
		// 	int angle = Random.Range(150, 180);
		// 	Instantiate(arrowPrefab, shootPoint.position, Quaternion.Euler(0, 0,  angle)); // 210~ 240
		// }
		
		if(transform.localScale.x > 0)
		{
			Instantiate(arrowPrefab, shootPoint.position, Quaternion.Euler(0, 0, 0));     // 0 ~ 30
		}
		else if (transform.localScale.x <= 0)
		{
			Instantiate(arrowPrefab, shootPoint.position, Quaternion.Euler(0, 0,  180)); // 210~ 240
		}
	}

	public void Hit()
	{
		// Player 체크
		Collider2D[] hit = Physics2D.OverlapBoxAll(damagePoint.transform.position, new Vector2(0.8f, 0.3f), 0, whatIsLayer);
		for (var i = 0; i < hit.Length; ++i)
		{
			// 플레이어
			if (hit[i].GetComponent<PlayerHpController>() == true)
			{
				// PlayerHpController.instance.hitTranfrom = gameObject.transform;			// 피격 위치 전달
				hit[i].GetComponent<PlayerHpController>().DamagePlayer(gameObject,1);
			}
			
			// 오브젝트
			if(hit[i].GetComponent<Breakables>() == true)
				hit[i].GetComponent<Breakables>().Smash();
		}

	}

	void OnDrawGizmos()
	{
		if (damagePoint != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(damagePoint.transform.position, new Vector2(0.8f, 0.3f));
		}
	}

	public void WeaknessStart()
	{
		weaknessState = true;
	}
	
	public void WeaknessEnd()
	{
		weaknessState = false;
	}
}