using System;
using UnityEngine;

namespace ArrowFlow.Types
{
    [Serializable]
    public class LevelFeature
    {
        [Range(1, 100)] public int StartLevelNum;
        [Range(1, 100)] public int EndLevelNum;
        public Sprite Graphic;
        public Sprite FeatureUIGraphic;
        public float ProgressPercent(int CurLvl)
        {
            if (EndLevelNum <= StartLevelNum)
            {
                return CurLvl >= EndLevelNum ? 1f : 0f;
            }

            float progress = (CurLvl - StartLevelNum + 1) / (float)(EndLevelNum - StartLevelNum + 1);
            return Mathf.Clamp01(progress);
        }
    }
}