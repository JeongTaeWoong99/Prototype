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
    private GameObject laserAlterLine;               // 얼터 라인
    [SerializeField]
    private GameObject missAlterLine;               // 얼터 라인

    // 공격 속도 제한 Phase03 용
    private float constrainSpeed = 1.0f;
    
    public List<GameObject>     attackPoint     = new List<GameObject>();    
    public List<BossMissCreate> bossMissCreates = new List<BossMissCreate>();

    private Vector3        dir;
    private float	       angle;

    private int[]   randomNumArray;            // 렌덤넘값
    private float[] randomNumArrayAgle;        // 렌덤플로트값
    
    private void Awake()
    {
        instance = this;
        
        randomNumArray     = new int[3];           // 3개
        randomNumArrayAgle = new float[3];         // 3개
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
        yield return new WaitForSeconds(3.0f);
        
        float attackRate    = 2f * constrainSpeed;  // 공격 주기
        int   count         = 12;                   // 발사체 생성 개수
        float intervalAngle = 360 / count;          // 발사체 사이의 각도

        // 원 형태로 방사하는 발사체 생성 (count 개수만큼)
        // 공격포인트 0번
        while (true)
        {
            Instantiate(lightPrefabs, attackPoint[0].transform.position, quaternion.identity);          // 라이트 생성
            yield return new WaitForSeconds(0.5F);    

            // 얼터라인
            // for (int i = 0; i < count; ++i)
            // {
            //     float angle = intervalAngle * i;
            //     Instantiate(missAlterLine, attackPoint[0].transform.position, Quaternion.Euler(0f,0f,angle));
            // }
            
            //yield return new WaitForSeconds(5F);
            
            for (int i = 0; i < count; ++i)
            {
                // 발사체 이동 방향(각도)
                // // weightAngle이 0일때
                // 0 + 12 * 0 = 0  도
                // 0 + 12 * 1 = 12 도
                // 0 + 12 * 2 = 24 도
                // .......... = 360도 -> 30개(=count)의 오브젝트가 각 방향으로 발사 됨.
                // weightAngle이 0 1 2 3 씩 증가하면서 무적존을 없애 줌.
                float angle = intervalAngle * i;
                // 발사체 생성
                Instantiate(missilePre, attackPoint[0].transform.position, Quaternion.Euler(0f,0f,angle));
            }

            // attackRate 시간만큼 대기
        yield return new WaitForSeconds(attackRate);
        }
    }

    private IEnumerator Phase2()
    {
        // 팔 펴지는 시간
        yield return new WaitForSeconds(3.0f);
        UIController.instance.bossSlider.gameObject.SetActive(true);            // 보스 HP슬라이더 보이게
        
        float   attackRate       = 2f * constrainSpeed;
        while (true)
        {
            while (true)
            {
                for (int j = 0; j < randomNumArray.Length; j++)
                {
                    randomNumArray[j] = Random.Range(1, 11);                    // 1~ 10 
                }
                
                for (int j = 0; j < randomNumArray.Length; j++)
                {
                    Debug.Log(j + "의 값 = " +randomNumArray[j]);
                }

                if (randomNumArray[0] != randomNumArray[1] && randomNumArray[1] != randomNumArray[2] && randomNumArray[0] != randomNumArray[2]) 
                    break;
            }
            
            // 라이트생성
            for (int i = 0; i < randomNumArray.Length; i++)
            {
                Instantiate(lightPrefabs, attackPoint[randomNumArray[i]].transform.position, quaternion.identity);          // 라이트 생성
            }
            
            // 앵글값 저장
            for (int i = 0; i < randomNumArray.Length; i++)
            {
                dir   = (PlayerController.instance.transform.position - attackPoint[randomNumArray[i]].transform.position).normalized; 
                angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                randomNumArrayAgle[i] = angle;
            }

            // 얼터라인 
            for (int i = 0; i < randomNumArray.Length; i++)
            {
                Instantiate(laserAlterLine, attackPoint[randomNumArray[i]].transform.position,Quaternion.Euler(0,0,randomNumArrayAgle[i]));          // 얼터라인
            }
            
            yield return new WaitForSeconds(0.15F);
            
            // 미사일발사
            for (int i = 0; i < randomNumArray.Length; i++)
            {
                Instantiate(laserPre, attackPoint[randomNumArray[i]].transform.position,Quaternion.Euler(0,0,randomNumArrayAgle[i]));               // 미사일 생성
            }

            // 발사체 생성(공격생성포인트 1~10 손가락)
            // for (int i = 1; i < 11; i++)
            // {
            //     Instantiate(lightPrefabs, attackPoint[i].transform.position, quaternion.identity);          // 라이트 생성
            //     dir   = (PlayerController.instance.transform.position - attackPoint[i].transform.position).normalized; 
            //     angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            //     Instantiate(laserAlterLine, attackPoint[i].transform.position,Quaternion.Euler(0,0,angle));          // 얼터라인
            //     
            //     yield return new WaitForSeconds(0.3F);
            //     
            //     Instantiate(laserPre, attackPoint[i].transform.position,Quaternion.Euler(0,0,angle));           // 미사일 생성
            //     
            //     yield return new WaitForSeconds(0.1F);
            // }

            // attackRate 시간만큼 대기
            yield return new WaitForSeconds(attackRate);
        }
    }
    
    private IEnumerator Phase3()
    {
        // 애니메이션 재생시간
        yield return new WaitForSeconds(6.0f);
        
        //float attackRate  = 1f;             // 공격 주기
        int   cycleAttack = 5;
        int   count       = 20;               // 발사체 생성 개수

        while (true)
        {
                // 3-1
                // 하늘로 쏳아 올리기
                for (int j = 0; j < cycleAttack; j++)
                {
                    for (int i = 0; i < count; i++)
                    {
                        angle = Random.Range(30f, 150f);
                        Instantiate(laserDummyPre, attackPoint[11].transform.position, Quaternion.Euler(0f,0f,angle));
                        var randTime = Random.Range(0.02f, 0.05f);
                        yield return new WaitForSeconds(randTime);
                    }
                }
                yield return new WaitForSeconds(5);
                // 하늘에서 공격 뿌리기
                for (int i = 0; i < 2; i++)
                {
                    bossMissCreates[i].phaseSpwanState = true;
                }
                // 패턴 딜레이
                yield return new WaitForSeconds(5);
                for (int i = 0; i < 2; i++)
                {
                    bossMissCreates[i].phaseSpwanState = false;
                }
                yield return new WaitForSeconds(5);
            
                
                // 3 - 2
                // 하늘 뿌리기
                for (int j = 0; j < cycleAttack-2; j++)     // 3번
                {
                    for (int i = 0; i < count/2; i++)       // 10개
                    {
                        angle = UnityEngine.Random.Range(30f, 150f);
                        Instantiate(missileDummyPre, attackPoint[11].transform.position, Quaternion.Euler(0f,0f,angle));
                    }
                    yield return new WaitForSeconds(1f);
                }
                yield return new WaitForSeconds(3);
                // 하늘에서 공격 뿌리기
                for (int i = 2; i < 12; i++)
                {
                    bossMissCreates[i].GetComponent<BossMissCreate>().StartCoroutine("MissCoroutine");
                }
                // 패턴 딜레이
                yield return new WaitForSeconds(5);
                for (int i = 2; i < 12; i++)
                {
                    bossMissCreates[i].GetComponent<BossMissCreate>().StopCoroutine("MissCoroutine");
                }
                yield return new WaitForSeconds(5);
            
        }
    }

}