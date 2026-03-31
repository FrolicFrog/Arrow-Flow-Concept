using System;
using UnityEngine;
using DG.Tweening;

namespace ArrowFlowGame.Types
{   
    [Serializable]
    public class HardLvlDialog
    {
        public GameObject Root;
        public RectTransform Dialog;

        private Tween floatTween;

        public void Init()
        {
            Root.SetActive(false);
        }

        public void Display()
        {
            Root.SetActive(true);
            GameManager.Instance.GlobalInputEnabled = false;
            PostProcessingManager.Instance.AnimateDimmedExposure();
            float screenHeight = Screen.height;

            Vector2 startPos = new Vector2(0, -screenHeight);
            Vector2 midPos = new Vector2(0, 0);
            Vector2 endPos = new Vector2(0, screenHeight);

            Dialog.anchoredPosition = startPos;
            Dialog.localScale = Vector3.zero;

            Sequence seq = DOTween.Sequence();

            seq.Append(Dialog.DOAnchorPos(midPos, 0.6f).SetEase(Ease.OutBack));
            seq.Join(Dialog.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack));

            seq.AppendCallback(() =>
            {
                floatTween = Dialog.DOAnchorPosY(midPos.y + 20f, 1f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });

            seq.AppendInterval(2f);

            seq.AppendCallback(() =>
            {
                if (floatTween != null)
                {
                    floatTween.Kill();
                }
            });

            seq.Append(Dialog.DOAnchorPos(endPos, 0.5f).SetEase(Ease.InCubic));
            seq.Join(Dialog.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));

            seq.OnComplete(() =>
            {
                Root.SetActive(false);
                GameManager.Instance.GlobalInputEnabled = true;
                PostProcessingManager.Instance.AnimateNormalExposure();
            });
        }
    }   
}