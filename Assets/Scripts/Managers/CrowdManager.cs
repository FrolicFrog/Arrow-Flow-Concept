using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class CrowdManager : Singleton<CrowdManager>
{
    [TextArea(3, 5)]
    public string DebugText;
    private readonly Dictionary<Vector2Int, CrowdElement> CrowdElements = new();
    private List<List<CrowdElement>> CrowdGrid = new();

    public List<CrowdElement> CurFront => GetFrontRow();
    public CrowdElement GetElementByGridIdx(Vector2Int GridIdx) => CrowdElements.TryGetValue(GridIdx, out CrowdElement element) ? element : null;
    public void RegisterElement(Vector2Int GridIdx, CrowdElement Element) => CrowdElements.Add(GridIdx, Element);

    public void RegisterGrid(List<List<CrowdElement>> crowdGrid)
    {
        CrowdGrid = new();

        foreach (List<CrowdElement> row in crowdGrid)
            CrowdGrid.Add(row.Reverse<CrowdElement>().ToList());
    }

    private List<CrowdElement> GetFrontRow()
    {
        List<CrowdElement> frontRow = new();
        foreach (List<CrowdElement> row in CrowdGrid)
        {
            foreach (var ele in row)
            {
                if (ele == null) continue;
                frontRow.Add(ele);
                break;
            }
        }

        return frontRow;
    }

    public void RemoveCrowdElement(CrowdElement Element)
    {
        if(Element.IsKeyed)
        {
            UnlockItemByKeyId(Element.GridIdxId);    
        }

        foreach (List<CrowdElement> row in CrowdGrid)
        {
            for (int i = 0; i < row.Count; i++)
            {
                if (row[i] == Element)
                {
                    row[i] = null;
                    AdjustCrowd(Element);
                    return;
                }
            }
        }
    }

    private void UnlockItemByKeyId(Vector2Int gridPos)
    {
        if(!ReferenceManager.Instance.KeyIdToLockedItem.TryGetValue(gridPos, out Lock LockObject)) 
        {
            Debug.LogError("No locked item found at position " + gridPos);
            return;
        }

        LockObject.Unlock();
    }

    private void AdjustCrowd(CrowdElement element)
    {
        Vector2Int[] directions = { new(0, -1), new(1, -1) };

        foreach (var dir in directions)
        {
            Vector2Int checkPos = element.GridPos + dir;

            if (CrowdElements.TryGetValue(checkPos, out CrowdElement crowdElement))
            {
                CrowdElements.Remove(checkPos);
                AdjustCrowd(crowdElement);

                crowdElement.GridPos = element.GridPos;
                CrowdElements[element.GridPos] = crowdElement; 

                if(crowdElement is Person person)
                {
                    person.transform.DOLocalMove(element.transform.localPosition, 0.5f)
                    .OnStart(() => person.IsWalking = true)
                    .OnComplete(() => person.IsWalking = false);
                }

                break;
            }
        }
    }
}