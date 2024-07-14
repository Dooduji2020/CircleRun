using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    private static List<Locale> locale = new List<Locale>();
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
    public static void SetLanguage(Language language)
    {
        Debug.Log($"Selected language: {language}");

        if (locale == null)
            locale = LocalizationSettings.AvailableLocales.Locales;
        
        LocalizationSettings.SelectedLocale = locale[(int)language];
    }
}
public enum Language
{
    Korea,
    English,
    Japane,
    China
}