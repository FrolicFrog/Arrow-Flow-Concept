using UnityEngine;
using ArrowFlowGame.Types;
using System;
using DG.Tweening;

public class Item : MonoBehaviour
{
    [Header("ANIMATION SETTINGS")]
    public ParticleSystem DespawnEffect;
    public float DespawnEffectScale = 1f;
    public float Elevation = 1f;
    public float ElevationAnimDur = 1f;
    public float BreathScaleUp = 1.05f;
    public float MinBreathingAnimDur = 0.5f;
    public float MaxBreathingAnimDur = 2f;

    public int VisualRowIndex = -1;
    public bool IsAtFrontBool = false;
    public VisualRows Row;
    protected Vector2Int Id;
    protected Action<Item> OnItemUsed = null;
    
    private Tween BreathingTween = null;
    public bool IsBeingDestroy = false;

    public virtual void Init(ItemData data, VisualRows Row, Action<Item> OnItemUsed)
    {
        Id = data.Id;
        this.Row = Row;
        this.OnItemUsed = OnItemUsed;
        BreathingTween = transform.DOScale(transform.localScale * BreathScaleUp, UnityEngine.Random.Range(MinBreathingAnimDur, MaxBreathingAnimDur + 1)).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }   

    private void Update()
    {
        VisualRowIndex = Row.IndexOf(this);
        IsAtFrontBool = IsAtFront();
    }

    public bool IsAtFront()
    {
        return Row.FrontItem == this;
    }

    protected virtual void OnComplete()
    {
        int index = Row.IndexOf(this);
        Row.Remove(this); // Replacing Dequeue() ensures it removes exactly this item
        
        OnItemUsed?.Invoke(this);
        IsBeingDestroy = true;
        BreathingTween.Kill();
        
        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform.DOMoveY(transform.position.y + Elevation, ElevationAnimDur));
        sequence.Join(transform.DORotate(new Vector3(0, 360, 0), ElevationAnimDur, RotateMode.FastBeyond360).SetRelative());
        
        sequence.InsertCallback(ElevationAnimDur * 0.6f, () => 
        {
            if (index == 0)
                Row.MoveToNext();
            else if (index > 0)
                Row.ShiftItemsForward(index);
        });
        
        sequence.InsertCallback(ElevationAnimDur * 0.8f, () => 
        {
            GameObject effect = Instantiate(DespawnEffect.gameObject, transform.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * DespawnEffectScale;
        });
        sequence.Insert(ElevationAnimDur * 0.6f, transform.DOScale(Vector3.zero, ElevationAnimDur * 0.4f).SetEase(Ease.InBack));
        sequence.InsertCallback(ElevationAnimDur, () => Destroy(gameObject));
        sequence.Play();
    }

    public virtual void OnMoveForward() { }
}