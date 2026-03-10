using UnityEngine;
using System;

namespace ArrowFlowGame.Types
{
    [Serializable]
    public class ItemMaterials
    {
        public Material Red;
        public Material Green;
        public Material Yellow;
        public Material Blue;
        public Material Pink;
        public Material Cyan;
        public Material Brown;
        public Material Black;
        public Material OffWhite;
        public Material White;
        public Material DarkGreen;
        public Material Orange;

        public Material GetMaterial(ItemType type)
        {
            switch (type)
            {
                case ItemType.RED:
                    return Red;
                case ItemType.GREEN:
                    return Green;
                case ItemType.YELLOW:
                    return Yellow;
                case ItemType.BLUE:
                    return Blue;
                case ItemType.PINK:
                    return Pink;
                case ItemType.CYAN:
                    return Cyan;
                case ItemType.BROWN:
                    return Brown;
                case ItemType.BLACK:
                    return Black;
                case ItemType.OFFWHITE:
                    return OffWhite;
                case ItemType.WHITE:
                    return White;
                case ItemType.DARKGREEN:
                    return DarkGreen;
                case ItemType.ORANGE:
                    return Orange;
                default:
                    return null;
            }
        }
    }
}