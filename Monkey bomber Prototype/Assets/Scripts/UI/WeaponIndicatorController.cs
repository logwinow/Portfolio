using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponIndicatorController : MonoBehaviour
{
    [SerializeField] private GameObject _background;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _countTMP;
    
    private void Start()
    {
        RefreshIndicator(null, 0);
    }

    public void RefreshIndicator(ItemData itemData, int count)
    {
        if (itemData == null)
        {
            SetActiveGUI(false);
            
            return;
        }
        
        SetActiveGUI(true);
        
        _icon.color = itemData.IconColor;

        if (!itemData.IsStackable)
        {
            _countTMP.gameObject.SetActive(false);
            
            return;
        }
        
        _countTMP.text = count.ToString();
    }

    private void SetActiveGUI(bool value)
    {
        _background.SetActive(value);
        _icon.gameObject.SetActive(value);
        _countTMP.gameObject.SetActive(value);
    }
}
