using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class PostProcessingManager : Singleton<PostProcessingManager>
{
    [Header("REFERENCES")]
    public Volume PostProcessVolume;
    
    [Header("SETTINGS")]
    public float DimmedExposure = -5f;
    public float NormalExposure = 0f;
    public float Duration = 0.5f;

    private ColorAdjustments colorAdjustments;

    protected override void Awake()
    {
        base.Awake();
        PostProcessVolume.profile.TryGet(out colorAdjustments);
    }

    public void AnimateDimmedExposure()
    {
        DOTween.To(() => colorAdjustments.postExposure.value, x => colorAdjustments.postExposure.value = x, DimmedExposure, Duration);
    }

    public void AnimateNormalExposure()
    {
        DOTween.To(() => colorAdjustments.postExposure.value, x => colorAdjustments.postExposure.value = x, NormalExposure, Duration);
    }
}