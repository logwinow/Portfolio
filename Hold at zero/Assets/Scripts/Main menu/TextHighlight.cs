using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextHighlight : MonoBehaviour
{
    [SerializeField]
    Color enterColor = Color.white;
    Color normalColor;
    [SerializeField]
    int pressedFontSize = 140;
    int normalFontSize;

    Text label;
    Image img;

    void Awake()
    {
        label = GetComponentInChildren<Text>();
        img = GetComponentInChildren<Image>();
        img.enabled = false;

        normalColor = label.color + new Color(0, 0, 0, 1f);
        normalFontSize = label.fontSize;
    }

    public void OnEnter()
    {
        if (StartFunctions.started)
        {
            label.color = enterColor;
            img.enabled = true;
        }
    }
    public void OnExit()
    {
        if (StartFunctions.started)
        {
            label.color = normalColor;
            img.enabled = false;
        }
    }
    public void OnDown()
    {
        if (StartFunctions.started)
            label.fontSize = pressedFontSize;
    }
    public void OnUp()
    {
        if (StartFunctions.started)
            label.fontSize = normalFontSize;
    }
}
