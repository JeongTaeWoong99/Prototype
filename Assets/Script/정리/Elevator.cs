using Unity.VisualScripting;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private bool onElevator;
    public float maxHeight;
    public float lowHeight;

    private void Update()
    {
        if (onElevator)
        {
            if(transform.position.y < maxHeight)
                transform.position += new Vector3(0f,2f,0f) * Time.deltaTime;
        }
        else
        {
            Vector2.MoveTowards(transform.position, new Vector2(transform.position.x,lowHeight),Time.deltaTime * -1f);
        }
        // else if(onElevator == false)
        // {
        //     if (transform.position.y > lowHeight)
        //     {
        //         transform.position += Vector3.down * Time.deltaTime;
        //     }
        // }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            onElevator = true;
        }
    }
    
    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            onElevator = false;
        }
    }
}
