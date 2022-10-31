using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static CameraController instance;
	
	[HideInInspector]
	public  Camera     mainCam;	     // 카메라 컴포넌트
	public  float      moveSpeed;    // 카메라 이동속도
	public  Transform  target;       // 카메라가 이동할 위치
	
	public  float      seenWoladY;   // 메인카메라 Y위치
	public  float      focusSpeed;   // 포커스 스피드
	public  GameObject focusNPC;     // 포커스 할 NPC

	public  bool       focusIn;

	private void Awake()
	{
		instance = this;

		mainCam = GetComponent<Camera>();
	}

	private void Update()
	{ 
		// 포커스할 focusNPC 없을 때
		if (focusNPC == false)
		{
			// 카메라 포지션 업데이트
			// 기존 위치에서 x,y 값을 target 포지션으로 이동하고
			// z 위치는 -10으로, 기존 위치와 동일하게 이동한다.(카메라 각도 이슈)
			transform.position = Vector3.MoveTowards(transform.position,
				                                      new Vector3(target.position.x, transform.position.y, transform.position.z),
					                          moveSpeed * Time.deltaTime);
		}
		// focusNPC가 true이고, focusIn이 true 일 때
		else if (focusIn == true)
		{
			if (CompareTag("NPC"))
			{
				mainCam.orthographicSize = 1.2f;
                				transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, transform.position.z),
                					new Vector3(focusNPC.transform.position.x,focusNPC.transform.position.y, transform.position.z),
                					focusSpeed * Time.unscaledTime);
			}
			else
			{
				transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, transform.position.z),
					                                      new Vector3(focusNPC.transform.position.x,focusNPC.transform.position.y, transform.position.z),
					                              focusSpeed * Time.unscaledTime);
			}

		}
		// focusNPC가 null이고, focusIn이 false 일 때
		else if(focusIn == false)
		{
			focusNPC = null;
			mainCam.orthographicSize = 1.8f;
			transform.position = new Vector3(target.transform.position.x,seenWoladY,transform.position.z);
			
		}
		

	}
	
}