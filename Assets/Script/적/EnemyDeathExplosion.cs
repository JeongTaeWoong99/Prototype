using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyDeathExplosion : MonoBehaviour
{
	public GameObject[] brokenPiece;        // 파편 배열
	public GameObject deathExplosion;		// 사망 폭발

	public void Smash()
	{
		// 파괴
		Destroy(gameObject);
		
		int piecesToDrop = brokenPiece.Length;
        
		// 조각생성 반복
		for (int i = 0; i < piecesToDrop; i++)
		{
			int angle       = Random.Range(0, 180);                 // 날라가는 각도
			Instantiate(brokenPiece[i], transform.position, Quaternion.Euler(0, 0,  angle));
		}
		Instantiate(deathExplosion, transform.position, quaternion.identity);

	}
}
