using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFunctions : MonoBehaviour
{
    public static bool started = false;
    void Start()
    {
        if (BGStatus.status == BGStatus.Status.Opened)
        {
            Messenger<Action>.Broadcast(MyMenuEvents.CLOSE_DOORS, () => Messenger.Broadcast(MyMenuEvents.VISIBLE_MENU_ITEMS));
        }
        else
        {
            Messenger<BGController.DoorMovingMode>.Broadcast(MyMenuEvents.MOMENT_SWITCH_DOORS, BGController.DoorMovingMode.Close);
            Messenger.Broadcast(MyMenuEvents.VISIBLE_MENU_ITEMS);
        }

        if (PlayerPrefs.HasKey("language"))
        {
            LanguageStat.ChangeLanguage((Language)PlayerPrefs.GetInt("language"));
        }
        else
            Messenger.Broadcast(MyMenuEvents.REFRASH_LANGUAGE);
    }
}
