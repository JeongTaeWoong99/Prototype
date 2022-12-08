using System.Collections;
using UnityEngine;

public class BossMissCreate : MonoBehaviour
{
	[HideInInspector]
	public bool       phaseSpwanState;		// 무작위 빔 패턴(웨폰 phase2에서 제어)
	public GameObject laserPre;		        // 레이저
	public  float     spwanTimeValue;		// 발사 사긴 (레이저)
	private float     spwanCount;			// 시간체크  (레이저)
	public  float     angleMax;				// 최대 앵글
	public  float     angleMin;				// 최소 앵글
	
	public  GameObject missPre;		        // 미사일
	private Vector3    dir;					
	private float	   angle;
	

	
	// 레이저 뿌리기 (2군데 스폰 포인트)
	private void Update()
	{
		spwanCount -= Time.deltaTime;
		if (phaseSpwanState && spwanCount<=0)
		{
			var angle = Random.Range(angleMin, angleMax);
			Instantiate(laserPre, transform.position, Quaternion.Euler(0f,0f,angle));
			spwanCount = spwanTimeValue;
		}
		
	}
	
	// 미사일 뿌리기(10군데 스폰 포인트)
	public void MissCoroutine()
	{
		StopCoroutine( "MissCoroutineStart");
		StartCoroutine("MissCoroutineStart");
	}
	
	private IEnumerator MissCoroutineStart()
	{
		for (int j = 0; j < 3; j++)     // 3번
		{ 
			GameObject missClone = Instantiate(missPre, transform.position,Quaternion.identity);						// 미사일을 만들고
			dir   = (PlayerController.instance.transform.position - gameObject.transform.position).normalized;			// 플레이어를 바라보는 각도를 구하고
			angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;															// ☆
			var cloneMissBody  = missClone.transform.GetChild(0).gameObject;									// 만들어진 미사일의 0번째 자식, 즉 body를 회전시켜 주어야 함.
			cloneMissBody.transform.localRotation = Quaternion.Euler(0f,0f,angle);
			yield return new WaitForSeconds(0.8f);
		}

		yield return null;
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position, 0.1f);
	}
}
