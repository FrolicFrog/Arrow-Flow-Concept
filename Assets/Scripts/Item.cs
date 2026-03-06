using UnityEngine;
using ArrowFlowGame.Types;

public class Item : MonoBehaviour
{
    protected VisualRows Row;
    protected string Id;

    public virtual void Init(ItemData data, VisualRows Row)
    {
        Id = data.Id;
        this.Row = Row;
    }

    protected virtual void OnComplete()
    {
        Row.Dequeue();
        Row.MoveToNext();
        Destroy(gameObject);
    }
}