using System;
using ArrowFlowGame.Types;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Arrow : Spawnable
{
    [Header("References")]
    public Renderer[] Renderers;
    public TrailRenderer Trail;
    public float ArrowSpeed = 50f;
    
    [Header("Curved Path Settings")]
    [Tooltip("Controls how wide the curve stretches.")]
    public float CurveMultiplier = 0.5f;

    private Transform Target;
    private ItemType ArrowType;
    private Action OnReachTarget;
    private bool TakeCurvedPath;

    private Vector3 startPosition;
    private Vector3 controlPoint;
    private float progress;
    private float initialDistance;
    private bool hasReachedTarget;
    
    public override void OnSpawn() {}

    public override void Init(ItemType type, Transform target, Action OnReachTarget = null, bool TakeCurvedPath = false)
    {
        Material ArrowMat = ReferenceManager.Instance.ItemMats.GetMaterial(type);
        Material ArrowBodyMat = ReferenceManager.Instance.ArrowBodyMats.GetMaterial(type);
        Material[] MatArr = new Material[3];
        MatArr[0] = ArrowMat;
        MatArr[1] = ArrowBodyMat;
        MatArr[2] = ArrowMat;

        foreach(Renderer r in Renderers)
            r.materials = MatArr;

        Trail.Clear();
        Trail.material = ReferenceManager.Instance.ArrowTrailMats.GetMaterial(type);

        ArrowType = type;
        Target = target;
        this.OnReachTarget = OnReachTarget;
        this.TakeCurvedPath = TakeCurvedPath;

        // Reset movement variables
        hasReachedTarget = false;
        startPosition = transform.position;
        progress = 0f;

        if (Target != null)
        {
            initialDistance = Vector3.Distance(startPosition, Target.position);

            if (TakeCurvedPath && initialDistance > 0)
            {
                // 1. Get random point in circle
                Vector2 randomCircle = Random.insideUnitCircle;

                // 2. Find relative axes so the curve expands outward perpendicular to the flight path
                Vector3 directionToTarget = (Target.position - startPosition).normalized;
                Vector3 right = Vector3.Cross(directionToTarget, Vector3.up).normalized;
                if (right == Vector3.zero) right = Vector3.right; // Edge case if shooting straight up/down
                Vector3 up = Vector3.Cross(right, directionToTarget).normalized;

                // 3. Set the control point halfway to the target, plus the random offset
                Vector3 midPoint = startPosition + (directionToTarget * (initialDistance * 0.5f));
                float offsetScale = initialDistance * CurveMultiplier;
                Vector3 offset = (right * randomCircle.x + up * randomCircle.y) * offsetScale;

                controlPoint = midPoint + offset;
            }
        }
    }

    private void Update()
    {
        if (Target == null || hasReachedTarget) return;

        if (TakeCurvedPath)
        {
            if (initialDistance > 0)
            {
                progress += (ArrowSpeed / initialDistance) * Time.deltaTime;
                progress = Mathf.Clamp01(progress);

                Vector3 nextPosition = GetQuadraticBezierPoint(startPosition, controlPoint, Target.position, progress);

                if (nextPosition != transform.position)
                {
                    transform.LookAt(nextPosition);
                }

                transform.position = nextPosition;

                if (progress >= 1f || Vector3.Distance(transform.position, Target.position) < 0.1f)
                {
                    TriggerReachTarget();
                }
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, Target.position, Time.deltaTime * ArrowSpeed);
            transform.LookAt(Target);

            if (Vector3.Distance(transform.position, Target.position) < 0.1f)
            {
                TriggerReachTarget();
            }
        }
    }

    private void TriggerReachTarget()
    {
        hasReachedTarget = true;
        OnReachTarget?.Invoke();
    }

    private Vector3 GetQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;

        // Formula: B(t) = (1-t)^2 * P0 + 2(1-t)*t * P1 + t^2 * P2
        Vector3 p = uu * p0; 
        p += 2f * u * t * p1; 
        p += tt * p2; 

        return p;
    }
}