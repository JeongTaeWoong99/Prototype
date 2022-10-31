using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public  string startSeenName;									    // 새로운 게임시작 씬 이름
	private int	   currentNum = 0;										// 현재 선택된 넘버
	public List<TextMeshProUGUI> UI = new List<TextMeshProUGUI>();      // UI 리스트

	public GameObject continueBorad;

	private void Start()
	{
		UI[currentNum].color = new Color(1f,0f,0f,1f);
	}

	private void Update()
	{
		if (continueBorad.gameObject.activeInHierarchy == false)
		{
			var clamp = Mathf.Clamp(currentNum, 0, 5);
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				if (currentNum < 4)
				{
					UI[currentNum].color = new Color(1f, 1f, 1f, 1f);
					++currentNum;
					UI[currentNum].color = new Color(1f, 0f, 0f, 1f);
				}
			}

			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				if (currentNum > 0)
				{
					UI[currentNum].color = new Color(1f, 1f, 1f, 1f);
					currentNum--;
					UI[currentNum].color = new Color(1f, 0f, 0f, 1f);
				}
			}

			if (Input.GetKeyDown(KeyCode.Return))
			{
				switch (currentNum)
				{
					case 0:
						NewLife();
						break;
					case 1:
						Continue();
						break;
					case 2:
						LoadMap();
						break;
					case 3:
						Achievements();
						break;
					case 4:
						Exit();
						break;
				}
			}
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				continueBorad.SetActive(false);

			}
		}
	}

	private void NewLife()			
	{
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = Time.timeScale * 0.02f;
		SceneManager.LoadScene(startSeenName);
	}

	private void Continue()
	{
		// 첫 클리어 전 이어하기
	}

	private void LoadMap()
	{
		// 로드맵 보이기
		continueBorad.SetActive(true);
	}

	private void Achievements()
	{
		// 업적 UI 보이기

	}

	private void Exit()
	{
		Application.Quit();
	}
}