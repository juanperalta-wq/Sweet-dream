using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string Master_Key = "MasterVolume";
    private const string Music_Key = "MusicVolume";
    private const string SFX_Key = "SFXVolume";

    private void Start()
    {
        // Cargar valores guardados
        masterSlider.value = PlayerPrefs.GetFloat(Master_Key, 1f);
        musicSlider.value = PlayerPrefs.GetFloat(Music_Key, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_Key, 1f);

        // Aplicar volumen maestro inmediatamente
        AudioListener.volume = masterSlider.value;

        // Escuchar cambios del usuario
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;

        PlayerPrefs.SetFloat(Master_Key, value);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(Music_Key, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat(SFX_Key, value);
        PlayerPrefs.Save();
    }

    public void ResetToDefaults()
    {
        masterSlider.value = 1f;
        musicSlider.value = 1f;
        sfxSlider.value = 1f;

        PlayerPrefs.SetFloat(Master_Key, 1f);
        PlayerPrefs.SetFloat(Music_Key, 1f);
        PlayerPrefs.SetFloat(SFX_Key, 1f);

        PlayerPrefs.Save();

        AudioListener.volume = 1f;
    }
}