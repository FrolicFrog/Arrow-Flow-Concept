using System;
using UnityEngine;
using ArrowFlowGame.Types;
using System.Collections.Generic;

public class LevelManager : Singleton<LevelManager>
{
    [HideInInspector] public int TestLevelToLoad = 1;

    [Header("REFERENCES")]
    [SerializeField] private Item ItemPrefab;
    [SerializeField] private CrowdElement CrowdElementPrefab;

    [Header("SETTINGS")]
    [Space(10)]

    [Header("Grid Settings")]
    public Vector2 GridSpacing;
    public Transform GridPos;

    private LevelData _LevelData;
    private int _CurrentLevelNumber;
    private VisualRows[] RowsTransform;

    private void Start()
    {
#if !UNITY_EDITOR
        Manager.Instance.TestMode = false;   
#endif

        int CurrentLevel = Manager.Instance.TestMode ? TestLevelToLoad : PlayerPrefs.GetInt("LastLevel", 1);
        _LevelData = Resources.Load<LevelData>($"Levels/{CurrentLevel}");
        _CurrentLevelNumber = CurrentLevel;

        if ((_LevelData == null || _CurrentLevelNumber > Constants.LAST_LEVEL) && !Manager.Instance.TestMode)
        {
            int LevelNumber = Utilities.GetRandomFakeLvl();
            _LevelData = Resources.Load<LevelData>($"Levels/{LevelNumber}");
        }

        SpawnItems();
        SpawnCrowdElements();
    }

    private void SpawnCrowdElements()
    {
        CrowdSpawnData CrowData = _LevelData.CrowdData;
        Vector2Int HalfSize = new(CrowData.Width / 2, CrowData.Height / 2);
        List<List<CrowdElement>> CrowdGrid = new();
        
        for (int i = 0; i < CrowData.Width; i++)
        {
            CrowdGrid.Add(new List<CrowdElement>());
            for (int j = 0; j < CrowData.Height; j++)
            {
                int flippedJ = CrowData.Height - 1 - j;

                Vector3Int GridIdx = new(i - HalfSize.x, flippedJ - HalfSize.y, 0);
                Vector3 Pos = GridManager.Instance.GetPosition(GridIdx);

                CrowdElement CrowdEle = Instantiate(CrowdElementPrefab, Pos, Quaternion.identity);
                CrowdEle.name = CrowData.CrowdGrid[i, j].Type.ToString();
                CrowdEle.Init(CrowData.CrowdGrid[i, j]);
                CrowdEle.GridPos = new Vector2Int(i, j);
                CrowdManager.Instance.RegisterElement(new Vector2Int(i, j), CrowdEle);
                CrowdGrid[i].Add(CrowdEle);
            }
        }

        CrowdManager.Instance.RegisterGrid(CrowdGrid);
    }
    
    private void SpawnItems()
    {
        ItemSpawnData SpawnData = _LevelData.ItemsData;
        int RowCount = SpawnData.RowsCount;

        RowsTransform = new VisualRows[RowCount];

        float TotalWidth = (RowCount - 1) * GridSpacing.x;
        float StartOffset = -TotalWidth / 2f;

        for (int i = 0; i < RowCount; i++)
        {
            Transform RowParent = new GameObject($"Row {i}").transform;
            ItemsRow Items = SpawnData.Rows[i];

            int MaxMovementTimes = Items.Count - 1;

            RowsTransform[i] = new VisualRows(RowParent, GridSpacing, MaxMovementTimes);

            for (int j = 0; j < Items.Count; j++)
            {
                Vector3 offset = new(StartOffset + GridSpacing.x * i, 0, -GridSpacing.y * j);
                Item ItemClone = Instantiate(ItemPrefab, GridPos.position + offset, Quaternion.identity, RowParent);
                ItemClone.Init(Items[j], RowsTransform[i]);

                RowsTransform[i].Add(ItemClone);
            }
        }
    }
}
