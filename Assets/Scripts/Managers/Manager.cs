using UnityEngine;

public class Manager: Singleton<Manager>
{
    [Header("Game Settings")]
    public bool TestMode = false;
    public int ForceTestLevel = 26;

    [Range(-1,120)]
    public int TargetFPS = 60;

    [Header("Fake Infinite Levels")]
    public int MaxLvl = 40;
    public int MinLvl = 4;
}