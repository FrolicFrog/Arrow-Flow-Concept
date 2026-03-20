using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ArrowFlow.Types
{
    [Serializable]
    public class HintDialog
    {
        public RectTransform Root;
        public RectTransform Dialog;
        public TextMeshProUGUI Label;

        public void Show(string message)
        {
            if(string.IsNullOrEmpty(message)) return;
            
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