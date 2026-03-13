using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ArrowFlowGame.Types;

public class LevelEditor : EditorWindow
{
    private bool IsItemsSectionExpanded = false;
    private bool IsLevelSectionExpanded = false;
    private bool IsCrowdSectionExpaneded = false;

    private LevelData CurLvlData => Resources.Load<LevelData>("Levels/" + CurLvlNum);
    private int CurLvlNum = 1;

    //Data
    private ItemSpawnData _ItemSpawnData;
    private CrowdSpawnData _CrowdSpawnData;

    //Styles
    private GUIStyle headerStyle;
    private GUIStyle boxStyle;
    private GUIStyle labelStyle;
    private GUIStyle expandableStyle;
    private GUIStyle buttonStyle;
    private GUIStyle counterButtonStyle;
    private Vector2 EditorScrollPos = Vector2.zero;

    //Toolbars
    private ToolbarOption<ItemType> ItemTypeToolbar = new ToolbarOption<ItemType>(ItemType.RED, "Arrow Type:", false);
    private ToolbarOption<Hits> HitsToolbar = new ToolbarOption<Hits>(Hits.ONE, "Hits:", true);
    private ToolbarOption<KEYED> KeyedToolbar = new ToolbarOption<KEYED>(KEYED.YES, "Is Keyed:", true);
    private ToolbarOption<GIANT> GiantToolbar = new ToolbarOption<GIANT>(GIANT.YES, "Is Giant:", true);



    [MenuItem("Arrow Flow/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor>("Level Editor");
    }

    private void OnEnable()
    {
        _ItemSpawnData ??= new ItemSpawnData();
        _CrowdSpawnData ??= new CrowdSpawnData(2, 2);
    }

    private void InitStyles()
    {
        headerStyle ??= new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(0, 0, 10, 10)
        };
        boxStyle ??= new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(10, 10, 10, 10),
            margin = new RectOffset(7, 7, 7, 7),
        };
        labelStyle ??= new GUIStyle(EditorStyles.label)
        {
            fontSize = 12,
            margin = new RectOffset(0, 0, 4, 4)
        };
        expandableStyle ??= new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            margin = new RectOffset(0, 0, 0, 0)
        };
        buttonStyle ??= new GUIStyle()
        {
            fontSize = 14,
            padding = new RectOffset(5, 5, 5, 5),
            normal = new GUIStyleState()
            {
                textColor = Color.white,
                background = Texture2D.whiteTexture
            },
            margin = new RectOffset(0, 0, 0, 0)
        };
        counterButtonStyle ??= new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0),
        };
    }
    void OnGUI()
    {
        EditorScrollPos = EditorGUILayout.BeginScrollView(EditorScrollPos);
        InitStyles();
        GUILayout.Label("Arrow Flow", headerStyle);
        LevelSettings();
        ItemsSettings();
        CrowdSettings();
        Actions();
        EditorGUILayout.EndScrollView();
    }

    private void CrowdSettings()
    {
        _CrowdSpawnData ??= new CrowdSpawnData(2, 2);

        GUILayout.BeginVertical(boxStyle);
        DropDownHeader(ref IsCrowdSectionExpaneded, "Crowd Settings");

        if (!IsCrowdSectionExpaneded)
        {
            GUILayout.EndVertical();
            return;
        }

        HorizontalLineAndSpace();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Row", GUILayout.Height(34)))
        {
            _CrowdSpawnData.Resize(_CrowdSpawnData.Width, _CrowdSpawnData.Height + 1);
        }
        if (GUILayout.Button("Remove Row", GUILayout.Height(34)))
        {
            _CrowdSpawnData.Resize(_CrowdSpawnData.Width, Mathf.Max(_CrowdSpawnData.Height - 1, 1));
        }
        if (GUILayout.Button("Add Col", GUILayout.Height(34)))
        {
            _CrowdSpawnData.Resize(_CrowdSpawnData.Width + 1, _CrowdSpawnData.Height);
        }
        if (GUILayout.Button("Remove Col", GUILayout.Height(34)))
        {
            _CrowdSpawnData.Resize(Mathf.Max(_CrowdSpawnData.Width - 1, 1), _CrowdSpawnData.Height);
        }
        if (GUILayout.Button("Clear", GUILayout.Height(34)))
        {
            _CrowdSpawnData = new CrowdSpawnData(2, 2);
        }
        GUILayout.EndHorizontal();

        ItemTypeToolbar.DrawToolbar();
        HitsToolbar.DrawToolbar();
        KeyedToolbar.DrawToolbar();
        GiantToolbar.DrawToolbar();
        GUILayout.Space(10);

        GUIStyle idStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 8,
            alignment = TextAnchor.LowerLeft
        };
        idStyle.normal.textColor = Color.white;

        for (int row = 0; row < _CrowdSpawnData.Height; row++)
        {
            GUILayout.BeginHorizontal();
            if (row % 2 != 0) EditorGUILayout.Space(30);
            GUILayout.FlexibleSpace();

            for (int col = 0; col < _CrowdSpawnData.Width; col++)
            {
                CrowdElementData element = _CrowdSpawnData[row, col];
                if (element == null) continue;

                Color oldColor = GUI.color;
                GUI.color = Utilities.GetColorByItemType(element.Type);

                string ButtonLabel = element.RequiredHits == 1 ? "" : ToolbarOption<Hits>.ConvertToString(element.RequiredHits);
                GUIContent buttonContent = new GUIContent(ButtonLabel, $"{row}-{col}"); // tooltip is 2nd param

                Rect rect = GUILayoutUtility.GetRect(buttonContent, GUI.skin.button, GUILayout.Width(30), GUILayout.Height(30));

                if (GUI.Button(rect, buttonContent))
                {
                    element.Type = ItemTypeToolbar.Value;
                    element.RequiredHits = (int)HitsToolbar.Value;
                    element.IsKeyed = KeyedToolbar.Value == KEYED.YES;
                    element.IsGiant = GiantToolbar.Value == GIANT.YES;
                }

                GUI.color = oldColor;

                if (element.IsKeyed)
                {
                    Rect dotRect = new Rect(rect.xMax - 8, rect.y + 2, 6, 6);
                    EditorGUI.DrawRect(dotRect, Color.yellow);
                }

                if (element.IsGiant)
                {
                    Rect dotRect = new Rect(rect.xMin, rect.y + 2, 6, 6);
                    EditorGUI.DrawRect(dotRect, Color.black);
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    private void Actions()
    {
        GUILayout.BeginHorizontal(boxStyle);

        if (GUILayout.Button("Load Level", GUILayout.Height(35)))
        {
            LoadLvl();
        }
        if (GUILayout.Button(CurLvlData == null ? "Create Level" : "Update Level", GUILayout.Height(35)))
        {
            UpdateLvl();
        }
        if (GUILayout.Button("Play Level", GUILayout.Height(35)))
        {
            LevelManager LM = FindAnyObjectByType<LevelManager>();
            if (LM == null)
            {
                Debug.LogWarning("No Level Manager Found in Scene");
            }
            else
            {
                LM.TestLevelToLoad = CurLvlNum;
                EditorUtility.SetDirty(LM);
                EditorApplication.isPlaying = true;
            }
        }
        GUILayout.EndHorizontal();
    }

    private void UpdateLvl()
    {
        if (CurLvlData == null)
        {
            LevelData TmpLvlData = CreateInstance<LevelData>();
            AssetDatabase.CreateAsset(TmpLvlData, $"Assets/Resources/Levels/{CurLvlNum}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // Ensure data is initialized before saving
        _ItemSpawnData ??= new ItemSpawnData();
        _CrowdSpawnData ??= new CrowdSpawnData(2, 2);

        CurLvlData.ItemsData = _ItemSpawnData;
        CurLvlData.CrowdData = _CrowdSpawnData;
        EditorUtility.SetDirty(CurLvlData);
        AssetDatabase.SaveAssets();
        Debug.Log("Level Updated");
    }

    private void LoadLvl()
    {
        if (CurLvlData == null) return;
        _ItemSpawnData = CurLvlData.ItemsData ?? new ItemSpawnData();
        _CrowdSpawnData = CurLvlData.CrowdData ?? new CrowdSpawnData(2, 2);
    }

    private void LevelSettings()
    {
        GUILayout.BeginVertical(boxStyle);
        DropDownHeader(ref IsLevelSectionExpanded, "Level Settings");

        if (!IsLevelSectionExpanded)
        {
            GUILayout.EndVertical();
            return;
        }

        HorizontalLineAndSpace();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Level Number :", GUILayout.ExpandWidth(false));
        CurLvlNum = EditorGUILayout.IntSlider(CurLvlNum, 1, 100);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void ItemsSettings()
    {
        _ItemSpawnData ??= new ItemSpawnData();

        GUILayout.BeginVertical(boxStyle);
        DropDownHeader(ref IsItemsSectionExpanded, "Items Spawn Settings");

        if (!IsItemsSectionExpanded)
        {
            GUILayout.EndVertical();
            return;
        }

        HorizontalLineAndSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Row", GUILayout.Height(34)))
        {
            _ItemSpawnData.AddRow();
        }
        if (GUILayout.Button("Clear", GUILayout.Height(34)))
        {
            _ItemSpawnData.Clear();
        }
        GUILayout.EndHorizontal();
        ItemRowsVisualization();
        GUILayout.EndVertical();
    }

    private void ItemRowsVisualization()
    {
        GUILayout.BeginHorizontal();
        for (int i = 0; i < _ItemSpawnData.RowsCount; i++)
        {
            ItemsRow Row = _ItemSpawnData[i];
            RowVisualization(Row, i);
        }
        GUILayout.EndHorizontal();
    }

    private void RowVisualization(ItemsRow Row, int RowIdx)
    {
        void DeleteRow() => _ItemSpawnData.RemoveAt(RowIdx);

        GUILayout.BeginVertical(boxStyle);
        RowHeader(Row, RowIdx, DeleteRow);
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();

        for (int i = 0; i < Row.Count; i++)
        {
            ItemData item = Row[i];

            void DeleteItem()
            {
                Row.RemoveAt(i);
            }

            if (item is SpawnItemData spawnItemData)
            {
                SpawnItemDataVisualization(spawnItemData, DeleteItem);
            }
            else if (item is LockItemData lockItemData)
            {
                LockItemDataVisualization(lockItemData);
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void LockItemDataVisualization(LockItemData lockItemData)
    {
        GUILayout.BeginVertical(boxStyle);
        EditorGUILayout.LabelField("LOCK");

        EditorGUILayout.Space(5);
        GUILayout.BeginHorizontal();
        lockItemData.HasKey = EditorGUILayout.Toggle(lockItemData.HasKey, GUILayout.Width(20));
        if (lockItemData.HasKey)
        {
            lockItemData.KeyId = EditorGUILayout.Vector2IntField("Unlock Key :", lockItemData.KeyId);
        }
        else
        {
            EditorGUILayout.LabelField("Unlock Key : None");
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void SpawnItemDataVisualization(SpawnItemData item, Action Delete)
    {
        GUILayout.BeginVertical(boxStyle);
        GUILayout.BeginHorizontal();

        Color CurColor = GUI.color;
        GUI.color = Utilities.GetColorByItemType(item.Type);

        if (GUILayout.Button(item.Id.ToString(), GUILayout.ExpandWidth(true), GUILayout.Height(45)))
        {
            EditorGUIUtility.systemCopyBuffer = item.Id.ToString();
        }

        if (GUILayout.Button("Delete", GUILayout.Width(65), GUILayout.Height(45)))
        {
            Delete?.Invoke();
        }

        GUI.color = CurColor;
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        item.Type = (ItemType)EditorGUILayout.EnumPopup(item.Type);
        item.SpawnCount = EditorGUILayout.IntSlider("Spawn Count: ", item.SpawnCount, 0, 250);
        item.IsMysterious = EditorGUILayout.Toggle("Is Mysterious :", item.IsMysterious);

        GUILayout.BeginHorizontal();
        item.HasConnection = EditorGUILayout.Toggle(item.HasConnection, GUILayout.Width(20));
        EditorGUILayout.LabelField("Has Connection(s)");
        GUILayout.EndHorizontal();

        if (item.HasConnection)
        {
            if (item.ConnectedTo == null) item.ConnectedTo = new List<Vector2Int>();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Connections: {item.ConnectedTo.Count}");
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                item.ConnectedTo.Add(Vector2Int.zero);
            }
            if (GUILayout.Button("-", GUILayout.Width(25)) && item.ConnectedTo.Count > 0)
            {
                item.ConnectedTo.RemoveAt(item.ConnectedTo.Count - 1);
            }
            GUILayout.EndHorizontal();

            for (int j = 0; j < item.ConnectedTo.Count; j++)
            {
                item.ConnectedTo[j] = EditorGUILayout.Vector2IntField($"Connection {j + 1}:", item.ConnectedTo[j]);
            }
        }

        GUILayout.EndVertical();
    }

    private void RowHeader(ItemsRow Row, int RowIdx, Action deleteRow)
    {
        string RowTitle() => (RowIdx + 1).ToString() + Utilities.GetOrdinalSuffix(RowIdx + 1);

        GUILayout.BeginHorizontal();
        GUILayout.Label(RowTitle(), new GUIStyle() { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } }, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Spawner", GUILayout.Height(35)))
        {
            Vector2Int id = new Vector2Int(RowIdx, Row.Count);
            SpawnItemData item = new SpawnItemData(id);
            Row.Add(item);
        }
        if (GUILayout.Button("Add Lock", GUILayout.Height(35)))
        {
            Vector2Int id = new Vector2Int(RowIdx, Row.Count);
            LockItemData item = new(id);
            Row.Add(item);
        }
        if (GUILayout.Button("Delete Row", GUILayout.Height(35)))
        {
            deleteRow?.Invoke();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void HorizontalLineAndSpace()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space(10);
    }

    private void DropDownHeader(ref bool IsExpanded, string Text)
    {
        if (GUILayout.Button($"{(IsExpanded ? "↓" : "→")} {Text}", expandableStyle))
            IsExpanded = !IsExpanded;
    }
}