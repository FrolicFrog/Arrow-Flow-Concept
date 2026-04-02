using System;
using System.Collections.Generic;
using System.Linq;
using ArrowFlow.Types;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [Header("SETTINGS")]
    public int NoPostProcessLayerIdx = 3;

    [Header("TUTORIALS")]
    [TextArea(3, 10)]
    public string FirstLvlHintText = "Tap shooter to place arrows into the conveyer.";
    public bool IsTakingSpawnerInputForTutorial { get; private set; } = false;
    public void Initialize()
    {
        int LevelNum = LevelManager.Instance.CurrentLevelNumber;
        if (LevelNum == 1 && !PlayerPrefs.HasKey("CompletedTutorialLvl1"))
        {
            BeginningTutorial();
        }
        else if(LevelNum == 14 && !PlayerPrefs.HasKey("CompletedTutorialLvl14"))
        {
            ConnectedShootersTutorial();
        }
    }

    private void ConnectedShootersTutorial()
    {
        PlayerPrefs.SetInt("CompletedTutorialLvl14", 1);
        PostProcessingManager.Instance.AnimateDimmedExposure();
        var Spawners = ReferenceManager.Instance.IdToSpawner.Values;
        
        Spawner PinkSpawner = Spawners.FirstOrDefault(S => S.Id == new Vector2Int(0,0));
        Spawner CyanSpawner = Spawners.FirstOrDefault(S => S.Id == new Vector2Int(1,0));

        if (PinkSpawner == null || CyanSpawner == null) return;

        IsTakingSpawnerInputForTutorial = true;
        Utilities.AssignLayerRecursively(PinkSpawner.transform, NoPostProcessLayerIdx);
        PinkSpawner.SetFingerAnimationVisible(true);
        PinkSpawner.CanTakeSecondaryActionInput = true;

        PinkSpawner.OnSecondaryActionClick += () =>
        {
            PinkSpawner.CanTakeSecondaryActionInput = false;
            PostProcessingManager.Instance.AnimateNormalExposure();
            PinkSpawner.SetFingerAnimationVisible(false);
            IsTakingSpawnerInputForTutorial = false;
            Utilities.AssignLayerRecursively(PinkSpawner.transform, 0);
            PinkSpawner.OnClick();

            CyanSpawner.SetFingerAnimationVisible(true);
            IsTakingSpawnerInputForTutorial = true;
            CyanSpawner.CanTakeSecondaryActionInput = true;
            CyanSpawner.OnSecondaryActionClick += () =>
            {
                CyanSpawner.CanTakeSecondaryActionInput = false;
                CyanSpawner.SetFingerAnimationVisible(false);
                IsTakingSpawnerInputForTutorial = false;
                CyanSpawner.OnClick();
            };
        };
    }

    private void BeginningTutorial()
    {
        PlayerPrefs.SetInt("CompletedTutorialLvl1", 1);
        PostProcessingManager.Instance.AnimateDimmedExposure();
        var Spawners = ReferenceManager.Instance.IdToSpawner.Values;
        
        Spawner YellowSpawner = Spawners.FirstOrDefault(S => S.Id == new Vector2Int(0,0));
        Spawner GreenSpawner = Spawners.FirstOrDefault(S => S.Id == new Vector2Int(1,0));

        if (YellowSpawner == null || GreenSpawner == null) return;

        IsTakingSpawnerInputForTutorial = true;
        Utilities.AssignLayerRecursively(YellowSpawner.transform, NoPostProcessLayerIdx);
        Utilities.AssignLayerRecursively(GreenSpawner.transform, NoPostProcessLayerIdx);
        YellowSpawner.SetFingerAnimationVisible(true);
        UIManager.Instance.ShowHintBox(FirstLvlHintText);
        YellowSpawner.CanTakeSecondaryActionInput = true;
        GreenSpawner.CanTakeSecondaryActionInput = true;

        GreenSpawner.OnSecondaryActionClick += () =>
        {
            GreenSpawner.CanTakeSecondaryActionInput = false;
            YellowSpawner.CanTakeSecondaryActionInput = false;
            UIManager.Instance.DismissHintBox();
            PostProcessingManager.Instance.AnimateNormalExposure();
            YellowSpawner.SetFingerAnimationVisible(false);
            Utilities.AssignLayerRecursively(YellowSpawner.transform, 0);
            Utilities.AssignLayerRecursively(GreenSpawner.transform, 0);
            IsTakingSpawnerInputForTutorial = false;
            GreenSpawner.OnClick();
        };
        YellowSpawner.OnSecondaryActionClick += () =>
        {
            GreenSpawner.CanTakeSecondaryActionInput = false;
            YellowSpawner.CanTakeSecondaryActionInput = false;
            UIManager.Instance.DismissHintBox();
            PostProcessingManager.Instance.AnimateNormalExposure();
            YellowSpawner.SetFingerAnimationVisible(false);
            Utilities.AssignLayerRecursively(YellowSpawner.transform, 0);
            Utilities.AssignLayerRecursively(GreenSpawner.transform, 0);
            IsTakingSpawnerInputForTutorial = false;
            YellowSpawner.OnClick();
        };
    }
}