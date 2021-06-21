using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoors : Interactable
{
    public bool CanExit
    {
        get
        {
            return GameManager.Instance.UnitsCount == 10;
        }
    }

    public override void Interact()
    {
        if (CanExit)
        {
            GameManager.Instance.EndGame();
            NextScene.StartLoading();
        }
    }
}
