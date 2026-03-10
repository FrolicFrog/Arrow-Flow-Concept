using TMPro;
using UnityEngine;

public class UIManager: Singleton<UIManager>
{
    [Header("SCREENS")]
    public UIScreen LevelCompleteScreen;
    public UIScreen LevelFailedScreen;
    public UIScreen MainUIScreen;

    [Header("REFERENCES")]
    public TextMeshProUGUI LevelLabel;

    protected override void Awake()
    {
        base.Awake();
        LevelCompleteScreen.Setup();
        LevelFailedScreen.Setup();
        MainUIScreen.Display();
    }

    public void UpdateLevelLabel(int level)
    {
        LevelLabel.text = $"LEVEL {level}";
    }

    public void ShowLevelCompleteScreen()
    {
        LevelCompleteScreen.ActionBtn.onClick.AddListener(() => LevelManager.Instance.NextLevel());
        LevelCompleteScreen.Display();
    }

    public void ShowLevelFailedScreen()
    {
        LevelFailedScreen.ActionBtn.onClick.AddListener(() => LevelManager.Instance.ReloadLevel());
        LevelFailedScreen.Display();
    }
}