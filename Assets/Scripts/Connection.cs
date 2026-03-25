using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    [Header("REFERENCES")]
    public Vector3 PosOffset;

    private Spawner Source;
    private Spawner Target;
    private Material MaterialA;
    private Material MaterialB;
    private GameObject PipeA;
    private GameObject PipeB;

    private void Update()
    {
        if (Source == null || Target == null) return;
        if (Source.HasCompleted || Target.HasCompleted)
        {
            DestroyPipe();
            this.enabled = false;
            return;
        }

        Vector3 start = Source.transform.position + PosOffset;
        Vector3 end = Target.transform.position + PosOffset;

        start.y = 1f; // slightly above ground
        end.y = 1f;

        Vector3 mid = (start + end) / 2;

        DestroyPipe();

        Vector3 dir = end - start;
        float halfLength = dir.magnitude / 2;

        PipeA = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        PipeA.transform.position = (start + mid) / 2;
        PipeA.transform.up = (mid - start).normalized;
        PipeA.transform.localScale = new Vector3(0.5f, halfLength / 2f, 0.5f);
        PipeA.GetComponent<Renderer>().material = Source.IsMysteriousCurrently ? ReferenceManager.Instance.MysteriousSpawnerMat : MaterialA;

        PipeB = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        PipeB.transform.position = (mid + end) / 2;
        PipeB.transform.up = (end - mid).normalized;
        PipeB.transform.localScale = new Vector3(0.5f, halfLength / 2f, 0.5f);
        PipeB.GetComponent<Renderer>().material = Target.IsMysteriousCurrently ? ReferenceManager.Instance.MysteriousSpawnerMat : MaterialB;
    }

    private void DestroyPipe()
    {
        if (PipeA != null) Destroy(PipeA);
        if (PipeB != null) Destroy(PipeB);
    }

    public void SetTarget(Spawner source, Spawner target)
    {
        Source = source;
        Target = target;

        MaterialA = ReferenceManager.Instance.SpawnerMaterials.GetMaterial(Source.Type);
        MaterialB = ReferenceManager.Instance.SpawnerMaterials.GetMaterial(Target.Type);
    }
}