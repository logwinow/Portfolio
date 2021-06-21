using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipController : MonoBehaviour
{
    [SerializeField]
    Image background;
    [SerializeField]
    Image dialogImg;
    [SerializeField]
    List<TipDeclaration> tipsInRussian;
    [SerializeField]
    List<TipDeclaration> tipsInEnglish;

    List<TipDeclaration> currentTips;

    int currentTip = 0;

    void Awake()
    {
        Messenger.AddListener(MyGameEvents.NEXT_TIP, Next);
        Messenger.AddListener(MyGameEvents.SHOW_CUR_TIP, ShowCurrent);
        Messenger.AddListener(MyMenuEvents.REFRASH_LANGUAGE, ChangeLanguage);
        Messenger<Action>.AddListener(MyMenuEvents.CLOSE_DOORS, OnCloseDoors);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(MyGameEvents.NEXT_TIP, Next);
        Messenger.RemoveListener(MyGameEvents.SHOW_CUR_TIP, ShowCurrent);
        Messenger.RemoveListener(MyMenuEvents.REFRASH_LANGUAGE, ChangeLanguage);
        Messenger<Action>.RemoveListener(MyMenuEvents.CLOSE_DOORS, OnCloseDoors);
    }

    public void Next()
    {
        bool showAtTheEnd = false;

        if (currentTips[currentTip].impactFeature != TipDeclaration.ImpactSwitch.None)
        {
            background.raycastTarget = false;
            background.color -= new Color(0, 0, 0, 1f);

            dialogImg.color -= new Color(0, 0, 0, 1f);
            dialogImg.raycastTarget = false;

            showAtTheEnd = true;

            Messenger<TipDeclaration.ImpactSwitch>.Broadcast(MyGameEvents.TUTORIAL_FEATURE_CHANGER, currentTips[currentTip].impactFeature);
        }
        else
        {
            ShowCurrent();
        }

        if (currentTip == currentTips.Count - 1)
        {
            Messenger<Action>.Broadcast(MyMenuEvents.CLOSE_DOORS, () => UnityEngine.SceneManagement.SceneManager.LoadScene(1));
            background.gameObject.SetActive(false);
            dialogImg.gameObject.SetActive(false);
            return;
        }

        currentTip++;
        if (!showAtTheEnd)
            ShowCurrent();
    }

    void ShowCurrent()
    {
        background.gameObject.SetActive(true);
        dialogImg.gameObject.SetActive(true);


        background.raycastTarget = true;
        if (background.color.a <= 0)
            background.color += new Color(0, 0, 0, 1f);

        if (dialogImg.color.a <= 0)
            dialogImg.color += new Color(0, 0, 0, 1f);
        dialogImg.raycastTarget = true;

        dialogImg.sprite = currentTips[currentTip].sprite;
    }

    void ChangeLanguage()
    {
        switch(LanguageStat.Language)
        {
            case Language.Russian:
                currentTips = tipsInRussian;
                break;
            case Language.English:
                currentTips = tipsInEnglish;
                break;
        }
    }

    void OnCloseDoors(Action a)
    {
        this.gameObject.SetActive(false);
    }
}

[Serializable]
public class TipDeclaration
{
    public Sprite sprite;
    public ImpactSwitch impactFeature = ImpactSwitch.None;

    public enum ImpactSwitch
    {
        TemperatureInc = 0,
        Pressure = 1,
        REFR = 2,
        WASAC = 3,
        ICPC = 4,
        None = -1,
        TemperatureDecr = 5
    }
}
