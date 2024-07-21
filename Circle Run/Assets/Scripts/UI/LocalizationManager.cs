using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using System.Xml.Schema;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    private static List<Locale> locale = new List<Locale>();

    private Language _language;
    public Language language
    {
        get => _language;
        set
        {
            if (value == _language)
                return;

            _language = value;
            SetLanguage();
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }
    public void Init()
    {
        Locale currentLocale = LocalizationSettings.SelectedLocale;
        locale = LocalizationSettings.AvailableLocales.Locales;

        switch (currentLocale.Identifier.Code)
        {
            case "ko":
                language = Language.Korea;
                break;
            case "en":
                language = Language.English;
                break;
            // case "ja":
            //     language = Language.Japane;
            //     break;
            // case "zh-TW":
            //     language = Language.China;
            //     break;
            default:
                language = Language.English;
                break;
        }
    }
    public static void ChangedTxt(string key, TextMeshProUGUI text)
    {
        try
        {
            LocalizeStringEvent local = text.GetComponent<LocalizeStringEvent>();
            local.enabled = true;
            local.StringReference.SetReference("Localization", key);
        }
        catch
        {
            text.text = "Localization Error";
        }
    }
    public void SetLanguage()
    {
        Debug.Log($"Selected language: {_language}");

        if (locale == null)
            locale = LocalizationSettings.AvailableLocales.Locales;

        LocalizationSettings.SelectedLocale = locale[(int)_language];
    }
    public int GetLanguage()
    {
        return (int)_language;
    }
}
public enum Language
{
    Korea,
    English,
    Japane,
    China
}