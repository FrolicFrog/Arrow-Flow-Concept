using System;
using System.Collections.Generic;
using System.Linq;
using ArrowFlow.Types;
using DG.Tweening;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [Header("SETTINGS")]
    public int NoPostProcessLayerIdx = 3;

    [Header("TUTORIALS")]
    [TextArea(3, 10)]
    public string FirstLvlHintText = "Tap shooter to place arrows into the conveyer.";
    public string FirstLvlSecondHintText = "Wait for all arrows to be spawned";
    public bool IsTakingSpawnerInputForTutorial { get; private set; } = false;
    public void Initialize()
    {
        int LevelNum = LevelManager.Instance.CurrentLevelNumber;
        if (LevelNum == 1 && !PlayerPrefs.HasKey("CompletedTutorialLvl1"))
        {
            BeginningTutorial();
        }
        else if (LevelNum == 14 && !PlayerPrefs.HasKey("CompletedTutorialLvl14"))
        {
            ConnectedShootersTutorial();
        }
    }

    private void ConnectedShootersTutorial()
    {
        PlayerPrefs.SetInt("CompletedTutorialLvl14", 1);
        PostProcessingManager.Instance.AnimateDimmedExposure();
        var Spawners = ReferenceManager.Instance.IdToSpawner.Values;

        Spawner PinkSpawner = Spawners.FirstOrDefault(S => S.Id == new Vector2Int(0, 0));
        Spawner CyanSpawner = Spawners.FirstOrDefault(S => S.Id == new Vector2Int(1, 0));

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
        UIManager.Instance.TutorialFirstScreen.gameObject.SetActive(true);
        var Spawners = ReferenceManager.Instance.IdToSpawner.Values;

        Spawner Spawner1 = Spawners.FirstOrDefault(S => S.Id == new Vector2Int(0, 0));
        Spawner Spawner2 = Spawners.FirstOrDefault(S => S.Id == new Vector2Int(1, 0));
        if (Spawner1 == null || Spawner2 == null) return;

        IsTakingSpawnerInputForTutorial = true;

        Spawner1.SetFingerAnimationVisible(true);
        UIManager.Instance.ShowHintBox(FirstLvlHintText);

        Spawner1.CanTakeSecondaryActionInput = true;
        Spawner2.CanTakeSecondaryActionInput = true;

        void SecondaryActionSpawner1()
        {
            Spawner1.OnSecondaryActionClick -= SecondaryActionSpawner1;
            Spawner2.OnSecondaryActionClick -= SecondaryActionSpawner2;

            Spawner1.SetFingerAnimationVisible(false);
            Spawner1.CanTakeSecondaryActionInput = false;

            UIManager.Instance.ShowHintBox(FirstLvlSecondHintText);
            UIManager.Instance.TutorialFirstScreen.gameObject.SetActive(false);

            DOVirtual.DelayedCall(1f, () =>
            {
                UIManager.Instance.DismissHintBox();
                
                if(!Spawner2.Clicked) Spawner2.SetFingerAnimationVisible(true);
            });

            void DisableFingerAndAction()
            {
                Spawner2.OnSecondaryActionClick -= DisableFingerAndAction;
                IsTakingSpawnerInputForTutorial = false;
                Spawner2.SetFingerAnimationVisible(false);
                Spawner2.OnClick();
            }

            Spawner2.OnSecondaryActionClick += DisableFingerAndAction;

            IsTakingSpawnerInputForTutorial = false;
            Spawner1.OnClick();
            IsTakingSpawnerInputForTutorial = true;
        }

        void SecondaryActionSpawner2()
        {
            Spawner2.OnSecondaryActionClick -= SecondaryActionSpawner2;
            Spawner1.OnSecondaryActionClick -= SecondaryActionSpawner1;

            Spawner2.SetFingerAnimationVisible(false);
            Spawner2.CanTakeSecondaryActionInput = false;

            UIManager.Instance.ShowHintBox(FirstLvlSecondHintText);
            UIManager.Instance.TutorialFirstScreen.gameObject.SetActive(false);

            DOVirtual.DelayedCall(1f, () =>
            {
                UIManager.Instance.DismissHintBox();
                Spawner1.SetFingerAnimationVisible(true);
            });

            void DisableFingerAndAction()
            {
                Spawner1.OnSecondaryActionClick -= DisableFingerAndAction;
                IsTakingSpawnerInputForTutorial = false;
                Spawner1.SetFingerAnimationVisible(false);
                Spawner1.OnClick();
            }

            Spawner1.OnSecondaryActionClick += DisableFingerAndAction;

            IsTakingSpawnerInputForTutorial = false;
            Spawner2.OnClick();
            IsTakingSpawnerInputForTutorial = true;
        }

        Spawner1.OnSecondaryActionClick += SecondaryActionSpawner1;
        Spawner2.OnSecondaryActionClick += SecondaryActionSpawner2;
    }
}