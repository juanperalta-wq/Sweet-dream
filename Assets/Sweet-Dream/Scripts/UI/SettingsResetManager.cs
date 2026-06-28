using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsResetManager : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField] private Slider sensitivitySlider;

    [Header("Audio")]
    [SerializeField] private AudioSettingsManager audioSettingsManager;

    [Header("Language")]
    [SerializeField] private LanguageSelector languageSelector;
    [SerializeField] private TMP_Dropdown languageDropdown;

    public void ResetSettings()
    {
        // ---------------- Sensibilidad ----------------
        PlayerPrefs.SetFloat("MouseSensitivity", 1.5f);
        sensitivitySlider.value = 1.5f;

        MouseSensitivityManager mouseManager = FindFirstObjectByType<MouseSensitivityManager>();

        if (mouseManager != null)
        {
            mouseManager.ApplySensitivity();
        }

        // ---------------- Audio ----------------
        audioSettingsManager.ResetToDefaults();

        // ---------------- Idioma ----------------
        PlayerPrefs.SetInt("Language", 0); // Español

        languageDropdown.value = 0;
        languageDropdown.RefreshShownValue();
        languageSelector.ChangeLanguage(0);

        PlayerPrefs.Save();
        Debug.Log("Configuración restaurada a valores predeterminados.");
    }
}