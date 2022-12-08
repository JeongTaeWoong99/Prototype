using System;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    [HideInInspector]
    public bool playerCheck;
    [HideInInspector] 
    public PlatformEffector2D platformObject;
    [HideInInspector]
    public float count;

    void Start()
    {
        playerCheck = false;
        platformObject = GetComponent<PlatformEffector2D>();
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if (playerCheck)
        {
            if (coll.gameObject.tag == "Player")
            {
                platformObject.rotationalOffset = 0f; // 값변경
                platformObject.gameObject.layer = 8; // 레이어변경
                Debug.Log("탈출");
            }
        }
        
    }
    
}