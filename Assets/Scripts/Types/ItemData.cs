using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowFlowGame.Types
{
    [Serializable]
    public class ItemData
    {
        public Vector2Int Id;
        public ItemData(Vector2Int Id)
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
        public bool HasConnection;
        public List<Vector2Int> ConnectedTo = new List<Vector2Int>();
        
        public SpawnItemData(Vector2Int Id) : base(Id)
        {
            
        }
    }

    [Serializable]
    public class LockItemData : ItemData
    {
        public bool HasKey;
        public Vector2Int KeyId;
        
        public LockItemData(Vector2Int Id) : base(Id)
        {

        }
    }
}