using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventPoint : MonoBehaviour
{
	public bool       explnationSet;
	
	public GameObject creatObjectSet;

	public bool       nextSeenSet;
	public string     nextSeenName;
	public float      waitToLoad;                               // 씬 전환 시간

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			if (explnationSet == true)
			{
				UIEvent.instance.Explanation();
				Destroy(gameObject);
			}

			if (creatObjectSet == true)
			{
				creatObjectSet.gameObject.SetActive(true);
				Destroy(gameObject);
			}

			if (nextSeenSet == true)
			{
				StartCoroutine(LevelEnd());
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