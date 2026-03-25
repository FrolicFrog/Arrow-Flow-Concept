using System;
using UnityEngine;

namespace ArrowFlow.Types
{
    [Serializable]
    public class LevelFeature
    {
        [Range(1,100)] public int StartLevelNum;
        [Range(1,100)] public int EndLevelNum;
        public Sprite Graphic;
        public Sprite FeatureUIGraphic;
        public float ProgressPercent(int CurLvl) => Mathf.Clamp01((CurLvl - StartLevelNum) / (float)(EndLevelNum - StartLevelNum));
    }
}