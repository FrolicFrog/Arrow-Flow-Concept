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
    public Dictionary<Vector2Int, CrowdElement> CrowdElementsDict => CrowdElements;

    public List<CrowdElement> CurFront => GetFrontRow();
    public CrowdElement GetElementByGridIdx(Vector2Int GridIdx) => CrowdElements.TryGetValue(GridIdx, out CrowdElement element) ? element : null;
    public void RegisterElement(Vector2Int GridIdx, CrowdElement Element) => CrowdElements.Add(GridIdx, Element);

    public event Action<CrowdElement> OnCrowdPersonKilled;

    private List<CrowdElement> GetFrontRow()
    {
        List<CrowdElement> frontRow = new();

        var columns = CrowdElements.Values.GroupBy(e => e.GridPos.x).OrderBy(g => g.Key);

        foreach (var column in columns)
        {
            var frontEle = column.Where(e => !(e is Person p && p.AlreadyTarget && p is not Giant))
                .OrderByDescending(e => e.GridPos.y)
                .FirstOrDefault();

            if (frontEle != null)
            {
                frontRow.Add(frontEle);
            }
        }

        return frontRow.OrderByDescending(e => e.OriginalGridPos.y).ToList();
    }

    public void RemoveCrowdElement(CrowdElement Element)
    {
        var keyPair = CrowdElements.FirstOrDefault(x => x.Value == Element);
        if (keyPair.Value == null) return;

        CrowdElements.Remove(keyPair.Key);
        OnCrowdPersonKilled?.Invoke(Element);
        AdjustCrowd(Element);
    }


    private void AdjustCrowd(CrowdElement element)
    {
        int[] xOffsets = { 0, 1, -1 };

        for (int yCheck = element.GridPos.y - 1; yCheck >= 0; yCheck--)
        {
            bool found = false;
            foreach (int xOff in xOffsets)
            {
                Vector2Int checkPos = new Vector2Int(element.GridPos.x + xOff, yCheck);

                if (CrowdElements.TryGetValue(checkPos, out CrowdElement crowdElement))
                {
                    CrowdElements.Remove(checkPos);
                    AdjustCrowd(crowdElement);

                    crowdElement.GridPos = element.GridPos;
                    CrowdElements[element.GridPos] = crowdElement;

                    Vector3 targetPos = element.TargetLocalPosition;
                    crowdElement.TargetLocalPosition = targetPos;

                    if (crowdElement is Person person)
                    {
                        person.transform.DOLocalMove(targetPos, 0.5f)
                        .OnStart(() => person.IsWalking = true)
                        .OnComplete(() => person.IsWalking = false);
                    }

                    found = true;
                    break;
                }
            }
            if (found) break;
        }
    }

    public int GetElementRowIdx(Person person)
    {
        int maxY = int.MinValue;

        foreach (var kvp in CrowdElements)
            maxY = Mathf.Max(maxY, kvp.Key.y);

        foreach (var kvp in CrowdElements)
        {
            if (kvp.Value == person)
            {
                return maxY - kvp.Key.y;
            }
        }

        return -1;
    }
}
