using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Custom.Patterns;
using  Custom.SmartCoroutines;

public class ContextMenu : Singleton<ContextMenu>
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _upgradeTitleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private ContextMenuItem _contextMenuItemPrefab;
    [SerializeField] private ContentSizeFitter _titleSizeFitter;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private float _delay;

    private Pool<ContextMenuItem> _contextMenuItemsPool;
    private ContentSizeFitter _sizeFitter;
    private SmartWaitingCoroutine _layoutingCor;

    protected override void Init()
    {
        _contextMenuItemsPool = new Pool<ContextMenuItem>(transform, _contextMenuItemPrefab);
        _sizeFitter = GetComponent<ContentSizeFitter>();
        _layoutingCor = new SmartWaitingCoroutine(this);
    }
    
    private void Start()
    { 
        gameObject.SetActive(false);
    }

    public void Set(SectionID itemUIiD, int upgradeNumber)
    {
        Set(GameManager.Instance.GetItemUI(itemUIiD), upgradeNumber);
    }

    public void Set(GameManager.ItemUI itemUI, int upgradeNumber)
    {
        GameManager.ItemUI.Upgrade upg;

        if (!itemUI.GetUpgrade(upgradeNumber, out upg))
        {
            _titleTMP.text = itemUI.Name;
            _upgradeTitleTMP.text = $"[Максимум]";
            _descriptionTMP.text = "Максимальное улучшение достигнуто.";
        }
        else
        {
            foreach (var r in upg.Resources)
            {
                _contextMenuItemsPool.GetAvailable().Set(r.ID, r.Count);
            }

            _titleTMP.text = itemUI.Name;
            _upgradeTitleTMP.text = $"[{upg.Name}]";
            _descriptionTMP.text = upg.Description;
        }

        _titleSizeFitter.SetLayoutVertical();
        _sizeFitter.SetLayoutVertical();
    }

    public void Close()
    {
        if (!gameObject.activeSelf)
            return;
        
        _layoutingCor.Stop();
        _contextMenuItemsPool.ReleaseAll();
        gameObject.SetActive(false);
    }

    public void SetOnPosition(Vector2 uiPos)
    {
        transform.position = uiPos + _offset;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void ShowAndSetWithDelay(Vector2 uiPos, GameManager.ItemUI itemUi, 
        int upgradeNumber, Action callback)
    {
        Set(itemUi, upgradeNumber);
        Show();
        transform.position = new Vector3(Screen.width, Screen.height) * 2;
        
        _layoutingCor.Start(_delay,
            methodAfter:
            delegate
            {
                callback?.Invoke();
            });
    }
}
