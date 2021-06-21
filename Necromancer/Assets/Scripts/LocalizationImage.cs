using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationImage : MonoBehaviour
{
    [SerializeField]
    bool setNative = false;
    [SerializeField]
    Sprite sprRus;
    [SerializeField]
    Sprite sprEng;

    private void Start()
    {
        if (Language.laguage == Language.LaguageType.Russian)
        {
            Image img = GetComponent<Image>();
            img.sprite = sprRus;
            if (setNative)
                img.SetNativeSize();
        }
        else // eng
        {
            Image img = GetComponent<Image>();
            img.sprite = sprEng;
            if (setNative)
                img.SetNativeSize();
        }
    }
}
