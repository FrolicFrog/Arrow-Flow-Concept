using System;
using System.Collections.Generic;
using ArrowFlowGame.Types;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Splines;
using Unity.VisualScripting;
using UnityEditor;
using DG.Tweening;

public class BeltManager : Singleton<BeltManager>
{
    [Header("References")]
    public TextMeshProUGUI CurCapacityText;
    public Vector3 LabelOriginalScale;
    public ArrowSocket ArrowSocketPrefab;
    public SplineContainer SplineContain;
    public Image ProgressBarFill;
    public Belt BeltObj;

    [Header("Settings")]
    public Color DangerLabelColor = Color.red;
    public Color NormalLabelColor = Color.white;

    [Range(1, 90)]
    public int TotalSockets;
    public int CurOccupied;
    public float LabelAnimScaleMult = 1.2f;


    public event Action<ArrowSocket> OnSocketOccupied;
    private readonly List<ArrowSocket> Sockets = new();
    private Tween DangerLabelAnim = null;
    public bool OverridingColor { get; set; }

    public void Initialize()
    {
        TotalSockets = LevelManager.Instance.LevelData.BeltCapacity;
        LabelOriginalScale = CurCapacityText.transform.localScale;
        InitializeSockets();
        UpdateProgressbar();
    }

    [ContextMenu("Initialize Sockets")]
    public void InitPreserve()
    {
        InitializeSockets(true, TutorialManager.Instance.NoPostProcessLayerIdx);
        UpdateProgressbar();
    }

    private void InitializeSockets(bool Preserve = false, int layerIdx = 0)
    {
        float Offset = (float)1 / TotalSockets;

        if (Preserve)
        {
            if (TotalSockets <= Sockets.Count)
            {
                if (TotalSockets < Sockets.Count)
                    Debug.Log("Can't preserve sockets because target capacity is less than existing capacity");

                return;
            }

            int CurrentSocketCount = Sockets.Count;
            bool currentSpeedState = CurrentSocketCount > 0 && Sockets[0].UseIncreasedSpeed;
            float NormalizedTime = Sockets[0].SplineAnimator.NormalizedTime;

            for (int i = 0; i < TotalSockets; i++)
            {
                if (i <= CurrentSocketCount - 1)
                {
                    Sockets[i].SplineAnimator.StartOffset = Offset * i;
                    Sockets[i].SplineAnimator.NormalizedTime = NormalizedTime;
                    Sockets[i].SplineAnimator.Restart(false);
                }
                else
                {
                    ArrowSocket Socket = ArrowSocket.CreateArrowSocket(ArrowSocketPrefab, SplineContain, Offset * i, layerIdx);
                    Socket.UseIncreasedSpeed = currentSpeedState;
                    Socket.SplineAnimator.NormalizedTime = NormalizedTime;
                    Sockets.Add(Socket);
                }
            }

            Sockets.ForEach(S => S.SplineAnimator.Restart(true));
        }
        else
        {
            for (int i = 0; i < TotalSockets; i++)
            {
                ArrowSocket Socket = ArrowSocket.CreateArrowSocket(ArrowSocketPrefab, SplineContain, Offset * i, layerIdx);
                Sockets.Add(Socket);
            }
        }
    }

    public static bool TryGetSocket(Vector3 Pos, out ArrowSocket Socket)
    {
        Socket = null;
        float Distance = float.MaxValue;

        foreach (ArrowSocket s in Instance.Sockets)
        {
            if (s == null) continue;
            if (s.IsOccupied) continue;

            float d = Vector3.Distance(Pos, s.transform.position);
            if (d < Distance)
            {
                Distance = d;
                Socket = s;
            }
        }

        return Socket != null;
    }

    public void SetSocketReady(ArrowSocket target, ItemType arrowType)
    {
        target.Ready(arrowType);
        CurOccupied++;
        UpdateProgressbar();
    }

    public void SetSocketEmpty(ArrowSocket arrowSocket)
    {
        arrowSocket.IsReady = false;
        arrowSocket.IsOccupied = false;
        arrowSocket.ArrowObject.SetActive(false);

        CurOccupied--;
        UpdateProgressbar();
    }

    private Vector3 originalScale;

    private void UpdateProgressbar()
    {
        float filledAmount = CurOccupied / (float)TotalSockets;

        if (originalScale == Vector3.zero)
        {
            originalScale = CurCapacityText.transform.localScale;
        }

        if (filledAmount > 0.7f)
        {
            if (DangerLabelAnim == null || !DangerLabelAnim.IsActive())
            {
                DangerLabelAnim = CurCapacityText.transform
                    .DOScale(originalScale * LabelAnimScaleMult, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }
        else
        {
            if (DangerLabelAnim != null && DangerLabelAnim.IsActive())
            {
                DangerLabelAnim.Kill();
                DangerLabelAnim = null;
                CurCapacityText.transform.localScale = originalScale;
            }
        }

        CurCapacityText.text = $"{CurOccupied}/{TotalSockets}";
        if(!OverridingColor)
        CurCapacityText.color = filledAmount > 0.7f ? DangerLabelColor : NormalLabelColor;
        ProgressBarFill.fillAmount = filledAmount;

        PostProcessingManager.Instance.UpdateDangerVignette(ProgressBarFill.fillAmount);
    }

    public void SocketOccupied(ArrowSocket socket)
    {
        OnSocketOccupied?.Invoke(socket);
    }

    public void UpdateSpeed(bool useIncreasedSpeed)
    {
        foreach (ArrowSocket s in Sockets)
            s.UseIncreasedSpeed = useIncreasedSpeed;
    }

    public void SwitchToLayer(int noPostProcessLayerIdx)
    {
        Utilities.AssignLayerRecursively(BeltObj.transform, noPostProcessLayerIdx);
    }

    public void SlowedBeltSpeed()
    {
        foreach(ArrowSocket s in Sockets)
            s.StartSuperSlowMotion();
    }

    public void NormalBeltSpeed()
    {
        foreach (ArrowSocket s in Sockets)
            s.StopSuperSlowMotion();
    }

    public void ClearBeltSockets()
    {
        foreach (ArrowSocket s in Sockets)
        {
            if (s == null) continue;
            Destroy(s.gameObject);
        }

        Sockets.Clear();
    }
}
