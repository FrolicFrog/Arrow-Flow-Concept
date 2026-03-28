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
        sequence.Join(transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(gameObject);

            if(Row.FrontItem == this)
                Row.MoveToNext();
        }));
    }
}

/*
MissingReferenceException: The object of type 'Lock' has been destroyed but you are still trying to access it.
Your script should either check if it is null or you should not destroy the object.
Spawner.OnClick () (at Assets/Scripts/Spawner.cs:169)
ClickDetector.Update () (at Assets/Scripts/ClickDetector.cs:20)
*/