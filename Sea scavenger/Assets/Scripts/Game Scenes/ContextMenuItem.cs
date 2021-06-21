using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;
using  TMPro;
using  Custom.Extensions.UI;

public class ContextMenuItem : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _countTMP;

    public void Set(SectionID resourceID, int count)
    {
        GameManager.Item item = GameManager.Instance.GetItem(resourceID);
        _icon.sprite = item.Prefab.GetComponent<SpriteRenderer>().sprite;
        _icon.SetNativeSize();
        _icon.rectTransform.FitInto(_icon.transform.parent.gameObject.GetComponent<RectTransform>());
        _countTMP.text = "x" + count.ToString();
    }
}
