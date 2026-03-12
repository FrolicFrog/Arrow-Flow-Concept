using UnityEngine;
using ArrowFlowGame.Types;
using DG.Tweening;
using System;
using TMPro;

public class Spawner: Item, IClickable
{
    [Header("REFERENCES")]
    public MeshRenderer Renderer;
    public Spawnable ItemToSpawn;
    public TextMeshPro CountLabel;

    [Header("ANIMATIONS")]
    public float SpawnDelay = 0.1f;
    public float RotationSpeed = 20f;
    public float ScaleMultiplier = 1f;
    public float ScaleAnimationDuration = 2f;

    private ItemType Type;
    private int SpawnCount;
    private int _LeftToSpawn;
    private bool IsMysterious = false;
    private int LeftToSpawn
    {
        get { return _LeftToSpawn; }
        set 
        { 
            _LeftToSpawn = Mathf.Max(0,value);
            CountLabel.text = _LeftToSpawn.ToString();
        }
    }

    private bool IsClicked = false;
    private Material OriginalMat;

    public override void Init(ItemData data, VisualRows Row, Action<Item> OnItemUsed)
    {
        base.Init(data, Row, OnItemUsed);

        if(data is not SpawnItemData spawnerData) return;
        IsMysterious = spawnerData.IsMysterious;
        Type = spawnerData.Type;
        SpawnCount = spawnerData.SpawnCount;
        LeftToSpawn = SpawnCount;

        OriginalMat = ReferenceManager.Instance.ItemMats.GetMaterial(spawnerData.Type);
        Renderer.material = IsMysterious ? ReferenceManager.Instance.MysteriousSpawnerMat : OriginalMat;
        CountLabel.enabled = !IsMysterious;
    }

    public void OnClick()
    {
        if(Row.FrontItem != this || IsClicked)
        return;

        IsClicked = true;

        transform
        .DOScale(transform.localScale * ScaleMultiplier, ScaleAnimationDuration)
        .SetLoops(2, LoopType.Yoyo);

        Sequence seq = DOTween.Sequence();
        
        for(int i = 0; i < SpawnCount; i++)
        {
            seq.AppendCallback(SpawnItem);
            if(i < SpawnCount - 1)
                seq.AppendInterval(SpawnDelay);
        }

        seq.OnComplete(OnComplete);
    }

    private void SpawnItem()
    {
        if(!BeltManager.TryGetSocket(transform.position, out ArrowSocket Socket))
        {
            EventManager.GameOver();
            return;
        }
        
        Socket.Occupied();
        Quaternion LookRot = Quaternion.LookRotation(Socket.transform.position - transform.position);
        Quaternion TargetRot = Quaternion.Euler(transform.rotation.x, LookRot.eulerAngles.y, transform.rotation.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRot, Time.deltaTime * RotationSpeed);
        Spawnable spawnable = Instantiate(ItemToSpawn, transform.position, Quaternion.identity);
        spawnable.Init(Type, Socket.transform, () =>
        {
            BeltManager.Instance.SetSocketReady(Socket, Type);
            BeltManager.Instance.SocketOccupied(Socket);
            Destroy(spawnable.gameObject);
        });
        
        LeftToSpawn--;
    }

    public override void OnMoveForward()
    {
        Renderer.material = OriginalMat;
        CountLabel.enabled = true;
    }
}