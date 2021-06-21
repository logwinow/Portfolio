using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<Image>().color.a > 0)
            AudioManager.Instance.PlayOneShot(AudioManager.BUTTON_HIGHLIGHTED);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GetComponent<Image>().color.a > 0)
            AudioManager.Instance.PlayOneShot(AudioManager.BUTTON_CLICKED);
    }
}
