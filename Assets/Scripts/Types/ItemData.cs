using System;
using UnityEngine;

namespace ArrowFlowGame.Types
{
    [Serializable]
    public class ItemData
    {
        public string Id;
        public ItemType Type;
        public int SpawnCount;
        public Vector2Int ConnectedTo;
        
        public ItemData(string Id)
        {
            this.Id = Id;
        }

        public override string ToString()
        {
            return $"{Type}-{Id} | {SpawnCount}";
        }
    }
}