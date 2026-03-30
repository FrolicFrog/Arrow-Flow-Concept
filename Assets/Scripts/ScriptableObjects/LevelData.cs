using ArrowFlowGame.Types;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level Data")]
public class LevelData: ScriptableObject
{
    public ItemSpawnData ItemsData;
    public CrowdSpawnData CrowdData;
    public int BeltCapacity = 10;
    public HardLevelType HardLevel = HardLevelType.NONE;
}