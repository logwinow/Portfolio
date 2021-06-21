using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Custom.Extensions.UI;

public class InventoryItem : DoubleClickableUI
{
    [SerializeField]
    private Image img;
    
    public bool IsAvailable { get; set; }

    protected override void OnDoubleClick(PointerEventData eventData)
    {
        if (IsAvailable)
            return;
        
        PlayerController.Instance.Inventory.Remove(gameObject);
    }

    public void Set(Sprite spr)
    {
        img.sprite = spr;
        img.SetNativeSize();
        img.rectTransform.FitInto(img.transform.parent.GetComponent<RectTransform>());
    }

    public void SetImageActive(bool active)
    {
        img.enabled = active;
    }
}
