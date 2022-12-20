using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

// 원형 공격    // 한점 공격
public enum AttackType {Phase1 = 0, Phase2,Phase3}

public class BossWeapon : MonoBehaviour
{
    public static BossWeapon instance;
    
    [SerializeField]
    private GameObject lightPrefabs;        // 공격전 공격포인트에서 반짝
    [SerializeField]
    private GameObject missilePre;          
    [SerializeField]
    private GameObject missileDummyPre; 
    [SerializeField]
    private GameObject laserPre;        
    [SerializeField]
    private GameObject laserDummyPre;
    
    [SerializeField]
    private GameObject laserAlterToPlayer;                 // 얼터 라인
    [SerializeField]
    private GameObject laserAlterToAngle;                  // 얼터 라인

    public List<GameObject>     bodyAttackPoint     = new List<GameObject>();   
    public List<GameObject>     backAttackPoint     = new List<GameObject>();    
    //public List<BossMissCreate> bossMissCreates = new List<BossMissCreate>();

    private Vector3        dir;
    private float	       angle;

    private int[]   randomNumArray;              // 렌덤넘값
    //private float[] randomNumArrayAgle;        // 렌덤플로트값
    
    
    public float attackRate       = 2f;                                          // 다음 공격 

    private void Awake()
    {
        instance = this;
        
        randomNumArray     = new int[4];           // 4개
        //randomNumArrayAgle = new float[4];         // 4개
    }

    public void StartFiring(AttackType attackType)
    {
        // attackType 열거형의 이름과 같은 코루틴을 실행
        StartCoroutine(attackType.ToString());
    }

    public void StopFiring(AttackType attackType)
    {
        // attackType 열거형의 이름과 같은 코루틴을 중지
        StopCoroutine(attackType.ToString());
    }

    private IEnumerator Phase1()
    {
        yield return new WaitForSeconds(3.0f);                                  // 팔 펴지는 시간
        UIController.instance.bossSlider.gameObject.SetActive(true);            // 보스 HP슬라이더 보이게
        
        while (true)
        {
            var randomAttackNum = Random.Range(0, 3);                       // 공격선택

            
            // 공격루틴(배경에서 플레이어에게 한개씩 10개 쏘기)
            if (randomAttackNum == 0)
            {
                for (int i = 0; i < backAttackPoint.Count; i++)
                {
                    dir = ((PlayerController.instance.transform.position) - backAttackPoint[i].transform.position).normalized;
                    angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    Instantiate(laserPre, backAttackPoint[i].transform.position, Quaternion.Euler(0, 0, angle));           // 미사일 생성(angle)
                    Instantiate(laserAlterToPlayer, backAttackPoint[i].transform.position, quaternion.identity);       // 얼터라인(생성될 때의 direction)
                    yield return new WaitForSeconds(0.1F);
                }
            }
            
            // 공격루틴(배경에서 플레이어에게 한번에 10개 쏘기 - 원형)
            if (randomAttackNum ==1)
            {
                for (int i = 0; i < backAttackPoint.Count; i++)
                {
                    dir = ((PlayerController.instance.transform.position) - backAttackPoint[i].transform.position).normalized;
                    angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    Instantiate(laserPre, backAttackPoint[i].transform.position, Quaternion.Euler(0, 0, angle));           // 미사일 생성(angle)
                    Instantiate(laserAlterToPlayer, backAttackPoint[i].transform.position, quaternion.identity);       // 얼터라인(생성될 때의 direction)
                }
            }
            
            // 공격루틴(손가락 플레이어 레이저 4개 * 3번)
            if (randomAttackNum == 2)
            {
                for (int k = 0; k < 3; k++)
                {
                    // 생성위치 중복없이 선택
                    while (true)
                    {
                        for (int j = 0; j < randomNumArray.Length; j++)
                        {
                            if (j % 2 == 0)
                                randomNumArray[j] = Random.Range(0, 5); // 0~ 4      0번과 2번
                            else
                                randomNumArray[j] = Random.Range(5, 10); // 5~ 9     1번과 3번

                        }

                        if (randomNumArray[0] != randomNumArray[2] &&
                            randomNumArray[1] != randomNumArray[3]) // 0번과 2번 - 1번과 3번 값 다른지 확인
                            break;
                    }

                    // 라이트생성
                    for (int i = 0; i < randomNumArray.Length; i++)
                    {
                        Instantiate(lightPrefabs, bodyAttackPoint[randomNumArray[i]].transform.position, quaternion.identity);
                    }

                    yield return new WaitForSeconds(0.2F);

                    // 미사일 + 얼터라인
                    for (int i = 0; i < randomNumArray.Length; i++)
                    {
                        dir = (PlayerController.instance.transform.position - bodyAttackPoint[randomNumArray[i]].transform.position).normalized;
                        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        Instantiate(laserPre, bodyAttackPoint[randomNumArray[i]].transform.position, Quaternion.Euler(0, 0, angle));           // 미사일 생성(angle)
                        Instantiate(laserAlterToPlayer, bodyAttackPoint[randomNumArray[i]].transform.position, quaternion.identity);       // 얼터라인(생성될 때의 direction)
                    }
                }
            }

            // attackRate 시간만큼 대기
            yield return new WaitForSeconds(attackRate);

            int randrandRnum = Random.Range(0,3); // 0~ 2  33% 확률로 
            
            // // 중간중간 미사일 4개 생성(0~10이여서 11개지만,  10군데에서만 발사하기)
            if (randrandRnum == 0)
            {
                // 생성위치 중복없이 선택
                while (true)
                {
                    for (int j = 0; j < randomNumArray.Length; j++)
                    {
                        if (j % 2 == 0)
                            randomNumArray[j] = Random.Range(0, 5); // 0~ 4      0번과 2번
                        else
                            randomNumArray[j] = Random.Range(5, 10); // 5~ 9     1번과 3번
                
                    }
                
                    if (randomNumArray[0] != randomNumArray[2] &&
                        randomNumArray[1] != randomNumArray[3]) // 0번과 2번 - 1번과 3번 값 다른지 확인
                        break;
                }
                
                // 미사일 + 얼터라인
                for (int i = 0; i < randomNumArray.Length; i++)
                {
                    Instantiate(missilePre, backAttackPoint[randomNumArray[i]].transform.position, Quaternion.identity);           // 미사일 생성(angle)
                }
            }

        }
    }
    
    private IEnumerator Phase2()
    {
        // 애니메이션 재생시간
        yield return new WaitForSeconds(6.0f);

        while (true)
        {
                var randomAttackNum = Random.Range(0, 4);                       // 공격선택
                
                // 공격패턴(하늘로 더미레이저를 쏳아 올리고, 무작위 난사)
                if (randomAttackNum == 0)
                {
                    int   count       = 100;               // 발사체 생성 개수
                    for (int i = 0; i < count; i++)
                    {
                        var randRngle = Random.Range(30f, 150f);
                        Instantiate(laserDummyPre, bodyAttackPoint[11].transform.position, Quaternion.Euler(0f, 0f, randRngle));
                        var randTime = Random.Range(0.05f, 0.1f);
                        yield return new WaitForSeconds(randTime);      // 1.5초 ~ 3초동안 하늘로 발사
                    }

                    yield return new WaitForSeconds(5);
                    
                    for (int i = 0; i < count / 2; i++)
                    {
                        var randRngle = Random.Range(285f, 330f);   // 왼쪽
                        Instantiate(laserPre, backAttackPoint[8].transform.position, Quaternion.Euler(0f, 0f, randRngle));
                        randRngle = Random.Range(210f, 255f);           // 오른쪽
                        Instantiate(laserPre, backAttackPoint[2].transform.position, Quaternion.Euler(0f, 0f, randRngle));
                        
                        var randTime = Random.Range(0.1f, 0.2f);
                        yield return new WaitForSeconds(randTime);      
                    }
                }

                // 공격패턴(하늘로 미사일 쏳아 올리고, 10개에서 한번에 날라오기)
                if (randomAttackNum == 1)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        var angleValue = 30 + i * 12;        //30~138
                        Instantiate(missileDummyPre, bodyAttackPoint[11].transform.position, Quaternion.Euler(0f, 0f, angleValue));
                    }
                    
                    yield return new WaitForSeconds(3);
                    
                    for (int i = 0; i < backAttackPoint.Count; i++)
                    {
                        // dir = ((PlayerController.instance.transform.position) - backAttackPoint[i].transform.position).normalized;
                        // angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        Instantiate(missilePre, backAttackPoint[i].transform.position, Quaternion.identity);           // 미사일 생성
                    }
                }
                
                // 공격루틴(배경에서 플레이어에게 한개씩 10개 쏘기)
                if (randomAttackNum == 2)
                {
                    for (int i = 0; i < backAttackPoint.Count; i++)
                    {
                        dir = ((PlayerController.instance.transform.position) - backAttackPoint[i].transform.position).normalized;
                        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        Instantiate(laserPre, backAttackPoint[i].transform.position, Quaternion.Euler(0, 0, angle));           // 미사일 생성(angle)
                        Instantiate(laserAlterToPlayer, backAttackPoint[i].transform.position, quaternion.identity);       // 얼터라인(생성될 때의 direction)
                        yield return new WaitForSeconds(0.05F);
                    }
                }
            
                // 공격루틴(배경에서 플레이어에게 한번에 10개 쏘기 - 원형)
                if (randomAttackNum ==3)
                {
                    for (int i = 0; i < backAttackPoint.Count; i++)
                    {
                        dir = ((PlayerController.instance.transform.position) - backAttackPoint[i].transform.position).normalized;
                        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        Instantiate(laserPre, backAttackPoint[i].transform.position, Quaternion.Euler(0, 0, angle));           // 미사일 생성(angle)
                        Instantiate(laserAlterToPlayer, backAttackPoint[i].transform.position, quaternion.identity);       // 얼터라인(생성될 때의 direction)
                    }
                }
                
                // attackRate 시간만큼 대기
                yield return new WaitForSeconds(attackRate);
                
                int randrandRnum = Random.Range(0,3); // 0~ 2  33% 확률로 
            
                // // 중간중간 미사일 4개 생성(0~10이여서 11개지만,  10군데에서만 발사하기)
                if (randrandRnum == 0)
                {
                    // 생성위치 중복없이 선택
                    while (true)
                    {
                        for (int j = 0; j < randomNumArray.Length; j++)
                        {
                            if (j % 2 == 0)
                                randomNumArray[j] = Random.Range(0, 5); // 0~ 4      0번과 2번
                            else
                                randomNumArray[j] = Random.Range(5, 10); // 5~ 9     1번과 3번
                
                        }
                
                        if (randomNumArray[0] != randomNumArray[2] &&
                            randomNumArray[1] != randomNumArray[3]) // 0번과 2번 - 1번과 3번 값 다른지 확인
                            break;
                    }
                
                    // 미사일 + 얼터라인
                    for (int i = 0; i < randomNumArray.Length; i++)
                    {
                        Instantiate(missilePre, backAttackPoint[randomNumArray[i]].transform.position, Quaternion.identity);           // 미사일 생성(angle)
                    }
                }

        }
    }

}