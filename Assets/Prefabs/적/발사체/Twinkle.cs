using System.Collections;
using UnityEngine;

public class Twinkle : MonoBehaviour
{
    private float          fadeTime = 0.1f;
    private SpriteRenderer spriteRenderer; // AlertLine의 SpriteRenderer을 Inspector view에서 수정하기 위해
                                           // SpriteRenderer 타입의 변수를 만들어서, Color 등등을 수정함.
                                           
    public  float      creatCool;			// 생성시간
    private float      creatCoolCount;      // 생성시간 체크
    private void       Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine("TwinkleLoop");
    }

    private void Update()
    {
        creatCoolCount += Time.deltaTime;
        if (creatCoolCount > creatCool)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator TwinkleLoop()
    {
        // ☆☆☆☆☆☆☆
        // 코루틴 메소드의 yield return 문항에서 StartCoroutine()을 싱행할 경우
        // StartCoroutine()의 코루틴이 모두 끝나야 다음 문항으로 넘어갈 수 있다.
        while (true)
        {
            // 함수 이름으로만 사용하는 경우 -> StartCoroutine("TwinkleLoop");
            // 입력 값이 있는 경우           -> yield return StartCoroutine(FadeEffect(1,0));
            // Alpha 값을 1에서 0으로 : Fade Out
            yield return StartCoroutine(FadeEffect(1,0));
            // Alpha 값을 0에서 1로   : Fade In
            yield return StartCoroutine(FadeEffect(0,1));
        }
    }

    private IEnumerator FadeEffect(float start, float end)
    {
        float currentTime = 0.0f;
        float percent     = 0.0f;

        while (percent < 1) // 0.1초만에 percnet가 0에서 1이 됨.
        {

           // fadeTime 시간동안 while() 반목문 실행
            currentTime += Time.deltaTime;         // 실제 시간 초 중첩
            percent     =  currentTime / fadeTime; // 0에서 1까지 올라감.

            // 유니티의 클래스에 설정되어 있는 spriteRenderer.color, transform.position은 프로퍼티로
            // spriteRenderer.coler.a = 1.0f과 같이 설정이 불가능하다
            // spriteRenderer.coler = new Color(spriterRenderer.color.r, 숫자, 숫자, 1.0f);과 같이 설정해야 한다.
            Color color = spriteRenderer.color;
            // built-in method
            // float result = Mathf.Lerp(start,end,percent);
            // start와 end 사이의 값 중 percent 위치에 있는 값을 반환한다.
            // ex) start가 0, end가 100일 때 percent가 0.3이면 30을  반환한다.
            // 1~0 범위에서 percent는 0~1까지 이동하고
            // 0~1 범위에서 percent는 0~1까지 이동한다.
            // 이것이 왔다갔다 하면서, 깜빡이는 것 처럼 보이는 것!
            color.a = Mathf.Lerp(start, end, percent);
            spriteRenderer.color = color;
            
            yield return null;
        }
    }
}