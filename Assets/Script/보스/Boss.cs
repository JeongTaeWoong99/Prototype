using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BossState {Pahse00 = 0,Phase01,Phase02,Phase03}

public class Boss : MonoBehaviour
{
    public static Boss instance;
    
    [SerializeField]
    private float      bossAppearPoint;                               // 처음위치에서 설정한 y값 까지 이동
    private BossState  bossState       = BossState.Pahse00;           // 최초 1회에만 사용할 변수 값, 변수는 계속 사용한다.
    private Movement2D movement2D;
    private BossWeapon bossWeapon;

    // [SerializeField]
    // private GameObject  boomPrefab; 
    public  GameObject  sparkPrefab;
    public  GameObject  boomPrefab;

    // 보스 사망 파티클 재생 후 씬 변환
    // [SerializeField]
    // private PlayerController playerController;
    // [SerializeField]
    // private string nextSceneName;              // 다음 씬 이름(다음 스테이지 or 게임 클리어)

    private Animator anim;

    private int              phaseNum = 0;                                              // pahse별 생성 파괴 함수에 사용
    public  List<GameObject> phase1BossBody = new List<GameObject>();    
    public  List<GameObject> phase2BossBody = new List<GameObject>();    
    //public  List<GameObject> phase3BossBody = new List<GameObject>();
    
    private DissolveController dissolveController;
    
    public  List<GameObject> brokenPiece = new List<GameObject>();        // 파편 배열
    


    private void Awake()
    {
        instance = this;
        
        movement2D         = GetComponent<Movement2D>();
        bossWeapon         = GetComponent<BossWeapon>();
        anim               = GetComponent<Animator>();
        dissolveController = GetComponent<DissolveController>();
    }

    private void Start()
    {
        // 보스의 첫 번째 상태인 지정된 위치로 이동 실행
        ChangeState(BossState.Pahse00);
    }

    public void ChangeState(BossState newState)
    {
        // 그전 상태 종료
        StopCoroutine(bossState.ToString());
        // 상태 변경                            
        bossState = newState;                               // 새로운 열거형의 숫자값 들어감
        // 새로운 상태 재생
        StartCoroutine(bossState.ToString());   // 열거형의 현재 숫자 값 실행
    }

    // 열거형 0번 
    private IEnumerator Pahse00()
    {
        // 아래에서 위로 이동
        movement2D.MoveTo(Vector3.up);
        
        while (true)
        {    // 보스 위치  -> 아래에서 위로
            if (transform.position.y >= bossAppearPoint)
            {
                dissolveController.isDissolving = false;    // false시 나타남.

                // 다 나타나고 Body 활성화
                // Wait 시간으로 인해 1초동안 올라오면서 나타나게 됨.
                yield return new WaitForSeconds(1f);
                phaseNum++;     // 0->1
                anim.SetTrigger("SetPhase1");

                // Phase0 멈춤
                // 이동방향을 (0,0,0)으로 설정해 멈추도록 한다.
                movement2D.MoveTo(Vector3.zero);
                // Phase01 애니메이션 변경
                ChangeState(BossState.Phase01);
                break;
            }
            yield return null;
        }
    }

    private IEnumerator Phase01()
    {
        // Phase2 공격패턴 시작
        bossWeapon.StartFiring(AttackType.Phase1);

        while (true)
        {
            // 보스의 현재 체력이 50% 이하가 되면
            if (BossHP.instance.currentHP <= BossHP.instance.maxHP * 0.5f)
            {
                phaseNum++;     // 1->2
                anim.SetTrigger("SetPhase2");
                
                // 스파크 생성
                for (int i = 0; i < phase1BossBody.Count; i++)
                {
                    Instantiate(sparkPrefab, phase1BossBody[i].transform.position, Quaternion.identity);
                }

                // Phase01 공격패턴 멈춤
                bossWeapon.StopFiring(AttackType.Phase1);
                // Phase02 애니메이션 시작
                ChangeState(BossState.Phase02);
                break;
            }

            yield return null;
        }
    }

    // Phase03
    private IEnumerator Phase02()
    {
        // Phase2 공격패턴 시작
        bossWeapon.StartFiring(AttackType.Phase2);

        while (true)
        {
            yield return null;
        }
    }

    public void OnDie()
    {
        // 보스 파괴 파티클 생성
        //GameObject clone = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
      
        // 파티클 재생 완료 후 씬 전환을 위한 설정
        //clone.GetComponent<BossExplosion>().Setup(playerController,nextSceneName);
       
        //보스 오브젝트 파괴
        //Destroy(gameObject);
        
        // GameObject[] restBullet = GameObject.FindGameObjectsWithTag("Bullet");
        // for (int i = 0; i < restBullet.Length;i++)
        // {
        //     Instantiate(boomPrefab, restBullet[i].transform.position, quaternion.identity);
        //     Destroy(restBullet[i]);
        // }
        
        bossWeapon.StopFiring(AttackType.Phase2);

        movement2D.MoveTo(Vector3.down);
        UIController.instance.fadeToBlack = true;

    }

    private void PhaseBossBodyActive()
    {
        if (phaseNum == 1)
        {
            for (int i = 0; i < phase1BossBody.Count; i++)
            {
                phase1BossBody[i].SetActive(true);
            }
        }
        else if(phaseNum == 2)
        {
            for (int i = 0; i < phase2BossBody.Count; i++)
            {
                phase2BossBody[i].SetActive(true);
            }
        }
        // else if (phaseNum == 3)
        // {
        //     for (int i = 0; i < phase3BossBody.Count; i++)
        //     {
        //         phase3BossBody[i].SetActive(true);
        //     }
        // }
    }

    private void PhaseBossBodyBoom()
    {
        // 애니메이션상 1단계 전 phase의 body를 부셔야 하기 때문에
        // pahseNum - 1 의 body를 부신다.
        
        if(phaseNum == 2)
        {
            for (int i = 0; i < phase1BossBody.Count; i++)
            {
                // 조각생성 반복
                for (int k = 0; k < brokenPiece.Count; k++)
                {
                    int angle       = Random.Range(0, 180);
                    Instantiate(brokenPiece[k], phase1BossBody[i].transform.position, Quaternion.Euler(0, 0,  angle));
                }
                Instantiate(boomPrefab, phase1BossBody[i].transform.position, quaternion.identity);
                Destroy(phase1BossBody[i]);
            }
        }
        else if (phaseNum == 3)
        {
            for (int i = 0; i < phase2BossBody.Count; i++)
            {
                Destroy(phase2BossBody[i]);
            }
        }
        // else if (phaseNum == 4)
        // {
        //     for (int i = 0; i < phase3BossBody.Count; i++)
        //     {
        //         Destroy(phase3BossBody[i]);
        //     }
        // }
    }
}
