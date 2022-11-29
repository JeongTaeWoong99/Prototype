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
		{
			storyTalkEndState = true;
			UITxet.gameObject.SetActive(false);      // 텍스트 가리기	
		}
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

	
	// 0번 원칙
	private IEnumerator ZeroPrinciple()
	{
		foreach (char c in storyTalk[talkNum])
		{
			UITxet.text += c;
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(2.0f);
		//  화면 지지직 효과
		//
		yield return new WaitForSeconds(2.0f);
		UITxet.text = "";
		talkNum++;


	}

	private IEnumerator StoryCoroutine()
	{
		foreach (char c in storyTalk[talkNum])
		{
			UITxet.text += c;
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(2.0f);
		
		//  화면 지지직 효과 
		//  
		//
		
		yield return new WaitForSeconds(2.0f);
		UITxet.text = "";
		talkNum++;

		// 초록색으로 색변경
		//
		//
		
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

}