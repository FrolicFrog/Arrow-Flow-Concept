using System;
using System.Collections.Generic;
using ArrowFlowGame.Types;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;

public class BeltManager : Singleton<BeltManager>
{
    [Header("References")]
    public TextMeshPro CurCapacityText;
    public ArrowSocket ArrowSocketPrefab;
    public SplineContainer SplineContain;

    [Header("Settings")]
    [Range(1,90)]
    public int TotalSockets;
    public int CurOccupied;


    private readonly List<ArrowSocket> Sockets = new();

    private void Start()
    {
        float Offset = (float)1 / TotalSockets;
        for(int i = 0; i < TotalSockets; i++)
        {
            ArrowSocket Socket = ArrowSocket.CreateArrowSocket(ArrowSocketPrefab, SplineContain, Offset * i);
            Sockets.Add(Socket);
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
        CurCapacityText.text = CurOccupied.ToString() + "/" + TotalSockets.ToString();
    }

    public void SetSocketEmpty(ArrowSocket arrowSocket)
    {
        arrowSocket.IsReady = false;
        arrowSocket.IsOccupied = false;
        arrowSocket.ArrowRenderer.enabled = false;
        
        CurOccupied--;
        CurCapacityText.text = CurOccupied.ToString() + "/" + TotalSockets.ToString();
    }
}
