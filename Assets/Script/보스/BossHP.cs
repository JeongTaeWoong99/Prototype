using System.Collections;
using UnityEngine;

public class BossHP : MonoBehaviour
{
    public static BossHP instance;
    
    [SerializeField]
    public  float          maxHP = 1000;
    public  float          currentHP;
    private SpriteRenderer spriteRenderer;

    // 슬라이더 기능
    // public float MaxHP     => maxHP;
    // public float CurrentHP => currentHP;

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
        // 보스 색상을 빨간색으로
        spriteRenderer.color = Color.red;
        // 0.05초 동안 대기
        yield return new WaitForSeconds(0.05f);
        // 보스의 색상을 원래 색상인 하얀색으로
        // (원래 색상이 하얀색이 아닐 경우 원래 색상 변수 선언)
        spriteRenderer.color = Color.white;
    }
}

