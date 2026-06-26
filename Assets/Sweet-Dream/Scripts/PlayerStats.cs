using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [TabGroup("Stats"), LabelWidth(80)]
    [PropertyRange(0f, 100f)]
    [SerializeField] private float sanity = 100f;

    [TabGroup("Stats"), LabelWidth(120)]
    [SerializeField] private float drainAmount = 1f;

    [TabGroup("Effects")]
    [SerializeField] private Volume postProcessVolume;

    [TabGroup("Effects")]
    [SerializeField] private MMF_Player sanityScareAt50;

    [TabGroup("Effects")]
    [SerializeField] private MMF_Player sanityScareAt25;

    public MMProgressBar TargetProgressBar;

    private ColorAdjustments colorAdjustments;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;
    private DepthOfField depthOfField;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet(out colorAdjustments);
            postProcessVolume.profile.TryGet(out vignette);
            postProcessVolume.profile.TryGet(out chromaticAberration);
            postProcessVolume.profile.TryGet(out lensDistortion);
            postProcessVolume.profile.TryGet(out depthOfField);
        }

        StartCoroutine(SanityDrainRoutine());
    }

    private void FixedUpdate()
    {
        ApplySanityEffects();
    }

    private void ApplySanityEffects()
    {
        // Efectos generales desde 100 hasta 0
        float sanityT = sanity / 100f;

        if (colorAdjustments != null)
            colorAdjustments.saturation.value = Mathf.Lerp(-100f, 0f, sanityT);

        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(0.7f, 0f, sanityT);

        // Efectos que arrancan desde 70 de cordura
        float effectT = Mathf.InverseLerp(70f, 0f, sanity);

        if (chromaticAberration != null)
            chromaticAberration.intensity.value = Mathf.Lerp(0f, 1f, effectT);

        if (lensDistortion != null)
            lensDistortion.intensity.value = Mathf.Lerp(0f, -0.5f, effectT);

        // sin blur por encima de 70, blur máximo en 0
        if (depthOfField != null)
        {
            depthOfField.focalLength.value = Mathf.Lerp(10f, 115.9f, effectT);
            depthOfField.aperture.value = Mathf.Lerp(22f, 5.6f, effectT);
        }
    }

    private IEnumerator SanityDrainRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Sanity -= drainAmount;
        }
    }

    public float Sanity
    {
        get => sanity;
        set
        {
            float previous = sanity;
            sanity = Mathf.Clamp(value, 0, 100);
            TargetProgressBar?.UpdateBar(sanity, 0f, 100f);

            if (previous > 50f && sanity <= 50f)
                sanityScareAt50?.PlayFeedbacks();

            if (previous > 25f && sanity <= 25f)
                sanityScareAt25?.PlayFeedbacks();
        }
    }
}