using System;
using System.Collections;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

public class UIAutoSystem : MonoBehaviour
{
    public static UIAutoSystem instance;
    
    public bool autoEventState;                    // false일 때 제어권 없음
    
    public  AutoAtion[] autoAtion;                 // public class BossAction의 행동 배열(각각 따로따로 설정 가능)
    private bool        currntAtionEndState;       // 현재 액션이 진행되고 있으면, 다시 false가 될 때 까지 다음게 실행안됨 + 딜레이가 끝나고
    [HideInInspector]
    public  int         currtAtionNum = 0;

    private bool talkAtionEndState;                // false 없음
    private bool objectAtionEndState;              
    private bool moveAtionEndState;

    private bool firstFadeEndState;


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
        // 이벤트상태 멈춤
         if(autoEventState == false)
             PlayerController.instance.theRB.velocity = new Vector2(0f, PlayerController.instance.theRB.velocity.y);						// 이동멈춤(E누르고 나가는 경우 방지)

        // 제어권이 없고!!, 앤셕이 0(1개라도 있는 경우)이 아니고, 스토리턱 상태가 끝났을 때
        if (autoAtion.Length != 0 && UIStoryTalk.instance.storyTalkEndState && autoEventState == false)
        {
            // currntAtionEndState가 false일 때, 다음액션 수행
            // currtAtionNum이 autoAtion.Length와 같아지면 마지막액션 수행됨.
            if (currntAtionEndState == false && currtAtionNum != autoAtion.Length)
            {
                currntAtionEndState = true;
                // 대사
                if (autoAtion[currtAtionNum].talkE.shouldTalk)
                {
                    talkAtionEndState = true;
                    StartCoroutine(AutoTalk());
                }

                // 오브젝트 활성화
                if (autoAtion[currtAtionNum].objectE.shouldObject)
                {
                    objectAtionEndState = true;
                    StartCoroutine(AutoObject());
                }

                // 움직임
                if (autoAtion[currtAtionNum].moveE.shouldMove)
                {
                    moveAtionEndState = true;
                    StartCoroutine(AutoMove());
                }

                // 페이드
                if (autoAtion[currtAtionNum].fadeE.shouldFade)
                {
                    StartCoroutine(AutoFade());
                }
                
                // 키잠금(true시 키 제한 / 시간지연이 안 들어가기 때문에, 다른 Ation과 같이 사용)
                if (autoAtion[currtAtionNum].keyE.rightKey)
                    PlayerController.instance.rightKeyLock = true;
                else
                    PlayerController.instance.rightKeyLock = false;

                if (autoAtion[currtAtionNum].keyE.leftKey)
                    PlayerController.instance.leftKeyLock = true;
                else
                    PlayerController.instance.leftKeyLock = false;

                if (autoAtion[currtAtionNum].keyE.jumpKey)
                    PlayerController.instance.jumpKeyLock = true;
                else
                    PlayerController.instance.jumpKeyLock = false;
                
            }
        }

    }
    
    
    private IEnumerator AutoTalk()
    {
        yield return new WaitForSeconds(autoAtion[currtAtionNum].talkE.startWaitTime);      // 시작지연
        autoAtion[currtAtionNum].talkE.talkCanvas.gameObject.SetActive(true);               // 캔버스 활성화

        
        foreach (char c in autoAtion[currtAtionNum].talkE.script)                           // 대사출력
        {
            autoAtion[currtAtionNum].talkE.talkText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        
        yield return new WaitForSeconds(autoAtion[currtAtionNum].talkE.endWaitTime);      // 대사가 다 끝나고 켄버스 사라짐 지연
        autoAtion[currtAtionNum].talkE.talkCanvas.gameObject.SetActive(false);            // 캔버스 비활성화
        autoAtion[currtAtionNum].talkE.talkText.text = "";                                // 텍스트 초기화
        talkAtionEndState = false;                                                        // 상태변경(1)
        AtionEnd();
    }

    private IEnumerator AutoObject()
    {
        yield return new WaitForSeconds(autoAtion[currtAtionNum].objectE.startWaitTime);      // 시작지연
        
        if(autoAtion[currtAtionNum].objectE.activeObject)                                     // 오브젝트 활성화
            autoAtion[currtAtionNum].objectE.activeObject.SetActive(true);                       
        
        if(autoAtion[currtAtionNum].objectE.deactiveObject)                                   // 오브젝트 비활성화
            autoAtion[currtAtionNum].objectE.deactiveObject.SetActive(false);
        
        yield return new WaitForSeconds(autoAtion[currtAtionNum].objectE.endWaitTime);        // 끝지연   
        objectAtionEndState = false;                                                          // 상태변경(2)
        AtionEnd();

    }

    private IEnumerator AutoMove()
    {
        yield return new WaitForSeconds(autoAtion[currtAtionNum].moveE.startWaitTime);      // 시작지연

        // 좌우반전 및 이동(플레이어와 무브포인트 계산)
        if (PlayerController.instance.gameObject.transform.position.x - autoAtion[currtAtionNum].moveE.movePoint.transform.position.x >= 0)
        {
            PlayerController.instance.gameObject.transform.localScale = new Vector2(-1f, 1f);
            while ((Vector2.Distance(PlayerController.instance.theRB.transform.position,autoAtion[currtAtionNum].moveE.movePoint.transform.position ) >= 0.5f))
            {
                PlayerController.instance.theRB.velocity = new Vector2(-PlayerController.instance.moveSpeed, PlayerController.instance.theRB.velocity.y);
                yield return null;
            }
            
        }
        else
        {
            PlayerController.instance.gameObject.transform.localScale = new Vector2(1f, 1f);
            while ((Vector2.Distance(PlayerController.instance.theRB.transform.position,autoAtion[currtAtionNum].moveE.movePoint.transform.position ) >= 0.5f))
            {
                PlayerController.instance.theRB.velocity = new Vector2(PlayerController.instance.moveSpeed, PlayerController.instance.theRB.velocity.y);
                yield return null;
            }
            
        }
        
        // 도착 후 좌우반전(오른쪽 왼쪽)
        if (autoAtion[currtAtionNum].moveE.lookRight)
            PlayerController.instance.gameObject.transform.localScale = new Vector2(1f, 1f);
        else if(autoAtion[currtAtionNum].moveE.lookLeft)
            PlayerController.instance.gameObject.transform.localScale = new Vector2(-1f, 1f);
        
        yield return new WaitForSeconds(autoAtion[currtAtionNum].moveE.endWaitTime);                    // 끝지연
        moveAtionEndState = false;                                                                      // 상태변경(3)
        AtionEnd();
    }
    
    private IEnumerator AutoFade()
    {
        yield return new WaitForSeconds(autoAtion[currtAtionNum].fadeE.startWaitTime);      // 시작지연
        // 밝게
        if (autoAtion[currtAtionNum].fadeE.fadeOutControl)
        {
            UIController.instance.fadeOutBlack = true;
            while (UIController.instance.fadeOutBlack)
            {
                if (UIController.instance.fadeOutBlack == false)
                {
                    break;
                }
                yield return null; // 코루틴 while안에 꼭 필요 ☆
            }
        }
        
        // 어둡게
        if (autoAtion[currtAtionNum].fadeE.fadeToControl)
        {
            UIController.instance.fadeToBlack = true;
            while (UIController.instance.fadeToBlack)
            {
                if (UIController.instance.fadeToBlack == false)
                {
                    break;
                }
                yield return null; // 코루틴 while안에 꼭 필요 ☆
            }
        }
        yield return new WaitForSeconds(autoAtion[currtAtionNum].fadeE.endWaitTime);      // 끝지연
        AtionEnd();
    }

    // 진행되고 있는 모든 액션이 끝나고, 상태 및 번호를 올림
    // 진행되고 있는, 액션이 모두 false가 되고, AtionEnd()실행
    private void AtionEnd()
    {
        // 3가지 Auto가 false일 때, 다음 AtionAuto로 넘거감.
        if (talkAtionEndState == false && objectAtionEndState == false && moveAtionEndState == false)
        {
             // 오토끝(전체가 다 끝났거나, 중단점이거나)
             if(currtAtionNum == autoAtion.Length || autoAtion[currtAtionNum].commonE.breakPoint)
                 autoEventState = true;
             
             // 앤션번호 및 상태관리
             currtAtionNum++;
             currntAtionEndState = false;
        }
    }
}


[Serializable]
public class AutoAtion
{
    public CommonElement commonE;
    public TalkElement   talkE;
    public ObjectElement objectE;
    public MoveElement   moveE;
    public KeyElement    keyE;
    public FadeElement   fadeE;
}

[Serializable]
public class CommonElement
{
    public bool              breakPoint;            // true -> 제어권 플레이어에게
}

[Serializable]
public class TalkElement
{
    public bool              shouldTalk;
    public float             startWaitTime;
    public Canvas            talkCanvas;
    public TextMeshProUGUI   talkText;
    public String            script;
    public float             endWaitTime;           // 대사가 다 끝나고, 켄버스 사라지는 시간
}

[Serializable]
public class ObjectElement
{
    public bool          shouldObject;
    public float         startWaitTime;
    public GameObject    activeObject;
    public GameObject    deactiveObject;
    public float         endWaitTime;           // 대사가 다 끝나고, 켄버스 사라지는 시간
}

[Serializable]
public class MoveElement
{
    public bool          shouldMove;
    public float         startWaitTime;
    public GameObject    movePoint;
    public bool          lookRight;
    public bool          lookLeft;
    public float         endWaitTime;           
}

[Serializable]
public class FadeElement
{
    public bool  shouldFade;
    public float startWaitTime;
    public bool  fadeOutControl;    // 밝아짐  
    public bool  fadeToControl;     // 어두워짐
    public float endWaitTime;           
}

[Serializable]
public class KeyElement
{
    public bool rightKey;
    public bool leftKey;
    public bool jumpKey;
}




