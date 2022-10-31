using UnityEngine;
public class EnemyTraking : MonoBehaviour
{
    // 켄버스 트레킹에 사용
    
    public  GameObject  body;   // Enemy 위치
    public  GameObject  hole;   // 전체삭제
        
    private void FixedUpdate()
    {
        if (body == true)
        {
            transform.position = body.transform.position;
        }
        else
        {
            Destroy(hole);
        }
    }
}