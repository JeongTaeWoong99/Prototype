using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerParing : MonoBehaviour
{
    public static PlayerParing instance;
    
    //[HideInInspector] 
    public bool prmMode;                           // PRM 모드
    //[HideInInspector]
    public bool traceMode;                         // Trace 모드
    
	[HideInInspector]
	public  bool      paringState = true;          // false 사용 중
	private float     percendFadeTime = 0.1f;      // 퍼센트 감속 값 조정
	public GameObject cameraMovePoint;             // 패링사용 카메라 무브포인트
	public GameObject scanPrefabs;                 // 스켄 이미지 
	public GameObject targetMarkPrefabs;           // 표적표시 프리팹
	public GameObject createPoint;                 // 점선/타임서클 생성 위치   
	public GameObject LinePrefabs;                 // 라인표시 프리팹

    public GameObject greenCirclePrefabs;          // 초록
	public GameObject yellowCirclePrefabs;         // 노랑
	public GameObject redCirclePrefabs;            // 빨강
    
	private List<GameObject> finalList      = new List<GameObject>();    // 블랫추적 후 정렬된 저장리스트
    private List<GameObject> enemyFinalList = new List<GameObject>();    // 역추적할 적 리스트
	private int  currentCheckFinalListNum   = -1;                        // 현재 체크 파이널리스트 숫자
	private bool keyPressState;                                     // 키상태
	private bool rhythmGameState;                                   // 타임서클 상태
	private List<GameObject> destroyList = new List<GameObject>();  // 격추 성공한 오브젝트
	public  GameObject timeCircPrefabs;                             // 타임서클 프리팹
	public  GameObject boomPrefabs;                                 // 격추 성공 폭파 프리팹
	private GameObject currentTimeCircleClone;                      // 현재 작동되고 있는 타임서클 정보                            
	private bool XkeeyState = false;                                    

	public float      zoomAndTimeSlowSpeed;        // 줌과 타임스케일 0되는 속도
	public float      scanSclaeSpeed;              // 쉴드 생성 속도

    public GameObject  pivot;
    private GameObject temp;

    private LayerMask basicLayer = ~(1 << 3);       // 실루엣 레이어 제외

    class SortData
    {
        public float transformData;
    }
    List<SortData> sortDataList = new List<SortData>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CameraController.instance.mainCam.cullingMask = basicLayer;                 // 게임 시작시 카메라 설정은 basic으로
    }

    public void Paring()
    {
        if (paringState)
        {
            CameraController.instance.focusPoint = cameraMovePoint;                                         // 포커스 바꾸기
            CameraController.instance.transform.position
                = new Vector3(cameraMovePoint.transform.position.x,cameraMovePoint.transform.position.y,CameraController.instance.transform.position.z);    // 위치이동
            CameraController.instance.focusIn  = true;													    // focusIn true
            Time.timeScale = 0.1f;                                                                          // 즉시 감속 속도
            Time.fixedDeltaTime = Time.timeScale * 0.02f;                                                   // 부드러운 fixedUpdate를 위해 설정
            paringState = false;

            StartCoroutine(SlowMosion());
        }
    }
    
    // 줌과 타임스케일 제로
    private IEnumerator SlowMosion()
    {
        float currentTime = 0.0f;
        float percent     = 0.0f;

        while (percent < 1)
        {
            currentTime +=  Time.unscaledDeltaTime * zoomAndTimeSlowSpeed;      // 줌과 타임스케일 0까지(Time.deltaTime이므로, 많이 증가하다가, 타임스케일 조정에 따라 같이 증가값이 적어짐)
            percent      =  currentTime / percendFadeTime;                      // 0에서 1까지 올라감.

            //카메라 스케일 조정(originOrthographicSize -> originFocusOrthographicSize)
            CameraController.instance.mainCam.orthographicSize 
                = Mathf.Lerp(CameraController.instance.originOrthographicSize,CameraController.instance.originFocusOrthographicSize, percent);

            // 타임스케일 조정(0.1 -> 0.0)
            // 픽스 델타타임 조정
            Time.timeScale      = Mathf.Lerp(0.1f  , 0.0f, percent);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;                                              
            
            // 다음 코루틴(타임스케일이 0.01f 보다 작아지면)
            if (Time.timeScale < 0.01f) 
            {
                Time.timeScale = 0.0f;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                StartCoroutine(Scan());
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator Scan()
    {
        float currentTime = 0.0f;
        float percent     = 0.0f;
        
        // 스캔 모양
        GameObject scanIns = Instantiate(scanPrefabs, transform.position, quaternion.identity);

        while (percent < 1) // 0.1초만에 percnet가 0에서 1이 됨.
        {
            currentTime += Time.unscaledDeltaTime * scanSclaeSpeed * 0.01f;  // 실제 시간 초 중첩
            percent      = currentTime / percendFadeTime;                      // 0에서 1까지 올라감.

            float scanScale;
            scanScale = Mathf.Lerp(0.0f, 3f, percent);           // 0 -> 2
            scanIns.transform.localScale = new Vector2(scanScale, scanScale);
            
            if (scanScale > 2.49f)
            {
                // 추적모드 // PRM모드
                if(traceMode)
                    StartCoroutine(Trace());
                else if(prmMode)
                    StartCoroutine(PRM());
                yield break;
            }
            
            yield return null;
        }
    }

    // 잠재 위험 측정(Potential risk measurement)
    private IEnumerator PRM()
    {
        yield return new WaitForSecondsRealtime(2f);
        CameraController.instance.mainCam.cullingMask = LayerMask.GetMask("Silhouette");    // 실루엣 화면으로
        
        yield return new WaitForSecondsRealtime(2f);
        GameObject singleUse = GameObject.FindWithTag("SingleUse");                                       // 스캔 오브젝트 삭제
        Destroy(singleUse);

        CameraController.instance.mainCam.cullingMask = basicLayer;                                    // 원래 화면으로
        ParingEnd();                                                                                      // 패링끝
    }
    
    // 추적모드 
    private IEnumerator Trace()
    {
        var originLineCreateMovePoint = createPoint.transform.position; // 스킬사용 했을 때, 초기 위치 저장
        
        GameObject[] Bullets        = GameObject.FindGameObjectsWithTag("Bullet");    // 보이는 Bullet
        GameObject[] visibleEnemy   = GameObject.FindGameObjectsWithTag("Enemy");     // 보이는 적
        GameObject[] existenceBoss  = GameObject.FindGameObjectsWithTag("BossBody");  // 보스가 존재하는지

        // 처리할 Bullets 정리(보이는지)
        for (int i = 0; i < Bullets.Length; i++)
        {
            if(Bullets[i].GetComponent<SpriteRenderer>().isVisible)
                finalList.Add(Bullets[i]);
        }
        
        // // 역추적할 적 정리(보이는지)
        for (int i = 0; i < visibleEnemy.Length; i++)
        {
            if (visibleEnemy[i].GetComponentInChildren<EnemyController>().theBody.isVisible)
                enemyFinalList.Add(visibleEnemy[i]);
        }
        
        // 역추적 할 보스(존재하는지)
        for (int i = 0; i < existenceBoss.Length; i++)
        {
            enemyFinalList.Add(existenceBoss[i]);
        }
        
        // 추적할 값이 있으면
        if (finalList.Count != 0)
        {
            for (int i = 0; i < finalList.Count; i++)
            {
                SortData sortData = new SortData();
                sortData.transformData = finalList[i].transform.position.x;
                sortDataList.Add(sortData);
            }   
            sortDataList.Sort(Sort);
            float averageX = (sortDataList[0].transformData + sortDataList[finalList.Count - 1].transformData) * 0.5f;
            
            sortDataList.Clear();
            
            for (int i = 0; i < finalList.Count; i++)
            {
                SortData sortData = new SortData();
                sortData.transformData = finalList[i].transform.position.y;
                sortDataList.Add(sortData);
            }
            sortDataList.Sort(Sort);
            
            
            float averageY = (sortDataList[0].transformData + sortDataList[finalList.Count - 1].transformData) * 0.5f;
            sortDataList.Clear();
            pivot.transform.position = new Vector2(averageX, averageY);
    
            for (int i = 0; i < finalList.Count; i++)
            {
                finalList[i].GetComponent<EnemyBullet>().anglePivotValue = Quaternion.FromToRotation(Vector3.up, finalList[i].transform.position - pivot.transform.position).eulerAngles.z;
            }
    
            // 자리교체
            for(int i = 0; i < finalList.Count - 1 ; i++)
            {
                for(int j = 0; j < finalList.Count - 1 - i; j++)
                {
                    if(finalList[j].GetComponent<EnemyBullet>().anglePivotValue > finalList[j + 1].GetComponent<EnemyBullet>().anglePivotValue)
                    {
                        temp = finalList[j].gameObject;
                        finalList[j] = finalList[j+1];
                        finalList[j+1] = temp;
                    }
                }
                temp = null;
            }
            
            
            // 표적생성
            // 최종 선별 저장된 미사일 리스트 저장
            for (int i = 0; i < finalList.Count;i++)
            {
                // AudioManager.instance.PlaySFX(0);                                                                     // 표적확인 사운드
                Instantiate(targetMarkPrefabs, finalList[i].transform.position, Quaternion.identity);                     // 표적표시
             
                yield return new WaitForSecondsRealtime(0.1f);
            }
            
            // 경로(점선)생성
            // 0번부터 시작(플레이어에서 시작하지 않도록)
            createPoint.transform.position = finalList[0].transform.position;
            for (int i = 0; i < finalList.Count - 1;i++)
            {
                while ((Vector2.Distance(createPoint.transform.position, finalList[i+1].transform.position) >= 0.1f))  // 다가가서 겹치는 두 오브젝트의 거리
                {
                    Instantiate(LinePrefabs, createPoint.transform.position, quaternion.identity);
                    createPoint.transform.position 
                        = Vector2.MoveTowards(createPoint.transform.position, finalList[i+1].transform.position, 0.05f);
                    
                    yield return new WaitForSecondsRealtime(0.015f);
                }
                
                // 범위 도착하면, 정중앙으로 이동
                createPoint.transform.position = finalList[i+1].transform.position;
            }
            
            //초기 위치 복귀
            //createPoint.transform.position = originLineCreateMovePoint;
            
            // 카메라 따라가기
            CameraController.instance.focusPoint = createPoint;									                                                    // 포커스 변경
            CameraController.instance.transform.position
                = new Vector3(finalList[0].transform.position.x,finalList[0].transform.position.y,CameraController.instance.transform.position.z);  // 위치이동(☆시작 타이밍 연출 줘도 될듯)
            CameraController.instance.focusIn    = true;									                                                        // focusIn true

            // 선따라 createPoint 이동 및 타임서클 생성
            // 범위 체크 및 발동 -> CheckRange() 함수
            for (int i = 0; i < finalList.Count;i++)
            {
                rhythmGameState = false;     // 타임서클 리듬게임 실행 상태(이동 중 체크 불가능)
                // 현재 제거하는 표적으로 이동 ☆☆
                while (Vector2.Distance(createPoint.transform.position, finalList[i].transform.position) >= 0.1f && finalList[i] != false)  // 다가가서 겹치는 두 오브젝트의 거리
                {
                    createPoint.transform.position = Vector2.MoveTowards(createPoint.transform.position, finalList[i].transform.position, 0.05f);
                    yield return new WaitForSecondsRealtime(0.01f);
                }
                
                // 타임서클 생성(위치 도착)
                currentTimeCircleClone = Instantiate(timeCircPrefabs, finalList[i].transform.position, Quaternion.identity); 
                
                // 타임서클 줄어들기
                while (finalList[i].transform.localScale.x - currentTimeCircleClone.transform.localScale.x <= 0.1f)// 0.2 - 0.5에서 0.2 - 0.
                {
                    rhythmGameState = true;       // 타임서클 리듬게임 실행 상태(도착 후 체크 가능)
                    currentCheckFinalListNum = i; // 현재 체크하고 있는 번호
                    
                    // 안에 들어가 있거나,
                    // X를 눌러서 XState가 true가 되면 줄어들기 멈춤
                    if (destroyList.Find(((x) => x.gameObject == finalList[currentCheckFinalListNum].gameObject)) || XkeeyState == true)
                    {
                        XkeeyState = false;
                        break;
                    }

                    // 줄어들기 ★ -> 표적크기까지 바꿔주기 ★
                    currentTimeCircleClone.transform.localScale 
                        = Vector2.MoveTowards(currentTimeCircleClone.transform.localScale, new Vector3(0.2f,0.2f), 0.024f); //※☆★ 빌드하면 나누기 4됨

                    // 누르는 타이밍 끝남(X 처리)
                    if (!(finalList[i].transform.localScale.x - currentTimeCircleClone.transform.localScale.x <= 0.1f))
                    {
                        Instantiate(redCirclePrefabs, finalList[currentCheckFinalListNum].transform.position, quaternion.identity);
                        rhythmGameState = false;       // 누르는 타이밍 끝남
                    }
                     
                    yield return new WaitForSecondsRealtime(0.01f);
                }

                // 마지막 노드에서 카메라 화면전환 바로 안 되도록
                if (i == finalList.Count-1)
                {
                    yield return new WaitForSecondsRealtime(0.3f);
                }
            }
            currentCheckFinalListNum = -1;         // 체크함수 작동 하지 않도록
            rhythmGameState          = false;      // 리듬겜 상태 false
            currentTimeCircleClone   = null;       // 현재클론 비워주기

            // 현재 게임 내에서 SingleUse 다 삭제
            GameObject[] singleUse = GameObject.FindGameObjectsWithTag("SingleUse");
            for (int i = 0; i < singleUse.Length;i++)
            {
                Destroy(singleUse[i]);
            }
            
            // 카메라 원래대로 복귀
            CameraController.instance.focusPoint = null;
            CameraController.instance.focusIn  = false;
            CameraController.instance.mainCam.orthographicSize = CameraController.instance.originOrthographicSize;
            CameraController.instance.transform.position
                = new Vector3(CameraController.instance.target.transform.position.x,CameraController.instance.target.transform.position.y,CameraController.instance.transform.position.z);
            createPoint.transform.position = originLineCreateMovePoint;  // 초기 위치 복귀
            yield return new WaitForSecondsRealtime(1.0f);           // 멈췄다가 터지는 연출
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f; 
            paringState = true;                                          // 스킬사용 상태

            // 상태변경 해줘야 할 불렛이 있고,
            for (int i = 0; i < destroyList.Count; i++)
            {
                // 레이저 레이저의 경우(1가지)
                //  -> 터지기
                if (destroyList[i].GetComponent<EnemyBullet>().beamBool == true)
                {
                    Instantiate(boomPrefabs, destroyList[i].transform.position, quaternion.identity);
                    Destroy(destroyList[i]);
                }
                
                // 미사일 해킹의 경우(2가지)
                // -> 역추적 할 적이 있다면 -> 역추적
                int minIndex = 0;
                if (destroyList[i].GetComponent<EnemyBullet>().missileBool == true && 
                    enemyFinalList.Count != 0)
                {
                    // 거리가 가장 짧은 적 구하기
                    // (2개 이상이면 for 작동)
                    // (1개면 minIndex = 0)
                    for (int j = 1; j < enemyFinalList.Count; j++)
                    {
                        if(Vector2.Distance(destroyList[i].transform.position, enemyFinalList[j-1].transform.GetChild(0).position)>
                           Vector2.Distance(destroyList[i].transform.position, enemyFinalList[j].transform.GetChild(0).position))
                        {
                            minIndex = j;
                        }
                    }
                    
                    if(enemyFinalList[minIndex].CompareTag("Enemy"))
                        destroyList[i].GetComponent<EnemyBullet>().currentTarget =  enemyFinalList[minIndex].transform.GetChild(0);
                    else if(enemyFinalList[minIndex].CompareTag("BossBody"))
                        destroyList[i].GetComponent<EnemyBullet>().currentTarget =  enemyFinalList[minIndex].transform;
                }
                // 역추적 할 적이 없다면 -> 터지기
                else
                {
                    Instantiate(boomPrefabs, destroyList[i].transform.position, quaternion.identity);
                    Destroy(destroyList[i]);
                }
                
            }
            enemyFinalList.Clear();
            
            destroyList.Clear();    // 남아있는 것 클리어
            
            ParingEnd();            
        }
        // final리스트에 들어온 값이 없을 때
        else
        {
            yield return new WaitForSecondsRealtime(2.0f);
            // 스캔 오브젝트 제거
            GameObject singleUse = GameObject.FindWithTag("SingleUse");
            Destroy(singleUse);
            ParingEnd();
        }
    }

    // 범위 체크 및 발동
    public void CheckRange()
    {
        // -1 때 누르지 못하도록
        if (currentCheckFinalListNum != -1)
        {
            if (Input.GetKeyDown(KeyCode.X) && keyPressState == false && rhythmGameState == true)
            {
                //bool keyState = true;
                // 초록
                if (finalList[currentCheckFinalListNum].transform.localScale.x - currentTimeCircleClone.transform.localScale.x >= 0.0f)
                {
                    // 이름이 이미 들어가 있으면 중복하여 넣지 않음. -> 1회만 
                    // 성공 표시 프리팹 생성
                    if (!destroyList.Find(((x) => x.gameObject == finalList[currentCheckFinalListNum].gameObject)))
                    {
                        destroyList.Add(finalList[currentCheckFinalListNum]);
                        Instantiate(greenCirclePrefabs, finalList[currentCheckFinalListNum].transform.position, quaternion.identity);
                    }
                }
                // 노랑
                else if (finalList[currentCheckFinalListNum].transform.localScale.x - currentTimeCircleClone.transform.localScale.x >= -0.1f)
                {
                    if (!destroyList.Find(((x) => x.gameObject == finalList[currentCheckFinalListNum].gameObject)))
                    {
                        destroyList.Add(finalList[currentCheckFinalListNum]);
                        Instantiate(yellowCirclePrefabs,finalList[currentCheckFinalListNum].transform.position, quaternion.identity);
                    }
                }
                // 빨강
                else if (finalList[currentCheckFinalListNum].transform.localScale.x - currentTimeCircleClone.transform.localScale.x >= -0.2f)
                {
                    XkeeyState = true;
                    // 엑스 이미지 생성
                    Instantiate(redCirclePrefabs, finalList[currentCheckFinalListNum].transform.position, quaternion.identity);
                }
            }
        }
        
        // -1이되고 때도, false 상태가 되게 하도록
        // if (Input.GetKeyUp(KeyCode.X) && rhythmGameState == true)
        // {
        //     // 중복 생성 방지, 1회만 되도록
        //     // bool keyState = false;
        // }
    }

    private void ParingEnd()
    {
        finalList.Clear();                              // 리스트 클리어
        //카메라 이동
        CameraController.instance.mainCam.orthographicSize = CameraController.instance.originOrthographicSize;
        CameraController.instance.transform.position 
            = new Vector3(CameraController.instance.target.transform.position.x,CameraController.instance.target.transform.position.y,CameraController.instance.transform.position.z); //☆
        CameraController.instance.focusPoint = null;
        CameraController.instance.focusIn  = false;
        
        // 시간 정상화
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        paringState = true;

        StopAllCoroutines();
    }
    
    int Sort(SortData A, SortData B)
    {
        if (A.transformData < B.transformData) 
            return -1;
        else if(A.transformData > B.transformData) 
            return 1;

        return 0;
    }
}