using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public enum Language
{
    Russian = 0,
    English = 1
}

public class LanguageButton : MonoBehaviour
{
    [SerializeField]
    Image rusIcon;
    [SerializeField]
    Image enIcon;

    void OnDestroy()
    {
        PlayerPrefs.SetInt("language", (int)LanguageStat.Language);
    }

    public void OnClick(Language l)
    {
        Color c;

        if (LanguageStat.Language != l)
        {
            switch (l)
            {
                case Language.English:
                    c = enIcon.color;
                    c.a = 1f;
                    enIcon.color = c;

                    c = rusIcon.color;
                    c.a = .5f;
                    rusIcon.color = c;
                    break;
                case Language.Russian:
                    c = rusIcon.color;
                    c.a = 1f;
                    rusIcon.color = c;

                    c = enIcon.color;
                    c.a = .5f;
                    enIcon.color = c;
                    break;
            }
            LanguageStat.ChangeLanguage(l);
        }
    }
}

public static class LanguageStat
{
    public static void ChangeLanguage(Language l)
    {
        language = l;
        Messenger.Broadcast(MyMenuEvents.REFRASH_LANGUAGE);
    }

    public static Language Language
    {
        get
        {
            return language;
        }
    }
    static Language language = Language.English;
}
