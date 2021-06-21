using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DoubleClickableUI : MonoBehaviour, IPointerClickHandler
{
    private const float DOUBLE_CLICK_TIME = 0.2f;
    private float lastClickTime = 0;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if ((Time.time - lastClickTime) <= DOUBLE_CLICK_TIME)
        {
            OnDoubleClick(eventData);
        }
        else
            lastClickTime = Time.time;
    }

    protected abstract void OnDoubleClick(PointerEventData eventData);
}
