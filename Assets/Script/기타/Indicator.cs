using UnityEngine;

public class Indicator : MonoBehaviour
{
    public  GameObject indicator;             // 워링 표시   
    private GameObject target;                // 플레이어
    public  LayerMask  camBoxLayer;           // 오직 충돌할 레이어   
    private SpriteRenderer bodySR;

    private void Start()
    {
        bodySR     = GetComponent<SpriteRenderer>();
        target = PlayerController.instance.gameObject;
    }

    private void Update()
    {
        // 화면 밖이라, 미사일이 안보이면 위험표시 키기
        if (bodySR.isVisible == false)
        {
            if (indicator.activeInHierarchy == false)
            {
                indicator.SetActive(true);
            }

            Vector2 direction = target.transform.position - transform.position;
            RaycastHit2D ray  = Physics2D.Raycast (transform.position, direction,10,camBoxLayer);

            if (ray == true)
            {
                indicator.transform.position = new Vector2(ray.point.x,ray.point.y);
            }

        }
        // 화면 안이라, 미사일이 보이면 위험표시 끄기
        else if (bodySR.isVisible)
        {
            if (indicator.activeInHierarchy)
            {
                indicator.SetActive(false);
            }
        }
    }
}
