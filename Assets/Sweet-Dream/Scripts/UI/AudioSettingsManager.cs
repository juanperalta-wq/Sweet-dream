using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer mainMixer;

    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string Master_Key = "MasterVolume";
    private const string Music_Key = "MusicVolume";
    private const string SFX_Key = "SFXVolume";

    private void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat(Master_Key, 1f);
        musicSlider.value = PlayerPrefs.GetFloat(Music_Key, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_Key, 1f);

        ApplyVolume(Master_Key, masterSlider.value);
        ApplyVolume(Music_Key, musicSlider.value);
        ApplyVolume(SFX_Key, sfxSlider.value);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value) => SetVolume(Master_Key, value);
    public void SetMusicVolume(float value) => SetVolume(Music_Key, value);
    public void SetSFXVolume(float value) => SetVolume(SFX_Key, value);

    private void SetVolume(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
        ApplyVolume(key, value);
    }

    private void ApplyVolume(string key, float sliderValue)
    {
        // El slider va de 0 a 1 (lineal), el Mixer espera dB (logarítmico).
        // 0.0001f evita log(0) = -infinito cuando el slider está en 0.
        float dB = sliderValue > 0.0001f ? Mathf.Log10(sliderValue) * 20f : -80f;
        mainMixer.SetFloat(key, dB);
    }

    public void ResetToDefaults()
    {
        masterSlider.value = 1f;
        musicSlider.value = 1f;
        sfxSlider.value = 1f;
        // El onValueChanged de cada slider ya dispara SetVolume() automáticamente
    }
}