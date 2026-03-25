using System.Linq;
using ArrowFlow.Types;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeatureManager : Singleton<FeatureManager>
{
    public LevelFeature[] LevelFeatures;
    public RectTransform FeatureDialog;
    public Image ImageDisplay;
    public TextMeshProUGUI ProgressText;

    [Header("FEATURE UI")]
    public GameObject FeatureUI;
    public Image FeatureImage;
    public Button FeatureContinueButton;
    public bool CompletedProgress = false;

    public void ShowFeature()
    {
        int CurLvl = LevelManager.Instance.CurrentLevelNumber;
        LevelFeature Feature = LevelFeatures.FirstOrDefault(F => F.StartLevelNum <= CurLvl && F.EndLevelNum >= CurLvl);
        if(Feature == null) return;

        CompletedProgress = Feature.EndLevelNum == CurLvl;
        ImageDisplay.sprite = Feature.Graphic;

        float StartFillAmount = Feature.ProgressPercent(LevelManager.Instance.CurrentLevelNumber - 1);  
        float EndFillAmount = Feature.ProgressPercent(LevelManager.Instance.CurrentLevelNumber);
        ImageDisplay.fillAmount = StartFillAmount;
        ProgressText.text = $"{Mathf.CeilToInt(StartFillAmount * 100)}%";
        Sequence Seq = DOTween.Sequence();
        FeatureDialog.localScale = Vector3.zero;
        FeatureDialog.gameObject.SetActive(true);

        Seq.Append(FeatureDialog.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutBounce));
        Seq.Append(ImageDisplay.DOFillAmount(EndFillAmount, 0.5f));
        Seq.Join
        (
            DOVirtual.Float(StartFillAmount, EndFillAmount, 0.5f, (V) =>
            {
                ProgressText.text = $"{Mathf.CeilToInt(V * 100)}%";
            })
        );

        if(!CompletedProgress) return;
        Seq.AppendCallback(() =>
        {
            FeatureImage.sprite = Feature.FeatureUIGraphic;
            FeatureContinueButton.transform.localScale = Vector3.zero;
            FeatureContinueButton.onClick.AddListener(() => LevelManager.Instance.NextLevel());
            FeatureUI.SetActive(true);
        });

        Seq.Join(FeatureContinueButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutBounce));
    }
}
