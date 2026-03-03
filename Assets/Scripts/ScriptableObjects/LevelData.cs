using ArrowFlowGame.Types;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level Data")]
public class LevelData: ScriptableObject
{
    public ItemSpawnData ItemsData;
    public CrowdSpawnData CrowdData;
}