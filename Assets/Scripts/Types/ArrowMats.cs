using System;
using UnityEngine;

namespace ArrowFlowGame.Types
{
    [Serializable]
    public class ArrowMats
    {
        public ItemMaterials BodyMaterials;
        public ItemMaterials TrailMaterials;
        public ItemMaterials TipBackMaterials;

        public Material[] GetArrowMatArray(ItemType type)
        {
            Material[] MatArr = new Material[3];
            MatArr[0] = TipBackMaterials.GetMaterial(type);
            MatArr[1] = BodyMaterials.GetMaterial(type);
            MatArr[2] = TipBackMaterials.GetMaterial(type);

            return MatArr;
        }
    }
}