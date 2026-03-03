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
        switch (Type)
        {
            case ItemType.RED:
                return Color.red;

            case ItemType.BLUE:
                return Color.blue;

            case ItemType.GREEN:
                return Color.green;

            case ItemType.CYAN:
                return Color.cyan;

            case ItemType.DARKGREEN:
                return new Color(0f, 0.5f, 0f);

            case ItemType.ORANGE:
                return new Color(1f, 0.5f, 0f);

            case ItemType.PINK:
                return new Color(1f, 0.4f, 0.7f);

            case ItemType.WHITE:
                return Color.white;

            default:
                return Color.white;
        }
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