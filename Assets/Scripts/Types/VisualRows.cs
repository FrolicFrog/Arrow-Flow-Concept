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

        public void MoveToNext()
        {
            if(_completedMovements >= MAX_MOVEMENTS) return;

            if(rowsTransform == null)
            {
                Debug.LogWarning("No Rows Transform Available to Animate");
                return;
            }

            _completedMovements++;
            rowsTransform.DOMoveZ(rowsTransform.transform.position.z + _gridSpacing.y, 0.5f)
            .OnComplete(() => {
                // Saftey check added in case the row is empty when the tween finishes
                if (_itemsInRows.Count > 0 && _itemsInRows[0] != null)
                    _itemsInRows[0].OnMoveForward();
            });
        }

        // Target and remove specific item (fixes the MissingReferenceException)
        public void Remove(Item item)
        {
            if (_itemsInRows.Contains(item))
            {
                _itemsInRows.Remove(item);
                item.transform.SetParent(null);
            }
        }

        // Shifts only the items starting from the gap, leaving items in front of the gap untouched
        public void ShiftItemsForward(int startIndex)
        {
            for (int i = startIndex; i < _itemsInRows.Count; i++)
            {
                Item item = _itemsInRows[i];
                if (item != null)
                {
                    Item currentItem = item; // Capture for lambda
                    currentItem.transform.DOMoveZ(currentItem.transform.position.z + _gridSpacing.y, 0.5f)
                        .OnComplete(() => 
                        {
                            if (_itemsInRows.Count > 0 && _itemsInRows[0] == currentItem)
                            {
                                currentItem.OnMoveForward();
                            }
                        });
                }
            }
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
    }
}