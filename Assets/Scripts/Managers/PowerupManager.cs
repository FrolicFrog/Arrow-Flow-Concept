using System;
using UnityEngine;
using ArrowFlow.Types;
using DG.Tweening;
using System.Collections.Generic;
using ArrowFlowGame.Types;
using System.Linq;
using Managers;
using LionStudios.Suite.Ads;

public class PowerupManager : Singleton<PowerupManager>
{
    public Powerup[] Powerups;
    public bool IsTakingSpawnerInputForExchangePowerup = false;
    private List<Spawner> ToExchange = null;

    public void Initialize()
    {
        int CurNum = LevelManager.Instance.CurrentLevelNumber;
        Array.ForEach(Powerups, P => P.Initialize(CurNum, this));
    }

    public void UsePowerup(PowerupType type, bool ShowTutorial = false, string Message = null)
    {
        if (TryShowAdIfNotAvailable(type)) return;

        if (type == PowerupType.BELTCAPACITY)
        {
            if (BeltManager.Instance.TotalSockets < 100)
            {
                UseBeltCapacityPowerup(ShowTutorial && !TutorialAlreadyShown(type), Message);
                AnalyticsManager.PowerupUsed(LevelManager.Instance.CurrentLevelNumber, type.ToString());
            }
            else
            {
                PostProcessingManager.Instance.AnimateDimmedExposure();
                GameManager.Instance.GlobalInputEnabled = false;
                UIManager.Instance.ShowHintBox("Belt capacity is already at maximum!");
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
            AnalyticsManager.PowerupUsed(LevelManager.Instance.CurrentLevelNumber, type.ToString());
        }
        else if (type == PowerupType.EXCHANGE)
        {
            UseExchangePowerup(ShowTutorial && !TutorialAlreadyShown(type), Message);
            AnalyticsManager.PowerupUsed(LevelManager.Instance.CurrentLevelNumber, type.ToString());
        }
    }

    private bool TryShowAdIfNotAvailable(PowerupType type)
    {
        Powerup BeltCapacityPowerup = Powerups.FirstOrDefault(P => P.type == PowerupType.BELTCAPACITY);
        if (BeltCapacityPowerup.QuantityOwned <= 0)
        {
            AnalyticsManager.TryShowRewardedAd("placement", () => BeltCapacityPowerup.QuantityOwned += 1);
            return true;
        }

        return false;
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
            if (LevelManager.Instance.Rows.Length < 2)
            {
                Debug.LogError("Need at least two columns to make the tutorial work");
                return;
            }

            VisualRows Row1 = LevelManager.Instance.Rows[0];
            VisualRows Row2 = LevelManager.Instance.Rows[1];

            Spawner S1 = EnableIndex(Row1.ToList(), 0);
            Spawner S2 = EnableIndex(Row2.ToList(), 1);
            
            if (S1 == null || S2 == null)
            {
                Debug.LogError("Invalid tutorial spawners");
                return;
            }

            foreach(var spawner in Spawner.AllSpawners)
                spawner.Interactable = spawner.Equals(S1);

            S2.CanTakeSecondaryActionInput = false;
            SetupFlowActionListeners(S1, S2);
        }
        else
        {
            foreach(var spawner in Spawner.AllSpawners)
            {
                spawner.Interactable = true;
                spawner.CanTakeSecondaryActionInput = true;
                spawner.CanAddSpawnerForExchange = true;
            }
        }
    }

    private Spawner EnableIndex(List<Item> items, int idx)
    {
        Spawner S = null;
        for (int i = 0; i < items.Count; i++)
        {
            var spawner = items[i] as Spawner;
            if (spawner == null) continue;

            spawner.CanTakeSecondaryActionInput = i == 0;
            if (i == idx) S = spawner;
        }

        return S;
    }

    private void SetupFlowActionListeners(Spawner s1, Spawner s2)
    {
        s1.SetFingerAnimationVisible(true);
        s1.CanAddSpawnerForExchange = false;
        s2.CanAddSpawnerForExchange = false;

        void s1Handler()
        {
            s1.OnSecondaryActionClick -= s1Handler;
            s1.CountLabel.color = Color.cyan;
            s1.Renderer.material.SetColor("_OutlineColor", Color.cyan);

            s1.SetFingerAnimationVisible(false);
            s2.SetFingerAnimationVisible(true);
            s2.CanTakeSecondaryActionInput = true;
            s1.CanTakeSecondaryActionInput = false;

            foreach(var spawner in Spawner.AllSpawners)
                spawner.Interactable = spawner.Equals(s2);

            void s2Handler()
            {
                s2.OnSecondaryActionClick -= s2Handler;
                s2.SetFingerAnimationVisible(false);

                s1.CountLabel.color = Color.black;
                s1.Renderer.material.SetColor("_OutlineColor", Color.black);

                Sequence Seq = ExchangeSpawners(s1, s2);
                IsTakingSpawnerInputForExchangePowerup = false;
                ToExchange = null;
                PostProcessingManager.Instance.AnimateNormalExposure();
                GameManager.Instance.GlobalInputEnabled = true;

                foreach (Spawner V in ReferenceManager.Instance.IdToSpawner.Values)
                {
                    if (V == null) continue;
                    Utilities.AssignLayerRecursively(V.transform, 0);
                }

                foreach(var spawner in Spawner.AllSpawners)
                    spawner.Interactable = true;

                for (int i = 0; i < LevelManager.Instance.Rows.Length; i++)
                {
                    if (i == 0 || i == 1) continue;
                    VisualRows VRs = LevelManager.Instance.Rows[i];
                    List<Item> Items = VRs.ToList();

                    foreach (Item I in Items)
                    {
                        if (I is not Spawner s) continue;
                        s.CanTakeSecondaryActionInput = true;
                    }
                }

                Seq.OnComplete(() =>
                {
                    ReferenceManager.Instance.Cameras.DOMove(ReferenceManager.Instance.CameraOriginalPos, 0.2f);
                    EnableAllPowerups();
                });
            }

            s2.OnSecondaryActionClick += s2Handler;
        }

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
        s.Renderer.material.SetColor("_OutlineColor", Color.cyan);

        if (ToExchange.Count == 2)
        {
            Sequence Seq = ExchangeSpawners(ToExchange[0], ToExchange[1]);
            IsTakingSpawnerInputForExchangePowerup = false;
            ToExchange = null;
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

        spawner1.VisualRowIndex = Idx2;
        spawner2.VisualRowIndex = Idx1;

        Vector3 pos1 = spawner1.transform.localPosition;
        Vector3 pos2 = spawner2.transform.localPosition;

        spawner1.transform.SetParent(Row2.rowsTransform, true);
        spawner2.transform.SetParent(Row1.rowsTransform, true);

        seq.Join(spawner1.transform.DOLocalMove(pos2, 0.4f).SetEase(Ease.InOutBack));
        seq.Join(spawner2.transform.DOLocalMove(pos1, 0.4f).SetEase(Ease.InOutBack));

        spawner1.CountLabel.color = new Color(Color.white.r, Color.white.g, Color.white.b, spawner1.IsAtFront() ? 1f : 0.5f);
        spawner1.Renderer.material.SetColor("_OutlineColor", Color.black);

        spawner2.CountLabel.color = new Color(Color.white.r, Color.white.g, Color.white.b, spawner2.IsAtFront() ? 1f : 0.5f);
        spawner2.Renderer.material.SetColor("_OutlineColor", Color.black);

        return seq;
    }

    private void UseMultiplierPowerup(bool showTutorial, string message)
    {
        PostProcessingManager.Instance.AnimateNormalExposure();
        GameManager.Instance.GlobalInputEnabled = true;
        Portal.Instance.OpenPortal();
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