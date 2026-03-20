using System;
using System.Linq;
using ArrowFlow.Types;
using UnityEngine;

public class PowerupManager : Singleton<PowerupManager>
{
    public Powerup[] Powerups;
    
    public void Initialize()
    {
        int CurNum = LevelManager.Instance.CurrentLevelNumber;
        Array.ForEach(Powerups, P => P.Initialize(CurNum));
    }

    public void UsePowerup(PowerupType type, bool ShowTutorial = false, string Message = null)
    {
        if(type == PowerupType.BELTCAPACITY)
        {
            UseBeltCapacityPowerup(ShowTutorial, Message);
        }
    }

    public void AllowInputForPowerupOnly(PowerupType type)
    {
        foreach(var p in Powerups)
        {
            p.ActionBtn.interactable = p.type == type;
        }
    }

    private void UseBeltCapacityPowerup(bool ShowTutorial = false, string message = null)
    {
        BeltManager.Instance.SwitchToLayer(TutorialManager.Instance.NoPostProcessLayerIdx);
        PostProcessingManager.Instance.AnimateDimmedExposure();

        if(ShowTutorial)
        {
            UIManager.Instance.ShowHintBox(message);
            BeltManager.Instance.BeltObj.OnClicked += () =>
            {
                Debug.Log("Click detected on belt");
                UIManager.Instance.DismissHintBox();
                BeltManager.Instance.BeltObj.IncreaseCapacity();
            };
        }
    }
}