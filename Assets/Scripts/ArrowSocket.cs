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
    public bool UseIncreasedSpeed 
    { 
        set
        {
            if(value)
                SplineAnimator.Duration = IncreasedSpeedDuration;
            else
                SplineAnimator.Duration = OriginalDuration;
        }
    }

    [Header("Settings")]
    public float OriginalDuration;
    public float IncreasedSpeedDuration;

    public static ArrowSocket CreateArrowSocket(ArrowSocket Prefab, SplineContainer Container, float Offset)
    {
        ArrowSocket socket = Instantiate(Prefab);
        socket.SplineAnimator.Container = Container;
        socket.SplineAnimator.StartOffset = Offset;
        socket.SplineAnimator.Play();

        return socket;
    }

    public void Occupied()
    {
        IsOccupied = true;
    }

    private void Update()
    {
        if(IsReady && IsFacingCrowd())
        {
            List<CrowdElement> FrontRow = CrowdManager.Instance.CurFront;
            foreach(CrowdElement elem in FrontRow)
            {
                if (elem == null) continue;
                if (elem is Person person)
                {
                    if(person.Type == CurType && (!person.AlreadyTarget || person is Giant) && !person.IsWalking)
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
                        }, UnityEngine.Random.Range(0,10) <= 6);

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
        ArrowRenderer.materials = ReferenceManager.Instance.ArrowMaterials.GetArrowMatArray(Type);

        ArrowRenderer.enabled = true;
        CurType = Type;
        IsReady = true;
    }
}