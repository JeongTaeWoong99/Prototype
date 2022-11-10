using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventPoint : MonoBehaviour
{
	// 설명
	public bool       explnationSet;
	
	// 생성
	public GameObject creatObjectSet;

	// 다음씬
	public bool       nextSeenSet;
	public string     nextSeenName;
	public float      waitToLoad;                               // 씬 전환 시간
	
	// 다음 이벤트 상황 완료
	public bool       autoEventCompleteSet;

	private void OnTriggerEnter2D(Collider2D other)
	{
		// 부딪히면
		if (other.CompareTag("Player"))
		{
			// 체크가 되어 있다면 -> 설명생성
			if (explnationSet)
			{
				UIEvent.instance.Explanation();
				Destroy(gameObject);
			}

			// 오브젝트가 들어가 있으면 -> 오브젝트 생성
			if (creatObjectSet)
			{
				creatObjectSet.gameObject.SetActive(true);
				Destroy(gameObject);
			}

			// 체크가 되어 있다면 -> 다음씬 넘어감
			if (nextSeenSet)
			{
				StartCoroutine(LevelEnd());
			}
			
			// 해당 위치로 이동 -> 테스트1번
			if (autoEventCompleteSet)
			{
				UIAutoSystem.instance.autoEventState = false;																	// 다시 이벤트 이어서
				//PlayerController.instance.theRB.velocity = new Vector2(PlayerController.instance.theRB.velocity.x, PlayerController.instance.theRB.velocity.y);		// 대쉬로 나가는거 방지
				PlayerController.instance.theRB.gravityScale = PlayerController.instance.originGavityScale;                     // 대쉬중 부딪혔을 때, 멀리나가는 문제 !! ☆★
				
				UIAutoSystem.instance.autoAtion[UIAutoSystem.instance.currtAtionNum-1].activeObject.SetActive(false);			// currtAtionNum++ 됐기 때문에, -1한 값을 꺼준다.
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