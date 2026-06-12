using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
public class PlayerStats : MonoBehaviour
{
    private Color HealthColor => health > 60 ? Color.green : health > 30 ? Color.yellow : Color.red;
    public static PlayerStats Instance { get; private set; }

    [TabGroup("Stats"), LabelWidth(80)]
    [PropertyRange(0f, 100f)]
    [SerializeField] private float sanity = 100f;

    [TabGroup("Stats"), LabelWidth(120)]
    [SerializeField] private float drainInterval = 1f;

    [TabGroup("Stats"), LabelWidth(120)]
    [SerializeField] private float drainAmount = 1f;

    [TabGroup("Stats"), LabelWidth(80)]
    [ProgressBar(0, 100, ColorGetter = "HealthColor")]
    [SerializeField] private float health = 100f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(SanityDrainRoutine());
    }

    private IEnumerator SanityDrainRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(drainInterval);
            Sanity -= drainAmount;
        }
    }

    #region Getters and Setters
    public float Sanity
    {
        get => sanity;
        set => sanity = Mathf.Clamp(value, 0, 100);
    }

    public float Health
    {
        get => health;
        set => health = Mathf.Clamp(value, 0, 100);
    }
    #endregion
}