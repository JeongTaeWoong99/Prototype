using System.Collections;
using UnityEngine;

public class BossHP : MonoBehaviour
{
    public static BossHP instance;
    
    [SerializeField]
    public  float          maxHP;
    public  float          currentHP;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        instance = this;
        
        currentHP      = maxHP;                          // 현재 체력을 최대 체력과 같게 설정
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        // 현재 체력을 damage만큼 감소
        currentHP -= damage;

        StopCoroutine("HitColorAnimation");
        StartCoroutine("HitColorAnimation");

        // 체력이 0이하 = 보스 사망
        if (currentHP <= 0)
        {
            Boss.instance.OnDie();
        }
    }

    private IEnumerator HitColorAnimation()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
        Debug.Log("실행");
    }
}

