using UnityEngine;
using UnityEngine.UI;               // 슬라이더 사용을 위해 추가

public class BossHPViewer : MonoBehaviour
{
	private Slider sliderHP;

	private void Awake()
	{
		sliderHP = GetComponent<Slider>();
	}

	private void Update()
	{
		// Slider UI에 체력 정보를 업데이트
		sliderHP.value = BossHP.instance.currentHP / BossHP.instance.maxHP;
	}

}