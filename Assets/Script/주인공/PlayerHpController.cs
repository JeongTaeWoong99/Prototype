using UnityEngine;

public class PlayerHpController : MonoBehaviour
{
    public static PlayerHpController instance;           

    public  int    currentHealth;
    public  int    maxHealth;
    public  int    penalty;
    public  float  damageInvincibleLength;                 // 피격 후 무적시간(0:15 = 0.25초, 보다 길게 0.5)
    [HideInInspector]
    public float   invincibleCount;                        // 무적 시간 카운트

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentHealth  = maxHealth;                                    // 게임 시작시 currentHealth값을 maxHealth 값으로 설정
        currentHealth -= penalty;
        UIController.instance.healthSlider.maxValue = maxHealth;      // 게임 시작시 healthSlider.maxValue값을 maxHealth 값으로 설정
        UIController.instance.healthSlider.value    = currentHealth;  // 게임 시작시 healthSlider.value값을 currentHealth 값으로 설정
        UIController.instance.healthText.text       = currentHealth.ToString() + " / " + maxHealth.ToString();
        // 게임 시작시 텍스트의 글자를 currentHealth로 바꾸고,
        // int에서 String으로 가는 것 이기 때문에 ToString() 처리를 해준다.
        // maxHealth.ToString() 도 마찬가지
    }

    private void Update()
    {
        // 무적 남은시간 감소
        // 무적시간 즉, invincibleCount가 남아 있으면, 무적시간의 남은시간을 감소시킴
        if (invincibleCount >= 0)
        {
            invincibleCount -= Time.deltaTime;

            // 투명도 정상화
            // if (invincibleCount <= 0)
            // {
            //     var color = PlayerController.instance.bodySR.color;
            //     color = new Color(color.r, color.g, color.b, 1f);
            //     PlayerController.instance.bodySR.color = color;
            // }
        }

        // 피격상태 체크해서, 카메라 흔들기
        CameraShakeCheck();

    }

    public void DamagePlayer(GameObject hitEnemyPosition,int damageInt)
    {
        // 무적시간이 0.0초 이하이면 HP감소 
        if (invincibleCount <= 0)
        {
            // 모션 중 피격시 에니메이션 재생은 X
            if(PlayerController.instance.skillState == true || PlayerParing.instance.paringState == true)     // 기본공격은 회피하도록 스킬은 무시하고 애니메이션 재생하도록
               PlayerController.instance.animator.SetTrigger("TakeHit");                                 // 히트시 애니메이션 재생 여부(안 하게되면 스킬상태 초기화 문제가 생김)

            invincibleCount = damageInvincibleLength;   // invincibleCount(남은시간)을 damageInvincibleLength(초기무적시간)으로 초기화
            currentHealth  -= damageInt;                // 체력 감소
            
            //너백(대미지를 입힌 대상의 위치정보를 가져옴.)
            int random = Random.Range(10, 20);
            if(hitEnemyPosition.transform.position.x - gameObject.transform.position.x >= 0) 
                PlayerController.instance.theRB.AddForce(-transform.right * random);
            else
                PlayerController.instance.theRB.AddForce(transform.right * random);

            // 투명도 50% 설정
            // var color = PlayerController.instance.bodySR.color;
            // color = new Color(color.r, color.g, color.b, 0.5f);
            // PlayerController.instance.bodySR.color = color;
            
            // 플레이어 사망했을 때
            if (currentHealth <= 0)
            {
                UIController.instance.deathState = true;                                                     // 사망상태 true
                UIController.instance.deathScreen.SetActive(true);                                           // 사망상태 true

                // 사망 프리팹 생성
                // 1회 생성 후 null 넣어서 중복생성 방지
                if (PlayerController.instance.deathPrefabs)
                {
                    Instantiate(PlayerController.instance.deathPrefabs, transform.position, Quaternion.identity);   
                    PlayerController.instance.deathPrefabs = null;
                    PlayerController.instance.gameObject.SetActive(false);                                                                 // 플레이어 비활성화
                }
            }

            // UI값 갱신
            UIController.instance.healthSlider.value = currentHealth;                                           // 데미지를 입은 후 변환
            UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();    // 데미지를 입은 후 변환
        }
    }

    // 대쉬 무적
    public void MakeInvincible(float length)
    {
        invincibleCount = length;
        // 바로 투명도 50%되지 않고, 애니메이션에서 점진적으로  1 -> 0.5 -> 1
        //var color = PlayerController.instance.bodySR.color;
        //color = new Color(color.r, color.g,color.b, 0.5f);
       // PlayerController.instance.bodySR.color = color;
    }
    
    
    public void CameraShakeCheck()
    {
        // if (PlayerController.instance.takeHitState == false)    // false일 때 애니메이션 재생 중
        //     CameraShake.instance.shakeCoroutineState = true;
        if (PlayerController.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Player_TakeHit"))    // false일 때 애니메이션 재생 중
            CameraShake.instance.shakeCoroutineState = true;
        else
            CameraShake.instance.shakeCoroutineState = false;
    }

}
