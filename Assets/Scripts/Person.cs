using System;
using DG.Tweening;
using UnityEngine;
using ArrowFlowGame.Types;

public class Person : CrowdElement
{
    public string DebugTxt;
    public MeshVariant[] MeshVariants;
    public Key KeyObj;
    public ParticleSystem DamageEffect;
    
    [Header("Animation Settings")]
    public float MinAnimSpeed = 0.8f;
    public float MaxAnimSpeed = 1.5f;
    [Range(0.1f, 10f)] public float YAnimOffset;
    
    public Animator Anim { get; private set; }

    private bool _isWalking;
    public bool IsWalking
    {
        get => _isWalking;
        set
        {
            _isWalking = value;
            if (Anim != null)
            {
                Anim.SetBool("IsWalking", _isWalking);
            }
        }
    }

    public bool AlreadyTarget { get; set; }
    public int RequiredHits = 1;
    
    private bool IsInFrontRow => CrowdManager.Instance.CurFront.Contains(this);
    private int RowIdx => CrowdManager.Instance.GetElementRowIdx(this);
    
    private int _lastRowIdx = -1;
    private float _randomAnimSpeed;

    protected override void Awake()
    {
        base.Awake();
        _randomAnimSpeed = UnityEngine.Random.Range(MinAnimSpeed, MaxAnimSpeed);
    }

    private void Start()
    {
        KeyObj.SetActive(IsKeyed);
    }

    private void Update()
    {
        int currentRow = RowIdx;
        DebugTxt = currentRow.ToString();
        
        if (_lastRowIdx != currentRow)
        {
            SwitchMeshVariant(currentRow);
        }
    }

    private void SwitchMeshVariant(int targetRow)
    {
        _lastRowIdx = targetRow;
        int variantIndex = Mathf.Clamp(targetRow, 0, MeshVariants.Length - 1);
        
        for (int i = 0; i < MeshVariants.Length; i++)
        {
            bool isTargetVariant = (i == variantIndex);
            MeshVariants[i].VariantObject.SetActive(isTargetVariant);
            
            if (isTargetVariant)
            {
                Anim = MeshVariants[i].VariantAnimator;
                Anim.speed = _randomAnimSpeed;
                Anim.SetBool("IsWalking", _isWalking);
            }
        }
    }

    public override void Init(CrowdElementData crowdElement)
    {
        base.Init(crowdElement);
        RequiredHits = crowdElement.RequiredHits;
    }

    public virtual void Damage()
    {
        RequiredHits = Mathf.Max(RequiredHits - 1, 0);
        if (RequiredHits <= 0)
        {
            Dead();
        }
    }

    protected virtual void Dead()
    {
        if (IsKeyed && ReferenceManager.Instance.KeyIdToLockedItem.TryGetValue(GridIdxId, out Lock lockObj))
        {
            KeyObj.transform.SetParent(null);
            KeyObj.Unlock(lockObj);
        }

        if (Anim != null) Anim.Play("Death");
        
        Sequence deathSequence = DOTween.Sequence();
        deathSequence.AppendCallback(() => SwitchMaterial(ReferenceManager.Instance.DamageFlashedPerson));
        deathSequence.AppendCallback(() => DamageEffect.Play());
        deathSequence.Join(transform.DOMoveY(transform.position.y + YAnimOffset, 0.5f).SetLoops(2, LoopType.Yoyo));
        deathSequence.InsertCallback(0.25f, () => SwitchMaterial(ReferenceManager.Instance.DeadPersonMaterial));
        deathSequence.Join(transform.DOScaleY(0, 0.3f).SetDelay(1f))
            .OnComplete(() => Destroy(gameObject));
    }

    protected virtual void SwitchMaterial(Material targetMaterial)
    {
        if (Renderers != null)
        {
            Array.ForEach(Renderers, r => r.material = targetMaterial);
        }
    }
}