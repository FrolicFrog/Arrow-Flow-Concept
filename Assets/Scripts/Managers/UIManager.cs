using System;
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

    //Tweens
    private Tween FadeInTween;
    private Tween FadeOutTween;

    protected override void Awake()
    {
        base.Awake();
        LevelCompleteScreen.Setup();
        LevelFailedScreen.Setup();
        MainUIScreen.Display();
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
        if (GameManager.Instance.CurGameState == ArrowFlowGame.Types.GameState.FAILED) return;

        GameManager.Instance.CurGameState = ArrowFlowGame.Types.GameState.FAILED;
        BeltManager.Instance.SlowedBeltSpeed();
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => EffectManager.Instance.Play("failed"));
        LevelFailedScreen.ActionBtn.onClick.AddListener(() => LevelManager.Instance.ReloadLevel());
        LevelFailedScreen.Display();
    }

    public void UpdateDangerVignetteAlpha(float fillAmount)
    {
        if (fillAmount > 0.7f)
        {
            if (FadeInTween != null && FadeInTween.IsActive() && FadeInTween.IsPlaying())
            {
                return;
            }

            if (FadeOutTween != null && FadeOutTween.IsActive())
            {
                FadeOutTween.Kill();
            }

            if (DangerVignette.color.a < 1f)
            {
                FadeInTween = DangerVignette.DOFade(1f, 1f);
            }
        }
        else
        {
            if (FadeOutTween != null && FadeOutTween.IsActive() && FadeOutTween.IsPlaying())
            {
                return;
            }

            if (FadeInTween != null && FadeInTween.IsActive())
            {
                FadeInTween.Kill();
            }

            if (DangerVignette.color.a > 0f)
            {
                FadeOutTween = DangerVignette.DOFade(0f, 1f);
            }
        }
    }

    public void ShowHintBox(string hintLabel, bool disableDialogGraphics = false)
    {
        HintBox.Show(hintLabel, disableDialogGraphics);
    }

    public void DismissHintBox()
    {
        HintBox.Hide();
    }
}