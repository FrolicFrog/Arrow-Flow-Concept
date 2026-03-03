using System;

namespace ArrowFlowGame.Types
{
    [Serializable]
    public class ItemData
    {
        public string Id;
        public ItemType Type;
        public int SpawnCount;
        public bool IsLocked;
        public string ConnectedTo;
        
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