using System;
using ArrowFlowGame.Types;
using DG.Tweening;
using UnityEngine;

public class Arrow : Spawnable
{
    [Header("References")]
    public Renderer[] Renderers;
    public float ArrowSpeed = 50f;

    private Transform Target;
    private ItemType ArrowType;
    private Action OnReachTarget;

    public override void Init(ItemType type, Transform target, Action OnReachTarget = null)
    {
        foreach(Renderer r in Renderers)
            r.material = ReferenceManager.Instance.ItemMats.GetMaterial(type);

        ArrowType = type;
        Target = target;
        this.OnReachTarget = OnReachTarget;
    }

    public override void OnSpawn()
    {
    }

    private void Update()
    {
        if(Target == null) return;
        transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, Time.deltaTime * ArrowSpeed);
        transform.LookAt(Target);

        if(Vector3.Distance(transform.position, Target.transform.position) < 0.1f)
        {
            OnReachTarget?.Invoke();
        }
    }
}
