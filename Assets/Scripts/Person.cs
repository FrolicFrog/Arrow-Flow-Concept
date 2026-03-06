using System;
using DG.Tweening;
using UnityEngine;

public class Person : CrowdElement
{
    public Animator Anim;
    [Range(0.1f, 10f)]public float YAnimOffset;
    public bool IsWalking
    {
        set
        {
            Anim.SetBool("IsWalking", value);
        }
    }

    public bool AlreadyTarget {get; set;}

    public void Dead()
    {
        Anim.Play("Death");
        transform.DOMoveY(transform.position.y + YAnimOffset, 0.7f);
        Destroy(gameObject, 1f);
    }
}