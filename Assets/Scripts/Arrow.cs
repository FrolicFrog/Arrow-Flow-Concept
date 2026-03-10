using System;
using ArrowFlowGame.Types;
using DG.Tweening;
using UnityEngine;

public class Arrow : Spawnable
{
    [Header("References")]
    public Renderer[] Renderers;
    public TrailRenderer Trail;
    public float ArrowSpeed = 50f;

    private Transform Target;
    private ItemType ArrowType;
    private Action OnReachTarget;
    public override void OnSpawn() {}

    public override void Init(ItemType type, Transform target, Action OnReachTarget = null)
    {
        Material ArrowMat = ReferenceManager.Instance.ItemMats.GetMaterial(type);
        Material ArrowBodyMat = ReferenceManager.Instance.ArrowBodyMats.GetMaterial(type);
        Material[] MatArr = new Material[3];
        MatArr[0] = ArrowMat;
        MatArr[1] = ArrowBodyMat;
        MatArr[2] = ArrowMat;

        foreach(Renderer r in Renderers)
            r.materials = MatArr;

        Trail.Clear();
        Trail.material = ReferenceManager.Instance.ArrowTrailMats.GetMaterial(type);

        ArrowType = type;
        Target = target;
        this.OnReachTarget = OnReachTarget;
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
