using UnityEngine;
using ArrowFlowGame.Types;
using System.Collections.Generic;
using System;

public class ReferenceManager : Singleton<ReferenceManager>
{
    public ItemMaterials ItemMats;
    public ItemMaterials ArrowBodyMats;
    public ItemMaterials ArrowTrailMats;
    public Material DeadPersonMaterial;
    public Material DamageFlashedPerson;

    [Header("MYSTERIOUS SPAWNERS")]
    public Material MysteriousSpawnerMat;
    public float ScrollSpeed;

    private Vector2 currentOffset = Vector2.zero;

    public Dictionary<Vector2Int, Lock> KeyIdToLockedItem = new();

    public void RegisterLock(Lock lockClone, LockItemData data)
    {
        KeyIdToLockedItem.Add(data.KeyId, lockClone);
    }

    private void Update()
    {
        if (MysteriousSpawnerMat == null) return;

        currentOffset.x += ScrollSpeed * Time.deltaTime;
        currentOffset.y += ScrollSpeed * Time.deltaTime;

        MysteriousSpawnerMat.SetTextureOffset("_MainTex", currentOffset);
        MysteriousSpawnerMat.SetTextureOffset("_BaseMap", currentOffset);
    }
}