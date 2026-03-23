using System;
using ArrowFlow.Types; // Assuming Array2D is here
using UnityEngine;     // For Mathf

namespace ArrowFlowGame.Types
{
    [Serializable]
    public class CrowdSpawnData
    {
        public Array2D<CrowdElementData> CrowdGrid;
        public int Width => CrowdGrid.Width;
        public int Height => CrowdGrid.Height;

        public CrowdSpawnData(int width, int height)
        {
            InitializeGrid(width, height);
        }

        private void InitializeGrid(int width, int height)
        {
            CrowdGrid = new Array2D<CrowdElementData>(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    CrowdGrid[x, y] = new CrowdElementData();
                }
            }
        }

        public void Resize(int newWidth, int newHeight, int insertAt = 0)
        {
            Array2D<CrowdElementData> oldGrid = CrowdGrid;
            InitializeGrid(newWidth, newHeight);

            int minWidth = Mathf.Min(oldGrid.Width, newWidth);
            int minHeight = Mathf.Min(oldGrid.Height, newHeight);

            for (int y = 0; y < minHeight; y++)
            {
                for (int x = 0; x < minWidth; x++)
                {
                    CrowdGrid[x, y + insertAt] = oldGrid[x, y];
                }
            }
        }

        public CrowdElementData this[int row, int column]
        {
            get => CrowdGrid[column, row];
            set
            {
                CrowdGrid[column, row] = value;
            }
        }
    }

    [Serializable]
    public class CrowdElementData
    {
        public ItemType Type = ItemType.RED;
        public Vector2Int GridPosition;
        public int RequiredHits = 1;
        public bool IsKeyed = false;
        public bool IsGiant = false;
    }
}