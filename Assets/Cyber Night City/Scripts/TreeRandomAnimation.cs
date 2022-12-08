using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRandomAnimation : MonoBehaviour
{
    void Start()
    {
        var anim = GetComponent<Animator>();
        var state = anim.GetCurrentAnimatorStateInfo(0);
        anim.Play(state.fullPathHash, 0, Random.Range(0, 1));
    }

    
}
