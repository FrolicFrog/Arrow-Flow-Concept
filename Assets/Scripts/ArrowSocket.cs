using System.Collections.Generic;
using ArrowFlowGame.Types;
using UnityEngine;
using UnityEngine.Splines;

public class ArrowSocket : MonoBehaviour
{
    [Header("References")]
    public Spawnable ArrowPrefab;
    public SplineAnimate SplineAnimator;
    public MeshRenderer ArrowRenderer;

    public bool IsOccupied;
    public bool IsReady;
    public ItemType CurType { get; private set; }

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
                    if(person.Type == CurType && !person.AlreadyTarget)
                    {
                        person.AlreadyTarget = true;

                        Spawnable Arrow = Instantiate(ArrowPrefab, transform.position, Quaternion.identity);
                        Arrow.Init(CurType, person.transform, () => 
                        {
                            person.Dead();
                            Destroy(Arrow.gameObject);
                            CrowdManager.Instance.RemoveCrowdElement(elem);
                        });

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

        return dot > 0f;
    }

    public void Ready(ItemType Type)
    {
        Material ArrowMat = ReferenceManager.Instance.ItemMats.GetMaterial(Type);
        Material ArrowBodyMat = ReferenceManager.Instance.ArrowBodyMats.GetMaterial(Type);
        Material[] MatArr = new Material[3];
        MatArr[0] = ArrowMat;
        MatArr[1] = ArrowBodyMat;
        MatArr[2] = ArrowMat;

        CurType = Type;
        ArrowRenderer.materials = MatArr;
        ArrowRenderer.enabled = true;
        IsReady = true;
    }
}