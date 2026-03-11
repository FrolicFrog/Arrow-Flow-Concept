using System;
using System.Collections.Generic;
using ArrowFlowGame.Types;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Splines;

public class BeltManager : Singleton<BeltManager>
{
    [Header("References")]
    public TextMeshProUGUI CurCapacityText;
    public ArrowSocket ArrowSocketPrefab;
    public SplineContainer SplineContain;
    public Image ProgressBarFill;

    [Header("Settings")]
    [Range(1,90)]
    public int TotalSockets;
    public int CurOccupied;

    public event Action<ArrowSocket> OnSocketOccupied;
    private readonly List<ArrowSocket> Sockets = new();

    private void Start()
    {
        float Offset = (float)1 / TotalSockets;
        for(int i = 0; i < TotalSockets; i++)
        {
            ArrowSocket Socket = ArrowSocket.CreateArrowSocket(ArrowSocketPrefab, SplineContain, Offset * i);
            Sockets.Add(Socket);
        }

        UpdateProgressbar();
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
        CurCapacityText.text = (CurOccupied/(float) TotalSockets * 100f).ToString("F0") + "%";
        ProgressBarFill.fillAmount = CurOccupied / (float)TotalSockets;
        UIManager.Instance.UpdateDangerVignetteAlpha(ProgressBarFill.fillAmount);
    }

    public void SocketOccupied(ArrowSocket socket)
    {
        OnSocketOccupied?.Invoke(socket);
    }

    public void UpdateSpeed(bool useIncreasedSpeed)
    {
        foreach(ArrowSocket s in Sockets)
        s.UseIncreasedSpeed = useIncreasedSpeed;
    }
}
