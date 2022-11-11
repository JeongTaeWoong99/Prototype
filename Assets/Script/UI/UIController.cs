using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public Image fadeScreen;             
    public float fadeSpeed;
    [HideInInspector]
    public bool  fadeOutBlack = true;
    [HideInInspector]
    public bool  fadeToBlack;

    public TextMeshProUGUI healthText;      // UI의 text      참조      
    public Slider          healthSlider;    // UI의 Slider    참조

	private int   currentNum = 0;                                       // 현재 선택된 넘버

	public GameObject  deathScreen;
	public List<TextMeshProUGUI> UI = new List<TextMeshProUGUI>();      // UI 리스트
	[HideInInspector]
	public bool		   deathState;

	public GameObject bossSlider;										// 보스 페널

	public Slider gageSlider;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		UI[currentNum].color = new Color(1f, 0f, 0f, 1f);
	}
	
	private void Update()
	{
		// death 작동
		Death();

		// 페이드 작동
		Fade();

	}

	private void Fade()
	{
		// storyTalkEndState이 끝나있어야 재생가능
		if (UIStoryTalk.instance.storyTalkEndState == true)
		{
			// fadeOutBlack가 true이면, 검정색 화면을 점점 밝게 함
			if (fadeOutBlack)
			{
				fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b,
					Mathf.MoveTowards(fadeScreen.color.a,0.0f,fadeSpeed * Time.deltaTime));
				// 화면이 다 밝아지면, fadeOutBlack 다시 false로 설정 
				if (fadeScreen.color.a < 0.05f)
				{
					fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b,0f);
					fadeOutBlack = false;
				}
			}
	
			// fadeToBlack가 true이면, 검정색 화면을 점점 어둠게 함
			if (fadeToBlack)
			{
				fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b,
					Mathf.MoveTowards(fadeScreen.color.a, 1.0f, fadeSpeed * Time.deltaTime));
				// 화면이 다 어두워지면, fadeToBlack 다시 false로 설정 
				if (fadeScreen.color.a > 0.95f)
				{
					fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b,1f);
					fadeToBlack = false;
				}
			}
			
		}
	}

	public void ReStart()
	{
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = Time.timeScale * 0.02f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	private void MainMenu()
	{
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = Time.timeScale * 0.02f;
		SceneManager.LoadScene(0);
	}

	private void Death()
	{
		if (deathState == true)
		{
			var clamp = Mathf.Clamp(currentNum, 0, 2);
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				if (currentNum < 1)
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
						ReStart();
						break;
					case 1:
						MainMenu();
						break;

				}
			}
		}
	}

}