using System;
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
        foreach (ItemData item in Row)
        {
            GUILayout.BeginVertical(boxStyle);
            Color CurColor = GUI.color;
            GUI.color = Utilities.GetColorByItemType(item.Type);
            if (GUILayout.Button(new GUIContent(item.Id.ToString(), "Click to copy id"), GUILayout.ExpandWidth(true), GUILayout.Height(45)))
            {
                EditorGUIUtility.systemCopyBuffer = item.Id;
            }
            GUI.color = CurColor;
            EditorGUILayout.Space(5);
            item.Type = (ItemType)EditorGUILayout.EnumPopup(item.Type);

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Is Locked:", labelStyle);
            item.IsLocked = EditorGUILayout.Toggle(item.IsLocked);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            item.SpawnCount = EditorGUILayout.IntSlider("Spawn Count: ", item.SpawnCount, 0, 250);

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Connected to:", labelStyle, GUILayout.ExpandWidth(false));
            item.ConnectedTo = EditorGUILayout.TextField(item.ConnectedTo);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
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
        if (GUILayout.Button("+", GUILayout.Height(35)))
        {
            string id = $"{RowIdx}_{Row.Count}";
            ItemData item = new(id);
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