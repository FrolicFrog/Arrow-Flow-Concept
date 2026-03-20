using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Belt : MonoBehaviour, IClickable
{
    [Header("SETTINGS")]
    public Vector3 IncreasedScale;
    public Vector3 IncreasedPosition;

    [Header("REFERENCES")]
    public Transform MeshTransform;

    public event Action OnClicked;

    public void OnClick()
    {
        Debug.Log("Belt clicked!");
        OnClicked?.Invoke();
    }

    [ContextMenu("Increase Capacity")]
    public void IncreaseCapacity()
    {
        MeshTransform.DOScale(IncreasedScale, 0.5f);
        MeshTransform.DOLocalMove(IncreasedPosition, 0.5f);
    }
}
