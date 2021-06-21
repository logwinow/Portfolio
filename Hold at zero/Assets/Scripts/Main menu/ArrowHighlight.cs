using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArrowHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    Color startColor = Color.white;
    [SerializeField]
    Vector3 startScale;
    public Image Img { get; set; }
    [SerializeField]
    Color higlightColor;
    [SerializeField]
    float sizePercent;

    bool active = true;

    void Awake()
    {
        Img = GetComponent<Image>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (active)
            Img.rectTransform.localScale = startScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (active)
            Img.rectTransform.localScale = startScale * sizePercent;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (active)
            Img.color = startColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (active)
            Img.color = higlightColor;
    }

    public void Unactivate()
    {
        Img.raycastTarget = false;
        Img.color -= new Color(0, 0, 0, 0.5f);
        active = false;
    }

    public void Activate()
    {
        Img.raycastTarget = true;
        Img.color = startColor;
        Img.rectTransform.localScale = startScale;
        active = true;
    }
}
