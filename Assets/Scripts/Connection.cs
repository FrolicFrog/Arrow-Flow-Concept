// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [RequireComponent(typeof(LineRenderer))]
// public class Connection : MonoBehaviour
// {
//     [Header("REFERENCES")]
//     public LineRenderer Line;
//     public Vector3 PosOffset;
//     public Material material
//     {
//         get => Line.material;
//         set => Line.material = value;
//     }

//     private Spawner Source;
//     private Spawner Target;

//     private void Awake()
//     {
//         if (Line == null)
//         {
//             Line = GetComponent<LineRenderer>();
//         }

//         // We use 4 points to prevent Unity from interpolating/blending colors across the length
//         Line.positionCount = 4;
//         Line.useWorldSpace = true;
//     }

//     public void SetTarget(Spawner source, Spawner target)
//     {
//         if (source == null || target == null) return;
//         Source = source;
//         Target = target;

//         Color color1 = Source.ConnectionColor;
//         Color color2 = Target.ConnectionColor;

//         Gradient gradient = new Gradient();
//         gradient.mode = GradientMode.Blend;

//         // Force colors to stay completely solid until exactly the midpoint
//         gradient.SetKeys(
//             new GradientColorKey[] {
//                 new GradientColorKey(color1, 0.0f),
//                 new GradientColorKey(color1, 0.499f),
//                 new GradientColorKey(color2, 0.501f),
//                 new GradientColorKey(color2, 1.0f)
//             },
//             new GradientAlphaKey[] {
//                 new GradientAlphaKey(1.0f, 0.0f),
//                 new GradientAlphaKey(1.0f, 1.0f)
//             }
//         );

//         Line.colorGradient = gradient;
//     }

//     private void OnDisable()
//     {
//         Line.enabled = false;
//     }

//     private void OnEnable()
//     {
//         Line.enabled = true;
//     }

//     private void Update()
//     {
//         if (Source == null || Target == null) return;

//         // Delete (disable) connection if either connected item has run OnComplete()
//         if (Source.HasCompleted || Target.HasCompleted)
//         {
//             Line.enabled = false;
//             this.enabled = false;
//             return;
//         }

//         Vector3 start = Source.transform.position;
//         Vector3 end = Target.transform.position;

//         start.y = 0f;
//         end.y = 0f;

//         start += PosOffset;
//         end += PosOffset;

//         // Create two midpoints extremely close to each other to trap the gradient blend in an invisible gap
//         Vector3 mid1 = Vector3.Lerp(start, end, 0.499f);
//         Vector3 mid2 = Vector3.Lerp(start, end, 0.501f);

//         Line.SetPosition(0, start);
//         Line.SetPosition(1, mid1);
//         Line.SetPosition(2, mid2);
//         Line.SetPosition(3, end);
//     }
// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    [Header("REFERENCES")]
    public Spawner Source;
    public Spawner Target;
    public Vector3 PosOffset;
    public Material MaterialA;
    public Material MaterialB;

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

        Vector3 dir = (end - start);
        float halfLength = dir.magnitude / 2;

        PipeA = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        PipeA.transform.position = (start + mid) / 2;
        PipeA.transform.up = (mid - start).normalized;
        PipeA.transform.localScale = new Vector3(0.5f, halfLength / 2f, 0.5f);
        PipeA.GetComponent<Renderer>().material = MaterialA;

        PipeB = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        PipeB.transform.position = (mid + end) / 2;
        PipeB.transform.up = (end - mid).normalized;
        PipeB.transform.localScale = new Vector3(0.5f, halfLength / 2f, 0.5f);
        PipeB.GetComponent<Renderer>().material = MaterialB;
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

        MaterialA = ReferenceManager.Instance.ItemMats.GetMaterial(Source.Type);
        MaterialB = ReferenceManager.Instance.ItemMats.GetMaterial(Target.Type);
    }
}