using UnityEngine;

public class DissolveController : MonoBehaviour
{
    [HideInInspector]
    public float dissolveAmount;        // 0~1
    [HideInInspector]
    public bool  isDissolving;          // false 나타남 // true 사라짐
    public float dissolveSpeed;         // 속도
    
    [ColorUsageAttribute(true,true)]
    public Color outColor;
    [ColorUsageAttribute(true, true)]
    public Color inColor;
    
    private Material mat;               // 머터리얼 정보
    
    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }
    
    void Update()
    {
        // 사라짐
        if (isDissolving)
        {
            DissolveOut(outColor);
        }

        // 나타남
        if (!isDissolving)
        {
            DissolveIn(inColor);
        }

        // Amount를 계속 갱신 // 범위를 넘으면 멈춤
        mat.SetFloat("_DissolveAmount", dissolveAmount);
    }
    
    // 사라짐
    public void DissolveOut(Color color)
    {
        mat.SetColor("_DissolveColor", color);
        if (dissolveAmount > -0.1)
            dissolveAmount -= Time.deltaTime * dissolveSpeed;
    }

    // 나타남
    public void DissolveIn(Color color)
    {
        mat.SetColor("_DissolveColor", color);
        if (dissolveAmount < 1)
            dissolveAmount += Time.deltaTime * dissolveSpeed;
    }
}
