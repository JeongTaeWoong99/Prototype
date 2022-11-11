using System;
using System.Collections;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

public class UIAutoSystem : MonoBehaviour
{
    public static UIAutoSystem instance;
    
    public bool autoEventState;                    // false일 때 제어권 없음
    
    public  float       startDelay;                // 1. 화면이 바뀌고(FADE로 인한 기다림) 이벤트를 시작할 때 딜레이
                                                   // 2. 그리고 breakingPoint 다음의 이벤트의 경우 딜레이
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

                // 페이드
                if (autoAtion[currtAtionNum].commonE.fadeOutControl)
                {
                    StartCoroutine(FadeOutControl());
                }
                else if (autoAtion[currtAtionNum].commonE.fadeToControl)
                {
                    StartCoroutine(FadeToControl());
                }
            }
        }

    }
    
    
    private IEnumerator AutoTalk()
    {
        // 페이드 된 후 딜레이 + breakPoint다음 딜레이
        if (currtAtionNum == 0 || autoAtion[currtAtionNum-1].commonE.breakPoint)
        {
            yield return new WaitForSeconds(startDelay);
            autoAtion[currtAtionNum].talkE.talkCanvas.gameObject.SetActive(true);
        }
        else
            autoAtion[currtAtionNum].talkE.talkCanvas.gameObject.SetActive(true);

        // 대사출력
        foreach (char c in autoAtion[currtAtionNum].talkE.script)
        {
            autoAtion[currtAtionNum].talkE.talkText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        
        // 지연 및 켄버스 끄기 및 텍스트 초기화 
        yield return new WaitForSeconds(autoAtion[currtAtionNum].commonE.nextAutoDelay);
        autoAtion[currtAtionNum].talkE.talkCanvas.gameObject.SetActive(false);
        autoAtion[currtAtionNum].talkE.talkText.text = "";

        // 상태변경 및 실행
        talkAtionEndState = false;
        AtionEnd();
    }

    private IEnumerator AutoObject()
    {
        // 페이드 된 후 딜레이 + breakPoint다음 딜레이
        if (currtAtionNum == 0 ||  autoAtion[currtAtionNum-1].commonE.breakPoint)
        {
            yield return new WaitForSeconds(startDelay);
            autoAtion[currtAtionNum].objectE.activeObject.SetActive(true);
        }
        else
            autoAtion[currtAtionNum].objectE.activeObject.SetActive(true);
        
        // 지연
        yield return new WaitForSeconds(autoAtion[currtAtionNum].commonE.nextAutoDelay);
        
        // 상태변경 및 실행
        objectAtionEndState = false;
        AtionEnd();

    }

    private IEnumerator AutoMove()
    {
        // 페이드 된 후 딜레이 + breakPoint다음 딜레이
        if (currtAtionNum == 0 || autoAtion[currtAtionNum-1].commonE.breakPoint)
            yield return new WaitForSeconds(startDelay);

        // 좌우반전 및 이동(플레이어와 무브포인트 계산)
        if (PlayerController.instance.gameObject.transform.position.x - autoAtion[currtAtionNum].moveE.movePoint.transform.position.x >= 0)
        {
            PlayerController.instance.gameObject.transform.localScale = new Vector2(-1f, 1f);
            while ((Vector2.Distance(PlayerController.instance.theRB.transform.position,autoAtion[currtAtionNum].moveE.movePoint.transform.position ) >= 0.1f))
            {
                PlayerController.instance.theRB.velocity = new Vector2(-PlayerController.instance.moveSpeed, PlayerController.instance.theRB.velocity.y);
                yield return null;
            }
            
        }
        else
        {
            PlayerController.instance.gameObject.transform.localScale = new Vector2(1f, 1f);
            while ((Vector2.Distance(PlayerController.instance.theRB.transform.position,autoAtion[currtAtionNum].moveE.movePoint.transform.position ) >= 0.1f))
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
        
        // 지연
        yield return new WaitForSeconds(autoAtion[currtAtionNum].commonE.nextAutoDelay);
        
        // 상태변경 및 실행
        moveAtionEndState = false;
        AtionEnd();
    }

    private IEnumerator FadeOutControl()
    {
        UIController.instance.fadeOutBlack = true;
        yield return new WaitForSeconds(autoAtion[currtAtionNum].commonE.fadeWaitTime);
        AtionEnd();
    }
    
    private IEnumerator FadeToControl()
    {
        UIController.instance.fadeToBlack = true;
        yield return new WaitForSeconds(autoAtion[currtAtionNum].commonE.fadeWaitTime);
        AtionEnd();
    }

    // 진행되고 있는 모든 액션이 끝나고, 상태 및 번호를 올림
    // 진행되고 있는, 액션이 모두 false가 되고, AtionEnd()실행
    private void AtionEnd()
    {
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
}

[Serializable]
public class CommonElement
{
    public float             nextAutoDelay;
    public bool              breakPoint;
    public bool              fadeOutControl;    // 밝아짐
    public bool              fadeToControl;     // 어두워짐
    public float             fadeWaitTime;
}

[Serializable]
public class TalkElement
{
    public bool              shouldTalk;
    public Canvas            talkCanvas;
    public TextMeshProUGUI   talkText;
    public String            script;
}

[Serializable]
public class ObjectElement
{
    public bool          shouldObject;
    public GameObject    activeObject;
}

[Serializable]
public class MoveElement
{
    public bool          shouldMove;
    public GameObject    movePoint;
    public bool          lookRight;
    public bool          lookLeft;
}

[Serializable]
public class KeyElement
{
    public bool rightKey;
    public bool leftKey;
    public bool jumpKey;
}


