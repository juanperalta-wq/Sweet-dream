using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySettings : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensitivityText;

    private const string SensitivityKey = "MouseSensitivity";

    private void Start()
    {
        float sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 1.5f);

        sensitivitySlider.value = sensitivity;

        UpdateSensitivity(sensitivity);

        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
    }

    private void UpdateSensitivity(float value)
    {
        sensitivityText.text = $"x{value:F1}";

        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();

        MouseSensitivityManager manager = FindFirstObjectByType<MouseSensitivityManager>();

        if (manager != null)
        {
            manager.ApplySensitivity();
        }
    }
    public static float GetSensitivity()
    {
        return PlayerPrefs.GetFloat(SensitivityKey, 1f);
    }
}