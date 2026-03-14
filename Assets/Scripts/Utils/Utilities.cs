using System;
using ArrowFlowGame.Types;
using UnityEngine;

public class Utilities
{
    public static string GetOrdinalSuffix(int number)
    {
        int lastTwo = number % 100;

        if (lastTwo >= 11 && lastTwo <= 13)
        {
            return "th";
        }

        switch (number % 10)
        {
            case 1:
                return "st";
            case 2:
                return "nd";
            case 3:
                return "rd";
            default:
                return "th";
        }
    }

    public static Color GetColorByItemType(ItemType Type)
    {
        return Type switch
        {
            ItemType.NONE => new Color(0.5f, 0.5f, 0.5f),
            ItemType.RED => Color.red,
            ItemType.GREEN => Color.green,
            ItemType.YELLOW => Color.yellow,
            ItemType.BLUE => Color.blue,
            ItemType.PINK => new Color(1f, 0.4f, 0.7f),
            ItemType.CYAN => Color.cyan,
            ItemType.BROWN => new Color(0.6f, 0.3f, 0.1f),
            ItemType.BLACK => Color.black,
            ItemType.OFFWHITE => new Color(0.9f, 0.9f, 0.9f),
            ItemType.WHITE => Color.white,
            ItemType.DARKGREEN => new Color(0f, 0.5f, 0f),
            ItemType.ORANGE => new Color(1f, 0.5f, 0f),
            _ => Color.white,
        };
    }

    public static int GetRandomFakeLvl()
    {
        return UnityEngine.Random.Range(Manager.Instance.MinLvl, Manager.Instance.MaxLvl);
    }

    public static bool TryGetColor<T>(T value, out Color color)
    {
        color = Color.white;
        if (value is ItemType itemType)
        {
            color = GetColorByItemType(itemType);
            return true;
        }

        return false;
    }

    public static int GetNearest(Vector3 Source, Vector3[] Positions)
    {
        int nearestIndex = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < Positions.Length; i++)
        {
            float distance = Vector3.Distance(Source, Positions[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }
}