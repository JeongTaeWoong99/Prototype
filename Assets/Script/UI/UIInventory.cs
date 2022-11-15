using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public static UIInventory instance;
    
    public bool       puaseState;	// true 켜짐 false 꺼짐
    public GameObject puasePanel;
    
    private int	   currentNum = 0;						   				            // 현재 선택된 넘버(시작은 0번)
    public List<TextMeshProUGUI> inventoryList  = new List<TextMeshProUGUI>();      // 선택된 텍스트 리스트
    public List<GameObject>      invenComponent = new List<GameObject>();           // 인벤구성요소 리스트 

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        instance = this;

        inventoryList[currentNum].color = new Color(1f,0f,0f,1f);       // 인벤토리가 활성화되면, currentNum 번호에 따라 텍스트빨간불 및 구성요소 보이게
        invenComponent[currentNum].SetActive(true);                             // 구성요소 활성화
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
            }
            else if(!puaseState && PlayerParing.instance.paringState && UIEvent.instance.eventState && UIStoryTalk.instance.storyTalkEndState)
            {
                Time.timeScale = 0;                        // 시간 멈춤    
                puaseState = true;
                puasePanel.gameObject.SetActive(true);
            }
        }
        
        // I키 활성화 상태 R L 키사용
        if (puaseState)
        {
            Mathf.Clamp(currentNum, 0, 4);
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (currentNum < 3)
                {
                    // 이전 색변화 및 구성요소 비활성화
                    inventoryList[currentNum].color = new Color(1f, 1f, 1f, 1f);
                    invenComponent[currentNum].SetActive(false); 
                    currentNum++;
                    // 바뀐 텍스트색 및 구성요소 활성화
                    inventoryList[currentNum].color = new Color(1f, 0f, 0f, 1f);
                    invenComponent[currentNum].SetActive(true); 
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (currentNum > 0)
                {
                    // 이전 색변화 및 구성요소 비활성화
                    inventoryList[currentNum].color = new Color(1f, 1f, 1f, 1f);
                    invenComponent[currentNum].SetActive(false); 
                    currentNum--;
                    // 바뀐 텍스트색 및 구성요소 활성화
                    inventoryList[currentNum].color = new Color(1f, 0f, 0f, 1f);
                    invenComponent[currentNum].SetActive(true); 
                }
            }
        }
    }
}
