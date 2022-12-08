using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

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
    public float currentGageValue;                      // 현재 게이지 값 저장
    private float gagePerSpeedbuff;
    
    [SerializeField]
    private LayerMask         groundLayer;              
    private CapsuleCollider2D capsuleCollider2D;        
    [HideInInspector]
    public  bool              isGrounded;
    private Vector2           footPosition;
    private bool              jumpState;
    private int               maxJumpCount     = 2;     
    private int               currentJumpCount = 0;     
    public  float             jumpForce;

    [HideInInspector]
    public  bool       attackState     = false;        
    private float      timeSinceAttack = 0.0f;          // 콤보 시간 체크  
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


    public bool isSlope;                        // 평지판단
    public float distance;                      // 표시해줄 선 거리
    public float angle;
    public Vector2 perepndi;
    public float maxangle;

    //[HideInInspector]
    public GameObject currentPlatform;
    public LayerMask  platformLayer;

    public GameObject afterimage;                   // 잔상파티클

    private void Awake()
    {
        instance = this;

        theRB             = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        animator          = GetComponent<Animator>();
        bodySR            = GetComponent<SpriteRenderer>();

        activeMoveSpeed   = moveSpeed;

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
        // 애니메이션 상태체크 및 오르막길 판단
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
            if (isSlope && isGrounded && jumpState && angle < maxangle)                               // 이동(3가지 경우)
                theRB.velocity = new Vector2(inputX * (activeMoveSpeed + gagePerSpeedbuff) * perepndi.x * -1,0f);      // 오르막길
            else if (!isSlope && isGrounded && jumpState)
                theRB.velocity = new Vector2(inputX * (activeMoveSpeed + gagePerSpeedbuff),0f);                        // 평지
            else if (!isGrounded)
                theRB.velocity = new Vector2(inputX * (activeMoveSpeed + gagePerSpeedbuff),theRB.velocity.y);            // 점프중

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
            
            // 내려가기
            if (Input.GetKeyDown("down"))
            {
                PlatGroundDown();
            }

            // 스킬
            // if (Input.GetKey(KeyCode.Q))
			// {
			//      PlayerSkill.instance.SetSkill("Q");
            // }
            
            // 기본상태 전환 // 모션 울찔울찔 방지 키 조건
            if (theRB.velocity != Vector2.zero &&(Input.GetKey("right") || Input.GetKey("left")))
            {
                animator.SetBool("Walking", true);
            }
            else
            {
                animator.SetBool("Walking", false);
            }
            
            // 패링모드
            if (Input.GetKeyDown(KeyCode.Z) && UIController.instance.gageSlider.value >= 0.5f)
            {
                currentGageValue  = UIController.instance.gageSlider.value;
                currentGageValue -= 0.5f;                                     // 현재 게이지값 저장
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

        // 대쉬 조건(공격중에도 사용 가능)
        if (Input.GetKey(KeyCode.S) && rollState && takeHitState && skillState && PlayerParing.instance.paringState &&
            UIStoryTalk.instance.storyTalkEndState && UIEvent.instance.eventState && !UIInventory.instance.puaseState && 
            UIController.instance.gageSlider.value >= 0.2f)
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
        theRB.velocity = new Vector2(0f, theRB.velocity.y);    // 미끄러지면서 공격하는 것 방지		
        
        // 공격시 앞으로 살짝 나감
        if(transform.localScale.x == 1)
            theRB.AddForce(transform.right * 100f);
        else
            theRB.AddForce(-transform.right * 100f);
        
        currentAttack++;                                         // 콤보 애니메이션 변경
        
        // 콤보숫자 넘어감 or 콤보누적시간 넘어감
         if (currentAttack > 2 || timeSinceAttack >1.0f)
             currentAttack = 1;
        
        animator.SetTrigger("Attack_" + currentAttack);     // 애니메이션 재생
        //bladeAnimator.SetTrigger("Attack_" + currentAttack);

         timeSinceAttack = 0.0f;                                 // 콤보시간 리셋(StateCheckTimer()에서 체크함)
    }
    
    private void GroundState()
    {
        Bounds bounds = capsuleCollider2D.bounds;
        footPosition  = new Vector2(bounds.center.x, bounds.min.y);
        isGrounded    = Physics2D.OverlapCircle(footPosition, 0.05f, groundLayer);     // ground + Object 레이어에 닿으면 true 아니면 false
        animator.SetFloat("AirSpeedY", theRB.velocity.y);                                      // 낙하모션 변경 트리거
        
        if (isGrounded && theRB.velocity.y <= 0.0f)                                                 // 점프시 isGruonded 바로 false되 않기 때문에, y 조건 추가
        {
            animator.SetBool("Landing", true);
            currentJumpCount = maxJumpCount;
        }
        else if (isGrounded == false && theRB.velocity.y <= 0.0f)
        {
            animator.SetBool("Landing", false);
        }
    }

    private void PlatGroundDown()
    {
        if (isGrounded)
        {
            Debug.Log("들어");
            // 플렛폼 그라운드 판단
            Collider2D[] plat = Physics2D.OverlapCircleAll(footPosition, 0.05f, platformLayer);
            for (var i = 0; i < plat.Length; ++i)
            {
                if (plat[i].GetComponent<PlatformEffector2D>() == true)
                {
                    currentPlatform = plat[i].gameObject;
                }
            }

            if (currentPlatform)
            {
                currentPlatform.GetComponent<PlatformScript>().playerCheck = true;              // 안전시간
                currentPlatform.GetComponent<PlatformEffector2D>().gameObject.layer = 16;       // 계속 충돌되어, 천천히 내려오는걸 방지하기 위한, 레이어 변경
                currentPlatform.GetComponent<PlatformEffector2D>().rotationalOffset = 180f;     // 각도 변경
                Debug.Log("실행");
                //Physics2D.IgnoreLayerCollision(7,16,true);
                // capsuleCollider2D.isTrigger = true;
            }
        }
        
    }

    private void Dash(float direction)
    {
        
        currentGageValue = UIController.instance.gageSlider.value;
        currentGageValue -= 0.2f;                                     // 현재 게이지값 저장

        animator.updateMode = AnimatorUpdateMode.UnscaledTime;                                  // 애니메이션 영향 안가게 모드변경
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;                                           // 부드러운 fixedUpdate를 위해 설정

        dashInputX           = direction;                          
        transform.localScale = new Vector2(direction, 1f);
        activeMoveSpeed      = dashSpeed;                                     // 속도변경
        dashCoolCounter      = dashCooldown;                                  // 쿨타임 작동
        animator.SetTrigger("Dash");                                     // 속도 변경후 애니메이션 재생 ☆
        PlayerHpController.instance.MakeInvincible(dashTime);                 // 대쉬 애니메이션 재생 길이만큼 무적시간 보내기
        
        afterimage.SetActive(true);                                            // 잔상효과 켜기
        if (transform.localScale.x == 1)
        {
            afterimage.transform.localScale = new Vector3(0.8f,0.8f,1f);
        }
        else
        {
            afterimage.transform.localScale = new Vector3(-0.8f,0.8f,1f);
        }
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
            if (hit[i].GetComponent<EnemyController>())
            {
                if(hit[i].GetComponent<EnemyController>().weaknessState)    // 취약상태 피격(보통 = 0, 기절 = 1)
                    hit[i].GetComponent<EnemyController>().DamageEnemy(damageToGive,2);
                else                                                        // 기본상태 피격
                    hit[i].GetComponent<EnemyController>().DamageEnemy(damageToGive,1);
                UIController.instance.gageSlider.value += 0.03f;
            }
            
            // 오브젝트
            if(hit[i].GetComponent<Breakables>())
                hit[i].GetComponent<Breakables>().Smash();
            
            // 보스 body 평타판정 스크립트가 들어있으면
            if (hit[i].GetComponent<BossBodytakeDamage>())
            {
                hit[i].GetComponent<BossBodytakeDamage>().TakeDamage(damageToGive);
                UIController.instance.gageSlider.value += 0.03f;
            }
            
            // 발사체 제거
            // if (hit[i].GetComponent<EnemyBullet>() == true)
            // {
            //     Instantiate(PlayerParing.instance.boomPrefabs, hit[i].transform.position, Quaternion.identity);
            //     Destroy(hit[i].gameObject);
            //     UIController.instance.gageSlider.value += 0.1f;
            // }
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
            if(isSlope)
                theRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            else
                theRB.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            attackState = false;
            inputX = 0.0f;
        }
        else
        {
            attackState = true;              
        }
        timeSinceAttack += Time.unscaledDeltaTime;      // 콤보 체크
        
        // 구르기
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Dash"))
        {
            rollState = false;
            // 게이지 감소 체크
            if (UIController.instance.gageSlider.value >= currentGageValue / UIController.instance.gageSlider.maxValue + 0.01)
            {
                UIController.instance.gageSlider.value = Mathf.Lerp(UIController.instance.gageSlider.value,currentGageValue/UIController.instance.gageSlider.maxValue,Time.unscaledDeltaTime * 5f);
            }
            
            if (dashInputX == 1)
            {   // 물리를 고려한 위치이동 = movePosition으로 하지 않으면, 날아가버림
                theRB.MovePosition(theRB.position + new Vector2(1f,0f) * dashSpeed * Time.fixedDeltaTime);
            }
            else if (dashInputX == -1)
            {
                theRB.MovePosition(theRB.position + new Vector2(-1f,0f)* dashSpeed * Time.fixedDeltaTime);
            }
            
        }
        else                             
        { 
            // 1회만 실행되어야 하는 것 ! ☆
            if (rollState == false)
            {
                animator.updateMode = AnimatorUpdateMode.Normal;
                Time.timeScale = 1f;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                afterimage.SetActive(false);                                   // 잔상효과 끄기
            }
            rollState = true;
            activeMoveSpeed    = moveSpeed;
        }

        if (dashCoolCounter > 0)                  
        {
            dashCoolCounter -= Time.deltaTime;    // 쿨타임 감소(연속사용 안되게 1초)
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

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Jump_Up") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Jump_Fall"))
        {
            jumpState = false;      // 점프 중
        }
        else
        {
            jumpState = true;       // 점프 중 X
        }
        
        // 미끄러짐 방지 및 오르막길 판단
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Idle"))
        {
            theRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            theRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distance, groundLayer);      // 플레이어 몸 기준으로 아래로 선을 그려서, distance만큼 표시해 주고, groundLayer랑 닿아서, 상호작용 하고

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
        Debug.DrawLine(hit.point,hit.point + hit.normal,Color.cyan);
        Debug.DrawLine(hit.point,hit.point + perepndi,Color.cyan);
        
        // 스피드 버프 적용
        // 자동회복
        gagePerSpeedbuff = UIController.instance.gageSlider.value * 2f;
        UIController.instance.gageSlider.value += Time.unscaledDeltaTime * 0.01f;

    }

}