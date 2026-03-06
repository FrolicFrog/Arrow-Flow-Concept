using System;
using ArrowFlowGame.Types;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class CrowdElement : MonoBehaviour
{
    [Header("REFERENCES")]
    public Renderer[] Renderers;
    public ItemType Type {get; private set;}
    public bool IsKeyed {get; private set;}
    public Vector2Int GridPos;
    public Vector2Int GridIdxId => new Vector2Int(GridPos.y, GridPos.x);

    public void Init(CrowdElementData crowdElement)
    {
        Material Mat = ReferenceManager.Instance.ItemMats.GetMaterial(crowdElement.Type);
        Array.ForEach(Renderers, r => r.material = Mat);
        Type = crowdElement.Type;
        IsKeyed = crowdElement.IsKeyed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
#if UNITY_EDITOR
        Handles.Label(transform.position + Vector3.up * 1.5f, GridPos.x + " , " + GridPos.y);
#endif
    }
}