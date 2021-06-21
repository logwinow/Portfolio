using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    MenuItem menuItem;
    [SerializeField]
    GameObject manual;

    Text label;
    
    enum MenuItem
    {
        Start = 0,
        Exit = 1,
        Manual = 2,
        Tutor = 3
    }

    void Awake()
    {
        Messenger.AddListener(MyMenuEvents.INVISIBLE_MENU_ITEMS, InvisibleItem);
        label = GetComponentInChildren<Text>();
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(MyMenuEvents.INVISIBLE_MENU_ITEMS, InvisibleItem);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch(menuItem)
        {
            case MenuItem.Exit:
                Messenger.Broadcast(MyMenuEvents.INVISIBLE_MENU_ITEMS);
                Messenger<Action>.Broadcast(MyMenuEvents.OPEN_DOORS, () => Application.Quit());
                break;
            case MenuItem.Start:
                Messenger<Action>.Broadcast(MyMenuEvents.OPEN_DOORS, () => UnityEngine.SceneManagement.SceneManager.LoadScene(2));
                Messenger.Broadcast(MyMenuEvents.INVISIBLE_MENU_ITEMS);
                break;
            case MenuItem.Manual:
                manual.SetActive(true);
                break;
            case MenuItem.Tutor:
                Messenger<Action>.Broadcast(MyMenuEvents.OPEN_DOORS, () => UnityEngine.SceneManagement.SceneManager.LoadScene(3));
                Messenger.Broadcast(MyMenuEvents.INVISIBLE_MENU_ITEMS);
                break;
        }
    }

    void InvisibleItem()
    {
        gameObject.SetActive(false);
    }
}
