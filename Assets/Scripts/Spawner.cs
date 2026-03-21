using UnityEngine;
using System.Collections.Generic;
using ArrowFlowGame.Types;
using DG.Tweening;
using System;
using TMPro;
using ArrowFlow.Types;

public class Spawner : Item, IClickable
{
    [Header("REFERENCES")]
    public ParticleSystem RevealEffect;
    public MeshRenderer Renderer;
    public Spawnable ItemToSpawn;
    public TextMeshPro CountLabel;
    public Connection LeftCon;
    public Connection RightCon;

    [Header("ANIMATIONS")]
    public float SpawnDelay = 0.1f;
    public float RotationSpeed = 20f;
    public float ScaleMultiplier = 1f;
    public float ScaleAnimationDuration = 2f;

    public ItemType Type {get; private set;}
    private int SpawnCount;
    private int _LeftToSpawn;
    private bool IsMysterious = false;
    private int LeftToSpawn
    {
        get { return _LeftToSpawn; }
        set
        {
            _LeftToSpawn = Mathf.Max(0, value);
            CountLabel.text = IsMysterious ? "?" : _LeftToSpawn.ToString();
        }
    }

    public Color ConnectionColor => Utilities.GetColorByItemType(Type);
    private bool IsClicked = false;
    private Material OriginalMat;
    private bool HasConnection;
    private bool HasShotAll = false;
    
    public bool HasCompleted { get; private set; } = false;
    public int Layer => Renderer.gameObject.layer;

    private List<Vector2Int> ConnectedSpawnerIds = new();

    private void OnEnable()
    {
        GameManager.Instance.OnGameStarted += CreateVisualConnections;
    }

    public override void Init(ItemData data, VisualRows Row, Action<Item> OnItemUsed)
    {
        base.Init(data, Row, OnItemUsed);
        
        HasCompleted = false;
        IsClicked = false;
        HasShotAll = false;

        if (data is not SpawnItemData spawnerData) return;
        IsMysterious = spawnerData.IsMysterious;
        Type = spawnerData.Type;
        SpawnCount = spawnerData.SpawnCount;
        LeftToSpawn = SpawnCount;

        OriginalMat = ReferenceManager.Instance.SpawnerMaterials.GetMaterial(spawnerData.Type);
        Renderer.material = IsMysterious ? ReferenceManager.Instance.MysteriousSpawnerMat : OriginalMat;
        HasConnection = spawnerData.HasConnection;

        if (spawnerData.ConnectedTo != null)
            ConnectedSpawnerIds = new List<Vector2Int>(spawnerData.ConnectedTo);
    }

    private void CreateVisualConnections()
    {
        if (!HasConnection || ConnectedSpawnerIds.Count == 0)
        {
            RightCon.enabled = false;
            LeftCon.enabled = false;
            return;
        }

        Vector3 origin = transform.position;
        Vector3 right = transform.right;

        Spawner rightSpawner = null;
        Spawner leftSpawner = null;

        float rightMinDist = float.MaxValue;
        float leftMinDist = float.MaxValue;

        foreach (Vector2Int id in ConnectedSpawnerIds)
        {
            Spawner spawner = ReferenceManager.Instance.IdToSpawner[id];
            Vector3 dir = spawner.transform.position - origin;

            float dot = Vector3.Dot(right, dir);
            float dist = dir.sqrMagnitude;

            if (dot > 0)
            {
                if (dist < rightMinDist)
                {
                    rightMinDist = dist;
                    rightSpawner = spawner;
                }
            }
            else
            {
                if (dist < leftMinDist)
                {
                    leftMinDist = dist;
                    leftSpawner = spawner;
                }
            }
        }

        if (rightSpawner != null)
        {
            RightCon.enabled = true;
            RightCon.SetTarget(this, rightSpawner);
        }
        else
        {
            RightCon.enabled = false;
        }

        if (leftSpawner != null)
        {
            LeftCon.enabled = true;
            LeftCon.SetTarget(this, leftSpawner);
        }
        else
        {
            LeftCon.enabled = false;
        }
    }

    public void OnClick()
    {
        if(PowerupManager.Instance.IsTakingSpawnerInputForExchangePowerup)
        {
            transform
            .DOScale(transform.localScale * ScaleMultiplier, ScaleAnimationDuration)
            .SetLoops(2, LoopType.Yoyo);

            PowerupManager.Instance.AddSpawnerToExchange(this);
            return;
        }

        if (Row.FrontItem != this || IsClicked)
        {
            Debug.Log("ROw front is : " + Row.FrontItem, Row.FrontItem.gameObject);
            Debug.Log("Cannot click spawner",gameObject);
            return;
        }

        HapticsManager.MediumHaptic();
        Quaternion OrgRotation = transform.rotation;
        List<Spawner> ConnectedSpawners = new List<Spawner>();

        if (HasConnection)
        {
            foreach (Vector2Int id in ConnectedSpawnerIds)
            {
                if (ReferenceManager.Instance.IdToSpawner.TryGetValue(id, out Spawner connectedSpawner))
                    ConnectedSpawners.Add(connectedSpawner);
            }
        }

        IsClicked = true;

        transform
        .DOScale(transform.localScale * ScaleMultiplier, ScaleAnimationDuration)
        .SetLoops(2, LoopType.Yoyo);

        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < SpawnCount; i++)
        {
            seq.AppendCallback(SpawnItem);

            if (i < SpawnCount - 1)
                seq.AppendInterval(SpawnDelay);
        }

        seq.onComplete += () =>
        {
            transform.rotation = OrgRotation;
            HasShotAll = true;

            if (!HasConnection || ConnectedSpawners.Count == 0)
            {
                OnComplete();
            }
            else if (ConnectedSpawners.TrueForAll(S => S.IsAtFront() && S.HasShotAll))
            {
                ConnectedSpawners.ForEach(S => S.OnComplete());
                OnComplete();
            }
        };
    }

    protected override void OnComplete()
    {
        if (!IsClicked || HasCompleted) return;
        HasCompleted = true;
        base.OnComplete();
    }

    private void SpawnItem()
    {
        if (!BeltManager.TryGetSocket(transform.position, out ArrowSocket Socket))
        {
            EventManager.GameOver();
            return;
        }

        Socket.Occupied();
        Quaternion LookRot = Quaternion.LookRotation(Socket.transform.position - transform.position);
        Quaternion TargetRot = Quaternion.Euler(transform.rotation.x, LookRot.eulerAngles.y, transform.rotation.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRot, Time.deltaTime * RotationSpeed);
        Spawnable spawnable = Instantiate(ItemToSpawn, transform.position, Quaternion.identity);
        HapticsManager.LightHaptic();
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
        if(!IsMysterious) return;

        RevealEffect.Play();
        IsMysterious = false;
        Renderer.material = OriginalMat;
        CountLabel.text = IsMysterious ? "?" : _LeftToSpawn.ToString();
    }
}