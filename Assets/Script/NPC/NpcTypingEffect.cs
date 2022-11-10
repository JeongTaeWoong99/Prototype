using System.Collections;       // IEnumerator 사용
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcTypingEffect : MonoBehaviour
{
	public GameObject message;                                  // 안내 메세지 
	public GameObject bodySR;
	public GameObject focusPoint;								// 포커스 포인트

	public Canvas canvas;
	public TextMeshProUGUI NPCTxet;                        
	public List<string> aloneTalk	 = new List<string>();
	public List<string> togetherTalk = new List<string>();

	private bool inZoon;				// 충돌 상태
	private int  togetherTalkNum;		// 함께 말하기 텍스트 번호
	private int  listRandNmu;			// 혼자 말하기 랜덤 번호

	public bool choiceTlak;

	private void Awake()
	{
		StartCoroutine(AloneCoroutine());
	}

	private void Update()
	{
		if (inZoon == true)
		{
			// 구르기 상태가 아닐 때
			if (!PlayerController.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Dash"))
			{
				NPCTalck();
			}
		}
	}

	private void NPCTalck()
	{
		 // 0번 대사[0] -> 1번째 대사
         if (Input.GetKeyDown(KeyCode.E) && inZoon == true && togetherTalkNum == 0)
         {
         		CameraController.instance.focusNPC = focusPoint;											// 포커스 할 NPC
         		CameraController.instance.focusIn  = true;													// focusIn true
         		UIEvent.instance.eventState        = false;													// 이벤트 진행 중(움직임 X)
         		PlayerController.instance.theRB.velocity = new Vector2(0.0f, 0.0f);						// 이동멈춤(E누르고 나가는 경우 방지)
         		
         		message.SetActive(false);																	// 감추기
         		UIController.instance.healthText.gameObject.SetActive(false);
                UIController.instance.healthSlider.gameObject.SetActive(false);
         		
         		// 좌우반전(NPC/PLAYER)
         		if (PlayerController.instance.gameObject.transform.position.x - transform.position.x >= 0)
         		{
         			bodySR.transform.localScale = new Vector2(1f, 1f);
         			PlayerController.instance.gameObject.transform.localScale = new Vector2(-1f, 1f);
         		}
         		else
         		{
         			bodySR.transform.localScale = new Vector2(-1f, 1f);
         			PlayerController.instance.gameObject.transform.localScale = new Vector2(1f, 1f);
         		}
         		
         		StopAllCoroutines();																		// TogatherCoroutines() 앞대사 코루틴 멈추기	
         		togetherTalkNum++;
         		StartCoroutine(TogatherCoroutines());
         }
         // 마지막 대사인데, E키 눌렀을 때(배열보다 더 커짐) -> 복귀
         else if (Input.GetKeyDown(KeyCode.E) && inZoon == true && togetherTalkNum + 2 > togetherTalk.Count)
         {
         	CameraController.instance.focusIn   = false;		// focusIn false
         	UIEvent.instance.eventState         = true;			// 이벤트 끝
         	
         	message.SetActive(true);																	// 감추기
            UIController.instance.healthText.gameObject.SetActive(true);
            UIController.instance.healthSlider.gameObject.SetActive(true);
         	
         	StopAllCoroutines();
         	togetherTalkNum = 0;								// 처음으로
         	StartCoroutine(TogatherCoroutines());
         }
         // 2번째 대사 ~ 마지막 대사
         else if (Input.GetKeyDown(KeyCode.E) && inZoon == true)
         {
         	StopAllCoroutines();									
         	togetherTalkNum++;
         	StartCoroutine(TogatherCoroutines());
         }
	}
	
	private IEnumerator AloneCoroutine()
	{
		if (aloneTalk.Count != 0)
		{
			canvas.gameObject.SetActive(true);
			
			while (true)
            {
            	listRandNmu = Random.Range(0, aloneTalk.Count);                  // 출력 문자열 선택(0 ~ 3-1)
          
            	for (int j = 0; j < aloneTalk[listRandNmu].Length + 1; j++)
            	{
            		NPCTxet.text = aloneTalk[listRandNmu].Substring(0, j);        // 출력 문자열 수 증가
            		yield return new WaitForSeconds(0.15f);
            	}
            	yield return new WaitForSeconds(3.0f);
            }
		}
		
	}

	private IEnumerator TogatherCoroutines()
	{
		if(togetherTalk.Count != 0)
		{
			canvas.gameObject.SetActive(true);
			
			for (int j = 0; j < togetherTalk[togetherTalkNum].Length + 1; j++)
			{
				NPCTxet.text = togetherTalk[togetherTalkNum].Substring(0, j);
				yield return new WaitForSeconds(0.1f);
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			// 좌우반전
			if (PlayerController.instance.gameObject.transform.position.x - transform.position.x >= 0)
				bodySR.transform.localScale = new Vector2(1f, 1f);
			else
				bodySR.transform.localScale = new Vector2(-1f, 1f);
			
			message.SetActive(true);					// 안내 메세지
			inZoon = true;								// 충돌상태
			StopAllCoroutines();
			StartCoroutine(TogatherCoroutines());  // 0번 대사
		}
	}

	public void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			message.SetActive(false);					// 안내 메세지
			inZoon = false;								// 충돌상태
			togetherTalkNum = 0;						// Togather텍스트 시작번호 초기화
			StopAllCoroutines();
			StartCoroutine(AloneCoroutine());
		}
	}
}
