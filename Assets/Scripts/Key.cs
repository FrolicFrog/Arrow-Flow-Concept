using System;
using DG.Tweening;
using UnityEngine;

public class Key : MonoBehaviour
{
    public Vector3 PosOffset;
    public float ScaleMult;
    public Vector3 EndRot;
    public Vector3 UnlockedRot;
    public float Duration = 1f;

    public void SetActive(bool isKeyed)
    {
        gameObject.SetActive(isKeyed);
    }

    public void Unlock(Lock lockObj)
    {
        Sequence KeyUnlockSeq = DOTween.Sequence();
        KeyUnlockSeq.JoinCallback(() => Destroy(transform.GetChild(0).gameObject));
        KeyUnlockSeq.Join(transform.DORotate(EndRot, Duration).SetEase(Ease.Linear));
        KeyUnlockSeq.Join(transform.DOScale(transform.localScale * ScaleMult, Duration).SetEase(Ease.Linear));
        KeyUnlockSeq.Join(transform.DOMove(lockObj.transform.position + PosOffset, Duration).SetEase(Ease.Linear));
        KeyUnlockSeq.Insert(Duration, transform.DORotate(UnlockedRot, 0.2f).OnComplete(() => 
        {
            lockObj.Unlock();
            gameObject.SetActive(false);
        }));
    }
}