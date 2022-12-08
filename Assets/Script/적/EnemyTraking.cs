using UnityEngine;
public class EnemyTraking : MonoBehaviour
{
    public  GameObject  body;   // 따라다닐 위치
    public  GameObject  hole;   // 따라다니던 오브젝트가 파괴될 시 없앨 본인
        
    private void FixedUpdate()
    {
        if (body)
        {
             transform.position = body.transform.position;
        }
        else if(body == false)
        {
            Destroy(hole);
        }
    }
} 