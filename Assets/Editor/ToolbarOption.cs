using System;
using System.Collections.Generic;
using ArrowFlowGame.Types;
using UnityEditor;
using UnityEngine;

public class ToolbarOption<T>
{
    public T Value { get; private set; }
    private GUIStyle boxStyle;
    private readonly string Name;
    private readonly bool ShowName;

    public ToolbarOption(T value, string name = "", bool showName = true)
    {
        Value = value;
        Name = name;
        ShowName = showName;
    }

    public void DrawToolbar()
    {
        boxStyle ??= new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(10, 10, 10, 10),
            margin = new RectOffset(7, 7, 7, 7),
        };
        GUILayout.BeginHorizontal(boxStyle, GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField(Name, GUILayout.Width(100));
        foreach (T option in Enum.GetValues(typeof(T)))
        {
            Color color = GUI.color;

            if (Utilities.TryGetColor(option, out Color curColor))
            {
                GUI.color = curColor;
            }

            string Label = (EqualityComparer<T>.Default.Equals(Value, option) ? "●" : "○") + (ShowName ? ConvertToString(option) : "");
            if (GUILayout.Button(new GUIContent(Label, ConvertToString(option)), GUILayout.Width(50), GUILayout.Height(25)))
            {
                Value = option;
            }

            GUI.color = color;
        }
        GUILayout.EndHorizontal();
    }

    public static string ConvertToString<T>(T value)
    {
        if (value is Hits hits)
        {
            return " " + Convert.ToInt32(value).ToString();
        }

        return " " + value?.ToString();
    }
}