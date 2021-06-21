using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Custom.Extensions.UI;

public class StorageItem : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI titleTmpUGUI;
    [SerializeField]
    private TextMeshProUGUI countTmpUGUI;

    private int count;
    private SectionID id;

    public SectionID ID => id;
    public int Count
    {
        get => count;
        set
        {
            SetCount(value);

            count = value;
        }
    }

    public void Set(SectionID id, int count)
    {
        this.id = id;
        this.count = count;

        GameManager.Item _item = GameManager.Instance.GetItem(id);

        icon.sprite = _item.Prefab.GetComponent<SpriteRenderer>().sprite;
        icon.SetNativeSize();
        icon.rectTransform.FitInto(icon.transform.parent.GetComponent<RectTransform>());

        titleTmpUGUI.text = _item.Title;
        SetCount(count);
    }

    private void SetCount(int count)
    {
        countTmpUGUI.text = "x" + count.ToString();
    }
}
