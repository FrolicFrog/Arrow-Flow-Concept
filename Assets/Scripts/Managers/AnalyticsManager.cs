using System.Collections.Generic;
using UnityEngine;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Ads;
using System;

namespace Managers
{
    public class AnalyticsManager : MonoBehaviour
    {
        private const int DEFAULT_ATTEMPT = 1;

        public static void LevelStarted(int LevelNumber)
        {
            LionAnalytics.MissionStarted
            (
                missionType: "main",
                missionName: $"main_{LevelNumber}",
                missionID: LevelNumber,
                missionAttempt: DEFAULT_ATTEMPT,
                additionalData: null,
                isGamePlay: true
            );
        }

        public static void LevelCompleted(int LevelNumber)
        {
            int TotalCoins = PlayerPrefs.GetInt("coins", 0);
            LionAnalytics.SetPlayerScore(TotalCoins);

            LionAnalytics.MissionCompleted
            (
                missionType: "main",
                missionName: $"main_{LevelNumber}",
                missionID: LevelNumber,
                missionAttempt: DEFAULT_ATTEMPT,
                additionalData: null,
                reward: null,
                isGamePlay: true
            );
        }

        public static void LevelFailed(int LevelNumber)
        {
            Dictionary<string, object> failData = new()
            {
                { "gameplay_data", new Dictionary<string, object> { { "mission_progress", 0 } } }
            };

            LionAnalytics.MissionFailed
            (
                missionType: "main",
                missionName: $"main_{LevelNumber}",
                missionID: LevelNumber,
                missionAttempt: DEFAULT_ATTEMPT,
                additionalData: failData,
                failReason: "Belt Filled Completely",
                isGamePlay: true
            );
        }

        public static void PowerupUsed(int LevelNumber, string PowerupName)
        {
            LionAnalytics.PowerUpUsed
            (
                missionID: LevelNumber.ToString(),
                missionType: "main",
                missionAttempt: DEFAULT_ATTEMPT,
                powerUpName: PowerupName,
                missionName: $"main_{LevelNumber}"
            );
        }

        public static void RewardedVideoShown(int LevelNumber, string placementName)
        {
            Dictionary<string, object> adData = new Dictionary<string, object>
            {
                { "mission_data", new Dictionary<string, object>
                    {
                        { "mission_type", "main" },
                        { "mission_name", $"main_{LevelNumber}" },
                        { "mission_id", LevelNumber },
                        { "mission_attempt", DEFAULT_ATTEMPT }
                    }
                },
                { "gameplay_data", new Dictionary<string, object>
                    {
                        { "mission_progress", 0 }
                    }
                }
            };

            LionAnalytics.RewardVideoShow(
                placement: placementName,
                network: null,
                level: LevelNumber,
                additionalData: adData
            );
        }

        public static bool TryShowRewardedAd(string placement, Action OnRewarded)
        {
            Dictionary<string, object> adData = new()
            {
                { "mission_data", new Dictionary<string, object>
                    {
                        { "mission_type", "main" },
                        { "mission_name", $"main_{LevelManager.Instance.CurrentLevelNumber}" },
                        { "mission_id", LevelManager.Instance.CurrentLevelNumber },
                        { "mission_attempt", 1 }
                    }
                },
                { "gameplay_data", new Dictionary<string, object>
                    {
                        { "mission_progress", 0 }
                    }
                }
            };

            return LionAds.TryShowRewarded(placement, OnRewarded, null, null, adData);
        }
    }
}