using UnityEngine;
using ArrowFlowGame.Types;
using System.Collections.Generic;
using System;

public class ReferenceManager : Singleton<ReferenceManager>
{
    public ItemMaterials SpawnerMaterials;
    public ItemMaterials PersonMaterials;
    public ArrowMats ArrowMaterials;
    public Material DeadPersonMaterial;
    public Material DamageFlashedPerson;

    [Header("CAMERA SETTINGS")]
    public Transform Cameras;
    public Vector3 CameraExchangedFocusPos;
    public Vector3 CameraOriginalPos;


    [Header("MYSTERIOUS SPAWNERS")]
    public Material MysteriousSpawnerMat;
    public float ScrollSpeed;

    [Header("KEYED OUTLINE")]
    public Color KeyedOutlineColor;
    public float KeyedOutlineWidth = 56f;
    private Vector2 currentOffset = Vector2.zero;

    public Dictionary<Vector2Int, Lock> KeyIdToLockedItem = new();
    public Dictionary<Vector2Int, Spawner> IdToSpawner = new();

    public List<Spawnable> ActiveArrows;

    public void RegisterLock(Lock lockClone, LockItemData data)
    {
        if (data.HasKey)
        KeyIdToLockedItem.Add(data.KeyId, lockClone);
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

    public void OnActiveArrowDispose()
    {
        if(BeltManager.Instance.CurOccupied >= BeltManager.Instance.TotalSockets && ActiveArrows.Count == 0)
        {
            EventManager.GameOver();
        }
    }
}