using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    private bool AlreadyShownHalfFillWarning
    {
        get => PlayerPrefs.GetInt("AlreadyShownHalfFillWarning", 0) == 1;
        set => PlayerPrefs.SetInt("AlreadyShownHalfFillWarning", value ? 1 : 0);
    }

    private void Start()
    {
        CrowdManager.Instance.OnCrowdPersonKilled += (_) => CheckForLvlCompletion();
        LevelManager.Instance.OnItemUsed += (_) => UpdateBeltSpeed();
        BeltManager.Instance.OnSocketOccupied += (_) => UpdateBeltSpeed();
        BeltManager.Instance.OnSocketOccupied += FirstHalfFillWarning;
    }

    private void FirstHalfFillWarning(ArrowSocket socket)
    {
        if (AlreadyShownHalfFillWarning) return;

        if (BeltManager.Instance.CurOccupied < BeltManager.Instance.TotalSockets * 0.5f) return;

        AlreadyShownHalfFillWarning = true;
        BeltManager.Instance.OnSocketOccupied -= FirstHalfFillWarning;
        GameManager.Instance.GlobalInputEnabled = false;
        Utilities.AssignLayerRecursively(BeltManager.Instance.BeltObj.transform, TutorialManager.Instance.NoPostProcessLayerIdx);

        PostProcessingManager.Instance.AnimateDimmedExposure();
        Time.timeScale = 0.2f;
        UIManager.Instance.DangerVignette.DOFade(1f,0.1f);

        TextMeshProUGUI text = BeltManager.Instance.CurCapacityText;
        Color originalColor = text.color;
        BeltManager.Instance.OverridingColor = true;

        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() => text.color = Color.red);
        seq.Append(text.transform.DOScale(BeltManager.Instance.LabelOriginalScale * 1.8f,0.2f).SetLoops(4, LoopType.Yoyo));
        seq.AppendCallback(() =>
        {
            UIManager.Instance.UpdateDangerVignetteAlpha(0f);
            BeltManager.Instance.OverridingColor = false;

            text.color = originalColor;
            text.transform.localScale = BeltManager.Instance.LabelOriginalScale;
            UIManager.Instance.DangerVignette.DOFade(0f,0.1f);

            Time.timeScale = 1f;
            Utilities.AssignLayerRecursively(BeltManager.Instance.BeltObj.transform, 0);
            PostProcessingManager.Instance.AnimateNormalExposure();
            GameManager.Instance.GlobalInputEnabled = true;
        });
    }

    private void UpdateBeltSpeed()
    {
        bool FilledUpLevel = BeltManager.Instance.CurOccupied >= BeltManager.Instance.TotalSockets * 0.7f;

        var list = ReferenceManager.Instance.IdToSpawner.Values.Where(x => x != null && x.IsBeingDestroy == false).ToList();

        foreach (var d in list)
            Debug.Log(d.name, d.gameObject);

        bool AllItemsUsed = list.Count == 0;
        bool UseIncreasedSpeed = FilledUpLevel || AllItemsUsed;
        BeltManager.Instance.UpdateSpeed(UseIncreasedSpeed);
    }

    private void CheckForLvlCompletion()
    {
        if (!AllCrowdPersonKilled()) return;

        GameManager.Instance.GlobalInputEnabled = false;
        UIManager.Instance.ShowLevelCompleteScreen();
    }

    private bool AllCrowdPersonKilled()
    {
        Dictionary<Vector2Int, CrowdElement> Dict = CrowdManager.Instance.CrowdElementsDict;
        if (Dict == null) return false;

        return Dict.Count == 0;
    }

    public static void GameOver()
    {
        GameManager.Instance.GlobalInputEnabled = false;
        UIManager.Instance.ShowLevelFailedScreen();
    }
}
