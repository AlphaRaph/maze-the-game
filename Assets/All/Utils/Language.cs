using UnityEngine;
using System.Collections.Generic;

public enum Language
{
    English,
    French
}

public static class Languages
{
    private static List<LanguageText> texts = new List<LanguageText>();

    public static void AddLanguageText(LanguageText text)
    {
        texts.Add(text);
    }

    public static void UpdateAllLanguageTexts ()
    { 
        Language currentLanguage = GetCurrentOne();
        Debug.Log($"Je mets à jour tous les textes à la langue {currentLanguage}");
        foreach (LanguageText text in texts)
        {
            Debug.Log(currentLanguage);
            text.ShowLanguage(currentLanguage);
        }
    }

    public static Language GetCurrentOne()
    {
        return SaveSystem.GetSettings().language;
    }

    public static void SetCurrentOne(Language language)
    {
        if (language != GetCurrentOne())
        {
            Debug.Log(language);
            Settings settings = SaveSystem.GetSettings();
            settings.language = language;
            SaveSystem.SaveSettings(settings);
            UpdateAllLanguageTexts();
        }
    }
}
