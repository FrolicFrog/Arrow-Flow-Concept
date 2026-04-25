using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ArrowFlowGame.Types
{
    public class VisualRows
    {
        public Transform rowsTransform;
        public Item FrontItem => _itemsInRows.Count > 0 ? _itemsInRows[0] : null;
        public int Count => _itemsInRows.Count;

        private Vector2 _gridSpacing;
        private int _completedMovements = 0;
        private List<Item> _itemsInRows = new List<Item>();

        private readonly int MAX_MOVEMENTS;

        public VisualRows(Transform RowsTransform, Vector2 GridSpacing, int MaxMovement)
        {
            rowsTransform = RowsTransform;    
            _gridSpacing = GridSpacing;
            MAX_MOVEMENTS = MaxMovement;
        }
        
        public void Remove(Item item)
        {
            if (_itemsInRows.Contains(item))
            {
                _itemsInRows.Remove(item);
                item.transform.SetParent(null);
            }
        }

        public void ShiftItemsForward(int startIndex)
        {
            for (int i = startIndex; i < _itemsInRows.Count; i++)
            {
                Item item = _itemsInRows[i];
                if (item != null)
                {
                    Item currentItem = item;
                    var Path = ParabolicPath(currentItem.transform.position, currentItem.transform.position + Vector3.forward * _gridSpacing.y, 6f, 10);
                    
                    currentItem.transform.DOPath(Path, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetDelay(0.1f * (i - startIndex))
                    .OnComplete(() => 
                    {
                        if (_itemsInRows.Count > 0 && _itemsInRows[0] == currentItem)
                            currentItem.OnMoveForward();
                    });
                }
            }
        }

        private Vector3[] ParabolicPath(Vector3 start, Vector3 end, float height, int pointCount)
        {
            Vector3[] path = new Vector3[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                float t = (float)i / (pointCount - 1);
                Vector3 point = Vector3.Lerp(start, end, t);
                point.y += Mathf.Sin(t * Mathf.PI) * height;

                path[i] = point;
            }

            return path;
        }

        public int IndexOf(Item item)
        {
            return _itemsInRows.IndexOf(item);
        }

        public void Dequeue()
        {
            if (_itemsInRows.Count == 0) return;
            Item FrontItem = _itemsInRows[0];
            _itemsInRows.RemoveAt(0);

            FrontItem.transform.SetParent(null);
        }

        public void Add(Item item)
        {
            _itemsInRows.Add(item);
        }

        public void SetItem(Spawner spawner1, int idx2)
        {
            _itemsInRows[idx2] = spawner1;
        }

        public List<Item> ToList() => new List<Item>(_itemsInRows);
        public Stack<Item> ToStack()
        {
            Stack<Item> stack = new Stack<Item>();
            
            for (int i = _itemsInRows.Count - 1; i >= 0; i--)
                stack.Push(_itemsInRows[i]);

            return stack;
        }

        public override string ToString()
        {
            string result = "";
            foreach (Item item in _itemsInRows)
            {
                if(item == null || item is not Spawner s) continue;
                
                result += s.Type.ToString() + " ";
            }
                

            return result;
        }
    }
}