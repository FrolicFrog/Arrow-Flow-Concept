using System;
using ArrowFlow.Types;
using DG.Tweening;

public class PowerupManager : Singleton<PowerupManager>
{
    public Powerup[] Powerups;
    private bool UsedBeltCapacityPowerupInLvl = false;

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
    }

    private void UseMultiplierPowerup(bool showTutorial, string message)
    {
        if(showTutorial)
        {
            Utilities.AssignLayerRecursively(Portal.Instance.transform, TutorialManager.Instance.NoPostProcessLayerIdx);
            PostProcessingManager.Instance.AnimateDimmedExposure();
            UIManager.Instance.ShowHintBox(message);
            Portal.Instance.OpenPortal(Portal.Instance.PortalDuration + 3f);
            DOVirtual.DelayedCall(3f, () =>
            {
                UIManager.Instance.DismissHintBox();
                Utilities.AssignLayerRecursively(Portal.Instance.transform, 0);
                PostProcessingManager.Instance.AnimateNormalExposure();
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
            PostProcessingManager.Instance.AnimateDimmedExposure();
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
        BeltManager.Instance.SwitchToLayer(0);
        PostProcessingManager.Instance.AnimateNormalExposure();
        BeltManager.Instance.BeltObj.OnClicked -= OnBeltClickCapacityPowerupTutorial;
    }

    public void AllowInputForPowerupOnly(PowerupType type)
    {
        foreach(var p in Powerups)
        {
            p.ActionBtn.interactable = p.type == type;
        }
    }
}