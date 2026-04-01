using UnityEngine.UI;
using TMPro;
using UnityEngine;
using DG.Tweening;
using ArrowFlow.Types;
using ArrowFlowGame.Types;

public class UIManager : Singleton<UIManager>
{
    [Header("SCREENS")]
    public LevelCompleteScreen LevelCompleteScreen;
    public LevelFailedScreen LevelFailedScreen;
    public UIScreen MainUIScreen;
    public HintDialog HintBox;
    public HardLvlDialog HardLevelWarning;
    public HardLvlDialog SuperHardLevelWarning;

    [Header("REFERENCES")]
    public TextMeshProUGUI LevelLabel;
    public Image DangerVignette;

    protected override void Awake()
    {
        base.Awake();
        LevelCompleteScreen.Setup();
        LevelFailedScreen.Setup();
        MainUIScreen.Display();

        // Ensure vignette is completely invisible at the start
        Color vColor = DangerVignette.color;
        vColor.a = 0f;
        DangerVignette.color = vColor;
    }

    public void Initialize()
    {
        UpdateLevelLabel(LevelManager.Instance.CurrentLevelNumber);
        HardLvlWarnings(LevelManager.Instance.LevelData.HardLevel);
    }

    private void HardLvlWarnings(HardLevelType hardLevel)
    {
        HardLevelWarning.Init();
        SuperHardLevelWarning.Init();

        if(hardLevel == HardLevelType.HARD)
        {
            HardLevelWarning.Display();
        }
        else if(hardLevel == HardLevelType.SUPER_HARD)
        {
            SuperHardLevelWarning.Display();
        }
    }

    public void UpdateLevelLabel(int level)
    {
        LevelLabel.text = $"LEVEL {level}";
    }

    public void ShowLevelCompleteScreen()
    {
        if (GameManager.Instance.CurGameState == ArrowFlowGame.Types.GameState.COMPLETED) return;

        GameManager.Instance.CurGameState = ArrowFlowGame.Types.GameState.COMPLETED;
        BeltManager.Instance.SlowedBeltSpeed();
        
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => EffectManager.Instance.Play("confetti"));
        LevelCompleteScreen.ActionBtn.onClick.AddListener(() => LevelManager.Instance.NextLevel());
        LevelCompleteScreen.Display();
    }

    public void ShowLevelFailedScreen()
    {
        if (GameManager.Instance.CurGameState == GameState.FAILED) return;

        GameManager.Instance.CurGameState = GameState.FAILED;
        BeltManager.Instance.SlowedBeltSpeed();
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
            BeltManager.Instance.ClearBeltSockets();
            BeltManager.Instance.CurCapacityText.transform.DOScale(BeltManager.Instance.LabelOriginalScale * 3f,0.5f).SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutBack)
            .OnComplete(() =>
            {
                EffectManager.Instance.Play("failed");
            });
        });

        seq.AppendInterval(0.5f);
        LevelFailedScreen.ActionBtn.onClick.AddListener(() => LevelManager.Instance.ReloadLevel());
        LevelFailedScreen.Display();
    }

    public void ShowHintBox(string hintLabel)
    {
        HintBox.Show(hintLabel);
    }

    public void DismissHintBox()
    {
        HintBox.Hide();
    }
}