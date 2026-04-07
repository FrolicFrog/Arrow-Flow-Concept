using System;
using System.Collections;
using DG.Tweening;
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
        private static WaitForSecondsRealtime _waitForSecondsRealtime10 = new(10);
        public Color DefeaultQuantityBgColor = new Color32(0x25, 0xB9, 0xFA, 0xFF);
        public Image QuantityBackgroundImage;
        public Sprite DefaultQuantityBackgroundImg;
        public Sprite AdIconImg;
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
                QuantityLabel.text = QuantityOwned > 0 ? QuantityOwned.ToString() : "";
                UpdateQuantityLabel();
            }
        }

        private bool IsUnlocked = false;
        private Vector3 OrgScale;
        private MonoBehaviour _Mb;

        public void Initialize(int CurrentLvl, MonoBehaviour Mb)
        {
            _Mb = Mb;
            OrgScale = Quantity.transform.localScale;
            IsUnlocked = CurrentLvl >= UnlocksAt;
            Quantity.gameObject.SetActive(CurrentLvl >= UnlocksAt);
            QuantityLabel.text = QuantityOwned > 0 ? QuantityOwned.ToString() : "";
            FingerAnimation.SetActive(CurrentLvl == UnlocksAt && !PowerupManager.TutorialAlreadyShown(type));

            if (!IsUnlocked)
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

            if (CurrentLvl == UnlocksAt && !PowerupManager.TutorialAlreadyShown(type))
            {
                GameManager.Instance.GlobalInputEnabled = false;
                PowerupManager.Instance.AllowInputForPowerupOnly(type);
                PostProcessingManager.Instance.AnimateDimmedExposure();
            }

            _Mb.StartCoroutine(AnimateIfNotAvailable());
        }

        private IEnumerator AnimateIfNotAvailable()
        {
            while (true)
            {
                if (QuantityOwned == 0 && IsUnlocked)
                    Quantity.transform.DOScale(OrgScale * 1.5f, 0.5f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutBack);

                yield return _waitForSecondsRealtime10;
            }
        }

        private void ShowUnlocked()
        {
            LockedLabel.text = "";
            UpdateQuantityLabel();
        }

        private void UpdateQuantityLabel()
        {
            if(QuantityOwned > 0)
            {
                PowerupGraphic.sprite = EnabledGraphic;
                QuantityBackgroundImage.sprite = DefaultQuantityBackgroundImg;
                QuantityBackgroundImage.color = DefeaultQuantityBgColor;
            }
            else
            {
                QuantityBackgroundImage.sprite = AdIconImg;
                QuantityBackgroundImage.color = Color.white;
            }
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