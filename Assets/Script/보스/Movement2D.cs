using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private Vector3 moveDirection = Vector3.zero; // Inspector view에서 0 0 0 으로 보이고, 수정 가능

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
    //                               ↑
    //                               ↑
    // 외부에서 이동방향을 설정할 수 있도록, 설정하는 함수
    // PlayerController.cs 에서 방향키를 이용하여 방향(direction)을 바꿔준다
    public void MoveTo(Vector3 direction)
    {                             // ↑
        moveDirection = direction;// ↑
    }
}