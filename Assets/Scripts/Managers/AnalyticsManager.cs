using UnityEngine;
using LionStudios.Suite.Analytics;

namespace Managers
{
    public class AnalyticsManager : MonoBehaviour
    {
        public static void LevelStarted(int LevelNumber)
        {
            LionAnalytics.MissionStarted(missionType: "main", missionName: $"main_{LevelNumber}", missionID: LevelNumber, missionAttempt: null, additionalData: null, isGamePlay: true);
        }

        public static void LevelCompleted(int LevelNumber)
        {
            int TotalCoins = PlayerPrefs.GetInt("coins", 0);
            LionAnalytics.SetPlayerScore(TotalCoins);
            LionAnalytics.MissionCompleted(missionType: "main", missionName: $"main_{LevelNumber}", missionID: LevelNumber, missionAttempt: null, additionalData: null, reward: null, isGamePlay: true);
        }

        public static void LevelFailed(int LevelNumber)
        {
            LionAnalytics.MissionFailed(missionType: "main", missionName: $"main_{LevelNumber}", missionID: LevelNumber, missionAttempt: null, additionalData: null, failReason: "Belt Filled Completely", isGamePlay: true);
        }

        public static void PowerupUsed(int LevelNumber, string PowerupName)
        {
            LionAnalytics.PowerUpUsed(missionID: LevelNumber.ToString(), missionType: "main", missionAttempt: -1, powerUpName: PowerupName, missionName: $"main_{LevelNumber}");
        }
    }
}