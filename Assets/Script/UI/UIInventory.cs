using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public static UIInventory instance;
    
    public bool       puaseState;	                    // true 켜짐 false 꺼짐
    public GameObject puasePanel;
    private int	      titleNum      = 0;				// 현재 선택된 넘버(시작은 0번)

    private bool      insideState;                        // 타이틀 안으로 들어와 있는지
    private int       insideLineNum   = 0;
    private int       insideCompoNum  = 0;
    
    public Image focusRed; 
    public titleComponent[] titleComponent;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        instance = this;

        titleComponent[titleNum].titleText.color = new Color(1f,0f,0f,1f);       // 인벤토리가 활성화되면, currentNum 번호에 따라 텍스트빨간불 및 구성요소 보이게
        titleComponent[titleNum].insideFullElement.SetActive(true);                      // 구성요소 활성화
    }
    
    private void Update()
    {
        // I키 활성화 비활성화
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (puaseState)
            {
                Time.timeScale = 1f;                        // 시간 정상화 
                puaseState = false;
                puasePanel.gameObject.SetActive(false);

                insideLineNum  = 0;
                insideCompoNum = 0;
            }
            else if(!puaseState && PlayerParing.instance.paringState && UIEvent.instance.eventState && UIStoryTalk.instance.storyTalkEndState)
            {
                Time.timeScale = 0;                        // 시간 멈춤    
                puaseState = true;
                puasePanel.gameObject.SetActive(true);
            }
        }
        
        // I키 활성화 상태 R L D U키사용
        if (puaseState)
        {
            Mathf.Clamp(titleNum, 0, titleComponent.Length); //invenComponent 숫자 넘지 않기(0~4)
            
            
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (titleNum < 3)
                {
                    // 이전 색변화 및 구성요소 비활성화
                    titleComponent[titleNum].titleText.color = new Color(1f, 1f, 1f, 1f);
                    titleComponent[titleNum].insideFullElement.SetActive(false); 
                    titleNum++;
                    // 바뀐 텍스트색 및 구성요소 활성화
                    titleComponent[titleNum].titleText.color = new Color(1f, 0f, 0f, 1f);
                    titleComponent[titleNum].insideFullElement.SetActive(true); 
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (titleNum > 0)
                {
                    // 이전 색변화 및 구성요소 비활성화
                    titleComponent[titleNum].titleText.color = new Color(1f, 1f, 1f, 1f);
                    titleComponent[titleNum].insideFullElement.SetActive(false); 
                    titleNum--;
                    // 바뀐 텍스트색 및 구성요소 활성화
                    titleComponent[titleNum].titleText.color = new Color(1f, 0f, 0f, 1f);
                    titleComponent[titleNum].insideFullElement.SetActive(true); 
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && titleComponent[titleNum].insideLine != null)
            {
            }
            
            if (Input.GetKeyDown(KeyCode.DownArrow) && titleComponent[titleNum].insideLine != null)
            {
                if (insideState == false)
                {
                    insideState = true;
                    focusRed.gameObject.SetActive(true);                                                                                         // 보이고
                    focusRed.transform.position = titleComponent[titleNum].insideLine[0].element[0].elementFocus.gameObject.transform.position;  // 위치이동
                    titleComponent[titleNum].insideLine[0].element[0].elementComponent.gameObject.SetActive(true);                               // 구성요소 활성화
                }

            }
        }
    }
}

[Serializable]
public class titleComponent
{
    public TextMeshProUGUI       titleText;                                   // 타이틀 텍스트
    public GameObject            insideFullElement;                           // 타이틀이 선택됬을 시 활성화 시킬 요소들
    public List<insideLineList>  insideLine = new List<insideLineList>();     // 포커스한 타이틀의 내부 줄 수
}

[Serializable]
public class insideLineList
{
    public List<insideElementList> element = new List<insideElementList>();   // 각각의 내부 줄의 구성요소 갯수 
}

[Serializable]
public class insideElementList
{
    public GameObject elementFocus;        // 각각의 내부줄의 포커스 
    public GameObject elementComponent;    // 각각의 내부줄의 포커스된 오브젝트의 구성요소
}

