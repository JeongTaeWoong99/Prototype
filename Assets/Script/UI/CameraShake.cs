using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    
    public float   force;			
    public Vector3 offset;
    
    private Quaternion originRot;

    [HideInInspector] 
    public bool shakeCoroutineState; 
    
    private void Awake()
    {
        instance = this;
    }
    
    private void Start()
    {
        originRot = transform.rotation;	// 기본값
    }

    private void Update()
    {
        if (shakeCoroutineState == true)
        {
            StartCoroutine(ShakeCoroutine());
        }
        else if(shakeCoroutineState == false)
        {
            StopAllCoroutines();
            StartCoroutine(Reset());
        }
    }


    public IEnumerator ShakeCoroutine()
    {
        Vector3 originEuler = transform.eulerAngles;
        while (true)
        {
            float rotX = Random.Range(-offset.x, offset.x);
            float rotY = Random.Range(-offset.y, offset.y);
            float rotZ = Random.Range(-offset.z, offset.z);

            Vector3    randomRot = originEuler + new Vector3(rotX, rotY, rotZ);
            Quaternion rot       = Quaternion.Euler(randomRot);

            while (Quaternion.Angle(transform.rotation, rot) > 0.1f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation,rot,force * Time.deltaTime);
                yield return null;
            }
            yield return null;
        }
    }

    public IEnumerator Reset()
    {
        while (Quaternion.Angle(transform.rotation, originRot) > 0f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, originRot, force * Time.deltaTime);
            yield return null;
        }
    }
}
