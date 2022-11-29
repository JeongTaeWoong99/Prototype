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
	public GameObject knightDamagePoint;                     
	[Header("Archer")]
	public Transform  archerShootPoint;                      
	public GameObject arrowPrefab;					   
	
	[HideInInspector]
	public bool         takeHitState   = false; 
	[HideInInspector]
	public bool         attackState   = false;

	public bool weaknessState;					// true 취약상태 false 기본상태

	//public bool tutoBotBool;					// true 훈련용봇

	private void Awake()
	{
		theRB	 = GetComponent<Rigidbody2D>();
		theBody  = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		pauseCounter = Random.Range(pauseLength * 0.75f, pauseLength * 1.25f);	// 초기 퍼즈카운터
		
		if (knightDamagePoint)		// Knight 공격거리
        {
        	attackDistance = Random.Range(0.3f, 0.8f);
        }
        else if(archerShootPoint)	// Archer 공격거리
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
		
		if (PlayerController.instance.gameObject.activeInHierarchy && UIStoryTalk.instance.storyTalkEndState)
		{
			if (attackState && takeHitState && PlayerController.instance.gameObject.activeInHierarchy)
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
				
				// 공격 범위 안(공격)
				if ((Vector2.Distance(transform.position, PlayerController.instance.transform.position) <= attackDistance))
				{
					animator.SetTrigger("Attack");
				}

			}
			
			// Walking 상태 전환
			if (moveDirection != 0.0f)
			{
				animator.SetBool("Walking", true);
			}
			else
			{
				animator.SetBool("Walking", false);
			}
			
			// 최종 이동
			theRB.velocity = new Vector2(moveDirection * moveSpeed, theRB.velocity.y);
		}
		else
		{
			animator.SetBool("Walking", false);
		}
	}
    
	// stateNum -> 1 기본 / 2 기절 / 
	public void DamageEnemy(int damage, int stateNum)
	{
		currentHealth -= damage * stateNum;						// 상태에 따라 데미지 들어가는 값 다르게
		animator.SetTrigger("TakeHit_" + stateNum);		// stateNum에 따라 피격모션 다르게
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
			Instantiate(deathAnim, transform1.position, Quaternion.identity);

			// 만약 훈련용봇이면
			// if (tutoBotBool)
			// {
			// 	UIAutoSystem.instance.autoEventState = false;																	// 다시 이벤트 이어서
			// 	PlayerController.instance.theRB.gravityScale = PlayerController.instance.originGavityScale;                     // 대쉬중 부딪혔을 때, 멀리나가는 문제 !! ☆★
			// 	UIAutoSystem.instance.autoAtion[UIAutoSystem.instance.currtAtionNum-1].objectE.activeObject.SetActive(false);	// currtAtionNum++ 됐기 때문에, -1한 값을 꺼준다.
			// }
			
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

			// 공격 애니메이션 0.3 ~ 1.0 부분 공격시 앞으로 미끄러짐
			// 근접적 경우에만 적용
			if (knightDamagePoint)		
			{
				if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.3 ||
				    animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8)
				{
					if (transform.localScale.x == 1)
						theRB.AddForce(transform.right  * 2f);
					else
						theRB.AddForce(-transform.right * 2f);
				}
			}
		}
		else
		{
			attackState = true;
		}
		
		// 타격
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit_1") || 
		    animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit_2"))
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
		if(transform.localScale.x > 0)
		{
			Instantiate(arrowPrefab, archerShootPoint.position, Quaternion.Euler(0, 0, 0));     // 0 ~ 30
		}
		else if (transform.localScale.x <= 0)
		{
			Instantiate(arrowPrefab, archerShootPoint.position, Quaternion.Euler(0, 0,  180)); // 210~ 240
		}
	}

	public void Hit()
	{
		// Player 체크
		Collider2D[] hit = Physics2D.OverlapBoxAll(knightDamagePoint.transform.position, new Vector2(0.8f, 0.3f), 0, whatIsLayer);
		for (var i = 0; i < hit.Length; ++i)
		{
			// 플레이어
			if (hit[i].GetComponent<PlayerHpController>())
			{
				hit[i].GetComponent<PlayerHpController>().DamagePlayer(gameObject,1);
			}
			
			// 오브젝트
			if(hit[i].GetComponent<Breakables>())
				hit[i].GetComponent<Breakables>().Smash();
		}

	}

	void OnDrawGizmos()
	{
		if (knightDamagePoint != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(knightDamagePoint.transform.position, new Vector2(0.8f, 0.3f));
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