using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

[Serializable]
public class UIScreen
{
    public GameObject Root;
    public RectTransform Screen;
    public Button ActionBtn;
    public GameObject[] ToggleScreens;

    public virtual void Setup()
    {
        Root.SetActive(false);
    }

    public virtual void Display()
    {
        HandleScreenToggles();
        Root.SetActive(true);
    }

    protected void HandleScreenToggles()
    {
        Array.ForEach(ToggleScreens, s => s.SetActive(false));        
    }

    public void Hide()
    {
        Root.SetActive(false);
    }   
}

[Serializable]
public class LevelCompleteScreen : UIScreen
{
    [Header("SEQUENCING")]
    public float Delay = 1f;
    public RectTransform VictoryTitle;
    public Vector2 TitleInitialPos;
    public Vector2 TitleFinalPos;
    public float FadeInDuration = 0.5f;
    public float EndAlpha = 0.97f;
    public Image Backdrop;

    public override void Setup()
    {
        Backdrop.color = new Color(Backdrop.color.r,Backdrop.color.g,Backdrop.color.b,0f);
        ActionBtn.transform.localScale = Vector3.zero;
        VictoryTitle.localScale = Vector3.zero;
        VictoryTitle.anchoredPosition = TitleInitialPos;
    }

    public override void Display()
    {
        base.Display();

        Sequence Seq = DOTween.Sequence();
        Seq.AppendInterval(Delay);
        Seq.Append(Backdrop.DOFade(EndAlpha, FadeInDuration));
        Seq.Join(VictoryTitle.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutBack));
        Seq.Insert(Delay + 0.2f, VictoryTitle.DOAnchorPos(TitleFinalPos, 0.4f).SetEase(Ease.InOutBack));
        Seq.Append(ActionBtn.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutBack));
    }
}


[Serializable]
public class LevelFailedScreen : UIScreen
{
    [Header("SEQUENCING")]
    public float Delay = 1f;
    public RectTransform FailedTitle;
    public Vector2 TitleInitialPos;
    public Vector2 TitleFinalPos;
    public float FadeInDuration = 0.5f;
    public float EndAlpha = 0.97f;
    public Image Backdrop;

    public override void Setup()
    {
        Backdrop.color = new Color(Backdrop.color.r,Backdrop.color.g,Backdrop.color.b,0f);
        ActionBtn.transform.localScale = Vector3.zero;
        FailedTitle.localScale = Vector3.zero;
        FailedTitle.anchoredPosition = TitleInitialPos;
    }

    public override void Display()
    {
        base.Display();

        Sequence Seq = DOTween.Sequence();
        Seq.AppendInterval(Delay);
        Seq.Append(Backdrop.DOFade(EndAlpha, FadeInDuration));
        Seq.Join(FailedTitle.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutBack));
        Seq.Insert(Delay + 0.2f, FailedTitle.DOAnchorPos(TitleFinalPos, 0.4f).SetEase(Ease.InOutBack));
        Seq.Append(ActionBtn.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutBack));
    }
}