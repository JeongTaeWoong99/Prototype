using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIAutoSystem : MonoBehaviour
{
    public static UIAutoSystem instance;
    
    public bool autoEventState;              // false일 때 제어권 없음

    public AutoAtion[] autoAtion;            // public class BossAction의 행동 배열(각각 따로따로 설정 가능)

    private bool  currntAtionEndState;       // 현재 액션이 진행되고 있으면, 다시 false가 될 때 까지 다음게 실행안됨 + 딜레이가 끝나고
    public  int   currtAtionNum = 0;
    public  float startDelay;                // 화면이 바뀌고(FADE로 인한 기다림) 이벤트를 시작할 때 딜레이

    private void Awake()
    {
        instance = this;
    }
    
    private void Start()
    {
        // auto가 없을경우 바로 true
        if (autoAtion.Length == 0)
            autoEventState = true;
    }

    private void Update()
    {
         if(autoEventState == false)
             PlayerController.instance.theRB.velocity = new Vector2(0f, PlayerController.instance.theRB.velocity.y);						// 이동멈춤(E누르고 나가는 경우 방지)

        // 제어권이 없고!!, 오토길이가 0이 아니고, 스토리턱 상태가 끝났을 때
        if (autoAtion.Length != 0 && UIStoryTalk.instance.storyTalkEndState && autoEventState == false)
        {
            if (currntAtionEndState == false && currtAtionNum != autoAtion.Length)
            {
                if (autoAtion[currtAtionNum].shouldTalk)
                {
                    currntAtionEndState = true;
                    StartCoroutine(AutoTalk());
                }

                if (autoAtion[currtAtionNum].shouldObject)
                {
                    currntAtionEndState = true;
                    StartCoroutine(AutoObject());
                }
                
                if (autoAtion[currtAtionNum].shouldMove)
                {
                    currntAtionEndState = true;
                    StartCoroutine(AutoMove());
                }
                
            }
            
        }
        // 이벤트 끝나면 제어권 다시 플레이어한테 주기


    }
    
    
    private IEnumerator AutoTalk()
    {
        // 맨처음만 대사딜레이 및 켄버스 활성화
        if (currtAtionNum == 0)
        {
            yield return new WaitForSeconds(startDelay);
            autoAtion[currtAtionNum].talkCanvas.gameObject.SetActive(true);
        }
        else
            autoAtion[currtAtionNum].talkCanvas.gameObject.SetActive(true);

        // 대사출력
        foreach (char c in autoAtion[currtAtionNum].script)
        {
            autoAtion[currtAtionNum].talkText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        
        // 지연 및 켄버스 끄기 및 텍스트 초기화 
        yield return new WaitForSeconds(autoAtion[currtAtionNum].nextAutoDelay);
        autoAtion[currtAtionNum].talkCanvas.gameObject.SetActive(false);
        autoAtion[currtAtionNum].talkText.text = "";
        
        
        // 오토끝(전체가 다 끝났거나, 중단점이거나) !!!
        if(currtAtionNum == autoAtion.Length || autoAtion[currtAtionNum].breakpoint)
            autoEventState = true;
        
        // 앤션번호 및 상태관리
        currtAtionNum++;
        currntAtionEndState = false;
    }

    private IEnumerator AutoObject()
    {
        // 맨처음만 대사딜레이 및 오브젝트 활성화
        if (currtAtionNum == 0)
        {
            yield return new WaitForSeconds(startDelay);
            autoAtion[currtAtionNum].activeObject.SetActive(true);
        }
        else
            autoAtion[currtAtionNum].activeObject.SetActive(true);
        
        // 지연
        yield return new WaitForSeconds(autoAtion[currtAtionNum].nextAutoDelay);
        
        // 오토끝(전체가 다 끝났거나, 중단점이거나)
        if(currtAtionNum == autoAtion.Length || autoAtion[currtAtionNum].breakpoint)
            autoEventState = true;
        
        // 앤션번호 및 상태관리
        currtAtionNum++;
        currntAtionEndState = false;
        
    }

    private IEnumerator AutoMove()
    {
        // 맨처음만 움직임 딜레이
        if (currtAtionNum == 0)
            yield return new WaitForSeconds(startDelay);

        // 좌우반전 및 이동(플레이어와 무브포인트 계산)
        if (PlayerController.instance.gameObject.transform.position.x - autoAtion[currtAtionNum].movePoint.transform.position.x >= 0)
        {
            PlayerController.instance.gameObject.transform.localScale = new Vector2(-1f, 1f);
            while ((Vector2.Distance(PlayerController.instance.theRB.transform.position,autoAtion[currtAtionNum].movePoint.transform.position ) >= 0.1f))
            {
                PlayerController.instance.theRB.velocity = new Vector2(-PlayerController.instance.moveSpeed, PlayerController.instance.theRB.velocity.y);
                yield return null;
            }
            
        }
        else
        {
            PlayerController.instance.gameObject.transform.localScale = new Vector2(1f, 1f);
            while ((Vector2.Distance(PlayerController.instance.theRB.transform.position,autoAtion[currtAtionNum].movePoint.transform.position ) >= 0.1f))
            {
                PlayerController.instance.theRB.velocity = new Vector2(PlayerController.instance.moveSpeed, PlayerController.instance.theRB.velocity.y);
                yield return null;
            }
            
        }
        
        // 도착 후 좌우반전(오른쪽 왼쪽)
        if (autoAtion[currtAtionNum].lookRight)
            PlayerController.instance.gameObject.transform.localScale = new Vector2(1f, 1f);
        else if(autoAtion[currtAtionNum].lookLeft)
            PlayerController.instance.gameObject.transform.localScale = new Vector2(-1f, 1f);
        
        // 지연
        yield return new WaitForSeconds(autoAtion[currtAtionNum].nextAutoDelay);
        
        // 오토끝(전체가 다 끝났거나, 중단점이거나)
        if(currtAtionNum == autoAtion.Length || autoAtion[currtAtionNum].breakpoint)
            autoEventState = true;
        
        // 앤션번호 및 상태관리
        currtAtionNum++;
        currntAtionEndState = false;
        
    }
}


[System.Serializable]
public class AutoAtion
{
    [Header("Common")] 
    public float             nextAutoDelay;
    public bool              breakpoint;
    
    [Header("Talk")] 
    public bool              shouldTalk;
    public Canvas            talkCanvas;
    public TextMeshProUGUI   talkText;
    public String            script;

    [Header("ObjectActive")] 
    public bool          shouldObject;
    public GameObject    activeObject;
    
    [Header("Move")] 
    public bool          shouldMove;
    public GameObject    movePoint;
    public bool          lookRight;
    public bool          lookLeft;



    // [Header("Move")] 
    // public bool       shouldMove;
    // public GameObject moveBody;
    // public Text   talkText;
    // public float  nextAutoDelay;
    //
    //
    //
    //
    //
    //
    // public float actionLength;
    //
    // public bool  shouldMove;
    // public bool  shouldChasePlayer;
    // public bool  shouldmoveToPoint;
    // public float moveSpeed;
    // public Transform pointToMoveTo;
    //
    // public bool shouldShoot;
    // public GameObject itemToShoot;
    // public float timeBetweenShots;
    // public Transform[] shotPoints; // 4방향 슛

}
