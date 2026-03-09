public class UIManager: Singleton<UIManager>
{
    public UIScreen LevelCompleteScreen;
    public UIScreen LevelFailedScreen;

    protected override void Awake()
    {
        base.Awake();
        LevelCompleteScreen.Setup();
        LevelFailedScreen.Setup();
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