using System;
using DG.Tweening;
using UnityEngine;
using ArrowFlowGame.Types;

public class Person : CrowdElement
{
    public Mesh LowerPolyMesh;
    public Mesh HigherPolyMesh; 
    public SkinnedMeshRenderer Renderer;
    public ParticleSystem DamageEffect;
    public Animator Anim;
    public float MinAnimSpeed = 0.8f;
    public float MaxAnimSpeed = 1.5f;
    [Range(0.1f, 10f)] public float YAnimOffset;
    public bool IsWalking
    {
        get
        {
            return Anim.GetBool("IsWalking");
        }
        set
        {
            Anim.SetBool("IsWalking", value);
            if(!value && IsInFrontRow)
            {
                Renderer.sharedMesh = HigherPolyMesh;
            }
        }
    }

    public bool AlreadyTarget {get; set;}
    public int RequiredHits = 1;
    private bool IsInFrontRow => CrowdManager.Instance.CurFront.Contains(this);

    protected override void Awake()
    {
        base.Awake();
        Anim.speed = UnityEngine.Random.Range(MinAnimSpeed, MaxAnimSpeed);
        Renderer.sharedMesh = LowerPolyMesh;
    }

    private void Start()
    {
        Renderer.sharedMesh = IsInFrontRow ? HigherPolyMesh : LowerPolyMesh;
    }

    public override void Init(CrowdElementData crowdElement)
    {
        base.Init(crowdElement);
        RequiredHits = crowdElement.RequiredHits;
    }

    public virtual void Damage()
    {
        RequiredHits = Mathf.Max(RequiredHits - 1, 0);
        if(RequiredHits <= 0)
        Dead();
    }

    protected virtual void Dead()
    {
        Anim.Play("Death");
        Sequence DeathSequence = DOTween.Sequence();
        DeathSequence.AppendCallback(() => SwitchMaterial(ReferenceManager.Instance.DamageFlashedPerson));
        DeathSequence.AppendCallback(() => DamageEffect.Play());
        DeathSequence.Join(transform.DOMoveY(transform.position.y + YAnimOffset, 0.7f));
        DeathSequence.InsertCallback(0.25f, () => SwitchMaterial(ReferenceManager.Instance.DeadPersonMaterial));
        DeathSequence.Join(transform.DOScaleY(0, 0.3f).SetDelay(0.4f)).OnComplete(() => Destroy(gameObject));
    }
    protected virtual void SwitchMaterial(Material TargetMaterial)
    {
        Array.ForEach(Renderers, R => R.material = TargetMaterial);
    }
}