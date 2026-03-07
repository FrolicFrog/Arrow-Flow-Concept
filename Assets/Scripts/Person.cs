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
        SwitchMaterial(ReferenceManager.Instance.DeadPersonMaterial);
        transform.DOMoveY(transform.position.y + YAnimOffset, 0.7f);
        transform.DOScaleY(0, 0.3f).SetDelay(0.4f).OnComplete(() => Destroy(gameObject));
    }

    private void SwitchMaterial(Material TargetMaterial)
    {
        Array.ForEach(Renderers, R => R.material = TargetMaterial);
    }
}