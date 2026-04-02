using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    private static readonly int CurrentCoins = -1; //No Coins System in Game

    public static void LevelStarted(int LevelNumber)
    {
        // TinySauce.OnGameStarted(LevelNumber);
    }

    public static void LevelCompleted(int LevelNumber)
    {
        // TinySauce.OnGameFinished(true, CurrentCoins, LevelNumber);
    }

    public static void LevelFailed(int LevelNumber)
    {
        // TinySauce.OnGameFinished(false, CurrentCoins, LevelNumber);
    }

    public static void PowerupUsed(int LevelNumber, string PowerupName)
    {
        // TinySauce.OnPowerUpUsed(PowerupName, LevelNumber);
    }
}