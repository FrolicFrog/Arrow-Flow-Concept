using System;
using ArrowFlowGame.Types;
using DG.Tweening;
using UnityEngine;

public class Lock : Item
{
    private bool HasKey;
    private Vector2Int KeyId;

    public override void Init(ItemData data, VisualRows Row, Action<Item> OnItemUsed)
    {
        base.Init(data, Row, OnItemUsed);
        if(data is not LockItemData lockData) return;
        HasKey = lockData.HasKey;
        KeyId = lockData.KeyId;
    }
    public void Unlock()
    {
        Debug.Log("Lock Unlocked", gameObject);
        OnComplete();
    }

    protected override void OnComplete()
    {
        Row.Dequeue();
        OnItemUsed?.Invoke(this);
        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject)));
    }
}