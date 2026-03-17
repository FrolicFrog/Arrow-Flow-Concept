using System;

namespace ArrowFlowGame.Types
{
    [Serializable]
    public enum ItemType
    {
        NONE,
        RED,
        GREEN,
        YELLOW,
        BLUE,
        PINK,
        CYAN,
        BROWN,
        BLACK,
        OFFWHITE,
        WHITE,
        DARKGREEN,
        ORANGE
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

    public enum GameState
    {
        NOT_STARTED,
        STARTED,
        FAILED,
        COMPLETED
    }

    public enum LABELTYPE
    {
        DEFAULT,
        ID,
        HITS,
        KEYED,
        GIANT,
    }

}