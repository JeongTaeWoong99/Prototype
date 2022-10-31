using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public static PlayerSkill instance;

    public  LayerMask  enemyLayer;

    public List<GameObject> skillPrefabs     = new List<GameObject>(); 
    public List<GameObject> damagePointSkill = new List<GameObject>();      
    public List<int>        damageToGive     = new List<int>();                        
    public List<float>      skillCooldown    = new List<float>();
    private float[]         skillCoolCounter = new float[9];

    private int keyNum;                                     // 스킬번호

    private void Awake()
    {
        instance   = this;
    }

    void Update()
    {
        // Q 쿨타임 감소
        if (skillCoolCounter[0] > 0.0f)                  
        {
            skillCoolCounter[0] -= Time.deltaTime;    
        }
        
        // 쿨타임 감소
    }

    public void SetSkill(string key)
	{
        if (key == "Q")
        {
            keyNum = 0;                         // 스킬번호
            if (skillCoolCounter[keyNum] <= 0)
            {
                PlayerController.instance.animator.SetTrigger("Skill_Q");
            }
        }
    }
    
    public void HitSkillRange()
    {
        Collider2D[] hit = Physics2D.OverlapBoxAll(damagePointSkill[keyNum].transform.position, new Vector2(1.7f, 1.0f), 0, enemyLayer);
        for (var i = 0; i < hit.Length; ++i)
        {
            if(hit[i].GetComponent<EnemyController>() == true)
                hit[i].GetComponent<EnemyController>().DamageEnemy(damageToGive[keyNum]);
            
            if(hit[i].GetComponent<Breakables>() == true)
                hit[i].GetComponent<Breakables>().Smash();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(damagePointSkill[0].transform.position, new Vector2(1.7f, 1f));
    }
    
    // 스킬애니메이션이 플레이어스킬애니메이션 보다 빨리 끝나면 안됨.(-> 남아있는 keyNum값의 스킬쿨 돌아감)
    public void CoolTimeReset()
	{
        skillCoolCounter[keyNum] = skillCooldown[keyNum];
    }
    
    public void MakeSkill()
	{
        Instantiate(skillPrefabs[keyNum], damagePointSkill[keyNum].transform.position, Quaternion.identity);
    }

}