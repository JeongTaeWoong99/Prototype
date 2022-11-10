using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIEvent : MonoBehaviour
{
	public static UIEvent instance;
	
	[HideInInspector]
	public bool        eventState = true;		   // 이벤트 중 false
	public bool        bossRoomState;			   // boss
	
	public  Transform  startCameraMovePoint;	   // 이동위치				  
	public  GameObject backGround;				   // 움직이지 않는 뒷배경(->삭제)
	private bool       startCameraMoveState;	   // 카메라 1회 호출 체크
	public  float      fadeToTriggerTime;		   // 페이드가 끝난 후 가다리는시간
	public  float      playerToMovePointTime;      // =                       
	public  float      movePointToPlayer;          // =						  
	
	private float      cameraWaitingtimeCount;     // 시간카운트

	private bool             explanationState   = true;		               // 설명화면 중 false
	public  GameObject       explanationBorad;
	public  TextMeshProUGUI  explanationText;
	public  List<GameObject> explanation        = new List<GameObject>();  // explanation의 GameObject
	public  List<string>     explanationString  = new List<string>();      // explanation의 String
	private int              explanationNum;
	
	
	private void Awake()
	{
		instance = this;
		Time.timeScale = 1.0f; //안전장치
	}

	private void Update()
	{
		// 메인스토리가 끝나고, 시작 무브 포인트가 있고, startCameraMoveState == false 일때
		if (UIStoryTalk.instance.storyTalkEndState == true && startCameraMovePoint == true && startCameraMoveState == false)
		{
			StartCameraMove();
		}

		// 설명상태 전환
		if (explanationState == false && Input.GetKeyDown(KeyCode.E))
		{
			Explanation();
		}
	}

	public void Explanation()
	{
		// StartCameraMove()가 끝난 후,  Explanation() 한번 재생 되는 것을 생각.
		// 진행 중 -> 설명 
		if (explanationState)
		{			
			explanationState = false;	
			eventState       = false;								// 이벤트 진행 중
			Time.timeScale   = 0.0f;								// 멈춤
			
			explanation[explanationNum].gameObject.SetActive(true);
			explanationBorad.gameObject.SetActive(true);
			
			explanationText.gameObject.SetActive(true);				
			StartCoroutine(StringCoroutines());
		}
		else if (explanationState == false)
		{
			explanationState = true;
			eventState = true;										  // 이벤트 진행 X
			Time.timeScale   = 1.0f;							      // 정상화
			
			explanation[explanationNum].gameObject.SetActive(false);
			explanationBorad.gameObject.SetActive(false);
			
			explanationText.gameObject.SetActive(false);
			StopAllCoroutines();
			
			explanationNum++;										  // 번호 증가★
		}
		
	}
	
	private IEnumerator StringCoroutines()
	{
		for (int j = 0; j < explanationString[explanationNum].Length + 1; j++)
		{
			explanationText.text = explanationString[explanationNum].Substring(0, j);
			yield return new  WaitForSecondsRealtime(0.05f);
		}
	}

	private void StartCameraMove()
	{
		cameraWaitingtimeCount += Time.deltaTime;
		eventState              = false;			// 이벤트 중
		
		// 다시 돌아와서, 위치가 같아지고, 시간이 넘었을 때
		if (CameraController.instance.transform.position.x == PlayerController.instance.transform.position.x 
		    && cameraWaitingtimeCount >= movePointToPlayer)
		{
			UIController.instance.healthText.gameObject.SetActive(true);
			UIController.instance.healthSlider.gameObject.SetActive(true);
			
			Destroy(backGround);				// 파괴
			startCameraMoveState = true;		// StartCameraMove() 더이상 호출 X				// 조건 걸어서 카메라 무브 여러번 사용하기
			eventState           = true;		// 이벤트 끝										// 조건 걸어서 계속 달리기
			
			//[0]이 있을 경우에만 실행
			if (explanation[0].gameObject != false)
			{
				Explanation();						// 설명 화면 출력(eventState -> false 뒤에 위치해야 함) ★
			}
			// ++ 해줘야 요소 1번부터 재생 됨.
			else
			{
				explanationNum++;										  // 번호 증가★
			}
		}
		// cameraWaitingtimeCount * 3.0f 넘으면
		else if (cameraWaitingtimeCount >= playerToMovePointTime)
		{
			CameraController.instance.target = PlayerController.instance.bodySR.transform;
		}
		// cameraWaitingtimeCount 넘으면 
		else if (cameraWaitingtimeCount >= fadeToTriggerTime)
		{
			CameraController.instance.target = startCameraMovePoint;
			UIController.instance.healthText.gameObject.SetActive(false);
			UIController.instance.healthSlider.gameObject.SetActive(false);
		}
	}

}
