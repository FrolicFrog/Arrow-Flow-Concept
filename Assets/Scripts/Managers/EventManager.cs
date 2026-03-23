using System;
using System.Collections.Generic;
using System.Linq;
using ArrowFlowGame.Types;
using UnityEngine;
public class EventManager : Singleton<EventManager>
{
    private void Start()
    {
        CrowdManager.Instance.OnCrowdPersonKilled += (_) => CheckForLvlCompletion();
        LevelManager.Instance.OnItemUsed += (_) => UpdateBeltSpeed();
        BeltManager.Instance.OnSocketOccupied += (_) => UpdateBeltSpeed();
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
