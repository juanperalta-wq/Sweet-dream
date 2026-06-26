using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSelector : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown languageDropdown;

    private IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        int savedLanguage = PlayerPrefs.GetInt("Language", 2);

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[savedLanguage];

        languageDropdown.value = savedLanguage;
        languageDropdown.RefreshShownValue();

        languageDropdown.onValueChanged.AddListener(ChangeLanguage);
    }

    public void ChangeLanguage(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        PlayerPrefs.SetInt("Language", index);
        PlayerPrefs.Save();
    }
}