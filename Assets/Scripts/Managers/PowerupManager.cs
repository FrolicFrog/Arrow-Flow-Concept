using System;
using UnityEngine;
using ArrowFlow.Types;
using DG.Tweening;
using System.Collections.Generic;
using ArrowFlowGame.Types;

public class PowerupManager : Singleton<PowerupManager>
{
    public Powerup[] Powerups;
    private bool UsedBeltCapacityPowerupInLvl = false;
    public bool IsTakingSpawnerInputForExchangePowerup = false;
    private List<Spawner> ToExchange = null;

    public void Initialize()
    {
        int CurNum = LevelManager.Instance.CurrentLevelNumber;
        Array.ForEach(Powerups, P => P.Initialize(CurNum));
    }

    public void UsePowerup(PowerupType type, bool ShowTutorial = false, string Message = null)
    {
        if(type == PowerupType.BELTCAPACITY && !UsedBeltCapacityPowerupInLvl)
        {
            UseBeltCapacityPowerup(ShowTutorial, Message);
            UsedBeltCapacityPowerupInLvl = true;
        }
        else if(type == PowerupType.MULTIPLIER)
        {
            UseMultiplierPowerup(ShowTutorial, Message);
        }
        else if(type == PowerupType.EXCHANGE)
        {
            UseExchangePowerup(ShowTutorial, Message);  
        }
    }

    private void UseExchangePowerup(bool showTutorial, string message)
    {
        ReferenceManager.Instance.Cameras.DOMove(ReferenceManager.Instance.CameraExchangedFocusPos, 0.2f);
        foreach(Spawner V in ReferenceManager.Instance.IdToSpawner.Values)
        {
            if(V == null) continue;
            Utilities.AssignLayerRecursively(V.transform, TutorialManager.Instance.NoPostProcessLayerIdx);
        }
        GameManager.Instance.GlobalInputEnabled = false;
        PostProcessingManager.Instance.AnimateDimmedExposure();
        ToExchange = new List<Spawner>(2);
        IsTakingSpawnerInputForExchangePowerup = true;

        if(showTutorial)
        {
            UIManager.Instance.ShowHintBox(message);
        }
    }

    public void AddSpawnerToExchange(Spawner s)
    {
        if(ToExchange == null)
        {
            Debug.LogWarning("NOT USING EXCHANGE POWERUP CURRENTLY");
            return;
        }

        if(ToExchange.Count >= 2)
        {
            Debug.LogWarning("ALREADY 2 SPAWNERS SELECTED");
            return;
        }

        ToExchange.Add(s);
        s.CountLabel.color = Color.cyan;
        if(ToExchange.Count == 2)
        {
            ExchangeSpawners(ToExchange[0], ToExchange[1]);
            IsTakingSpawnerInputForExchangePowerup = false;
            ToExchange = null;
            UIManager.Instance.DismissHintBox();
            PostProcessingManager.Instance.AnimateNormalExposure();
            GameManager.Instance.GlobalInputEnabled = true;

            foreach(Spawner V in ReferenceManager.Instance.IdToSpawner.Values)
            {
                if(V == null) continue;
                Utilities.AssignLayerRecursively(V.transform, 0);
            }

            ReferenceManager.Instance.Cameras.DOMove(ReferenceManager.Instance.CameraOriginalPos, 0.2f);
            EnableAllPowerups();
        }
    }

    private void ExchangeSpawners(Spawner spawner1, Spawner spawner2)
    {
        VisualRows Row1 = spawner1.Row;
        int Idx1 = spawner1.VisualRowIndex;
        
        VisualRows Row2 = spawner2.Row;
        int Idx2 = spawner2.VisualRowIndex;

        Row1.SetItem(spawner2, Idx1);
        Row2.SetItem(spawner1, Idx2);

        spawner1.Row = Row2;
        spawner2.Row = Row1;

        Vector3 pos1 = spawner1.transform.localPosition;
        Vector3 pos2 = spawner2.transform.localPosition;

        spawner1.transform.SetParent(Row2.rowsTransform, true);
        spawner2.transform.SetParent(Row1.rowsTransform, true);

        spawner1.transform.DOLocalMove(pos2, 0.2f);
        spawner2.transform.DOLocalMove(pos1, 0.2f);
        spawner1.CountLabel.color = Color.white;
        spawner2.CountLabel.color = Color.white;
    }

    private void UseMultiplierPowerup(bool showTutorial, string message)
    {
        if(showTutorial)
        {
            Utilities.AssignLayerRecursively(Portal.Instance.transform, TutorialManager.Instance.NoPostProcessLayerIdx);
            PostProcessingManager.Instance.AnimateDimmedExposure();
            GameManager.Instance.GlobalInputEnabled = false;
            UIManager.Instance.ShowHintBox(message);
            Portal.Instance.OpenPortal(Portal.Instance.PortalDuration + 3f);
            DOVirtual.DelayedCall(3f, () =>
            {
                UIManager.Instance.DismissHintBox();
                Utilities.AssignLayerRecursively(Portal.Instance.transform, 0);
                GameManager.Instance.GlobalInputEnabled = true;
                PostProcessingManager.Instance.AnimateNormalExposure();
                EnableAllPowerups();            
            });
        }
        else
        {
            Portal.Instance.OpenPortal();
        }
    }

    private void UseBeltCapacityPowerup(bool ShowTutorial = false, string message = null)
    {
        if(ShowTutorial)
        {
            BeltManager.Instance.SwitchToLayer(TutorialManager.Instance.NoPostProcessLayerIdx);
            BeltManager.Instance.BeltObj.ShowFingerAnimation = true;
            PostProcessingManager.Instance.AnimateDimmedExposure();
            GameManager.Instance.GlobalInputEnabled = false;
            UIManager.Instance.ShowHintBox(message);
            BeltManager.Instance.BeltObj.OnClicked += OnBeltClickCapacityPowerupTutorial;
        }
        else
        {
            OnBeltClickCapacityPowerupTutorial();
        }
    }

    private void OnBeltClickCapacityPowerupTutorial()
    {
        UIManager.Instance.DismissHintBox();
        EffectManager.Instance.Play("belt-capacity");
        BeltManager.Instance.BeltObj.IncreaseCapacity();
        BeltManager.Instance.BeltObj.ShowFingerAnimation = false;
        BeltManager.Instance.SwitchToLayer(0);
        PostProcessingManager.Instance.AnimateNormalExposure();
        GameManager.Instance.GlobalInputEnabled = true;
        EnableAllPowerups();
        BeltManager.Instance.BeltObj.OnClicked -= OnBeltClickCapacityPowerupTutorial;
    }

    public void AllowInputForPowerupOnly(PowerupType type)
    {
        foreach(var p in Powerups)
        {
            p.ActionBtn.interactable = p.type == type;
        }
    }
    
    public void EnableAllPowerups()
    {
        foreach(var p in Powerups)
        {
            p.ActionBtn.interactable = true;
        }
    }
}