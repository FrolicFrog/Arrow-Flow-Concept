using System;
using UnityEngine;

namespace ArrowFlowGame.Types
{
    [Serializable]
    public class ItemData
    {
        public string Id;
        public ItemData(string Id)
        {
            this.Id = Id;
        }
    }

    [Serializable]
    public class SpawnItemData : ItemData
    {
        public ItemType Type;
        public int SpawnCount;
        public bool IsMysterious = false;
        public Vector2Int ConnectedTo;
        
        public SpawnItemData(string Id) : base(Id)
        {
            
        }
    }

    [Serializable]
    public class LockItemData : ItemData
    {
        public Vector2Int KeyId;
        
        public LockItemData(string Id) : base(Id)
        {

        }
    }
}