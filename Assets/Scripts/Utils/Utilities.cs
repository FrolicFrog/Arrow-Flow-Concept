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
            ItemType.RED => Color.red,
            ItemType.BLUE => Color.blue,
            ItemType.GREEN => Color.green,
            ItemType.CYAN => Color.cyan,
            ItemType.DARKGREEN => new Color(0f, 0.5f, 0f),
            ItemType.ORANGE => new Color(1f, 0.5f, 0f),
            ItemType.PINK => new Color(1f, 0.4f, 0.7f),
            ItemType.WHITE => Color.white,
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
}