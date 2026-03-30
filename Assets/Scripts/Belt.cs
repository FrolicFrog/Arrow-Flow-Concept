using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public class Belt : MonoBehaviour, IClickable
{
    [Header("SETTINGS")]
    public Vector3 IncreasedScale;
    public Vector3 IncreasedPosition;

    [Header("SPLINE SETTINGS")]
    public int[] KnotIndices;
    public Vector3[] TargetPositions;

    [Header("REFERENCES")]
    public Transform MeshTransform;
    public SplineContainer SplineCon;
    public GameObject FingerAnimation;
    public int Layer => MeshTransform.gameObject.layer;
    public bool ShowFingerAnimation
    {
        get => FingerAnimation.activeSelf;
        set => FingerAnimation.SetActive(value);
    }

    public event Action OnClicked;

    public void OnClick()
    {
        Debug.Log("Belt clicked!");
        transform.DOScale(transform.localScale * 0.95f, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutBack);
        OnClicked?.Invoke();
    }

    [ContextMenu("Increase Capacity")]
    public void IncreaseCapacity()
    {
        transform.DOScale(transform.localScale * 1.05f, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutBack);
        MeshTransform.DOScale(IncreasedScale, 0.5f);
        MeshTransform.DOLocalMove(IncreasedPosition, 0.5f);
        int ToIncrease = (BeltManager.Instance.TotalSockets + 10) > 100 ? (100 - BeltManager.Instance.TotalSockets) : 10;

        BeltManager.Instance.TotalSockets += ToIncrease;
        BeltManager.Instance.InitPreserve();
        
        Spline spline = SplineCon.Spline;

        for (int i = 0; i < KnotIndices.Length; i++)
        {
            int index = KnotIndices[i];
            Vector3 startPos = spline[index].Position;

            DOVirtual.Vector3(startPos, TargetPositions[i], 0.5f, (val) =>
            {
                var knot = spline[index];
                knot.Position = val;
                spline[index] = knot;
            });
        }
    }
}