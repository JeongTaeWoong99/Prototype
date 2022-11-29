using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static CameraController instance;
	
	[HideInInspector]
	public  Camera     mainCam;	     // 카메라 컴포넌트
	public  float      moveSpeed;    // 카메라 이동속도
	public  Transform  target;       // 카메라가 이동할 위치
	
	public  float      focusSpeed;   // 포커스 스피드
	public  GameObject focusPoint;   // 포커스 할 NPC

	[HideInInspector]
	public  bool       focusIn;

	[HideInInspector] 
	public float originOrthographicSize;			// 원래크기
	public float originFocusOrthographicSize;       // 축소 할 크기
	private void Awake()	
	{
		instance = this;
		
		mainCam = GetComponent<Camera>();
		originOrthographicSize = mainCam.orthographicSize;
	}

	private void Update()
	{ 
		// 포커스할 focusNPC 없고, 타겟이 있을 때 (플레이어 따라다님)
		if (focusPoint == false && target)
		{
			// moveSpeed * Time.deltaTime
			transform.position = Vector3.MoveTowards(transform.position,
				                                      new Vector3(target.position.x, target.position.y, transform.position.z),
					                          moveSpeed * Time.deltaTime);
		}
		// focusNPC가 true이고, focusIn이 true 일 때
		else if (focusIn)
		{
			// NPC 포커스 포인트 (포커스 스피드 1배)
			if (CompareTag("NPC"))
			{
				mainCam.orthographicSize = originFocusOrthographicSize;
				transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, transform.position.z),
														  new Vector3(focusPoint.transform.position.x,focusPoint.transform.position.y, transform.position.z),
												  focusSpeed * Time.unscaledDeltaTime);
			}
			// Z키 포커스 포인트 (포커스 스피드 3배)
			else
			{
				transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, transform.position.z),
					                                      new Vector3(focusPoint.transform.position.x,focusPoint.transform.position.y, transform.position.z),
					                              focusSpeed * 3f * Time.unscaledDeltaTime);
			}

		}
		// focusNPC가 null이고, focusIn이 false 일 때
		else if(focusIn == false)
		{
			focusPoint = null;
			mainCam.orthographicSize = originOrthographicSize;
			//transform.position = new Vector3(target.transform.position.x,seenWoladY,transform.position.z);
		}
		
	}
	
}