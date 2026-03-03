using UnityEngine;
using System;

namespace ArrowFlowGame.Types
{
    [Serializable]
    public class ItemMaterials
    {
        public Material Red;
        public Material Green;
        public Material DarkGreen;
        public Material Blue;
        public Material White;
        public Material Orange;
        public Material Cyan;
        public Material Pink;

        public Material GetMaterial(ItemType type)
        {
            switch (type)
            {
                case ItemType.RED:
                    return Red;
                case ItemType.GREEN:
                    return Green;
                case ItemType.DARKGREEN:
                    return DarkGreen;
                case ItemType.BLUE:
                    return Blue;
                case ItemType.WHITE:
                    return White;
                case ItemType.ORANGE:
                    return Orange;
                case ItemType.CYAN:
                    return Cyan;
                case ItemType.PINK:
                    return Pink;
                default:
                    return White;
            }
        }
    }
}