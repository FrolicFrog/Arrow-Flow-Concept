using System;
using System.Collections.Generic;
using ArrowFlowGame.Types;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Splines;
using Unity.VisualScripting;
using UnityEditor;

public class BeltManager : Singleton<BeltManager>
{
    [Header("References")]
    public TextMeshProUGUI CurCapacityText;
    public ArrowSocket ArrowSocketPrefab;
    public SplineContainer SplineContain;
    public Image ProgressBarFill;
    public Belt BeltObj;

    [Header("Settings")]
    [Range(1,90)]
    public int TotalSockets;
    public int CurOccupied;

    public event Action<ArrowSocket> OnSocketOccupied;
    private readonly List<ArrowSocket> Sockets = new();

    public void Initialize()
    {
        TotalSockets = LevelManager.Instance.LevelData.BeltCapacity;
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
        float Offset = (float) 1 / TotalSockets;

        if(Preserve)
        {
           if(TotalSockets <= Sockets.Count)
            {
                if(TotalSockets < Sockets.Count) 
                Debug.Log("Can't preserve sockets because target capacity is less than existing capacity");

                return;
            }

            int CurrentSocketCount = Sockets.Count;
            bool currentSpeedState = CurrentSocketCount > 0 && Sockets[0].UseIncreasedSpeed;
            float NormalizedTime = Sockets[0].SplineAnimator.NormalizedTime;

            for(int i = 0; i < TotalSockets; i++)
            {
                if(i <= CurrentSocketCount - 1)
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
            for(int i = 0; i < TotalSockets; i++)
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

        foreach(ArrowSocket s in Instance.Sockets)
        {
            if(s == null) continue;
            if(s.IsOccupied) continue;

            float d = Vector3.Distance(Pos, s.transform.position);
            if(d < Distance)            
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
        arrowSocket.ArrowRenderer.enabled = false;
        
        CurOccupied--;
        UpdateProgressbar();
    }

    private void UpdateProgressbar()
    {
        Debug.Log("UPDATED PROGESS");


        CurCapacityText.text = $"{CurOccupied}/{TotalSockets}";
        ProgressBarFill.fillAmount = CurOccupied / (float)TotalSockets;
        UIManager.Instance.UpdateDangerVignetteAlpha(ProgressBarFill.fillAmount);
    }

    public void SocketOccupied(ArrowSocket socket)
    {
        OnSocketOccupied?.Invoke(socket);
    }

    public void UpdateSpeed(bool useIncreasedSpeed)
    {
        Debug.Log("UPDATED SPEED");

        foreach(ArrowSocket s in Sockets)
        s.UseIncreasedSpeed = useIncreasedSpeed;
    }

    public void SwitchToLayer(int noPostProcessLayerIdx)
    {
        Debug.Log("UPDATED LAYER");

        Utilities.AssignLayerRecursively(BeltObj.transform, noPostProcessLayerIdx);
    }
}
