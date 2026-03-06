using UnityEngine;
using ArrowFlowGame.Types;
using System.Collections.Generic;

public class ReferenceManager : Singleton<ReferenceManager>
{
    public ItemMaterials ItemMats;
    public ItemMaterials ArrowBodyMats;
    public Dictionary<Vector2Int, Item> KeyIdToLockedItem = new();
}