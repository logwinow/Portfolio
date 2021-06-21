using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageName : MonoBehaviour
{
    Text label;
    [SerializeField]
    List<TextLanguage> texts;

    void Awake()
    {
        Messenger.AddListener(MyMenuEvents.REFRASH_LANGUAGE, OnLanguageResresh);
        label = GetComponent<Text>();
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(MyMenuEvents.REFRASH_LANGUAGE, OnLanguageResresh);
    }

    void OnLanguageResresh()
    {
        label.text = texts.Find((t) => t.language == LanguageStat.Language).text;
    }
}

[Serializable]
public struct TextLanguage
{
    public string text;
    public Language language;
}
