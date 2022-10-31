using UnityEngine;

public class SkillController : MonoBehaviour
{
    public void SkillHit()
	{
        PlayerSkill.instance.HitSkillRange();
	}

	public void CoolTimeReset()
	{
		PlayerSkill.instance.CoolTimeReset();
	}
	
	public void DistroySkillEffect()
	{
        Destroy(gameObject);
	}

}
