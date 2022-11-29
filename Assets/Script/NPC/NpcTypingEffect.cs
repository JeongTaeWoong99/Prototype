using System.Collections;       // IEnumerator ???
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcTypingEffect : MonoBehaviour
{
	public GameObject message;                                  // ??? ????? 
	public GameObject bodySR;
	public GameObject focusPoint;								// ??¨¨?? ?????

	public Canvas canvas;
	public TextMeshProUGUI NPCTxet;                        
	public List<string> aloneTalk	 = new List<string>();
	public List<string> togetherTalk = new List<string>();

	private bool inZoon;				// ?úô ????
	private int  togetherTalkNum;		// ??? ????? ???? ???
	private int  listRandNmu;			// ??? ????? ???? ???

	public bool choiceTlak;

	private void Awake()
	{
		StartCoroutine(AloneCoroutine());
	}

	private void Update()
	{
		if (inZoon == true)
		{
			// ?????? ???¡Æ? ??? ??
			if (!PlayerController.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Dash"))
			{
				NPCTalck();
			}
		}
	}

	private void NPCTalck()
	{
		 // 0?? ???[0] -> 1??¡Æ ???
         if (Input.GetKeyDown(KeyCode.E) && inZoon == true && togetherTalkNum == 0)
         {
         		CameraController.instance.focusPoint = focusPoint;											// ??¨¨?? ?? NPC
         		CameraController.instance.focusIn  = true;													// focusIn true
         		UIEvent.instance.eventState        = false;													// ???? ???? ??(?????? X)
         		PlayerController.instance.theRB.velocity = new Vector2(0.0f, 0.0f);						// ???????(E?????? ?????? ??? ????)
         		
         		message.SetActive(false);																	// ?????
         		UIController.instance.healthText.gameObject.SetActive(false);
                UIController.instance.healthSlider.gameObject.SetActive(false);
         		
         		// ?¢¯????(NPC/PLAYER)
         		if (PlayerController.instance.gameObject.transform.position.x - transform.position.x >= 0)
         		{
         			bodySR.transform.localScale = new Vector2(1f, 1f);
         			PlayerController.instance.gameObject.transform.localScale = new Vector2(-1f, 1f);
         		}
         		else
         		{
         			bodySR.transform.localScale = new Vector2(-1f, 1f);
         			PlayerController.instance.gameObject.transform.localScale = new Vector2(1f, 1f);
         		}
         		
         		StopAllCoroutines();																		// TogatherCoroutines() ???? ???? ?????	
         		togetherTalkNum++;
         		StartCoroutine(TogatherCoroutines());
         }
         // ?????? ????¥å?, E? ?????? ??(?ò÷???? ?? ¨¨??) -> ????
         else if (Input.GetKeyDown(KeyCode.E) && inZoon == true && togetherTalkNum + 2 > togetherTalk.Count)
         {
         	CameraController.instance.focusIn   = false;		// focusIn false
         	UIEvent.instance.eventState         = true;			// ???? ??
         	
         	message.SetActive(true);																	// ?????
            UIController.instance.healthText.gameObject.SetActive(true);
            UIController.instance.healthSlider.gameObject.SetActive(true);
         	
         	StopAllCoroutines();
         	togetherTalkNum = 0;								// ???????
         	StartCoroutine(TogatherCoroutines());
         }
         // 2??¡Æ ??? ~ ?????? ???
         else if (Input.GetKeyDown(KeyCode.E) && inZoon == true)
         {
         	StopAllCoroutines();									
         	togetherTalkNum++;
         	StartCoroutine(TogatherCoroutines());
         }
	}
	
	private IEnumerator AloneCoroutine()
	{
		if (aloneTalk.Count != 0)
		{
			canvas.gameObject.SetActive(true);
			
			while (true)
            {
            	listRandNmu = Random.Range(0, aloneTalk.Count);                  // ??? ????? ????(0 ~ 3-1)
          
            	for (int j = 0; j < aloneTalk[listRandNmu].Length + 1; j++)
            	{
            		NPCTxet.text = aloneTalk[listRandNmu].Substring(0, j);        // ??? ????? ?? ????
            		yield return new WaitForSeconds(0.15f);
            	}
            	yield return new WaitForSeconds(3.0f);
            }
		}
		
	}

	private IEnumerator TogatherCoroutines()
	{
		if(togetherTalk.Count != 0)
		{
			canvas.gameObject.SetActive(true);
			
			for (int j = 0; j < togetherTalk[togetherTalkNum].Length + 1; j++)
			{
				NPCTxet.text = togetherTalk[togetherTalkNum].Substring(0, j);
				yield return new WaitForSeconds(0.1f);
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			// ?¢¯????
			if (PlayerController.instance.gameObject.transform.position.x - transform.position.x >= 0)
				bodySR.transform.localScale = new Vector2(1f, 1f);
			else
				bodySR.transform.localScale = new Vector2(-1f, 1f);
			
			message.SetActive(true);					// ??? ?????
			inZoon = true;								// ?úô????
			StopAllCoroutines();
			StartCoroutine(TogatherCoroutines());  // 0?? ???
		}
	}

	public void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			message.SetActive(false);					// ??? ?????
			inZoon = false;								// ?úô????
			togetherTalkNum = 0;						// Togather???? ?????? ????
			StopAllCoroutines();
			StartCoroutine(AloneCoroutine());
		}
	}
}
