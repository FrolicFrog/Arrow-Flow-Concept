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
    public Dictionary<Vector2Int, Spawner> IdToSpawner = new();

    public void RegisterLock(Lock lockClone, LockItemData data)
    {
        if (data.HasKey)
        {
            KeyIdToLockedItem.Add(data.KeyId, lockClone);
        }
    }
    
    public void RegisterSpawner(Spawner spawnerClone, SpawnItemData spawnerData)
    {
        IdToSpawner.Add(spawnerData.Id, spawnerClone);
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