using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStoryTalk : MonoBehaviour
{
	public static UIStoryTalk instance;
	
	public  TextMeshProUGUI UITxet;
	public  List<string>    storyTalk = new List<string>();
	[HideInInspector]
	public  bool			storyTalkEndState;
	private int			    talkNum;

	private void Awake()
	{
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = Time.timeScale * 0.02f;
		instance = this;
	}

	private void Start()
	{
		// 스토리talk가 없을경우 바로 true
		if (storyTalk.Count == 0)
			storyTalkEndState = true;
		else
			StartCoroutine(StoryCoroutine());
	}

	private void Update()
	{
		// 스토리 스킵(Enter + 스토리 나오는 중)
		if (Input.GetKeyDown(KeyCode.E) && storyTalkEndState == false)
		{
			storyTalkEndState = true;				 // 스토리토크 상태 false -> true
			UITxet.gameObject.SetActive(false);      // 텍스트 가리기
			StopCoroutine(StoryCoroutine());	 
		}
	}

	private IEnumerator StoryCoroutine()
	{
		while (true)
		{
			foreach (char c in storyTalk[talkNum])
			{
				UITxet.text += c;
				yield return new WaitForSeconds(0.1f);
			}
			
			yield return new WaitForSeconds(2.0f);
			UITxet.text += Environment.NewLine + Environment.NewLine;		// 줄 바꾸기
			talkNum++;

			// 스토리 종료
			if (talkNum == storyTalk.Count)
			{
				storyTalkEndState = true;				 // 스토리토크 상태 false -> true
				UITxet.gameObject.SetActive(false);      // 텍스트 가리기
				StopCoroutine(StoryCoroutine());
				break;
			}
		}
	}
	
	// private IEnumerator StoryCoroutine()
	// {
	// 	while (true)
	// 	{
	// 		for (int j = 0; j < storyTalk[talkNum].Length + 1; j++)
	// 		{
	// 			UITxet.text = storyTalk[talkNum].Substring(0, j);        // 출력 문자열 수 증가
	// 			yield return new WaitForSeconds(0.1f);
	// 		}
	// 		talkNum++;
	// 		yield return new WaitForSeconds(2.0f);
	//
	// 		// 스토리 종료
	// 		if (talkNum == storyTalk.Count)
	// 		{
	// 			storyTalkEndState = true;
	// 			UITxet.gameObject.SetActive(false);      // 텍스트 가리기
	// 			StopCoroutine(StoryCoroutine());
	// 			break;
	// 		}
	// 	}
	// }

}