using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LanguageIcon : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    Language thisIsLanguage;
    [SerializeField]
    LanguageButton languageButton;

    void Start()
    {
        StartCoroutine(DelayFrame(() => { if (LanguageStat.Language != thisIsLanguage) { gameObject.GetComponent<TitleFade>().maxAlpha = .5f; } })); ;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        languageButton.OnClick(thisIsLanguage);
    }

    IEnumerator DelayFrame(Action a)
    {
        yield return null;
        if (a != null)
            a.Invoke();
    }
}
