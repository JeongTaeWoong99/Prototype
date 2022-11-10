using UnityEngine;

public class Breakables : MonoBehaviour
{
    public GameObject[] brokenPiece;        // 파편 배열
    public int          maxPieces;          // 파편 최대 갯수

    public void Smash()
	{
        // 파괴
        Destroy(gameObject);

        // 파편 조각 생성 3~6개
        int piecesToDrop = Random.Range(3, maxPieces);
        
        // 조각생성 반복
        for (int i = 0; i < piecesToDrop; i++)
        {
            int angle       = Random.Range(0, 180);                 // 날라가는 각도
            int randomPiece = Random.Range(0, brokenPiece.Length);  // 랜덤 조각 모양 선택
            Instantiate(brokenPiece[randomPiece], transform.position, Quaternion.Euler(0, 0,  angle));
        }

    }

}
