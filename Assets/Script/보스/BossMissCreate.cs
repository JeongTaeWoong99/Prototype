using System.Collections;
using UnityEngine;

public class BossMissCreate : MonoBehaviour
{
	public bool phaseSpwanState;		// 무작위 빔 패턴
	public bool phaseSpwanState2;		// 미사일 패턴
	public GameObject spwanMiss;
	public GameObject spwanMiss2;
	public float angleMax;
	public float angleMin;

	public  float spwanTimeValue;
	private float spwanCount;
	
	private void Update()
	{
		spwanCount -= Time.deltaTime;
		if (phaseSpwanState && spwanCount<=0)
		{
			var angle = Random.Range(angleMin, angleMax);
			Instantiate(spwanMiss, transform.position, Quaternion.Euler(0f,0f,angle));
			spwanCount = spwanTimeValue;
		}
		
	}
	
	public void MissCoroutine()
	{
		StopCoroutine("MissCoroutineStart");
		StartCoroutine("MissCoroutineStart");
	}

	private IEnumerator MissCoroutineStart()
	{
		for (int j = 0; j < 3; j++)     // 3번
		{
			var angle = UnityEngine.Random.Range(angleMin, angleMax);
			Instantiate(spwanMiss2, transform.position, Quaternion.Euler(0f,0f,angle));
			yield return new WaitForSeconds(0.8f);
		}

		yield return null;
	}
}
