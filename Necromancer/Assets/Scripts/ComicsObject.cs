using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicsObject : MonoBehaviour
{
    public float GetAlpha()
    {
        if (GetComponent<Image>() != null)
        {
            return GetComponent<Image>().color.a;
        }
        else // text
        {
            return GetComponent<Text>().color.a;
        }
    }

    public void SetAlpha(float a)
    {
        if (GetComponent<Image>() != null)
        {
            GetComponent<Image>().color = new Color(1, 1, 1, a);
        }
        else // text
        {
            GetComponent<Text>().color = new Color(0, 0, 0, a); ;
        }
    }
}
