using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyTalk : MonoBehaviour
{
    public TextMeshProUGUI UITxet;
    public List<string>    aloneTalk	 = new List<string>();
    private int            listRandNmu;		                        	// 혼자 말하기 랜덤 번호
    public GameObject      textBoard;
    private float          randomTime;
    
    private void Awake()
    {
        StartCoroutine(AloneCoroutine());
    }
    
    private IEnumerator AloneCoroutine()
    {
        while (true)
        {
            listRandNmu = Random.Range(0, aloneTalk.Count);                  // 출력 문자열 선택(0 ~ 3-1)
            randomTime  = Random.Range(2f, 4f);                              // 말풍선 랜덤시간
            
            for (int j = 0; j < aloneTalk[listRandNmu].Length + 1; j++)
            {
                UITxet.text = aloneTalk[listRandNmu].Substring(0, j);        // 출력 문자열 수 증가
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(randomTime);
            textBoard.SetActive(false);
            yield return new WaitForSeconds(randomTime);
            textBoard.SetActive(true);
        }
    }
    
}