using System;
using UnityEngine;

public class AlterLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public  float        fadeSpeed;           // 사라지는 속도
    public  LayerMask    groundLayer;         // 오직 충돌할 레이어   
    
    // ToPlayer
    public bool toPlayerBool;
    private Transform target;                
    private Vector2 direction;
    private RaycastHit2D ray;
    private Vector2    endPosition;

    // ToAngle
    // public bool toAngleBool;
    // [HideInInspector] 
    // public GameObject targett;
    // [HideInInspector]
    // public float randomNumX;
    

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        // Toplayer
        if (toPlayerBool)
        {
            target = PlayerController.instance.gameObject.transform;
            direction = target.transform.position - transform.position;
            ray = Physics2D.Raycast(transform.position, direction, 100, groundLayer);
            endPosition = new Vector2(ray.point.x, ray.point.y);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, endPosition);

        }
        
        // if (toAngleBool)
        // {
        //     direction = targett.transform.position - transform.position;
        //     // randomNumX = Random.Range(-3f, 3f);
        //     // //var randomNumY = Random.Range(-1f, 1f);
        //     ray = Physics2D.Raycast(transform.position,  direction, 10, groundLayer);
        //     //Quaternion.Euler(0, 0, startAngleValue + intervalAngle * i)
        //     endPosition = new Vector2(ray.point.x, ray.point.y);
        //     lineRenderer.SetPosition(0, transform.position);
        //     lineRenderer.SetPosition(1, endPosition);
        // }
    }

    private void Update()
    {
        lineRenderer.widthMultiplier -= fadeSpeed * Time.deltaTime;
        if (lineRenderer.widthMultiplier < 0f)
        {
            Destroy(gameObject);
        }
    }
}
