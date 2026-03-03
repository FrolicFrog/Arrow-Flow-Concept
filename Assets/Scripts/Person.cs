using System;
using UnityEngine;

public class Person : CrowdElement
{
    public Animator Anim;
    public bool IsWalking
    {
        set
        {
            Anim.SetBool("IsWalking", value);
        }
    }

    public bool AlreadyTarget {get; set;}
}