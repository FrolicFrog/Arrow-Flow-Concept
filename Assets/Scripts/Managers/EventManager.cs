using System;
using System.Collections.Generic;
using ArrowFlowGame.Types;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    private void Start()
    {
        CrowdManager.Instance.OnCrowdPersonKilled += (_) => CheckForLvlCompletion();
        LevelManager.Instance.OnItemUsed += (_) => CheckForLvlCompletion();
        BeltManager.Instance.OnSocketOccupied += (_) => UpdateBeltSpeed();
    }

    private void UpdateBeltSpeed()
    {
        bool UseIncreasedSpeed = BeltManager.Instance.CurOccupied == BeltManager.Instance.TotalSockets;
        BeltManager.Instance.UpdateSpeed(UseIncreasedSpeed);
    }

    private void CheckForLvlCompletion()
    {
        if(AllCrowdPersonKilled() && AllSpawnersUsed())
        {
            GameManager.Instance.CurGameState = GameState.COMPLETED;
            GameManager.Instance.GlobalInputEnabled = false;
            UIManager.Instance.ShowLevelCompleteScreen();
        }
    }

    private bool AllSpawnersUsed()
    {
        VisualRows[] Rows = LevelManager.Instance.Rows;
        foreach(var row in Rows)
        {
            if(row.Count != 0)
                return false;
        }

        return true;
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
