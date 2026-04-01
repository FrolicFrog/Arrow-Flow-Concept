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
        
        // Extract components from the volume profile
        PostProcessVolume.profile.TryGet(out colorAdjustments);
        
        if (PostProcessVolume.profile.TryGet(out vignette))
        {
            // Ensure the override state is active, and set initial value to 0
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

    // Call this from UIManager or BeltManager instead of the UI Image logic
    public void UpdateDangerVignette(float fillAmount)
    {
        if (vignette == null) return; // Safety check in case the profile lacks a Vignette

        bool shouldBeDanger = fillAmount > 0.7f;

        // If state hasn't changed, do nothing
        if (shouldBeDanger == isDangerState) return;

        isDangerState = shouldBeDanger;

        // Kill any ongoing vignette fade or breathing loop
        vignetteTween?.Kill();

        if (isDangerState)
        {
            // FADE IN & BREATHE
            Sequence dangerSequence = DOTween.Sequence();
            
            // 1. Smoothly fade up to Max intensity
            dangerSequence.Append(DOTween.To(
                () => vignette.intensity.value, 
                x => vignette.intensity.value = x, 
                MaxVignetteIntensity, 
                VignetteFadeDuration));
            
            // 2. Loop endlessly between MinBreathingIntensity and MaxVignetteIntensity
            dangerSequence.Append(DOTween.To(
                () => vignette.intensity.value, 
                x => vignette.intensity.value = x, 
                MinBreathingIntensity, 
                VignetteBreathDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo));

            vignetteTween = dangerSequence;
        }
        else
        {
            // FADE OUT
            // Smoothly fade back down to 0 intensity
            vignetteTween = DOTween.To(
                () => vignette.intensity.value, 
                x => vignette.intensity.value = x, 
                0f, 
                VignetteFadeDuration);
        }
    }
}