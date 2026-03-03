using UnityEngine;
using ArrowFlowGame.Types;
using DG.Tweening;
using System;
using TMPro;

public class Item: MonoBehaviour, IClickable
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
    private VisualRows Row;
    private string Id;
    private int SpawnCount;
    private int _LeftToSpawn;
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

    public void Init(ItemData data, VisualRows Row)
    {
        Type = data.Type;
        Id = data.Id;
        SpawnCount = data.SpawnCount;
        LeftToSpawn = SpawnCount;

        Renderer.material = ReferenceManager.Instance.ItemMats.GetMaterial(data.Type);
        this.Row = Row;
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

        seq.OnComplete(() =>
        {
            Row.Dequeue();
            Row.MoveToNext();
            Destroy(gameObject);
        });
    }

    private void SpawnItem()
    {
        if(BeltManager.TryGetSocket(transform.position, out ArrowSocket Socket))
        {
            Socket.Occupied();
            Quaternion LookRot = Quaternion.LookRotation(Socket.transform.position - transform.position);
            Quaternion TargetRot = Quaternion.Euler(transform.rotation.x, LookRot.eulerAngles.y, transform.rotation.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRot, Time.deltaTime * RotationSpeed);
            Spawnable spawnable = Instantiate(ItemToSpawn, transform.position, Quaternion.identity);
            spawnable.Init(Type, Socket.transform, () =>
            {
                BeltManager.Instance.SetSocketReady(Socket, Type);
                Destroy(spawnable.gameObject);
            });
            
            LeftToSpawn--;
        }
        else
        {
            Debug.LogError("No Socket Found for Arrow! Probably Game Over");
        }
    }

}