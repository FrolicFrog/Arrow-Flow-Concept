using ArrowFlowGame.Types;
using UnityEngine;

public class Lock : Item
{
    private Vector2Int KeyId;

    public override void Init(ItemData data, VisualRows Row)
    {
        base.Init(data, Row);
        if(data is not LockItemData lockData) return;
        KeyId = lockData.KeyId;
    }
    public void Unlock()
    {
        Debug.Log("Lock Unlocked", gameObject);
        OnComplete();
    }
}