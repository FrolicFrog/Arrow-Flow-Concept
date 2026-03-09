using UnityEngine;
using ArrowFlowGame.Types;
using System;

public class Item : MonoBehaviour
{
    protected VisualRows Row;
    protected string Id;
    protected Action<Item> OnItemUsed = null;

    public virtual void Init(ItemData data, VisualRows Row, Action<Item> OnItemUsed)
    {
        Id = data.Id;
        this.Row = Row;
        this.OnItemUsed = OnItemUsed;
    }

    protected virtual void OnComplete()
    {
        Row.Dequeue();
        Row.MoveToNext();
        OnItemUsed?.Invoke(this);
        Destroy(gameObject);
    }
}