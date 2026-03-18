using System.Linq;
using ArrowFlow.Types;
using UnityEngine.UI;

public class FeatureManager : Singleton<FeatureManager>
{
    public LevelFeature[] LevelFeatures;
    public Image ImageDisplay;
    public Button ContinueBtn;


    private void Start()
    {
        LevelFeature Feature = LevelFeatures.FirstOrDefault(F => F.LevelNum == LevelManager.Instance.CurrentLevelNumber);
        if(Feature == null) return;

        ImageDisplay.sprite = Feature.Graphic;
    }
}