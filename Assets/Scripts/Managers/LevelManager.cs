using UnityEngine;
using ArrowFlowGame.Types;
using System.Collections.Generic;
using ArrowFlow.Types;
using System;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    [HideInInspector] public int TestLevelToLoad = 1;

    [Header("REFERENCES")]
    [SerializeField] private Spawner SpawnItemPrefab;
    [SerializeField] private Lock LockItemPrefab;
    [SerializeField] private CrowdElement CrowdElementPrefab;
    [SerializeField] private CrowdElement GiantPersonPrefab;

    [Header("SETTINGS")]
    [Space(10)]

    [Header("Grid Settings")]
    public Vector2 GridSpacing;
    public Transform GridPos;

    // Internal Private Variables....
    private LevelData _LevelData;
    public LevelData LevelData => _LevelData;
    private int _CurrentLevelNumber;
    public int CurrentLevelNumber => _CurrentLevelNumber;
    private VisualRows[] RowsTransform;
    public VisualRows[] Rows => RowsTransform;
    public event Action<Item> OnItemUsed;

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
        
        PowerupManager.Instance.Initialize();
        BeltManager.Instance.Initialize();
        UIManager.Instance.Initialize();
        GameManager.Instance.Initialize();
    }

    private void SpawnCrowdElements()
    {
        CrowdSpawnData CrowdData = _LevelData.CrowdData;
        Vector2Int HalfSize = new(CrowdData.Width / 2, 0);
        List<List<CrowdElement>> CrowdGrid = new();
        
        for (int i = 0; i < CrowdData.Width; i++)
        {
            CrowdGrid.Add(new List<CrowdElement>());
            for (int j = 0; j < CrowdData.Height; j++)
            {
                if(CrowdData.CrowdGrid[i, j].Type == ItemType.NONE) continue;
                
                int flippedJ = CrowdData.Height - 1 - j;

                Vector3Int GridIdx = new(i - HalfSize.x, flippedJ - HalfSize.y, 0);
                Vector3 Pos = GridManager.Instance.GetPosition(GridIdx);
                
                if (CrowdData.Width % 2 == 0)
                {
                    Pos.x += GridManager.Instance.GridSystem.cellSize.x / 2f;
                }

                bool IsGiant = CrowdData.CrowdGrid[i, j].IsGiant;
                CrowdElement PersonPrefab = IsGiant ? GiantPersonPrefab : CrowdElementPrefab;

                CrowdElement CrowdEle = Instantiate(PersonPrefab, Pos, Quaternion.identity);
                CrowdEle.name = CrowdData.CrowdGrid[i, j].Type.ToString();
                CrowdEle.Init(CrowdData.CrowdGrid[i, j]);
                CrowdEle.GridPos = new Vector2Int(i, j);
                CrowdEle.OriginalGridPos = new Vector2Int(i, j);
                CrowdEle.GridIdxId = new Vector2Int(j, i);
                CrowdManager.Instance.RegisterElement(new Vector2Int(i, j), CrowdEle);
                CrowdGrid[i].Add(CrowdEle);
            }
        }
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
                ItemData data = Items[j];
                if(data is SpawnItemData SpawnerData)
                {
                    Spawner SpawnerClone = Instantiate(SpawnItemPrefab, GridPos.position + offset, Quaternion.identity, RowParent);
                    SpawnerClone.Init(Items[j], RowsTransform[i], OnItemUsed);
                    RowsTransform[i].Add(SpawnerClone);
                    ReferenceManager.Instance.RegisterSpawner(SpawnerClone, SpawnerData);
                }
                else if(data is LockItemData LockData)
                {
                    Lock LockClone = Instantiate(LockItemPrefab, GridPos.position + offset, Quaternion.identity, RowParent);
                    LockClone.Init(Items[j], RowsTransform[i], OnItemUsed);
                    RowsTransform[i].Add(LockClone);
                    ReferenceManager.Instance.RegisterLock(LockClone, LockData);
                }
                else
                {
                    Debug.Log("Item type not supported: " + data.GetType().Name.ToString());
                }
            }
        }
    }

    public void NextLevel()
    {
        PlayerPrefs.SetInt("LastLevel", _CurrentLevelNumber + 1);
        ReloadLevel();
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
