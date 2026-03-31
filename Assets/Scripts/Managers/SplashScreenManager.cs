using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManaer : MonoBehaviour
{
    [Header("SETTINGS")]
    public float SplashScreenDuration = 2f;
    public readonly string MainSceneName = "Game";

    private void Start()
    {
        DOVirtual.DelayedCall(SplashScreenDuration, () => SceneManager.LoadScene(MainSceneName));
    }
}
