using System;

namespace ArrowFlowGame.Types
{
    [Serializable]
    public enum ItemType
    {
        NONE,
        RED,
        GREEN,
        DARKGREEN,
        BLUE,
        WHITE,
        ORANGE,
        CYAN,
        PINK
    }

    public enum Hits
    {
        ONE = 1,
        TEN = 10,
        FIFTEEN = 15,
        TWENTY = 20,
        TWENTYFIVE = 25,
        THIRTY = 30,
    }

    public enum KEYED
    {
        YES,
        NO
    }
    public enum GIANT
    {
        YES,
        NO
    }

    public enum CrowdElementType
    {
        PERSON,
        KEY
    }

}