using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMenuController : PauseMenuController
{
    public void ExitButton()
    {
        GameManager.Instance.Exit();
    }
}
