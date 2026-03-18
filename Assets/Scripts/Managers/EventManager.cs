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
        bool AllItemsUsed = ReferenceManager.Instance.IdToSpawner.Values.Where(x => x != null).ToList().Count > 0;
        bool UseIncreasedSpeed = FilledUpLevel || AllItemsUsed;
        BeltManager.Instance.UpdateSpeed(UseIncreasedSpeed);
    }

    private void CheckForLvlCompletion()
    {
        if(!AllCrowdPersonKilled()) return;
        
        GameManager.Instance.CurGameState = GameState.COMPLETED;
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
        GameManager.Instance.CurGameState = GameState.FAILED;
        GameManager.Instance.GlobalInputEnabled = false;
        UIManager.Instance.ShowLevelFailedScreen();
    }
}
