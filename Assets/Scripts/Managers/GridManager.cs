using UnityEngine;

public class GridManager: Singleton<GridManager>
{
    [Header("References")]
    public Grid GridSystem;
    public Transform Origin;

    public Vector3 GetPosition(Vector3Int Index)
    {
        return Origin.position + GridSystem.GetCellCenterWorld(Index);
    }
}