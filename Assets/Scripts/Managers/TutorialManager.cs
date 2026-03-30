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
    public string FirstLvlHintText = "Tap the shooter to attack!";
    public bool IsTakingSpawnerInputForTutorial { get; private set; } = false;
    public void Initialize()
    {
        int LevelNum = LevelManager.Instance.CurrentLevelNumber;
        if (LevelNum == 1 && !PlayerPrefs.HasKey("CompletedTutorialLvl1"))
        {
            BeginningTutorial();
        }
    }

    private void BeginningTutorial()
    {
        PlayerPrefs.SetInt("CompletedTutorialLvl1", 1);
        PostProcessingManager.Instance.AnimateDimmedExposure();
        var Spawners = ReferenceManager.Instance.IdToSpawner.Values;

        Spawner FirstSpawner = Spawners.FirstOrDefault();
        if (FirstSpawner == null) return;

        IsTakingSpawnerInputForTutorial = true;
        Utilities.AssignLayerRecursively(FirstSpawner.transform, NoPostProcessLayerIdx);
        FirstSpawner.SetFingerAnimationVisible(true);
        UIManager.Instance.ShowHintBox(FirstLvlHintText);
        FirstSpawner.CanTakeSecondaryActionInput = true;
        FirstSpawner.OnSecondaryActionClick += () =>
        {
            FirstSpawner.CanTakeSecondaryActionInput = false;
            PostProcessingManager.Instance.AnimateNormalExposure();
            UIManager.Instance.DismissHintBox();
            FirstSpawner.SetFingerAnimationVisible(false);
            Utilities.AssignLayerRecursively(FirstSpawner.transform, 0); 
            IsTakingSpawnerInputForTutorial = false;
            FirstSpawner.OnClick();
        };
    }
}