using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class UIManager : Singleton<UIManager>
{
    [Header("SCREENS")]
    public UIScreen LevelCompleteScreen;
    public UIScreen LevelFailedScreen;
    public UIScreen MainUIScreen;

    [Header("REFERENCES")]
    public TextMeshProUGUI LevelLabel;
    public Image DangerVignette;



    private bool vignetteVisible = false;
    private Tween vignetteTween;

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

    public void UpdateDangerVignetteAlpha(float fillAmount)
    {
        if (fillAmount > 0.8f)
        {
            if (!vignetteVisible)
            {
                vignetteVisible = true;
                vignetteTween?.Kill();
                vignetteTween = DangerVignette.DOFade(fillAmount, 0.3f);
            }
            else
            {
                Color c = DangerVignette.color;
                DangerVignette.color = new Color(c.r, c.g, c.b, fillAmount);
            }
        }
        else
        {
            if (vignetteVisible)
            {
                vignetteVisible = false;
                vignetteTween?.Kill();
                vignetteTween = DangerVignette.DOFade(0f, 0.3f);
            }
        }
    }
}