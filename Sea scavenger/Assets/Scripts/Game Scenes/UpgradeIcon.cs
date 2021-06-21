using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeIcon : MonoBehaviour
{
    [SerializeField]
    private Color upgradedColor;

    private bool isUpgraded = false;
    private GameManager.ItemUI.Upgrade _upgrade;

    public bool IsUpgraded => isUpgraded;
    public GameManager.ItemUI.Upgrade Upgrade => _upgrade;

    public void Set(GameManager.ItemUI.Upgrade upgrade, 
        bool isUpgraded = false)
    {
        _upgrade = upgrade;
        
        if (isUpgraded)
            SetUpgraded();
    }
    
    public void SetUpgraded()
    {
        GetComponent<Image>().color = upgradedColor;

        isUpgraded = true;
    }
}
