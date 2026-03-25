using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
        Debug.Log($"Socket Occupied. {BeltManager.Instance.CurOccupied} / {BeltManager.Instance.TotalSockets}");
        if(BeltManager.Instance.CurOccupied >= BeltManager.Instance.TotalSockets * 0.5f && !AlreadyShownHalfFillWarning)
        {
            Debug.Log("Show Warning");
            AlreadyShownHalfFillWarning = true;
            GameManager.Instance.GlobalInputEnabled = false;
            BeltManager.Instance.StopBelt();
            BeltManager.Instance.CurCapacityText.transform.DOScale(BeltManager.Instance.CurCapacityText.transform.localScale * 2f, 0.5f).SetEase(Ease.InOutBack).SetLoops(4, LoopType.Yoyo)
            .OnComplete(() =>
            {
                GameManager.Instance.GlobalInputEnabled = true;
                BeltManager.Instance.ResumeBelt();
            });
        }
    }

    private void UpdateBeltSpeed()
    {
        bool FilledUpLevel = BeltManager.Instance.CurOccupied >= BeltManager.Instance.TotalSockets * 0.7f;
        
        var list = ReferenceManager.Instance.IdToSpawner.Values.Where(x => x != null && x.IsBeingDestroy == false).ToList();
        
        foreach(var d in list)
        Debug.Log(d.name, d.gameObject);

        bool AllItemsUsed = list.Count == 0;
        bool UseIncreasedSpeed = FilledUpLevel || AllItemsUsed;
        BeltManager.Instance.UpdateSpeed(UseIncreasedSpeed);
    }

    private void CheckForLvlCompletion()
    {
        if(!AllCrowdPersonKilled()) return;
        
        GameManager.Instance.GlobalInputEnabled = false;
        UIManager.Instance.ShowLevelCompleteScreen();
    }

    private bool AllCrowdPersonKilled()
    {
        Dictionary<Vector2Int, CrowdElement> Dict = CrowdManager.Instance.CrowdElementsDict;
        if(Dict == null) return false;

        return Dict.Count == 0;
    }

    public static void GameOver()
    {
        GameManager.Instance.GlobalInputEnabled = false;
        UIManager.Instance.ShowLevelFailedScreen();
    }
}
