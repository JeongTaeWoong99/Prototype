using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

	[HideInInspector]
    public Rigidbody2D    theRB;    
    [HideInInspector]
    public SpriteRenderer bodySR;   
    [HideInInspector]
    public Animator       animator;
    //public Animator       bladeAnimator;
    
    public GameObject     deathPrefabs;                 // 사망 프리팹         

    [HideInInspector]
    public  bool  rollState    = false;                 
    [HideInInspector]
    public  float inputX;                                // Horizontal 
    private float activeMoveSpeed;                      
    public  float moveSpeed;                            
    public  float dashSpeed;                            
    public  float dashCooldown;                         
    private float dashCoolCounter;                      
    private float dashInputX;
    [HideInInspector]
    public float originGavityScale;                     // 원래의 중력 -> 대쉬시 반의 값으로 변환
    
    public  float jumpForce;                            
    //private bool  isLongJump = false;                   
    
    [SerializeField]
    private LayerMask         groundLayer;              
    private CapsuleCollider2D capsuleCollider2D;        
    [HideInInspector]
    public  bool              isGrounded;
    private Vector2           footPosition;             
    private int               maxJumpCount     = 2;     
    private int               currentJumpCount = 0;     

    [HideInInspector]
    public  bool       attackState     = false;        
    //private float      timeSinceAttack = 0.0f;          
    private int        currentAttack   = 0;             
    public  LayerMask  enemyLayer;
    public  GameObject damagePoint;                    
    public  int        damageToGive    = 50;           

    [HideInInspector]
    public bool         takeHitState   = false;        

    private AnimationClip[] clips; 
    private float           dashTime;               // 대쉬 애니메이션 클립 길이

    [HideInInspector]
    public bool  skillState;          // 스킬 사용 중 false // 사용 중 x true     

    [HideInInspector] 
    public bool rightKeyLock;
    [HideInInspector] 
    public bool leftKeyLock;
    [HideInInspector] 
    public bool jumpKeyLock;
    
    private void Awake()
    {
        instance = this;

        theRB             = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        animator          = GetComponent<Animator>();
        bodySR            = GetComponent<SpriteRenderer>();

        activeMoveSpeed   = moveSpeed;
        originGavityScale = theRB.gravityScale;

        // 클립을 모두 받아와서, 이름별로 총 재생길이 저장
        clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)   
        {
            switch (clip.name)
            {
                case "Player_Dash":
                    dashTime = clip.length;
                    break;
            }
        }
    }

    private void Update()
    {
        // 애니메이션 상태체크
        StateCheckTimer();
        
        if (attackState && rollState && takeHitState && skillState && PlayerParing.instance.paringState &&
            UIStoryTalk.instance.storyTalkEndState && UIEvent.instance.eventState && UIAutoSystem.instance.autoEventState && !UIInventory.instance.puaseState)
        {
            //이동
            inputX = Input.GetAxisRaw("Horizontal");                                    // right left +1 -1
            if(inputX ==  1 && rightKeyLock)                                            // 키잠금
                 inputX = 0;
            else if (inputX == -1 && leftKeyLock)
                 inputX = 0;
            theRB.velocity = new Vector2(inputX * activeMoveSpeed, theRB.velocity.y); // 최종이동

            // 좌우반전
            if (inputX > 0)
            {
                transform.localScale = new Vector2(1f, 1f);
            }
            else if (inputX < 0)
            {   
                transform.localScale = new Vector2(-1f, 1f);
            }

            //공격
            if (Input.GetKeyDown(KeyCode.A))
            {
                Attack();
            }

            // 점프
            if (Input.GetKeyDown("up") && !jumpKeyLock)
            {
                Jump();
            }

            // 스킬
            // if (Input.GetKey(KeyCode.Q))
			// {
			//      PlayerSkill.instance.SetSkill("Q");
            // }
            
            // 기본상태 전환 // 모션 울찔울찔 방지 키 조건
            if (theRB.velocity != Vector2.zero && isGrounded == true &&(Input.GetKey("right") || Input.GetKey("left")))
            {
                animator.SetBool("Walking", true);
            }
            else
            {
                animator.SetBool("Walking", false);
            }
            
            // 패링모드
            if (Input.GetKeyDown(KeyCode.Z))
            {
                PlayerParing.instance.Paring();
            }
        }
        // 오토무브를 위한 조건
        else
        {
            // 제어권없는 상태에서 MoveAtion 중 일때
            if (UIAutoSystem.instance.autoEventState == false && theRB.velocity != Vector2.zero)
            {
                animator.SetBool("Walking", true);
            }
            else
            {
                animator.SetBool("Walking", false);
            }
            
        }

        // 대쉬 조건
        if (Input.GetKey(KeyCode.S) && rollState == true && takeHitState == true&& skillState == true &&
            UIStoryTalk.instance.storyTalkEndState == true && UIEvent.instance.eventState == true)
        {
            if (dashCoolCounter <= 0)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    Dash(-1f);
                }
                else if ((Input.GetKey(KeyCode.RightArrow)))
                {
                    Dash(1f);
                }
            }
        }
        
        // 바닥 및 착지 상태
        GroundState();
        
        // 노드범위체크 리듬게임 들어가면 작동
        PlayerParing.instance.CheckRange();
        
        // 강제다시시작
        if (Input.GetKeyDown(KeyCode.C))
        {
            UIController.instance.ReStart();
        }
    }

    private void Attack()
    {
        // 미끄러지면서 공격하는 것 방지
        theRB.velocity = new Vector2(0f, theRB.velocity.y);						// 이동멈춤(E누르고 나가는 경우 방지)
        
        // 공격시 앞으로 살짝 나감
        if(transform.localScale.x == 1)
            theRB.AddForce(transform.right * 50f);
        else
            theRB.AddForce(-transform.right * 50f);
        
        //currentAttack++;
        
        // Loop back to one after third attack
        // if (currentAttack > 3)
        //     currentAttack = 1;
        currentAttack = 1;
        
        // Reset Attack combo if time since last attack is too large
        // if (timeSinceAttack > 1.0f)
        //     currentAttack = 1;
        
        // Call one of three attack animations "Attack1", "Attack2", "Attack3"
        animator.SetTrigger("Attack_" + currentAttack);
        //bladeAnimator.SetTrigger("Attack_" + currentAttack);
        
        // Reset timer
        // timeSinceAttack = 0.0f;
    }
    
    private void GroundState()
    {
        Bounds bounds = capsuleCollider2D.bounds;
        footPosition  = new Vector2(bounds.center.x, bounds.min.y);
        isGrounded    = Physics2D.OverlapCircle(footPosition, 0.05f, groundLayer);     // ground + Object 레이어에 닿으면 true 아니면 false
        animator.SetFloat("AirSpeedY", theRB.velocity.y);                                      // 낙하모션 변경 트리거
        
        if (isGrounded == true && theRB.velocity.y <= 0.0f)                                         // 점프시 isGruonded 바로 false되 않기 때문에, y 조건 추가
        {
            animator.SetBool("Landing", true);
            currentJumpCount = maxJumpCount;
        }
    }

    private void Dash(float direction)
	{
        dashInputX           = direction;                          
        transform.localScale = new Vector2(direction, 1f);
        activeMoveSpeed      = dashSpeed;                                     // 속도변경
        dashCoolCounter      = dashCooldown;                                  // 쿨타임 작동
        animator.SetTrigger("Dash");                                     // 속도 변경후 애니메이션 재생 ☆
        PlayerHpController.instance.MakeInvincible(dashTime);                 // 대쉬 애니메이션 재생 길이만큼 무적시간 보내기
    }
    
    private void Jump()
    {
        animator.SetBool("Landing", false);
        if (currentJumpCount is 2 or 1)
        {
            animator.SetTrigger("JumpUp");
            theRB.velocity = Vector2.up * jumpForce;
            currentJumpCount--;
        }
    }

    public void Hit()
    {
        // Enemy - Object - BOSS 체크
        Collider2D[] hit = Physics2D.OverlapBoxAll(damagePoint.transform.position, new Vector2(1.2f, 0.8f), 0, enemyLayer);
        for (var i = 0; i < hit.Length; ++i)
        {
            // 적
            if (hit[i].GetComponent<EnemyController>() == true)
            {
                if(hit[i].GetComponent<EnemyController>().weaknessState == true)
                    hit[i].GetComponent<EnemyController>().DamageEnemy(damageToGive * 10);
                else
                    hit[i].GetComponent<EnemyController>().DamageEnemy(damageToGive);
            }
            
            // 오브젝트
            if(hit[i].GetComponent<Breakables>() == true)
                hit[i].GetComponent<Breakables>().Smash();
            
            // 보스 body 평타판정 스크립트가 들어있으면
            if(hit[i].GetComponent<BossBodytakeDamage>() == true)
                hit[i].GetComponent<BossBodytakeDamage>().TakeDamage(damageToGive);
            
            // 보스 body 평타판정 스크립트가 들어있으면
            if (hit[i].GetComponent<EnemyBullet>() == true)
            {
                Instantiate(PlayerParing.instance.boomPrefabs, hit[i].transform.position, Quaternion.identity);
                Destroy(hit[i].gameObject);
                UIController.instance.gageSlider.value += 0.1f;
            }
        }
    }

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(damagePoint.transform.position, new Vector2(1.2f, 0.8f));
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(footPosition, 0.05f);
    }

    private void StateCheckTimer()
	{
        // 공격
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_1")) ||
            (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_2")) ||
            (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_3")))          
        {
            attackState = false;
            inputX = 0.0f;
        }
        else
        {
            attackState = true;              
        }
        
        // 구르기
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Dash"))          
        {
            rollState = false;
            theRB.gravityScale = originGavityScale * 0.5f;
            theRB.velocity = new Vector2(dashInputX * activeMoveSpeed, theRB.velocity.y); // 이동
        }
        else                                     
        {
            rollState = true;
            theRB.gravityScale = originGavityScale;
            activeMoveSpeed = moveSpeed;
        }
    
        if (dashCoolCounter > 0)                  
        {
            dashCoolCounter -= Time.deltaTime;    // 쿨타임 감소
        }

        // 스킬
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Skill_Q"))
        {
            skillState = false;             
        }
        else
        {
            skillState = true;              
        }
        
        // 타격
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_TakeHit"))
        {
            //takeHitState = false;             
        }
        else
		{
            takeHitState = true;              
		}
        
        
    }

}