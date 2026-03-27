using System;
using System.Collections.Generic;
using ArrowFlowGame.Types;
using UnityEngine;
using UnityEngine.Splines;

public class ArrowSocket : MonoBehaviour
{
    [Header("References")]
    public float MaxDot = 0.91f;

    public Spawnable ArrowPrefab;
    public SplineAnimate SplineAnimator;
    public MeshRenderer ArrowRenderer;

    public bool IsOccupied;
    public bool IsReady;
    public ItemType CurType { get; private set; }
    private bool _useIncreasedSpeed;
    public bool UseIncreasedSpeed
    {
        get => _useIncreasedSpeed;
        set
        {
            _useIncreasedSpeed = value;

            float normalizedTime = SplineAnimator.NormalizedTime;

            if (value)
                SplineAnimator.Duration = IncreasedSpeedDuration;
            else
                SplineAnimator.Duration = OriginalDuration;

            SplineAnimator.NormalizedTime = normalizedTime;
        }
    }

    [Header("Settings")]
    public float OriginalDuration;
    public float IncreasedSpeedDuration;
    public float SlowMotionSpeedDuration = 50f;

    public static ArrowSocket CreateArrowSocket(ArrowSocket Prefab, SplineContainer Container, float Offset, int layerIdx)
    {
        ArrowSocket socket = Instantiate(Prefab, BeltManager.Instance.BeltObj.transform);
        socket.SplineAnimator.Container = Container;
        socket.SplineAnimator.StartOffset = Offset;
        socket.SplineAnimator.Duration = socket.OriginalDuration;
        Utilities.AssignLayerRecursively(socket.transform, layerIdx);

        socket.SplineAnimator.Restart(true);

        // Force evaluate position and rotation immediately to prevent 0-frame position bugs
        Unity.Mathematics.float3 localPos = Container.EvaluatePosition(Offset);
        Unity.Mathematics.float3 tangent = Container.EvaluateTangent(Offset);
        Unity.Mathematics.float3 up = Container.EvaluateUpVector(Offset);

        socket.transform.position = Container.transform.TransformPoint(localPos);
        socket.transform.rotation = Quaternion.LookRotation(Container.transform.TransformDirection(tangent), Container.transform.TransformDirection(up));

        return socket;
    }

    public void Occupied()
    {
        IsOccupied = true;
    }

    public void StartSuperSlowMotion()
    {
        float normalizedTime = SplineAnimator.NormalizedTime;
        SplineAnimator.Duration = SlowMotionSpeedDuration;
        SplineAnimator.NormalizedTime = normalizedTime;
    }

    public void StopSuperSlowMotion()
    {
        float normalizedTime = SplineAnimator.NormalizedTime;
        SplineAnimator.Duration = OriginalDuration;
        SplineAnimator.NormalizedTime = normalizedTime;
    }

    private void Update()
    {
        if (IsReady && IsFacingCrowd())
        {
            List<CrowdElement> FrontRow = CrowdManager.Instance.CurFront;
            foreach (CrowdElement elem in FrontRow)
            {
                if (elem == null) continue;
                if (elem is Person person)
                {
                    if (person.Type == CurType && (!person.AlreadyTarget || person is Giant) && !person.IsWalking)
                    {
                        person.AlreadyTarget = true;

                        Spawnable Arrow = Instantiate(ArrowPrefab, transform.position, Quaternion.identity);
                        Arrow.Init(CurType, person.transform, () =>
                        {
                            person.Damage();
                            Destroy(Arrow.gameObject);

                            if (person.RequiredHits <= 0)
                            {
                                CrowdManager.Instance.RemoveCrowdElement(elem);
                            }
                            else
                            {
                                person.AlreadyTarget = false;
                            }
                        }, UnityEngine.Random.Range(0, 10) <= 6);

                        BeltManager.Instance.SetSocketEmpty(this);
                        break;
                    }
                }
            }
        }
    }

    private bool IsFacingCrowd()
    {
        Vector3 directionToTarget = (GridManager.Instance.Origin.position - transform.position).normalized;
        Vector3 forward = -transform.up;
        float dot = Vector3.Dot(forward, directionToTarget);
        return dot > MaxDot;
    }

    public void Ready(ItemType Type)
    {
        ArrowRenderer.sharedMaterials = ReferenceManager.Instance.ArrowMaterials.GetArrowMatArray(Type);

        ArrowRenderer.enabled = true;
        CurType = Type;
        IsReady = true;
    }
}