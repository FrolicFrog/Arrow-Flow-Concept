using System;
using UnityEngine;
using ArrowFlow.Types;
using DG.Tweening;
using System.Collections.Generic;
using ArrowFlowGame.Types;
using System.Linq;

public class PowerupManager : Singleton<PowerupManager>
{
    public Powerup[] Powerups;
    public bool IsTakingSpawnerInputForExchangePowerup = false;
    private List<Spawner> ToExchange = null;

    public void Initialize()
    {
        int CurNum = LevelManager.Instance.CurrentLevelNumber;
        Array.ForEach(Powerups, P => P.Initialize(CurNum));
    }

    public void UsePowerup(PowerupType type, bool ShowTutorial = false, string Message = null)
    {
        if (type == PowerupType.BELTCAPACITY)
        {
            if(BeltManager.Instance.TotalSockets < 100)
            UseBeltCapacityPowerup(ShowTutorial && !TutorialAlreadyShown(type), Message);
            else
            {
                PostProcessingManager.Instance.AnimateDimmedExposure();
                GameManager.Instance.GlobalInputEnabled = false;
                UIManager.Instance.ShowHintBox("Belt capacity is already at maximum!", true);
                DOVirtual.DelayedCall(1f, () =>
                {
                    UIManager.Instance.DismissHintBox();
                    PostProcessingManager.Instance.AnimateNormalExposure();
                    GameManager.Instance.GlobalInputEnabled = true;
                });
            }
        }
        else if (type == PowerupType.MULTIPLIER)
        {
            UseMultiplierPowerup(ShowTutorial && !TutorialAlreadyShown(type), Message);
        }
        else if (type == PowerupType.EXCHANGE)
        {
            UseExchangePowerup(ShowTutorial && !TutorialAlreadyShown(type), Message);
        }
    }

    public static bool TutorialAlreadyShown(PowerupType type)
    {
        return PlayerPrefs.GetInt("Tutorial_" + type.ToString(), 0) == 1;
    }

    private void UseExchangePowerup(bool showTutorial, string message)
    {
        ReferenceManager.Instance.Cameras.DOMove(ReferenceManager.Instance.CameraExchangedFocusPos, 0.2f);
        foreach (Spawner V in ReferenceManager.Instance.IdToSpawner.Values)
        {
            if (V == null) continue;
            Utilities.AssignLayerRecursively(V.transform, TutorialManager.Instance.NoPostProcessLayerIdx);
        }

        GameManager.Instance.GlobalInputEnabled = false;
        PostProcessingManager.Instance.AnimateDimmedExposure();
        ToExchange = new List<Spawner>(2);
        IsTakingSpawnerInputForExchangePowerup = true;

        if (showTutorial)
        {
            SetTutorialShown(PowerupType.EXCHANGE);
            UIManager.Instance.ShowHintBox(message);
            if (LevelManager.Instance.Rows.Length < 2)
            {
                Debug.LogError("Need at least two columns to make the tutorial work");
                return;
            }

            VisualRows Row1 = LevelManager.Instance.Rows[0];
            VisualRows Row2 = LevelManager.Instance.Rows[1];

            Spawner S1 = null;
            List<Item> ItemList1 = Row1.ToList();
            for (int i = 0; i < ItemList1.Count; i++)
            {
                var spawner = ItemList1[i] as Spawner;
                if (spawner == null) continue;

                spawner.CanTakeSecondaryActionInput = i == 0;
                if (i == 0)
                    S1 = spawner;
            }

            Spawner S2 = null;
            List<Item> ItemList2 = Row2.ToList();
            for (int i = 0; i < ItemList2.Count; i++)
            {
                var spawner = ItemList2[i] as Spawner;
                if (spawner == null) continue;

                spawner.CanTakeSecondaryActionInput = i == 1;
                if (i == 1)
                    S2 = spawner;
            }

            if (S1 == null || S2 == null)
            {
                Debug.LogError("Invalid tutorial spawners");
                return;
            }
            
            S2.CanTakeSecondaryActionInput = false;
            SetupFlowActionListeners(S1, S2);
        }
    }

    private void SetupFlowActionListeners(Spawner s1, Spawner s2)
    {
        s1.SetFingerAnimationVisible(true);

        System.Action s1Handler = null;
        s1Handler = () =>
        {
            s1.OnSecondaryActionClick -= s1Handler;

            s1.SetFingerAnimationVisible(false);
            s2.SetFingerAnimationVisible(true);
            s2.CanTakeSecondaryActionInput = true;
            
            System.Action s2Handler = null;
            s2Handler = () =>
            {
                s2.OnSecondaryActionClick -= s2Handler;
                s2.SetFingerAnimationVisible(false);

                Sequence Seq = ExchangeSpawners(ToExchange[0], ToExchange[1]);
                IsTakingSpawnerInputForExchangePowerup = false;
                ToExchange = null;
                UIManager.Instance.DismissHintBox();
                PostProcessingManager.Instance.AnimateNormalExposure();
                GameManager.Instance.GlobalInputEnabled = true;

                foreach (Spawner V in ReferenceManager.Instance.IdToSpawner.Values)
                {
                    if (V == null) continue;
                    Utilities.AssignLayerRecursively(V.transform, 0);
                }

                Seq.OnComplete(() =>
                {
                    ReferenceManager.Instance.Cameras.DOMove(ReferenceManager.Instance.CameraOriginalPos, 0.2f);
                    EnableAllPowerups();
                });
            };

            s2.OnSecondaryActionClick += s2Handler;
        };

        s1.OnSecondaryActionClick += s1Handler;
    }

    public void AddSpawnerToExchange(Spawner s)
    {
        if (ToExchange == null)
        {
            Debug.LogWarning("NOT USING EXCHANGE POWERUP CURRENTLY");
            return;
        }

        if (ToExchange.Count >= 2)
        {
            Debug.LogWarning("ALREADY 2 SPAWNERS SELECTED");
            return;
        }

        ToExchange.Add(s);
        s.CountLabel.color = Color.cyan;
        s.Renderer.material.SetColor("_Outline_Color", Color.cyan);

        if (ToExchange.Count == 2)
        {
            Sequence Seq = ExchangeSpawners(ToExchange[0], ToExchange[1]);
            IsTakingSpawnerInputForExchangePowerup = false;
            ToExchange = null;
            UIManager.Instance.DismissHintBox();
            PostProcessingManager.Instance.AnimateNormalExposure();
            GameManager.Instance.GlobalInputEnabled = true;

            foreach (Spawner V in ReferenceManager.Instance.IdToSpawner.Values)
            {
                if (V == null) continue;
                Utilities.AssignLayerRecursively(V.transform, 0);
            }

            Seq.OnComplete(() =>
            {
                ReferenceManager.Instance.Cameras.DOMove(ReferenceManager.Instance.CameraOriginalPos, 0.2f);
                EnableAllPowerups();
            });
        }
    }
    private Sequence ExchangeSpawners(Spawner spawner1, Spawner spawner2)
    {
        Sequence seq = DOTween.Sequence();

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

        seq.Join(spawner1.transform.DOLocalMove(pos2, 0.4f).SetEase(Ease.InOutBack));
        seq.Join(spawner2.transform.DOLocalMove(pos1, 0.4f).SetEase(Ease.InOutBack));

        spawner1.CountLabel.color = new Color(Color.white.r, Color.white.g, Color.white.b, spawner1.IsAtFront() ? 1f : 0.5f);
        spawner1.Renderer.material.SetColor("_Outline_Color", Color.black);

        spawner2.CountLabel.color = new Color(Color.white.r, Color.white.g, Color.white.b, spawner2.IsAtFront() ? 1f : 0.5f);
        spawner2.Renderer.material.SetColor("_Outline_Color", Color.black);

        return seq;
    }

    private void UseMultiplierPowerup(bool showTutorial, string message)
    {
        if (showTutorial)
        {
            SetTutorialShown(PowerupType.MULTIPLIER);
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
        if (ShowTutorial)
        {
            SetTutorialShown(PowerupType.BELTCAPACITY);
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
        foreach (var p in Powerups)
        {
            p.ActionBtn.interactable = p.type == type;
        }
    }

    public void EnableAllPowerups()
    {
        foreach (var p in Powerups)
        {
            p.ActionBtn.interactable = true;
        }
    }

    private void SetTutorialShown(PowerupType type)
    {
        PlayerPrefs.SetInt("Tutorial_" + type.ToString(), 1);
    }
}