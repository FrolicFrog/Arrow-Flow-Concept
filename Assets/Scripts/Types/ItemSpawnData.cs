using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowFlowGame.Types
{
    [Serializable]
    public class ItemSpawnData : IEnumerable<ItemsRow>
    {
        public List<ItemsRow> Rows;
        public int RowsCount => Rows.Count;
        public void AddRow(ItemsRow row = null) => Rows.Add(row ?? new ItemsRow());
        public void InsertRow(int idx, ItemsRow row = null) => Rows.Insert(idx, row ?? new ItemsRow());

        public ItemSpawnData()
        {
            Rows = new List<ItemsRow>();
        }

        public ItemsRow this[int item]
        {
            get => Rows[item];
            set => Rows[item] = value;
        }

        public IEnumerator<ItemsRow> GetEnumerator()
        {
            return Rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void RemoveAt(int rowIdx)
        {
            Rows.RemoveAt(rowIdx);
        }

        public void Clear() => Rows.Clear();
    }

    [Serializable]
    public class ItemsRow : IEnumerable<ItemData>
    {
        [SerializeReference]
        public List<ItemData> items;

        public ItemData this[int i] => items[i];

        public ItemsRow()
        {
            items = new List<ItemData>();
        }

        public int Count => items.Count;

        public void Add(ItemData item)
        {
            items.Add(item);
        }
        public void Remove(ItemData item)
        {
            items.Remove(item);
        }
        public IEnumerator<ItemData> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public ItemData Pop()
        {
            var item = items[0];
            items.RemoveAt(0);
            return item;
        }

        public bool TryPop(out ItemData item)
        {
            if (items.Count == 0)
            {
                item = default;
                return false;
            }

            item = Pop();
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void RemoveAt(int i)
        {
            items.RemoveAt(i);
        }
    }
}