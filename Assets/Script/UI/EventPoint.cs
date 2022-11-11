using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventPoint : MonoBehaviour
{
	// 설명
	public bool       explnationBool; 
 	
	// 생성
	public bool       creatObjectBool;
	public GameObject creatObject;

	// 다음씬
	public bool       nextSeenBool;   
	public string     nextSeenName;  
	public float      waitToLoad;                               // 씬 전환 시간
	
	// 다음 이벤트 상황 완료
	public bool         autoEventBool;
	public GameObject[] eventObjectDistroy;        // 삭제
	

	private void OnTriggerEnter2D(Collider2D other)
	{
		// 부딪히면
		if (other.CompareTag("Player"))
		{
			// 체크가 되어 있다면 -> 설명생성
			if (explnationBool)
			{
				UIEvent.instance.Explanation();
				Destroy(gameObject);
			}

			// 오브젝트가 들어가 있으면 -> 오브젝트 생성
			if (creatObjectBool)
			{
				creatObject.gameObject.SetActive(true);
				Destroy(gameObject);
			}

			// 체크가 되어 있다면 -> 다음씬 넘어감
			if (nextSeenBool)
			{
				StartCoroutine(LevelEnd());
			}
			
			//  테스트
			if(autoEventBool)
			{
				UIAutoSystem.instance.autoEventState = false;																	// 다시 이벤트 이어서
				// 삭제
				for(int i = 0;i<eventObjectDistroy.Length;i++)
					Destroy(eventObjectDistroy[i]);
			}
			
			
		}
	}
	
	private IEnumerator LevelEnd()
	{
		UIController.instance.fadeToBlack           = true;       // 화면 어둠게
		PlayerHpController.instance.invincibleCount = waitToLoad; // 플레이어 무적
		UIEvent.instance.eventState                 = false;	  // 이벤트 중 이므로 플레이어 움직이지 않음
		
		yield return new WaitForSeconds(waitToLoad);              // 씬 전환 딜레이
		
		SceneManager.LoadScene(nextSeenName);
	}
}