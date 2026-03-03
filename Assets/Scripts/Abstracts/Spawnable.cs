using System;
using ArrowFlowGame.Types;
using UnityEngine;

public abstract class Spawnable : MonoBehaviour
{
    public abstract void Init(ItemType type, Transform target, Action OnReachTarget = null);
    public abstract void OnSpawn();
}