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
    public ItemType Type { get; private set; }
    public bool IsKeyed { get; private set; }
    public Vector2Int GridPos;
    public Vector2Int OriginalGridPos;
    public Vector2Int GridIdxId { get; set; }
    public Vector3 TargetLocalPosition { get; set; }

    protected virtual void Awake()
    {
        TargetLocalPosition = transform.localPosition;
    }

    public virtual void Init(CrowdElementData crowdElement)
    {
        Material mat = ReferenceManager.Instance.PersonMaterials.GetMaterial(crowdElement.Type);

        foreach (var r in Renderers)
        {
            r.sharedMaterial = mat;
        }

        Type = crowdElement.Type;
        IsKeyed = crowdElement.IsKeyed;

        if (IsKeyed)
        {
            var block = new MaterialPropertyBlock();

            foreach (var r in Renderers)
            {
                r.GetPropertyBlock(block);

                block.SetColor("_OutlineColor", ReferenceManager.Instance.KeyedOutlineColor);
                block.SetFloat("_OutlineWidth", ReferenceManager.Instance.KeyedOutlineWidth);

                r.SetPropertyBlock(block);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
#if UNITY_EDITOR
        Handles.Label(transform.position + Vector3.up * 1.5f, GridPos.x + " , " + GridPos.y);
#endif
    }
}