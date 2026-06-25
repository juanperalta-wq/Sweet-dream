using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using MoreMountains.Tools;
public class PlayerStats : MonoBehaviour
{

    public static PlayerStats Instance { get; private set; }

    [TabGroup("Stats"), LabelWidth(80)]
    [PropertyRange(0f, 100f)]
    [SerializeField] private float sanity = 100f;

    [TabGroup("Stats"), LabelWidth(120)]
    [SerializeField] private float drainAmount = 1f;


    public MMProgressBar TargetProgressBar;

    void ChangeBarValue()
    {
        TargetProgressBar.UpdateBar(Sanity, 0f, 100f);
    }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(SanityDrainRoutine());
    }
    private void FixedUpdate()
    {
        ChangeBarValue();
    }

    private IEnumerator SanityDrainRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Sanity -= drainAmount;
        }
    }

    #region Getters and Setters
    public float Sanity
    {
        get => sanity;
        set
        {
            sanity = Mathf.Clamp(value, 0, 100);
            TargetProgressBar?.UpdateBar(sanity, 0f, 100f);
        }
    }
    #endregion
}
