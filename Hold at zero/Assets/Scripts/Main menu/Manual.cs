using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manual : MonoBehaviour
{
    [SerializeField]
    List<Sprite> pagesInRussian;
    [SerializeField]
    List<Sprite> pagesInEnglish;
    List<Sprite> pages;
    [SerializeField]
    Image outputTable;
    [SerializeField]
    ArrowHighlight leftArrow;
    [SerializeField]
    ArrowHighlight rightArrow;
    [SerializeField]
    ArrowHighlight closeButton;

    int currentPage = 0;

    void Awake()
    {
        Messenger.AddListener(MyMenuEvents.INVISIBLE_MENU_ITEMS, Close);
        Messenger.AddListener(MyMenuEvents.REFRASH_LANGUAGE, OnRefresh);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(MyMenuEvents.INVISIBLE_MENU_ITEMS, Close);
        Messenger.RemoveListener(MyMenuEvents.REFRASH_LANGUAGE, OnRefresh);
    }

    void Start()
    {
        OnRefresh();
    }

    public void Close()
    {
        if (closeButton != null)
            closeButton.Activate();
        gameObject.SetActive(false);
    }

    void OnRefresh()
    {
        switch (LanguageStat.Language)
        {
            case Language.Russian:
                pages = pagesInRussian;
                break;
            case Language.English:
                pages = pagesInEnglish;
                break;
        }

        outputTable.sprite = pages[currentPage];
    }

    public void Next()
    {
        if (currentPage + 1 < pages.Count)
        {
            currentPage++;
            outputTable.sprite = pages[currentPage];

            if (currentPage == pages.Count - 1)
            {
                if (rightArrow != null)
                    rightArrow.Unactivate();
            }

            if (currentPage == 1)
                if (leftArrow != null)
                    leftArrow.Activate();
        }
    }

    public void Back()
    {
        if (currentPage - 1 >= 0)
        {
            currentPage--;
            outputTable.sprite = pages[currentPage];

            if (currentPage == 0)
            {
                if (leftArrow != null)
                    leftArrow.Unactivate();
            }

            if (currentPage == pages.Count - 2)
                if (rightArrow != null)
                    rightArrow.Activate();
        }
    }
}
