using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArrowFlow.Types
{
    [Serializable]
    public class HintDialog
    {
        public RectTransform Root;
        public RectTransform Dialog;
        public Image Graphics;
        public TextMeshProUGUI Label;

        public void Show(string message, bool disableDialogGraphics = false)
        {
            if(string.IsNullOrEmpty(message)) return;

            Graphics.enabled = !disableDialogGraphics;
            Label.text = message;
            Dialog.localScale = Vector3.zero;
            Root.gameObject.SetActive(true);
            Dialog.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutBack);
        }

        public void Hide()
        {
            Root.gameObject.SetActive(false);
        }
    }
}