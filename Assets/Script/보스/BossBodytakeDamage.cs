using System.Collections;
using UnityEngine;

public class BossBodytakeDamage : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        BossHP.instance.TakeDamage(damage);
        
        StopCoroutine("HitColorAnimation");
        StartCoroutine("HitColorAnimation");
    }

    private IEnumerator HitColorAnimation()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
    

}