using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public float pollingTime = 0.5f;

    private float timeUntilNextUpdate;
    private int frameCount;
    private int currentFps;

    private void Start()
    {
        timeUntilNextUpdate = pollingTime;
    }

    private void Update()
    {
        frameCount++;
        timeUntilNextUpdate -= Time.unscaledDeltaTime;

        if (timeUntilNextUpdate <= 0)
        {
            currentFps = (int)(frameCount / pollingTime);
            fpsText.text = currentFps.ToString();

            frameCount = 0;
            timeUntilNextUpdate = pollingTime;
        }
    }
}
