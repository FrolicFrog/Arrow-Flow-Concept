using UnityEngine;
using ArrowFlowGame.Types;
using System.Collections.Generic;
using System;

public class ReferenceManager : Singleton<ReferenceManager>
{
    public ItemMaterials ItemMats;
    public ItemMaterials ArrowBodyMats;
    public Material DeadPersonMaterial;
    public Dictionary<Vector2Int, Lock> KeyIdToLockedItem = new();

    public void RegisterLock(Lock lockClone, LockItemData data)
    {
        KeyIdToLockedItem.Add(data.KeyId, lockClone);
    }
}