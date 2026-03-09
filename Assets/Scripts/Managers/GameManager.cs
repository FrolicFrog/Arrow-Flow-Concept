using ArrowFlowGame.Types;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("INPUT")]
    public bool GlobalInputEnabled = true;
    public GameState CurGameState = GameState.NOT_STARTED;
}