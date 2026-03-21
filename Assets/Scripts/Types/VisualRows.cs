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
            .OnComplete(() => _itemsInRows[0].OnMoveForward());
        }

        public int IndexOf(Item item)
        {
            return _itemsInRows.IndexOf(item);
        }

        public void Dequeue()
        {
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
    }
}