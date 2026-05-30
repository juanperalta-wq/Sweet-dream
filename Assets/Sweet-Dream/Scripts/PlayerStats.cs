using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerStats : MonoBehaviour
{
    private Color HealthColor => health > 60 ? Color.green : health > 30 ? Color.yellow : Color.red;
    public static PlayerStats Instance { get; private set; }

    [TabGroup("Stats"), LabelWidth(80)]
    [PropertyRange(0f, 100f)]
    [SerializeField] private float sanity = 100f;

    [TabGroup("Stats"), LabelWidth(120)]
    [Tooltip("Cuántos puntos de cordura pierde el jugador por segundo de forma pasiva")]
    [SerializeField] private float pasiveSanityDrain = 0.5f;

    [TabGroup("Stats"), LabelWidth(80)]
    [ProgressBar(0, 100, ColorGetter = "HealthColor")]
    [SerializeField] private float health = 100f;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        SanityDrain();
    }

    public void SanityDrain()
    {
        if (sanity > 0)
            RemoveSanity(pasiveSanityDrain * Time.deltaTime);
    }

    public void AddHealth(float amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0f, 100f);
    }

    public void RemoveHealth(float amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0f, 100f);
    }

    public void AddSanity(float amount)
    {
        sanity += amount;
        sanity = Mathf.Clamp(sanity, 0f, 100f);
    }

    public void RemoveSanity(float amount)
    {
        sanity -= amount;
        sanity = Mathf.Clamp(sanity, 0f, 100f);
    }

    #region Getters
    public float Sanity => sanity;
    public float Health => health;
    #endregion
}