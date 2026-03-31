using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class FromTo<T>
{
    public T From;
    public T To;
}

public class Portal : Singleton<Portal>
{
    [Header("SETTINGS")]
    public float PortalDuration = 5f;
    public FromTo<float> PortalZPos;
    public FromTo<Vector3> PortalAreaScale;


    [Header("REFERENCES")]
    public Transform Poles;
    public Transform PortalArea;
    public BoxCollider ColliderObj;

    private bool IsPortalOpen = true;
    private Tween ScalingTween = null;
    private Vector3 OrgScale;
    private bool IsScalingAnimating = false;

    private void Start()
    {
        OrgScale = transform.localScale;
        ClosePortal();
    }

    [ContextMenu("Open Portal")]
    public void OpenPortal(float? PortalDuration = null)
    {
        if(IsPortalOpen) return;

        PortalDuration ??= this.PortalDuration;
        PortalArea.gameObject.SetActive(true);
        Poles.gameObject.SetActive(true);
        Poles.DOLocalMoveZ(PortalZPos.To, 0.5f).SetEase(Ease.InOutSine);
        PortalArea.DOScale(PortalAreaScale.To, 0.5f).SetEase(Ease.InOutSine);
        DOVirtual.DelayedCall(PortalDuration.Value, ClosePortal);
        IsPortalOpen = true;
        ColliderObj.enabled = true;
    }

    [ContextMenu("Close Portal")]
    private void ClosePortal()
    {
        if(!IsPortalOpen) return;
        
        Poles.DOLocalMoveZ(PortalZPos.From, 0.5f).SetEase(Ease.InOutSine);
        PortalArea.DOScale(PortalAreaScale.From, 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            PortalArea.gameObject.SetActive(false);
            Poles.gameObject.SetActive(false);
        });

        IsPortalOpen = false;
        ColliderObj.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Arrow"))
        {
            Debug.Log("Arrow Hit Portal");
            if(!other.TryGetComponent(out Arrow arrow)) return;
            if(arrow.ClonedBy != null) return;

            if(!IsScalingAnimating)
            transform.DOScale(OrgScale * 1.05f, 0.08f).SetEase(Ease.OutBounce).SetLoops(2, LoopType.Yoyo).OnStart(() => IsScalingAnimating = true).OnComplete(() => IsScalingAnimating = false);

            List<CrowdElement> FrontRow = CrowdManager.Instance.CurFront;
            foreach(CrowdElement elem in FrontRow)
            {
                if (elem == null) continue;
                if (elem is Person person)
                {
                    if(person.Type == arrow.Type && (!person.AlreadyTarget || person is Giant) && !person.IsWalking)
                    {
                        person.AlreadyTarget = true;

                        Arrow ClonedArrow = Instantiate(arrow, arrow.transform.position, arrow.transform.rotation);
                        ClonedArrow.ClonedBy = arrow;

                        ClonedArrow.Init(arrow.Type, person.transform, () => 
                        {
                            person.Damage();
                            Destroy(ClonedArrow.gameObject);
                            
                            if (person.RequiredHits <= 0)
                            {
                                CrowdManager.Instance.RemoveCrowdElement(elem);
                            }
                            else
                            {
                                person.AlreadyTarget = false;
                            }
                        }, UnityEngine.Random.Range(0,10) <= 6);

                        break;
                    }    
                }
            }
        }
    }
}