using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class PostProcessingManager : Singleton<PostProcessingManager>
{
    [Header("REFERENCES")]
    public Volume PostProcessVolume;
    
    [Header("EXPOSURE SETTINGS")]
    public float DimmedExposure = -5f;
    public float NormalExposure = 0f;
    public float Duration = 0.5f;

    [Header("VIGNETTE SETTINGS")]
    [Tooltip("The highest intensity of the vignette during the danger state.")]
    public float MaxVignetteIntensity = 0.5f;
    [Tooltip("The lowest intensity the vignette drops to while breathing.")]
    public float MinBreathingIntensity = 0.25f;
    public float VignetteFadeDuration = 0.5f;
    public float VignetteBreathDuration = 1f;

    // Post-Processing Components
    private ColorAdjustments colorAdjustments;
    private Vignette vignette;

    // State Tracking
    private Tween vignetteTween;
    private bool isDangerState = false;

    protected override void Awake()
    {
        base.Awake();
        PostProcessVolume.profile.TryGet(out colorAdjustments);
        
        if (PostProcessVolume.profile.TryGet(out vignette))
        {
            vignette.intensity.overrideState = true;
            vignette.intensity.value = 0f;
        }
    }

    public void AnimateDimmedExposure()
    {
        if (colorAdjustments == null) return;
        DOTween.To(() => colorAdjustments.postExposure.value, x => colorAdjustments.postExposure.value = x, DimmedExposure, Duration);
    }

    public void AnimateNormalExposure()
    {
        if (colorAdjustments == null) return;
        DOTween.To(() => colorAdjustments.postExposure.value, x => colorAdjustments.postExposure.value = x, NormalExposure, Duration);
    }

    public void UpdateDangerVignette(float fillAmount)
    {
        if (vignette == null) return;

        bool shouldBeDanger = fillAmount > 0.7f;
        if (shouldBeDanger == isDangerState) return;

        isDangerState = shouldBeDanger;
        vignetteTween?.Kill();

        if (isDangerState)
        {
            Sequence dangerSequence = DOTween.Sequence();
            dangerSequence
            .Append
            (
                DOTween.To
                (
                    () => vignette.intensity.value, 
                    x => vignette.intensity.value = x, 
                    MaxVignetteIntensity, 
                    VignetteFadeDuration
                )
            );
            
            dangerSequence
            .Append
            (
                DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, MinBreathingIntensity, VignetteBreathDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
            );

            vignetteTween = dangerSequence;
        }
        else
        {
            vignetteTween = DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, 0f, VignetteFadeDuration);
        }
    }
}