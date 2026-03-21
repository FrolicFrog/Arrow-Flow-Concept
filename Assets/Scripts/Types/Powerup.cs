using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArrowFlow.Types
{
    public enum PowerupType
    {
        MULTIPLIER,
        BELTCAPACITY,
        EXCHANGE
    }

    [Serializable]
    public class Powerup
    {
        public PowerupType type;
        public int UnlocksAt;
        public Sprite EnabledGraphic;
        public Image PowerupGraphic;
        public Button ActionBtn;
        public TextMeshProUGUI LockedLabel;
        public RectTransform Quantity;
        public TextMeshProUGUI QuantityLabel;
        public string PlayerPrefsKey;
        public GameObject FingerAnimation;
        public string HintLabel = "Tap on conveyer to increase it's capacity.";
        public int QuantityOwned
        {
            get => PlayerPrefs.GetInt(PlayerPrefsKey, 2);
            set
            {
                PlayerPrefs.SetInt(PlayerPrefsKey, value);
                QuantityLabel.text = QuantityOwned > 0 ? QuantityOwned.ToString() : "+";
            }
        }

        public void Initialize(int CurrentLvl)
        {
            bool IsUnlocked = CurrentLvl >= UnlocksAt;
            Quantity.gameObject.SetActive(CurrentLvl >= UnlocksAt);
            QuantityLabel.text = QuantityOwned > 0 ? QuantityOwned.ToString() : "+";
            FingerAnimation.SetActive(CurrentLvl == UnlocksAt);

            if(!IsUnlocked)
            {
                ShowLocked();
                return;
            }

            ShowUnlocked();
            ActionBtn.onClick.AddListener(() => 
            {
                PowerupManager.Instance.UsePowerup(type, CurrentLvl == UnlocksAt, HintLabel);
                QuantityOwned = Math.Max(0, QuantityOwned -= 1);
                DisableFingerAnimation();
            });

            if(CurrentLvl == UnlocksAt)
            {
                GameManager.Instance.GlobalInputEnabled = false;
                PowerupManager.Instance.AllowInputForPowerupOnly(type);
                PostProcessingManager.Instance.AnimateDimmedExposure();
            }
        }

        private void ShowUnlocked()
        {
            LockedLabel.text = "";
            PowerupGraphic.sprite = EnabledGraphic;
        }

        private void ShowLocked()
        {
            LockedLabel.text = "LV. " + UnlocksAt;
        }

        private void DisableFingerAnimation()
        {
            FingerAnimation.SetActive(false);
        }
    }    
}