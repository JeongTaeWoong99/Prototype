using Unity.Mathematics;
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
	
	private bool    isSlope;                      // 평지판단
	private float   angle;						  // 지면과의 각도
	private Vector2 perepndi;					  // 오르막길 대각선 각도
	public  float   distance;                     // 표시해줄 선 거리
	public  float   maxangle;					  // 오르막제한각도
	public  LayerMask  groundLayer;				  // 그라운드레이어                     
	
	// [HideInInspector]
	// public bool         lotasionState   = false;
	public GameObject   archerHead;
	public GameObject   archergunArm;
	// private Vector3     dir;
	// private float	   bodyAngle;
	//public int rotateSpeed;

	public GameObject alterLaser;

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
					if(knightDamagePoint) 
						moveDirection = PlayerController.instance.transform.position.x - transform.position.x;
					else if (archerShootPoint)
					{
						if (Vector2.Distance(transform.position, PlayerController.instance.transform.position) <= 1.0f)
						{
							moveDirection = PlayerController.instance.transform.position.x - transform.position.x;
							moveDirection *= -1f;
						}
						else
						{
							moveDirection = PlayerController.instance.transform.position.x - transform.position.x;
						}
					}
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
				if (moveDirection < 0.0f)	//왼쪽
				{
					transform.localScale = new Vector2(-1f, 1f);
					if (archerShootPoint)
					{
						archergunArm.transform.localScale = new Vector2(1f, 1f);
						archerHead.transform.localScale   = new Vector2(1f, 1f);
					}
					
					moveDirection = -1.0f;
				}
				else if (moveDirection > 0.0f)	// 오른쪽(본방향)
				{
					transform.localScale = new Vector2(1f,1f);
					if (archerShootPoint)
					{
						archergunArm.transform.localScale = new Vector2(-1f, -1f);
						archerHead.transform.localScale   = new Vector2(-1f, -1f);
					}
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
					// 아쳐의 경우
					if (archerShootPoint)
					{
						if (!(Vector2.Distance(transform.position, PlayerController.instance.transform.position) <= 1.0f))
						{
							Vector2 direction = new Vector2(archergunArm.transform.position.x - PlayerController.instance.transform.position.x, archergunArm.transform.position.y - PlayerController.instance.transform.position.y);
							float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
							float originAngle = angle;
							float absAngle = Mathf.Abs(angle);
							if (transform.localScale.x == 1) // 오른쪽 // 따로 이산된 2 분면을 다룬다 90~180 + -90 ~ -180
							{
								if (absAngle > 130f && absAngle < 180)
								{
									archergunArm.transform.rotation = Quaternion.Euler(0, 0, originAngle);
									archerHead.transform.rotation   = Quaternion.Euler(0, 0, originAngle);
									Instantiate(alterLaser, archerShootPoint.position, quaternion.identity); // 라인렌더러의 디렉션으로 돌아감
									animator.SetTrigger("Attack");
								}
							}
							else if (transform.localScale.x == -1) // 왼쪽 // 연속된 2분면을 다룬다 90~ -90
							{
								if (absAngle > 0f && absAngle < 50)
								{
									archergunArm.transform.rotation = Quaternion.Euler(0, 0, originAngle);
									archerHead.transform.rotation   = Quaternion.Euler(0, 0, originAngle);
									Instantiate(alterLaser, archerShootPoint.position, quaternion.identity);              // 라인렌더러의 디렉션으로 돌아감
									animator.SetTrigger("Attack");
								}
							}
						}
					}
					// 나이트의 경우
					else if (knightDamagePoint)
					{
						animator.SetTrigger("Attack");
					}
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
		currentHealth -= damage * stateNum;						            // 상태에 따라 데미지 들어가는 값 다르게
		animator.SetTrigger("TakeHit_" + stateNum);		            // stateNum에 따라 피격모션 다르게
		moveDirection = 0.0f;
		
		if(archerShootPoint)
			BodyDeactive();
		
		//너백
		int random = Random.Range(300, 500);
		if (PlayerController.instance.gameObject.transform.position.x - transform.position.x >= 0
		    && !isSlope)
			theRB.AddForce(-transform.right * random* perepndi * -1);
		else
			theRB.AddForce(transform.right * random * perepndi * -1);

		if (currentHealth <= 0)
		{
			Destroy(gameObject);
			var transform1 = transform;
			deathAnim.transform.localScale = transform1.localScale;
			Instantiate(deathAnim, transform1.position, Quaternion.identity);

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
					if (transform.localScale.x == 1 && !isSlope)
						theRB.AddForce(transform.right  * 2f * perepndi * -1);
					else if(transform.localScale.x == -1 && !isSlope)
						theRB.AddForce(-transform.right * 2f  * perepndi * -1);
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
		
		// 미끄러짐 방지 및 오르막길 판단
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
		{
			theRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
		}
		else
		{
			theRB.constraints = RigidbodyConstraints2D.FreezeRotation;
		}
        
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distance, groundLayer);      // 몸 기준으로 아래로 선을 그려서, distance만큼 표시해 주고, groundLayer랑 닿아서, 상호작용 하고

		if (hit)
		{
			perepndi = Vector2.Perpendicular(hit.normal).normalized;                                                               // nomal은 닿은 레이어 기준 90도(중앙)인데, Perpendicular는 반시계 90도 (Vector2 타입 반환) -> 언덕을 오를때 곱해줘야 해서 normalized
			// Perpendicular 반시계 음수 백터값을 반환함
			angle    = Vector2.Angle(hit.normal, Vector2.up);                                                                      // Angle은 닿은 레이어 기준 중앙                                      (float 타입 반환)

			if (angle != 0)                 // 언덕 판단
				isSlope = true;
			else
			{
				isSlope = false;
			}
		}

	}

	public void MakeLaser()
	{
		Instantiate(arrowPrefab, archerShootPoint.position, archerShootPoint.rotation);
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
		if (archerShootPoint)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, attackDistance);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, rangeToChasePlayer);
			
		}
		
		if (knightDamagePoint)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(knightDamagePoint.transform.position, new Vector2(0.8f, 0.3f));
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, rangeToChasePlayer);
		}
		
		
	}

	public void BodyActive()
	{
		archergunArm.SetActive(true);
		archerHead.SetActive(true);
		archergunArm.GetComponent<Animator>().SetTrigger("Attack");
	}

	public void BodyDeactive()
	{
		archergunArm.SetActive(false);
		archerHead.SetActive(false);
	}
}