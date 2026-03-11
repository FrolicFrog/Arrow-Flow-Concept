using UnityEngine;
using ArrowFlowGame.Types;
using System;
using DG.Tweening;

public class Item : MonoBehaviour
{
    [Header("ANIMATION SETTINGS")]
    public float Elevation = 1f;
    public float ElevationAnimDur = 1f;
    public float BreathScaleUp = 1.05f;
    public float MinBreathingAnimDur = 0.5f;
    public float MaxBreathingAnimDur = 2f;


    protected VisualRows Row;
    protected string Id;
    protected Action<Item> OnItemUsed = null;
    
    private Tween BreathingTween = null;

    public virtual void Init(ItemData data, VisualRows Row, Action<Item> OnItemUsed)
    {
        Id = data.Id;
        this.Row = Row;
        this.OnItemUsed = OnItemUsed;
        BreathingTween = transform.DOScale(transform.localScale * BreathScaleUp, UnityEngine.Random.Range(MinBreathingAnimDur, MaxBreathingAnimDur + 1)).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    protected virtual void OnComplete()
    {
        Row.Dequeue();
        OnItemUsed?.Invoke(this);
        BreathingTween.Kill();
        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform.DOMoveY(transform.position.y + Elevation, ElevationAnimDur));
        sequence.Join(transform.DORotate(new Vector3(0, 360, 0), ElevationAnimDur, RotateMode.FastBeyond360).SetRelative());
        sequence.InsertCallback(ElevationAnimDur * 0.6f, () => Row.MoveToNext());
        sequence.Insert(ElevationAnimDur * 0.6f, transform.DOScale(Vector3.zero, ElevationAnimDur * 0.4f).SetEase(Ease.InBack));
        sequence.InsertCallback(ElevationAnimDur, () => Destroy(gameObject));
        sequence.Play();
    }
}