using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleFade : MonoBehaviour
{
    [SerializeField]
    float alphaDecrease = 4f;
    Image img;
    Text label;
    public float maxAlpha = 1f;

    enum Appearance
    {
        Fade = 1,
        Rise = -1
    }

    void Awake()
    {
        Messenger.AddListener(MyMenuEvents.INVISIBLE_MENU_ITEMS, Fade);
        Messenger.AddListener(MyMenuEvents.VISIBLE_MENU_ITEMS, Rise);

        if (GetComponent<Text>() != null)
            label = GetComponent<Text>();
        else
            img = GetComponent<Image>();
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(MyMenuEvents.INVISIBLE_MENU_ITEMS, Fade);
        Messenger.RemoveListener(MyMenuEvents.VISIBLE_MENU_ITEMS, Rise);
    }

    void Fade()
    {
        if (gameObject.activeInHierarchy == true)
            StartCoroutine(Fading(Appearance.Fade));
    }

    void Rise()
    {
        StartCoroutine(Fading(Appearance.Rise));
        
    }

    IEnumerator Fading(Appearance appear)
    {
        yield return null;

        if (label == null)
        {
            while (true)
            {
                Color c = img.color;
                c.a -= (int)appear * Time.deltaTime * alphaDecrease;
                img.color = c;

                if (appear == Appearance.Fade ? img.color.a <= 0 : img.color.a >= (maxAlpha == 0 ? 1f : maxAlpha))
                    break;

                yield return null;
            }
        }
        else
        {
            while (true)
            {
                Color c = label.color;
                c.a -= (int)appear * Time.deltaTime * alphaDecrease;
                label.color = c;

                if (appear == Appearance.Fade ? label.color.a <= 0 : label.color.a >= (maxAlpha == 0 ? 1f : maxAlpha))
                    break;

                yield return null;
            }
        }

        StartFunctions.started = true;
    }
}
