using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImpactField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    ManualOverload manual;
    [SerializeField]
    float alphaValue = .5f;
    [SerializeField]
    GameObject icon;

    Image img;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        manual.OnPointerEnter(eventData);
        icon.SetActive(false);
        img.color -= new Color(0, 0, 0, 1f);
        img.raycastTarget = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //manual.OnPointerExit(() =>
        //{
        //    icon.SetActive(true);

        //    Color tmp = img.color;
        //    tmp.a = alphaValue;
        //    img.color = tmp;
        //}
        //);
    }
}
